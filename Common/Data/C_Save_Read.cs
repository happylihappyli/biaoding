using Common_Robot2;
using ConverxHull;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Funny;
using System.Collections.Generic;

namespace Test1
{
    public class C_Save_Read
    {
        C_Space space;
        public C_Save_Read(C_Space space)
        {
            this.space = space;
        }

        public IniFile myIni = new IniFile(Application.StartupPath + "\\Config.ini");

        public bool read_check(CheckBox? pCheckbox, string name)
        {
            bool bReturn = (myIni.ReadString("Robot1", name, "0") == "1");
            if (pCheckbox!=null)
                pCheckbox.Checked = bReturn;
            return bReturn;
        }


        public double read_double(TextBox? pText, string name, string strDefault)
        {

            double mValue = double.Parse(myIni.ReadString("Robot1", name, strDefault));
            if (pText!=null)
                pText.Text = mValue + "";
            return mValue;

        }
        public double read_double2(TextBox? pText, string section, string name, string strDefault)
        {

            double mValue = double.Parse(myIni.ReadString(section, name, strDefault));
            if (pText != null)
                pText.Text = mValue + "";
            return mValue;

        }
        

        public string read_string(TextBox? pText, string name, string strDefault)
        {

            string mValue = myIni.ReadString("Robot1", name, strDefault);
            if (pText!=null) pText.Text = mValue + "";
            return mValue;

        }


        public double save_double2(TextBox pText,
            string section,string name,double value=0)
        {
            if (pText != null)
            {
                value = double.Parse(pText.Text);
            }
            myIni.WriteString(section, name, value+"");
            return value;
        }
        public double save_double(TextBox pText, string name)
        {
            return save_double2(pText, "Robot1", name);
        }

        public string save_string(TextBox pText, string name,string value="")
        {

            if (pText != null)
            {
                value = pText.Text;
            }
            myIni.WriteString("Robot1", name, value);
            //string mValue = pText.Text;
            return value;
        }
        public bool save_check(CheckBox pCheckbox, string name)
        {
            bool bReturn = pCheckbox.Checked;
            myIni.WriteString("Robot1", name, pCheckbox.Checked ? "1" : "0");
            return bReturn;
        }




