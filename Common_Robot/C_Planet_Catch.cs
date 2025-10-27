
using ConverxHull;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using Test1;

namespace Common_Robot2
{
    //抓取面信息
    public class C_Planet_Catch : C_Planet
    {
        public double fa_length = 0;//中心点在法向量投影距离
        public List<C_Planet>? pArray;
        public bool bDelete = false;
        public Angle rot_y;
        public Angle rot_z;

        public C_Point3D arm_pCenter;
        public C_Point3D arm_faxiangliang;
        public C_Point3D? arm_pCatchCenter;

        public C_Point3D check_z_point;//用来判断范围的
        public C_Point3D arm_point_b_uv;
        public bool collision = false;// area_percent;
        public C_Data pData;
        public int z_add = 0;//连续抓1次，抓不到加一定深度
        public C_Point3D check_collision;
        public int index_x;
        public int index_y;
        public int index_z;

        //   https://www.geogebra.org/3d/ks9gsfrn B

        public C_Point3D arm_a;//机械臂针尖坐标（或吸盘）
        public C_Point3D arm_b;//机械臂最后一个臂的轴心位置:B
        public C_Point3D? arm_d;//机械臂D


        public bool collision_obj=false;
        public double dbAngle;
        public double len1;
        public double len2;
        public double len3;

        public ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();
        public List<C_Point3D> rect;
        public double rect_angle;

        public C_Space? space;
        public Angle rot_x;
        public C_Point3D arm_p0;
        public C_Point3D arm_p1;
        public C_Point3D arm_p2;
        public C_Point3D pp0;
        public C_Point3D pp1;
        public C_Point3D pp2;
        public C_Point3D pp3;

        //吸盘偏移位置多少
        public float dx = 0;
        public float dy = 0;
        public float dz = 0;
        public List<C_Cup>? list_cup =null;
        public bool bSuck=false;

        public C_Planet_Catch(C_Space? space,int Group_ID,C_Data? pMain):base(Group_ID)
        {
            this.space = space;
            this.pData = pMain;
        }



        public static double angle_process(double angle)
        {
            if (angle > Math.PI) angle = angle - Math.PI * 2;
            if (angle < -Math.PI) angle = angle + Math.PI * 2;
            angle = Math.Round(angle, 5);
            return angle;
        }





        /// <summary>
        /// 有两个
        /// </summary>
        /// <returns></returns>
        public C_Point3D[] 计算C2坐标_v2(C_Node pNode, C_Robot pRobot,C_Point3D B1, C_Point3D D1)
        {
            //根据B,D 算C2
            C_Point3D D = 转截面坐标系(D1);
            C_Point3D B = 转截面坐标系(B1);
            C_Point3D[] arr = 计算两个圆的交点_v2(pNode, D, B, pRobot.arm_c2d, pRobot.arm_bc2);
            if (arr == null) return null;

            double angle = Math.Atan2(B1.y, B1.x);
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = 截面转普通坐标系(arr[i], angle);
            }
            return arr;
        }


        //public C_Result 根据坐标计算6轴角度(
        //    C_Node pNode,
        //    C_Camera_Const camera1,
        //    C_Robot pRobot,
        //    C_Train pTrain,
        //    int GroupID,
        //    C_Point3D A, C_Point3D B, C_Point3D D, List<C_Point3D> pList,

        //    bool bDebug_Mode = false)
        //{



        //    if (bDebug_Mode) Main.WriteLine("A=" + A.ToString());
        //    if (bDebug_Mode) Main.WriteLine("B=" + B.ToString());
        //    if (bDebug_Mode) Main.WriteLine("D=" + D.ToString());
        //    //Main.WriteLine("计算BD以及是否超范围");
        //    C_Point3D[] arr = space.main.计算D和C2_C以及是否超范围(pNode, pRobot, GroupID, A, B);

        //    if (arr == null)
        //    {
        //        Main.WriteLine("前面超范围没有过滤！");

        //        return new C_Result("", A, B, "前面超范围没有过滤！");
        //    }

