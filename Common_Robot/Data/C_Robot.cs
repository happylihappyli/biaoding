using ConverxHull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Test1;
using Test1.Common.Data;

namespace Common_Robot2
{
    public class C_Robot
    {
        //D再xy平面的的投影是Z
        //默认是安川机械臂参数
        public float arm_oz = 155;//155mm
        public float arm_dz = 0;// 450;//D的高度

        public float arm_c2d = 614;//614mm

        public float arm_bc = 640;
        public float arm_cc2 = 200;

        /// <summary>
        /// Math.Sqrt(arm_bc * arm_bc + arm_cc2 * arm_cc2);
        /// 640 200  对角线 ,勾股定理
        /// </summary>
        public float arm_bc2 = 670.52218f; //mm 

        public float tools_len = 215 + 100;//工具坐标215mm

        public Vector3[] rotate_center = new Vector3[7];// 旋转轴的坐标，最后一轴用来计算吸盘的方向，有些吸盘不是对称的
        public Vector3[] obj_rotate = new Vector3[7];// 旋转轴的方向

        public bool rotate7=true;//是否要计算第七轴
        public float 碰撞检测z区域=100000;

        public void Rotate(int index, double angle2)
        {
            float angle = (float)angle2;

            if (rotate7)
            {
                for (var i = index + 1; i < 7; i++)
                {

                    //obj_rotate[i].ApplyAxisAngle(obj_rotate[index], angle);
                    Vector3 axis = obj_rotate[index];
                    obj_rotate[i] = Main.绕轴旋转(obj_rotate[i], axis, angle);// Vector3.Transform(obj_rotate[i], rotationMatrix);
                    //Vector3 bTmp = rotate_center[i];// new Vector3(rotate_center[i].X, rotate_center[i].Y, rotate_center[i].Z);
                    //Vector3 bTmp = rotate_center[i] - rotate_center[index];
                    //bTmp = Main.绕轴旋转(rotate_center[i] - rotate_center[index], axis, angle) + rotate_center[index];
                    rotate_center[i] = Main.绕轴旋转(rotate_center[i] - rotate_center[index], axis, angle) + rotate_center[index];
                }
            }
            else
            {
                for (var i = index + 1; i < 6; i++)
                {
                    //obj_rotate[i].ApplyAxisAngle(obj_rotate[index], angle);
                    Vector3 axis = obj_rotate[index];
                    obj_rotate[i] = Main.绕轴旋转(obj_rotate[i], axis, angle);

                    //Vector3 bTmp = rotate_center[i];// new Vector3(rotate_center[i].X, rotate_center[i].Y, rotate_center[i].Z);
                    //bTmp.Subtract(bottom_L[index]);
                    //Vector3 bTmp = rotate_center[i] - rotate_center[index];
                    //bTmp.ApplyAxisAngle(obj_rotate[index], angle);
                    //bTmp = Main.绕轴旋转(rotate_center[i] - rotate_center[index], axis, angle);
                    //bTmp.Add(bottom_L[index]);
                    //bTmp = Main.绕轴旋转(rotate_center[i] - rotate_center[index], axis, angle) + rotate_center[index];
                    rotate_center[i] = Main.绕轴旋转(rotate_center[i] - rotate_center[index], axis, angle) + rotate_center[index];
                }
            }
        }

        public void init_fanake(float AB)
        {
            float oz = 75;
            float dz = 0;
            float c2d = 840;
            float bc = 890;
            float cc2 = 215;
            float falan = 90;
            float tools_len = AB - falan;

            this.init_robot(oz, dz, c2d, bc, cc2, tools_len);

            this.rotate_center[0] = new Vector3(0,0,0);//轴1的中心
            this.obj_rotate[0] = new Vector3(0,0,1);//L1旋转的向量

            this.rotate_center[1] = new Vector3(75,0,0);//轴2的中心
            this.obj_rotate[1] = new Vector3(0,1,0);//L2旋转的向量

            this.rotate_center[2] = new Vector3(75, 0, 840);//轴3的中心
            this.obj_rotate[2] = new Vector3(0, -1, 0);//L3旋转的向量

            this.rotate_center[3] = new Vector3(75, 0, 1055);//轴4的中心
            this.obj_rotate[3] = new Vector3(-1,0,0);//L4旋转的向量

            this.rotate_center[4] = new Vector3(965, 0, 1055);//轴5的中心
            this.obj_rotate[4] = new Vector3(0, -1,0);//L5旋转的向量


            Vector3 b5 = this.rotate_center[4];
            this.rotate_center[5] = new Vector3(b5.X + AB, b5.Y, b5.Z);//轴6的中心
            //this.rotate_center[5] = new Vector3(1055, 0, 1055);//轴6的中心
            this.obj_rotate[5] = new Vector3(-1, 0,0);//L6旋转的向量


            this.rotate_center[6] = new Vector3(b5.X + AB, b5.Y, b5.Z+100);//轴7的中心
            this.obj_rotate[6] = new Vector3(0, 0, 1);//L7旋转的向量
        }


