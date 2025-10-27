using Common_Robot2;
using ConverxHull;
using pcammls;

namespace Test1
{

    /// <summary>
    /// 读取点云模块
    /// </summary>
    public class Cloud_Get : C_Node
    {
        public string data_out = "";
        public string key_cloud = "";

        public string key_bottom = "";
        public string key_rect = "";

        public string filter_rect = "0";
        public string filter_bottom = "0";
        public string step_y = "5";
        public string step_x = "5";
        public string height_min = "3"; //zzz
        //private double f_height_min = 3;

        public int width = 0;
        public int height = 0;
        public C_Point3D[,]? Image_Point_3D = null;

        public TY_VECT_3F_ARRAY? glb_p3dArray = null;
        public TY_IMAGE_DATA? img = null;
        public uint16_t_ARRAY? registration_depth_data = null;//对齐到彩色图坐标系的深度图
        public string key_camera="";


        public string? file_polygon="";
        public string? key_region = "";
        public string? angle = "";
        public string? row ="";
        public string? dir="";
        public uint8_t_ARRAY? undistort_color_data;
        public string color="0";//=1 保存彩色点云

        public Cloud_Get(string name, C_Space space_parent, C_Space space) : base(name,space_parent, space)
        {
        }

        public override void init()
        {
            //this.f_height_min = float.Parse(this.height_min);
        }

        public override Task run_sub()
        {
            try
            {
                run_sub_main();
            }catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return Task.CompletedTask;
        }

