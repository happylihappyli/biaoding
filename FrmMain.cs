using ConverxHull;
using MathNet.Numerics.LinearAlgebra.Double;
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
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strFile = textBox1.Text;// "D:\\data\\camera123.txt";
            C_Camera_TuYang camera1 = new C_Camera_TuYang();

            if (File.Exists(strFile) == false)
            {
                Main.show_tip(null, " Camera 文件=" + strFile + "，不存在！", " Camera 文件=" + strFile + "，不存在！");
                Main.speak_async(this.Name + " 相机标定文件不存在！");
                return;
            }
            else
            {
                string name = Path.GetFileName(strFile);
                Main.speak_async(name);
            }

            //this.save_var("%file", "string", strFile);

            //camera1.file = strFile;

            C_Point3D p1;
            C_Point3D p2;
            DenseMatrix m1;
            (p1, p2, m1) = Main.读取标定文件(strFile);


            camera1.p1_center = p1;
            camera1.p2_center = p2;
            camera1.camera1.p1_center = p1;
            camera1.camera1.p2_center = p2;

            camera1.M_Rotate = m1; //坐标变换矩阵
            camera1.camera1.M_Rotate = m1;

            //if (this.test == "1")
            {
                Main.计算标定数据误差(null, strFile, m1, p1, p2);
            }

            tx_output.Text=p1.ToString()+"\r\n"+
                p2.ToString() + "\r\n" +
                m1.ToMatrixString();
        }
    }
}
