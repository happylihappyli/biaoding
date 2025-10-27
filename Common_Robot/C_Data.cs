
using ConverxHull;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Newtonsoft.Json.Linq;
using OuelletConvexHull;
using System.Collections;
using System.Collections.Concurrent;
using System.Text;
using Test1;

namespace Common_Robot2
{
    public class C_Data
    {
        private string label = "";//包裹的标签，比如box,bag
        public string Label { get => label; set => label = value; }


        public List<C_Point3D>? rect;
        public double rect_angle;

        public List<C_Point3D>? list_3D_Point;
        public List<C_Point3D>? list_3D_Point_filter;//过滤后的点
        public List<C_Point3D>? list_3D_Point_arm;//机械臂坐标下的过滤后的点


        public StringBuilder Filter_Conidtion = new StringBuilder();
        public C_Mp3.Play_State pLast = 0;
        //非静态变量

        public int min_count = 5;//魔方格子里最少多少个点


        public string strFile = "";
        
        public bool bCatch = false;//=true 如果这个点云被抓取过了


        public JObject? JObject { get; set; }

        public List<int> sucker = new List<int>();//吸盘分区，0，1，2

        public int Group_ID = 0;//

        public C_Rect? pRect;//切割的矩形（加了偏移和缩放）
        public C_Rect? pRect_No_Move;//切割的矩形
        public Bitmap pBmp;
        public int hand_width = 200, hand_height = 100;//吸盘的宽度，高度
        public double hand_step_x = 20;
        public double hand_step_y = 20;

        public Angle hand_angle = Angle.FromDegrees(0);//旋转角度，18度转一次，转10次即可

        public List<C_Point3D> Draw_Circle = new List<C_Point3D>();
        public C_Point3D? centerAll = null;//当前空间所有点云的中间点，这个作为原点


        public FileStream? fs_debug = null;
        public TextWriter? w_debug = null;
        public List<List<C_Planet>>? All_Big_Surface = null;
        public List<C_Point3D>? pList_Point_Filter = null;//过滤掉的一些点，这个点坐标的原点也换过了

        public C_Point3D? pMax = null;
        public C_Point3D? pMin = null;
        public C_Point3D? pCenter = null;
        public C_Point3D? pMax_Relative = null;//（这个是做了平移后的最大值）
        public C_Point3D? pMin_Relative = null; //（这个是做了平移后的最小值）

        public double point_x_len = 0;  //点云x宽度
        public double point_y_len = 0;  //点云x宽度
        public double point_z_len = 0;  //点云z宽度

        //public C_Planet[,,]? GridSpace = null;//以魔方的形式，记录所有的小平面（原点变了）

        public double score = 0;

        public C_Space? space = null;
        public List<C_Point3D>? edge_points = null;

        public ConcurrentDictionary<string,Object> dic= new ConcurrentDictionary<string,Object>();

        public C_Planet_Catch planet;//抓取面 原先是 pPlanet_Top


        public bool bFilter = false;
        public bool bFilter_Size = false;//大小过滤
        public bool bFilter_Angle = false;//角度过滤
        public bool bFilter_UnEven = false;//角度过滤
        public bool bFilter_Collision = false;//碰撞过滤


        public static Task[] tasks;
        public C_Point3D[,] pPoint3Ds;
        public static List<Thread> threads = new List<Thread>();
        public bool bCover;


        public ConcurrentDictionary<string, C_Var> vars = new ConcurrentDictionary<string, C_Var>();
        public C_BoxInfo pBoxInfo;


        public C_Data(C_Space space)
        {
            this.space = space;
        }

        public void 识别区域(I_Train pTrain,C_Node pNode)
        {
            var pArray = new ArrayList();
            space.save_vars(pTrain,pNode,"Mains", "ArrayList", pArray);
        }


        public void run_pre()
        {
            centerAll = Tools.计算中心点坐标(list_3D_Point);// pList_Point_Filter);
            C_Point3D[] arr = Main.计算最大最小值(list_3D_Point);//计算x，y，z最大，最小值

            pMin = arr[0];
            pMax = arr[1];
            pCenter = arr[2];
        }


