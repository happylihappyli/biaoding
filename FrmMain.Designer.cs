namespace Test1
{
    partial class FrmMain
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
            button1 = new Button();
            tx_output = new TextBox();
            textBox1 = new TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(255, 172);
            button1.Name = "button1";
            button1.Size = new Size(277, 75);
            button1.TabIndex = 0;
            button1.Text = "计算";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // tx_output
            // 
            tx_output.Location = new Point(79, 328);
            tx_output.Multiline = true;
            tx_output.Name = "tx_output";
            tx_output.Size = new Size(641, 302);
            tx_output.TabIndex = 1;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(79, 62);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(641, 30);
            textBox1.TabIndex = 2;
            textBox1.Text = "D:\\data\\camera123.txt";
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 723);
            Controls.Add(textBox1);
            Controls.Add(tx_output);
            Controls.Add(button1);
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "矩阵计算";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox tx_output;
        private TextBox textBox1;
    }
}