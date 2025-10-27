
using Common_Robot2;
using ConverxHull;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;

namespace Test1
{

    /// <summary>
    /// 以后可以用 S_Calculate_Transform_Matrix 模块
    /// </summary>
    public class S_Read_Camera_Const : C_Node
    {

        public string key_save = "";
        public string file = "";
        public string test="0";

        public S_Read_Camera_Const(string name, C_Space space_parent, C_Space space) : base(name, space_parent, space)
        {
        }
        
        
        public override Task run_sub()
        {

            run_sub_main();
            return Task.CompletedTask;
        }

        private void run_sub_main()
        {
            string file2 = space.read_string(this,this.file);
            read(file2, pTrain);
        }

        public void read(string strFile, I_Train pTrain)
        {
            C_Camera_TuYang? camera1;
            lock(space.vars_new)
            {
                camera1 = (C_Camera_TuYang?)this.read_var(key_save, "C_Camera_TuYang");
                if (camera1 == null)
                {
                    camera1 = new C_Camera_TuYang();
                    this.save_var(key_save, "C_Camera_TuYang", camera1);
                }
            }

            if (File.Exists(strFile) == false)
            {
                Main.show_tip(this, " Camera 文件=" + strFile + "，不存在！", " Camera 文件=" + strFile + "，不存在！");
                Main.speak_async(this.Name+ " 相机标定文件不存在！");
                return;
            }
            else
            {
                string name= Path.GetFileName(strFile);
                Main.speak_async(name);
            }

            this.save_var( "%file", "string",strFile);

            camera1.file = strFile;

            C_Point3D p1;
            C_Point3D p2;
            DenseMatrix m1;
            (p1,p2,m1)=Main.读取标定文件(strFile);


            camera1.p1_center = p1;
            camera1.p2_center = p2;
            camera1.camera1.p1_center = p1;
            camera1.camera1.p2_center = p2;

            camera1.M_Rotate = m1; //坐标变换矩阵
            camera1.camera1.M_Rotate = m1;

            if (this.test == "1")
            {
                Main.计算标定数据误差(this,strFile, m1, p1, p2);
            }
        }

        public override void init()
        {
        }
    }
}
