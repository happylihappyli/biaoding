using ConverxHull;
using System.Numerics;
using Test1;
using Test1.Common.Data;


namespace Common_Robot2
{

    /// <summary>
    /// 大族S35这种机械臂
    /// </summary>
    public class C_Robot_DaZu_S35
    {
        //D再xy平面的的投影是Z
        //默认是安川机械臂参数
        public float arm_od = 216;//mm
        public float arm_dz = 185.7f;//D的高度
        public float arm_dc = 850;
        public float arm_ck = 791.5f;
        public float arm_kb = 158.5f;
        public float arm_ab = 134.5f;

        public double db_a1_min = 0;
        public double db_a1_max = 0;
        public double db_a2_min = 0;
        public double db_a2_max = 0;
        public double db_a3_min = 0;
        public double db_a3_max = 0;
        public double db_a4_min = 0;
        public double db_a4_max = 0;
        public double db_a5_min = -250 - 90;
        public double db_a5_max = -250 + 90;

        //public double db_a5_minus_a1 = 140;
        public double db_diff = 0;

        public float tools_len = 215 + 100;//工具坐标215mm

        public Vector3[] rotate_center = new Vector3[7];// 旋转轴的坐标，最后一轴用来计算吸盘的方向，有些吸盘不是对称的
        public Vector3[] obj_rotate = new Vector3[7];// 旋转轴的方向

        public bool rotate6=true;//是否要计算第6轴

        public float 碰撞检测z区域=450; //大于这个数字就不需要检测了
        public float 碰撞检测z地面=-850;
        public bool bDebug = false;

        public void Rotate_Rad(int index, double angle_rad)
        {
            float angle = (float)angle_rad;

            if (rotate6)
            {
                for (var i = index + 1; i < 6; i++)
                {
                    Vector3 axis = obj_rotate[index];
                    obj_rotate[i] = Main.绕轴旋转(obj_rotate[i], axis, angle);

                    Vector3 bTmp = new Vector3(rotate_center[i].X, rotate_center[i].Y, rotate_center[i].Z);
                    bTmp = bTmp - rotate_center[index];
                    bTmp = Main.绕轴旋转(bTmp, axis, angle);
                    bTmp = bTmp + rotate_center[index];
                    rotate_center[i] = bTmp;
                }
            }
            else
            {
                for (var i = index + 1; i < 5; i++)
                {
                    Vector3 axis = obj_rotate[index];
                    obj_rotate[i] = Main.绕轴旋转(obj_rotate[i], axis, angle);
                    Vector3 bTmp = new Vector3(rotate_center[i].X, rotate_center[i].Y, rotate_center[i].Z);
                    bTmp = bTmp - rotate_center[index];
                    bTmp = Main.绕轴旋转(bTmp, axis, angle);
                    bTmp = bTmp + rotate_center[index];
                    rotate_center[i] = bTmp;
                }
            }
        }


