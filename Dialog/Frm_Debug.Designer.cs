namespace Test1
{
    partial class Frm_Debug
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
            button_next = new Button();
            panel1 = new Panel();
            b_copy = new Button();
            label1 = new Label();
            tx_step = new TextBox();
            ck_debug = new CheckBox();
            ck_refresh = new CheckBox();
            b_refresh = new Button();
            listView1 = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            c状态 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            TrainID = new ColumnHeader();
            Node_Type = new ColumnHeader();
            panel2 = new Panel();
            tabControl1 = new TabControl();
            tabPage2 = new TabPage();
            list_node = new ListView();
            cl_name = new ColumnHeader();
            cl_value = new ColumnHeader();
            tabPage1 = new TabPage();
            list_var = new ListView();
            col_name = new ColumnHeader();
            col_value = new ColumnHeader();
            tabPage3 = new TabPage();
            list_train = new ListView();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            timer1 = new System.Windows.Forms.Timer(components);
            panel3 = new Panel();
            listView_error = new ListView();
            col_node_name = new ColumnHeader();
            col_msg = new ColumnHeader();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage3.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // button_next
            // 
            button_next.Location = new Point(727, 17);
            button_next.Margin = new Padding(2, 3, 2, 3);
            button_next.Name = "button_next";
            button_next.Size = new Size(149, 46);
            button_next.TabIndex = 0;
            button_next.Text = "执行";
            button_next.UseVisualStyleBackColor = true;
            button_next.Click += button1_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(b_copy);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(tx_step);
            panel1.Controls.Add(ck_debug);
            panel1.Controls.Add(ck_refresh);
            panel1.Controls.Add(b_refresh);
            panel1.Controls.Add(button_next);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(2, 2);
            panel1.Margin = new Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new Size(1282, 80);
            panel1.TabIndex = 3;
            panel1.Paint += panel1_Paint;
            // 
            // b_copy
            // 
            b_copy.Location = new Point(328, 13);
            b_copy.Name = "b_copy";
            b_copy.Size = new Size(145, 46);
            b_copy.TabIndex = 8;
            b_copy.Text = "复制选择的节点";
            b_copy.UseVisualStyleBackColor = true;
            b_copy.Click += b_copy_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(541, 33);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 7;
            label1.Text = "运行步数：";
            // 
            // tx_step
            // 
            tx_step.Location = new Point(633, 31);
            tx_step.Name = "tx_step";
            tx_step.Size = new Size(61, 23);
            tx_step.TabIndex = 6;
            tx_step.Text = "1";
            // 
            // ck_debug
            // 
            ck_debug.AutoSize = true;
            ck_debug.Location = new Point(28, 29);
            ck_debug.Name = "ck_debug";
            ck_debug.Size = new Size(75, 21);
            ck_debug.TabIndex = 5;
            ck_debug.Text = "调试模式";
            ck_debug.UseVisualStyleBackColor = true;
            ck_debug.CheckedChanged += ck_debug_CheckedChanged;
            // 
            // ck_refresh
            // 
            ck_refresh.AutoSize = true;
            ck_refresh.Location = new Point(134, 29);
            ck_refresh.Margin = new Padding(4);
            ck_refresh.Name = "ck_refresh";
            ck_refresh.Size = new Size(75, 21);
            ck_refresh.TabIndex = 4;
            ck_refresh.Text = "自动刷新";
            ck_refresh.UseVisualStyleBackColor = true;
            // 
            // b_refresh
            // 
            b_refresh.Location = new Point(233, 13);
            b_refresh.Margin = new Padding(4);
            b_refresh.Name = "b_refresh";
            b_refresh.Size = new Size(70, 46);
            b_refresh.TabIndex = 3;
            b_refresh.Text = "刷新";
            b_refresh.UseVisualStyleBackColor = true;
            b_refresh.Click += b_refresh_Click;
            // 
            // listView1
            // 
            listView1.Activation = ItemActivation.OneClick;
            listView1.CheckBoxes = true;
            listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, c状态, columnHeader3, columnHeader4, columnHeader7, TrainID, Node_Type });
            listView1.Dock = DockStyle.Fill;
            listView1.FullRowSelect = true;
            listView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            listView1.Location = new Point(6, 6);
            listView1.Margin = new Padding(4);
            listView1.Name = "listView1";
            listView1.Size = new Size(837, 374);
            listView1.TabIndex = 4;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.ColumnClick += listView1_ColumnClick;
            listView1.ItemCheck += listView1_ItemCheck;
            listView1.ItemChecked += listView1_ItemChecked;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "key";
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Name";
            columnHeader2.Width = 120;
            // 
            // c状态
            // 
            c状态.Text = "状态";
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "暂停";
            columnHeader3.Width = 84;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "继续";
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "毫秒";
            // 
            // TrainID
            // 
            TrainID.Text = "TrainID";
            TrainID.Width = 90;
            // 
            // Node_Type
            // 
            Node_Type.Text = "节点Type";
            Node_Type.Width = 120;
            // 
            // panel2
            // 
            panel2.Controls.Add(tabControl1);
            panel2.Dock = DockStyle.Right;
            panel2.Location = new Point(851, 82);
            panel2.Margin = new Padding(4, 4, 4, 40);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(6);
            panel2.Size = new Size(433, 700);
            panel2.TabIndex = 5;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(6, 6);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(421, 688);
            tabControl1.TabIndex = 1;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(list_node);
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(8);
            tabPage2.Size = new Size(413, 658);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "模块属性";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // list_node
            // 
            list_node.Columns.AddRange(new ColumnHeader[] { cl_name, cl_value });
            list_node.Dock = DockStyle.Fill;
            list_node.FullRowSelect = true;
            list_node.Location = new Point(8, 8);
            list_node.Name = "list_node";
            list_node.Size = new Size(397, 642);
            list_node.TabIndex = 0;
            list_node.UseCompatibleStateImageBehavior = false;
            list_node.View = View.Details;
            list_node.SelectedIndexChanged += list_node_SelectedIndexChanged;
            list_node.DoubleClick += list_node_DoubleClick;
            // 
            // cl_name
            // 
            cl_name.Text = "名称";
            cl_name.Width = 120;
            // 
            // cl_value
            // 
            cl_value.Text = "数据";
            cl_value.Width = 200;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(list_var);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Margin = new Padding(8);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(8);
            tabPage1.Size = new Size(413, 658);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "变量";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // list_var
            // 
            list_var.Alignment = ListViewAlignment.Left;
            list_var.Columns.AddRange(new ColumnHeader[] { col_name, col_value });
            list_var.Dock = DockStyle.Fill;
            list_var.FullRowSelect = true;
            list_var.Location = new Point(8, 8);
            list_var.Margin = new Padding(10);
            list_var.Name = "list_var";
            list_var.Size = new Size(397, 642);
            list_var.TabIndex = 0;
            list_var.UseCompatibleStateImageBehavior = false;
            list_var.View = View.Details;
            // 
            // col_name
            // 
            col_name.Text = "名称";
            col_name.Width = 98;
            // 
            // col_value
            // 
            col_value.Text = "数据";
            col_value.Width = 249;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(list_train);
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(8);
            tabPage3.Size = new Size(413, 658);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "小火车";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // list_train
            // 
            list_train.Columns.AddRange(new ColumnHeader[] { columnHeader5, columnHeader6 });
            list_train.Dock = DockStyle.Fill;
            list_train.FullRowSelect = true;
            list_train.Location = new Point(8, 8);
            list_train.Name = "list_train";
            list_train.Size = new Size(397, 642);
            list_train.TabIndex = 1;
            list_train.UseCompatibleStateImageBehavior = false;
            list_train.View = View.Details;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "名称";
            columnHeader5.Width = 120;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "数据";
            columnHeader6.Width = 200;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // panel3
            // 
            panel3.Controls.Add(listView1);
            panel3.Controls.Add(listView_error);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(2, 82);
            panel3.Name = "panel3";
            panel3.Padding = new Padding(6);
            panel3.Size = new Size(849, 700);
            panel3.TabIndex = 6;
            // 
            // listView_error
            // 
            listView_error.Columns.AddRange(new ColumnHeader[] { col_node_name, col_msg });
            listView_error.Dock = DockStyle.Bottom;
            listView_error.Location = new Point(6, 380);
            listView_error.Name = "listView_error";
            listView_error.Size = new Size(837, 314);
            listView_error.TabIndex = 5;
            listView_error.UseCompatibleStateImageBehavior = false;
            listView_error.View = View.Details;
            // 
            // col_node_name
            // 
            col_node_name.Text = "节点名称";
            col_node_name.Width = 200;
            // 
            // col_msg
            // 
            col_msg.Text = "消息";
            col_msg.Width = 6000;
            // 
            // Frm_Debug
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1286, 784);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Margin = new Padding(2, 3, 2, 3);
            Name = "Frm_Debug";
            Padding = new Padding(2);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "调试工具";
            Load += Frm_Debug_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button button_next;
        private Panel panel1;
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private Button b_refresh;
        private ColumnHeader c状态;
        private Panel panel2;
        private ListView list_var;
        private ColumnHeader col_name;
        private ColumnHeader col_value;
        private System.Windows.Forms.Timer timer1;
        private CheckBox ck_refresh;
        private ColumnHeader columnHeader7;
        private CheckBox ck_debug;
        private Label label1;
        private TextBox tx_step;
        private ColumnHeader TrainID;
        private ColumnHeader Node_Type;
        private Button b_copy;
        private Panel panel3;
        private ListView list_node;
        private ColumnHeader cl_name;
        private ColumnHeader cl_value;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private ListView list_train;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ListView listView_error;
        private ColumnHeader col_node_name;
        private ColumnHeader col_msg;
    }
}