namespace HSVTest
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.trackBarH = new System.Windows.Forms.TrackBar();
            this.txH = new System.Windows.Forms.Label();
            this.trackBarS = new System.Windows.Forms.TrackBar();
            this.trackBarV = new System.Windows.Forms.TrackBar();
            this.txS = new System.Windows.Forms.Label();
            this.txV = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarV)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.pictureBox1.Location = new System.Drawing.Point(280, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(250, 250);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // trackBarH
            // 
            this.trackBarH.Location = new System.Drawing.Point(91, 268);
            this.trackBarH.Maximum = 255;
            this.trackBarH.Name = "trackBarH";
            this.trackBarH.Size = new System.Drawing.Size(669, 42);
            this.trackBarH.TabIndex = 1;
            this.trackBarH.Value = 128;
            this.trackBarH.Scroll += new System.EventHandler(this.onParam_Scroll);
            // 
            // txH
            // 
            this.txH.AutoSize = true;
            this.txH.Location = new System.Drawing.Point(766, 280);
            this.txH.Name = "txH";
            this.txH.Size = new System.Drawing.Size(23, 12);
            this.txH.TabIndex = 2;
            this.txH.Text = "128";
            // 
            // trackBarS
            // 
            this.trackBarS.Location = new System.Drawing.Point(91, 316);
            this.trackBarS.Maximum = 255;
            this.trackBarS.Name = "trackBarS";
            this.trackBarS.Size = new System.Drawing.Size(669, 42);
            this.trackBarS.TabIndex = 3;
            this.trackBarS.Value = 128;
            this.trackBarS.Scroll += new System.EventHandler(this.onParam_Scroll);
            // 
            // trackBarV
            // 
            this.trackBarV.Location = new System.Drawing.Point(91, 364);
            this.trackBarV.Maximum = 255;
            this.trackBarV.Name = "trackBarV";
            this.trackBarV.Size = new System.Drawing.Size(669, 42);
            this.trackBarV.TabIndex = 4;
            this.trackBarV.Value = 128;
            this.trackBarV.Scroll += new System.EventHandler(this.onParam_Scroll);
            // 
            // txS
            // 
            this.txS.AutoSize = true;
            this.txS.Location = new System.Drawing.Point(766, 330);
            this.txS.Name = "txS";
            this.txS.Size = new System.Drawing.Size(23, 12);
            this.txS.TabIndex = 5;
            this.txS.Text = "128";
            // 
            // txV
            // 
            this.txV.AutoSize = true;
            this.txV.Location = new System.Drawing.Point(766, 377);
            this.txV.Name = "txV";
            this.txV.Size = new System.Drawing.Size(23, 12);
            this.txV.TabIndex = 6;
            this.txV.Text = "128";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 280);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "Hue";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 330);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "Satuation";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 377);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "Value";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 425);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txV);
            this.Controls.Add(this.txS);
            this.Controls.Add(this.trackBarV);
            this.Controls.Add(this.trackBarS);
            this.Controls.Add(this.txH);
            this.Controls.Add(this.trackBarH);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "HSV Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarV)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TrackBar trackBarH;
        private System.Windows.Forms.Label txH;
        private System.Windows.Forms.TrackBar trackBarS;
        private System.Windows.Forms.TrackBar trackBarV;
        private System.Windows.Forms.Label txS;
        private System.Windows.Forms.Label txV;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

