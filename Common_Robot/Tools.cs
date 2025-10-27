
using Common_Robot2;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Spatial.Euclidean;
using MathNet.Spatial.Units;
using Newtonsoft.Json.Linq;
using Renci.SshNet.Sftp;
using ConverxHull;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using B2_Treap.Funny;
using System.Collections.Concurrent;
using Test1;

namespace Common_Robot2
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Tools
    {
        C_Space space;
        public Tools(C_Space space)
        {
            this.space = space;
        }


        public static double time1 = 0;
        public static void Time_Start()
        {
            time1 = DateTime.Now.Ticks;
        }

        public static void Time_Print(C_Node pNode, string tag)
        {
            double time2 = (DateTime.Now.Ticks - time1) / 10000;
            Main.WriteLine(pNode.Name + " End " + tag + " 毫秒= " + time2);
        }



        public static string ticks_to_time(long numberOfTicks)
        {
            DateTime myDate = new DateTime(numberOfTicks);
            return myDate.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string? getCpu()
        {
            string? strCpu = null;
            ManagementClass myCpu = new ManagementClass("win32_Processor");
            ManagementObjectCollection myCpuConnection = myCpu.GetInstances();
            foreach (ManagementObject myObject in myCpuConnection)
            {
                strCpu = myObject.Properties["Processorid"].Value.ToString();
                break;
            }
            return strCpu;
        }



        //取第一块硬盘编号      
        public static string? GetHardDiskID()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                string? strHardDiskID = null;
                foreach (ManagementObject mo in searcher.Get())
                {
                    strHardDiskID = mo?["SerialNumber"]?.ToString()?.Trim();
                    break;
                }
                return strHardDiskID;
            }
            catch
            {
                return "";
            }
        }//end       



        ///// <summary>
        ///// 获取网卡ID，这个有问题，最好别用
        ///// </summary>
        ///// <returns></returns>
        //public static string GetMacAddressNumber()
        //{
        //    try
        //    {
        //        string mac = "";
        //        ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        //        ManagementObjectCollection moc = mc.GetInstances();
        //        foreach (ManagementObject mo in moc)
        //            if ((bool)mo["IPEnabled"] == true)
        //            {
        //                mac += mo["MacAddress"].ToString() + " ";
        //                break;
        //            }
        //        moc = null;
        //        mc = null;
        //        return mac.Trim();
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message + "uMnIk";
        //    }
        //}


        public static string get_md5(string code)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(code);
            string hash;

            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                hash = String.Concat(md5.ComputeHash(bytes)
                .Select(x => x.ToString("x2")));
            }
            return hash;
        }
        public static string UnBase64String(string value)
        {
            if (value == null || value == "")
            {
                return "";
            }
            byte[] bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }

        public static bool check_serial(string? number)
        {
            string a = getCpu() + "-" + GetHardDiskID();
            string md5= get_md5(a);

            string salt = UnBase64String("bWF0aGZhbi5jb20uZnVubnlhaS5jb20uc25kdmlzaW9uLmNu");

            string strLine = get_md5(md5 +salt);
            return (strLine == number);
        }

        public static async void download_file(string url,string file)
        {

            HttpClient httpClient = new HttpClient();
            var uri = new Uri(url);
            var response = await httpClient.GetAsync(uri);
            using (var fs = new FileStream(file,FileMode.CreateNew))
            {
                await response.Content.CopyToAsync(fs);
            }

            //WebClient mywebclient = new WebClient();
            //try
            //{
            //    mywebclient.DownloadFile(url, file);
            //    //mywebclient.do
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
        }
        public static byte[] Base64Decode(string base64EncodedData)
        {
            return Convert.FromBase64String(base64EncodedData);
        }


        public static string? read(JObject pObj2, string key)
        {
            string? result = "";
            if (pObj2.ContainsKey(key))
            {
                result = pObj2.GetValue(key)?.ToString();
            }
            return result;
        }

        public string var_read(I_Train pTrain,C_Node pNode,string value)
        {
            string value2 = value;
            if (value2.StartsWith("@"))
            {
                value2 = value2.Substring(1);

                object? obj= space.read_vars(pTrain, pNode, value2, "string");
                if (obj == null) return "";
                var type = obj.GetType().Name.ToLower();
                switch (type)
                {
                    case "string":
                        value2 = (string)obj;
                        break;
                    case "double":
                        value2 = (double)obj + "";
                        break;
                    case "float":
                        value2 = (float)obj + "";
                        break;
                    case "int":
                        value2 = (int)obj + "";
                        break;
                }
            }
            return value2;
        }


        public static double[,] YUV2RGB_CONVERT_MATRIX = new double[3, 3] { { 1, 0, 1.4022 }, { 1, -0.3456, -0.7145 }, { 1, 1.771, 0 } };

        public static C_Bmp ImageTo_C_BMP(Bitmap bmp)
        {
            BitmapData bmdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] buffer = new byte[bmdata.Stride * bmdata.Height - 1];// As Byte
            Marshal.Copy(bmdata.Scan0, buffer, 0, buffer.Length);
            int stride = bmdata.Stride;
            bmp.UnlockBits(bmdata);


            return new C_Bmp(buffer, stride, bmp.Width, bmp.Height);
        }

        
        public static byte[]? ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[]?)converter?.ConvertTo(img, typeof(byte[]));
        }


        public static void init(string strFile,C_Camera_Const p1)
        {

            if (File.Exists(strFile) == false)
            {
                return ;
            }
            double[] m = Tools.计算变换矩阵所有参数(strFile);
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

            double m1 = m[9];
            double m2 = m[10];
            double m3 = m[11];


            p1.p1_center = new C_Point3D(m[12], m[13], m[14]);
            p1.p2_center = new C_Point3D(m[15], m[16], m[17]);


            Main.WriteLine("(a1 a2 a3 a4 a5 a6 a7 a8 a9 m1 m2 m3)="
                + a1 + "," + a2 + "," + a3 + ","
                + a4 + "," + a5 + "," + a6 + ","
                + a7 + "," + a8 + "," + a9 + ","
                + m1 + "," + m2 + "," + m3);


            double[,] d = new double[3, 3];
            d[0, 0] = a1;
            d[0, 1] = a2;
            d[0, 2] = a3;
            //d[0, 3] = m1;
            d[1, 0] = a4;
            d[1, 1] = a5;
            d[1, 2] = a6;
            //d[1, 3] = m2;
            d[2, 0] = a7;
            d[2, 1] = a8;
            d[2, 2] = a9;
            //d[2, 3] = m3;
            //d[3, 0] = 0;
            //d[3, 1] = 0;
            //d[3, 2] = 0;
            //d[3, 3] = 1;

            p1.M_Rotate= DenseMatrix.OfArray(d);//坐标变换矩阵
        }







        ////计算x，y，z最大，最小值
        public static (C_Point3D x1, C_Point3D x2) 计算最大最小值(List<C_Point3D> pList)
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

            //C_Planet pPlanet = new C_Planet(-1);

            C_Point3D x0y0 = new C_Point3D(x_min, y_min, z_min);
            C_Point3D x1y1 = new C_Point3D(x_max, y_max, z_max);

            return (x0y0, x1y1);
        }



        public void print_states()
        {
            foreach(var item in space.vars_step)
            {
                Main.WriteLine("=============================");
                Main.WriteLine(item.Key);
                Main.WriteLine(item.Value?.Name);
                Main.WriteLine("active_input_count="+item.Value?.active_input_count);

                if (item.Value!=null)
                Main.WriteLine("active_now=" + string_array(item.Value.active_now, item.Value.active_input_count));
            }
        }

        public static string string_array(int[] active_now,int len)
        {
            string strReturn = "";
            for(var i=0; i<Math.Min(len, active_now.Length); i++)
            {
                strReturn += active_now[i] + ",";
            }
            return strReturn;
        }

        public static double 到底面距离(C_Point3D pPoint2, C_Planet pPlanet_Bottom)
        {
            C_Point3D tmp = pPoint2-(pPlanet_Bottom.center);
            double check = tmp*(pPlanet_Bottom.z_faxiangliang);
            return check;
        }


        /// <summary>
        /// hudu zhuan jiaodu
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float rad_to_degree(double angle)
        {
            float a = (float)Math.Round(angle * 180 / Math.PI, 3);
            string x = a.ToString();
            if (x == "-0")
            {
                return 0;
            }
            return a;
        }


        public static C_Point3D 计算平均法向量(List<C_Planet> pList_Planet)
        {
            double[] d = new double[3];

            double sum_x = 0;
            double sum_y = 0;
            double sum_z = 0;

            for (int i = 0; i < pList_Planet.Count; i++)
            {
                C_Planet pPlanet = (C_Planet)pList_Planet[i];
                sum_x += pPlanet.z_faxiangliang.x;
                sum_y += pPlanet.z_faxiangliang.y;
                sum_z += pPlanet.z_faxiangliang.z;

            }
            d[0] = sum_x / pList_Planet.Count;
            d[1] = sum_y / pList_Planet.Count;
            d[2] = sum_z / pList_Planet.Count;

            return new C_Point3D(d[0], d[1], d[2]);
        }

        public static C_Point3D 计算中心点坐标(List<C_Point3D> pList_Point)
        {
            double[] d = new double[3];

            double sum_x = 0;
            double sum_y = 0;
            double sum_z = 0;

            int count = 0;
            for (int i = 0; i < pList_Point.Count; i++)
            {
                C_Point3D p = pList_Point[i];
                if (p != null) {

                    sum_x += p.x;
                    sum_y += p.y;
                    sum_z += p.z;
                    count++;
                }

            }
            d[0] = sum_x / count;
            d[1] = sum_y / count;
            d[2] = sum_z / count;

            return new C_Point3D(d[0], d[1], d[2]);
        }


        public static double 点到多边形的最短距离(Polygon polygon, double X, double Y)
        {
            List<PointF> Points = polygon.points;

            double min = 1000000;
            int max_point = Points.Count - 1;
            for (int i = 0; i < max_point; i++)
            {
                double distance = 点到线的距离(Points[i], Points[i + 1], X, Y);
                if (distance < min) min = distance;
            }
            return min;
        }

        public static double 点到线的距离(
            PointF line_p1, PointF line_p2,
            double x, double y)
        {
            C_Point3D p1 = new C_Point3D(line_p1.X, line_p1.Y, 0);
            C_Point3D p2 = new C_Point3D(line_p2.X, line_p2.Y, 0);
            C_Point3D p3 = new C_Point3D(x, y, 0);

            C_Point3D v1 = (p2 -p1).normalize();

            C_Point3D v2 = p2-p3;

            C_Point3D v2_cos = v1*(v2*v1);

            C_Point3D v2_sin = v2-v2_cos;

            return v2_sin.length();

        }

        public static C_Point3D 计算多个小平面的中心点坐标(List<C_Planet> pList_Planet)
        {
            double[] d = new double[3];

            double sum_x = 0;
            double sum_y = 0;
            double sum_z = 0;

            for (int i = 0; i < pList_Planet.Count; i++)
            {
                C_Planet p = pList_Planet[i];
                sum_x += p.center.x;
                sum_y += p.center.y;
                sum_z += p.center.z;

            }
            d[0] = sum_x / pList_Planet.Count;
            d[1] = sum_y / pList_Planet.Count;
            d[2] = sum_z / pList_Planet.Count;

            return new C_Point3D(d[0], d[1], d[2]);
        }



        //最小二乘方法计算
        public static C_Point3D 平面拟合计算法向量等(List<C_Point3D> pList_Point,bool b_auto_correct_z=true)
        {
            Matrix<double> m = 平面拟合计算法向量等_sub(pList_Point);

            double a = m[0, 0];
            double b = m[1, 0];
            double c = m[2, 0];
            double c_z = -1;
            double len = Math.Sqrt(a * a + b * b + c_z * c_z);
            double a2 = a / len;
            double b2 = b / len;
            double c_z2 = -1 / len;

            C_Point3D fa = new C_Point3D(a2, b2, c_z2);

            if (b_auto_correct_z)
            {
                if (fa.z < 0) fa = fa*(-1);//法向量取反,朝下
            }
            return fa;
        }



        //通过A计算B
        public static C_Point3D A_To_B(C_Point3D a)
        {

            double len = Math.Sqrt(a.x * a.x + a.y * a.y);
            double len_new = len - 225;

            double x_new = a.x * len_new / len;
            double y_new = a.y * len_new / len;

            return new C_Point3D(x_new, y_new, a.z);
        }




        /// <summary>
        /// 通过转换矩阵计算从摄像头坐标系到机械臂坐标系,estun
        /// </summary>
        /// <param name="pPoint"></param>
        /// <returns></returns>


        //计算的坐标是类似安川机械臂的原点
        public static C_Point3D FromA_To_Real_A(C_Point3D a)
        {
            return new C_Point3D(a.x, a.y, a.z - 468);
        }



        public static Matrix<double> 平面拟合计算法向量等_sub
            (List<C_Point3D> pList_Point)
        {
            double[,] d = new double[pList_Point.Count, 3];
            double[,] d2 = new double[pList_Point.Count, 1];
            for (int i = 0; i < pList_Point.Count; i++)
            {
                C_Point3D p = pList_Point[i];
                d[i, 0] = p.x;
                d[i, 1] = p.y;
                d[i, 2] = 1;

                d2[i, 0] = p.z;
            }
            DenseMatrix M = DenseMatrix.OfArray(d);
            DenseMatrix Y = DenseMatrix.OfArray(d2);

            return (M.Transpose().Multiply(M)).Inverse().Multiply(M.Transpose()).Multiply(Y);
        }



        public static Matrix<double> 拟合摄像头参数(List<C_Point3D> pListCamera, List<C_Point3D> pListCamera2)
        {
            double[,] d = new double[pListCamera.Count * 2, 4];
            double[,] d2 = new double[pListCamera.Count * 2, 1];
            for (int i = 0; i < pListCamera.Count; i++)
            {
                C_Point3D p1 = (C_Point3D)pListCamera[i];
                C_Point3D p2 = (C_Point3D)pListCamera2[i];
                d[i * 2 + 0, 0] = p1.z;
                d[i * 2 + 0, 1] = 0;
                d[i * 2 + 0, 2] = p2.x;
                d[i * 2 + 0, 3] = 0;

                d[i * 2 + 1, 0] = 0;
                d[i * 2 + 1, 1] = p1.z;
                d[i * 2 + 1, 2] = 0;
                d[i * 2 + 1, 3] = p2.y;

                d2[i * 2 + 0, 0] = p1.x * p1.z;
                d2[i * 2 + 1, 0] = p1.y * p1.z;
            }
            DenseMatrix M = DenseMatrix.OfArray(d);
            DenseMatrix Y = DenseMatrix.OfArray(d2);

            return (M.Transpose().Multiply(M)).Inverse().Multiply(M.Transpose()).Multiply(Y);
        }



        public static C_Matrix 计算变换矩阵(string strFile)
        {

            C_Matrix matrix = new C_Matrix();


            double[] m = Tools.计算变换矩阵所有参数(strFile);
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


            matrix.p1_center = new C_Point3D(m[12], m[13], m[14]);  //原先的中心坐标
            matrix.p2_center = new C_Point3D(m[15], m[16], m[17]);  //变换后中心坐标


            Main.WriteLine("(a1 a2 a3 a4 a5 a6 a7 a8 a9 m1 m2 m3)="
                + a1 + "," + a2 + "," + a3 + ","
                + a4 + "," + a5 + "," + a6 + ","
                + a7 + "," + a8 + "," + a9 + ","
                + _m1 + "," + m2 + "," + m3);


            double[,] d = new double[3, 3];
            d[0, 0] = a1;
            d[0, 1] = a2;
            d[0, 2] = a3;
            //d[0, 3] = m1;
            d[1, 0] = a4;
            d[1, 1] = a5;
            d[1, 2] = a6;
            //d[1, 3] = m2;
            d[2, 0] = a7;
            d[2, 1] = a8;
            d[2, 2] = a9;
            //d[2, 3] = m3;
            //d[3, 0] = 0;
            //d[3, 1] = 0;
            //d[3, 2] = 0;
            //d[3, 3] = 1;

            matrix.M_Rotate = DenseMatrix.OfArray(d);//坐标变换矩阵

            return matrix;
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Kabsch_algorithm
        /// </summary>
        /// <param name="pList_Point"></param>
        /// <returns></returns>
        public static double[] 计算变换矩阵所有参数(string strFile)
        {

            List<C_Point3D> pList_Point1 = new List<C_Point3D>();
            List<C_Point3D> pList_Point2 = new List<C_Point3D>();
            StreamReader file = new StreamReader(strFile);
            string? line;
            double sum_x1 = 0;
            double sum_y1 = 0;
            double sum_z1 = 0;
            double sum_x2 = 0;
            double sum_y2 = 0;
            double sum_z2 = 0;
            int n = 0;
            while ((line = file.ReadLine()) != null)
            {
                line = line.Replace("，", ",");
                line = line.Replace(" ", ",");
                line = line.Replace(",,", ",");
                string[] strSplit = line.Split(',');
                if (strSplit.Length > 5)
                {

                    try
                    {
                        double x1 = double.Parse(strSplit[0]);
                        double y1 = double.Parse(strSplit[1]);
                        double z1 = double.Parse(strSplit[2]);
                        double x2 = double.Parse(strSplit[3]);
                        double y2 = double.Parse(strSplit[4]);
                        double z2 = double.Parse(strSplit[5]);

                        sum_x1 += x1;
                        sum_y1 += y1;
                        sum_z1 += z1;

                        sum_x2 += x2;
                        sum_y2 += y2;
                        sum_z2 += z2;

                        pList_Point1.Add(new C_Point3D(x1, y1, z1));
                        pList_Point2.Add(new C_Point3D(x2, y2, z2));
                        n++;
                    }
                    catch (Exception e)
                    {
                        Main.WriteLine(e.ToString());
                        Main.WriteLine("error:" + line);
                    }


                }
            }
            file.Close();
            C_Point3D p1_center = new C_Point3D(sum_x1 / n, sum_y1 / n, sum_z1 / n);
            C_Point3D p2_center = new C_Point3D(sum_x2 / n, sum_y2 / n, sum_z2 / n);


            double[,] d1 = new double[pList_Point1.Count, 3];
            double[,] d2 = new double[pList_Point1.Count, 3];
            for (int i = 0; i < pList_Point1.Count; i++)
            {
                C_Point3D p1 = (C_Point3D)pList_Point1[i];
                C_Point3D p2 = (C_Point3D)pList_Point2[i];
                d1[i, 0] = p1.x - p1_center.x;
                d1[i, 1] = p1.y - p1_center.y;
                d1[i, 2] = p1.z - p1_center.z;

                d2[i, 0] = p2.x - p2_center.x; ;
                d2[i, 1] = p2.y - p2_center.y; ;
                d2[i, 2] = p2.z - p2_center.z; ;
            }
            DenseMatrix P = DenseMatrix.OfArray(d1);
            DenseMatrix Q = DenseMatrix.OfArray(d2);
            Matrix<double> H=P.Transpose().Multiply(Q);

            Svd<double> svd=H.Svd();
            Matrix<double> U = svd.U;
            Matrix<double> V = svd.VT.Transpose();
            double d=(double)Math.Sign(V.Multiply(U.Transpose()).Determinant());

            double[,] dx = { { 1, 0, 0 },{ 0, 1, 0 },{ 0, 0, d } };
            
            Matrix<double> X = DenseMatrix.OfArray(dx);
            //Matrix<double> R=V.Multiply(X).Multiply(U.Transpose());
            Matrix<double> R = V.Multiply(X).Multiply(U.Transpose()).Transpose();

            C_Point3D offset=p2_center-p1_center;
            double[] RR= {R[0,0],R[0,1],R[0,2], R[1, 0], R[1, 1], R[1, 2], R[2, 0], R[2, 1], R[2, 2],
            offset.x,offset.y,offset.z,
            p1_center.x,p1_center.y,p1_center.z,p2_center.x,p2_center.y,p2_center.z};
            return RR;
        }





        public static int 找到大于20的最小分组(ConcurrentDictionary<int, C_Count> pDic_X)
        {
            int iMin = 10000;
            foreach (var item in pDic_X)
            {
                if (item.Value.count > 20)
                {
                    if (item.Key < iMin) iMin = item.Key;
                }
            }
            return iMin;
        }

        public static int 找到大于20的最大分组(ConcurrentDictionary<int, C_Count> pDic_X)
        {
            int iMax = -10000;
            foreach (var item in pDic_X)
            {
                if (item.Value.count > 20)
                {
                    if (item.Key > iMax) iMax = item.Key;
                }
            }
            return iMax;
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
        /// 所有点在抓取面上拟合一条直线的角度
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="pList"></param>
        /// <param name="pCatch"></param>
        /// <returns></returns>
        public double 计算第六轴角度(
            C_Node pNode,
            C_Point3D A, C_Point3D B, C_Point3D C,
            List<C_Point3D> pList)
        {
            if (pList == null)
            {
                return 0;
            }

            C_Point3D BC = C-B;
            C_Point3D BA = A-B;
            Vector3D v_bc = new Vector3D(BC.x, BC.y, BC.z);
            Vector3D v_ba = new Vector3D(BA.x, BA.y, BA.z);

            UnitVector3D v_OY = v_bc.CrossProduct(v_ba).Normalize();
            UnitVector3D v_OX = v_OY.CrossProduct(v_ba).Normalize();

            List<C_Point3D> pList_Point = new List<C_Point3D>();
            double sum_x = 0;
            double sum_y = 0;
            for (int i = 0; i < pList.Count; i++)
            {
                C_Point3D p = (C_Point3D)pList[i];

                C_Point3D A_p =p-A;

                Vector3D v_A_p = new Vector3D(A_p.x, A_p.y, A_p.z);
                
                double x = v_A_p.DotProduct(v_OX);
                double y = v_A_p.DotProduct(v_OY);
                pList_Point.Add(new C_Point3D(x, y, 0));
                sum_x += x;
                sum_y += y;
            }


            //Vector3D v_p0 = new Vector3D(A.x, A.y, A.z);
            double x0 = sum_x/ pList.Count;// v_p0.DotProduct(v_OX);
            double y0 = sum_y/ pList.Count;//'v_p0.DotProduct(v_OY);



            double[,] d = new double[pList_Point.Count, 2];
            double[,] d2 = new double[pList_Point.Count, 1];


            string file_debug =  @"D:\debug_angle6.txt";//space.vars.path_web +
            Main.WriteLine("调试文件=" + file_debug);
            FileStream? fs_debug=null;
            StreamWriter? w_debug = null;

            if (space.vars.bDebug_Mode)
            {
                fs_debug = new FileStream(file_debug, FileMode.Create, FileAccess.Write);
                w_debug = new StreamWriter(fs_debug, Encoding.UTF8);
            }

            for (int i = 0; i < pList_Point.Count; i++)
            {
                C_Point3D p = (C_Point3D)pList_Point[i];
                d[i, 0] = p.x;
                d[i, 1] = 1;

                d2[i, 0] = p.y;

                if (w_debug != null) w_debug.WriteLine(p.ToString());
            }

            double min_k = 0;//最小角度
            double min_distance = 99999999;
            for (double k = -90; k < 90; k+=2)
            {
                double a1 =Math.Cos(k/180*Math.PI);
                double b1 = Math.Sin(k / 180 * Math.PI);
                double c1 = -(a1 * x0 + b1 * y0);

                double sum_distance = 0;
                for (int i = 0; i < pList_Point.Count; i++)
                {
                    C_Point3D p = pList_Point[i];

                    sum_distance += Math.Abs(a1 * p.x + b1 * p.y + c1) / Math.Sqrt(a1*a1+b1*b1);
                }
                if (sum_distance < min_distance)
                {
                    min_distance=sum_distance;
                    min_k = k;
                    //C_Main.WriteLine("a=" + k + ",d=" + min_distance);
                }
            }


            if (space.vars.bDebug_Mode)
            {
                w_debug?.Close();
                if (fs_debug != null) fs_debug.Close();
            }

            return min_k / 180 * Math.PI;
        }




        //public static void 搜索周围8个位置(
        //    C_Main pMain,
        //    TextWriter w_debug,
        //    int Group_ID,
        //    int[,,] check, C_Planet[,,] GridSpace, ArrayList pListNew,
        //        C_Planet pPlanet_Search, C_Planet pPlanet_First)
        //{

        //    int x_index = pPlanet_Search.x_index;
        //    int y_index = pPlanet_Search.y_index;
        //    int z_index = pPlanet_Search.z_index;


        //    if (w_debug != null) w_debug.WriteLine("search =" + x_index + "," + y_index + "," + z_index);

        //    for (int x = x_index - 1; x < x_index + 2; x++)
        //    {

        //        for (int y = y_index - 1; y < y_index + 2; y++)
        //        {
        //            for (int z = z_index - 1; z < z_index + 2; z++)
        //            {
        //                if (x >= 0 && x < pMain.x_count &&
        //                    y >= 0 && y < pMain.y_count &&
        //                    z >= 0 && z < pMain.z_count)
        //                {
        //                    if (check[x, y, z] == 0)
        //                    {

        //                        check[x, y, z] = 1;//代表搜索过或者不需要搜素
        //                        if (GridSpace[x, y, z] != null)
        //                        {
        //                            if (w_debug != null) w_debug.WriteLine("check =" + x + "," + y + "," + z);
        //                            检查是否相似_如果满足相似条件_添加(pMain, w_debug, Group_ID, check, GridSpace, pListNew, pPlanet_First, GridSpace[x, y, z]);

        //                        }
        //                    }
        //                }
        //            }

        //        }
        //    }

        //}


        //两个平面相似的条件是，法向量角度差小于  5度=pi/2*5/90
        //小抓取面原点在p1法向量上的投影距离小于1
        //private static void 检查是否相似_如果满足相似条件_添加(
        //    C_Main pMain,
        //    TextWriter w_debug,
        //    int Group_ID,
        //    int[,,] check,
        //    C_Planet[,,] GridSpace,
        //    ArrayList pListNew, C_Planet pPlanet_First, C_Planet p2)
        //{
        //    if (p2 == null) return;
        //    double cos_value = pPlanet_First.z_faxiangliang.x * p2.z_faxiangliang.x +
        //        pPlanet_First.z_faxiangliang.y * p2.z_faxiangliang.y +
        //        pPlanet_First.z_faxiangliang.z * p2.z_faxiangliang.z;
        //    if (Angle.FromRadians(Math.Acos(cos_value)).Degrees < const_arm.yuzhi_faxiangliang_angle)
        //    {
        //        double cos_value1 = pPlanet_First.x0y0.x * pPlanet_First.z_faxiangliang.x +
        //                pPlanet_First.x0y0.y * pPlanet_First.z_faxiangliang.y +
        //                pPlanet_First.x0y0.z * pPlanet_First.z_faxiangliang.z;
        //        double cos_value2 = p2.x0y0.x * pPlanet_First.z_faxiangliang.x +
        //                p2.x0y0.y * pPlanet_First.z_faxiangliang.y +
        //                p2.x0y0.z * pPlanet_First.z_faxiangliang.z;
        //        if (Math.Abs(cos_value1 - cos_value2) < const_arm.yuzhi_surface_distance)
        //        {
        //            if (p2.Group_ID == 0)
        //            {
        //                if (w_debug != null) w_debug.WriteLine("add =" + Group_ID);
        //                p2.Group_ID = Group_ID;
        //                pListNew.Add(p2);
        //                搜索周围8个位置(pMain, w_debug,
        //                    Group_ID, check, GridSpace, pListNew, p2, pPlanet_First);
        //            }
        //        }
        //    }
        //}


        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }


        public Color RGB_from_int(int color)
        {
            int r = 0xFF & color;
            int g = 0xFF00 & color;
            g >>= 8;
            int b = 0xFF0000 & color;
            b >>= 16;
            return Color.FromArgb(r, g, b);
        }

        public static double GetHardDiskFreeSpace(string str_HardDiskName)

        {

            double freeSpace =0;

            str_HardDiskName = str_HardDiskName + ":\\";

            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();

            foreach (System.IO.DriveInfo drive in drives)

            {

                if (drive.Name == str_HardDiskName)
                {

                    freeSpace = drive.TotalFreeSpace / (1024 * 1024 * 1024);

                }
            }

            return freeSpace;

        }



        //public static byte[] Copy2Array(uint8_t_ARRAY color_pixel_arr, int width, int height)// byte[] imgBGR)
        //{
        //    //C_Main.WriteLine("ByteToBitmap Start",true);
        //    //Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

        //    int len = width * height * 3;
        //    //构造一个位图数组进行数据存储
        //    byte[] rgbvalues = new byte[len];

        //    //C_Main.WriteLine("开始内存复制图片1", true);
        //    {
        //        IntPtr ptr2 = color_pixel_arr.VoidPtr2();

        //        Marshal.Copy(ptr2, rgbvalues, 0, len);
        //    }
        //    //C_Main.WriteLine("开始内存复制图片2", true);

        //    return rgbvalues;
        //}



        


        public static Bitmap CopyBmp(Bitmap bmp_source)// byte[] imgBGR)
        {
            int width = bmp_source.Width;
            int height = bmp_source.Height;

            //C_Main.WriteLine("ByteToBitmap Start");
            Rectangle rect = new Rectangle(0, 0, width, height);

            BitmapData bmpdata1 = bmp_source.LockBits(rect, ImageLockMode.ReadOnly, bmp_source.PixelFormat);
            IntPtr ptr1 = bmpdata1.Scan0;

            //Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            int len = width * height * 4;
            //构造一个位图数组进行数据存储
            byte[] rgbvalues = new byte[len];
            //C_Main.WriteLine("开始内存复制图片1");
            {
                int offset = 0;
                //long ptr = bmpData.Scan0.ToInt64();
                for (int i = 0; i < height; i++)
                {
                    Marshal.Copy(ptr1, rgbvalues, offset, width * 3);
                    offset += width * 3;
                    ptr1 += bmpdata1.Stride;
                }


                //Marshal.Copy(ptr1, rgbvalues, 0, len);
            }
            //C_Main.WriteLine("开始内存复制图片2");

            //以可读写的方式将图像数据锁定
            BitmapData bmpdata = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            //得到图形在内存中的首地址
            IntPtr ptr = bmpdata.Scan0;

            //把处理后的图像数组复制回图像
            Marshal.Copy(rgbvalues, 0, ptr, len);

            //解锁位图像素
            bmp_source.UnlockBits(bmpdata1);
            bmp.UnlockBits(bmpdata);

            //C_Main.WriteLine("ByteToBitmap End");
            return bmp;
        }



        public static List<C_Point3D>? 计算法向量投影小于10MM的点(
            List<C_Point3D> pListPoint, C_Point3D center1, C_Point3D fa)
        {
            if (pListPoint.Count == 0) return null;

            List<C_Point3D> pList = new List<C_Point3D>();
            for (var i = 0; i < pListPoint.Count; i++)
            {
                C_Point3D p = (C_Point3D)pListPoint[i];
                double distance = (p-center1)*(fa);
                if (distance <10)
                {
                    pList.Add(p);
                }
            }
            return pList;
        }



        public static C_Count? 计算法向量投影小于5mm的个数(
            List<C_Point3D> pListPoint, C_Point3D center1, C_Point3D fa)
        {
            if (pListPoint.Count == 0) return null;

            Treap<C_Count> treap = new Treap<C_Count>();

            double wucha = 5;

            for (var i = 0; i < pListPoint.Count; i++)
            {
                C_Point3D p = (C_Point3D)pListPoint[i];
                int distance = (int)Math.Round((p -center1)*(fa)/5);

                double d2 = p.distance(center1);
                if (Math.Abs(d2 - 50) < wucha)
                {
                    //吸盘是半径50mm，误差
                    C_Count p1 = treap.find(new C_K_ID(distance));
                    if (p1 == null)
                    {
                        p1 = new C_Count();
                        p1.key = distance * 5;
                        treap.insert(new C_K_ID(distance), p1);
                    }
                    p1.count += 1;
                }
            }

            TreapEnumerator p2 = treap.Elements(false);

            C_Count? pCount2 =null;
            int max = 0;
            while (p2.HasMoreElements())
            {
                C_Count? pCount = (C_Count?)p2.NextElement();
                if (pCount?.count> max)
                {
                    max = pCount.count;
                    pCount2 = pCount;
                }
            }
            return pCount2;
        }



        public static double 计算法向量投影超过10的个数(
            List<C_Point3D> pListPoint, C_Point3D center1,C_Point3D fa)
        {
            if (pListPoint.Count == 0) return 0.0;

            double sum_distance = 0;
            int Count = 0;
            for(var i=0;i<pListPoint.Count;i++)
            {
                C_Point3D p = (C_Point3D)pListPoint[i];
                double distance = (p-center1)*(fa); 
                if (distance > 10)
                {
                    Count++;
                    sum_distance += distance;
                }
            }
            return  Count;
        }



        public static C_Point3D f_3DPoint_to_depth(
            C_Camera_Calib src_calib,//C_Camera_Const p1,
            C_Point3D pPoint)
        {

            double cam_cx = src_calib.intrinsic.data[2];// 6.52000427e+02;// * 640 / 1280;
            double cam_cy = src_calib.intrinsic.data[5];//5.06252594e+02;// * 480 / 960;
            double cam_fx = src_calib.intrinsic.data[0];//1.09007092e+03;
            double cam_fy = src_calib.intrinsic.data[4];//1.09046729e+03;

            double factor = 1;
            if (cam_fx == 0 || cam_fy == 0)
            {
                MessageBox.Show("照相机内参有错误！");
            }
            //逐点处理，此过程可以使用numpy优化
            double depth = pPoint.z * factor;
            double u = pPoint.x * cam_fx / pPoint.z + cam_cx;
            double v = pPoint.y * cam_fy / pPoint.z + cam_cy;
            return new C_Point3D(u, v, depth);
        }

        public static string ini_read(string file, string section, string key)
        {
            if (File.Exists(file) == false)
            {
                return "";
            }
            IniFile pIni = new IniFile(file);
            string strReturn = pIni.ReadString(section, key, "");
            return strReturn;
        }


        public static void ini_save(string file, string section, string key, string value)
        {
            IniFile pIni = new IniFile(file);

            pIni.WriteString(section, key, value);

        }


        public static string process_html(string filename, string strBody)
        {
            string strBody2 = strBody;

            string pattern = @"<!--include=""(.*?)""-->";

            foreach (Match match in Regex.Matches(strBody, pattern))
            {
                string file2 = match.Groups[1].Value;
                string file_read = Path.GetDirectoryName(filename) + "\\" + file2;
                if (File.Exists(file_read))
                {
                    string strContent = File.ReadAllText(file_read, Encoding.UTF8);
                    strBody2 = strBody2.Replace(match.Value, strContent);
                }
            }

            pattern = @"<!--表格格式化(\r){0,1}\n((.|\n)*?)-->";
            string[] separatingStrings = { "******" };
            foreach (Match match in Regex.Matches(strBody, pattern))
            {
                string strContent = match.Groups[2].Value;
                string[] strSplit = strContent.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
                string strTemplate = strSplit[0];
                string strCode = strSplit[1];
                strSplit = strCode.Split('\n');

                string strResult = "";
                for (var i = 0; i < strSplit.Length; i++)
                {
                    if (strSplit[i] != "")
                    {
                        string strLine = strTemplate;
                        string[] strSplit2 = strSplit[i].Split('|');
                        for (var j = 0; j < strSplit2.Length; j++)
                        {
                            strLine = strLine.Replace("{" + j + "}", strSplit2[j]);
                        }
                        strResult += strLine + "\r\n";
                    }
                }

                strBody2 = strBody2.Replace(match.Value, strResult);
            }

            return strBody2;
        }

        public static double 计算z的平均值(List<C_Point3D> pListPoint)
        {
            double z_sum = 0;
            for (var i = 0; i < pListPoint.Count; i++)
            {
                C_Point3D p=(C_Point3D)pListPoint[i];
                z_sum += p.z;
            }
            return z_sum/pListPoint.Count;
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

        //http://csharphelper.com/blog/2014/07/determine-whether-a-polygon-is-convex-in-c/
        public static bool 判断是否在多边形内(List<C_Point3D> Points, double X, double Y)
        {
            int max_point = Points.Count - 1;
            double total_angle = GetAngle(
                Points[max_point].x, Points[max_point].y,
                X, Y, Points[0].x, Points[0].y);

            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(
                    Points[i].x, Points[i].y,
                    X, Y,
                    Points[i + 1].x, Points[i + 1].y);
            }
            return (Math.Abs(total_angle) > 1);
        }


        //http://csharphelper.com/blog/2014/07/determine-whether-a-polygon-is-convex-in-c/
        public static bool 判断是否在多边形内(Polygon pPolygon, double X, double Y)
        {
            List<PointF> Points = pPolygon.points;

            int max_point = Points.Count - 1;
            double total_angle = GetAngle(
                Points[max_point].X, Points[max_point].Y,
                X, Y, Points[0].X, Points[0].Y);

            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(
                    Points[i].X, Points[i].Y,
                    X, Y,
                    Points[i + 1].X, Points[i + 1].Y);
            }
            return (Math.Abs(total_angle) > 1);
        }
        
    }
}