        //    C_Point3D C2 = arr[0];
        //    C_Point3D C = arr[2];

        //    Main.WriteLine("计算旋转轴角度");
        //    double[] angle = new double[6];
        //    double a1 = Math.Atan2(D.y, D.x);//第一个轴
        //    a1 = angle_process(a1);
        //    if (bDebug_Mode)
        //        Main.WriteLine("1=" + a1 + ",=" + Tools.to_degree(a1));

        //    C_Point3D vector_dc2 = C2-(D);
        //    C_Point3D vector_od_in_z0 = D-(new C_Point3D(0, 0, D.z))-(new C_Point3D(0, 0, 0));//od 在z=0平面的投影
        //    double len_vector_dc2_in_z0 = vector_dc2*(vector_od_in_z0.normalize());

        //    double new_y = len_vector_dc2_in_z0;// Math.Sqrt(vector_dc2.x * vector_dc2.x + vector_dc2.y * vector_dc2.y);
        //    double new_x = vector_dc2.z;
        //    double a2 = Math.Atan2(new_y, new_x); //Math.PI / 2 - Math.Atan2(new_y, new_x);
        //    a2 = angle_process(a2);
        //    if (bDebug_Mode)
        //        Main.WriteLine("2=" + a2 + ",=" + Tools.to_degree(a2));


        //    C_Point3D vector_cb = B-(C);
        //    C_Point3D vector_c2c = C-(C2);

        //    Vector3D v_dc2 = new Vector3D(vector_dc2.x, vector_dc2.y, vector_dc2.z);
        //    Vector3D v_c2c = new Vector3D(vector_c2c.x, vector_c2c.y, vector_c2c.z);

        //    C_Point3D Z = new C_Point3D(D.x, D.y, 0);

        //    C_Point3D vector_oz = Z;
        //    Vector3D v_oz = new Vector3D(vector_oz.x, vector_oz.y, vector_oz.z);
        //    Vector3D v_c2r = v_dc2.CrossProduct(v_oz); //v_dc2.CrossProduct(v_z2);

        //    if (v_dc2.DotProduct(new Vector3D(0, 0, 1)) < 0)
        //    {
        //        v_c2r = -v_c2r;
        //    }

        //    UnitVector3D J = v_dc2.CrossProduct(v_c2r).Normalize();

        //    new_x = v_c2c.DotProduct(v_dc2.Normalize());
        //    new_y = v_c2c.DotProduct(J);

        //    double a3 = Math.Atan2(new_y, new_x);
        //    a3 = angle_process(a3);
        //    if (bDebug_Mode)
        //        Main.WriteLine("3=" + a3 + ",=" + Tools.to_degree(a3));

        //    C_Point3D vector_ba = B-(A);
        //    C_Point3D vector_bc = B-(C);

        //    Vector3D v_ba = new Vector3D(vector_ba.x, vector_ba.y, vector_ba.z);
        //    Vector3D v_bc = new Vector3D(vector_bc.x, vector_bc.y, vector_bc.z);

        //    var len_bg = v_ba.DotProduct(v_bc.Normalize());

        //    //G
        //    C_Point3D G = B+(C-(B)*(len_bg / v_bc.Length));

        //    //J
        //    C_Point3D vector_cd = C-(D);
        //    Vector3D v_cb = new Vector3D(vector_cb.x, vector_cb.y, vector_cb.z);
        //    Vector3D v_cd = new Vector3D(vector_cd.x, vector_cd.y, vector_cd.z);
        //    UnitVector3D v_cj = v_cb.CrossProduct(v_cd).Normalize();
        //    v_cj.CrossProduct(v_cb);
        //    UnitVector3D v_gk = v_cj;
        //    C_Point3D vector_gc = G-(C);
        //    Vector3D v_gc = new Vector3D(vector_gc.x, vector_gc.y, vector_gc.z);
        //    UnitVector3D v_gh = v_gk.CrossProduct(v_gc).Normalize();