        public void 读取配置(string strFile)
        {
            myIni = new IniFile(strFile);

            {
                //space.vars.default_position = read_string(null, "tx_arm_default", "-58289,7943,-3950,-1075,-20895,0"); ;

                //this.upload_user = read_string(null, "tx_upload_user", "test");
                //this.upload_password = read_string(null, "upload_password", "");

                //this.upload_class_id = read_string(null, "tx_upload_class_id", "0");
                //this.upload_class_id2 = read_string(null, "tx_upload_class_id2", "0");


                //C_Vars.log_path = read_string(null, "tx_log_path", "D:\\");

                int count = (int)this.read_double2(null, "position", "count", "0");
                for (var i = 0; i < count; i++)
                {
                    string name = this.read_string(null, "position_name_" + i, "default");
                    //pFrmSet.cb_position.Items.Add(name);
                }

                int index = (int)this.read_double2(null, "position", "index", "-1");// cb_position.SelectedIndex);
                if (index > -1)
                {
                    //pFrmSet.cb_position.SelectedIndex = index;
                }

                //space.vars.ball_radius = read_double(null, "tx_radius", "150");//碰撞检测半径

                //space.vars.z_min = read_double(null, "tx_z_min", "-3000");
                //space.vars.z_max = read_double(null, "tx_z_max", "3000");

                //space.vars.max_area = read_double(null, "tx_max_area", "10");
                //space.vars.wait_next_loop = (int)read_double(null, "tx_plc_wait_time", "3000");

                //space.vars.angle_dif = (int)read_double(null, "tx_angle_dif", "55");
                //space.vars.match_percent = read_double(null, "tx_match_count", "0.7");
                //space.vars.db_rotate_hk = read_double(null, "tx_rotate_hk", "180");

                space.vars.draw_offset_x_tuyang = read_double(null, "tx_x_offset_tuyang", "20");
                space.vars.draw_offset_y_tuyang = read_double(null, "tx_y_offset_tuyang", "3");


                //space.vars.z_add = (int)read_double(null, "tx_z_add", "10");
                //space.vars.position_dif = (int)read_double(null, "tx_position_dif", "20");

                //space.vars.b_upload_file = read_check(null, "ck_upload_file");
                space.vars.bDebug_Mode = read_check(null, "ck_debug_mode");


                //space.vars.b_simplify = read_check(null, "ck_simplify");

                read_string(null, "tx_Wait", "3000");

                //space.vars.Program = read_string(null, "cb_program", "WORK");

                //space.vars.time_wait = (int)read_double(null, "tx_time_wait", "400");
                //space.vars.wait = (int)read_double(null, "tx_Wait", "400");
                //space.vars.time_wait_sum = space.vars.time_wait + space.vars.wait;


                //space.vars.iFilter_z = read_double(null,"tx_z_filter", "3");

                space.vars.step_x = (int)read_double(null, "tx_3d_x", "5");
                space.vars.step_y = (int)read_double(null, "tx_3d_y", "5");


                //space.vars.server_upload = read_string(null, "tx_upload_server", "http://www.sndgf.com.cn:14380/upload/upload_file.php");
                

                //space.vars.max_ry = read_double(null, "tx_max_ry", "30");

                //space.vars.min_width = (int)read_double(null, "tx_min_width", "100");
                //space.vars.min_height = (int)read_double(null, "tx_min_height", "100");

                //space.vars.bStart_Web = read_check(null, "ck_start_web");
                //space.vars.start_calibration = read_check(null, "ck_calibration");



                if (myIni.ReadString("rect", "x0", "0") != "0")
                {
                    var x1 = int.Parse(myIni.ReadString("rect", "x0", "0"));
                    var y1 = int.Parse(myIni.ReadString("rect", "y0", "0"));
                    var x2 = int.Parse(myIni.ReadString("rect", "x1", "100"));
                    var y2 = int.Parse(myIni.ReadString("rect", "y1", "100"));
                }

                if (myIni.ReadString("rect2", "x0", "0") != "0")
                {
                    var x1 = int.Parse(myIni.ReadString("rect2", "x0", "0"));
                    var y1 = int.Parse(myIni.ReadString("rect2", "y0", "0"));
                    var x2 = int.Parse(myIni.ReadString("rect2", "x1", "100"));
                    var y2 = int.Parse(myIni.ReadString("rect2", "y1", "100"));
                }

                //space.vars.tool_z = read_double2(null, "offset", "tx_tool_z", "-81.2");
                //space.vars.catch_z = read_double2(null, "offset", "tx_catch_z", "-266");

                //space.vars.b_send_to_plc = read_check(null, "ck_send_to_plc");


                //space.vars.similar_avg_dif_avg = read_double(null, "tx_similar_avg_dif_avg", "5");
                //space.vars.similar_avg = read_double(null, "tx_similar_avg", "5");


            }
        }

        public void 输出点云或彩色点云数据(List<C_Point3D> pList3)
        {
            Console.WriteLine("输出点云数据");
            string strFile_write2 =  @"D:\points_web.xyz.txt"; //space.vars.path_web +
            FileStream pFile2 = new FileStream(strFile_write2, FileMode.Create, FileAccess.Write);
            TextWriter pWriter2 = new StreamWriter(pFile2, Encoding.UTF8);
            for (int i = 0; i < pList3.Count; i++)
            {
                C_Point3D pPoint = pList3[i];

                string strLine;
                if (pPoint.pExtend != null)
                {
                    C_Color pColor = pPoint.pExtend;
                    strLine = pPoint.ToString() + "," + pColor.ToString();
                }
                else
                {
                    strLine = pPoint.ToString();
                }

                pWriter2.WriteLine(strLine);
            }
            pWriter2.Close();
            pFile2.Close();
        }

