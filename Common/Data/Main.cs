
using B2_Token.Funny;
using Common_Robot2;
using ConverxHull;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Microsoft.VisualBasic.FileIO;
using NAudio.Wave.SampleProviders;
using Newtonsoft.Json.Linq;
using OuelletConvexHull;
using palletizing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing.Imaging;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Speech.Synthesis;
using System.Text;
using Test1.Common.Data;
using Three.Net.Math;
using Three.Net.Objects;
using Windows.Devices.Pwm;
using File = System.IO.File;
using Vector3 = System.Numerics.Vector3;

namespace Test1
{
    public partial class Main
    {
        C_Space space;
        C_Node pNode;

        public static int x_distance = 850;

        public Main(C_Space space,C_Node pNode)
        {
            this.space = space;
            this.pNode = pNode;
        }


        public static string TimeNow()
        {
            return DateTime.Now.ToString("mm:ss:fff");
        }

        public static double time1 = 0;
        public static void Time_Start()
        {
            DateTime DateTime1 = DateTime.Now; //仅从当前时间获得毫秒
            time1 = DateTime1.Ticks;
        }


        public static void create_directory(string path_code)
        {
            if (Directory.Exists(path_code) == false)
            {
                Directory.CreateDirectory(path_code);
            }
        }


        public static string FileToBase64Str(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);// 读取文件
            string base64 = Convert.ToBase64String(bytes);// 转换为 base64
            return base64;
        }
        public static void Time_Print(C_Node pNode, string tag)
        {
            DateTime DateTime2 = DateTime.Now; //仅从当前时间获得毫秒
            double time2 = (DateTime2.Ticks - time1) / 10000;
            Main.WriteLine(pNode," End " + tag + " 毫秒= " + time2);
        }



        public static List<C_Point3D> 中心点附近x毫米范围的点(
            double distance,
            List<C_Point3D> pList_Point_Filter, C_Point3D p1)
        {
            List<C_Point3D> list = new List<C_Point3D>();
            for (int i = 0; i < pList_Point_Filter.Count; i++)
            {
                C_Point3D c_Point3D = pList_Point_Filter[i];
                float num = Math.Abs(c_Point3D.vector.X - p1.vector.X);
                float num2 = Math.Abs(c_Point3D.vector.Y - p1.vector.Y);
                double num3 = Math.Sqrt(num * num + num2 * num2);
                if (num3 < distance) // 50.0)
                {
                    list.Add(c_Point3D);
                }
            } 

            return list;
        }

        public static C_Point3D 平面拟合计算法向量等(List<C_Point3D> pList_Point, bool b_auto_correct_z = true)
        {
            Matrix<double> matrix = 平面拟合计算法向量等_sub(pList_Point);
            double num = matrix[0, 0];
            double num2 = matrix[1, 0];
            //double num3 = matrix[2, 0];
            //double num4 = -1.0;
            //double num5 = Math.Sqrt(num * num + num2 * num2 + num4 * num4);
            double x = num;// / a;
            double y = num2;// / b;
            double z = 1.0 - num - num2;// / c;
            C_Point3D c_Point3D = new C_Point3D(x, y, z);

            return c_Point3D.normalize();
        }


        /// <summary>
        /// https://blog.csdn.net/tjuzhsir/article/details/112799809
        /// z分量也可能==0，默认z分量不等于，有漏洞
        /// ax+by+cz+d=0
        /// 假设 a+b+c=1 ,上面系数缩放不影响
        /// c=1-a-b
        /// ax+by+(1-a-b)z+d=0
        /// a(x-z)+b(y-z)+d=-1*z
        /// a,b,d是未知数的方程
        /// </summary>
        /// <param name="pList_Point"></param>
        /// <returns></returns>
        public static Matrix<double> 平面拟合计算法向量等_sub(List<C_Point3D> pList_Point)
        {
            double[,] array = new double[pList_Point.Count, 3];
            double[,] array2 = new double[pList_Point.Count, 1];
            for (int i = 0; i < pList_Point.Count; i++)
            {
                C_Point3D point = pList_Point[i];
                array[i, 0] = (point.x - point.z);
                array[i, 1] = (point.y - point.z);
                array[i, 2] = 1.0;
                array2[i, 0] = -1 * point.z;
            }

            DenseMatrix denseMatrix = DenseMatrix.OfArray(array);
            DenseMatrix other = DenseMatrix.OfArray(array2);
            return denseMatrix.Transpose().Multiply(denseMatrix).Inverse()
                .Multiply(denseMatrix.Transpose())
                .Multiply(other);
        }



        public static bool compare_box_right(C_BoxInfo 要检测的盒子, List<C_Point3D> cloud1)
        {
            //现在抓取方向是z，否中用另一个模块
            C_Point3D A = 要检测的盒子.center;

            //判断右边是否有点云阻挡
            C_Point3D min = A+(new C_Point3D(-200, 0, 0));
            min.y = 要检测的盒子.y_min - 200;

            if (要检测的盒子.z_max- 要检测的盒子.z_min < 150)
            {
                min.z = Math.Min(要检测的盒子.z_min, 要检测的盒子.z_max - 200) + 20;// 要检测的盒子.z_min + 20;
            }
            else
            {
                min.z = 要检测的盒子.z_min + 20;
            }
                

            C_Point3D max = A+(new C_Point3D(100, 0, 130));
            max.y = 要检测的盒子.y_min - 30;

            List<C_Point3D> cloud2 = Main.cloud_filter(cloud1, "x", min.x, max.x);
            List<C_Point3D> cloud3 = Main.cloud_filter(cloud2, "y", min.y, max.y);
            List<C_Point3D> cloud4 = Main.cloud_filter(cloud3, "z", min.z, max.z);

            Main.save_cloud("D:\\cloud\\gid_" + 要检测的盒子.Group_ID + ".txt", cloud4);

            return cloud4.Count > 20;
        }


        public static  bool compare_box_left(C_BoxInfo 要检测的盒子, List<C_Point3D> cloud1)
        {
            //现在抓取方向是z，否中用另一个模块
            C_Point3D A = 要检测的盒子.center;

            //判断左边是否有点云阻挡

            C_Point3D min = A+(new C_Point3D(-200, 0, 0));
            min.y = 要检测的盒子.y_max + 30;
            
            if (要检测的盒子.z_max - 要检测的盒子.z_min < 150)
            {
                min.z = Math.Min(要检测的盒子.z_min, 要检测的盒子.z_max - 200) + 20;// 要检测的盒子.z_min + 20;
            }
            else
            {
                min.z = 要检测的盒子.z_min + 20;
            }

            C_Point3D max = A+(new C_Point3D(100, 0, 130));// 450));
            max.y = 要检测的盒子.y_max + 200;

            List<C_Point3D> cloud2 = Main.cloud_filter(cloud1, "x", min.x, max.x);
            List<C_Point3D> cloud3 = Main.cloud_filter(cloud2, "y", min.y, max.y);
            List<C_Point3D> cloud4 = Main.cloud_filter(cloud3, "z", min.z, max.z);

            Main.save_cloud("D:\\cloud\\gid_" + 要检测的盒子.Group_ID + ".txt", cloud4);
            return cloud4.Count > 20;

        }

        public static bool compare_box_up(C_BoxInfo 要检测的盒子, List<C_Point3D> cloud1,float  y_max_minus=0,float y_min_add=0)
        {
            //现在抓取方向是z，否中用另一个模块
            C_Point3D A = 要检测的盒子.center;

            //判断上面是否有箱子压着

            C_Point3D min = A+(new C_Point3D(-200, 0, 0));
            min.z = 要检测的盒子.z_max + 30;// - 200;
            min.y = 要检测的盒子.y_min+ y_min_add;

            C_Point3D max = A+(new C_Point3D(100, 0, 0));
            max.z = 要检测的盒子.z_max + 200;
            max.y = 要检测的盒子.y_max + y_max_minus;

            List<C_Point3D> cloud2 = Main.cloud_filter(cloud1, "x", min.x, max.x);
            List<C_Point3D> cloud3 = Main.cloud_filter(cloud2, "y", min.y, max.y);
            List<C_Point3D> cloud4 = Main.cloud_filter(cloud3, "z", min.z, max.z);

            //Main.save_cloud("D:\\cloud\\gid_" + 要检测的盒子.Group_ID + "_cloud3.txt", cloud3);
            //Main.save_cloud("D:\\cloud\\gid_" + 要检测的盒子.Group_ID + ".txt", cloud4);

            return cloud4.Count > 20;

        }