        //    C_Point3D vector_ga = G-(A);
        //    Vector3D v_ga = new Vector3D(vector_ga.x, vector_ga.y, vector_ga.z);
        //    new_x = v_ga.DotProduct(v_gh);
        //    new_y = v_ga.DotProduct(v_gk);
        //    Vector3D v_gh_new = v_gh.ScaleBy(new_x);
        //    Vector3D v_gk_new = v_gk.ScaleBy(new_y);

        //    var minLen = pRobot.tools_len * Math.Sin(0.2 / 180 * Math.PI);//5轴要至少0.2度的偏差，否则就当作是0
        //    double a4;

        //    if (Math.Abs(new_x) < minLen && Math.Abs(new_y) < minLen)
        //    {
        //        a4 = 0;
        //    }
        //    else
        //    {
        //        a4 = -Math.Atan2(new_y, new_x);// - Math.PI;
        //    }
        //    a4 = angle_process(a4);
        //    if (bDebug_Mode)
        //        Main.WriteLine("4=" + a4 + ",=" + Tools.to_degree(a4));

        //    new_x = v_ba.DotProduct(v_bc.Normalize());
        //    new_y = v_ba.DotProduct(v_ga.Normalize());
        //    double a5 = Math.Atan2(new_y, new_x) - Math.PI;
        //    a5 = angle_process(a5);
        //    if (bDebug_Mode)
        //        Main.WriteLine("5=" + a5 + ",=" + Tools.to_degree(a5));

        //    angle[0] = a1;
        //    angle[1] = a2;
        //    angle[2] = a3;
        //    angle[3] = a4;
        //    angle[4] = a5;


        //    //var camera1 = (C_Camera_Const)pTrain.vars["camera1_const"];
        //    C_Point3D A_new = Tools.机械臂坐标到摄像头坐标(camera1, A);// arm_point_a);
        //    C_Point3D B_new = Tools.机械臂坐标到摄像头坐标(camera1, B);// arm_point_b);
        //    C_Point3D C_new = Tools.机械臂坐标到摄像头坐标(camera1, C);
        //    angle[5] = space.tools.计算第六轴角度(pNode, A_new, B_new, C_new, pList);



        //    if (验算ABC(pNode, angle, A, B, C2, C, D) == false)
        //    {
        //        if (bDebug_Mode)
        //            Main.WriteLine("6=" + angle[5] + ",=" + Tools.to_degree(angle[5]));

        //        angle[0] = angle[0] * 180 / Math.PI;
        //        angle[1] = angle[1] * 180 / Math.PI;
        //        angle[2] = angle[2] * 180 / Math.PI;
        //        angle[3] = angle[3] * 180 / Math.PI;
        //        angle[4] = angle[4] * 180 / Math.PI;
        //        angle[5] = angle[5] * 180 / Math.PI;

        //        for (var i = 0; i < angle.Length; i++)
        //        {
        //            double a = angle[i] * 180 / Math.PI;
        //            //Main.WriteLine(GroupID + " i=" + a);
        //        }

        //        if (angle[0] < -170 || angle[0] > 170)
        //        {
        //            return new C_Result("", A, B, "角度1 超范围！");
        //        }
        //        if (angle[1] < -90 || angle[1] > 155)
        //        {
        //            return new C_Result("", A, B, "角度2 超范围！");
        //        }
        //        if (angle[2] < -85 || angle[2] > 150)
        //        {
        //            return new C_Result("", A, B, "角度3 超范围！");
        //        }
        //        if (angle[3] < -200 || angle[3] > 200)
        //        {
        //            return new C_Result("", A, B, "角度4 超范围！");
        //        }
        //        if (angle[4] < -150 || angle[4] > 150)
        //        {
        //            return new C_Result("", A, B, "角度5 超范围！");
        //        }


        //        //angle[0] = angle[0] * 1435.35;
        //        //angle[1] = angle[1] * 1300.32;
        //        //angle[2] = angle[2] * 1422.23;
        //        //angle[3] = angle[3] * 970;
        //        //angle[4] = angle[4] * 980.24;
        //        angle[5] = -angle[5];//角度是相反的