        public C_Point3D[]? 计算D和C2_C以及是否超范围_v2(C_Node pNode,C_Robot pRobot, C_Point3D B)
        {
            C_Planet_Catch pCatch = new C_Planet_Catch(space, 0, null);
            C_Point3D D = Main.计算D坐标(pRobot, B);

            C_Point3D[] arr = pCatch.计算C2坐标_v2(pNode,pRobot, B, D);

            if (arr == null)
            {
                return null;//超范围
            }
            return arr;
        }

        public RotatedRect 计算外接矩形(List<C_Point3D> pList)
        {
            Bitmap bmp = new Bitmap(1200, 900);
            Graphics e = Graphics.FromImage(bmp);
            int n = pList.Count;
            C_Point3D[] points = new C_Point3D[n];
            for (int j = 0; j < n; j++)
            {
                C_Point3D p = pList[j];
                C_Point3D p2 = p;//.add(new C_Point3D(500,300,0));//Tools.f_3DPoint_to_depth(camera1.color_calib, p.add(centerAll));
                points[j] = p2;
                e.DrawEllipse(Pens.Red, p2.x, p2.y, 10, 10);
            }

            var windowsPoints = points.Select(p => new ConverxHull.Point(p.x, p.y)).ToList();

            var ouelletConvexHull = new ConvexHull(windowsPoints);
            ouelletConvexHull.CalcConvexHull(ConvexHullThreadUsage.OnlyOne);

            List<C_Point3D> ouelletAsVertices = ouelletConvexHull.GetResultsAsArrayOfPoint()
                .Select(p => new C_Point3D(p.X, p.Y, 0)).ToList();

            Polygon currentPolygon = new Polygon();
            for (var i = 0; i < ouelletAsVertices.Count - 1; i++)
            {
                var p1 = ouelletAsVertices[i];
                var p2 = ouelletAsVertices[i + 1];
                e.DrawLine(new Pen(Color.Yellow), (float)p1.x, (float)p1.y, (float)p2.x, (float)p2.y);
                currentPolygon.AddPoint(new System.Drawing.Point((int)p1.x, (int)p1.y));
            }
            currentPolygon.CompletePolygon = true;


            double min_angle = 计算最小外接矩形(ouelletAsVertices);


            List<C_Point3D> currentPolygon2 = 旋转点云(min_angle, ouelletAsVertices);

            (C_Point3D a,C_Point3D c) = Tools.计算最大最小值(currentPolygon2);

            //C_Point3D a = pPlanet.x0y0;
            C_Point3D b = new C_Point3D(a.x, c.y, 0);
            //C_Point3D c = pPlanet.x1y1;
            C_Point3D d = new C_Point3D(c.x, a.y, 0);

            List<C_Point3D> result = new List<C_Point3D>();
            result.Add(a);
            result.Add(b);
            result.Add(c);
            result.Add(d);

            List<C_Point3D> result2 = 旋转点云(-min_angle, result);
            a = result2[0];
            b = result2[1];
            c = result2[2];
            d = result2[3];

            e.DrawLine(new Pen(Color.Blue, 3), (float)a.x, (float)a.y, (float)b.x, (float)b.y);
            e.DrawLine(new Pen(Color.Blue, 3), (float)c.x, (float)c.y, (float)b.x, (float)b.y);
            e.DrawLine(new Pen(Color.Blue, 3), (float)c.x, (float)c.y, (float)d.x, (float)d.y);
            e.DrawLine(new Pen(Color.Blue, 3), (float)a.x, (float)a.y, (float)d.x, (float)d.y);


            return new RotatedRect(a, b, c, d);

        }


        public double 计算最小外接矩形(List<C_Point3D> currentPolygon)
        {
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


            return min_angle;
        }

        public List<C_Point3D> 旋转点云(double angle, List<C_Point3D> currentPolygon)
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

        public double 计算最小外接正矩形面积(List<C_Point3D> currentPolygon)
        {


            C_Planet pPlanet = 计算最大最小值(currentPolygon);

            return Math.Abs(pPlanet.x0y0.x - pPlanet.x1y1.x) * Math.Abs(pPlanet.x0y0.y - pPlanet.x1y1.y);
        }



