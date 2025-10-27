using Common_Robot2;
using ConverxHull;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using Three.Net.Math;
using static Common_Robot2.C_Node;
using Three.Net.Objects;
using System.Numerics;
using Test1.Common.Data;

namespace Test1
{



    public static class Main_Robot
    {


        //public static (bool bError, Vector3[] bottom_L, Vector3[] rotate_L)
        //    DaZu机械臂六轴计算XYZ(C_Robot_DaZu pRobot, double[] angle)//string file,
        //{
        //    //一定要初始化，否则角度不对
        //    pRobot.reset_angle();// ini_dazu();

        //    double[] d_angle = new double[7];
        //    d_angle[0] = 0;
        //    d_angle[1] = 90;
        //    d_angle[2] = 0;
        //    d_angle[3] = 90;
        //    d_angle[4] = 0;


        //    for (var i = 0; i < 6; i++)
        //    {
        //        double a = (angle[i] + d_angle[i]) * Math.PI / 180;
        //        pRobot.Rotate_Rad(i, a);
        //    }


        //    return (false, pRobot.rotate_center, pRobot.obj_rotate);
        //}




        /// <summary>
        /// 机械臂平移到虚拟相机1的位置变换矩阵是：M1
        /// 虚拟相机1旋转到相机2的位置变换矩阵是：M2
        /// 因为M1，M2是可以计算的
        /// 机械臂下的任意一点A
        /// 在虚拟相机2的坐标为：X=M2* (M1* A)
        /// 虚拟相机2到实际相机的矩阵为M
        /// M * X = Y
        /// Y是点A在相机里的坐标
        /// 这样就可以求得M
        /// </summary>
        /// <param name="pRobot2"></param>
        /// <param name="angles"></param>
        /// <returns></returns>
        public static (Three.Net.Math.Matrix4 m1, Three.Net.Math.Matrix4 m2, C_Point3D vX, C_Point3D vY, C_Point3D vZ)
            DaZu_S35_机械臂变换矩阵(C_Robot_DaZu_S35 pRobot2, float[] angles)
        {
            //Main_Robot.DaZu机械臂六轴计算XYZ(pRobot2, angles);

            (C_Point3D vA, C_Point3D vB, C_Point3D vK) = pRobot2.求正解_角度(angles, pRobot2.arm_ab);

            //C_Point3D vK = Main_Point.Vector3_To_Point(pRobot2.rotate_center[6]);//第7轴，轴心
            //C_Point3D vA = Main_Point.Vector3_To_Point(pRobot2.rotate_center[5]);//第6轴，轴心
            //C_Point3D vB = Main_Point.Vector3_To_Point(pRobot2.rotate_center[4]);//第5轴，轴心

            C_Point3D vZ = (vB - vA).normalize();//z向量

            C_Point3D vY = (vK - vA).normalize();//y向量

            C_Point3D vX = vY.crossProduct(vZ).normalize(); //上面两个向量叉乘就是x向量

            Three.Net.Math.Matrix4 m1 = new Three.Net.Math.Matrix4(1, 0, 0, -vA.x, 0, 1, 0, -vA.y, 0, 0, 1, -vA.z, 0, 0, 0, 1);// 机械臂平移到虚拟相机1的位置

            //新坐标在旧的坐标系下表示的矩阵，所以要逆矩阵
            Three.Net.Math.Matrix4 m2_pre = new Three.Net.Math.Matrix4(vX.x, vY.x, vZ.x, 0, vX.y, vY.y, vZ.y, 0, vX.z, vY.z, vZ.z, 0, 0, 0, 0, 1);

            Three.Net.Math.Matrix4 m2 = Three.Net.Math.Matrix4.GetInverse(m2_pre); //虚拟相机1旋转到虚拟相机2的位置

            return (m1, m2, vX, vY, vZ);
        }



        /// <summary>
        /// 机械臂平移到虚拟相机1的位置变换矩阵是：M1
        /// 虚拟相机1旋转到相机2的位置变换矩阵是：M2
        /// 因为M1，M2是可以计算的
        /// 机械臂下的任意一点A
        /// 在虚拟相机2的坐标为：X=M2* (M1* A)
        /// 虚拟相机2到实际相机的矩阵为M
        /// M * X = Y
        /// Y是点A在相机里的坐标
        /// 这样就可以求得M
        /// </summary>
        /// <param name="pRobot2"></param>
        /// <param name="angles"></param>
        /// <returns></returns>
        public static (Three.Net.Math.Matrix4 m1, Three.Net.Math.Matrix4 m2, C_Point3D vX, C_Point3D vY, C_Point3D vZ)
            DaZu机械臂变换矩阵(C_Robot_DaZu pRobot2, float[] angles)
        {
            //Main_Robot.DaZu机械臂六轴计算XYZ(pRobot2, angles);
            (C_Point3D vA, C_Point3D vB, C_Point3D vK) = pRobot2.求正解_角度(angles, pRobot2.arm_ab);
            //C_Point3D vK = Main_Point.Vector3_To_Point(pRobot2.rotate_center[6]);//第7轴，轴心
            //C_Point3D vA = Main_Point.Vector3_To_Point(pRobot2.rotate_center[5]);//第6轴，轴心
            //C_Point3D vB = Main_Point.Vector3_To_Point(pRobot2.rotate_center[4]);//第5轴，轴心

            C_Point3D vZ = (vB - vA).normalize();//z向量

            C_Point3D vY = (vK - vA).normalize();//y向量

            C_Point3D vX= vY.crossProduct(vZ).normalize(); //上面两个向量叉乘就是x向量

            Three.Net.Math.Matrix4 m1 = new Three.Net.Math.Matrix4(1, 0, 0, -vA.x, 0, 1, 0, -vA.y, 0, 0, 1, -vA.z, 0, 0, 0, 1);// 机械臂平移到虚拟相机1的位置

            //新坐标在旧的坐标系下表示的矩阵，所以要逆矩阵
            Three.Net.Math.Matrix4 m2_pre = new Three.Net.Math.Matrix4(vX.x, vY.x, vZ.x, 0, vX.y, vY.y, vZ.y, 0, vX.z, vY.z, vZ.z, 0, 0, 0, 0, 1);

            Three.Net.Math.Matrix4 m2 = Three.Net.Math.Matrix4.GetInverse(m2_pre); //虚拟相机1旋转到虚拟相机2的位置

            //C_Point3D test = new C_Point3D(1, 0, 0);
            //var test2=Main_Point.Multiply_Point(m2, test);

            return (m1, m2,vX,vY,vZ);
        }