        public void init_robot(
            float od= 216, float dz= 185.7f, float dc= 850, float ck= 791.5f, float kb= 158.5f, float ab= 134.5f,
            float tools_len = 183.28f)    // float c2d, float bc, float cc2,
        {

            //D再xy平面的的投影是Z
            this.arm_od = od;   //  216     mm
            this.arm_dz = dz;   //  185.7   mm;//D的高度
            this.arm_dc = dc;   //  850     mm
            this.arm_ck = ck;   //  791.5   mm
            this.arm_kb = kb;   //  158.5   mm
            this.arm_ab = ab;   //  134.5   mm

            ///////////
            this.tools_len = tools_len;// 215 + 100;//工具坐标 mm

            reset_angle();
        }
        ///// <summary>
        ///// 初始化大族协作机械臂，单位mm
        ///// </summary>
        public void reset_angle() //float tools_len=183.28f)
        {
            //264-206.5+158.5=216

            //下面坐标是，机械臂六轴坐标为 0,-90,0,-90,0,0 的坐标
            //所以机械臂的初始位置是，2轴转90度，4轴转90度

            rotate_center[0] = new Vector3(0, 0, 0);//轴1的中心
            obj_rotate[0] = new Vector3(0, 0, 1);//L1旋转的向量

            rotate_center[1] = new Vector3(0, 0-this.arm_od, arm_dz);//轴2的中心   0,-216,185.7
            obj_rotate[1] = new Vector3(0, -1, 0);//L2旋转的向量

            rotate_center[2] = new Vector3(0, 0 - this.arm_od, arm_dz + arm_dc);
            obj_rotate[2] = new Vector3(0, -1, 0);//L3旋转的向量

            rotate_center[3] = new Vector3(0, 0 - this.arm_od, arm_dz + arm_dc + arm_ck);
            obj_rotate[3] = new Vector3(0, -1, 0);//L4旋转的向量

            rotate_center[4] = new Vector3(0, 0 - this.arm_od, arm_dz + arm_dc + arm_ck + arm_ab + tools_len);
            obj_rotate[4] = new Vector3(0, 0, 1);//L5旋转的向量

            rotate_center[5] = new Vector3(0, 0 - this.arm_od-100, arm_dz + arm_dc + arm_ck + arm_ab + tools_len);
            obj_rotate[5] = new Vector3(0, -1, 0);//L6旋转的向量 P7


            //rotate_center[5] = new Vector3(0, 0 - this.arm_od - arm_ab - tools_len, arm_dz + arm_dc + arm_ck + arm_kb);// 1985.7f);;
            //obj_rotate[5] = new Vector3(0, -1, 0);//L6旋转的向量

            //下面坐标是，机械臂六轴坐标为 0,-90,0,-90,0,0 的坐标
            //所以机械臂的初始位置是，2轴转90度，4轴转90度


            double[] d_angle = new double[7];
            d_angle[0] = 0;
            d_angle[1] = 90;
            d_angle[2] = 0;
            d_angle[3] = 90;
            d_angle[4] = 0;
            d_angle[5] = 0;

            for (var i = 0; i < 5; i++)
            {
                double a = d_angle[i] * Math.PI / 180;
                this.Rotate_Rad(i, a);
            }
        }

        public List<string> 根据坐标计算5轴角度(C_Node pNode,C_Point3D A_new, C_Point3D B_new,C_Point3D P7)
        {
            List<string> list = new List<string>();

            if (A_new == null)
            {
                Main.WriteLine(pNode, "A_new ==null");
                return null;
            }
            if (B_new == null)
            {
                Main.WriteLine(pNode, "B_new ==null");
                return null;
            }

            float h = arm_dz;// 185.7f;
            float DC = arm_dc;// 850;
            float CK = arm_ck;// 791.5f;
            double r1 = arm_od;// 216;//264-206.5+158.5=232.5
            //double r2 = 264;
            double Dz = arm_dz;// 185.7;

            Main.WriteLine(pNode, "A=" + A_new);
            Main.WriteLine(pNode, "B=" + B_new);

            (C_Point3D D, C_Point3D D_b) = Main_Robot.计算D坐标_Dazu(B_new, Dz, r1);

            if (bDebug) Main.WriteLine(pNode, "D=" + D);
            if (bDebug) Main.WriteLine(pNode, "D_b=" + D_b);

            if (D == null)
            {
                Main.speak_async("太近 无解！");
                Main.WriteLine(pNode, " D  ==  null");
                return null;
            }

            C_Point3D O = new C_Point3D(0, 0, h);//O不是原点 !! 调试用
            C_Point3D DB = B_new - D;
            C_Point3D DO = O - D;
            float cos = DO * DB; //调试验证用！！

            List<C_Point3D> C;
            C_Point3D Vx;
            C_Point3D Vy;
            C_Point3D Vz;

            //前面两组解，D是靠左边，可以暂时不计算
            //(C, Vx, Vy, Vz) = Main_Robot.计算C坐标_Dazu(D, K1, h, DC, CK);
            //if (C != null)
            //{
            //    calculate_save(pNode, list, A_new, B_new, D, G, C[0], Vx, K1, P7, h);
            //    calculate_save(pNode, list, A_new, B_new, D, G, C[1], Vx, K1, P7, h);
            //}


            (C, Vx, Vy, Vz) = Main_Robot.计算C坐标_Dazu(D_b, B_new, h, DC, CK);
            if (C != null)
            {
                calculate_save(pNode, list, A_new, B_new, D_b,  C[0], Vx, P7, h);
                calculate_save(pNode, list, A_new, B_new, D_b,  C[1], Vx, P7, h);
            }

            if (list.Count == 0)
            {
                if (bDebug) Main.WriteLine("D");
            }

            if (list.Count>0)
            {
                List<string> list2 = new List<string>();
                for (var i=0;i<list.Count; i++)
                {
                    string[] strSplit = list[i].Split(",");

                    float[] angles1 = new float[6];
                    for (var k = 0; k < 6; k++)
                    {
                        angles1[k] = float.Parse(strSplit[k]);
                    }
                    //(C_Point3D A_2, C_Point3D B_2, C_Point3D K_2, C_Point3D C_2, C_Point3D D_2, C_Point3D P7_2)
                    //    = this.根据角度计算ABC2CD(angles1, A_new.distance(B_new));
                    //if (A_2.distance(A_new) > 1)
                    //{
                    //    MessageBox.Show("error A_2 A_new");
                    //    continue;
                    //}
                    //if (B_2.distance(B_new) > 1)
                    //{
                    //    MessageBox.Show("error B_2 B_new");
                    //    continue;
                    //}

                    //angles1[5] += 计算角度差(A_new, B_new, P7, P7_2); 


                    //(C_Point3D A_3, C_Point3D B_3, C_Point3D K_3, C_Point3D C_3, C_Point3D D_3, C_Point3D P7_3)
                    //    = this.根据角度计算ABC2CD(angles1, A_new.distance(B_new));
                    //if (P7_3.distance(P7) > 0.1)
                    //{
                    //    MessageBox.Show("error P7_3 P7 ");
                    //    return null;
                    //}
                    string line = string.Join(",", angles1);

                    list2.Add(line);
                }
                return list2;
            }
            return list;
        }



