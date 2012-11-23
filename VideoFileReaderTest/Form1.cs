using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FTimer = System.Windows.Forms.Timer;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;

namespace VideoFileReaderTest
{
    public partial class Form1 : Form
    {
        private Capture _capture;
        private FTimer mTimer;
        private int para1 = 57;
        private double para2 = 3;
        private List<Rectangle> ratRect = new List<Rectangle>();
        private List<Rectangle> redCircles = new List<Rectangle>();
        private int lastLocation = 0;
        private double lastFrame = 0;
        public enum DIRECTION
        {
            UNKNOWN,
            LEFT2RIGHT,
            RIGHT2LEFT,
        };
        private DIRECTION walkDir = DIRECTION.UNKNOWN;

        private double
            totalFrames,
            FPS,
            currentFrame;

        private Image<Gray, byte> bgImage;

        public Form1()
        {
            InitializeComponent();
            tableLayoutPanel1.SetColumnSpan(pictureBox1, 2);
        }

        #region Private Static Methods

        private static Image<Gray, byte> getBackgroundImage(Capture _capture)
        {
            Image<Bgr, byte> frame = _capture.QueryFrame();
            return frame.Convert<Gray, byte>();
        }

        private static Image<Gray, byte> getRatContour(
            Image<Bgr, byte> frame,
            Image<Gray, byte> bgImage,
            int para1,
            int para2,
            out List<Rectangle> ratRect)
        {
            ratRect = new List<Rectangle>();
            Image<Gray, byte> gImage = frame.Convert<Gray, byte>();
            gImage = gImage.Sub(bgImage);
            gImage = preProcessImage(gImage, para1, para2);
            ratRect = findContourRects(gImage, 10000);
            return gImage;
        }

        private static Image<Gray, byte> getRedCircle(
            Image<Bgr, byte> frame,
            out List<Rectangle> redCircles)
        {
            redCircles = new List<Rectangle>();
            Image<Hsv, byte> hsvImage = frame.Convert<Hsv, byte>();
            Image<Gray, byte>[] channels = hsvImage.Split();
            Image<Gray, byte> HueImage = channels[0].InRange(new Gray(170), new Gray(180));
            Image<Gray, byte> SatImage = channels[1].InRange(new Gray(70), new Gray(120));
            Image<Gray, byte> ValueImage = channels[2].InRange(new Gray(128), new Gray(255));
            Image<Gray, byte> cImage;
            //cImage = HueImage;
            cImage = HueImage.And(ValueImage);
            cImage = cImage.And(SatImage);

            cImage = cImage.Erode(1);
            cImage = cImage.Dilate(10);
            
            redCircles = findContourRects(cImage, 0);

            return cImage;
        }

        private static Image<Gray, byte> preProcessImage(
            Image<Gray, byte> gImage,
            int para1,
            int para2)
        {   
            //Image<Gray, byte> gImage = frame.Convert<Gray, byte>();
            //gImage = gImage.ConvertScale<byte>(0.25, 0);
            //Image<Gray, byte> gImage2 = gImage.Clone();
            //gImage2 = gImage2.SmoothBlur((int)para2, (int)para2);
            //gImage2 = gImage2.Not();
            //gImage = gImage.AddWeighted(gImage2, 1, 0.5, 0);
            //gImage._EqualizeHist();
            gImage = gImage.Resize(0.25, INTER.CV_INTER_NN);
            gImage = gImage.SmoothBlur(11, 11);
            gImage = gImage.ThresholdBinary(new Gray(para1), new Gray(255));


            //gImage = gImage.ThresholdAdaptive(
            //    new Gray(255),
            //    Emgu.CV.CvEnum.ADAPTIVE_THRESHOLD_TYPE.CV_ADAPTIVE_THRESH_MEAN_C,
            //    Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY,
            //    para1,
            //    new Gray(para2)
            //);

            gImage = gImage.MorphologyEx(
                new StructuringElementEx(
                    3,
                    (int)para2,
                    1,
                    (int)para2 / 2,
                    Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE),
                    Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN,
                    1);

            gImage = gImage.Resize(4, INTER.CV_INTER_NN);
            return gImage;
        }

        private static List<Rectangle> findContourRects(Image<Gray, byte> gImage, int minArea)
        {
            List<Rectangle> ratRect = new List<Rectangle>();
            Contour<Point> contours = gImage.FindContours();
            ratRect = findContours(contours, minArea);
            return ratRect;
        }

        private static List<Rectangle> findContours(Contour<Point> contours, int minArea)
        {
            List<Rectangle> ratRect = new List<Rectangle>();
            for (; contours != null; contours = contours.HNext)
            {
                if (contours.Area > minArea)
                {
                    ratRect.Add(contours.BoundingRectangle);
                }
            }
            return ratRect;
        }

        #endregion

        #region Thread(Timer) Handler