        public static List<C_Point3D> cloud_read(C_Node pNode,string file,string seperate)
        {
            List<C_Point3D> pList = new List<C_Point3D>();
            try
            {
                using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
                {
                    string? line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Replace("，", ",");
                        if (line.StartsWith("//"))
                        {
                            continue;
                        }
                        string[] strSplit = line.Split(seperate.ToCharArray());
                        if (strSplit.Length > 1)
                        {
                            float x = float.Parse(strSplit[0]);
                            float y = float.Parse(strSplit[1]);
                            float z = 0;
                            if (strSplit.Length > 2)
                            {
                                z = float.Parse(strSplit[2]);
                            }
                            C_Point3D p = new C_Point3D(x, y, z);

                            if (strSplit.Length > 5)
                            {
                                var r = byte.Parse(strSplit[3]);
                                var g = byte.Parse(strSplit[4]);
                                var b = byte.Parse(strSplit[5]);
                                p.pExtend = new C_Color(r, g, b);
                            }
                            pList.Add(p);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Main.WriteLine(pNode,"The file could not be read:");
                MessageBox.Show(e.Message);
            }
            return pList;
        }


        public static (List<C_Point3D> list_camera, List<C_Point3D> list_arm) check_in_out(int Group_ID,
            List<C_Point3D> edges, 
            List<C_Point3D> list_point_camera,
            List<C_Point3D> list_point_arm, 
            List<C_Point3D> list_uv)
        {
            List<C_Point3D> list_camera2 = new List<C_Point3D>();
            List<C_Point3D> list_arm2 = new List<C_Point3D>();

            ConcurrentBag<C_Point3D> list_camera = new ConcurrentBag<C_Point3D>();
            try
            {
                Parallel.For(0, list_uv.Count, i => {
                    C_Point3D uv = list_uv[i];
                    if (uv!=null && Main.判断是否在多边形内(edges, uv.x, uv.y))
                    {
                        list_point_camera[i].Group_ID = Group_ID;

                        C_Point3D pCamera = list_point_camera[i];
                        C_Point3D pArm = list_point_arm[i];
                        pCamera.convert = pArm;

                        list_camera.Add(pCamera);
                    }
                });


                C_Point3D[] array = list_camera.ToArray();
                foreach (var item in array)
                {
                    if (item != null)
                    {
                        list_camera2.Add(item);
                        list_arm2.Add(item.convert);
                    }
                }


            }
            catch (Exception e)
            {
                Main.WriteLine(e.ToString());
            }


            return (list_camera2, list_arm2);
        }

        public static void Add_Panel(Form pFrm,string dock, string group,C_Space space, System.Windows.Forms.Control button)
        {
            if (space.vars_ui.ContainsKey(group) == false)
            {
                MessageBox.Show("没有这个group=" + group);
                return;
            }

            var pParent = space.vars_ui[group];
            if (pParent.GetType().Name == "S_Form")
            {
                pFrm.Controls.Add(button);
            }
            else if (pParent.GetType().Name == "S_Panel_Map")
            {
                Add_Panel_Sub(space, pParent, button);
            }



            button.BringToFront();
            switch (dock)
            {
                case "left":
                    button.Dock = DockStyle.Left;
                    break;
                case "right":
                    button.Dock = DockStyle.Right;
                    break;
                case "top":
                    button.Dock = DockStyle.Top;
                    break;
                case "bottom":
                    button.Dock = DockStyle.Bottom;
                    break;
                case "fill":
                    button.Dock = DockStyle.Fill;
                    break;
            }
        }


        public static void Add_Panel_Sub(C_Space space,S_UI pParent,System.Windows.Forms.Control button)
        {
            if (space.vars_ui.ContainsKey(pParent.key_parent) == false)
            {
                MessageBox.Show("没有这个group=" + pParent.key_parent);
                return;
            }
            var pParent2 = space.vars_ui[pParent.key_parent];
            if (pParent2.GetType().Name == "S_Tab_View")
            {
                S_Panel_Map pMap = (S_Panel_Map)pParent;
                S_Tab_View tab_view = (S_Tab_View)pParent2;

                int index = int.Parse(pMap.dock_rank) - 1;

                while (index > tab_view.ui_item.Controls.Count-1)
                {
                    var page2 = new TabPage();
                    tab_view.ui_item.Controls.Add(page2);
                }

                TabPage page  = (TabPage)tab_view.ui_item.Controls[index];
                if (page!=null) //.ui_item.Controls.Count > index)
                {
                    page.Text = pMap.Name;
                    page.Tag = index;
                    page.Controls.Add(button);
                }
            }
            else if (pParent2.GetType().Name == "S_Split_View")
            {
                S_Panel_Map pMap = (S_Panel_Map)pParent;
                S_Split_View pParent3 = (S_Split_View)pParent2;
                if (pMap.dock_rank == "1")
                {
                    try
                    {
                        pParent3.ui_item.SplitterDistance = Math.Max(pParent3.ui_item.Panel1MinSize, pParent3.ui_item.Width * pMap.width / pParent2.width);
                        
                    }
                    catch(Exception ex)
                    {

                    }
                    pParent3.ui_item.Panel1.Controls.Add(button);
                }
                else
                {
                    pParent3.ui_item.Panel2.Controls.Add(button);
                }
            }
            else
            {
                S_Panel pParent3 = (S_Panel)pParent2;
                pParent3.panel.Controls.Add(button);
            }
        }

        private static TabPage find_tab_page(S_Tab_View pParent3, int index)
        {
            for(var i=0;i< pParent3.ui_item.Controls.Count; i++)
            {
                if (pParent3.ui_item.Controls[i].Tag.ToString() == index+"")
                {
                    return (TabPage)pParent3.ui_item.Controls[i];
                }
            }
            return null;
        }

        public static string file_replace(I_Train pTrain,string file_save)
        {
            if (file_save.IndexOf("@") > -1)
            {
                string time_id = pTrain.get_Time_ID();// (string)space.vars.read_vars(pTrain, this, this.key_time_id, "string");

                if (time_id == "")
                {
                    time_id = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss__FFF");
                }
                file_save = file_save.Replace("@", time_id);
            }
            return file_save;
        }

        public static List<C_Point3D> 计算外接多边形(List<C_Point3D> pList)
        {
            int n = pList.Count;
            C_Point3D[] points = new C_Point3D[n];
            for (int j = 0; j < n; j++)
            {

                C_Point3D p = pList[j];

                C_Point3D p2 = p;
                points[j] = p2;
            }

            var windowsPoints = points.Select(p => new ConverxHull.Point(p.x, p.y)).ToList();

            var ouelletConvexHull = new ConvexHull(windowsPoints);
            ouelletConvexHull.CalcConvexHull(ConvexHullThreadUsage.OnlyOne);

            List<C_Point3D> ouelletAsVertices = ouelletConvexHull.GetResultsAsArrayOfPoint()
                .Select(p => new C_Point3D(p.X, p.Y, 0)).ToList();

            List<C_Point3D> result = new List<C_Point3D>();
            for (var i = 0; i < ouelletAsVertices.Count; i++)
            {
                C_Point3D p1 = ouelletAsVertices[i];
                //var p2 = ouelletAsVertices[i + 1];
                //e.DrawLine(new Pen(Color.Yellow), (float)p1.x, (float)p1.y, (float)p2.x, (float)p2.y);
                //currentPolygon.AddPoint(new System.Drawing.Point((int)p1.x, (int)p1.y));

                result.Add(p1);
            }
            return result;

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



        //http://csharphelper.com/blog/2014/07/determine-whether-a-polygon-is-convex-in-c/
        public static bool 判断是否在多边形内_ByAngle(List<C_Point3D> Points, double X, double Y)
        {
            int max_point = Points.Count - 1;
            double total_angle = GetAngle(
                Points[max_point].x, Points[max_point].y,
                X, Y, Points[0].x, Points[0].y);

            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(
                    Points[i].x, Points[i].y,  X, Y,
                    Points[i + 1].x, Points[i + 1].y);
            }
            return (Math.Abs(total_angle) > 1);
        }


        /// <summary>
        /// 射线法判断
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public static bool 判断是否在多边形内(List<C_Point3D> polygon, double X, double Y)
        {
            int n = polygon.Count;
            bool inside = false;

            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                // 检查点是否在多边形的边上
                if ((polygon[i].y > Y) != (polygon[j].y > Y) &&
                    (X <= (polygon[j].x - polygon[i].x) * (Y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
                {
                    inside = !inside;
                }
            }

            return inside;
        }




        // Return the angle ABC.
        // Return a value between PI and -PI.
        // Note that the value is the opposite of what you might
        // expect because Y coordinates increase downward.
        public static double GetAngle(double Ax, double Ay,
            double Bx, double By, double Cx, double Cy)
        {
            // Get the dot product.
            double dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            double cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return Math.Atan2(cross_product, dot_product);
        }

        // Return the dot product AB · BC.
        // Note that AB · BC = |AB| * |BC| * Cos(theta).
        private static double DotProduct(double Ax, double Ay,
            double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }


        // Return the cross product AB x BC.
        // The cross product is a vector perpendicular to AB
        // and BC having length |AB| * |BC| * Sin(theta) and
        // with direction given by the right-hand rule.
        // For two vectors in the X-Y plane, the result is a
        // vector with X and Y components 0 so the Z component
        // gives the vector's length and direction.
        public static double CrossProductLength(double Ax, double Ay,
            double Bx, double By, double Cx, double Cy)
        {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }




        /// <summary>
        /// 保存点云
        /// </summary>
        public static void save_cloud(string file2, C_Matrix camera1, List<C_Point3D> pList)
        {
            string time_id = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss__FFF");
            file2 = file2.Replace("@", time_id);

            Main.创建目录如果不存在(file2);

            StreamWriter file = new StreamWriter(file2);

            foreach (C_Point3D p in pList)
            {
                C_Point3D? p2=Main_Robot.摄像头坐标转到机械臂坐标(camera1, p);
                file.WriteLine(p2?.ToString());
            }
            file.Close();
        }


        public static void save_cloud3(List<C_Three_Point> pList, string file1, string file2, string file3)
        {
            string time_id = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss__FFF");
            file1 = file1.Replace("@", time_id);
            file2 = file2.Replace("@", time_id);
            file3 = file3.Replace("@", time_id);

            Main.创建目录如果不存在(file1);
            StreamWriter writer = new StreamWriter(file1);

            foreach (C_Three_Point p in pList)
            {
                writer.WriteLine(p.A?.ToString());
            }
            writer.Close();
            /////////////////////////////////////////

            Main.创建目录如果不存在(file2);
            writer = new StreamWriter(file2);

            foreach (C_Three_Point p in pList)
            {
                writer.WriteLine(p.B?.ToString());
            }
            writer.Close();
            /////////////////////////////////////////

            Main.创建目录如果不存在(file3);
            writer = new StreamWriter(file3);

            foreach (C_Three_Point p in pList)
            {
                writer.WriteLine(p.P7?.ToString());
            }
            writer.Close();
        }

        /// <summary>
        /// 保存点云
        /// </summary>
        public static string save_cloud(string file2, List<C_Point3D> pList)
        {
            string time_id = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss__fff");
            file2 = file2.Replace("@", time_id);

            Main.创建目录如果不存在(file2);
            try
            {
                StreamWriter file = new StreamWriter(file2);

                foreach (C_Point3D p in pList)
                {
                    if (p != null)
                    {
                        if (p.pExtend != null)
                        {
                            file.WriteLine(p.ToString() + "," + p.pExtend.r+"," + p.pExtend.g + "," + p.pExtend.b );
                        }
                        else
                        {
                            file.WriteLine(p.ToString());
                        }
                    }
                    else
                    {
                        Main.WriteLine("p== null");
                    }
                }
                file.Close();
            }
            catch(Exception ex)
            {

            }

            return file2.ToString();
        }


        /// <summary>
        /// 用余弦定理,三条边求3个角
        /// 在截面坐标系下计算，算完毕要转回原先坐标系，返回值 c2,c2两个解
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="r1"></param>
        /// <param name="p2_x"></param>
        /// <param name="p2_y"></param>
        /// <param name="r2"></param>
        /// <returns></returns>

        public static List<C_Point3D> 计算两个圆的交点(
            C_Point3D point_A, C_Point3D point_B,
            float r1, float r2)
        {
            // 定义三个点 A, B, C

            // 计算边的长度
            double a = r2;// Distance(B, C); // BC
            double b = r1;// Distance(A, C); // AC
            double c = point_A.distance(point_B);// Distance(A, B); // AB

            if (c > a + b || b>c+a || a>c+b)
            {
                //必须能组成三角形，否则有错误！
                return null;
            }

            // 计算角度
            double angleA = CalculateAngle(b, c, a);
            double angleB = CalculateAngle(a, c, b);
            double angleC = CalculateAngle(a, b, c);

            C_Point3D v_A = new C_Point3D(point_A.x, point_A.y, 0);
            C_Point3D v_B = new C_Point3D(point_B.x, point_B.y, 0);

            C_Point3D v_AB = v_B - v_A;// new C_Point3D(AB.x, AB.y, 0);
            C_Point3D v_AC = v_AB.rotate_xy(angleA).normalize() * r1;
            C_Point3D v_AC2 = v_AB.rotate_xy(-angleA).normalize() * r1;

            C_Point3D v_C = v_AC + v_A;
            C_Point3D v_C2 = v_AC2 + v_A;

            List<C_Point3D> arr = new List<C_Point3D>();
            arr.Add(v_C);
            arr.Add(v_C2);

            //double remain= r2- v_C.distance(v_B);
            //Console.WriteLine(remain);

            return arr;
        }


        // 使用余弦定理计算角度
        static double CalculateAngle(double a, double b, double c)
        {
            double cosAngle = (a * a + b * b - c * c) / (2 * a * b);
            if (cosAngle >= 1) return Math.Acos(1);
            if (cosAngle < -1) return Math.Acos(-1);
            return Math.Acos(cosAngle);
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

        public static List<C_Point3D> 计算两个圆的交点C2_根据C2计算C(
        C_Robot pRobot,C_Node pNode, int Group_ID,
        C_Point3D point_D, C_Point3D point_B,
        float r1, float r2)
        {
            //计算两个圆的交点C2
            List<C_Point3D> arr = Main.计算两个圆的交点(point_D,point_B,r1,r2);

            if (arr == null)
            {
                return null;
            }

            //上面两个坐标C就是立体图里面的C2，下面根据根据C2算C
            C_Point3D vector_C2B = point_B-(arr[0]);
            Vector2D v_C2B = new Vector2D(vector_C2B.x, vector_C2B.y);

            double length640 = pRobot.arm_bc;
            double length200 = pRobot.arm_cc2;

            Angle a = Angle.FromRadians(Math.Atan2(length640, length200));
            Vector2D v_C2C = v_C2B.Normalize().Rotate(a).ScaleBy(length200);
            //C=C2+向量C2C
            arr.Add(arr[0]+(new C_Point3D(v_C2C.X, v_C2C.Y, 0)));


            //上面两个坐标就是立体图里面的C2，下面根据根据C2算C
            vector_C2B = point_B-(arr[1]);
            v_C2B = new Vector2D(vector_C2B.x, vector_C2B.y);
            a = Angle.FromRadians(Math.Atan2(length640, length200));
            v_C2C = v_C2B.Normalize().Rotate(a).ScaleBy(length200);
            arr.Add(arr[1]+(new C_Point3D(v_C2C.X, v_C2C.Y, 0)));

            return arr;
        }


        public static List<C_Point3D>? 计算C2和C坐标(
            C_Node pNode, C_Robot? pRobot, 
            int Group_ID, C_Point3D B1, C_Point3D D1)
        {
            if (pRobot == null) return null;

            //根据B,D 算C2
            C_Point3D D = 转截面坐标系(D1);
            C_Point3D B = 转截面坐标系(B1);
            List<C_Point3D> arr = 计算两个圆的交点C2_根据C2计算C(
                pRobot,pNode, Group_ID, D, B, pRobot.arm_c2d, pRobot.arm_bc2);
            if (arr==null || arr.Count==0)
            {
                return arr;
            }

            double angle = Math.Atan2(B1.y, B1.x);
            for (var i = 0; i < arr.Count; i++)
            {
                arr[i] = 截面转普通坐标系(arr[i], angle);
            }
            return arr;
        }


        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

        /// <summary>
        /// Robot RcvData PLC 传输带 监听记录
        /// </summary>
        /// <param name="RData"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public void PLC_Callback(byte[] RData, int status, string message)
        {
            Main.WriteLine("当前状态：" + status);
            switch (status)
            {
                case 0:
                    string strData = Main.ToHexString(RData);
                    Main.WriteLine("收到消息：" + strData);
                    if (strData == "88020106")
                    {
                        Main.WriteLine("传输带移动成功，开始新一轮抓取");

                        //space.vars.照相机模块.开始拍照();
                    }
                    if (strData == "88020107")
                    {
                        //皮带
                        //space.vars.照相机模块.开始拍照();
                    }
                    break;
                case 1: //连接成功
                    //Main.WriteLine(
                    //    string.Format("【IP:{0} 端口:{1}】", space.vars.ip_plc, space.vars.port_plc) + "连接成功...");
                    break;
                default:
                    //断开连接
                    //Main.WriteLine(
                    //    string.Format("【IP:{0} 端口:{1}】", space.vars.ip_plc, space.vars.port_plc) + "断开连接...");

                    break;
            }
        }



        /// <summary>
        /// Robot RcvData 机械臂监听记录
        /// </summary>
        /// <param name="RData"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        public void Robot_CallBack(byte[] RData, int status, string message)
        {
            Main.WriteLine("当前状态：" + status);   
            switch (status)
            {
                case 0:
                    string str = Encoding.GetEncoding("GBK").GetString(RData);
                    if (str == "Connect sucessful")
                    {
                        Main.WriteLine("监听连接成功...");
                    }
                    else if (str == "move sucessful")
                    {
                        Main.WriteLine("抓取成功...");
                        //Tools.bRobot_Catch = true;
                        //space.vars.bPrepare_Get_Picture = true;
                    }
                    else if (str == "put sucessful")
                    {
                        space.vars.count_put += 1;//ABB

                        if (space.vars.count_put == 1)
                        {
                        }
                        else
                        {
                            int count = space.vars.count_put - 1;
                            if (count == 0) count = 1;
                            //space.vars.speed = Math.Round((space.vars.second_put + 0.0) / count, 2);
                        }

                        Main.WriteLine("放置成功...");
                    }
                    break;
                case 1: //连接成功
                    //Main.WriteLine(
                    //    string.Format("【IP:{0} 端口:{1}】", space.vars.ip_robot, space.vars.port_robot) + "连接成功...");
                    break;
                default: //断开连接
                    //Main.WriteLine(
                    //    string.Format("【IP:{0} 端口:{1}】", space.vars.ip_robot, space.vars.port_robot) + "断开连接...");
                    break;
            }
        }


        public static void CPP_Server_Start()
        {
            //监听CPP 通信
            //m1.CPP_Server.InitServer(space.vars.ip_cpp, space.vars.port_cpp);

            //Main.WriteLine(string.Format("CPP_Server 【IP:{0} 端口:{1}】", space.vars.ip_cpp, space.vars.port_cpp) + "开始监听");

        }

        public static void PLC_Start()
        {
            //监听PLC输送带
            //m1.PLC_Server.InitServer(space.vars.ip_plc, space.vars.port_plc);

            //Main.WriteLine(string.Format("PLC 【IP:{0} 端口:{1}】", space.vars.ip_plc, space.vars.port_plc) + "开始监听");

        }



        //public WebServer CreateWebServer(string url,string root_path)
        //{
        //    space.vars.pArm = new WebSocketArm("/terminal");
        //    var server = new WebServer(o => o
        //            .WithUrlPrefix(url)
        //            .WithMode(HttpListenerMode.EmbedIO))
        //        .WithIPBanning(o => o
        //            .WithMaxRequestsPerSecond()
        //            .WithRegexRules("HTTP exception 404"))
        //        .WithLocalSessionManager()
        //        //.WithWebApi("/test", m => m
        //        //    .WithController<TestController>())
        //        .WithModule(space.vars.pArm)
        //        .WithStaticFolder("/", root_path, true, m => m
        //            .WithContentCaching(false)) // Add static files after other modules to avoid conflicts
        //        .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));

        //    server.StateChanged += (s, e) => Main.WriteLine($"WebServer New State - {e.NewState}");

        //    return server;
        //}




        public static bool check_finished(List<C_Space> spaces)
        {
            for (var i = 0; i < spaces.Count; i++)
            {
                var space2 = spaces[i];
                if (space2 != null && space2.finished == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static List<C_Point3D> 生成点云(C_Point3D center, string fix = "x", int r = 200, int count = 200)
        {

            List<C_Point3D> pList2 = new List<C_Point3D>();


            for (var i = 0; i < count; i++)
            {
                C_Point3D point;
                if (fix == "x" || fix == "-x")
                {
                    Random random = new Random();
                    double y = center.y + r * Math.Cos(random.NextDouble());
                    double z = center.z + r * Math.Sin(random.NextDouble());
                    point = new C_Point3D(center.x, y, z);
                    pList2.Add(point);
                }
                else if (fix == "y")
                {
                    Random random = new Random();
                    double x = center.x + r * Math.Cos(random.NextDouble());
                    double z = center.z + r * Math.Sin(random.NextDouble());
                    point = new C_Point3D(x, center.y, z);
                    pList2.Add(point);
                }
                else if (fix == "z")
                {
                    Random random = new Random();
                    double y = center.y + r * Math.Cos(random.NextDouble());
                    double x = center.x + r * Math.Sin(random.NextDouble());
                    point = new C_Point3D(x, y, center.z);
                    pList2.Add(point);
                }
            }


            if (fix == "x" || fix == "-x")
            {
                C_Point3D point = new C_Point3D(center.x, center.y - 2 * r, center.z - r);
                pList2.Add(point);
                point = new C_Point3D(center.x, center.y + 2 * r, center.z - r);
                pList2.Add(point);
                point = new C_Point3D(center.x, center.y - 2 * r, center.z + r);
                pList2.Add(point);
                point = new C_Point3D(center.x, center.y + 2 * r, center.z + r);
                pList2.Add(point);
            }
            else if (fix == "y")
            {
                C_Point3D point = new C_Point3D(center.x - 2 * r, center.y, center.z - r);
                pList2.Add(point);
                point = new C_Point3D(center.x + 2 * r, center.y, center.z - r);
                pList2.Add(point);
                point = new C_Point3D(center.x - 2 * r, center.y, center.z + r);
                pList2.Add(point);
                point = new C_Point3D(center.x + 2 * r, center.y, center.z + r);
                pList2.Add(point);
            }
            else if (fix == "zx")
            {
                C_Point3D point = new C_Point3D(center.x - 2 * r, center.y - r, center.z);
                pList2.Add(point);
                point = new C_Point3D(center.x + 2 * r, center.y - r, center.z);
                pList2.Add(point);
                point = new C_Point3D(center.x - 2 * r, center.y + r, center.z);
                pList2.Add(point);
                point = new C_Point3D(center.x + 2 * r, center.y + r, center.z);
                pList2.Add(point);
            }
            else if (fix == "z")
            {
                C_Point3D point = new C_Point3D(center.x - r, center.y - 2 * r, center.z);
                pList2.Add(point);
                point = new C_Point3D(center.x + r, center.y - 2 * r, center.z);
                pList2.Add(point);
                point = new C_Point3D(center.x - r, center.y + 2 * r, center.z);
                pList2.Add(point);
                point = new C_Point3D(center.x + r, center.y + 2 * r, center.z);
                pList2.Add(point);
            }
            return pList2;
        }



        public static void 计算抓取面坐标和欧拉角等(C_Node pNode,C_Camera_Const camera1, C_Planet_Catch pPlanet)
        {
            pPlanet.arm_pCenter = pPlanet.center;// Tools.摄像头坐标转到机械臂坐标(camera1, pPlanet.center);
            pPlanet.arm_faxiangliang = pPlanet.z_faxiangliang;// Tools.摄像头坐标转到机械臂坐标(camera1, pPlanet.z_faxiangliang.scale(1000.0))-(Tools.摄像头坐标转到机械臂坐标(camera1, new C_Point3D(0f, 0f, 0f)));
            Vector3D vector3D = new Vector3D(pPlanet.arm_faxiangliang.x, pPlanet.arm_faxiangliang.y, pPlanet.arm_faxiangliang.z);
            pPlanet.rot_z = Angle.FromRadians(Math.Atan2(pPlanet.arm_faxiangliang.y, pPlanet.arm_faxiangliang.x));
            pPlanet.rot_y = Angle.FromRadians(Math.Acos((double)(0f - pPlanet.arm_faxiangliang.z) / vector3D.Length));
            
            pPlanet.arm_pCatchCenter = pPlanet.arm_pCenter;// new C_Point3D(x, y, z);
            Main.WriteLine(pNode,"arm_pCatchCenter=" + pPlanet.arm_pCatchCenter.ToString());

            //pPlanet.arm_pCatch_Before = pPlanet.arm_pCenter;// new C_Point3D(x2, y2, z2);
        }


        public static float angle_process(double angle)
        {
            if (angle > Math.PI) angle = angle - Math.PI * 2;
            if (angle < -Math.PI) angle = angle + Math.PI * 2;
            angle = Math.Round(angle, 5);
            return (float)angle;
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


        public static void init_robot(C_Robot pRobot, string file)
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

            pRobot.init_robot(oz, dz, c2d, bc, cc2, tools_len);

            float x = float.Parse(ini.ReadString("bottom0", "x", "0"));
            float y = float.Parse(ini.ReadString("bottom0", "y", "0"));
            float z = float.Parse(ini.ReadString("bottom0", "z", "0"));

            float r_x = float.Parse(ini.ReadString("rotate0", "x", "0"));
            float r_y = float.Parse(ini.ReadString("rotate0", "y", "0"));
            float r_z = float.Parse(ini.ReadString("rotate0", "z", "1"));

            pRobot.rotate_center[0] = new Vector3(x, y, z);//轴1的中心
            pRobot.obj_rotate[0] = new Vector3(r_x, r_y, r_z);//, 0);//L1旋转的向量


            x = float.Parse(ini.ReadString("bottom1", "x", "0"));
            y = float.Parse(ini.ReadString("bottom1", "y", "0"));
            z = float.Parse(ini.ReadString("bottom1", "z", "0"));

            r_x = float.Parse(ini.ReadString("rotate1", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate1", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate1", "z", "1"));

            pRobot.rotate_center[1] = new Vector3(x, y, z);//轴2的中心
            pRobot.obj_rotate[1] = new Vector3(r_x, r_y, r_z);//L2旋转的向量


            x = float.Parse(ini.ReadString("bottom2", "x", "0"));
            y = float.Parse(ini.ReadString("bottom2", "y", "0"));
            z = float.Parse(ini.ReadString("bottom2", "z", "0"));

            r_x = float.Parse(ini.ReadString("rotate2", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate2", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate2", "z", "1"));
            pRobot.rotate_center[2] = new Vector3(x, y, z);//轴3的中心
            pRobot.obj_rotate[2] = new Vector3(r_x, r_y, r_z);//L3旋转的向量




            x = float.Parse(ini.ReadString("bottom3", "x", "0"));
            y = float.Parse(ini.ReadString("bottom3", "y", "0"));
            z = float.Parse(ini.ReadString("bottom3", "z", "0"));

            r_x = float.Parse(ini.ReadString("rotate3", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate3", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate3", "z", "1"));
            pRobot.rotate_center[3] = new Vector3(x, y, z);//轴4的中心
            pRobot.obj_rotate[3] = new Vector3(r_x, r_y, r_z);//L4旋转的向量




            x = float.Parse(ini.ReadString("bottom4", "x", "0"));
            y = float.Parse(ini.ReadString("bottom4", "y", "0"));
            z = float.Parse(ini.ReadString("bottom4", "z", "0"));

            r_x = float.Parse(ini.ReadString("rotate4", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate4", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate4", "z", "1"));
            pRobot.rotate_center[4] = new Vector3(x, y, z);//轴5的中心
            pRobot.obj_rotate[4] = new Vector3(r_x, r_y, r_z);//L5旋转的向量


            x = float.Parse(ini.ReadString("bottom5", "x", "0"));
            y = float.Parse(ini.ReadString("bottom5", "y", "0"));
            z = float.Parse(ini.ReadString("bottom5", "z", "0"));

            r_x = float.Parse(ini.ReadString("rotate5", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate5", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate5", "z", "1"));
            pRobot.rotate_center[5] = new Vector3(x, y, z);//轴6的中心
            pRobot.obj_rotate[5] = new Vector3(r_x, r_y, r_z);//L6旋转的向量


            x = float.Parse(ini.ReadString("bottom6", "x", "0"));
            y = float.Parse(ini.ReadString("bottom6", "y", "0"));
            z = float.Parse(ini.ReadString("bottom6", "z", "0"));

            r_x = float.Parse(ini.ReadString("rotate6", "x", "0"));
            r_y = float.Parse(ini.ReadString("rotate6", "y", "0"));
            r_z = float.Parse(ini.ReadString("rotate6", "z", "1"));
            pRobot.rotate_center[6] = new Vector3(x, y, z);//轴7的中心
            pRobot.obj_rotate[6] = new Vector3(r_x, r_y, r_z);//L7旋转的向量
        }


        

        public static Vector3 get_Vector3_from_string(string strLine)
        {
            var strSplit = strLine.Split(',');

            if (strSplit.Length > 2)
            {
                return new Vector3(float.Parse(strSplit[0]), float.Parse(strSplit[1]), float.Parse(strSplit[2]));
            }
            else
            {
                return new Vector3(0, 0, 0);
            }
        }





        /// <summary>
        /// K是第七轴的旋转轴
        /// </summary>
        /// <param name="pRobot"></param>
        /// <param name="angle_rad"></param>
        /// <param name="d_AB"></param>
        /// <returns></returns>
        public static (C_Point3D A, C_Point3D B, C_Point3D C2, C_Point3D C, C_Point3D D, C_Point3D K)
            计算ABC2CD_角度(C_Robot pRobot, float[] angles, float d_AB)
        {
            float[] angle_rad = new float[6];
            for (var i = 0; i < 6; i++)
            {
                angle_rad[i] = angles[i] / 180 * (float)Math.PI;
            }


            pRobot.init_fanake(d_AB);

            for (var i = 0; i < 6; i++)
            {
                pRobot.Rotate(i, angle_rad[i]);
            }

            var v = pRobot.rotate_center[5];
            C_Point3D A = new C_Point3D(v.X, v.Y, v.Z);
            v = pRobot.rotate_center[4];
            C_Point3D B = new C_Point3D(v.X, v.Y, v.Z);
            v = pRobot.rotate_center[3];
            C_Point3D C = new C_Point3D(v.X, v.Y, v.Z);
            v = pRobot.rotate_center[2];
            C_Point3D C2 = new C_Point3D(v.X, v.Y, v.Z);
            v = pRobot.rotate_center[1];
            C_Point3D D = new C_Point3D(v.X, v.Y, v.Z);

            return (A, B, C2, C, D, new C_Point3D(pRobot.rotate_center[6]));
        }

        /// <summary>
        /// K是第七轴的旋转轴
        /// </summary>
        /// <param name="pRobot"></param>
        /// <param name="angle_rad"></param>
        /// <param name="d_AB"></param>
        /// <returns></returns>
        public static (C_Point3D A,C_Point3D B, C_Point3D C2,C_Point3D C,C_Point3D D,C_Point3D K)
            计算ABC2CD_弧度(C_Robot pRobot,float[] angle_rad, float d_AB)
        {

            pRobot.init_fanake(d_AB);

            for (var i = 0; i < 6; i++)
            {
                pRobot.Rotate(i, angle_rad[i]);
            }

            var v = pRobot.rotate_center[5];
            C_Point3D A = new C_Point3D(v.X, v.Y, v.Z);
            v = pRobot.rotate_center[4];
            C_Point3D B = new C_Point3D(v.X, v.Y, v.Z);
            v = pRobot.rotate_center[3];
            C_Point3D C = new C_Point3D(v.X, v.Y, v.Z);
            v = pRobot.rotate_center[2];
            C_Point3D C2 = new C_Point3D(v.X, v.Y, v.Z);
            v = pRobot.rotate_center[1];
            C_Point3D D = new C_Point3D(v.X, v.Y, v.Z);

            return (A,B,C2,C,D,new C_Point3D(pRobot.rotate_center[6]));
        }


        public static Vector3 绕轴旋转(Vector3 v1,Vector3 axis,float angle)
        {

            // 创建旋转矩阵
            Matrix4x4 rotationMatrix = Matrix4x4.CreateFromAxisAngle(axis, angle);
            // 应用旋转
            return Vector3.Transform(v1, rotationMatrix);
        }

        public static (bool bError, Vector3[] bottom_L, Vector3[] rotate_L) 验算ABC(
            C_Robot pRobot,
            C_Node pNode,
            float[] angle_rad, 
            C_Point3D A, C_Point3D B, C_Point3D C2, C_Point3D C, C_Point3D D, C_Point3D K)
        {

            float distance = (float)(A.distance(B));
            (C_Point3D A_, C_Point3D B_, C_Point3D C2_, C_Point3D C_, C_Point3D D_,C_Point3D K_) = 计算ABC2CD_弧度(pRobot,angle_rad,distance);


            bool bError = true;
            if (D.distance(D_)<1)
            {
                if (C2.distance(C2_) < 1)
                {
                    if (C.distance(C_) < 1)
                    {
                        if (B.distance(B_) < 1)
                        {
                            if (A.distance(A_) < 1)
                            {
                                if (K == null)
                                {
                                    bError = false;
                                }
                                else
                                {
                                    if (K.distance(K_) < 1)
                                    {
                                        bError = false;
                                    }
                                    else
                                    {
                                        Main.WriteLine(pNode, "K_ " + K_.ToString() + " " + K.ToString());//);
                                    }
                                }
                            }
                            else
                            {
                                Main.WriteLine(pNode, "A " + A_.ToString() + " " + A.ToString());//);
                            }
                        }
                        else
                        {
                            Main.WriteLine(pNode, "B " + B_.ToString() + " " + B.ToString());//);
                        }
                    }
                    else
                    {
                        Main.WriteLine(pNode, "C " + C_.ToString() + " " + C.ToString());//);
                    }
                }
                else
                {
                    Main.WriteLine(pNode, "C2 " + C2_.ToString() + " " + C2.ToString());//);
                }
            }
            else
            {
                Main.WriteLine(pNode, "D " + D_.ToString() + " " + D.ToString());//);
            }

            //根据D算C2
            return (bError,pRobot.rotate_center, pRobot.obj_rotate);
        }



        /// <summary>
        /// 用中位数的方法来算
        /// </summary>
        /// <param name="pList_Point"></param>
        /// <returns></returns>
        public static C_Point3D 计算中心点坐标(List<C_Point3D> pList_Point)
        {
            pList_Point.Sort(delegate (C_Point3D a, C_Point3D b)
            {
                return Math.Sign(a.x - b.x);
            });
            float new_x = (pList_Point[0].x + pList_Point[pList_Point.Count - 1].x) / 2;

            pList_Point.Sort(delegate (C_Point3D a, C_Point3D b)
            {
                return Math.Sign(a.y - b.y);
            });
            float new_y = (pList_Point[0].y + pList_Point[pList_Point.Count - 1].y) / 2;

            pList_Point.Sort(delegate (C_Point3D a, C_Point3D b)
            {
                return Math.Sign(a.z - b.z);
            });
            float new_z = (pList_Point[0].z + pList_Point[pList_Point.Count - 1].z) / 2;

            return new C_Point3D(new_x, new_y, new_z);
        }


        ///// <summary>
        ///// 用中位数的方法来算
        ///// </summary>
        ///// <param name="pList_Point"></param>
        ///// <returns></returns>
        //public static C_Point3D 计算中心点坐标(List<C_Point3D> pList_Point)
        //{
        //    pList_Point.Sort(delegate (C_Point3D a, C_Point3D b)
        //    {
        //        return Math.Sign(a.x - b.x);
        //    });
        //    float new_x = pList_Point[pList_Point.Count / 2].x;

        //    pList_Point.Sort(delegate (C_Point3D a, C_Point3D b)
        //    {
        //        return Math.Sign(a.y - b.y);
        //    });
        //    float new_y = pList_Point[pList_Point.Count / 2].y;

        //    pList_Point.Sort(delegate (C_Point3D a, C_Point3D b)
        //    {
        //        return Math.Sign(a.z - b.z);
        //    });
        //    float new_z = pList_Point[pList_Point.Count / 2].z;

        //    return new C_Point3D(new_x, new_y, new_z);
        //}



        //public static void show_tip(string title,string msg)
        //{
        //    new ToastContentBuilder()
        //        .AddArgument("action", "viewConversation")
        //        .AddArgument("conversationId", 9813)
        //        .AddText(title)
        //        .AddText(msg)
        //        .Show();
        //}

        public static void AddAddress(string address, string domain, string user)
        {
            string argsDll = string.Format(@"http delete urlacl url={0}", address);
            string args = string.Format(@"http add urlacl url={0} user={1}", address, user);
            //string args = string.Format(@"http add urlacl url={0} user={1}\{2}", address, domain, user);
            ProcessStartInfo psi = new ProcessStartInfo("netsh", argsDll);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            
            {
                Process.Start(psi)?.WaitForExit();//删除urlacl
            }
            psi = new ProcessStartInfo("netsh", args);
            psi.Verb = "runas";
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            Process.Start(psi)?.WaitForExit();//添加urlacl
        }



        public static Bitmap CopyBmp(Bitmap bmp_source)
        {
            if (bmp_source == null)
            {
                return null;
            }
            lock (bmp_source)
            {

                int width = bmp_source.Width;
                int height = bmp_source.Height;

                Rectangle rect = new Rectangle(0, 0, width, height);

                Bitmap bmp = new Bitmap(width, height, bmp_source.PixelFormat);

                PixelFormat format = bmp_source.PixelFormat;

                lock (bmp_source)
                {
                    BitmapData bmpDataSrc = bmp_source.LockBits(rect, ImageLockMode.ReadOnly, format);
                    // Get the address of the first line.获取首行地址
                    IntPtr ptrSrc = bmpDataSrc.Scan0;

                    BitmapData bmpDataDest = bmp.LockBits(rect, ImageLockMode.WriteOnly, format);
                    IntPtr ptrDest = bmpDataDest.Scan0;


                    // Declare an array to hold the bytes of the bitmap.定义数组保存位图
                    int bytes = Math.Abs(bmpDataSrc.Stride) * height;
                    byte[] rgbValues = new byte[bytes];

                    // Copy the RGB values into the array.复制RGB值到数组
                    Marshal.Copy(ptrSrc, rgbValues, 0, bytes);
                    //复制到新图
                    Marshal.Copy(rgbValues, 0, ptrDest, bytes);

                    // Unlock the bits.解锁
                    bmp_source.UnlockBits(bmpDataSrc);
                    bmp.UnlockBits(bmpDataDest);
                }


                return bmp;
            }
        }


        public void CPP_Callback(byte[] RData, int status, string message)
        {
            Main.WriteLine("当前状态：" + status);
            switch (status)
            {
                case 0:
                    string strData = Encoding.UTF8.GetString(RData); // TCPServer.ToHexString(RData);
                    Main.WriteLine("收到消息：" + strData);

                    //C_Send.发送给CPP("hello,你好，中国");

                    if (strData == "123")
                    {
                        Main.WriteLine("123");
                        C_Send.发送给CPP("234！");
                    }
                    break;
                case 1: //连接成功
                    //Main.WriteLine(
                    //    string.Format("【IP:{0} 端口:{1}】", space.vars.ip_plc, space.vars.port_plc) + "连接成功...");
                    break;
                default:
                    //断开连接
                    //Main.WriteLine(
                    //    string.Format("【IP:{0} 端口:{1}】", space.vars.ip_plc, space.vars.port_plc) + "断开连接...");

                    break;
            }
        }




        public void 下一步()
        {
            //Count_Arm_Time.clear();
            //if (space.vars.b_send_to_robot_arm)
            {
                //启动的时候，移动到放置点
                //C_Result pResult = C_Send.计算放置点坐标(null, null);
                //space.vars.array_command.Add(
                //    new C_Robot_Command(C_Robot_Command.Command_Type.Robot, 2, pResult.A, pResult.strLine, ""));
                //Thread.Sleep(1500);
            }

            //C_Send.First_Count = 0;

            //space.vars.Position_History.Clear(); //清空历史数据

            space.vars.count_put = -1;
            FileStream fs_debug = new FileStream("D:\\log_debug.txt", FileMode.Create, FileAccess.Write);
            TextWriter w_debug = new StreamWriter(fs_debug, Encoding.UTF8);

            var line = Main.TimeNow();
            w_debug.WriteLine(line);

            w_debug.Close();
            fs_debug.Close();

            space.vars.pTimeStart = new TimeSpan(DateTime.Now.Ticks);
            //C_Main.actives_Nodes.Clear();
            //m1.照相机模块.Run(true);
        }

        public static double get_double_from_obj(C_Node pNode, object? obj)
        {
            double ret = 0;
            if (obj == null)
            {
                Main.WriteLine(pNode, "错误 get_double_from_obj = null");
                return 0;
            }
            if (obj.GetType().Name == "Int32")
            {
                ret = (int)obj;
            }
            else if (obj.GetType().Name == "Double")
            {
                ret = (double)obj;
            }
            else if (obj.GetType().Name == "Byte")
            {
                ret = (byte)obj+0;
            }
            else if (obj.GetType().Name == "String")
            {
                if ((string)obj == "")
                {
                    ret = 0;
                }
                else
                {
                    try
                    {
                        ret = double.Parse((string)obj);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show(pNode.Name+ "error type=" +obj.GetType().Name );
            }
            return ret;
        }

        public static string hex(object obj,int min=2)
        {
            string type = obj.GetType().Name.ToLower();

            string line = "";
            switch (type)
            {
                case "int32":
                    line = ((int)obj).ToString("X");
                    break;
                case "double":
                    double m1 = (double)obj;
                    line = ((int)m1).ToString("X");
                    break;
                case "string":
                    string m2 = (string)obj;
                    line = int.Parse(m2).ToString("X");
                    break;
            }
            while (line.Length < min)
            {
                line= "0"+line ;
            }
            return line;
        }


        public static string switch_if(bool check, string strTrue, string strFalse)
        {
            if (check)
            {
                return strTrue;
            }
            else
            {
                return strFalse;
            }
        }

        public static DenseMatrix Read_Camera(string file_camera)
        {
            DenseMatrix m = new DenseMatrix(3, 3);

            string content=File.ReadAllText(file_camera);

            string[] strSplit=content.Split("\r\n");

            double[,] d = new double[3, 3];
            for (var i = 0; i < Math.Min(3, strSplit.Length); i++)
            {
                string[] strSplit2= strSplit[i].Split(" ");
                for(var j = 0; j < 3; j++)
                {
                    d[i,j]= double.Parse(strSplit2[j]);
                }
            }

            m = DenseMatrix.OfArray(d);//坐标变换矩阵

            return m;
        }


        public static byte[] AutoBalance(
            byte[] buffer,int width,int height)
        {

            double max_r = 0;
            double max_g = 0;
            double max_b = 0;
            double min_r = 255;
            double min_g = 255;
            double min_b = 255;

            for (var i = 0; i < buffer.Length; i += 4)
            {
                if (buffer[i] > max_r)
                {
                    max_r = buffer[i];
                }
                if (buffer[i+1] > max_g)
                {
                    max_g = buffer[i+1];
                }
                if (buffer[i+2] > max_b)
                {
                    max_b = buffer[i+2];
                }

                if (buffer[i] < min_r)
                {
                    min_r = buffer[i];
                }
                if (buffer[i + 1] < min_g)
                {
                    min_g = buffer[i + 1];
                }
                if (buffer[i + 2] < min_b)
                {
                    min_b = buffer[i + 2];
                }
            }

            return ApplyWhiteBalance(buffer, max_r, max_g, max_b, min_r, min_g, min_b);
        }

        public static byte[] ApplyWhiteBalance(byte[] buffer,
            double max_r, double max_g, double max_b,
            double min_r, double min_g, double min_b)
        {
            byte[] buffer2=new byte[buffer.Length];

            int count = 0;
            for (var i = 0; i < buffer.Length; i += 4)
            {
                count += 1;

                double r= buffer[i];
                double g = buffer[i + 1];
                double b = buffer[i + 2];

                double r_new = (r - min_r) / (max_r - min_r) * 255;
                double g_new = (g - min_g) / (max_g - min_g) * 255;
                double b_new = (b - min_b) / (max_b - min_b) * 255;

                buffer2[i] = (byte)Main.Trim_To_Int(r_new,0,255);
                buffer2[i+1] = (byte)Main.Trim_To_Int(g_new, 0, 255);
                buffer2[i+2] = (byte)Main.Trim_To_Int(b_new, 0, 255); 
                buffer2[i+3] = buffer[i+3];
            }

            return buffer2;
        }

        public static int Trim_To_Int(double value,int min,int max)
        {
            int a = (int)Math.Round(value);
            if (a < min) a = min;
            if (a > max) a = max;
            return a;
        }

        public static Bitmap convert_byte_to_bmp(byte[] imageData,int w,int h)
        {
            //int w = 100;
            //int h = 200;
            int ch = 4; //number of channels (ie. assuming 32 bit ARGB in this case)

            //byte[] imageData = new byte[w * h * ch]; //you image data here
            Bitmap bitmap = new Bitmap(w, h, PixelFormat.Format32bppArgb);// .Format24bppRgb);
            BitmapData bmData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            IntPtr pNative = bmData.Scan0;
            Marshal.Copy(imageData, 0, pNative, w * h * ch);
            bitmap.UnlockBits(bmData);
            return bitmap;
        }

        public static string File_Log="";

        public static List<string> Logs=new List<string>();
        public static Frm_Debug pDebug=new Frm_Debug();

        public static void WriteLine(C_Node? pNode, string strLine,bool bError=false)
        {
            string time = DateTime.Now.ToString("mm:ss.fff");
            string line = time + " " + pNode?.Name+ "="+pNode?.alias + " " + strLine;

            Task.Run(() =>
            {
                if (pNode != null && pNode.space.vars_ui.ContainsKey("#ui_console"))
                {
                    S_Cmd_View pCmd = (S_Cmd_View)pNode.space.vars_ui["#ui_console"];

                    if (pNode?.pTrain?.mode == "不记录")
                    {
                    }
                    else
                    {
                        pCmd?.ui_item?.WriteOutput(line + "\r\n", System.Drawing.Color.White);
                        pCmd?.ui_item?.scroll_last();
                    }
                }

                if (pNode?.pTrain?.mode == "不记录")
                {
                }
                else
                {
                    Console.WriteLine(line);
                    if (pNode!=null) pNode.space.Log(line);
                }
                if (bError)
                {
                    if (Main.pDebug != null)
                    {
                        pDebug.WriteError(pNode.Name, strLine);
                    }
                }
            });
        }

        public static void WriteLine(string strLine)
        {
            string time = DateTime.Now.ToString("mm:ss.fff");
            string line = time + " " + strLine;
            Console.WriteLine(line);
            //C_Space.Log(line);
        }


        public static bool 计算是否碰撞到障碍物(
            C_Node pNode,
            C_Matrix? camera1,
            double r, List<C_Point3D> pListArray_Objs, C_Planet_Catch pPlanet)
        {
            //

            if (pListArray_Objs == null)
            {
                Main.WriteLine(pNode, "没有设置障碍物!");
                return false;
            }
            C_Point3D A = pPlanet.arm_a;
            C_Point3D B = pPlanet.arm_b;

            //判断AB为轴，r为半径的圆柱是否和障碍物的点会碰撞
            for (var i = 0; i < pListArray_Objs.Count; i++)
            {
                C_Point3D position_camera = pListArray_Objs[i];
                C_Point3D? C = Main_Robot.摄像头坐标转到机械臂坐标(camera1, position_camera);

                C_Point3D AB = B-(A);
                C_Point3D AC = C-(A);
                C_Point3D BC = C-(AB);
                Vector3D v_AB = new Vector3D(AB.x, AB.y, AB.z);
                Vector3D v_AC = new Vector3D(AC.x, AC.y, AC.z);
                Vector3D v_BC = new Vector3D(BC.x, BC.y, BC.z);
                double alpha = v_AB.Normalize().DotProduct(v_AC);
                double beta = -v_AB.Normalize().DotProduct(v_BC);
                if (alpha > 0 && beta > 0)
                {
                    Vector3D v_AD = v_AB.Normalize().ScaleBy(alpha);
                    Vector3D v_DC = v_AC - v_AD;
                    if (v_DC.Length < r)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public static double[] 计算变换矩阵所有参数(string strFile)
        {
            List<C_Point3D> arrayList = new List<C_Point3D>();
            List<C_Point3D> arrayList2 = new List<C_Point3D>();
            StreamReader? streamReader = new StreamReader(strFile);
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            int num7 = 0;
            string? line;
            while ((line = streamReader?.ReadLine()) != null)
            {
                if (line.StartsWith("//"))
                {
                    continue;
                }

                line = line.Replace("，", ",");
                line = line.Replace(" ", ",");
                line = line.Replace(",,", ",");
                string[] array = line.Split(',');
                if (array.Length > 5)
                {
                    try
                    {
                        double num8 = double.Parse(array[0]);
                        double num9 = double.Parse(array[1]);
                        double num10 = double.Parse(array[2]);
                        double num11 = double.Parse(array[3]);
                        double num12 = double.Parse(array[4]);
                        double num13 = double.Parse(array[5]);
                        num += num8;
                        num2 += num9;
                        num3 += num10;
                        num4 += num11;
                        num5 += num12;
                        num6 += num13;
                        arrayList.Add(new C_Point3D(num8, num9, num10));
                        arrayList2.Add(new C_Point3D(num11, num12, num13));
                        num7++;
                    }
                    catch (Exception ex)
                    {
                        Main.WriteLine(ex.ToString());
                        Main.WriteLine("error:" + line);
                    }
                }
            }

            streamReader?.Close();

            C_Point3D point1 = new C_Point3D(num / (double)num7, num2 / (double)num7, num3 / (double)num7);
            C_Point3D point2 = new C_Point3D(num4 / (double)num7, num5 / (double)num7, num6 / (double)num7);
            double[,] array2 = new double[arrayList.Count, 3];
            double[,] array3 = new double[arrayList.Count, 3];
            for (int i = 0; i < arrayList.Count; i++)
            {
                C_Point3D point3 = arrayList[i];
                C_Point3D point4 = arrayList2[i];
                array2[i, 0] = point3.x - point1.x;
                array2[i, 1] = point3.y - point1.y;
                array2[i, 2] = point3.z - point1.z;
                array3[i, 0] = point4.x - point2.x;
                array3[i, 1] = point4.y - point2.y;
                array3[i, 2] = point4.z - point2.z;
            }

            DenseMatrix denseMatrix = DenseMatrix.OfArray(array2);
            DenseMatrix other = DenseMatrix.OfArray(array3);
            Matrix<double> matrix = denseMatrix.Transpose().Multiply(other);
            Svd<double> svd = matrix.Svd();
            Matrix<double> u = svd.U;
            Matrix<double> matrix2 = svd.VT.Transpose();
            double num14 = Math.Sign(matrix2.Multiply(u.Transpose()).Determinant());
            double[,] array4 = new double[3, 3]
            {
                { 1.0, 0.0, 0.0 },
                { 0.0, 1.0, 0.0 },
                { 0.0, 0.0, num14 }
            };
            Matrix<double> other2 = DenseMatrix.OfArray(array4);
            Matrix<double> matrix3 = matrix2.Multiply(other2).Multiply(u.Transpose()).Transpose();
            C_Point3D point5 = point2-(point1);
            return new double[18]
            {
                matrix3[0, 0],
                matrix3[0, 1],
                matrix3[0, 2],
                matrix3[1, 0],
                matrix3[1, 1],
                matrix3[1, 2],
                matrix3[2, 0],
                matrix3[2, 1],
                matrix3[2, 2],
                point5.x,
                point5.y,
                point5.z,
                point1.x,
                point1.y,
                point1.z,
                point2.x,
                point2.y,
                point2.z
            };
        }

        public static (
            C_Point3D p1_center,
            C_Point3D p2_center, 
            DenseMatrix M_Rotate) 
            读取标定文件(string strFile)
        {
            double[] m = Main.计算变换矩阵所有参数(strFile);
            //X^T=(a1 a2 a3 a4 a5 a6 a7 a8 a9 m1 m2 m3)
            double a1 = m[0];
            double a2 = m[1];
            double a3 = m[2];
            double a4 = m[3];
            double a5 = m[4];
            double a6 = m[5];
            double a7 = m[6];
            double a8 = m[7];
            double a9 = m[8];

            double _m1 = m[9];
            double m2 = m[10];
            double m3 = m[11];


            C_Point3D p1_center = new C_Point3D(m[12], m[13], m[14]);
            C_Point3D p2_center = new C_Point3D(m[15], m[16], m[17]);

            Main.WriteLine("(a1 a2 a3 a4 a5 a6 a7 a8 a9 m1 m2 m3)="
                + a1 + "," + a2 + "," + a3 + ","
                + a4 + "," + a5 + "," + a6 + ","
                + a7 + "," + a8 + "," + a9 + ","
                + _m1 + "," + m2 + "," + m3);


            double[,] d = new double[3, 3];
            d[0, 0] = a1;
            d[0, 1] = a2;
            d[0, 2] = a3;
            d[1, 0] = a4;
            d[1, 1] = a5;
            d[1, 2] = a6;
            d[2, 0] = a7;
            d[2, 1] = a8;
            d[2, 2] = a9;


            DenseMatrix M_Rotate = DenseMatrix.OfArray(d);//坐标变换矩阵
            return (p1_center, p2_center, M_Rotate);
        }

        public static void 计算标定数据误差(C_Node pNode,string strFile, DenseMatrix m1, C_Point3D p1, C_Point3D p2)
        {

            //验证采集数据

            StreamReader? file = new StreamReader(strFile);
            string? line;
            while ((line = file.ReadLine()) != null)
            {

                if (line.StartsWith("//"))
                {
                    continue;
                }
                line = line.Replace("，", ",");
                line = line.Replace(" ", ","); ;
                line = line.Replace(",,", ",");
                string[] strSplit = line.Split(',');

                if (strSplit.Length > 5)
                {
                    try
                    {
                        double x = double.Parse(strSplit[0]) - p1.x;
                        double y = double.Parse(strSplit[1]) - p1.y;
                        double z = double.Parse(strSplit[2]) - p1.z;
                        double x1 = double.Parse(strSplit[3]);
                        double y1 = double.Parse(strSplit[4]);
                        double z1 = double.Parse(strSplit[5]);


                        double[,] d2 = new double[3, 1];
                        d2[0, 0] = x;
                        d2[1, 0] = y;
                        d2[2, 0] = z;

                        DenseMatrix R = m1;// camera1.M_Rotate;// Tools.M_Rotate;
                        DenseMatrix xyz = DenseMatrix.OfArray(d2);
                        Matrix<double> xyz1 = xyz.Transpose().Multiply(R);// M.Multiply(xyz);

                        double dx = Math.Round(xyz1[0, 0] + p2.x - x1, 2);
                        double dy = Math.Round(xyz1[0, 1] + p2.y - y1, 2);
                        double dz = Math.Round(xyz1[0, 2] + p2.z - z1, 2);


                        if (Math.Abs(dx) > 10 || Math.Abs(dy) > 10 || Math.Abs(dz) > 10)
                        {
                            Main.speak_async("注意安全，注意安全！");
                            Main.WriteLine(pNode,"标定误差太大！" + dx + "," + dy + "," + dz + "=>" + line);
                        }
                        else
                        {

                            Main.WriteLine(pNode, "d=\t"
                                + format(dx) + "\t" + format(dy) + "\t" + format(dz)
                                + "\t\t" + x1 + "\t" + y1 + "\t" + z1);
                        }

                    }
                    catch (Exception e)
                    {
                        Main.WriteLine(pNode, e.ToString());
                    }

                }
            }
            file.Close();
        }


        /// <summary>
        /// 用中位数的方法来算
        /// </summary>
        /// <param name="pList_Point"></param>
        /// <returns></returns>
        public static (C_Point3D center,C_Point3D min,C_Point3D max) 计算中位数中心点坐标(List<C_Point3D> pList_Point)
        {
            List<C_Point3D> list2=new List<C_Point3D>();
            for(var i = 0; i < pList_Point.Count; i++)
            {
                if (pList_Point[i] != null)
                {
                    list2.Add(pList_Point[i]);
                }
            }
            list2.Sort(delegate (C_Point3D a, C_Point3D b)
            {
                return Math.Sign(a.x - b.x);
            });
            float new_x = list2[list2.Count / 2].x;
            float x_min = list2[0].x;
            float x_max = list2[list2.Count - 1].x;

            list2.Sort(delegate (C_Point3D a, C_Point3D b)
            {
                return Math.Sign(a.y - b.y);
            });
            float new_y = list2[list2.Count / 2].y;
            float y_min = list2[0].y;
            float y_max = list2[list2.Count - 1].y;

            list2.Sort(delegate (C_Point3D a, C_Point3D b)
            {
                return Math.Sign(a.z - b.z);
            });
            float new_z = list2[list2.Count / 2].z;
            float z_min = list2[0].z;
            float z_max = list2[list2.Count - 1].z;



            return (new C_Point3D(new_x, new_y, new_z),
                new C_Point3D(x_min,y_min,z_min), 
                new C_Point3D(x_max, y_max, z_max));
        }



        public static double? cloud_get_mid(C_Node pNode, List<C_Point3D> list, string xyz, int index)
        {
            List<double> list_height = new List<double>();
            switch (xyz)
            {
                case "x":
                case "0":
                    for (var i = 0; i < list?.Count; i++)
                    {
                        C_Point3D pPoint2 = list[i];
                        if (pPoint2 != null) list_height.Add(pPoint2.x);
                    }
                    break;
                case "y":
                case "1":
                    for (var i = 0; i < list?.Count; i++)
                    {
                        C_Point3D pPoint2 = list[i];
                        if (pPoint2!=null) list_height.Add(pPoint2.y);
                    }
                    break;
                case "z":
                case "2":
                    for (var i = 0; i < list?.Count; i++)
                    {
                        C_Point3D pPoint2 = list[i];
                        if (pPoint2 != null) list_height.Add(pPoint2.z);
                    }
                    break;
            }
            list_height.Sort();


            double? value = null;
            if (list_height?.Count > 0)
            {
                if (index < list_height?.Count && index >= 0)
                {
                    value = list_height[index];
                }
                else if (index == -1)
                {
                    value = list_height[list_height.Count - 1];
                }
            }
            return value;
        }


        public static string format(double x)
        {
            if (x >= 0)
            {
                return "+" + x;
            }
            else
            {
                return "" + x;
            }
        }

        public static (float max, int idx) get_max(List<C_Planet_Catch> list_draw,string stype="z")
        {
            float maxval = -10000;
            int maxidx = -1;

            if (stype=="z")
            {
                for (int i = 0; i < list_draw.Count; ++i)
                {
                    if (maxval < list_draw[i].arm_a.z)
                    {
                        maxval = list_draw[i].arm_a.z;
                        maxidx = i;
                    }
                }
            }
            else if (stype=="x")
            {
                for (int i = 0; i < list_draw.Count; ++i)
                {
                    if (maxval < list_draw[i].arm_a.x)
                    {
                        maxval = list_draw[i].arm_a.x;
                        maxidx = i;
                    }
                }
            }
            return (maxval, maxidx);
        }

        public static (float minval, int minidx) get_min(List<C_Planet_Catch> list_draw, string stype = "z")
        {
            float minval = 10000;
            int minidx = -1;

            if (stype == "z")
            {
                for (int i = 0; i < list_draw.Count; ++i)
                {
                    if (minval > list_draw[i].arm_a.z)
                    {
                        minval = list_draw[i].arm_a.z;
                        minidx = i;
                    }
                }
            }
            else if (stype == "x")
            {

                for (int i = 0; i < list_draw.Count; ++i)
                {
                    if (minval > list_draw[i].arm_a.x)
                    {
                        minval = list_draw[i].arm_a.x;
                        minidx = i;
                    }
                }
            }

            return (minval, minidx);
            
        }



        public static byte[] hext_to_byte(string hex)
        {
            byte[] b = new byte[hex.Length / 2];
            for (var i = 0; i < hex.Length; i += 2)
            {
                string a = hex.Substring(i, 2);
                b[i / 2] = Convert.ToByte(a, 16);
            }
            return b;
        }


        public static byte[] ConvertHexStringToByteArray(string[] strSplit)
        {
            byte[] data = new byte[strSplit.Length];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = strSplit[index];
                try
                {
                    data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("hex error " + byteValue+","+ ex.ToString());
                }
            }

            return data;
        }



        public static (List<C_Point3D> save1, List<C_Point3D> save2) cloud_filter2(
            List<C_Point3D>? read1, List<C_Point3D>? read2,string xyz, float f_min, float f_max)
        {
            List<C_Point3D> save1 = new List<C_Point3D>();
            List<C_Point3D> save2 = new List<C_Point3D>();
            lock (save1)
            {
                switch (xyz)
                {
                    case "x":
                    case "0":
                        for (var i = 0; i < read1.Count; i++)
                        {
                            C_Point3D p = read1[i];
                            if (f_min <= p.x && p.x <= f_max)
                            {
                                save1.Add(p);

                                C_Point3D p2 = read2[i];
                                save2.Add(p2);
                            }
                        }
                        break;
                    case "y":
                    case "1":
                        for (var i = 0; i < read1.Count; i++)
                        {
                            C_Point3D p = read1[i];
                            if (f_min <= p.y && p.y <= f_max)
                            {
                                save1.Add(p);
                                C_Point3D p2 = read2[i];
                                save2.Add(p2);
                            }
                        }
                        break;
                    case "z":
                    case "2":
                        for (var i = 0; i < read1.Count; i++)
                        {
                            C_Point3D p = read1[i];
                            if (f_min <= p.z && p.z <= f_max)
                            {
                                save1.Add(p);
                                C_Point3D p2 = read2[i];
                                save2.Add(p2);
                            }
                        }
                        break;
                }
            }
            return (save1,save2);
        }



        public static List<C_Point3D> cloud_filter(List<C_Point3D> read1,string xyz,float f_min,float f_max)
        {
            List<C_Point3D> save1 = new List<C_Point3D>();
            lock (save1)
            {
                switch (xyz)
                {
                    case "x":
                    case "0":
                        for (var i = 0; i < read1.Count; i++)
                        {
                            C_Point3D p = read1[i];
                            if (p!=null && f_min <= p.x && p.x <= f_max)
                            {
                                save1.Add(p);
                            }
                        }
                        break;
                    case "y":
                    case "1":
                        for (var i = 0; i < read1.Count; i++)
                        {
                            C_Point3D p = read1[i];
                            if (p != null && f_min <= p.y && p.y <= f_max)
                            {
                                save1.Add(p);
                            }
                        }
                        break;
                    case "z":
                    case "2":
                        for (var i = 0; i < read1.Count; i++)
                        {
                            C_Point3D p = read1[i];
                            if (p != null && f_min <= p.z && p.z <= f_max)
                            {
                                save1.Add(p);
                            }
                        }
                        break;
                }
            }
            return save1;
        }



        ////计算x，y，z最大，最小值
        public static C_Point3D[] 计算最大最小值(List<C_Point3D> pList)
        {
            double x_min = 100000;
            double x_max = -100000;
            double y_min = 100000;
            double y_max = -100000;
            double z_min = 100000;
            double z_max = -100000;

            for (int i = 0; i < pList.Count; i++)
            {
                C_Point3D p = pList[i];
                if (p != null)
                {
                    if (p.x < x_min) x_min = p.x;
                    if (p.y < y_min) y_min = p.y;
                    if (p.z < z_min) z_min = p.z;

                    if (p.x > x_max) x_max = p.x;
                    if (p.y > y_max) y_max = p.y;
                    if (p.z > z_max) z_max = p.z;
                }
            }

            C_Point3D[] arr = new C_Point3D[3];

            arr[0] = new C_Point3D(x_min, y_min, z_min);
            arr[1] = new C_Point3D(x_max, y_max, z_max);

            double x = (arr[0].x + arr[1].x) / 2;
            double y = (arr[0].y + arr[1].y) / 2;
            double z = (arr[0].z + arr[1].z) / 2;
            arr[2] = new C_Point3D(x, y, z);
            return arr;
        }
        public static double 计算最小外接正矩形面积(List<C_Point3D> currentPolygon)
        {
            (C_Point3D x0y0, C_Point3D x1y1) = Tools.计算最大最小值(currentPolygon);

            return Math.Abs(x0y0.x - x1y1.x) * Math.Abs(x0y0.y - x1y1.y);
        }

        public static List<C_Point3D> 旋转点云(double angle, List<C_Point3D> currentPolygon)
        {
            List<C_Point3D> polygon2 = new List<C_Point3D>();

            for (int i = 0; i < currentPolygon.Count; i++)
            {
                C_Point3D a = currentPolygon[i];
                double angle2 = Math.Atan2(a.y, a.x);

                double distance = a.distance(new C_Point3D(0, 0, 0));
                if (distance > 0)
                {
                    double x = distance * Math.Cos(angle2 - angle);
                    double y = distance * Math.Sin(angle2 - angle);
                    C_Point3D b = new C_Point3D(x, y, 0);
                    polygon2.Add(b);
                }
                else
                {
                    polygon2.Add(a);
                }
            }
            return polygon2;
        }

        public static double 计算最小外接矩形(List<C_Point3D> currentPolygon)
        {
            if (currentPolygon.Count < 3)
            {
                Main.WriteLine("Error");
                MessageBox.Show("Error");
            }
            C_Point3D a = currentPolygon[0];
            C_Point3D b = currentPolygon[1];
            List<C_Point3D> currentPolygon2;

            double min_angle = Math.Atan2(b.y - a.y, b.x - a.x);
            currentPolygon2 = 旋转点云(min_angle, currentPolygon);
            double min_area = 计算最小外接正矩形面积(currentPolygon2);

            C_Point3D c, d;
            double angle2, min_area2;
            for (int i = 0; i < currentPolygon.Count - 1; i++)
            {
                c = currentPolygon[i];
                d = currentPolygon[i + 1];
                angle2 = Math.Atan2(d.y - c.y, d.x - c.x);
                currentPolygon2 = 旋转点云(angle2, currentPolygon);
                min_area2 = 计算最小外接正矩形面积(currentPolygon2);
                if (min_area2 < min_area)
                {
                    min_area = min_area2;
                    min_angle = angle2;
                }
            }
            c = currentPolygon[currentPolygon.Count - 1];
            d = currentPolygon[0];
            angle2 = Math.Atan2(d.y - c.y, d.x - c.x);
            currentPolygon2 = 旋转点云(angle2, currentPolygon);
            min_area2 = 计算最小外接正矩形面积(currentPolygon2);
            if (min_area2 < min_area)
            {
                min_area = min_area2;
                min_angle = angle2;
            }

            return min_angle;// *180/Math.PI;
        }


        public static C_Node? get_node_from_parent_vars_steps(C_Space space, string key_step)
        {
            C_Node pItem;
            C_Space space2 = space;

            if (key_step != null && space2.vars_step.ContainsKey(space2.Name + key_step))
            {
                pItem = (C_Node)space2.vars_step[space2.Name+ key_step];
                return pItem;
            }

            if (key_step != null && space2.vars_step.ContainsKey(key_step))
            {
                pItem = (C_Node)space2.vars_step[key_step];
                return pItem;
            }

            while (space2.parent != null)
            {
                space2 = space2.parent;
                if (key_step != null && space2.vars_step.ContainsKey(space2.Name + key_step))
                {
                    pItem = (C_Node)space2.vars_step[space2.Name + key_step];
                    return pItem;
                }
                if (key_step != null && space2.vars_step.ContainsKey(key_step))
                {
                    pItem = (C_Node)space2.vars_step[key_step];
                    return pItem;
                }
            }
            return null;
        }



        public static S_UI? get_ui_from_parent_vars_steps(C_Space space, string key_ui)
        {
            S_UI pItem;
            C_Space space2 = space;

            if (key_ui != null && space2.vars_ui.ContainsKey(key_ui))
            {
                pItem = (S_UI)space2.vars_ui[key_ui];
                return pItem;
            }


            while (space2.parent != null)
            {
                space2 = space2.parent;
                if (key_ui != null && space2.vars_ui.ContainsKey(key_ui))
                {
                    pItem = (S_UI)space2.vars_ui[key_ui];
                    return pItem;
                }
            }
            return null;
        }

        public static void open_explorer_show(string file_new)
        {

            // 打开资源管理器并导航到目录
            //ProcessStartInfo startInfo = new ProcessStartInfo("explorer.exe");
            //startInfo.Arguments = directoryPath;
            //Process.Start(startInfo);

            //// 等待一秒钟，让资源管理器加载目录
            //System.Threading.Thread.Sleep(1000);

            // 选择文件
            Process.Start("Explorer.exe", "/select," + file_new);
        }

        /// <summary>
        /// 查找这个范围内的所有点
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="x1"></param>
        /// <param name="y0"></param>
        /// <param name="y1"></param>
        /// <param name="z0"></param>
        /// <param name="z1"></param>
        /// <returns></returns>
        public static List<C_Point3D> 查找区域的点(
            List<C_Point3D>? pList,
            double x0, double x1, double y0, double y1, double z0, double z1)
        {
            List<C_Point3D> pList2 = new List<C_Point3D>();
            for (var i = 0; i < pList?.Count; i++)
            {
                C_Point3D p = pList[i];
                if (p.x >= x0 && p.x < x1)
                {
                    if (p.y >= y0 && p.y < y1)
                    {
                        if (p.z >= z0 && p.z < z1)
                        {
                            pList2.Add(p);
                        }
                    }
                }
            }
            return pList2;
        }



        /// <summary>
        /// 有两个
        /// </summary>
        /// <returns></returns>
        public static C_Point3D? 计算D坐标(C_Robot? pRobot, C_Point3D B)
        {
            if (pRobot == null)
            {
                return null;
            }
            //根据B算D
            var a = Math.Atan2(B.y, B.x);
            var x = pRobot.arm_oz * Math.Cos(a);
            var y = pRobot.arm_oz * Math.Sin(a);
            var z = pRobot.arm_dz;

            return new C_Point3D(x, y, z);
        }


        public static double my_acos(double angle)
        {
            if (angle < -1)
            {
                return Math.PI;
            }
            if (angle > 1)
            {
                return 0;
            }
            return Math.Acos(angle);
        }

        public static List<C_Point3D> GetPloygon_From_JObject(JObject pItem)
        {
            JArray pJArray;
            if (pItem.SelectToken("contour")!=null)
            {
                pJArray = (JArray)pItem.SelectToken("contour");
            }
            else
            {
                pJArray = (JArray)pItem.SelectToken("vertex");
            }

            List<C_Point3D> polygon=new List<C_Point3D>();  
            for (var j = 0; j < pJArray.Count; j++)
            {
                int x1 = (int)pJArray[j][0];
                int y1 = (int)pJArray[j][1];

                polygon.Add(new C_Point3D(x1, y1, 0));
            }

            return polygon;
        }

		public static float 中位数x(List<C_Point3D> cloud1)
        {

            cloud1.Sort(delegate (C_Point3D a, C_Point3D b)
            {
                return Math.Sign(b.x - a.x);
            });

            int mid = cloud1.Count / 2;
            return cloud1[mid].x;
        }


        public static float 中位数y(List<C_Point3D> cloud1)
        {

            cloud1.Sort(delegate (C_Point3D a, C_Point3D b)
            {
                return Math.Sign(b.y- a.y);
            });

            int mid = cloud1.Count / 2;
            return cloud1[mid].y;
        }



        public static float 中位数z(List<C_Point3D> cloud1)
        {

            cloud1.Sort(delegate (C_Point3D a, C_Point3D b)
            {
                return Math.Sign(b.z - a.z);
            });

            int mid = cloud1.Count / 2;
            return cloud1[mid].z;
        }

        /// <summary>
        /// 点云扣除中间一个方形区域，这个区域是抓取的箱子
        /// </summary>
        /// <param name="cloud1"></param>
        /// <param name="y_min"></param>
        /// <param name="y_max"></param>
        /// <param name="z_min"></param>
        /// <param name="z_max"></param>
        /// <returns></returns>
        public static List<C_Point3D> cloud_minus(List<C_Point3D> cloud1, float y_min, float y_max, float z_min, float z_max)
        {
            List<C_Point3D> list = new List<C_Point3D>();
            for(var i=0;i<cloud1.Count;i++)
            {
                C_Point3D p1 = cloud1[i];

                if (p1.y>= y_min && p1.y<= y_max && p1.z>=z_min && p1.z <= z_max)
                {
                }
                else
                {
                    list.Add(p1);
                }
            }
            return list;
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
        public static List<C_Box_Eight> 
            获取动态物体列表(
            C_Space space,C_Node pNode,C_Robot pRobot,
            C_Three_Point n3,
            float k_up = 100, float k_down = 100)
        {
            C_Point3D A = n3.A;
            C_Point3D B = n3.B;
            C_Point3D K = n3.P7;
            C_Point3D AK_normal = (K - A).normalize();
            C_Point3D AB_normal = (B - A).normalize();
            C_Point3D AM_normal = AK_normal.crossProduct(AB_normal).normalize();

            C_Box_Eight pBox = new C_Box_Eight();
            pBox.A = A + AK_normal * k_up + AM_normal * 300;
            pBox.B = A - AK_normal * k_down + AM_normal * 300;
            pBox.C = A + AK_normal * k_up - AM_normal * 300;
            pBox.D = A - AK_normal * k_down - AM_normal * 300;

            pBox.E = A + AK_normal * k_up + AM_normal * 300 - AB_normal * 200;
            pBox.F = A - AK_normal * k_down + AM_normal * 300 - AB_normal * 200;
            pBox.G = A + AK_normal * k_up - AM_normal * 300 - AB_normal * 200;
            pBox.H = A - AK_normal * k_down - AM_normal * 300 - AB_normal * 200;

            List<C_Box_Eight> list = new List<C_Box_Eight>();
            list.Add(pBox);


            C_Point3D yueshu = (n3.B - n3.A).crossProduct(n3.P7 - n3.A);
            (C_Result pResult, _) = Main_Robot.根据坐标计算6轴角度_v2(yueshu, space, pNode, pRobot, 0, n3.A, n3.B, n3.P7, 0, 1, -1);

            if (pResult.C2 == null)
            {
                return list;
            }

            //二轴物体
            A = pResult.D * 0.5f + pResult.C2 * 0.5f;
            AB_normal = (pResult.C2 - pResult.D).normalize();
            AK_normal  =  AB_normal.crossProduct(pResult.D.normalize()).normalize();
            AM_normal = AK_normal.crossProduct(AB_normal).normalize();

            float ab_len_half = (pResult.C2 - pResult.D).length()/2;

            pBox = new C_Box_Eight();
            pBox.A = A + AK_normal * 50 + AM_normal * 50 + AB_normal * ab_len_half;
            pBox.B = A - AK_normal * 50 + AM_normal * 50 + AB_normal * ab_len_half;
            pBox.C = A + AK_normal * 50 - AM_normal * 50 + AB_normal * ab_len_half;
            pBox.D = A - AK_normal * 50 - AM_normal * 50 + AB_normal * ab_len_half;

            pBox.E = A + AK_normal * 50 + AM_normal * 50 - AB_normal * ab_len_half;
            pBox.F = A - AK_normal * 50 + AM_normal * 50 - AB_normal * ab_len_half;
            pBox.G = A + AK_normal * 50 - AM_normal * 50 - AB_normal * ab_len_half;
            pBox.H = A - AK_normal * 50 - AM_normal * 50 - AB_normal * ab_len_half;

            list.Add(pBox);

            //添加一轴相机支架，先添加底盘

            A = new C_Point3D(0,0,0);
            AB_normal = new C_Point3D(0, 0, 1);
            AK_normal = pResult.D.normalize();
            AM_normal = AB_normal.crossProduct(AK_normal).normalize();

            ab_len_half = 50;
            float ak_len_half = 300;
            float am_len_half = 300;

            pBox = new C_Box_Eight();
            pBox.A = A + AK_normal * ak_len_half + AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.B = A - AK_normal * ak_len_half + AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.C = A + AK_normal * ak_len_half - AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.D = A - AK_normal * ak_len_half - AM_normal * am_len_half + AB_normal * ab_len_half;

            pBox.E = A + AK_normal * ak_len_half + AM_normal * am_len_half - AB_normal * ab_len_half;
            pBox.F = A - AK_normal * ak_len_half + AM_normal * am_len_half - AB_normal * ab_len_half;
            pBox.G = A + AK_normal * ak_len_half - AM_normal * am_len_half - AB_normal * ab_len_half;
            pBox.H = A - AK_normal * ak_len_half - AM_normal * am_len_half - AB_normal * ab_len_half;

            list.Add(pBox);

            //相机支架
            A = AM_normal * 330;// new C_Point3D(0, 330, 0);
            AB_normal = new C_Point3D(0, 0, 1);
            AK_normal = pResult.D.normalize();
            AM_normal = AB_normal.crossProduct(AK_normal).normalize();

            ab_len_half = 1300;
            float ab_len_half2 = 0;
            ak_len_half = 50;
            am_len_half = 50;

            pBox = new C_Box_Eight();
            pBox.A = A + AK_normal * ak_len_half + AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.B = A - AK_normal * ak_len_half + AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.C = A + AK_normal * ak_len_half - AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.D = A - AK_normal * ak_len_half - AM_normal * am_len_half + AB_normal * ab_len_half;

            pBox.E = A + AK_normal * ak_len_half + AM_normal * am_len_half - AB_normal * ab_len_half2;
            pBox.F = A - AK_normal * ak_len_half + AM_normal * am_len_half - AB_normal * ab_len_half2;
            pBox.G = A + AK_normal * ak_len_half - AM_normal * am_len_half - AB_normal * ab_len_half2;
            pBox.H = A - AK_normal * ak_len_half - AM_normal * am_len_half - AB_normal * ab_len_half2;

            list.Add(pBox);



            //四轴 BC
            A = pResult.B * 0.5f+ pResult.C * 0.5f;
            AB_normal = (pResult.C - pResult.B).normalize(); //BC
            AK_normal = AB_normal.crossProduct(pResult.A-pResult.B).normalize();
            AM_normal = AB_normal.crossProduct(AK_normal).normalize();

            ab_len_half = (pResult.C - pResult.B).length() / 2;
            ab_len_half2 = ab_len_half;
            ak_len_half = 50;
            am_len_half = 50;

            pBox = new C_Box_Eight();
            pBox.A = A + AK_normal * ak_len_half + AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.B = A - AK_normal * ak_len_half + AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.C = A + AK_normal * ak_len_half - AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.D = A - AK_normal * ak_len_half - AM_normal * am_len_half + AB_normal * ab_len_half;

            pBox.E = A + AK_normal * ak_len_half + AM_normal * am_len_half - AB_normal * ab_len_half2;
            pBox.F = A - AK_normal * ak_len_half + AM_normal * am_len_half - AB_normal * ab_len_half2;
            pBox.G = A + AK_normal * ak_len_half - AM_normal * am_len_half - AB_normal * ab_len_half2;
            pBox.H = A - AK_normal * ak_len_half - AM_normal * am_len_half - AB_normal * ab_len_half2;

            list.Add(pBox);


            //六轴 BA
            A = pResult.B * 0.5f + pResult.A * 0.5f;
            AB_normal = (pResult.A - pResult.B).normalize(); //BC
            AK_normal = AB_normal.crossProduct(pResult.C - pResult.B).normalize();
            AM_normal = AB_normal.crossProduct(AK_normal).normalize();

            ab_len_half = (pResult.A - pResult.B).length() / 2;
            ab_len_half2 = ab_len_half;
            ak_len_half = 50;
            am_len_half = 50;

            pBox = new C_Box_Eight();
            pBox.A = A + AK_normal * ak_len_half + AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.B = A - AK_normal * ak_len_half + AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.C = A + AK_normal * ak_len_half - AM_normal * am_len_half + AB_normal * ab_len_half;
            pBox.D = A - AK_normal * ak_len_half - AM_normal * am_len_half + AB_normal * ab_len_half;

            pBox.E = A + AK_normal * ak_len_half + AM_normal * am_len_half - AB_normal * ab_len_half2;
            pBox.F = A - AK_normal * ak_len_half + AM_normal * am_len_half - AB_normal * ab_len_half2;
            pBox.G = A + AK_normal * ak_len_half - AM_normal * am_len_half - AB_normal * ab_len_half2;
            pBox.H = A - AK_normal * ak_len_half - AM_normal * am_len_half - AB_normal * ab_len_half2;

            list.Add(pBox);


            return list;
        }



        public static (C_Box_Eight list_point,List<Two_Point3D> list_line) 
            get_Box_Eight_Line_from_three_Point(C_Three_Point x,
            float k_up =100, float k_down =100)
        {
            C_Point3D A = x.A;
            C_Point3D B = x.B;
            C_Point3D K = x.P7;
            C_Point3D AK_normal = (K - A).normalize();
            C_Point3D AB_normal = (B - A).normalize();
            C_Point3D AM_normal = AK_normal.crossProduct(AB_normal).normalize();

            C_Box_Eight pBox = new C_Box_Eight();
            pBox.A = A + AK_normal * k_up + AM_normal * 300;
            pBox.B = A - AK_normal * k_down + AM_normal * 300;
            pBox.C= A + AK_normal * k_up - AM_normal * 300;
            pBox.D = A - AK_normal * k_down - AM_normal * 300;


            pBox.E = A + AK_normal * k_up + AM_normal * 300 - AB_normal * 200;
            pBox.F = A - AK_normal * k_down + AM_normal * 300 - AB_normal * 200;
            pBox.G = A + AK_normal * k_up - AM_normal * 300 - AB_normal * 200;
            pBox.H = A - AK_normal * k_down - AM_normal * 300 - AB_normal * 200;

            List<Two_Point3D> list_line = new List<Two_Point3D>();
            list_line.Add(new Two_Point3D(pBox.A, pBox.B));
            list_line.Add(new Two_Point3D(pBox.C, pBox.D));
            list_line.Add(new Two_Point3D(pBox.A, pBox.C));
            list_line.Add(new Two_Point3D(pBox.B, pBox.D));

            list_line.Add(new Two_Point3D(pBox.E, pBox.F));
            list_line.Add(new Two_Point3D(pBox.G, pBox.H));
            list_line.Add(new Two_Point3D(pBox.E, pBox.G));
            list_line.Add(new Two_Point3D(pBox.F, pBox.H));


            list_line.Add(new Two_Point3D(pBox.A, pBox.E));
            list_line.Add(new Two_Point3D(pBox.B, pBox.F));
            list_line.Add(new Two_Point3D(pBox.C, pBox.G));
            list_line.Add(new Two_Point3D(pBox.D, pBox.H));

            return (pBox, list_line);
        }



        public static (C_Box_Eight list_point8, List<Two_Point3D> list_line) get_Box_Eight_from_box(C_Box box)
        {

            C_Point3D A = box.pos;
            C_Box_Eight pBox = new C_Box_Eight();
            pBox.A = A + (new C_Point3D(box.x_len / 2, box.y_len / 2, box.z_len / 2));
            pBox.B = A + (new C_Point3D(box.x_len / 2, -box.y_len / 2, box.z_len / 2));
            pBox.C = A + (new C_Point3D(-box.x_len / 2, box.y_len / 2, box.z_len / 2));
            pBox.D = A + (new C_Point3D(-box.x_len / 2, -box.y_len / 2, box.z_len / 2));


            pBox.E = A + (new C_Point3D(box.x_len / 2, box.y_len / 2, -box.z_len / 2));
            pBox.F = A + (new C_Point3D(box.x_len / 2, -box.y_len / 2, -box.z_len / 2));
            pBox.G = A + (new C_Point3D(-box.x_len / 2, box.y_len / 2, -box.z_len / 2));
            pBox.H = A + (new C_Point3D(-box.x_len / 2, -box.y_len / 2, -box.z_len / 2));




            //C_Point3D p1 = A + (new C_Point3D( box.x_len / 2, box.y_len / 2, box.z_len / 2));
            //C_Point3D p2 = A + (new C_Point3D( box.x_len / 2, -box.y_len / 2, box.z_len / 2));
            //C_Point3D p3 = A + (new C_Point3D( -box.x_len / 2, box.y_len / 2, box.z_len / 2));
            //C_Point3D p4 = A + (new C_Point3D(-box.x_len / 2, -box.y_len / 2, box.z_len / 2));

            //C_Point3D p5 = A + (new C_Point3D(box.x_len / 2, box.y_len / 2, -box.z_len / 2));
            //C_Point3D p6 = A + (new C_Point3D(box.x_len / 2, -box.y_len / 2, -box.z_len / 2));
            //C_Point3D p7 = A + (new C_Point3D(-box.x_len / 2, box.y_len / 2, -box.z_len / 2));
            //C_Point3D p8 = A + (new C_Point3D(-box.x_len / 2, -box.y_len / 2, -box.z_len / 2));


            //List<C_Point3D> list = new List<C_Point3D>();

            //list.Add(p1);
            //list.Add(p2);
            //list.Add(p3);
            //list.Add(p4);
            //list.Add(p5);
            //list.Add(p6);
            //list.Add(p7);
            //list.Add(p8);

            List<Two_Point3D> list_line = new List<Two_Point3D>();
            list_line.Add(new Two_Point3D(pBox.A, pBox.B));
            list_line.Add(new Two_Point3D(pBox.C, pBox.D));
            list_line.Add(new Two_Point3D(pBox.A, pBox.C));
            list_line.Add(new Two_Point3D(pBox.B, pBox.D));

            list_line.Add(new Two_Point3D(pBox.E, pBox.F));
            list_line.Add(new Two_Point3D(pBox.G, pBox.H));
            list_line.Add(new Two_Point3D(pBox.E, pBox.G));
            list_line.Add(new Two_Point3D(pBox.F, pBox.H));


            list_line.Add(new Two_Point3D(pBox.A, pBox.E));
            list_line.Add(new Two_Point3D(pBox.B, pBox.F));
            list_line.Add(new Two_Point3D(pBox.C, pBox.G));
            list_line.Add(new Two_Point3D(pBox.D, pBox.H));

            return (pBox, list_line);
        }



        public static Vector3 rotate_vector(Vector3 vector, float angleX, float angleY, float angleZ)
        {
            // 定义旋转角度（以弧度为单位）
            //float angleX = MathF.PI / 4; // 45度
            //float angleY = MathF.PI / 4; // 45度
            //float angleZ = MathF.PI / 4; // 45度

            // 创建旋转矩阵
            Matrix4x4 rotationX = Matrix4x4.CreateRotationX(angleX);
            Matrix4x4 rotationY = Matrix4x4.CreateRotationY(angleY);
            Matrix4x4 rotationZ = Matrix4x4.CreateRotationZ(angleZ);

            // 组合旋转矩阵
            Matrix4x4 combinedRotation = rotationZ * rotationY * rotationX;

            // 定义一个向量
            //Vector3 vector = new Vector3(1, 0, 0); // 初始向量

            // 应用旋转
            Vector3 rotatedVector = Vector3.Transform(vector, combinedRotation);
            return rotatedVector;
        }



        /// <summary>
        /// 正态分布示例
        /// </summary>
        public static double random(double mean, double stdDev,double min,double max)
        {
            Normal normalDist = new Normal(mean, stdDev);

            double sample = normalDist.Sample();
            
            while (sample < min || sample > max)
            {
                sample = normalDist.Sample();
            }
            return sample;
        }
        
        public static void 创建目录如果不存在(string file)
        {
            string? path = Path.GetDirectoryName(file);
            if (path != null && Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void save_six(List<string> list2, string file1)
        {
            if (file1 == "")
            {
                Main.speak_async("文件名设置有错误！");
                return;
            }
            string time_id = DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss__FFF");
            file1 = file1.Replace("@", time_id);

            Main.创建目录如果不存在(file1);
            int count = 0;
            string line = "[\r\n";
            foreach (string six in list2)
            {
                count++;
                line += "{\"num\":\"" + count + "\",\"six\":\"" + six + "\"},\r\n";
            }
            if (line.EndsWith(",\r\n"))
            {
                line = line.Substring(0, line.Length - 3) + "\r\n";
            }
            line += "]";
            File.WriteAllText(file1, line);
        }

        public static C_Point3D 线和面交点(C_Point3D pointA, C_Point3D pointB, C_Point3D planePoint, C_Point3D planeNormal)
    {
            // 计算线段的方向向量
            C_Point3D lineDir = pointB - pointA;

            // 计算平面方程的常数项
            double D = -1 * (planeNormal*planePoint);

            // 计算线段与平面的交点
            double t = -1 * (planeNormal*pointA + D) / (planeNormal*lineDir);

            // 检查交点是否在线段上
            if (t >= 0 && t <= 1)
            {
                // 计算交点
                C_Point3D intersectionPoint = pointA + lineDir*(float)t;
                return intersectionPoint;
            }

            return null; // 没有交点
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="list_boxs"></param>
        /// <param name="dxyz">边缘放大1.5cm</param>
        /// <returns></returns>
        //public static (bool pengzhuang,C_Box_Eight box_eight) 检查碰撞(List<C_Box_Eight> list_move_box, List<C_Box> list_boxs,float dxyz=15)
        //{
        //    for(var j = 0; j < list_move_box.Count; j++)
        //    {
        //        var move_box = list_move_box[j];
        //        //读取盒子8个点,12条边信息，避免碰到盒子
        //        for (var i = 0; i < list_boxs.Count; i++)
        //        {
        //            C_Box box = list_boxs[i].Clone();
        //            box.x_len += dxyz * 2;
        //            box.y_len += dxyz * 2;
        //            box.z_len += dxyz * 2;

        //            (C_Box_Eight static_box, _) = Main.get_Box_Eight_from_box(box);

        //            if (判断盒子碰撞(move_box, static_box))
        //            {
        //                return (true, static_box);
        //            }
        //        }
        //    }
            

        //    return (false,null);
        //}

        public static bool 判断盒子碰撞(C_Box_Eight pBox_Eight, C_Box_Eight pBox_Eight2)
        {
            C_Point3D A1 = pBox_Eight.A;
            C_Point3D B1 = pBox_Eight.B;
            //C_Point3D C1 = list_point8_A[2];
            C_Point3D D1 = pBox_Eight.D;
            //C_Point3D E1 = list_point8_A[4];
            C_Point3D F1 = pBox_Eight.F;
            //C_Point3D G1 = list_point8_A[6];
            //C_Point3D H1 = list_point8_A[7];


            C_Point3D A2 = pBox_Eight2.A;
            C_Point3D B2 = pBox_Eight2.B;
            //C_Point3D C2 = list_point8_B[2];
            C_Point3D D2 = pBox_Eight2.D;
            //C_Point3D E2 = list_point8_B[4];
            C_Point3D F2 = pBox_Eight2.F;
            //C_Point3D G2 = list_point8_B[6];
            //C_Point3D H2 = list_point8_B[7];


            C_Point3D v1 = A1 - B1;
            C_Point3D v2 = D1 - B1;
            C_Point3D v3 = F1 - B1;
            
            C_Point3D v1_b = A2 - B2;
            C_Point3D v2_b = D2 - B2;
            C_Point3D v3_b = F2 - B2;


            bool bNot;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v1.crossProduct(v2));
            if (bNot) return false;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v1.crossProduct(v3));
            if (bNot) return false;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v2.crossProduct(v3));
            if (bNot) return false;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v1_b.crossProduct(v2_b));
            if (bNot) return false;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v1_b.crossProduct(v3_b));
            if (bNot) return false;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v2_b.crossProduct(v3_b));
            if (bNot) return false;
            ///////


            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v1.crossProduct(v1_b));
            if (bNot) return false;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v1.crossProduct(v2_b));
            if (bNot) return false;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v1.crossProduct(v3_b));
            if (bNot) return false;


            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v2.crossProduct(v1_b));
            if (bNot) return false;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v2.crossProduct(v2_b));
            if (bNot) return false;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v2.crossProduct(v3_b));
            if (bNot) return false;


            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v3.crossProduct(v1_b));
            if (bNot) return false;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v3.crossProduct(v2_b));
            if (bNot) return false;
            bNot = NotInteractiveOBB(pBox_Eight, pBox_Eight2, v3.crossProduct(v3_b));
            if (bNot) return false;


            return true;
        }


        /// <summary>
        /// 计算投影是否不相交
        /// </summary>
        /// <param name="vertexs1"></param>
        /// <param name="vertexs2"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static bool NotInteractiveOBB(C_Box_Eight vertexs1, C_Box_Eight vertexs2, C_Point3D axis)
        {
            //计算OBB包围盒在分离轴上的投影极限值
            float[] limit1 = GetProjectionLimit(vertexs1.toList(), axis);
            float[] limit2 = GetProjectionLimit(vertexs2.toList(), axis);
            //两个包围盒极限值不相交，则不碰撞
            return limit1[0] > limit2[1] || limit2[0] > limit1[1];
        }

        /// <summary>
        /// 计算顶点投影极限值
        /// </summary>
        /// <param name="vertexts"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float[] GetProjectionLimit(List<C_Point3D> vertexts, C_Point3D axis)
        {
            float[] result = new float[2] { float.MaxValue, float.MinValue };
            for (int i = 0, len = vertexts.Count; i < len; i++)
            {
                C_Point3D vertext = vertexts[i];
                float dot = vertext * axis.normalize();// Vector3.Dot(vertext, axis);
                result[0] = Mathf.Min(dot, result[0]);
                result[1] = Mathf.Max(dot, result[1]);
            }
            return result;
        }

        public static List<C_Point3D> 输出盒子点云(C_Box box)
        {
            List<C_Point3D> list = new List<C_Point3D>();
            (_, List<Two_Point3D> list_line) = Main.get_Box_Eight_from_box(box);

            for (var i = 0; i < list_line.Count; i++)
            {
                var item = list_line[i];
                C_Point3D p1 = item.pPoint1;
                C_Point3D p2 = item.pPoint2;

                for (var k = 0; k < 50; k++)
                {
                    C_Point3D p_new = p1 * (k / 50f) + p2 * (1f - k / 50f);
                    p_new.pExtend = new C_Color(255, 0, 0);
                    list.Add(p_new);
                }
            }
            return list;
        }

        public static List<C_Point3D> 输出List_Box_Eight点云(List<C_Box_Eight> list_boxs)
        {
            List<C_Point3D> list = new List<C_Point3D>();

            for (var i=0;i< list_boxs.Count; i++)
            {
                C_Box_Eight box = list_boxs[i];
                var list2 = 输出_Box_Eight点云(box);

                list.AddRange(list2);
            }
            return list;
        }
        public static List<C_Point3D> 输出_Box_Eight点云(C_Box_Eight box)
        {
            List<C_Point3D> list = new List<C_Point3D>();

            list.AddRange(输出两点点云(box.A, box.B));
            list.AddRange(输出两点点云(box.C, box.D));
            list.AddRange(输出两点点云(box.A, box.C));
            list.AddRange(输出两点点云(box.B, box.D));


            list.AddRange(输出两点点云(box.E, box.F));
            list.AddRange(输出两点点云(box.G, box.H));
            list.AddRange(输出两点点云(box.E, box.G));
            list.AddRange(输出两点点云(box.F, box.H));

            list.AddRange(输出两点点云(box.A, box.E));
            list.AddRange(输出两点点云(box.B, box.F));
            list.AddRange(输出两点点云(box.C, box.G));
            list.AddRange(输出两点点云(box.D, box.H));

            return list;
        }


        public static List<C_Point3D> 输出3点点云(C_Three_Point n3,float k_up,float k_down)
        {
            (_, List<Two_Point3D> list_line) = Main.get_Box_Eight_Line_from_three_Point(n3, k_up, k_down);

            List<C_Point3D> list = new List<C_Point3D>();
            for (var i = 0; i < list_line.Count; i++)
            {
                var item = list_line[i];
                var list2 = 输出两点点云(item.pPoint1,item.pPoint2);
                list.AddRange(list2);
            }
            return list;
        }

        public static List<C_Point3D> 输出两点点云(C_Point3D p1, C_Point3D p2,int n=50)
        {
            List<C_Point3D> list = new List<C_Point3D>();
            for (var k = 0; k < n; k++)
            {
                C_Point3D p_new = p1 * ((float)k / n) + p2 * (1 - (float) k/ n);
                list.Add(p_new);
            }
            return list;
        }


        public static void speak(string msg)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            try
            {
                //synth.SelectVoiceByHints(VoiceGender.Male);
                synth.Speak(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public static void speak_async(string msg)
        {
            Task.Run(() =>
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff") + " TTS=" + msg);
                int index = msg.IndexOf("+++");
                if (index > 0)
                {
                    speak(msg.Split("+++")[0]);
                }
                else
                {
                    speak(msg);
                }
            });
        }

        public static string 分割字符(string line, string split)
        {
            List<string> list = new List<string>();
            for(var i = 0; i < line.Length; i++)
            {
                list.Add(line.Substring(i, 1));
            }
            return string.Join(",", list);
        }



        public static (C_Block_Space cut1, C_Block_Space cut2, C_Block_Space cut3)
            空间切割(C_Block_Space b_space, I_Size block, string split1, string split2, string split3)
        {
            (C_Block_Space step2, C_Block_Space cut_1) = split(split1, b_space, block);
            (C_Block_Space step3, C_Block_Space cut_2) = split(split2, step2, block);
            (_, C_Block_Space cut_3) = split(split3, step3, block);

            return (cut_1, cut_2, cut_3);
        }

        public static (C_Block_Space remain, C_Block_Space cut_one) split(string split1, C_Block_Space parent, I_Size block)
        {
            C_Block_Space cut_one = null;
            C_Block_Space remain = null;
            if (split1 == "z")
            {
                if (parent.z_len - block.z_len >= 0)
                {
                    cut_one = new C_Block_Space(parent.x_len, parent.y_len, parent.z_len - block.z_len, parent.x0, parent.y0, parent.z0 + block.z_len);
                    remain = new C_Block_Space(parent.x_len, parent.y_len, block.z_len, parent.x0, parent.y0, parent.z0);
                }
                else
                {
                    return (null, null);
                }
            }
            else if (split1 == "x")
            {
                if (parent.x_len - block.x_len >= 0)
                {
                    cut_one = new C_Block_Space(parent.x_len - block.x_len, parent.y_len, parent.z_len, parent.x0 + block.x_len, parent.y0, parent.z0);
                    remain = new C_Block_Space(block.x_len, parent.y_len, parent.z_len, parent.x0, parent.y0, parent.z0);
                }
                else
                {
                    return (null, null);
                }
            }
            else if (split1 == "y")
            {
                if (parent.y_len - block.y_len >= 0)
                {
                    cut_one = new C_Block_Space(parent.x_len, parent.y_len - block.y_len, parent.z_len, parent.x0, parent.y0 + block.y_len, parent.z0);
                    remain = new C_Block_Space(parent.x_len, block.y_len, parent.z_len, parent.x0, parent.y0, parent.z0);
                }
                else
                {
                    return (null, null);
                }
            }
            return (remain, cut_one);

        }

        public static C_Point3D 从数组读取C_Point3D(List<string> pList, int index)
        {
            string strX = pList[index + 0];
            string strY = pList[index + 1];
            string strZ = pList[index + 2];

            float x = float.Parse(strX);
            float y = float.Parse(strY);
            float z = float.Parse(strZ);

            return new C_Point3D(x, y, z);
        }
    }
}