        public void init_robot_and_ab(string file, float AB)
        {
            if (File.Exists(file) == false)
            {
                MessageBox.Show("机器人文件不存在:" + file);
                return;
            }
            IniFile ini = new IniFile(file);


            float oz = float.Parse(ini.ReadString("main", "oz", "0"));
            float dz = float.Parse(ini.ReadString("main", "dz", "0"));
            float c2d = float.Parse(ini.ReadString("main", "c2d", "0"));
            float bc = float.Parse(ini.ReadString("main", "bc", "0"));
            float cc2 = float.Parse(ini.ReadString("main", "cc2", "0"));
            float tools_len = float.Parse(ini.ReadString("main", "tools_len", "0"));

            this.init_robot(oz, dz, c2d, bc, cc2, tools_len);

            float x = float.Parse(ini.ReadString("bottom0", "x", "0"));
            float y = float.Parse(ini.ReadString("bottom0", "y", "0"));
            float z = float.Parse(ini.ReadString("bottom0", "z", "0"));

            float r_x = float.Parse(ini.ReadString("rotate0", "x", "0"));
            float r_y = float.Parse(ini.ReadString("rotate0", "y", "0"));
            float r_z = float.Parse(ini.ReadString("rotate0", "z", "1"));

            this.rotate_center[0] = new Vector3(x, y, z);//轴1的中心
            this.obj_rotate[0] = new Vector3(r_x, r_y, r_z);//, 0);//L1旋转的向量


            x = float.Parse(ini.ReadString("bottom1", "x", "0"));
            y = float.Parse(ini.ReadString("bottom1", "y", "0"));
            z = float.Parse(ini.ReadString("bottom1", "z", "0"));

            r_x = float.Parse(ini.ReadString("rotate1", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate1", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate1", "z", "1"));

            this.rotate_center[1] = new Vector3(x, y, z);//轴2的中心
            this.obj_rotate[1] = new Vector3(r_x, r_y, r_z);//L2旋转的向量


            x = float.Parse(ini.ReadString("bottom2", "x", "0"));
            y = float.Parse(ini.ReadString("bottom2", "y", "0"));
            z = float.Parse(ini.ReadString("bottom2", "z", "0"));

            r_x = float.Parse(ini.ReadString("rotate2", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate2", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate2", "z", "1"));
            this.rotate_center[2] = new Vector3(x, y, z);//轴3的中心
            this.obj_rotate[2] = new Vector3(r_x, r_y, r_z);//L3旋转的向量


            x = float.Parse(ini.ReadString("bottom3", "x", "0"));
            y = float.Parse(ini.ReadString("bottom3", "y", "0"));
            z = float.Parse(ini.ReadString("bottom3", "z", "0"));

            r_x = float.Parse(ini.ReadString("rotate3", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate3", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate3", "z", "1"));
            this.rotate_center[3] = new Vector3(x, y, z);//轴4的中心
            this.obj_rotate[3] = new Vector3(r_x, r_y, r_z);//L4旋转的向量

            x = float.Parse(ini.ReadString("bottom4", "x", "0"));
            y = float.Parse(ini.ReadString("bottom4", "y", "0"));
            z = float.Parse(ini.ReadString("bottom4", "z", "0"));

            r_x = float.Parse(ini.ReadString("rotate4", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate4", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate4", "z", "1"));
            this.rotate_center[4] = new Vector3(x, y, z);//轴5的中心
            this.obj_rotate[4] = new Vector3(r_x, r_y, r_z);//L5旋转的向量

            r_x = float.Parse(ini.ReadString("rotate5", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate5", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate5", "z", "1"));

            Vector3 b5 = this.rotate_center[4];
            this.rotate_center[5] = new Vector3(b5.X + AB, b5.Y, b5.Z);//轴6的中心
            this.obj_rotate[5] = new Vector3(r_x, r_y, r_z);//L6旋转的向量


            x = float.Parse(ini.ReadString("bottom6", "x", "0"));
            y = float.Parse(ini.ReadString("bottom6", "y", "0"));
            z = float.Parse(ini.ReadString("bottom6", "z", "0"));

            r_x = float.Parse(ini.ReadString("rotate6", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate6", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate6", "z", "1"));

            this.rotate_center[6] = new Vector3(b5.X + AB, b5.Y, b5.Z+100);//轴6的中心=轴6的中心+dz
            this.obj_rotate[6] = new Vector3(r_x, r_y, r_z);//L7旋转的向量
        }




        public void ini_anchuan_static()
        {
            //D再xy平面的的投影是Z
            this.arm_oz = 155;//155mm
            this.arm_dz = 0;// 450;//D的高度

            this.arm_c2d = 614;//614mm
            this.arm_bc = 640;
            this.arm_cc2 = 200;

            //mm Math.Sqrt(200 * 200 + 640 * 640);//200 640 对角线
            this.arm_bc2 = (float)Math.Sqrt(arm_bc* arm_bc+arm_cc2* arm_cc2);// 670.52218; 

            this.tools_len = 215 + 100;// 215 + 100;//工具坐标215mm
        }