        private void timer_Tick(object sender, EventArgs e)
        {
            Image<Bgr, byte> frame = _capture.QueryFrame();
            if (frame != null)
            {
                currentFrame = _capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES);
                if ((int)currentFrame % 5 != 0)
                    return;
                trackBar3.Value = (int)currentFrame;
                Console.WriteLine("fn: " + currentFrame);

                Image<Gray, byte> gImage = getRatContour(
                    frame,
                    bgImage,
                    para1,
                    (int)para2,
                    out ratRect);
                pictureBox2.Image = gImage.ToBitmap();

                Image<Gray, byte> cImage = getRedCircle(frame, out redCircles);
                pictureBox3.Image = cImage.ToBitmap();

                foreach (Rectangle rect in redCircles)
                {
                    Rectangle bigRect = new Rectangle(
                        rect.Left - rect.Width,
                        rect.Top - rect.Height,
                        rect.Width * 3,
                        rect.Height * 3);
                    frame.Draw(bigRect, new Bgr(255, 0, 255), 4);
                }

                foreach (Rectangle rect in ratRect)
                {
                    frame.Draw(rect, new Bgr(0, 255, 255), 4);
                }

                if (ratRect.Count > 0)
                {
                    
                    double spendTime = (currentFrame - lastFrame) / FPS;
                    
                    int maxArea = int.MinValue;
                    Rectangle currentLocation = Rectangle.Empty;
                    foreach (Rectangle rect in ratRect)
                    {
                        int tempArea = rect.Width * rect.Height;
                        if (tempArea > maxArea)
                        {
                            maxArea = tempArea;
                            currentLocation = rect;
                        }
                    }
                    if (walkDir == DIRECTION.UNKNOWN)
                    {
                        int center = (currentLocation.Left + currentLocation.Right) / 2;
                        if (center < frame.Width / 2)
                        {
                            walkDir = DIRECTION.LEFT2RIGHT;
                        }
                        else
                        {
                            walkDir = DIRECTION.RIGHT2LEFT;
                        }
                    }
                    double speed = 0;
                    if (walkDir == DIRECTION.LEFT2RIGHT)
                    {
                        speed = ((currentLocation.Right - lastLocation) / spendTime);
                        lastLocation = currentLocation.Right;
                    }
                    else
                    {
                        speed = ((currentLocation.Left - lastLocation) / spendTime);
                        lastLocation = currentLocation.Left;
                    }
                    speed = Math.Round(speed, 2);
                    txSpeed.Text = "Instant Velocity: " + speed.ToString() + " pixel/s";
                    
                    lastFrame = currentFrame;
                }

                pictureBox1.Image = frame.ToBitmap();
            }
            else
            {
                _capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, 0);
                frame = _capture.QueryFrame();
                pictureBox1.Image = frame.ToBitmap();
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                currentFrame = 0;
                trackBar3.Value = 0;
                mTimer.Stop();
                txSpeed.Text = "Instant Velocity:";
            }
        }

        #endregion

        #region Event Handler

        private void onLoadFile(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                stopTimer();
                _capture = new Capture(ofd.FileName);
                Image<Bgr, byte> frame = _capture.QueryFrame();
                pictureBox1.Image = frame.ToBitmap();
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                _capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, 0);
                bgImage = getBackgroundImage(_capture);
                _capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, 0);
                totalFrames = _capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_COUNT);
                FPS = _capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_FPS);
                currentFrame = _capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES);
                Console.WriteLine("fn: " + currentFrame);
                trackBar3.Maximum = (int)totalFrames - 1;
                trackBar3.Minimum = 0;
                trackBar3.Value = 0;

                txSpeed.Text = "Instant velocity:";
                walkDir = DIRECTION.UNKNOWN;

                button2.Enabled = true;
                button3.Enabled = true;
                trackBar3.Enabled = true;
            }
        }

        private void stopTimer()
        {
            if (mTimer == null)
                return;
            if (!mTimer.Enabled)
                return;

            mTimer.Stop();
        }

        private void onParam1_Scroll(object sender, EventArgs e)
        {
            try
            {
                para1 = ((TrackBar)sender).Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void onParam2_Scroll(object sender, EventArgs e)
        {
            para2 = (double)((TrackBar)sender).Value;
            para2 = (int)para2 % 2 == 0 ? para2 + 1 : para2;
        }

        private void onSeekbar_Scroll(object sender, EventArgs e)
        {
            currentFrame = (double)trackBar3.Value;
            _capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, currentFrame);
            pictureBox1.Image = _capture.QueryFrame().ToBitmap();
        }

        private void onPlay_Click(object sender, EventArgs e)
        {
            _capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, currentFrame);
            mTimer = new FTimer();

            mTimer.Interval = (int)(1000 / FPS / 5);
            mTimer.Tick += timer_Tick;
            mTimer.Start();
        }

        private void onStop_Click(object sender, EventArgs e)
        {
            mTimer.Stop();
        }

        #endregion

    }
}