        public void run_sub_main()
        {

            int point_count = 0;
            double? d_max = 0;
            double? d_min = 3000;
            Main.WriteLine(this, "开始读取点云");

            C_Planet? Bottom = (C_Planet?)this.read_var(this.key_bottom, "C_Planet");
            if (Bottom == null && filter_bottom == "1")
            {
                MessageBox.Show(this.Name + ",key_bottom 设置 错误");
            }

            List<C_Point3D> cloud1 = new List<C_Point3D>();
            this.save_var(this.key_cloud, "List<C_Point3D>", cloud1);



            int[,] debug = new int[128, 96];
            List<C_Point3D>? polygon = null;
            if (this.filter_rect == "1" && this.key_rect != "")
            {
                polygon = (List<C_Point3D>?)this.read_var(this.key_rect, "List<C_Point3D>");
            }

            string? str_file_polygon = this.read_string(this.file_polygon);

            if (polygon == null && str_file_polygon != null && str_file_polygon.Length > 0)
            {
                string str_angle = this.read_string(this.angle);
                if (str_angle == "")
                {
                    Main.speak_async(this.Name + " 角度错误");
                    return;
                }
                int i_angle = int.Parse(str_angle);

                string str_row = this.read_string(this.row);
                if (str_row == "")
                {
                    Main.speak_async(this.Name + " 行读取错误");
                    str_row = "0";
                }
                int i_row = int.Parse(str_row);
                {

                    string? strDir = this.read_string(this.dir);

                    str_file_polygon = str_file_polygon.Replace("{fold}", strDir + "");
                    str_file_polygon = str_file_polygon.Replace("{angle}", i_angle + "");
                    str_file_polygon = str_file_polygon.Replace("{row}", i_row + "");

                    if (File.Exists(str_file_polygon))
                    {
                        Main.WriteLine(this, "过滤框：" + str_file_polygon);
                        polygon = Main.cloud_read(this, str_file_polygon, ",");
                        this.save_var(this.key_region, "List<C_Point3D>", polygon);
                    }
                    else
                    {
                        File.WriteAllText(str_file_polygon, "");
                    }
                }
            }

            int distance = 6;
            if ((polygon == null || polygon.Count == 0) && img != null && this.Image_Point_3D != null)
            {
                if (this.step_x == "" || this.step_y == "")
                {
                    MessageBox.Show("step_x,step_y 参数不能为空！");
                    return;
                }
                int step_y = int.Parse(this.step_y);
                int step_x = int.Parse(this.step_x);
                for (var v = distance; v < height - distance; v += step_y)
                {
                    for (var u = distance; u < width - distance; u += step_x)
                    {
                        uint index = (uint)(img.width * v + u);
                        double? d = (double?)this.registration_depth_data?[index];

                        if (d > 0)
                        {
                            C_Point3D pPoint2 = Main_Camera.图像转3D点云(this.glb_p3dArray, img.width, u, v);


                            if (double.IsNaN(pPoint2.x) == false)
                            {
                                this.Image_Point_3D[u, v] = pPoint2;
                            }
                            else
                            {
                                continue;
                            }

                            if (pPoint2.x == 0 && pPoint2.y == 0 & pPoint2.z == 0)
                            {
                                continue;
                            }

                            {
                                point_count++;
                                cloud1.Add(pPoint2);

                                int xx = u / 10;
                                int yy = v / 10;
                                debug[xx, yy] = (int)pPoint2.z / 100;

                                this.Image_Point_3D[u, v] = pPoint2;
                                int? z = ((int)pPoint2.z) * 100;
                            }
                        }
                    }
                }

            }
            else if (img != null && polygon != null && polygon.Count > 2)
            {

                int step_y = int.Parse(this.step_y);
                int step_x = int.Parse(this.step_x);
                for (var v = distance; v < 960 - distance; v += step_y)
                {
                    for (var u = distance; u < 1280 - distance; u += step_x)
                    {

                        if (Main.判断是否在多边形内(polygon, u, v))
                        {
                            uint index =(uint)(img.width * v + u);
                            double? d = (double?)this.registration_depth_data?[index];

                            if (d > 0)
                            {
                                if (d > d_max) d_max = d;
                                if (d < d_min) d_min = d;

                                C_Point3D pPoint2 = Main_Camera.图像转3D点云(this.glb_p3dArray, img.width, u, v);

                                this.Image_Point_3D[u, v] = pPoint2;

                                if (double.IsNaN(pPoint2.x))
                                {
                                    continue;
                                }

                                if (pPoint2.x == 0 && pPoint2.y == 0 & pPoint2.z == 0)
                                {
                                    continue;
                                }

                                {
                                    point_count++;
                                    cloud1.Add(pPoint2);
                                }
                            }

                        }
                    }
                }

                Main.WriteLine(this, ",3D点个数=" + cloud1.Count);
            }

            if (this.color == "1")
            {
                MySave_Point3D_Color(cloud1, glb_p3dArray, undistort_color_data, width, height);
            }
        }



        //保存点云的颜色信息
        public void MySave_Point3D_Color(
            List<C_Point3D> cloud1,
            TY_VECT_3F_ARRAY p3dArray,
            uint8_t_ARRAY undistort_color_data,
            int width, int height)
        {

            Main.WriteLine("保存点云的颜色信息");

            if (this.Image_Point_3D == null) return;

            for (int v = 0; v < height; v += 1) 
            {
                for (int u = 0; u < width; u += 1)
                {
                    uint offset = (uint)(width * v + u);
                    uint offset2 = offset * 3;

                    C_Point3D pPoint2 = this.Image_Point_3D[u, v];
                    if (pPoint2 != null)
                    {
                        pPoint2.x = p3dArray.getitem(offset).x;
                        pPoint2.y = p3dArray.getitem(offset).y;
                        pPoint2.z = p3dArray.getitem(offset).z;
                        cloud1.Add(pPoint2);

                        byte b = undistort_color_data[offset2];
                        byte g = undistort_color_data[offset2 + 1];
                        byte r = undistort_color_data[offset2 + 2];

                        pPoint2.pExtend = new C_Color(r, g, b);
                    }
                }
            }
            //this.save_var("#color_cloud", "List<C_Point3D>", pList_Point3D_Color);

            Main.WriteLine("彩色点云 保存结束");
        }

    }
}