        public void ini_estun_static()
        {
            //D再xy平面的的投影是Z
            this.arm_oz = 260;//155mm
            this.arm_dz = 0;// 450;//D的高度

            this.arm_c2d = 945;//614mm
            this.arm_bc2 = 1025; //mm Math.Sqrt(200 * 200 + 640 * 640);//200 640 对角线

            this.tools_len = 0;// 215 + 100;//工具坐标 mm

        }

        public void init_robot(float oz, float dz, float c2d, float bc, float cc2, float tools_len)
        {

            //D再xy平面的的投影是Z
            this.arm_oz = oz;//155mm
            this.arm_dz = dz;// 450;//D的高度

            this.arm_c2d = c2d;//614mm


            this.arm_bc = bc;// 640mm
            this.arm_cc2 = cc2;// 200mm

            //mm Math.Sqrt(200 * 200 + 640 * 640);//200 640 对角线
            this.arm_bc2 = (float)Math.Sqrt(arm_bc * arm_bc + arm_cc2 * arm_cc2);// 670.52218; 
            //this.arm_bc2 = b2c; //mm Math.Sqrt(200 * 200 + 640 * 640);//200 640 对角线

            this.tools_len = tools_len;// 215 + 100;//工具坐标 mm
        }




        public void ini_estun()
        {
            rotate_center[0] = new Vector3(0, 0, 0);//轴1的中心
            obj_rotate[0] = new Vector3(0, 0, -1);//, 0);//L1旋转的向量,和安川相反

            rotate_center[1] = new Vector3(2.6f, 0, 0);//轴2的中心
            obj_rotate[1] = new Vector3(0, 1, 0);//L2旋转的向量

            rotate_center[2] = new Vector3(2.6f, 0, 9.45f);//轴3的中心
            obj_rotate[2] = new Vector3(0, 1, 0);//L3旋转的向量,和安川相反

            rotate_center[3] = new Vector3(2.6f, 0, 9.45f);//轴4的中心 estun机械臂，这里假设轴4是锁定的
            obj_rotate[3] = new Vector3(-1, 0, 0);//L4旋转的向量

            rotate_center[4] = new Vector3(12.85f, 0, 9.45f);//轴5的中心
            obj_rotate[4] = new Vector3(0, -1, 0);//L5旋转的向量

            rotate_center[5] = new Vector3(15.10f, 0, 9.45f);//轴6的中心
            obj_rotate[5] = new Vector3(-1, 0, 0);//L6旋转的向量
        }

        public void ini_anchuang()
        {
            rotate_center[0] = new Vector3(0, 0, 0);//轴1的中心
            obj_rotate[0] = new Vector3(0, 0, 1);//, 0);//L1旋转的向量

            rotate_center[1] = new Vector3(1.55f, 0, 0);//轴2的中心
            obj_rotate[1] = new Vector3(0, 1, 0);//L2旋转的向量

            rotate_center[2] = new Vector3(1.55f, 0, 6.14f);//轴3的中心
            obj_rotate[2] = new Vector3(0, -1, 0);//L3旋转的向量

            rotate_center[3] = new Vector3(1.55f, 0, 8.14f);//轴4的中心
            obj_rotate[3] = new Vector3(-1, 0, 0);//L4旋转的向量

            rotate_center[4] = new Vector3(7.95f, 0, 8.14f);//轴5的中心
            obj_rotate[4] = new Vector3(0, -1, 0);//L5旋转的向量

            rotate_center[5] = new Vector3(8.95f, 0, 8.14f);//轴6的中心
            obj_rotate[5] = new Vector3(-1, 0, 0);//L6旋转的向量
        }


        /// <param name="mid"></param>
        /// <param name="list_boxs"></param>
        /// <param name="dxyz">边缘放大1.5cm</param>
        public (bool b碰撞, C_Box_Eight box_eight, List<C_Box_Eight> list_move_box)
            检查碰撞(
            C_Space space,
            C_Node pNode,
            C_Three_Point n1,
            List<C_Box> list_boxs,
            float k_up = 100, float k_down = 100, float len_ab = 360, float dxyz = 15)
        {

            if (n1.A.z > 碰撞检测z区域)
            {
                return (false, null, null);
            }

            List<C_Box_Eight> list_move_box = Main.获取动态物体列表(space,pNode,this, n1, k_up, k_down);

            for (var j = 0; j < list_move_box.Count; j++)
            {
                var move_box = list_move_box[j];
                //读取盒子8个点,12条边信息，避免碰到盒子
                for (var i = 0; i < list_boxs.Count; i++)
                {
                    C_Box box = list_boxs[i].Clone();
                    box.x_len += dxyz * 2;
                    box.y_len += dxyz * 2;
                    box.z_len += dxyz * 2;

                    (C_Box_Eight static_box, _) = Main.get_Box_Eight_from_box(box);

                    if (Main.判断盒子碰撞(move_box, static_box))
                    {
                        return (true, static_box, list_move_box);
                    }
                }
            }


            return (false, null, list_move_box);
        }
    }
}