        public List<C_Point3D> 中心点附近30mm范围的点(
            List<C_Point3D> pList_Point_Filter, C_Point3D p1)
        {
            List<C_Point3D> pList = new List<C_Point3D>();
            for (int i = 0; i < pList_Point_Filter.Count; i++)
            {
                C_Point3D p = pList_Point_Filter[i];
                var x = Math.Abs(p.x - p1.x);
                var y = Math.Abs(p.y - p1.y);
                var d = Math.Sqrt(x * x + y * y);
                if (d < 30)
                {
                    pList.Add(p);
                }
            }
            return pList;
        }


        public List<C_Point3D> 中心点附近100mm范围的点(
            List<C_Point3D> pList_Point_Filter, C_Planet_Catch pPlanet)
        {
            List<C_Point3D> pList = new List<C_Point3D>();
            for (int i = 0; i < pList_Point_Filter.Count; i++)
            {
                C_Point3D p = pList_Point_Filter[i];
                var x = Math.Abs(p.x - pPlanet.center.x);
                var y = Math.Abs(p.y - pPlanet.center.y);
                var d = Math.Sqrt(x * x + y * y);
                if (d < 100)
                {
                    if (p.z < pPlanet.center.z + 10)
                    {
                        pList.Add(p);
                    }
                }
            }
            return pList;
        }


        //public List<C_Point3D> 中心点附近100mm范围的点(
        //    List<C_Point3D> pList_Point_Filter, C_Planet_Catch pPlanet)
        //{
        //    List<C_Point3D> pList = new List<C_Point3D>();
        //    for (int i = 0; i < pList_Point_Filter.Count; i++)
        //    {
        //        C_Point3D p = pList_Point_Filter[i];
        //        var x = Math.Abs(p.x - 0);
        //        var y = Math.Abs(p.y - 0);
        //        var d = Math.Sqrt(x * x + y * y);
        //        if (d < 100)
        //        {
        //            if (p.z < 0 + 10)
        //            {
        //                pList.Add(p);
        //            }
        //        }
        //    }
        //    return pList;
        //}


        public ArrayList 中心点附近100mm范围的其他点(
            List<C_Point3D> pList_Point_Filter, C_Planet_Catch pPlanet)
        {
            ArrayList pList = new ArrayList();
            for (int i = 0; i < pList_Point_Filter.Count; i++)
            {
                C_Point3D p = pList_Point_Filter[i];
                var x = Math.Abs(p.x - 0);
                var y = Math.Abs(p.y - 0);
                var d = Math.Sqrt(x * x + y * y);
                if (d < 100)
                {
                    if (p.z >= 0 + 10)
                    {
                        pList.Add(p);
                    }
                }
            }
            return pList;
        }


        //public void 计算ABD_v2(
        //    C_Node pNode,
        //    C_Camera_Const camera1,
        //    C_Planet Bottom,
        //    C_Robot pRobot,
        //    I_Train pTrain, C_Planet_Catch pPlanet)
        //{
        //    this.计算抓取面坐标和欧拉角等(camera1,pPlanet);

        //    pPlanet.arm_a = Tools.FromA_To_Real_A(pPlanet.arm_pCenter);

        //    pPlanet.arm_b = Tools.A_To_B(pPlanet.arm_a);
        //    pPlanet.arm_d = C_Planet_Catch.计算D坐标(pRobot, pPlanet.arm_b);

        //}

        //public void 计算ABD(
        //    C_Node pNode,
        //    C_Robot pRobot,
        //    C_Planet_Catch pPlanet)
        //{
        //    double x = 0;
        //    double y = 0;
        //    double z = 0;

        //    ////这个接触点的位置
        //    //x = pPlanet.arm_pCenter.x;
        //    //y = pPlanet.arm_pCenter.y;
        //    //z = pPlanet.arm_pCenter.z;
        //    //if (space.vars.bDebug_Mode) Main.WriteLine(this,"针尖的位置：x,y,z=" + x + "," + y + "," + z);

        //    ////这个是有吸盘的时候，针尖的位置
        //    x = pPlanet.arm_pCatchCenter.x;
        //    y = pPlanet.arm_pCatchCenter.y;
        //    z = pPlanet.arm_pCatchCenter.z;
        //    //if (space.vars.bDebug_Mode) Main.WriteLine(this,"有吸盘的时候针尖的位置：x,y,z=" + x + "," + y + "," + z);

        //    pPlanet.arm_a = new C_Point3D(x, y, z);