        public void 输出三角面数据(List<C_Planet> pAll_Small_Surface)
        {
            Console.WriteLine("输出三角面数据");  //输出三角面数据 //三个点构成一个平面
            string file_surface = @"d:\surface_normal.txt";//space.vars.path_web + 
            FileStream fs_surface = new FileStream(file_surface, FileMode.Create, FileAccess.Write);
            TextWriter w_surface = new StreamWriter(fs_surface, Encoding.UTF8);

            for (int i = 0; i < pAll_Small_Surface.Count; i++)
            {
                C_Planet pPlanet = (C_Planet)pAll_Small_Surface[i];
                w_surface.WriteLine(pPlanet.x0y0.ToString() + "," + pPlanet.Group_ID);
                w_surface.WriteLine(pPlanet.x0y1.ToString() + "," + pPlanet.Group_ID);
                w_surface.WriteLine(pPlanet.x1y0.ToString() + "," + pPlanet.Group_ID);
                //
                w_surface.WriteLine(pPlanet.x0y1.ToString() + "," + pPlanet.Group_ID);
                w_surface.WriteLine(pPlanet.x1y0.ToString() + "," + pPlanet.Group_ID);
                w_surface.WriteLine(pPlanet.x1y1.ToString() + "," + pPlanet.Group_ID);
            }
            w_surface.Close();
            fs_surface.Close();

        }


        public ArrayList? 读取障碍物()
        {
            //过滤底部信息需要先读取底部的标定信息
            string file_vector = @"d:\points_objs.txt"; //space.vars.path_web + 
            if (File.Exists(file_vector)==false)
            {
                return null;
            }
            FileStream fs_debug = new FileStream(file_vector, FileMode.Open, FileAccess.Read);

            ArrayList pListBottom = new ArrayList();
            StreamReader sr = new StreamReader(fs_debug, Encoding.UTF8);
            string? line = sr.ReadLine();
            while (line != null)
            {
                string[] strSplit = line.Split(',');
                if (strSplit.Length > 2)
                {
                    double x = double.Parse(strSplit[0]);
                    double y = double.Parse(strSplit[1]);
                    double z = double.Parse(strSplit[2]);
                    C_Point3D pPoint = new C_Point3D(x, y, z);
                    pListBottom.Add(pPoint);
                }
                line = sr?.ReadLine();
            }
            sr?.Close();
            fs_debug.Close();


            return pListBottom;
        }



        //public C_Planet 读取坐标纠正信息()
        //{
        //    //过滤底部信息需要先读取底部的标定信息
        //    string file_vector = @"d:\xyz_modify.txt";//space.vars.path_web + 
        //    if (File.Exists(file_vector) == false)
        //    {
        //        space.console.WriteLine("读取坐标纠正信息==null!!!!!");
        //        return null;
        //    }
        //    FileStream fs_debug = new FileStream(file_vector, FileMode.Open, FileAccess.Read);

        //    List<C_Point3D> pListBottom = new List<C_Point3D>();
        //    StreamReader sr = new StreamReader(fs_debug, Encoding.UTF8);
        //    string line = sr.ReadLine();
        //    ArrayList pList;
        //    while (line != null)
        //    {
        //        string[] strSplit = line.Split(',');
        //        if (strSplit.Length > 5)
        //        {
        //            double x = double.Parse(strSplit[0]);
        //            double y = double.Parse(strSplit[1]);
        //            double z = double.Parse(strSplit[2]);
        //            double dx = double.Parse(strSplit[3]);
        //            double dy = double.Parse(strSplit[4]);
        //            double dz = double.Parse(strSplit[5]);
        //            C_Point3D pPoint1 = new C_Point3D(x, y, z);

        //            C_Point3D pPoint2 = new C_Point3D(dx, dy, dz);

