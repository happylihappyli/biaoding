namespace Test1
{
    partial class FrmValue
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
            tx_value = new TextBox();
            label1 = new Label();
            panel1 = new Panel();
            tx_name = new TextBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tx_value
            // 
            tx_value.Dock = DockStyle.Fill;
            tx_value.Location = new Point(0, 53);
            tx_value.Multiline = true;
            tx_value.Name = "tx_value";
            tx_value.Size = new Size(826, 465);
            tx_value.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Left;
            label1.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(10, 10);
            label1.Margin = new Padding(10);
            label1.Name = "label1";
            label1.Padding = new Padding(2);
            label1.Size = new Size(58, 32);
            label1.TabIndex = 1;
            label1.Text = "变量";
            // 
            // panel1
            // 
            panel1.Controls.Add(tx_name);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(10);
            panel1.Size = new Size(826, 53);
            panel1.TabIndex = 2;
            // 
            // tx_name
            // 
            tx_name.Dock = DockStyle.Fill;
            tx_name.Font = new Font("Microsoft YaHei UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            tx_name.Location = new Point(68, 10);
            tx_name.Margin = new Padding(10);
            tx_name.Name = "tx_name";
            tx_name.Size = new Size(748, 34);
            tx_name.TabIndex = 2;
            // 
            // FrmValue
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(826, 518);
            Controls.Add(tx_value);
            Controls.Add(panel1);
            Name = "FrmValue";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FrmValue";
            Load += FrmValue_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tx_value;
        private Label label1;
        private Panel panel1;
        private TextBox tx_name;
    }
}