        //    //if (space.vars.bDebug_Mode) Main.WriteLine(this,"法向量 x,y,z=" + pPlanet.arm_faxiangliang.x + "," + pPlanet.arm_faxiangliang.y + "," + pPlanet.arm_faxiangliang.z);

        //    //315是标定针的位置，
        //    double length = -315;// + (const_arm.tool_z + pPlanet.z_add);

        //    x = pPlanet.arm_pCatchCenter.x + pPlanet.arm_faxiangliang.x / pPlanet.arm_faxiangliang.length() * length;
        //    y = pPlanet.arm_pCatchCenter.y + pPlanet.arm_faxiangliang.y / pPlanet.arm_faxiangliang.length() * length;
        //    z = pPlanet.arm_pCatchCenter.z + pPlanet.arm_faxiangliang.z / pPlanet.arm_faxiangliang.length() * length;

        //    pPlanet.arm_b = new C_Point3D(x, y, z);
        //    pPlanet.arm_d = C_Planet_Catch.计算D坐标(pRobot, pPlanet.arm_b);

        //}

        public List<C_Point3D> 移动数据中心(C_Node pNode, List<C_Point3D> pList_Point_Filter, C_Point3D center)
        {
            List<C_Point3D> pList = new List<C_Point3D>();
            for (int i = 0; i < pList_Point_Filter.Count; i++)
            {
                C_Point3D p = pList_Point_Filter[i];
                var x = p.x - centerAll.x;
                var y = p.y - centerAll.y;
                var z = p.z - centerAll.z;

                C_Point3D p2 = new C_Point3D(x, y, z);
                p2.pExtend = p.pExtend;

                pList.Add(p2);
            }
            //Main.WriteLine(this,"移动数据中心结束");
            return pList;

        }


        ////计算x，y，z最大，最小值
        public static C_Planet 计算最大最小值(List<C_Point3D> pList)
        {
            double x_min = 100000;
            double x_max = -100000;
            double y_min = 100000;
            double y_max = -100000;
            double z_min = 100000;
            double z_max = -100000;

            for (int i = 0; i < pList.Count; i++)
            {
                C_Point3D p = (C_Point3D)pList[i];
                if (p.x < x_min) x_min = p.x;
                if (p.y < y_min) y_min = p.y;
                if (p.z < z_min) z_min = p.z;

                if (p.x > x_max) x_max = p.x;
                if (p.y > y_max) y_max = p.y;
                if (p.z > z_max) z_max = p.z;
            }

            C_Planet pPlanet = new C_Planet(-1);

            pPlanet.x0y0 = new C_Point3D(x_min, y_min, z_min);
            pPlanet.x1y1 = new C_Point3D(x_max, y_max, z_max);

            return pPlanet;
        }




        private void 查找每个魔方空间的平面(
            C_Planet[,,] GridSpace,
            ArrayList pSurface,
            C_Point3D p1, C_Point3D p2)
        {
            int x_step = 1;
            int y_step = 1;
            int z_step = 1;

            List<C_Point3D> pSearch = new List<C_Point3D>();//打算搜索的位置
            for (var x0 = p1.x; x0 < p2.x; x0 += x_step)
            {
                for (var y0 = p1.y; y0 < p2.y; y0 += y_step)
                {
                    for (var z0 = p1.z; z0 < p2.z; z0 += z_step)
                    {
                        pSearch.Add(new C_Point3D(x0, y0, z0));
                    }
                }
            }

            //if (space.vars.bParallel) //如果是并行计算
            {
                Parallel.For(0, pSearch.Count, (i, loopstate) =>
                {
                    C_Point3D pPoint = pSearch[i];
                    int x_index = (int)((pPoint.x - p1.x) / x_step);
                    int y_index = (int)((pPoint.y - p1.y) / y_step);
                    int z_index = (int)((pPoint.z - p1.z) / z_step);

                    List<C_Point3D> pList_Point = Tools.查找区域的点(pList_Point_Filter,
                        pPoint.x, pPoint.x + x_step, pPoint.y, pPoint.y + y_step, pPoint.z, pPoint.z + z_step);

                    if (pList_Point.Count >= min_count)
                    {

                        Matrix<double> m = Tools.平面拟合计算法向量等_sub
                            (pList_Point);

                        double a = m[0, 0];
                        double b = m[1, 0];
                        double c = m[2, 0];
                        double c_z = -1;
                        double len = Math.Sqrt(a * a + b * b + c_z * c_z);
                        double a2 = a / len;
                        double b2 = b / len;
                        double c_z2 = -1 / len;

                        C_Point3D center = Tools.计算中心点坐标(pList_Point);

                        C_Point3D[] arr = Main.计算最大最小值(pList_Point);//计算x，y，z最大，最小值
                        double _x0 = arr[0].x;
                        double _y0 = arr[0].y;
                        double _x1 = arr[1].x;
                        double _y1 = arr[1].y;

                        C_Planet pPlanet = new C_Planet(-1);
                        pPlanet.z_faxiangliang = new C_Point3D(a2, b2, c_z2);
                        pPlanet.x0y0 = new C_Point3D(_x0, _y0, a * _x0 + b * _y0 + c * 1);
                        pPlanet.x0y1 = new C_Point3D(_x0, _y1, a * _x0 + b * _y1 + c * 1);
                        pPlanet.x1y0 = new C_Point3D(_x1, _y0, a * _x1 + b * _y0 + c * 1);
                        pPlanet.x1y1 = new C_Point3D(_x1, _y1, a * _x1 + b * _y1 + c * 1);
                        pPlanet.center = new C_Point3D(center.x, center.y, center.z);
                        pPlanet.x_index = x_index;
                        pPlanet.y_index = y_index;
                        pPlanet.z_index = z_index;
                        GridSpace[x_index, y_index, z_index] = pPlanet;
                        pSurface.Add(pPlanet);//所有的放到 pSurface
                    }
                });
            }
        }