        //        //double angle_dif = double.Parse((string)space.vars.read_vars(pTrain, "#angle_dif", "string"));
        //        //var angle6 = Math.Round(angle[5]);
        //        //var angle6_old=angle6;
        //        //angle6 += angle_dif;
        //        //Main.WriteLine("angle6=" + angle6_old + " add=" + angle6);
        //        //angle[5] = Math.Round((angle6) * 454.75);

        //        string strLine = string.Join(",", angle);
        //        //for (var i = 0; i < 5; i++)
        //        //{
        //        //    strLine += Math.Round(angle[i]) + ",";
        //        //}
        //        //strLine += angle[5];

        //        return new C_Result(strLine, A, B);
        //    }
        //    Main.WriteLine("@ 验算有问题！");

        //    Main.WriteLine("angle=");
        //    for (var i = 0; i <= 5; i++)
        //    {
        //        Main.WriteLine(i + "=" + angle[i]);
        //    }

        //    Main.WriteLine("A=" + A.ToString2());
        //    Main.WriteLine("B=" + B.ToString2());
        //    Main.WriteLine("C2=" + C2.ToString2());
        //    Main.WriteLine("C=" + C.ToString2());
        //    Main.WriteLine("D=" + D.ToString2());

        //    验算ABC(pNode, angle, A, B, C2, C, D);

        //    return new C_Result("", A, B, "验算有问题！");
        //}

        public bool 验算ABC(
            C_Node pNode,
            double[] angle, C_Point3D A, C_Point3D B, C_Point3D C2, C_Point3D C, C_Point3D D)
        {
            C_Robot pRobot = new C_Robot();
            pRobot.ini_anchuang();


            for (var i = 0; i < 6; i++)
            {
                double a = angle[i] * 180 / Math.PI;
                Main.WriteLine(""+a);
                pRobot.Rotate(i, angle[i]);
            }

            bool bError = true;
            if (check_equal(100, pRobot.rotate_center[2], C2))
            {
                if (check_equal(100, pRobot.rotate_center[3], C))
                {
                    if (check_equal(100, pRobot.rotate_center[4], B))
                    {
                        bError = false;
                    }
                    else
                    {
                        Main.WriteLine("B " + pRobot.rotate_center[4].ToString() + " " + B.ToString());
                    }
                }
                else
                {
                    Main.WriteLine("C " + pRobot.rotate_center[3].ToString() + " " + C.ToString());
                }
            }
            else
            {
                Main.WriteLine("C2 " + pRobot.rotate_center[2].ToString() + " " + C2.ToString());
            }

            //根据D算C2，
            return bError;
        }



        public bool 验算ABC_v2(C_Node pNode, double[] angle, C_Point3D B, C_Point3D C2, C_Point3D D)
        {
            C_Robot pRobot = new C_Robot();
            pRobot.ini_estun();// ini_anchuang();
            for (var i = 0; i < 6; i++)
            {
                double a = angle[i] * 180 / Math.PI;
                Main.WriteLine(""+a);
                pRobot.Rotate(i, angle[i]);
            }

            bool bError = true;
            if (check_equal(100, pRobot.rotate_center[2], C2))
            {
                if (check_equal(100, pRobot.rotate_center[4], B))
                {
                    bError = false;
                }
                else
                {
                    Main.WriteLine("B " + pRobot.rotate_center[4].ToString() + " " + B.ToString());
                }
            }
            else
            {
                Main.WriteLine("C2 " + pRobot.rotate_center[2].ToString() + " " + C2.ToString());
            }

            //根据D算C2，
            return bError;
        }



        public static bool check_equal(double dbScale, Vector3 v, C_Point3D p)
        {
            if (Math.Round(v.X * dbScale - p.x, 1) != 0)
            {
                return false;
            }
            if (Math.Round(v.Y * dbScale - p.y, 1) != 0)
            {
                return false;
            }
            if (Math.Round(v.Z * dbScale - p.z, 1) != 0)
            {
                return false;
            }

            return true;
        }

