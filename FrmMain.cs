using ConverxHull;
using MathNet.Numerics.LinearAlgebra.Double;
using NAudio.Wave.SampleProviders;
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
        C_Camera_TuYang camera1 = new C_Camera_TuYang();
        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strFile = comboBox1.Text;

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

            tx_output.Text = p1.ToString() + "\r\n" +
                p2.ToString() + "\r\n" +
                m1.ToMatrixString();
        }

        private void b_convert_Click(object sender, EventArgs e)
        {
            C_Point3D p1 = new C_Point3D(tx_camera.Text, ",", false);
            C_Point3D p2 =  Main_Robot.摄像头坐标转到机械臂坐标(camera1, p1);

            tx_robot.Text = p2.ToString();

            C_Point3D A = (p2 - new C_Point3D(0, 49, 0));
            A.digits = 2;
            tx_falan.Text = A.ToString();

            C_Point3D B = (A - new C_Point3D(0, 79.5, 0));
            tx_b.Text = B.ToString();

            C_Point3D p7 = (A - new C_Point3D(0, 0, 100));
            tx_p7.Text = p7.ToString();
        }
    }
}
