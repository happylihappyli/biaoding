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
            comboBox1 = new ComboBox();
            label1 = new Label();
            b_convert = new Button();
            tx_camera = new TextBox();
            label2 = new Label();
            tx_robot = new TextBox();
            label3 = new Label();
            label4 = new Label();
            tx_falan = new TextBox();
            label5 = new Label();
            tx_b = new TextBox();
            label6 = new Label();
            tx_p7 = new TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(575, 245);
            button1.Margin = new Padding(4);
            button1.Name = "button1";
            button1.Size = new Size(342, 97);
            button1.TabIndex = 0;
            button1.Text = "计算";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // tx_output
            // 
            tx_output.Location = new Point(101, 424);
            tx_output.Margin = new Padding(4);
            tx_output.Multiline = true;
            tx_output.Name = "tx_output";
            tx_output.Size = new Size(815, 389);
            tx_output.TabIndex = 1;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "D:\\data\\camera123.txt", "D:\\camera_robot.txt" });
            comboBox1.Location = new Point(103, 142);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(814, 39);
            comboBox1.TabIndex = 3;
            comboBox1.Text = "D:\\data\\camera123.txt";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(101, 79);
            label1.Name = "label1";
            label1.Size = new Size(182, 31);
            label1.TabIndex = 4;
            label1.Text = "标定数据文件：";
            // 
            // b_convert
            // 
            b_convert.Location = new Point(1341, 245);
            b_convert.Name = "b_convert";
            b_convert.Size = new Size(318, 97);
            b_convert.TabIndex = 5;
            b_convert.Text = "转换";
            b_convert.UseVisualStyleBackColor = true;
            b_convert.Click += b_convert_Click;
            // 
            // tx_camera
            // 
            tx_camera.Location = new Point(1031, 141);
            tx_camera.Name = "tx_camera";
            tx_camera.Size = new Size(628, 38);
            tx_camera.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(1031, 79);
            label2.Name = "label2";
            label2.Size = new Size(134, 31);
            label2.TabIndex = 4;
            label2.Text = "相机坐标：";
            // 
            // tx_robot
            // 
            tx_robot.Location = new Point(1031, 424);
            tx_robot.Name = "tx_robot";
            tx_robot.Size = new Size(628, 38);
            tx_robot.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(1031, 364);
            label3.Name = "label3";
            label3.Size = new Size(158, 31);
            label3.TabIndex = 4;
            label3.Text = "机械臂坐标：";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(1031, 494);
            label4.Name = "label4";
            label4.Size = new Size(206, 31);
            label4.TabIndex = 4;
            label4.Text = "机械臂法兰坐标：";
            // 
            // tx_falan
            // 
            tx_falan.Location = new Point(1031, 554);
            tx_falan.Name = "tx_falan";
            tx_falan.Size = new Size(628, 38);
            tx_falan.TabIndex = 6;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(1031, 618);
            label5.Name = "label5";
            label5.Size = new Size(173, 31);
            label5.TabIndex = 4;
            label5.Text = "机械臂B坐标：";
            // 
            // tx_b
            // 
            tx_b.Location = new Point(1031, 678);
            tx_b.Name = "tx_b";
            tx_b.Size = new Size(628, 38);
            tx_b.TabIndex = 6;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(1031, 749);
            label6.Name = "label6";
            label6.Size = new Size(187, 31);
            label6.TabIndex = 4;
            label6.Text = "机械臂P7坐标：";
            // 
            // tx_p7
            // 
            tx_p7.Location = new Point(1031, 809);
            tx_p7.Name = "tx_p7";
            tx_p7.Size = new Size(628, 38);
            tx_p7.TabIndex = 6;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(14F, 31F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1829, 1001);
            Controls.Add(tx_p7);
            Controls.Add(tx_b);
            Controls.Add(tx_falan);
            Controls.Add(label6);
            Controls.Add(tx_robot);
            Controls.Add(label5);
            Controls.Add(tx_camera);
            Controls.Add(label4);
            Controls.Add(b_convert);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(comboBox1);
            Controls.Add(tx_output);
            Controls.Add(button1);
            Margin = new Padding(4);
            Name = "FrmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "矩阵计算";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox tx_output;
        private ComboBox comboBox1;
        private Label label1;
        private Button b_convert;
        private TextBox tx_camera;
        private Label label2;
        private TextBox tx_robot;
        private Label label3;
        private Label label4;
        private TextBox tx_falan;
        private Label label5;
        private TextBox tx_b;
        private Label label6;
        private TextBox tx_p7;
    }
}