        public static C_Point3D 转截面坐标系(C_Point3D p1)
        {
            var new_x = Math.Sqrt(p1.x * p1.x + p1.y * p1.y);
            var new_y = p1.z;
            return new C_Point3D(new_x, new_y, 0);
        }


        public static C_Point3D 截面转普通坐标系(C_Point3D p1, double angle)
        {
            var new_x = p1.x * Math.Cos(angle);
            var new_y = p1.x * Math.Sin(angle);
            return new C_Point3D(new_x, new_y, p1.y);
        }


        public static C_Point3D 计算B_v2(C_Point3D A)
        {
            C_Point3D A2 = Tools.FromA_To_Real_A(A);
            C_Point3D B = Tools.A_To_B(A2);
            return B;
        }


        public C_Result 根据坐标计算3轴角度(
            I_Train pTrain,
            C_Node pNode,
            C_Robot pRobot,
            //int type,
            C_Point3D B, C_Point3D D, double angle_last)
        {
            //if (const_arm.bDebug_Mode) Main.WriteLine("A="+A.ToString());
            if (space.vars.bDebug_Mode) Main.WriteLine("B=" + B.ToString());
            if (space.vars.bDebug_Mode) Main.WriteLine("D=" + D.ToString());
            Main.WriteLine("计算BD以及是否超范围");
            C_Point3D[] arr = space.main.计算D和C2_C以及是否超范围_v2(pNode,pRobot, B);

            if (arr == null)
            {
                Main.WriteLine("前面超范围没有过滤！");

                return new C_Result("", B, B,null,null,D, "前面超范围没有过滤！");
            }

            C_Point3D C2 = arr[0];

            //Main.WriteLine("计算旋转轴角度");
            double[] angle = new double[6];
            double a1 = -1 * Math.Atan2(D.y, D.x);//第一个轴
            a1 = angle_process(a1);
            Main.WriteLine("1=" + a1 + ",=" + Tools.rad_to_degree(a1));

            C_Point3D vector_dc2 = C2-(D);
            C_Point3D vector_od_in_z0 = D-(new C_Point3D(0, 0, D.z))-(new C_Point3D(0, 0, 0));//od 在z=0平面的投影
            double len_vector_dc2_in_z0 = vector_dc2*(vector_od_in_z0.normalize());

            double new_y = len_vector_dc2_in_z0;// Math.Sqrt(vector_dc2.x * vector_dc2.x + vector_dc2.y * vector_dc2.y);
            double new_x = vector_dc2.z;
            double a2 = Math.Atan2(new_y, new_x); //Math.PI / 2 - Math.Atan2(new_y, new_x);
            a2 = angle_process(a2);
            //Main.WriteLine("2=" + a2 + ",=" + Tools.to_degree(a2));


            C_Point3D vector_c2b = B-(C2);

            Vector3D v_dc2 = new Vector3D(vector_dc2.x, vector_dc2.y, vector_dc2.z);
            Vector3D v_c2b = new Vector3D(vector_c2b.x, vector_c2b.y, vector_c2b.z);

            C_Point3D Z = new C_Point3D(D.x, D.y, 0);

            C_Point3D vector_oz = Z;
            Vector3D v_oz = new Vector3D(vector_oz.x, vector_oz.y, vector_oz.z);
            Vector3D v_c2r = v_dc2.CrossProduct(v_oz); //v_dc2.CrossProduct(v_z2);

            if (v_dc2.DotProduct(new Vector3D(0, 0, 1)) < 0)
            {
                v_c2r = -v_c2r;
            }

            UnitVector3D J = v_dc2.CrossProduct(v_c2r).Normalize();

            new_x = v_c2b.DotProduct(v_dc2.Normalize());
            new_y = v_c2b.DotProduct(J);

            double a3 = Math.Atan2(new_y, new_x);
            a3 = -1 * (angle_process(a3) + Math.PI / 2);
            //Main.WriteLine("3=" + a3 + ",=" + Tools.to_degree(a3));

            angle[0] = a1;
            angle[1] = a2;
            angle[2] = a3;


            if (验算ABC_v2(pNode, angle, B, C2, D) == false)
            {
                //Main.WriteLine("6=" + angle[5] + ",=" + Tools.to_degree(angle[5]));


                angle[0] = angle[0] * 180 / Math.PI;
                angle[1] = angle[1] * 180 / Math.PI;
                angle[2] = angle[2] * 180 / Math.PI;

                string strLine = "";
                for (var i = 0; i < 3; i++)
                {
                    strLine += Math.Round(angle[i] * 100) / 100 + ",";
                }



                string tmp = (string)space.read_vars(pTrain,pNode, "#angle_dif", "string");
                double angle_dif = 0;// double.Parse((tmp);
                if (tmp != null)
                {
                    angle_dif = double.Parse(tmp);
                }
                //double angle_dif = double.Parse((string)space.vars.read_vars(pTrain, "#angle_dif", "string"));
                double angle4 = -angle[0] + angle_last+ angle_dif; 
                strLine += Math.Round(angle4, 2);

                return new C_Result(strLine, B, B,C2,null,D);
            }
            Main.WriteLine("@ 验算有问题！");

            Main.WriteLine("angle=");
            for (var i = 0; i <= 5; i++)
            {
                Main.WriteLine(i + "=" + angle[i]);
            }


            Main.WriteLine("B=" + B.ToString2());
            Main.WriteLine("C2=" + C2.ToString2());
            Main.WriteLine("D=" + D.ToString2());

            //验算ABC(angle,  B, C2,  D);

            return new C_Result("", B, B,C2, null, D, "验算有问题！");
        }