        public  (C_Point3D K, C_Point3D K2) 计算K坐标_Dazu(
            C_Point3D A, C_Point3D B, C_Point3D D, C_Point3D G,
            double h = 185.7, double bk = 158.5)
        {

            C_Point3D O = new C_Point3D(0, 0, h);//O不是原点
            //C_Point3D Z = new C_Point3D(0, 0, 1);

            C_Point3D DO = O - D;


            //DHJ所在平面来计算
            var AB = B - A;
            var AG = G - A;
            var BG = B - G;
            if (AG.length() < 0.1)
            {
                //A在DCK平面上
                //var cross = AB.crossProduct(AG);
                //if (cross.x == 0 && cross.y == 0 && cross.z == 0)
                {
                    var BA = A - B;

                    var Q = Main_Quaternion.CreateFromAxisAngle(DO.normalize().vector, (float)Math.PI / 2);

                    Vector3 BK = Main_Quaternion.Transform((BA.normalize() * (float)bk).vector, Q);

                    return (B + Main_Robot.Vector3_To_Point3D(BK), B - Main_Robot.Vector3_To_Point3D(BK));
                }
            }
            else if (BG.length() < 0.1)
            {
                //AB垂直DCK平面
                //var cross = AB.crossProduct(AG);
                //if (cross.x == 0 && cross.y == 0 && cross.z == 0)
                {
                    //var K = B + new C_Point3D(0, 0, bk);
                    return (B + new C_Point3D(0, 0, bk), B - new C_Point3D(0, 0, bk));
                }
            }
            else
            {
                var BK = AB.crossProduct(AG).normalize() * (float)bk;//BK也有上下两个解
                return (B - BK, B + BK);
            }
        }
        public float 计算角度差(C_Point3D A_1, C_Point3D B_1, C_Point3D P7, C_Point3D P7_2)
        {
            C_Point3D v7 = (P7 - A_1).normalize();
            C_Point3D v7_2 = (P7_2 - A_1).normalize();

            var cos = v7 * v7_2;
            if (cos > 1) cos = 1;
            if (cos < -1) cos = -1;

            float delta_angle = (float)(Math.Acos(cos) * 180 / Math.PI);

            var rotate = v7.crossProduct(v7_2).normalize();
            if ((A_1 - B_1).normalize() * rotate > 0)
            {
                delta_angle = -delta_angle;
            }
            if (float.IsNaN(delta_angle))
            {
                Console.WriteLine("D");
            }
            return delta_angle;
        }

