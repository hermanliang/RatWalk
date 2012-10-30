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

namespace VideoFileReaderTest
{
    public partial class Form1 : Form
    {
        private Capture _capture;
        private Timer mTimer;
        private int para1 = 3;
        private double para2 = 1;
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
                //pictureBox1.Image = _capture.QueryFrame().ToBitmap();
                mTimer = new Timer();
                mTimer.Interval = 1000 / 90;
                mTimer.Tick += timer_Tick;
                mTimer.Start();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Image<Bgr, byte> frame = _capture.QueryFrame();
            if (frame != null)
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
                        5,
                        (int)para2,
                        2,
                        (int)para2 / 2,
                        Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_ELLIPSE),
                    Emgu.CV.CvEnum.CV_MORPH_OP.CV_MOP_OPEN,
                    1);

                pictureBox1.Image = gImage.ToBitmap();
                //pictureBox1.Image = frame.ToBitmap();
            }
            else
            {
                mTimer.Stop();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            para1 = ((TrackBar)sender).Value * 2 + 1;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            para2 = (double)((TrackBar)sender).Value;
            para2 = (int)para2 % 2 == 0 ? para2 + 1 : para2;
        }
    }
}
