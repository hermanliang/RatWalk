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

namespace HSVTest
{
    public partial class Form1 : Form
    {
        private int hValue, sValue, vValue;
        public Form1()
        {
            InitializeComponent();
        }

        private void onParam_Scroll(object sender, EventArgs e)
        {
            hValue = trackBarH.Value;
            sValue = trackBarS.Value;
            vValue = trackBarV.Value;
            txH.Text = hValue.ToString();
            txS.Text = sValue.ToString();
            txV.Text = vValue.ToString();
            pictureBox1.Image = getBitmapFromHSV(hValue, sValue, vValue);
        }

        private static Bitmap getBitmapFromHSV(int hue, int satu, int val)
        {
            Image<Hsv, byte> image = new Image<Hsv,byte>(100, 100, new Hsv(hue, satu, val));
            return image.ToBitmap();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            onParam_Scroll(sender, e);
        }
    }
}
