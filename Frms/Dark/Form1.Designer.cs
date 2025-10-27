namespace Test1
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pBoxBack = new System.Windows.Forms.PictureBox();
            this.pBoxStop = new System.Windows.Forms.PictureBox();
            this.pBoxStart = new System.Windows.Forms.PictureBox();
            this.pBoxSet = new System.Windows.Forms.PictureBox();
            this.pBoxView = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxStop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxView)).BeginInit();
            this.SuspendLayout();
            // 
            // pBoxBack
            // 
            this.pBoxBack.Image = ((System.Drawing.Image)(resources.GetObject("pBoxBack.Image")));
            this.pBoxBack.Location = new System.Drawing.Point(0, 0);
            this.pBoxBack.Name = "pBoxBack";
            this.pBoxBack.Size = new System.Drawing.Size(1012, 706);
            this.pBoxBack.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBoxBack.TabIndex = 5;
            this.pBoxBack.TabStop = false;
            // 
            // pBoxStop
            // 
            this.pBoxStop.BackColor = System.Drawing.Color.Transparent;
            this.pBoxStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pBoxStop.Image = ((System.Drawing.Image)(resources.GetObject("pBoxStop.Image")));
            this.pBoxStop.Location = new System.Drawing.Point(896, 6);
            this.pBoxStop.Name = "pBoxStop";
            this.pBoxStop.Size = new System.Drawing.Size(101, 57);
            this.pBoxStop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBoxStop.TabIndex = 10;
            this.pBoxStop.TabStop = false;
            this.pBoxStop.Click += new System.EventHandler(this.pBoxStop_Click);
            // 
            // pBoxStart
            // 
            this.pBoxStart.BackColor = System.Drawing.Color.Transparent;
            this.pBoxStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pBoxStart.Image = ((System.Drawing.Image)(resources.GetObject("pBoxStart.Image")));
            this.pBoxStart.Location = new System.Drawing.Point(792, 6);
            this.pBoxStart.Name = "pBoxStart";
            this.pBoxStart.Size = new System.Drawing.Size(101, 57);
            this.pBoxStart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBoxStart.TabIndex = 9;
            this.pBoxStart.TabStop = false;
            this.pBoxStart.Click += new System.EventHandler(this.pBoxStart_Click);
            // 
            // pBoxSet
            // 
            this.pBoxSet.BackColor = System.Drawing.Color.Transparent;
            this.pBoxSet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pBoxSet.Image = ((System.Drawing.Image)(resources.GetObject("pBoxSet.Image")));
            this.pBoxSet.Location = new System.Drawing.Point(688, 6);
            this.pBoxSet.Name = "pBoxSet";
            this.pBoxSet.Size = new System.Drawing.Size(101, 57);
            this.pBoxSet.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBoxSet.TabIndex = 8;
            this.pBoxSet.TabStop = false;
            // 
            // pBoxView
            // 
            this.pBoxView.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pBoxView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.pBoxView.Location = new System.Drawing.Point(172, 136);
            this.pBoxView.Name = "pBoxView";
            this.pBoxView.Size = new System.Drawing.Size(827, 545);
            this.pBoxView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pBoxView.TabIndex = 11;
            this.pBoxView.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 697);
            this.Controls.Add(this.pBoxView);
            this.Controls.Add(this.pBoxStop);
            this.Controls.Add(this.pBoxStart);
            this.Controls.Add(this.pBoxSet);
            this.Controls.Add(this.pBoxBack);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pBoxBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxStop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pBoxBack;
        private System.Windows.Forms.PictureBox pBoxStop;
        private System.Windows.Forms.PictureBox pBoxStart;
        private System.Windows.Forms.PictureBox pBoxSet;
        private System.Windows.Forms.PictureBox pBoxView;
    }
}