        public string calculate_save(
            C_Node pNode,
            List<string> list,
            C_Point3D A_new, C_Point3D B_new,
            C_Point3D D, C_Point3D C1,
            C_Point3D Vx, C_Point3D P7,float h)
        {
            (double a1, double a2, double a3, double a4)
                = calculate(pNode,A_new, B_new, D, C1, Vx,h);

            float db_a1 = Tools.rad_to_degree(a1);
            float db_a2 = Tools.rad_to_degree(a2);
            float db_a3 = Tools.rad_to_degree(a3);
            float db_a4 = Tools.rad_to_degree(a4);


            while (db_a1 < this.db_a1_min)
            {
                db_a1 += 360;
            }
            while (db_a1 > this.db_a1_max)
            {
                db_a1 -= 360;
            }

            while (db_a4 < this.db_a4_min)
            {
                db_a4 += 360;
            }
            while (db_a4 > this.db_a4_max)
            {
                db_a4 -= 360;
            }


            float[] angles1 = new float[6];
            angles1[0] = db_a1;
            angles1[1] = db_a2;
            angles1[2] = db_a3;
            angles1[3] = db_a4;
            angles1[4] = 0;// db_a5;



            (C_Point3D A_2, C_Point3D B_2, C_Point3D C_2, C_Point3D D_2, C_Point3D P7_2)
                = this.根据角度计算ABC2CD(angles1, A_new.distance(B_new));
            angles1[4] +=计算角度差(A_new, B_new, P7, P7_2);

            while (angles1[4] < this.db_a5_min)
            {
                angles1[4] += 360;
            }
            while (angles1[4] > this.db_a5_max)
            {
                angles1[4] -= 360;
            }
            angles1[4] += (float)this.db_diff;





            C_Point3D O = new C_Point3D(0, 0, h);//O不是原点 !! 调试用
            C_Point3D DB = B_new - D;
            C_Point3D DO = O - D;
            float cos= DO* DB; //调试验证用！！

            if (A_new.distance(A_2) > 1)
            {
                if (bDebug) Console.WriteLine("error");
            }

            if (B_new.distance(B_2) > 1)
            {
                if (bDebug) Console.WriteLine("error");
            }


            string line = string.Join(",",angles1);
            Main.WriteLine(pNode, line);
            if (db_a1 >= this.db_a1_min && db_a1 <= this.db_a1_max)
            {
                if (db_a2 >= this.db_a2_min && db_a2 <= this.db_a2_max)
                {
                    if (db_a3 >= this.db_a3_min && db_a3 <= this.db_a3_max)
                    {
                        if (db_a4 >= this.db_a4_min && db_a4 <= this.db_a4_max)
                        {
                            list.Add(line);
                            if (bDebug) Main.WriteLine(pNode, "add");
                        }
                        else
                        {
                            
                            if (bDebug) Main.WriteLine(pNode, "filter 4");
                        }
                    }
                    else
                    {
                        if (bDebug) Main.WriteLine(pNode, "filter 3");
                    }
                }
                else
                {
                    if (bDebug) Main.WriteLine(pNode, "filter 2");
                }
            }
            else
            {
                if (bDebug) Main.WriteLine(pNode, "filter 1");
            }

            return line;
        }