        /// <summary>
        /// 计算圆形吸盘抓取的法向量匹配度
        /// </summary>
        /// <param name="w_debug"></param>
        private void 计算圆形吸盘抓取的法向量匹配度(C_Node pNode, int Group_ID, TextWriter w_debug)
        {
            Main.WriteLine(pNode, "计算抓取的匹配度");

            FileStream fs_vector = null;
            TextWriter w_vector = null;
            if (space.vars.bDebug)
            {
                string file_vector = @"D:\vector_catch_xyz.txt";// space.vars.path_web +
                fs_vector = new FileStream(file_vector, FileMode.Create, FileAccess.Write);
                w_vector = new StreamWriter(fs_vector, Encoding.UTF8);
            }

            if (All_Big_Surface.Count == 0)
            {
                Main.WriteLine(pNode, "error 需要搜素的面为空，前面约束条件要放松！");
            }

            for (int i = 0; i < Math.Min(15, All_Big_Surface.Count); i++)
            {
                //double max_match = 0;//每个面找一个
                if (w_debug != null) w_debug.WriteLine("==========");
                List<C_Planet> pListNew = All_Big_Surface[i];
                C_Planet pPlanet1 = (C_Planet)pListNew[0];//取第一个小平面

                C_Point3D center = Tools.计算多个小平面的中心点坐标(pListNew);

                C_Point3D fa = Tools.计算平均法向量(pListNew);
                //if (space.vars.b_auto_correct_z)
                {
                    if (fa.z < 0)
                    {
                        fa = fa*(-1);//法向量取反
                    }
                }
                UnitVector3D z2 = new Vector3D(fa.x, fa.y, fa.z).Normalize();
                //x2,y2,z2是盒子的坐标系
                //x轴和法向量上叉乘
                Vector3D z1 = new Vector3D(0, 0, 1);

                UnitVector3D y2 = z1.CrossProduct(z2).Normalize();
                UnitVector3D x2 = y2.CrossProduct(z2);

                if (w_debug != null) w_debug.WriteLine("Group_ID=" + pPlanet1.Group_ID);
                if (w_debug != null) w_debug.WriteLine("center=" + center.ToString());


                int hand_group_id;//抓取面的Group_ID

                hand_group_id = pPlanet1.Group_ID;

                C_Planet_Catch pPlanet = new C_Planet_Catch(space, Group_ID, this);

                pPlanet.pArray = pListNew;
                //加上原点的位置，换算成原先坐标。下面debug显示不用加坐标变换
                pPlanet.center = center+centerAll;

                pPlanet.Group_ID = pPlanet1.Group_ID;
                pPlanet.x_head = new C_Point3D(x2.X, x2.Y, x2.Z);
                pPlanet.z_faxiangliang = new C_Point3D(z2.X, z2.Y, z2.Z);
                pPlanet.y_Direction = new C_Point3D(y2.X, y2.Y, y2.Z);

                //pPlanet.percent = 计算匹配度(hand_group_id, center, x2, y2);
                // 计算面积匹配度(hand_group_id, center, x2, y2);//面积匹配度
                if (w_debug != null) w_debug.WriteLine(hand_group_id + "=匹配度=" + pPlanet.percent);

                //pCatch_Surfaces.Add(pPlanet);


                if (space.vars.bDebug)
                {
                    string max_vectors = center.ToString() + "," + Math.Round(x2.X, 2) + "," + Math.Round(x2.Y, 2)
                    + "," + Math.Round(x2.Z, 2) + "," + pPlanet.Group_ID + "," + pPlanet.pArray.Count + "\n";
                    max_vectors += center.ToString() + "," + pPlanet.z_faxiangliang.ToString() + "," + pPlanet.Group_ID + "," + pPlanet.pArray.Count + "\n";
                    w_vector?.WriteLine(max_vectors);
                }
            }

            if (space.vars.bDebug)
            {
                w_vector?.Close();
                fs_vector?.Close();
            }
        }



