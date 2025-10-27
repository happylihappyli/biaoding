using Common_Robot2;
using ConverxHull;
using Newtonsoft.Json.Linq;
using OuelletConvexHull;

namespace Test1
{
    public class m1
    {
        public static C_Train? pTrain;
        public static string? Path_Main;
        public static Form? form=null;




        /// <summary>
        /// 思路是把高度从低到高排序，然后选择85%位置的坐标，不是最高的，最高可能有飞点
        /// </summary>
        /// <param name="grid_height2"></param>
        /// <param name="x_min"></param>
        /// <param name="x_max"></param>
        /// <param name="y_min"></param>
        /// <param name="y_max"></param>
        /// <param name="max"></param>
        /// <param name="count_min"></param>
        /// <returns></returns>
        public static int 计算当前区域的高度(
            int[,] grid_height2,
            int x_min, int x_max, int y_min, int y_max, double rate = 0.85)
        {

            List<int> list = new List<int>();

            for (int x1 = x_min; x1 < x_max + 1; x1++)
            {
                for (int y1 = y_min; y1 < y_max + 1; y1++)
                {
                    try
                    {
                        int v = grid_height2[x1, y1];
                        list.Add(v);
                    }catch(Exception ex)
                    {
                        Main.WriteLine(ex.ToString());   
                    }
                }
            }

            list.Sort(delegate (int x, int y)
            {
                return Math.Sign(x - y);
            });

            int index = (int)Math.Floor(list.Count * rate);
            return list[index];
        }


        //这个函数可能有问题
        public static C_Point3D depth_to_3DPoint3(C_Camera_Calib src_calib, C_Point3D pPoint)
        {
            //相机内参
            double cam_cx = src_calib.intrinsic.data[2];// * 640 / 1280;
            double cam_cy = src_calib.intrinsic.data[5];// * 480 / 960;
            double cam_fx = src_calib.intrinsic.data[0];// 1.09007092e+03;
            double cam_fy = src_calib.intrinsic.data[4];//1.09046729e+03;
            double factor = 1;
            if (cam_fx == 0 || cam_fy == 0)
            {
                MessageBox.Show("cam_fx，cam_fy 错误，不能为0");
            }
            if (factor == 0)
            {
                MessageBox.Show("factor 错误，不能为0");
            }
            double p_z = pPoint.z / factor;
            double p_x = (pPoint.x - cam_cx) * p_z / cam_fx;
            double p_y = (pPoint.y - cam_cy) * p_z / cam_fy;
            return new C_Point3D(p_x, p_y, p_z);
        }

        public static byte[] hext_to_byte(string hex)
        {
            string[] strSplit = hex.Split(' ');

            byte[] b = new byte[strSplit.Length];
            for (var i = 0; i < strSplit.Length; i++)
            {
                string a = strSplit[i];
                b[i] = Convert.ToByte(a, 16);
            }
            return b;
        }


        //public static byte[] hext_to_byte(string hex)
        //{
        //    byte[] b = new byte[hex.Length / 2];
        //    for (var i = 0; i < hex.Length; i += 2)
        //    {
        //        string a = hex.Substring(i, 2);
        //        b[i / 2] = Convert.ToByte(a, 16);
        //    }
        //    return b;
        //}

        public static string bit_to_hex(string bits)
        {
            //string a = "11111110";
            string c = Convert.ToInt32(bits, 2).ToString("X2");
            return c;
        }
        public static JObject get_json_from_type(string type_json)
        {
            string file = System.Windows.Forms.Application.StartupPath + "\\module.json";
            string content = File.ReadAllText(file);

            JArray module_arr = JArray.Parse(content);

            JObject? obj = null;
            for (var i = 0; i < module_arr.Count; i++)
            {
                JObject p = (JObject)module_arr[i];
                if (p.ContainsKey("type"))
                {
                    if (p?.GetValue("type")?.ToString() == type_json)
                    {
                        obj = p;
                    }
                }
            }

            return obj;
        }




        /// <summary>
        /// 计算的时候，z=0，也就是看作2D平面上的点
        /// </summary>
        /// <param name="pList"></param>
        /// <returns></returns>
        public static (List<C_Point3D> rect, double angle,double len1,double len2)
            计算外接矩形(List<C_Point3D> pList)
        {

            var windowsPoints = pList.Select(p => new ConverxHull.Point(p.x, p.y)).ToList();

            var ouelletConvexHull = new ConvexHull(windowsPoints);
            ouelletConvexHull.CalcConvexHull(ConvexHullThreadUsage.OnlyOne);

            List<C_Point3D> ouelletAsVertices = ouelletConvexHull.GetResultsAsArrayOfPoint()
                .Select(p => new C_Point3D(p.X, p.Y, 0)).ToList();

            Polygon currentPolygon = new Polygon();
            for (var i = 0; i < ouelletAsVertices.Count - 1; i++)
            {
                var p1 = ouelletAsVertices[i];
                var p2 = ouelletAsVertices[i + 1];
                currentPolygon.AddPoint(new System.Drawing.Point((int)p1.x, (int)p1.y));
            }
            currentPolygon.CompletePolygon = true;


            double min_angle = Main.计算最小外接矩形(ouelletAsVertices);

            List<C_Point3D> currentPolygon2 = Main.旋转点云(min_angle, ouelletAsVertices);

            (C_Point3D a, C_Point3D c) = Tools.计算最大最小值(currentPolygon2);


            //C_Point3D a = pPlanet.x0y0;
            //C_Point3D c = pPlanet.x1y1;
            C_Point3D b = new C_Point3D(a.x, c.y, 0);
            C_Point3D d = new C_Point3D(c.x, a.y, 0);

            List<C_Point3D> result = new List<C_Point3D>();
            result.Add(a);
            result.Add(b);
            result.Add(c);
            result.Add(d);

            List<C_Point3D> result2 = Main.旋转点云(-min_angle, result);
            a = result2[0];
            b = result2[1];
            c = result2[2];
            d = result2[3];

            double alen1 = 0;
            double alen2 = 0;

            alen1 = a.distance(b);
            alen2 = b.distance(c);

            if (alen1 > alen2)
            {
                return (result2, min_angle + Math.PI / 2, alen1, alen2);// new RotatedRect(a, b, c, d);
            }
            else
            {
                List<C_Point3D> result3 = new List<C_Point3D>();
                result3.Add(b);
                result3.Add(c);
                result3.Add(d);
                result3.Add(a);

                return (result3, min_angle,alen2,alen1);// new RotatedRect(a, b, c, d);

            }


        }


    }
}