        //            int mx =(int)Math.Round( x / 100);
        //            int my = (int)Math.Round(y / 100);
        //            int mz = (int)Math.Round(y / 100);
        //            string key = mx + "," + my + "," + mz;
        //            if (space.vars.xyz_modify.TryGetValue(key, out pList) == false)
        //            {
        //                pList=new ArrayList(); 
        //                space.vars.xyz_modify.Add(key, pList);
        //            }
        //            pList.Add(new Two_Point3D(pPoint1, pPoint2));

        //        }
        //        line = sr.ReadLine();
        //    }
        //    sr.Close();
        //    fs_debug.Close();


        //    C_Point3D fa = Tools.平面拟合计算法向量等(pListBottom, false);

        //    //double a = m[0, 0];
        //    //double b = m[1, 0];
        //    //double c = m[2, 0];
        //    //double c_z = -1;
        //    //double len = Math.Sqrt(a * a + b * b + c_z * c_z);
        //    //double a2 = a / len;
        //    //double b2 = b / len;
        //    //double c_z2 = -1 / len;

        //    C_Planet pPlanet = new C_Planet(-1);
        //    pPlanet.z_faxiangliang = fa;//_Point3D(a2, b2, c_z2);
        //    pPlanet.center = Tools.计算中心点坐标(pListBottom);

        //    return pPlanet;
        //}



        public C_Planet? 读取底面信息()
        {
            //过滤底部信息需要先读取底部的标定信息
            string file_vector = @"d:\bottom.txt";//space.vars.path_web + 
            if (File.Exists(file_vector) == false)
            {
                Console.WriteLine("底面信息==null!!!!!");
                return null;
            }
            FileStream fs_debug = new FileStream(file_vector, FileMode.Open, FileAccess.Read);

            List<C_Point3D> pListBottom = new List<C_Point3D>();
            StreamReader sr = new StreamReader(fs_debug, Encoding.UTF8);
            string? line = sr.ReadLine();
            while (line != null)
            {
                if (line.StartsWith("camera="))
                {
                    line = line.Substring(7);
                }
                string[] strSplit = line.Split(',');
                if (strSplit.Length > 2)
                {
                    double x = double.Parse(strSplit[0]);
                    double y = double.Parse(strSplit[1]);
                    double z = double.Parse(strSplit[2]);
                    C_Point3D pPoint = new C_Point3D(x, y, z);
                    pListBottom.Add(pPoint);
                }
                line = sr.ReadLine();
            }
            sr.Close();
            fs_debug.Close();


            C_Point3D fa = Main.平面拟合计算法向量等(pListBottom, false);

            //double a = m[0, 0];
            //double b = m[1, 0];
            //double c = m[2, 0];
            //double c_z = -1;
            //double len = Math.Sqrt(a * a + b * b + c_z * c_z);
            //double a2 = a / len;
            //double b2 = b / len;
            //double c_z2 = -1 / len;

            C_Planet pPlanet = new C_Planet(-1);
            pPlanet.z_faxiangliang = fa;//_Point3D(a2, b2, c_z2);
            pPlanet.center = Tools.计算中心点坐标(pListBottom);

            return pPlanet;
        }

        public void 输出法向量数据(List<C_Planet> pSurface)
        {
            Console.WriteLine("输出法向量数据");

            string file_vector =  @"d:\vector_faxiangliang.txt";//space.vars.path_web +
            FileStream fs_vector = new FileStream(file_vector, FileMode.Create, FileAccess.Write);
            StreamWriter w_vector = new StreamWriter(fs_vector, Encoding.UTF8);
            for (int i = 0; i < pSurface.Count; i++)
            {
                C_Planet pPlanet = (C_Planet)pSurface[i];
                w_vector.WriteLine(pPlanet.center.ToString() + "," + pPlanet.z_faxiangliang.ToString() + "," + pPlanet.Group_ID + "," + pPlanet.x_index + "_" + pPlanet.y_index + "_" + pPlanet.z_index);
            }
            w_vector.Close();
            fs_vector.Close();
        }

    }
}