        public bool 计算是否碰撞到障碍物(
            C_Node pNode,
            List<C_Point3D> pListArray_Objs,
            I_Train pTrain, C_Planet_Catch pPlanet)
        {
            C_Camera_Const camera1 = (C_Camera_Const)space.read_vars(pTrain, pNode, "#camera1_const", "C_Camera_Const");

            if (pListArray_Objs == null)//space.vars.
            {
                Main.WriteLine("没有设置障碍物!");
                return false;
            }
            C_Point3D A = pPlanet.arm_a;
            C_Point3D B = pPlanet.arm_b;

            double r = 130;//130mm
            //判断AB为轴，r为半径的圆柱是否和障碍物的点会碰撞
            for (var i = 0; i < pListArray_Objs.Count; i++)
            {
                C_Point3D C_camera = pListArray_Objs[i];
                C_Point3D C = Main_Robot.摄像头坐标转到机械臂坐标(camera1, C_camera);

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



        //public bool 计算是否碰撞到其他包裹(
        //    C_Train pTrain, C_Planet_Catch pPlanet)
        //{

        //    double len = -(space.vars.ball_radius);// + 30);
        //    var x5 = pPlanet.arm_pCenter.x + pPlanet.arm_faxiangliang.x / pPlanet.arm_faxiangliang.length() * len;
        //    var y5 = pPlanet.arm_pCenter.y + pPlanet.arm_faxiangliang.y / pPlanet.arm_faxiangliang.length() * len;
        //    var z5 = pPlanet.arm_pCenter.z + pPlanet.arm_faxiangliang.z / pPlanet.arm_faxiangliang.length() * len;
        //    pPlanet.check_collision = new C_Point3D(x5, y5, z5);

        //    C_Camera_Const camera1 = (C_Camera_Const)space.vars.read_vars(pTrain, "#camera1_const", "C_Camera_Const");

        //    C_Point3D p1_camera = Tools.机械臂坐标到摄像头坐标(
        //        camera1, pPlanet.check_collision);
        //    C_Point3D p1_pic = Tools.f_3DPoint_to_depth(
        //        camera1.color_calib, p1_camera);
        //    //space.vars.draw_test.Add(p1_pic);


        //    C_Point3D p2_camera = Tools.机械臂坐标到摄像头坐标(
        //        camera1, pPlanet.arm_pCenter);
        //    C_Point3D p2_pic = Tools.f_3DPoint_to_depth(
        //        camera1.color_calib, p2_camera);

        //    space.vars.draw_test.Add(new Some_Point(p1_pic, p2_pic));

        //    List<C_Point3D> pList_Point3D = (List<C_Point3D>)space.vars.read_vars(pTrain, "#cloud1", "List<C_Point3D>");

        //    List<C_Point3D> pList_Point = Tools.查找区域的点(pList_Point3D,
        //        p1_camera.x - space.vars.ball_radius, p1_camera.x + space.vars.ball_radius,
        //        p1_camera.y - space.vars.ball_radius, p1_camera.y + space.vars.ball_radius,
        //        p1_camera.z - space.vars.ball_radius, p1_camera.z + space.vars.ball_radius);

        //    // int Group_ID = -1;
        //    ArrayList pCollision = new ArrayList();

        //    lock (this.pList_Collision)
        //    {

        //        for (var i = 0; i < pList_Point.Count; i++)
        //        {
        //            C_Point3D p2 = (C_Point3D)pList_Point[i];

        //            double len2 = p1_camera.distance(p2);
        //            if (len2 < space.vars.ball_radius)
        //            {
        //                //Group_ID = p2.Group_ID;
        //                if (this.Group_ID != p2.Group_ID)
        //                {
        //                    this.pList_Collision.Add(p2);
        //                    pCollision.Add(p2);
        //                }
        //            }
        //        }
        //    }

        //    if (pCollision.Count > 0)
        //        Main.WriteLine(this,this.Group_ID + " 碰撞的点个数为：" + pCollision.Count);
        //    return pCollision.Count > 5;
        //}


        //public ArrayList 显示结果(C_Train pTrain)
        //{
        //    //if (pTrain.pBitmapRGB == null) return null;

        //    //space.vars.bShowListView = true;

        //    ArrayList pListArray = new ArrayList();


        //    List<C_Data> pArrayMains = (List<C_Data>)space.vars.read_vars(pTrain,pNode, "Mains", "List<C_Main>");// new ArrayList();

        //    {
        //        for (var k = 0; k < pArrayMains.Count; k++)
        //        {
        //            C_Data pMain = (C_Data)pArrayMains[k];
        //            if (pMain != null)
        //            {
        //                for (int i = 0; i < pMain.pCatch_Surfaces.Count; i++)
        //                {
        //                    C_Planet_Catch pPlanet = (C_Planet_Catch)pMain.pCatch_Surfaces[i];
        //                    if (pPlanet != null)
        //                    {
        //                        pListArray.Add(pPlanet);
        //                    }

        //                }

        //            }
        //        }
        //    }
        //    return pListArray;
        //}


        static void 统计一个维度(double value, ConcurrentDictionary<int, C_Count> pDic)
        {
            int value2 = (int)Math.Round(value / 100);

            if (pDic.ContainsKey(value2))
            {
                C_Count pCount = pDic[value2];
                pCount.count += 1;
            }
            else
            {
                C_Count pCount = new C_Count();
                pCount.count = 1;
                pDic.TryAdd(value2, pCount);
            }
        }



        public void 输出抓取面信息()
        {
            FileStream fs_vector = null;
            TextWriter w_vector = null;
            {
                string file_vector = @"D:\vector_catch_xyz.txt";//space.vars.path_web + 
                fs_vector = new FileStream(file_vector, FileMode.Create, FileAccess.Write);
                w_vector = new StreamWriter(fs_vector, Encoding.UTF8);
            }

            w_vector.Close();
            fs_vector.Close();
        }

        private static ArrayList 合并两个大块平面(
            C_Planet_Catch pPlanet, C_Planet_Catch pPlanet2)
        {
            ArrayList pListNew = new ArrayList();
            for (var i = 0; i < pPlanet.pArray.Count; i++)
            {
                C_Planet? p1 = (C_Planet?)pPlanet.pArray[i];
                if (p1 != null)
                {
                    p1.Group_ID = pPlanet2.Group_ID;
                    pListNew.Add(p1);
                }
                    
            }
            for (var i = 0; i < pPlanet2.pArray.Count; i++)
            {
                pListNew.Add(pPlanet2.pArray[i]);
            }

            return pListNew;
        }


        public double calculate_area()
        {
            double sum = 0;
            double x1, y1, x2, y2;
            for (var i = 0; i < edge_points.Count - 1; i++)
            {
                x1 = edge_points[i].x;
                y1 = edge_points[i].y;
                x2 = edge_points[i + 1].x;
                y2 = edge_points[i + 1].y;

                sum += 0.5 * (y1 + y2) * (x1 - x2);
            }

            x1 = edge_points[edge_points.Count - 1].x;
            y1 = edge_points[edge_points.Count - 1].y;
            x2 = edge_points[0].x;
            y2 = edge_points[0].y;
            sum += 0.5 * (y1 + y2) * (x1 - x2);

            return Math.Abs(sum);
        }

        ////////////////////////////////
    }


}



