namespace ToastNotifications
{
    partial class Notification
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            lifeTimer = new System.Windows.Forms.Timer(components);
            labelBody = new Label();
            labelTitle = new Label();
            SuspendLayout();
            // 
            // lifeTimer
            // 
            lifeTimer.Tick += lifeTimer_Tick;
            // 
            // labelBody
            // 
            labelBody.Anchor = AnchorStyles.None;
            labelBody.BackColor = Color.Transparent;
            labelBody.Font = new Font("Calibri", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            labelBody.ForeColor = Color.Navy;
            labelBody.Location = new Point(16, 69);
            labelBody.Margin = new Padding(7, 0, 7, 0);
            labelBody.Name = "labelBody";
            labelBody.Size = new Size(551, 100);
            labelBody.TabIndex = 0;
            labelBody.Text = "Body goes here and here and here and here and here";
            labelBody.TextAlign = ContentAlignment.TopCenter;
            labelBody.Click += labelRO_Click;
            // 
            // labelTitle
            // 
            labelTitle.BackColor = Color.Transparent;
            labelTitle.Font = new Font("Calibri", 12.75F, FontStyle.Bold, GraphicsUnit.Point);
            labelTitle.ForeColor = Color.Black;
            labelTitle.Location = new Point(7, 2);
            labelTitle.Margin = new Padding(7, 0, 7, 0);
            labelTitle.Name = "labelTitle";
            labelTitle.Size = new Size(590, 50);
            labelTitle.TabIndex = 0;
            labelTitle.Text = "title goes here";
            labelTitle.Click += labelTitle_Click;
            // 
            // Notification
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(595, 191);
            ControlBox = false;
            Controls.Add(labelTitle);
            Controls.Add(labelBody);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(7, 7, 7, 7);
            Name = "Notification";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "EDGE Shop Flag Notification";
            TopMost = true;
            Activated += Notification_Activated;
            FormClosed += Notification_FormClosed;
            Load += Notification_Load;
            Shown += Notification_Shown;
            Click += Notification_Click;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer lifeTimer;
        private Label labelBody;
        private Label labelTitle;
    }
}