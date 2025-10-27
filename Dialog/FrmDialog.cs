using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test1
{
    public partial class FrmDialog : Form
    {
        public FrmDialog()
        {
            InitializeComponent();
        }

        public string title { get; internal set; }
        public string msg { get; internal set; }

        private void FrmDialog_Load(object sender, EventArgs e)
        {
            this.Text = title;
            textBox1.Text = msg;
            this.TopMost = true;
            this.Activate();
        }

        private void yes_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }
    }
}