        //https://blog.csdn.net/zx3517288/article/details/53326420
        /// <summary>
        /// 在截面坐标系下计算，算完毕要转回原先坐标系，返回值 c2,c2,c,c,有两个解
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="r1"></param>
        /// <param name="p2_x"></param>
        /// <param name="p2_y"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public C_Point3D[] 计算两个圆的交点_v2(C_Node pNode, C_Point3D p1_D, C_Point3D p2_B, double r1, double r2)
        {
            double L = Math.Sqrt((p1_D.x - p2_B.x) * (p1_D.x - p2_B.x) + (p1_D.y - p2_B.y) * (p1_D.y - p2_B.y));//AB
            if (L > r1 + r2)
            {
                Main.WriteLine("L>r1+r2 超范围！");
                //space.vars.Step = "等待下一步";
                return null;
            }
            double k1 = (p2_B.y - p1_D.y) / (p2_B.x - p1_D.x);
            double k2 = -1 / k1;
            double AE = (r1 * r1 - r2 * r2 + L * L) / (2 * L);
            double x0 = p1_D.x + AE / L * (p2_B.x - p1_D.x);
            double y0 = p1_D.y + AE / L * (p2_B.y - p1_D.y);

            double CE = Math.Sqrt(r1 * r1 - AE * AE);

            double EF = CE / Math.Sqrt(1 + k2 * k2);
            double CF = Math.Abs(EF * k2);


            //加减要和k2符号有关
            double x_c = 0;
            if (k2 > 0)
            {
                x_c = x0 + EF;
            }
            else
            {
                x_c = x0 - EF;
            }

            double y_c = y0 + CF;
            double x_d = x0 - EF;
            double y_d = y0 - CF;
            C_Point3D[] arr = new C_Point3D[2];
            arr[0] = new C_Point3D(x_c, y_c, 0);
            arr[1] = new C_Point3D(x_d, y_d, 0);

            ////================================================
            ////上面两个坐标C就是立体图里面的C2，下面根据根据C2算C
            //C_Point3D vector_C2B = p2_B-(arr[0]);
            //Vector2D v_C2B = new Vector2D(vector_C2B.x, vector_C2B.y);
            //Angle a = Angle.FromRadians(Math.Atan2(640, 200));
            //Vector2D v_C2C = v_C2B.Normalize().Rotate(a).ScaleBy(200);

            ////C=C2+向量C2C
            //arr[2] = arr[0]+(new C_Point3D(v_C2C.X, v_C2C.Y, 0));


            ////上面两个坐标就是立体图里面的C2，下面根据根据C2算C
            //vector_C2B = p2_B-(arr[1]);
            //v_C2B = new Vector2D(vector_C2B.x, vector_C2B.y);
            //a = Angle.FromRadians(Math.Atan2(640, 200));
            //v_C2C = v_C2B.Normalize().Rotate(a).ScaleBy(200);
            //arr[3] = arr[1]+(new C_Point3D(v_C2C.X, v_C2C.Y, 0));

            return arr;
        }