        public (double a1, double a2, double a3, double a4)
            calculate(C_Node pNode,
            C_Point3D A_new, C_Point3D B_new, C_Point3D D, 
            C_Point3D C, C_Point3D Vx,  double h = 185.7)
        {
            //Main.WriteLine(pNode, "==============开始计算旋转轴角度");
            double a1 = Math.Atan2(D.y, D.x);//第一个轴，OD的角度，+90，刚好是机械臂1轴的角度
            a1 += Math.PI / 2;
            a1 = angle_process(a1);
            //Main.WriteLine(this, "角度1=" + a1 + ",=" + Tools.rad_to_degree(a1));


            C_Point3D vector_dc = (C - D).normalize();
            C_Point3D vector_dh = Vx;

            double a2 = Math.Atan2(vector_dc * (new C_Point3D(0, 0, 1)), vector_dc * vector_dh);/////  my_acos(vector_dc * vector_dh);
            a2 = -a2;
            a2 = angle_process(a2);
            //Main.WriteLine(this, "2=" + a2 + ",=" + Tools.rad_to_degree(a2));


            C_Point3D O = new C_Point3D(0, 0, h);//O不是原点
            C_Point3D OD = D - O;
            C_Point3D DO = O - D;


            C_Point3D? vector_cd = (D - C).normalize();
            C_Point3D? vector_cb = (B_new - C).normalize();
            double a3 = Math.PI - Main.my_acos(vector_cd * vector_cb);
            if (vector_cd.crossProduct(vector_cb) * OD > 0)
            {
                a3 = -a3;
            }
            a3 = angle_process(a3);
            //Main.WriteLine(this, "3=" + a3 + ",=" + Tools.rad_to_degree(a3));



            ///////////////////////////////////////////
            C_Point3D vector_bc = (C - B_new).normalize();
            C_Point3D vector_ba = (A_new - B_new).normalize();
            double a4 = Main.my_acos(vector_bc * vector_ba);
            if (vector_bc.crossProduct(vector_ba) * OD <= 0)
            {
                a4 = -a4;
            }
            a4 += Math.PI / 2;
            a4 = angle_process(a4);
            //Main.WriteLine(this, "4=" + a4 + ",=" + Tools.rad_to_degree(a4));

            return (a1, a2, a3, a4);
        }


        public double angle_process(double db_a)
        {
            if (db_a < -Math.PI) db_a += Math.PI * 2;
            if (db_a > Math.PI) db_a -= Math.PI * 2;
            return db_a;
        }


        public (C_Point3D A, C_Point3D B, C_Point3D P7)
            求正解_角度(float[] angle, float distance_AB)
        {
            this.tools_len = distance_AB - this.arm_ab;
            this.reset_angle();

            for (var i = 0; i < 6; i++)
            {
                double a = angle[i] * Math.PI / 180;
                this.Rotate_Rad(i, a);
            }

            Vector3 vA = this.rotate_center[4];
            C_Point3D A = new C_Point3D(vA);

            Vector3 vB = this.rotate_center[3];
            C_Point3D B = new C_Point3D(vB);

            Vector3 v7 = this.rotate_center[5];
            C_Point3D P7 = new C_Point3D(v7);

            return (A, B, P7);
        }

