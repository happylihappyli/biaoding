using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Three.Net.Core;
using Three.Net.Math;

namespace Common_Robot2
{

    /// <summary>
    /// 大族S25，S30这种机械臂
    /// </summary>
    public class C_Robot2
    {
        //D再xy平面的的投影是Z
        //默认是安川机械臂参数
        public double arm_oz = 155;//155mm
        public double arm_dz = 0;// 450;//D的高度

        public double arm_c2d = 614;//614mm

        public double arm_bc = 640;
        public double arm_cc2 = 200;

        /// <summary>
        /// Math.Sqrt(arm_bc * arm_bc + arm_cc2 * arm_cc2);
        /// 640 200  对角线 ,勾股定理
        /// </summary>
        public double arm_bc2 = 670.52218; //mm 

        public double tools_len = 215 + 100;//工具坐标215mm

        public Vector3[] rotate_center = new Vector3[7];// 旋转轴的坐标，最后一轴用来计算吸盘的方向，有些吸盘不是对称的
        public Vector3[] obj_rotate = new Vector3[7];// 旋转轴的方向

        public bool rotate7=true;//是否要计算第七轴

        public void Rotate(int index, double add2)
        {
            float add = (float)add2;

            if (rotate7)
            {
                for (var i = index + 1; i < 7; i++)
                {
                    obj_rotate[i].ApplyAxisAngle(obj_rotate[index], add);
                    Vector3 bTmp = new Vector3(rotate_center[i].x, rotate_center[i].y, rotate_center[i].z);
                    bTmp.Subtract(rotate_center[index]);
                    bTmp.ApplyAxisAngle(obj_rotate[index], add);
                    bTmp.Add(rotate_center[index]);
                    rotate_center[i] = bTmp;
                }
            }
            else
            {
                for (var i = index + 1; i < 6; i++)
                {
                    obj_rotate[i].ApplyAxisAngle(obj_rotate[index], add);
                    Vector3 bTmp = new Vector3(rotate_center[i].x, rotate_center[i].y, rotate_center[i].z);
                    bTmp.Subtract(rotate_center[index]);
                    bTmp.ApplyAxisAngle(obj_rotate[index], add);
                    bTmp.Add(rotate_center[index]);
                    rotate_center[i] = bTmp;
                }
            }
        }


        public void init_robot(double oz, double dz,double c2d,double bc,double cc2,double tools_len)
        {

            //D再xy平面的的投影是Z
            this.arm_oz = oz;//155mm
            this.arm_dz = dz;// 450;//D的高度

            this.arm_c2d = c2d;//614mm


            this.arm_bc = bc;// 640mm
            this.arm_cc2 = cc2;// 200mm

            //mm Math.Sqrt(200 * 200 + 640 * 640);//200 640 对角线
            this.arm_bc2 = Math.Sqrt(arm_bc * arm_bc + arm_cc2 * arm_cc2);// 670.52218; 
            //this.arm_bc2 = b2c; //mm Math.Sqrt(200 * 200 + 640 * 640);//200 640 对角线

            this.tools_len = tools_len;// 215 + 100;//工具坐标 mm
        }



        /// <summary>
        /// 初始化大族协作机械臂，单位mm
        /// </summary>
        public void ini_dazu()
        {
            rotate_center[0] = new Vector3(0, 0, 0);//轴1的中心
            obj_rotate[0] = new Vector3(0, 0, 1);//, 0);//L1旋转的向量

            rotate_center[1] = new Vector3(0, -216f, 185.7f);//轴2的中心
            obj_rotate[1] = new Vector3(0, -1, 0);//L2旋转的向量

            rotate_center[2] = new Vector3(0, -216f, 1035.7f);
            obj_rotate[2] = new Vector3(0, -1, 0);//L3旋转的向量

            rotate_center[3] = new Vector3(0, -216f, 1827.2f);// -0.575f);
            obj_rotate[3] = new Vector3(0, -1, 0);//L4旋转的向量

            rotate_center[4] = new Vector3(0, -216f, 1985.7f);
            obj_rotate[4] = new Vector3(0, 0, 1);//L5旋转的向量

            rotate_center[5] = new Vector3(0, -216f - 134.5f, 1985.7f);
            obj_rotate[5] = new Vector3(0, 1, 0);//L6旋转的向量

            rotate_center[6] = new Vector3(0, -216f - 134.5f, 2085.7f);
            obj_rotate[6] = new Vector3(0, 0, 1);//L7旋转的向量

        }

    }
}