        //https://blog.csdn.net/zx3517288/article/details/53326420
        /// <summary>
        /// 在截面坐标系下计算，算完毕要转回原先坐标系，返回值 c2,c2,c,c,有两个解
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="r1"></param>
        /// <param name="p2_x"></param>
        /// <param name="p2_y"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        //public C_Point3D[] 计算两个圆的交点(C_Node pNode, int Group_ID, C_Point3D p1_D, C_Point3D p2_B, double r1, double r2)
        //{
        //    double L = Math.Sqrt((p1_D.x - p2_B.x) * (p1_D.x - p2_B.x) + (p1_D.y - p2_B.y) * (p1_D.y - p2_B.y));//AB
        //    if (L > r1 + r2)
        //    {
        //        Main.WriteLine(Group_ID + " L>r1+r2 超范围！");
        //        //space.vars.Step = "等待下一步";
        //        return null;
        //    }
        //    double k1 = (p2_B.y - p1_D.y) / (p2_B.x - p1_D.x);
        //    double k2 = -1 / k1;
        //    double AE = (r1 * r1 - r2 * r2 + L * L) / (2 * L);
        //    double x0 = p1_D.x + AE / L * (p2_B.x - p1_D.x);
        //    double y0 = p1_D.y + AE / L * (p2_B.y - p1_D.y);

        //    double CE = Math.Sqrt(r1 * r1 - AE * AE);

        //    double EF = CE / Math.Sqrt(1 + k2 * k2);
        //    double CF = Math.Abs(EF * k2);


        //    //加减要和k2符号有关
        //    double x_c = 0;
        //    if (k2 > 0)
        //    {
        //        x_c = x0 + EF;
        //    }
        //    else
        //    {
        //        x_c = x0 - EF;
        //    }

        //    double y_c = y0 + CF;
        //    double x_d = x0 - EF;
        //    double y_d = y0 - CF;
        //    C_Point3D[] arr = new C_Point3D[4];
        //    arr[0] = new C_Point3D(x_c, y_c, 0);
        //    arr[1] = new C_Point3D(x_d, y_d, 0);

        //    //================================================
        //    //zzz 下面的数据最好写到配置里，现在写死了。
        //    //上面两个坐标C就是立体图里面的C2，下面根据根据C2算C
        //    C_Point3D vector_C2B = p2_B-(arr[0]);
        //    Vector2D v_C2B = new Vector2D(vector_C2B.x, vector_C2B.y);
        //    Angle a = Angle.FromRadians(Math.Atan2(640, 200));
        //    Vector2D v_C2C = v_C2B.Normalize().Rotate(a).ScaleBy(200);

        //    //C=C2+向量C2C
        //    arr[2] = arr[0]+(new C_Point3D(v_C2C.X, v_C2C.Y, 0));


        //    //上面两个坐标就是立体图里面的C2，下面根据根据C2算C
        //    vector_C2B = p2_B-(arr[1]);
        //    v_C2B = new Vector2D(vector_C2B.x, vector_C2B.y);
        //    a = Angle.FromRadians(Math.Atan2(640, 200));
        //    v_C2C = v_C2B.Normalize().Rotate(a).ScaleBy(200);
        //    arr[3] = arr[1]+(new C_Point3D(v_C2C.X, v_C2C.Y, 0));

        //    return arr;
        //}
    }
}