        public static (C_Result pResult, double len1, double len2) 根据坐标计算6轴角度(
            string direction,
            bool Angle_Modify,
            C_Space space,
            C_Node pNode,
            C_Robot? pRobot,
            int GroupID,
            C_Point3D A,
            C_Point3D B,
            C_Point3D D,
            List<C_Point3D> list_arm,
            string file,
            float angle_dif)
        {


            pNode.save_var("_A_string", "string", A.ToString());
            pNode.save_var("_B_string", "string", B.ToString());
            pNode.save_var("_D_string", "string", D.ToString());


            List<C_Point3D>? arr = Main_Robot.计算D和C2_C以及是否超范围(space, pNode, pRobot, GroupID, A, B);

            if (arr == null || arr?.Count == 0)
            {
                Main.WriteLine(pNode, "GroupID=" + GroupID + " 前面超范围没有过滤！");
                return (new C_Result("", A, B, null, null, D, "前面超范围没有过滤！"), 0, 0);
            }

            C_Point3D? C2 = arr?[0];
            C_Point3D? C = arr?[2];

            if (C == null || C2 == null)
            {
                Main.WriteLine(pNode, "GroupID=" + GroupID + " C,C2==null！");
                return (new C_Result("", A, B, C2, C, D, "C,C2==null！"), 0, 0);
            }

            //Main.WriteLine(pNode, "计算旋转轴角度");
            float[] angle = new float[6];
            float a1 = (float)Math.Atan2(D.y, D.x);//第一个轴
            a1 = Main.angle_process(a1);
            //Main.WriteLine(pNode, "1=" + a1 + ",=" + Tools.to_degree(a1));

            C_Point3D vector_dc2 = C2 - (D);
            C_Point3D vector_od_in_z0 = D - (new C_Point3D(0, 0, D.z)) - (new C_Point3D(0, 0, 0));//od 在z=0平面的投影
            double len_vector_dc2_in_z0 = vector_dc2 * (vector_od_in_z0.normalize());

            double new_y = len_vector_dc2_in_z0;// Math.Sqrt(vector_dc2.x * vector_dc2.x + vector_dc2.y * vector_dc2.y);
            double new_x = vector_dc2.z;
            float a2 = (float)Math.Atan2(new_y, new_x); //Math.PI / 2 - Math.Atan2(new_y, new_x);
            a2 = Main.angle_process(a2);

            //Main.WriteLine(pNode, "2=" + a2 + ",=" + Tools.to_degree(a2));


            C_Point3D? vector_cb = B - (C);
            C_Point3D? vector_c2c = C - (C2);

            Vector3D v_dc2 = new Vector3D(vector_dc2.x, vector_dc2.y, vector_dc2.z);
            Vector3D v_c2c = new Vector3D(vector_c2c.x, vector_c2c.y, vector_c2c.z);

            C_Point3D Z = new C_Point3D(D.x, D.y, 0);

            C_Point3D vector_oz = Z;
            Vector3D v_oz = new Vector3D(vector_oz.x, vector_oz.y, vector_oz.z);
            Vector3D v_c2r = v_dc2.CrossProduct(v_oz); //v_dc2.CrossProduct(v_z2);

            if (v_dc2.DotProduct(new Vector3D(0, 0, 1)) < 0)
            {
                v_c2r = -v_c2r;
            }

            UnitVector3D J = v_dc2.CrossProduct(v_c2r).Normalize();

            new_x = v_c2c.DotProduct(v_dc2.Normalize());
            new_y = v_c2c.DotProduct(J);

            float a3 = (float)Math.Atan2(new_y, new_x);
            a3 = Main.angle_process(a3);
            //Main.WriteLine(pNode, "3=" + a3 + ",=" + Tools.to_degree(a3));

            C_Point3D vector_ba = B - (A);
            C_Point3D vector_bc = B - (C);

            Vector3D v_ba = new Vector3D(vector_ba.x, vector_ba.y, vector_ba.z);
            Vector3D v_bc = new Vector3D(vector_bc.x, vector_bc.y, vector_bc.z);

            var len_bg = v_ba.DotProduct(v_bc.Normalize());

            //G
            C_Point3D G = B + (C - B) * (float)(len_bg / v_bc.Length);

            //J
            C_Point3D vector_cd = C - (D);
            Vector3D v_cb = new Vector3D(vector_cb.x, vector_cb.y, vector_cb.z);
            Vector3D v_cd = new Vector3D(vector_cd.x, vector_cd.y, vector_cd.z);
            UnitVector3D v_cj = v_cb.CrossProduct(v_cd).Normalize();
            v_cj.CrossProduct(v_cb);
            UnitVector3D v_gk = v_cj;
            C_Point3D vector_gc = G - (C);
            Vector3D v_gc = new Vector3D(vector_gc.x, vector_gc.y, vector_gc.z);
            UnitVector3D v_gh = v_gk.CrossProduct(v_gc).Normalize();

            C_Point3D vector_ga = G - (A);
            Vector3D v_ga = new Vector3D(vector_ga.x, vector_ga.y, vector_ga.z);
            new_x = v_ga.DotProduct(v_gh);
            new_y = v_ga.DotProduct(v_gk);

            var minLen = pRobot?.tools_len * Math.Sin(0.2 / 180 * Math.PI);//5轴要至少0.2度的偏差，否则就当作是0
            float a4;

            if (Math.Abs(new_x) < minLen && Math.Abs(new_y) < minLen)
            {
                a4 = 0;
            }
            else
            {
                a4 = -(float)Math.Atan2(new_y, new_x);// - Math.PI;
            }
            a4 = Main.angle_process(a4);
            //Main.WriteLine(pNode, "4=" + a4 + ",=" + Tools.to_degree(a4));

            new_x = v_ba.DotProduct(v_bc.Normalize());
            new_y = v_ba.DotProduct(v_ga.Normalize());
            float a5 = (float)Math.Atan2(new_y, new_x) - (float)Math.PI;
            a5 = Main.angle_process(a5);
            //Main.WriteLine(pNode, "5=" + a5 + ",=" + Tools.to_degree(a5));

            angle[0] = a1;
            angle[1] = a2;
            angle[2] = a3;

            if (a4 > Math.PI / 2)
            {
                a4 -= (float)Math.PI;
                a5 = -a5;
            }
            else if (a4 < -Math.PI / 2)
            {
                a4 += (float)Math.PI;
                a5 = -a5;
            }

            angle[3] = a4;
            angle[4] = a5;

            (angle[5], double len1, double len2) = 计算第六轴角度(GroupID, pNode, A, B, C, list_arm);

            //Main.WriteLine(pNode, "角度6=" + angle[5]);

            angle[5] += angle_dif / 180 * (float)Math.PI;

            (bool bError, Vector3[] bottom_L, _) = Main.验算ABC(pRobot,pNode, angle, A, B, C2, C, D,null);

            if (bError == false)
            {
                //Main.WriteLine(pNode, "6=" + angle[5] + ",=" + Tools.to_degree(angle[5]));

                angle[0] = angle[0] * 180 / (float)Math.PI;
                angle[1] = angle[1] * 180 / (float)Math.PI;
                angle[2] = angle[2] * 180 / (float)Math.PI;
                angle[3] = angle[3] * 180 / (float)Math.PI;
                angle[4] = angle[4] * 180 / (float)Math.PI;
                angle[5] = angle[5] * 180 / (float)Math.PI;

                if (Angle_Modify)
                {
                    //用 bottom_L[6],bottom_L[5] 判断吸盘是否朝上

                    if (direction == "-x")
                    {
                        if (bottom_L[6].Z < bottom_L[5].Z)
                        {
                            angle[5] += 180; //保证bottom_6一直在上面
                        }
                    }
                    else if (direction == "z")
                    {
                        if (bottom_L[6].X < bottom_L[5].X)
                        {
                            angle[5] += 180; //保证bottom_6一直在上面
                        }
                    }
                    else if (direction == "zy")
                    {
                        if (bottom_L[6].Y < bottom_L[5].Y)
                        {
                            angle[5] += 180; //保证bottom_6一直在上面
                        }
                    }

                }


                if (angle[0] < -170 || angle[0] > 170)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度1 超范围！"), len1, len2);
                }
                if (angle[1] < -90 || angle[1] > 155)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度2 超范围！"), len1, len2);
                }
                if (angle[2] < -85 || angle[2] > 150)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度3 超范围！"), len1, len2);
                }
                if (angle[3] < -200 || angle[3] > 200)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度4 超范围！"), len1, len2);
                }
                if (angle[4] < -150 || angle[4] > 150)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度5 超范围！"), len1, len2);
                }



                if (angle[5] > 180)
                {
                    angle[5] -= 360;
                }
                else if (angle[5] < -180)
                {
                    angle[5] += 360;
                }
                string strLine = string.Join(",", angle);

                return (new C_Result(strLine, A, B,C2,C,D), len1, len2);
            }
            Main.WriteLine(pNode, "@ 验算有问题！");

            Main.WriteLine(pNode, "angle=========================");
            for (var i = 0; i <= 5; i++)
            {
                Main.WriteLine(pNode, i + "=" + angle[i]);
            }

            Main.WriteLine(pNode, "A=" + A.ToString2());
            Main.WriteLine(pNode, "B=" + B.ToString2());
            Main.WriteLine(pNode, "C2=" + C2.ToString2());
            Main.WriteLine(pNode, "C=" + C.ToString2());
            Main.WriteLine(pNode, "D=" + D.ToString2());

            Main.验算ABC(pRobot, pNode, angle, A, B, C2, C, D,null);

            return (new C_Result("", A, B, C2, C, D, "验算有问题！"), len1, len2);
        }



        /// <summary>
        /// 所有点在抓取面上拟合一条直线的角度
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="pList"></param>
        /// <param name="pCatch"></param>
        /// <returns></returns>
        public static (float angle, double len1, double len2) 计算第六轴角度(
            int Group_ID,
            C_Node pNode,
            C_Point3D A, C_Point3D B, C_Point3D C,
            List<C_Point3D> pList)
        {
            if (pList == null)
            {
                return (0, 0, 0);
            }

            C_Point3D BC = C - (B);
            C_Point3D BA = A - (B);
            C_Point3D AB = B - (A);
            Vector3D v_bc = new Vector3D(BC.x, BC.y, BC.z);
            Vector3D v_ba = new Vector3D(BA.x, BA.y, BA.z);
            Vector3D v_ab = new Vector3D(AB.x, AB.y, AB.z);

            C_Planet Planet_Project = new C_Planet(0, A, BA);

            UnitVector3D v_OY = v_bc.CrossProduct(v_ba).Normalize();
            UnitVector3D v_OX = v_OY.CrossProduct(v_ab).Normalize();

            List<C_Point3D> pList_Point = new List<C_Point3D>();//投影面 Planet_Project 上的点的坐标

            Main.WriteLine("=================");

            List<C_Point3D> list2 = new List<C_Point3D>();
            for (int i = 0; i < pList.Count; i++)
            {
                C_Point3D p = pList[i];

                C_Point3D p2 = Planet_Project.投影到一个面后的坐标(p);

                list2.Add(p2);

                {
                    //A 差不多是中心点，A_p也就是投影面Planet_Project上的点p在以A为原点的坐标下的坐标
                    C_Point3D A_p = p2 - (A);
                    Vector3D v_A_p = new Vector3D(A_p.x, A_p.y, A_p.z);
                    double x = v_A_p.DotProduct(v_OX);
                    double y = v_A_p.DotProduct(v_OY);
                    pList_Point.Add(new C_Point3D(x, y, 0));
                }
            }


            if (pList_Point.Count <= 4)
            {
                MessageBox.Show(pNode.Name + "点云个数<=4！");
            }

            (List<C_Point3D> rect, double rect_angle, double len1, double len2) = m1.计算外接矩形(pList_Point);

            Main.WriteLine("角度：" + rect_angle * 180 / Math.PI);

            return ((float)rect_angle, len1, len2);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="约束面法向量"></param>
        /// <param name="direction"></param>
        /// <param name="Angle_Modify"></param>
        /// <param name="space"></param>
        /// <param name="pNode"></param>
        /// <param name="pRobot"></param>
        /// <param name="GroupID"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="D"></param>
        /// <param name="list_arm"></param>
        /// <param name="file"></param>
        /// <param name="angle_dif"></param>
        /// <param name="four_axis_max">四轴最大值（单位是PI）默认是0.5</param>
        /// <param name="four_axis_min">四轴最小值（单位是PI）默认是-0.5</param>
        /// <returns></returns>
        public static (C_Result pResult,C_Point3D D)
            根据坐标计算6轴角度_v2(
                    C_Point3D 约束面法向量,
                    C_Space space,
                    C_Node pNode,
                    C_Robot? pRobot,
                    int GroupID,
                    C_Point3D A,
                    C_Point3D B,
                    C_Point3D K,
                    float angle_dif,
                    double four_axis_max = 0.5,
                    double four_axis_min = -0.5)
        {


            C_Point3D? D = Main.计算D坐标(pRobot, B);


            if (D == null)
            {
                Main.WriteLine(pNode, " pRobot  ==  null");
                pNode.Next_Step = Node_Next.False;
                return  (null,null);
            }

            pNode.save_var("_A_string", "string", A.ToString());
            pNode.save_var("_B_string", "string", B.ToString());
            pNode.save_var("_D_string", "string", D.ToString());


            List<C_Point3D>? arr = Main_Robot.计算D和C2_C以及是否超范围(space, pNode, pRobot, GroupID, A, B);

            if (arr == null || arr?.Count == 0)
            {
                Main.WriteLine(pNode, "GroupID=" + GroupID + " 前面超范围没有过滤！");
                return (new C_Result("", A, B, null,null, D, "前面超范围没有过滤！"),D);
            }

            C_Point3D? C2 = arr?[0];
            C_Point3D? C = arr?[2];

            if (C == null || C2 == null)
            {
                Main.WriteLine(pNode, "GroupID=" + GroupID + " C,C2==null！");
                return (new C_Result("", A, B, C2, C, D, "C,C2==null！"),D);
            }

            float[] angle_rad = new float[6];
            float a1 = (float)Math.Atan2(D.y, D.x);//第一个轴
            a1 = Main.angle_process(a1);
            //Main.WriteLine(pNode, "1=" + a1 + ",=" + Tools.to_degree(a1));

            C_Point3D vector_dc2 = C2 - (D);
            C_Point3D vector_od_in_z0 = D - (new C_Point3D(0, 0, D.z)) - (new C_Point3D(0, 0, 0));//od 在z=0平面的投影
            double len_vector_dc2_in_z0 = vector_dc2 * (vector_od_in_z0.normalize());

            double new_y = len_vector_dc2_in_z0;// Math.Sqrt(vector_dc2.x * vector_dc2.x + vector_dc2.y * vector_dc2.y);
            double new_x = vector_dc2.z;
            double a2 = Math.Atan2(new_y, new_x); //Math.PI / 2 - Math.Atan2(new_y, new_x);
            a2 = Main.angle_process(a2);

            //Main.WriteLine(pNode, "2=" + a2 + ",=" + Tools.to_degree(a2));


            C_Point3D? vector_cb = B - (C);
            C_Point3D? vector_c2c = C - (C2);

            Vector3D v_dc2 = new Vector3D(vector_dc2.x, vector_dc2.y, vector_dc2.z);
            Vector3D v_c2c = new Vector3D(vector_c2c.x, vector_c2c.y, vector_c2c.z);

            C_Point3D Z = new C_Point3D(D.x, D.y, 0);

            C_Point3D vector_oz = Z;
            Vector3D v_oz = new Vector3D(vector_oz.x, vector_oz.y, vector_oz.z);
            Vector3D v_c2r = v_dc2.CrossProduct(v_oz); //v_dc2.CrossProduct(v_z2);

            if (v_dc2.DotProduct(new Vector3D(0, 0, 1)) < 0)
            {
                v_c2r = -v_c2r;
            }

            UnitVector3D J=new UnitVector3D();
            try
            {
                J = v_dc2.CrossProduct(v_c2r).Normalize();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            new_x = v_c2c.DotProduct(v_dc2.Normalize());
            new_y = v_c2c.DotProduct(J);

            double a3 = Math.Atan2(new_y, new_x);
            a3 = Main.angle_process(a3);
            //Main.WriteLine(pNode, "3=" + a3 + ",=" + Tools.to_degree(a3));

            C_Point3D vector_ba = B - (A);
            C_Point3D vector_bc = B - (C);

            Vector3D v_ba = new Vector3D(vector_ba.x, vector_ba.y, vector_ba.z);
            Vector3D v_bc = new Vector3D(vector_bc.x, vector_bc.y, vector_bc.z);

            var len_bg = v_ba.DotProduct(v_bc.Normalize());

            //G
            C_Point3D G = B + (C - B) * (float)(len_bg / v_bc.Length);

            //J
            C_Point3D vector_cd = C - (D);
            Vector3D v_cb = new Vector3D(vector_cb.x, vector_cb.y, vector_cb.z);
            Vector3D v_cd = new Vector3D(vector_cd.x, vector_cd.y, vector_cd.z);
            UnitVector3D v_cj = v_cb.CrossProduct(v_cd).Normalize();
            v_cj.CrossProduct(v_cb);
            UnitVector3D v_gk = v_cj;
            C_Point3D vector_gc = G - (C);
            Vector3D v_gc = new Vector3D(vector_gc.x, vector_gc.y, vector_gc.z);
            UnitVector3D v_gh = v_gk.CrossProduct(v_gc).Normalize();

            C_Point3D vector_ga = G - (A);
            Vector3D v_ga = new Vector3D(vector_ga.x, vector_ga.y, vector_ga.z);
            new_x = v_ga.DotProduct(v_gh);
            new_y = v_ga.DotProduct(v_gk);

            var minLen = pRobot?.tools_len * Math.Sin(0.2 / 180 * Math.PI);//5轴要至少0.2度的偏差，否则就当作是0
            double a4;

            if (Math.Abs(new_x) < minLen && Math.Abs(new_y) < minLen)
            {
                a4 = 0;
            }
            else
            {
                a4 = -Math.Atan2(new_y, new_x);// - Math.PI;
            }
            a4 = Main.angle_process(a4);
            //Main.WriteLine(pNode, "4=" + a4 + ",=" + Tools.to_degree(a4));

            new_x = v_ba.DotProduct(v_bc.Normalize());
            try
            {
                new_y = v_ba.DotProduct(v_ga.Normalize());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            double a5 = Math.Atan2(new_y, new_x) - Math.PI;
            a5 = Main.angle_process(a5);//第五轴角度

            angle_rad[0] = (float)a1;
            angle_rad[1] = (float)a2;
            angle_rad[2] = (float)a3;

            if (a4 > Math.PI * four_axis_max)
            {
                a4 -= Math.PI;
                a5 = -a5;
            }
            else if (a4 < Math.PI * four_axis_min)
            {
                a4 += Math.PI;
                a5 = -a5;
            }

            angle_rad[3] = (float)a4;
            angle_rad[4] = (float)a5;

            angle_rad[5] = 0;

            ////////////////////////
            var AB = (B - A).normalize();

            var F7 = 约束面法向量;

            C_Point3D V7 = F7.crossProduct(AB).normalize();//期待的轴7的方向


            (bool bError, Vector3[] bottom_L, Vector3[] rotate_L) = Main.验算ABC(pRobot, pNode, angle_rad, A, B, C2, C, D,null);

            var R7 = new C_Point3D(rotate_L[6].X, rotate_L[6].Y, rotate_L[6].Z).normalize();

            double db_cos = V7.x * R7.x + V7.y * R7.y + V7.z * R7.z;

            float[] angle_rad1 = new float[6];
            float[] angle_rad2 = new float[6];

            for(var i = 0; i < 6; i++)
            {
                angle_rad1[i] = angle_rad[i];
                angle_rad2[i] = angle_rad[i];
            }
            if (db_cos != 0)
            {
                if (db_cos > 1) db_cos = 1;
                if (db_cos < -1) db_cos = -1;
                angle_rad1[5] = angle_rad[5] + (float)Math.Acos(db_cos);
                angle_rad2[5] = angle_rad[5] - (float)Math.Acos(db_cos);
            }

            (_, _, Vector3[] rotate_L2_a) = Main.验算ABC(pRobot, pNode, angle_rad1, A, B, C2, C, D, null);
            var R7_new1 = new C_Point3D(rotate_L2_a[6].X, rotate_L2_a[6].Y, rotate_L2_a[6].Z).normalize();
            double aa1 = V7 * R7_new1;

            (_, _, Vector3[] rotate_L2_b) = Main.验算ABC(pRobot, pNode, angle_rad2, A, B, C2, C, D, null);
            var R7_new2 = new C_Point3D(rotate_L2_b[6].X, rotate_L2_b[6].Y, rotate_L2_b[6].Z).normalize();
            double aa2 = V7 * R7_new2;

            //Console.WriteLine(aa1+","+aa2);

            if (aa1 > aa2)
            {
                for (var i = 0; i < 6; i++)
                {
                    angle_rad[i] = angle_rad1[i];
                }
            }
            else
            {
                for (var i = 0; i < 6; i++)
                {
                    angle_rad[i] = angle_rad2[i];
                }
            }

            {
                Main.验算ABC(pRobot, pNode, angle_rad, A, B, C2, C, D, K);
            }


            angle_rad[5] += angle_dif / 180 * (float)Math.PI;//加上六轴偏移角度

            float[] angle = new float[6];

            angle[0] = angle_rad[0] * 180 / (float)Math.PI;
            angle[1] = angle_rad[1] * 180 / (float)Math.PI;
            angle[2] = angle_rad[2] * 180 / (float)Math.PI;
            angle[3] = angle_rad[3] * 180 / (float)Math.PI;
            angle[4] = angle_rad[4] * 180 / (float)Math.PI;
            angle[5] = angle_rad[5] * 180 / (float)Math.PI;


            if (bError == false)
            {
                if (angle[0] < -170 || angle[0] > 170)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度1 超范围！"),D);
                }
                if (angle[1] < -90 || angle[1] > 155)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度2 超范围！"),D);
                }
                if (angle[2] < -85 || angle[2] > 150)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度3 超范围！"),D);
                }
                if (angle[3] < -200 || angle[3] > 200)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度4 超范围！"),D);
                }
                if (angle[4] < -150 || angle[4] > 150)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度5 超范围！"),D);
                }

                if (angle[5] > 180)
                {
                    angle[5] -= 360;
                }
                else if (angle[5] < -180)
                {
                    angle[5] += 360;
                }
                return (new C_Result(string.Join(",", angle), A, B, C2, C, D),D);
            }
            Main.WriteLine(pNode, "@ 验算有问题！");

            Main.WriteLine(pNode, "angle=========================");
            for (var i = 0; i <= 5; i++)
            {
                Main.WriteLine(pNode, i + "=" + angle_rad[i]);
            }

            Main.验算ABC(pRobot, pNode, angle_rad, A, B, C2, C, D, K);

            return (new C_Result("", A, B, C2, C, D, "验算有问题！"),D);
        }


        public static float six_max(string six1, string six2)
        {
            string[] strSplit1 = six1.Split(",");
            string[] strSplit2 = six2.Split(",");
            float max = float.MinValue;
            for (var i = 0; i < Math.Min(strSplit1.Length, strSplit2.Length); i++)
            {
                float a1 = float.Parse(strSplit1[i]);
                float a2 = float.Parse(strSplit2[i]);
                if (Math.Abs(a1 - a2) > max)
                {
                    max = Math.Abs(a1 - a2);
                }
            }

            return max;
        }


        public static List<C_Point3D>? 计算D和C2_C以及是否超范围(
            C_Space space,
            C_Node pNode,
            C_Robot? pRobot, int Group_ID, C_Point3D A, C_Point3D B)
        {
            C_Point3D? D = Main.计算D坐标(pRobot, B);// pPlanet.arm_point_b);

            if (D == null)
            {
                return new List<C_Point3D>();
            }
            List<C_Point3D>? arr = Main.计算C2和C坐标(pNode, pRobot, Group_ID, B, D);

            return arr;
        }


        /// <summary>
        /// 计算xy所在平面投影下的其他坐标 
        /// ODB刚好是一个直角三角形
        /// OB=sqrt(Bx*Bx+By*By)
        /// OD=r1
        /// alpha=DOB=acos(alpha) 另一个解 -acos(alpha)暂时不用
        /// </summary>
        /// <returns></returns>
        public static (C_Point3D D,C_Point3D D2) 计算D坐标_Dazu(C_Point3D B, double Dz,double r1= 216) //,double r2=264)
        {
            //B在DH上的投影为B2
            C_Point3D B2 = new C_Point3D(B.x, B.y, Dz);
            double len_ob2 = Math.Sqrt(B2.x * B2.x + B2.y * B2.y);
            if (len_ob2 < r1)
            {
                return (null, null);
            }
            double alpha = Main.my_acos(r1 / len_ob2);

            Vector2D OB2 = new Vector2D(B.x, B.y);

            Vector2D OD = OB2.Normalize().Rotate(Angle.FromRadians(alpha)).ScaleBy(r1);

            //OD2是另一个解
            Vector2D OD2 = OB2.Normalize().Rotate(Angle.FromRadians(-alpha)).ScaleBy(r1);

            return (new C_Point3D(OD.X, OD.Y, Dz), new C_Point3D(OD2.X, OD2.Y, Dz));
        }




        public static (C_Result pResult, double len1, double len2) 根据坐标计算6轴角度(
            string direction,
            bool Angle_Modify,
            C_Space space,
            C_Node pNode,
            C_Robot? pRobot,
            int GroupID,
            C_Point3D A,
            C_Point3D B,
            C_Point3D D,
            string file)
        {


            pNode.save_var("_A_string", "string", A.ToString());
            pNode.save_var("_B_string", "string", B.ToString());
            pNode.save_var("_D_string", "string", D.ToString());


            List<C_Point3D>? arr = Main_Robot.计算D和C2_C以及是否超范围(space, pNode, pRobot, GroupID, A, B);

            if (arr == null || arr?.Count == 0)
            {
                Main.WriteLine(pNode, "GroupID=" + GroupID + " 前面超范围没有过滤！");
                return (new C_Result("", A, B, null,null, D, "前面超范围没有过滤！"), 0, 0);
            }

            C_Point3D? C2 = arr?[0];
            C_Point3D? C = arr?[2];

            if (C == null || C2 == null)
            {
                Main.WriteLine(pNode, "GroupID=" + GroupID + " C,C2==null！");
                return (new C_Result("", A, B, C2, C, D, "C,C2==null！"), 0, 0);
            }

            //Main.WriteLine(pNode, "计算旋转轴角度");
            float[] angle = new float[6];
            double a1 = Math.Atan2(D.y, D.x);//第一个轴
            a1 = Main.angle_process(a1);
            //Main.WriteLine(pNode, "1=" + a1 + ",=" + Tools.to_degree(a1));

            C_Point3D vector_dc2 = C2 - (D);
            C_Point3D vector_od_in_z0 = D - (new C_Point3D(0, 0, D.z)) - (new C_Point3D(0, 0, 0));//od 在z=0平面的投影
            double len_vector_dc2_in_z0 = vector_dc2 * (vector_od_in_z0.normalize());

            double new_y = len_vector_dc2_in_z0;// Math.Sqrt(vector_dc2.x * vector_dc2.x + vector_dc2.y * vector_dc2.y);
            double new_x = vector_dc2.z;
            double a2 = Math.Atan2(new_y, new_x); //Math.PI / 2 - Math.Atan2(new_y, new_x);
            a2 = Main.angle_process(a2);

            //Main.WriteLine(pNode, "2=" + a2 + ",=" + Tools.to_degree(a2));


            C_Point3D? vector_cb = B - (C);
            C_Point3D? vector_c2c = C - (C2);

            Vector3D v_dc2 = new Vector3D(vector_dc2.x, vector_dc2.y, vector_dc2.z);
            Vector3D v_c2c = new Vector3D(vector_c2c.x, vector_c2c.y, vector_c2c.z);

            C_Point3D Z = new C_Point3D(D.x, D.y, 0);

            C_Point3D vector_oz = Z;
            Vector3D v_oz = new Vector3D(vector_oz.x, vector_oz.y, vector_oz.z);
            Vector3D v_c2r = v_dc2.CrossProduct(v_oz); //v_dc2.CrossProduct(v_z2);

            if (v_dc2.DotProduct(new Vector3D(0, 0, 1)) < 0)
            {
                v_c2r = -v_c2r;
            }

            UnitVector3D J = v_dc2.CrossProduct(v_c2r).Normalize();

            new_x = v_c2c.DotProduct(v_dc2.Normalize());
            new_y = v_c2c.DotProduct(J);

            double a3 = Math.Atan2(new_y, new_x);
            a3 = Main.angle_process(a3);
            //Main.WriteLine(pNode, "3=" + a3 + ",=" + Tools.to_degree(a3));

            C_Point3D vector_ba = B - (A);
            C_Point3D vector_bc = B - (C);

            Vector3D v_ba = new Vector3D(vector_ba.x, vector_ba.y, vector_ba.z);
            Vector3D v_bc = new Vector3D(vector_bc.x, vector_bc.y, vector_bc.z);

            var len_bg = v_ba.DotProduct(v_bc.Normalize());

            //G
            C_Point3D G = B + (C - B) * (float)(len_bg / v_bc.Length);

            //J
            C_Point3D vector_cd = C - (D);
            Vector3D v_cb = new Vector3D(vector_cb.x, vector_cb.y, vector_cb.z);
            Vector3D v_cd = new Vector3D(vector_cd.x, vector_cd.y, vector_cd.z);
            UnitVector3D v_cj = v_cb.CrossProduct(v_cd).Normalize();
            v_cj.CrossProduct(v_cb);
            UnitVector3D v_gk = v_cj;
            C_Point3D vector_gc = G - (C);
            Vector3D v_gc = new Vector3D(vector_gc.x, vector_gc.y, vector_gc.z);
            UnitVector3D v_gh = v_gk.CrossProduct(v_gc).Normalize();

            C_Point3D vector_ga = G - (A);
            Vector3D v_ga = new Vector3D(vector_ga.x, vector_ga.y, vector_ga.z);
            new_x = v_ga.DotProduct(v_gh);
            new_y = v_ga.DotProduct(v_gk);

            var minLen = pRobot?.tools_len * Math.Sin(0.2 / 180 * Math.PI);//5轴要至少0.2度的偏差，否则就当作是0
            double a4;

            if (Math.Abs(new_x) < minLen && Math.Abs(new_y) < minLen)
            {
                a4 = 0;
            }
            else
            {
                a4 = -Math.Atan2(new_y, new_x);// - Math.PI;
            }
            a4 = Main.angle_process(a4);
            //Main.WriteLine(pNode, "4=" + a4 + ",=" + Tools.to_degree(a4));

            new_x = v_ba.DotProduct(v_bc.Normalize());
            new_y = v_ba.DotProduct(v_ga.Normalize());
            double a5 = Math.Atan2(new_y, new_x) - Math.PI;
            a5 = Main.angle_process(a5);
            //Main.WriteLine(pNode, "5=" + a5 + ",=" + Tools.to_degree(a5));

            angle[0] = (float)a1;
            angle[1] = (float)a2;
            angle[2] = (float)a3;

            if (a4 > Math.PI / 2)
            {
                a4 -= Math.PI;
                a5 = -a5;
            }
            else if (a4 < -Math.PI / 2)
            {
                a4 += Math.PI;
                a5 = -a5;
            }

            angle[3] = (float)a4;
            angle[4] = (float)a5;

            //(angle[5], double len1, double len2) = 计算第六轴角度(GroupID, pNode, A, B, C, list_arm);
            angle[5] = 0;

            //Main.WriteLine(pNode, "角度6=" + angle[5]);

            //angle[5] += angle_dif / 180 * Math.PI; ;

            (bool bError, Vector3[] bottom_L, _) = Main.验算ABC(pRobot, pNode, angle, A, B, C2, C, D,null);

            if (bError == false)
            {
                //Main.WriteLine(pNode, "6=" + angle[5] + ",=" + Tools.to_degree(angle[5]));

                angle[0] = angle[0] * 180 / (float)Math.PI;
                angle[1] = angle[1] * 180 / (float)Math.PI;
                angle[2] = angle[2] * 180 / (float)Math.PI;
                angle[3] = angle[3] * 180 / (float)Math.PI;
                angle[4] = angle[4] * 180 / (float)Math.PI;
                angle[5] = angle[5] * 180 / (float)Math.PI;

                if (Angle_Modify)
                {
                    //用 bottom_L[6],bottom_L[5] 判断吸盘是否朝上

                    if (direction == "-x")
                    {
                        if (bottom_L[6].Z < bottom_L[5].Z)
                        {
                            angle[5] += 180; //保证bottom_6一直在上面
                        }
                    }
                    else if (direction == "z")
                    {
                        if (bottom_L[6].X < bottom_L[5].X)
                        {
                            angle[5] += 180; //保证bottom_6一直在上面
                        }
                    }
                    else if (direction == "zy")
                    {
                        if (bottom_L[6].Y < bottom_L[5].Y)
                        {
                            angle[5] += 180; //保证bottom_6一直在上面
                        }
                    }
                }


                if (angle[0] < -170 || angle[0] > 170)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度1 超范围！"), 0, 0);
                }
                if (angle[1] < -90 || angle[1] > 155)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度2 超范围！"), 0, 0);
                }
                if (angle[2] < -85 || angle[2] > 150)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度3 超范围！"), 0, 0);
                }
                if (angle[3] < -200 || angle[3] > 200)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度4 超范围！"), 0, 0);
                }
                if (angle[4] < -150 || angle[4] > 150)
                {
                    return (new C_Result("", A, B, C2, C, D, "角度5 超范围！"), 0, 0);
                }

                if (angle[5] > 180)
                {
                    angle[5] -= 360;
                }
                else if (angle[5] < -180)
                {
                    angle[5] += 360;
                }
                string strLine = string.Join(",", angle);

                return (new C_Result(strLine, A, B, C2, C, D), 0, 0);
            }
            Main.WriteLine(pNode, "@ 验算有问题！");

            Main.WriteLine(pNode, "angle=========================");
            for (var i = 0; i <= 5; i++)
            {
                Main.WriteLine(pNode, i + "=" + angle[i]);
            }

            Main.WriteLine(pNode, "A=" + A.ToString2());
            Main.WriteLine(pNode, "B=" + B.ToString2());
            Main.WriteLine(pNode, "C2=" + C2.ToString2());
            Main.WriteLine(pNode, "C=" + C.ToString2());
            Main.WriteLine(pNode, "D=" + D.ToString2());

            Main.验算ABC(pRobot, pNode, angle, A, B, C2, C, D, null);

            return (new C_Result("", A, B, C2, C, D, "验算有问题！"), 0, 0);
        }


        /// <summary>
        /// https://www.geogebra.org/3d/vvzpzwmz
        /// </summary>
        /// <param name="pRobot"></param>
        /// <param name="A"></param>
        /// <param name="D"></param>
        /// <param name="B"></param>
        /// <param name="r1"></param>
        /// <returns></returns>
        public static C_Point3D? 计算G坐标_Dazu(C_Point3D A, C_Point3D D,double h=185.7)//, double r1)
        {
            //DHJ所在平面来计算,这个平面的x，y分量为：
            //Vx = DH.Normalize() = cross(Z ,DO).Normalize()
            //Vy = DJ.Normalize() = cross(DO,DH).Normalize()
            //Vz = DO.Normalize() = DO.Normailize()
            //G_tou(投影面坐标) = (dot(DA,Vx),dot(DA,Vy))
            //DG = G_tou.x*Vx+G_tou.y*Vy
            //GA = dot(DA,Vz)*DO

            C_Point3D O = new C_Point3D(0, 0, h);//O不是原点
            C_Point3D Z = new C_Point3D(0, 0, 1);

            C_Point3D DO = O - D;
            C_Point3D OD = D - O;

            var Vz = DO.normalize();
            var Vx = Z.crossProduct(Vz).normalize();
            var Vy = DO.crossProduct(Vx).normalize();
            var DA = A - D;
            var G_tou = new C_Point3D(DA * Vx, DA * (Vy), 0);//A到平面的投影是G
            var DG = Vx * G_tou.x + Vy * G_tou.y;
            var OG = DG + OD;
            return OG+O;
        }



        // 定义从 Point2D 到 Point3D 的隐式转换
        public static Vector3 Point3D_To_Vector3(C_Point3D point)
        {
            return new Vector3(point.x, point.y, point.z); // Z 轴设为 0
        }



        public static C_Point3D Vector3_To_Point3D(Vector3 v)
        {
            return new C_Point3D(v.X, v.Y, v.Z);
        }



        public static (List<C_Point3D> c, C_Point3D Vx, C_Point3D Vy, C_Point3D Vz)
            计算C坐标_Dazu(C_Point3D D, C_Point3D K, float h, float r1, float r2) //=185.7f,= 850,= 791.5f
        {

            C_Point3D point_O = new C_Point3D(0, 0, h);//O不是原点!!!!!
            C_Point3D Z = new C_Point3D(0, 0, 1);

            C_Point3D DO = point_O - D;
            //C_Point3D OD = D;

            var Vz = DO.normalize();
            var Vx = Z.crossProduct(DO).normalize();
            var Vy = DO.crossProduct(Vx).normalize();
            C_Point3D DK = K - D;

            //先计算投影到这个平面的坐标
            double Kx = DK * Vx;
            double Ky = DK * Vy;

            C_Point3D v2_D = new C_Point3D(0, 0, 0);

            List<C_Point3D> list_c = Main.计算两个圆的交点(v2_D, new C_Point3D(Kx, Ky, 0), r1, r2);
            if (list_c == null)
            {
                return (null,null,null,null);
            }

            //然后把投影坐标变回来

            List<C_Point3D> list_result = new List<C_Point3D>();

            for (var i = 0; i < list_c.Count; i++)
            {
                C_Point3D item = list_c[i];
                C_Point3D item_new = Vx * item.x + Vy * item.y + D;
                list_result.Add(item_new);

                float distance = item_new.distance(K);
                if ((distance - 791.5)>1)
                {
                    Console.WriteLine("error!");
                }

            }


            return (list_result,Vx,Vy,Vz);
        }


        public static C_Point3D? 摄像头坐标转到机械臂坐标(
            C_Matrix m1,
            C_Point3D pPoint)
        {

            return Main_Robot.Convert_Matrix(m1, pPoint);
        }

        public static C_Point3D? Convert_Matrix_Invert(
            C_Matrix p1,
            C_Point3D pPoint)
        {
            if (pPoint == null)
            {
                Main.WriteLine("机械臂坐标到摄像头坐标 pPoint == null！");
                return null;
            }
            if (p1.p2_center == null)
            {

                Main.WriteLine("机械臂坐标到摄像头坐标 p1.p2_center == null！");
                return null;
            }

            double[,] d2 = new double[3, 1];
            d2[0, 0] = pPoint.x - p1.p2_center.x;
            d2[1, 0] = pPoint.y - p1.p2_center.y;
            d2[2, 0] = pPoint.z - p1.p2_center.z;

            DenseMatrix M = p1.M_Rotate;
            DenseMatrix X = DenseMatrix.OfArray(d2);

            Matrix<double> m = M.Inverse().Transpose().Multiply(X);

            C_Point3D pPoint2 = new C_Point3D(
                m[0, 0] + p1.p1_center.x,
                m[1, 0] + p1.p1_center.y,
                m[2, 0] + p1.p1_center.z);
            return pPoint2;
        }



        public static C_Point3D? Convert_Matrix(
            C_Matrix m3,
            C_Point3D pPoint)
        {

            if (m3.p1_center == null) return null;
            if (m3.p2_center == null) return null;

            double[,] d2 = new double[3, 1];
            d2[0, 0] = pPoint.x - m3.p1_center.x;
            d2[1, 0] = pPoint.y - m3.p1_center.y;
            d2[2, 0] = pPoint.z - m3.p1_center.z;

            DenseMatrix X = DenseMatrix.OfArray(d2);
            Matrix<double> Y =  m3.M_Rotate.Transpose().Multiply(X);

            C_Point3D pPoint2 = new C_Point3D(
                Y[0, 0] + m3.p2_center.x,
                Y[1, 0] + m3.p2_center.y,
                Y[2, 0] + m3.p2_center.z);
            return pPoint2;
        }

        /// <summary>
        /// 通过转换矩阵计算从机械臂坐标系到摄像头坐标系
        /// </summary>
        /// <param name="pPoint"></param>
        /// <returns></returns>
        public static C_Point3D? 机械臂坐标到摄像头坐标(
            C_Matrix m1,
            C_Point3D pPoint)
        {
            return Convert_Matrix_Invert(m1, pPoint);
        }

        public static float[] six_to_array(string six_angles)
        {
            float[] angles = new float[6];

            string[] strSplit = six_angles.Split(",");
            for (var x = 0; x < 6; x++)
            {
                angles[x] = float.Parse(strSplit[x]);
            }
            return angles;
        }

        public static float[] 计算中间六轴(float[] angles1, float[] angles2, float d1)
        {
            float[] angles = new float[6];

            for (var i = 0; i < 6; i++)
            {
                angles[i] = angles1[i] * d1 + angles2[i] * (1 - d1);
            }
            return angles;
        }

        public static string 计算最近的六轴(List<string> list,C_Three_Point p1)
        {
            double min = double.MaxValue;
            string line = "";
            for (int i = 0; i < list.Count; i++)
            {
                double six_length = Main_Robot.six_max(p1.six, list[i]);
                if (six_length < min)
                {
                    min = six_length;
                    line = list[i];
                }
            }
            return line;
        }
    }
}