        /// <summary>
        /// robot move box ...
        /// </summary>
        /// <param name="space"></param>
        /// <param name="pNode"></param>
        /// <param name="pRobot"></param>
        /// <param name="n3"></param>
        /// <param name="k_up"></param>
        /// <param name="k_down"></param>
        /// <returns></returns>
        public List<C_Box_Eight>
            获取动态物体列表(
            C_Node pNode, 
            C_Three_Point n3,
            float k_up = 100, float k_down = 100)
        {
            C_Point3D A = n3.A;
            C_Point3D B = n3.B;
            C_Point3D P7 = n3.P7;

            float len_ab = A.distance(B);

            C_Point3D AB = A - B;
            C_Point3D AB_normal = AB.normalize();
            C_Point3D AK_normal = (P7 - A).normalize();
            C_Point3D AM_normal = AK_normal.crossProduct(AB_normal).normalize();

            C_Point3D center = A;

            float ab_len_half = AB.length() / 2;
            C_Box_Eight pBox = new C_Box_Eight();
            pBox.A = center + AK_normal * k_up + AM_normal * 300 + AB_normal * ab_len_half;
            pBox.B = center - AK_normal * k_down + AM_normal * 300 + AB_normal * ab_len_half;
            pBox.C = center + AK_normal * k_up - AM_normal * 300 + AB_normal * ab_len_half;
            pBox.D = center - AK_normal * k_down - AM_normal * 300 + AB_normal * ab_len_half;

            pBox.E = center + AK_normal * k_up + AM_normal * 300 - AB_normal * ab_len_half;
            pBox.F = center - AK_normal * k_down + AM_normal * 300 - AB_normal * ab_len_half;
            pBox.G = center + AK_normal * k_up - AM_normal * 300 - AB_normal * ab_len_half;
            pBox.H = center - AK_normal * k_down - AM_normal * 300 - AB_normal * ab_len_half;

            List<C_Box_Eight> list = new List<C_Box_Eight>();
            list.Add(pBox);

            List<string> list_angle = this.根据坐标计算5轴角度(pNode, n3.A, n3.B,n3.P7);

            if (list_angle == null || list_angle.Count==0)
            {
                return null;
            }

            float[] angles = Main_Robot.six_to_array(n3.six);

            (C_Point3D A_, C_Point3D B_,C_Point3D C, C_Point3D D, C_Point3D P1) = this.根据角度计算ABC2CD(angles, len_ab);

            C_Point3D O = new C_Point3D(0.0f, 0.0f, arm_dz);

            // 3轴物体 CD
            center = C * 0.5f + D * 0.5f;
            AB = C - D;
            AB_normal = (C - D).normalize();
            AK_normal = AB_normal.crossProduct(D-O).normalize();
            AM_normal = AK_normal.crossProduct(AB_normal).normalize();
            ab_len_half = AB.length() / 2;

            pBox = new C_Box_Eight();
            pBox.A = center + AK_normal * 50 + AM_normal * 50 + AB_normal * ab_len_half;
            pBox.B = center - AK_normal * 50 + AM_normal * 50 + AB_normal * ab_len_half;
            pBox.C = center + AK_normal * 50 - AM_normal * 50 + AB_normal * ab_len_half;
            pBox.D = center - AK_normal * 50 - AM_normal * 50 + AB_normal * ab_len_half;

            pBox.E = center + AK_normal * 50 + AM_normal * 50 - AB_normal * ab_len_half;
            pBox.F = center - AK_normal * 50 + AM_normal * 50 - AB_normal * ab_len_half;
            pBox.G = center + AK_normal * 50 - AM_normal * 50 - AB_normal * ab_len_half;
            pBox.H = center - AK_normal * 50 - AM_normal * 50 - AB_normal * ab_len_half;

            list.Add(pBox);


            //// 4轴物体 CK
            //center = C * 0.5f + K * 0.5f;
            //AB_normal = (K - C).normalize();
            //AK_normal = AB_normal.crossProduct(D - O).normalize();
            //AM_normal = AK_normal.crossProduct(AB_normal).normalize();
            //ab_len_half = (C - K).length() / 2;

            //pBox = new C_Box_Eight();
            //pBox.A = center + AK_normal * 50 + AM_normal * 50 + AB_normal * ab_len_half;
            //pBox.B = center - AK_normal * 50 + AM_normal * 50 + AB_normal * ab_len_half;
            //pBox.C = center + AK_normal * 50 - AM_normal * 50 + AB_normal * ab_len_half;
            //pBox.D = center - AK_normal * 50 - AM_normal * 50 + AB_normal * ab_len_half;

            //pBox.E = center + AK_normal * 50 + AM_normal * 50 - AB_normal * ab_len_half;
            //pBox.F = center - AK_normal * 50 + AM_normal * 50 - AB_normal * ab_len_half;
            //pBox.G = center + AK_normal * 50 - AM_normal * 50 - AB_normal * ab_len_half;
            //pBox.H = center - AK_normal * 50 - AM_normal * 50 - AB_normal * ab_len_half;

            //list.Add(pBox);



            //// 5轴物体 KB
            //center = B * 0.5f + K * 0.5f;
            //AB_normal = (K - B).normalize();
            //AK_normal = AB_normal.crossProduct(D - O).normalize();
            //AM_normal = AK_normal.crossProduct(AB_normal).normalize();
            //ab_len_half = (K-B).length() / 2;

            //pBox = new C_Box_Eight();
            //pBox.A = center + AK_normal * 50 + AM_normal * 50 + AB_normal * ab_len_half;
            //pBox.B = center - AK_normal * 50 + AM_normal * 50 + AB_normal * ab_len_half;
            //pBox.C = center + AK_normal * 50 - AM_normal * 50 + AB_normal * ab_len_half;
            //pBox.D = center - AK_normal * 50 - AM_normal * 50 + AB_normal * ab_len_half;

            //pBox.E = center + AK_normal * 50 + AM_normal * 50 - AB_normal * ab_len_half;
            //pBox.F = center - AK_normal * 50 + AM_normal * 50 - AB_normal * ab_len_half;
            //pBox.G = center + AK_normal * 50 - AM_normal * 50 - AB_normal * ab_len_half;
            //pBox.H = center - AK_normal * 50 - AM_normal * 50 - AB_normal * ab_len_half;

            //list.Add(pBox);

            ////六轴 BA
            //center = B * 0.5f + A * 0.5f;
            //AB = A - B;
            //AB_normal = AB.normalize(); //BC
            //AK_normal = AB_normal.crossProduct(K - B).normalize();
            //AM_normal = AB_normal.crossProduct(AK_normal).normalize();

            //ab_len_half = AB.length() / 2;
            //float ab_len_half2 = ab_len_half;
            //float ak_len_half = 50;
            //float am_len_half = 50;

            //pBox = new C_Box_Eight();
            //pBox.A = center + AK_normal * ak_len_half + AM_normal * am_len_half + AB_normal * ab_len_half;
            //pBox.B = center - AK_normal * ak_len_half + AM_normal * am_len_half + AB_normal * ab_len_half;
            //pBox.C = center + AK_normal * ak_len_half - AM_normal * am_len_half + AB_normal * ab_len_half;
            //pBox.D = center - AK_normal * ak_len_half - AM_normal * am_len_half + AB_normal * ab_len_half;

            //pBox.E = center + AK_normal * ak_len_half + AM_normal * am_len_half - AB_normal * ab_len_half2;
            //pBox.F = center - AK_normal * ak_len_half + AM_normal * am_len_half - AB_normal * ab_len_half2;
            //pBox.G = center + AK_normal * ak_len_half - AM_normal * am_len_half - AB_normal * ab_len_half2;
            //pBox.H = center - AK_normal * ak_len_half - AM_normal * am_len_half - AB_normal * ab_len_half2;

            //list.Add(pBox);


            return list;
        }




