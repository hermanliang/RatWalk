using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;

namespace VideoFileReaderTest
{
    public partial class Form1 : Form
    {
        private Capture _capture;
        private Timer mTimer;
        private int para1 = 57;
        private double para2 = 17;

        private double
            totalFrames,
            FPS,
            currentFrame;

        private Image<Gray, byte> bgImage;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _capture = new Capture(ofd.FileName);

                getBackgroundImage();

                totalFrames = _capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_COUNT);
                FPS = _capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_FPS);
                currentFrame = _capture.GetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES);
                Console.WriteLine("fn: " + currentFrame);
                trackBar3.Maximum = (int)totalFrames - 1;
                trackBar3.Minimum = 0;

            }
        }

        private void getBackgroundImage()
        {
            Image<Bgr, byte> frame = _capture.QueryFrame();
            pictureBox1.Image = frame.ToBitmap();

            bgImage = preProcessImage(frame);
        }

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

                Image<Gray, byte> gImage = getRatContour(frame);
                //pictureBox1.Image = gImage.ToBitmap();

                Image<Gray, byte> cImage = getRedCircle(frame);
                foreach (Rectangle rect in redCircles)
                {
                    Rectangle bigRect = new Rectangle(
                        rect.Left - rect.Width,
                        rect.Top - rect.Height,
                        rect.Width * 3,
                        rect.Height * 3);
                    frame.Draw(bigRect, new Bgr(255, 0, 255), 2);
                }

                foreach (Rectangle rect in ratRect)
                {
                    frame.Draw(rect, new Bgr(0, 255, 255), 2);
                }

                //pictureBox1.Image = cImage.ToBitmap();

                pictureBox1.Image = frame.ToBitmap();
            }
            else
            {
                _capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, 0);
                frame = _capture.QueryFrame();
                pictureBox1.Image = frame.ToBitmap();
                currentFrame = 0;
                trackBar3.Value = 0;
                mTimer.Stop();
            }
        }

        private Image<Gray, byte> getRatContour(Image<Bgr, byte> frame)
        {
            Image<Gray, byte> gImage = preProcessImage(frame);
            gImage = gImage.Sub(bgImage);
            return gImage;
        }

        private List<Rectangle> redCircles = new List<Rectangle>();

        private Image<Gray, byte> getRedCircle(Image<Bgr, byte> frame)
        {
            Image<Hsv, byte> hsvImage = frame.Convert<Hsv, byte>();
            Image<Gray, byte>[] channels = hsvImage.Split();
            Image<Gray, byte> HueImage = channels[0].InRange(new Gray(170), new Gray(180));
            Image<Gray, byte> SatImage = channels[1].InRange(new Gray(70), new Gray(120));
            Image<Gray, byte> ValueImage = channels[2].InRange(new Gray(200), new Gray(255));
            Image<Gray, byte> cImage = HueImage.And(ValueImage);
            cImage = cImage.And(SatImage);

            cImage = cImage.Dilate(4);
            cImage = cImage.Erode(5);

            Contour<Point> contours = cImage.FindContours();

            findRedPosition(contours);
            
            
            return cImage;
        }

        private void findRedPosition(Contour<Point> contours)
        {
            redCircles.Clear();
            for (; contours != null; contours = contours.HNext)
            {
                redCircles.Add(contours.BoundingRectangle);
            }
        }

        private Image<Gray, byte> preProcessImage(Image<Bgr, byte> frame)
        {
            Image<Gray, byte> gImage = frame.Convert<Gray, byte>();
            gImage = gImage.ConvertScale<byte>(0.25, 0);
            //Image<Gray, byte> gImage2 = gImage.Clone();
            //gImage2 = gImage2.SmoothBlur((int)para2, (int)para2);
            //gImage2 = gImage2.Not();
            //gImage = gImage.AddWeighted(gImage2, 1, 0.5, 0);
            //gImage._EqualizeHist();
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

            Contour<Point> contours = gImage.FindContours();
            findRatContour(contours);

            return gImage;
        }

        List<Rectangle> ratRect = new List<Rectangle>();

        private void findRatContour(Contour<Point> contours)
        {
            ratRect.Clear();
            for (; contours != null; contours = contours.HNext)
            {
                if (contours.Area > 10000)
                {
                    ratRect.Add(contours.BoundingRectangle);
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            bool isAlive = false;
            if (mTimer != null && mTimer.Enabled)
            {
                isAlive = true;
                mTimer.Stop();
            }
            para1 = ((TrackBar)sender).Value;
            _capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, 0);
            getBackgroundImage();
            _capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, currentFrame);
            if (isAlive)
            {
                mTimer.Start();
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            para2 = (double)((TrackBar)sender).Value;
            para2 = (int)para2 % 2 == 0 ? para2 + 1 : para2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, currentFrame);
            mTimer = new Timer();

            mTimer.Interval = (int)(1000 / FPS);
            mTimer.Tick += timer_Tick;
            mTimer.Start();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            currentFrame = (double)trackBar3.Value;
            _capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_POS_FRAMES, currentFrame);
            pictureBox1.Image = _capture.QueryFrame().ToBitmap();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mTimer.Stop();
        }
    }
}