        /// <summary>
        /// P1是第七轴的旋转轴
        /// </summary>
        /// <param name="pRobot"></param>
        /// <param name="angle_rad"></param>
        /// <param name="d_AB"></param>
        /// <returns></returns>
        public (C_Point3D A, C_Point3D B, C_Point3D C, C_Point3D D, C_Point3D P7)
            根据角度计算ABC2CD(float[] angles, float d_AB)
        {

            this.tools_len = d_AB - this.arm_ab;
            this.reset_angle();


            float[] angle_rad = new float[6];
            for (var i = 0; i < 6; i++)
            {
                angle_rad[i] = angles[i] / 180 * (float)Math.PI;
            }

            for (var i = 0; i < 6; i++)
            {
                this.Rotate_Rad(i, angle_rad[i]);
            }

            var v = this.rotate_center[4];
            C_Point3D A = new C_Point3D(v);
            v = this.rotate_center[3];
            C_Point3D B = new C_Point3D(v);
            v = this.rotate_center[2];
            C_Point3D C = new C_Point3D(v);
            C_Point3D D = new C_Point3D(this.rotate_center[1]);
            C_Point3D P7 = new C_Point3D(this.rotate_center[5]);

            return (A, B, C, D, P7);
        }


        /// <param name="mid"></param>
        /// <param name="list_boxs"></param>
        /// <param name="dxyz">边缘放大1.5cm</param>
        public (bool b碰撞, C_Box_Eight box_eight, List<C_Box_Eight> list_move_box)
            检查碰撞(
            C_Node pNode,
            C_Three_Point n1,
            List<C_Box> list_boxs,
            float k_up = 100, float k_down = 100, float len_ab = 360, float dxyz = 15)
        {
            
            if (n1.A.z > 碰撞检测z区域)
            {
                return (false, null, null);
            }
            if (n1.A.z <= 碰撞检测z地面)
            {
                return (true, null, null);
            }

            List<C_Box_Eight> list_move_box = 获取动态物体列表(pNode, n1, k_up, k_down);

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
