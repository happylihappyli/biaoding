using Common_Robot2;
using ConverxHull;
using System.Collections;

using pcammls;
using SDK = pcammls.pcammls;
using SDK_ISP = pcammls_isp.pcammls_isp_api;//TYISP.cs
using System.Runtime.InteropServices;
using System.Drawing.Imaging;


namespace Test1
{
    /// <summary>
    /// 拍照步骤，按模块化思路来做
    /// </summary>
    public class S_Camera : C_Node, I_Camera
    {
        public static bool bInit = false;

        public IntPtr dev_handle;

        public string control_2d = "";
        public string control_3d = "";
        public string key_save = "2d_pic";
        
        private string _key_camera = "";//C_Camera_Const
        public string key_camera { get => _key_camera; set => _key_camera = value; }

        public string key_rgb = "";

        public C_DrawBox box1 = new C_DrawBox();//摄像头的机械臂抓取区域

        public int camera_w = 1280;
        public int camera_h = 960;

        public C_Camera_TuYang? camera_const = null;
        public Bitmap pBitmap_Depth;
        public List<C_Point3D> pList_Point3D_Color = new List<C_Point3D>();

        public bool bCatching;

        public string camera_format = "";


        public string wait = "0";
        private string _camera_id = "";//照相机ID
        public string camera_id { get => _camera_id; set => _camera_id = value; }

        public TimeSpan old;

        public int img_index = 0;
        public IntPtr handle;
        public TY_FRAME_DATA? frame = null;
        public IntPtr color_isp_handle;

        public TY_VECT_3F_ARRAY? glb_p3dArray = null;
        public TY_VECT_3F_ARRAY? p3dArray=null;


        public int Pic_Count = -1;
        public bool Camera_Receive_Read = false;
        public int Pic_Index = 1;
        public List<Data_RGB> RGB_Array = new List<Data_RGB>();
        public ArrayList Depth_Array = new ArrayList();


        public bool recvFlag = false;
        public uint8_t_ARRAY[] buffer = new uint8_t_ARRAY[4];
        public Cloud_Get? 读取3D点云模块;
        public Img_Get? 读取2D图片模块;

        public uint8_t_ARRAY? undistort_color_data;//畸变矫正后的彩色图
        public uint16_t_ARRAY? registration_depth_data;//对齐到彩色图坐标系的深度图
        public uint8_t_ARRAY? color_data;//彩色原图
        public uint8_t_ARRAY? color_data2;//彩色原图2
        public int color_width;
        public int color_height;

        public TY_IMAGE_DATA src = new TY_IMAGE_DATA();
        public TY_IMAGE_DATA dst = new TY_IMAGE_DATA();

        public C_Point3D[,]? Image_Point_3D = null;
        private uint16_t_ARRAY depth_result;
        public Data_RGB? pLast = null;

        private string _key_2d_step = "";
        private string _key_3d_step = "";

        public string key_3d = "#3F_ARRAY";
        public string key_camera_time = "";
        public string save_color_cloud = "0";
        public string key_compare = "";
        public string depth_camera = "0";

        public string time_min = "2000";
        public double d_time_min = 2000;
        public string key_ids="";

        public bool Stop { get; set; }
        public string key_2d_step { get => _key_2d_step; set => _key_2d_step = value; }
        public string key_3d_step { get => _key_3d_step; set => _key_3d_step = value; }

        public S_Camera(string name, C_Space space_parent, C_Space space) : base(name,space_parent, space)
        {
            pBitmap_Depth = new Bitmap(camera_w, camera_h);
        }

        public class ParametersMain
        {
            public IntPtr mHandle;
            public int color_size;
            public IntPtr color_isp_handle;
            public ParametersMain(IntPtr ihandle, int colorSize, IntPtr colorIspHandle)
            {
                this.mHandle = ihandle;
                this.color_size = colorSize;
                this.color_isp_handle = colorIspHandle;
            }
        }

        public override void init()
        {
            if (space.vars_step.ContainsKey(space.Name + this.alias) == false)
            {
                space.vars_step.TryAdd(space.Name + this.alias, this);
            }
            Img_Get? m2d = (Img_Get?)space.vars_step[space.Name+ this.control_2d];
            Cloud_Get? m3d = (Cloud_Get?)space.vars_step[space.Name + this.control_3d];
            this.set_2d(m2d);
            this.set_3d(m3d);

            this.d_time_min = double.Parse(this.time_min);
        }


        public void set_2d(Img_Get? 读取2D图片模块)
        {
            this.读取2D图片模块 = 读取2D图片模块;
        }

        public void set_3d(Cloud_Get? 读取3D点云模块)
        {
            this.读取3D点云模块 = 读取3D点云模块;
        }


        public void 深度图处理2(
            Data_RGB? pLast,
            I_Train? pTrain,
            C_Camera_TuYang? p1,
            uint16_t_ARRAY? depth_pixel_arr,
            TY_VECT_3F_ARRAY? p3dArray,
            TY_IMAGE_DATA img,
            int color_width, int color_height,
            int depth_width, int depth_height)
        {

            int n = color_width * color_height * 3;
            try
            {
                Main.WriteLine("读取点云数据！");


                uint u = (uint)img.width / 2;
                uint v = (uint)img.height / 2;
                uint offset = (uint)img.width * v + u;
                ushort? d_tmp = depth_pixel_arr?[offset];

                {
                    this.Image_Point_3D = new C_Point3D[this.camera_w, this.camera_h];
                }

                this.pList_Point3D_Color = new List<C_Point3D>();


                if (this.depth_camera == "1")// && space.vars.bUndistort == false)
                {
                    Main.WriteLine("设置深度图片开始（没有对齐的深度图）");
                    try
                    {
                        if (img.width > 0 && img.height > 0)
                        {

                            this.pBitmap_Depth = new Bitmap(img.width, img.height);
                        }
                        else
                        {
                            Main.WriteLine("pic error img width=0 !");
                        }
                    }
                    catch (Exception ex)
                    {
                        Main.WriteLine(this,ex.ToString());
                    }
                    Main.WriteLine("设置深度图片结束");
                }
                Main.WriteLine("图片抓取成功! ");

                Main.WriteLine(this,this.Name+" 将深度图对齐到彩色图坐标系 Start! ");
                SDK.TYMapDepthImageToColorCoordinate(p1?.depth_calib,
                    (uint)depth_width, (uint)depth_height,
                    depth_pixel_arr?.cast(), p1?.color_calib,
                    (uint)color_width, (uint)color_height,
                    this.registration_depth_data.cast());

                Main.WriteLine(this,this.Name + "深度图转3D点云图! ");

                SDK.TYMapDepthImageToPoint3d(p1?.color_calib, img.width, img.height, this.registration_depth_data.cast(), p3dArray?.cast(), 1);
                depth_result = registration_depth_data;
                this.glb_p3dArray = p3dArray;
                this.save_var(this.key_3d, "TY_VECT_3F_ARRAY", p3dArray);
                读取点云(img, pTrain);


                //if (this.save_color_cloud == "1")// space.vars.read_undistort_rgb)
                //{
                //    MySave_Point3D_Color(p3dArray, this.undistort_color_data, color_width, color_height);
                //}
                this.bCatching = false;
                //
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                Main.WriteLine(this,e.ToString());
            }
        }

        public void SaveBMP(
            I_Train pTrain,
            uint8_t_ARRAY color_pixel_arr,
            string key_rgb,
            int width, int height)
        {

            Bitmap pBitmapRGB = ByteToBitmap(color_pixel_arr, width, height);

            Bitmap Bmp2 = new Bitmap(pBitmapRGB);
            if (key_rgb != "")
            {
                Bitmap bmp3 = Main.CopyBmp(Bmp2);
                this.save_var(key_rgb, "Bitmap", bmp3);
            }

            this.save_var(this.key_compare, "Bitmap", Main.CopyBmp(Bmp2));
        }

        public override Task run_sub()
        {
            if (this.bStart == false)
            {
                MessageBox.Show(this.Name + " 相机要初始化后才能使用！相机初始化请调用 S_Camera_Init 模块！");
            }
            else
            {
                run_sub_main();
            }
            return Task.CompletedTask;
        }

        public void run_sub_main()
        {

            Main.WriteLine(this,"---------------------------开始拍照@"+this.Name);
            if (this.key_2d_step != "")
            {
                string? control_name = (string?)this.read_var(this.key_2d_step, "string");
                Img_Get? m2d = (Img_Get?)space.vars_step[space.Name + control_name];
                this.set_2d(m2d);
            }

            if (this.key_3d_step != "")
            {
                string? control_name = (string?)this.read_var(this.key_3d_step, "string");
                Cloud_Get? m3d = (Cloud_Get?)space.vars_step[space.Name + control_name];
                this.set_3d(m3d);
            }


            if (this.key_rgb != "")
            {
                this.key_save = this.key_rgb;
            }
            TimeSpan pTimeNew = new TimeSpan(DateTime.Now.Ticks);

            if (old != null)
            {
                double Milliseconds = pTimeNew.Subtract(old).TotalMilliseconds;
                if (Milliseconds < this.d_time_min)
                {
                    Main.WriteLine(this,"拍照速度太快！");
                    Main.speak_async("拍照速度太快");
                    return;
                }
            }
            old = pTimeNew;

            Pic_Index++;

            Main.WriteLine(this, "开始拍照！");
            this.bCatching = true;

            new Task(() =>
            {
                Console.Beep(3000, 200);
            }).Start();


        }


        //保存点云的颜色信息
        //public void MySave_Point3D_Color(
        //    TY_VECT_3F_ARRAY p3dArray,
        //    uint8_t_ARRAY undistort_color_data,
        //    int width, int height)
        //{
        //    Main.WriteLine("MySave_Point3D_Color Start");

        //    if (this.Image_Point_3D == null) return;
            
        //    {
        //        for (int v = 0; v < height; v += 1) // C_Main.step_x)
        //        {
        //            for (int u = 0; u < width; u += 1)// C_Main.step_y)
        //            {
        //                int offset = width * v + u;
        //                int offset2 = offset * 3;

        //                C_Point3D pPoint2 = this.Image_Point_3D[u, v];
        //                if (pPoint2 != null)
        //                {
        //                    pPoint2.x = p3dArray.getitem(offset).x;
        //                    pPoint2.y = p3dArray.getitem(offset).y;
        //                    pPoint2.z = p3dArray.getitem(offset).z;
        //                    this.pList_Point3D_Color.Add(pPoint2);

        //                    byte b = undistort_color_data[offset2];
        //                    byte g = undistort_color_data[offset2 + 1];
        //                    byte r = undistort_color_data[offset2 + 2];

        //                    pPoint2.pExtend = new C_Color(r, g, b);
        //                }
        //            }
        //        }
        //        this.save_var("#color_cloud", "List<C_Point3D>", pList_Point3D_Color);
        //    }

        //    Main.WriteLine("MySave_Point3D_Color End");
        //}


        public void camear_init()
        {
            if (S_Camera.bInit == false)
            {
                bInit = true;
                
                SDK.TYInitLib(); ////初始化 获取相机lib版本
                TY_VERSION_INFO info = new TY_VERSION_INFO();
                SDK.TYLibVersion(info);
                Main.WriteLine(this, string.Format("版本 :{0} {1} {2}", info.major, info.minor, info.patch));
            }

        }

        public void Start_Camera(C_Camera_TuYang camera2)
        {
            Main.speak_async("启动摄像头");
            if (this.wait == "")
            {
                Main.speak_async("相机的wait没有设置");
                MessageBox.Show("相机的wait没有设置！");
                return;
            }
            Thread.Sleep(int.Parse(this.wait));

            camear_init();

            uint8_t_ARRAY[] buffer = new uint8_t_ARRAY[2];

            DeviceInfoVector devs = new DeviceInfoVector();
            try
            {
                Main.WriteLine(this," 摄像头 ID=" + this.camera_id);
                string? camera_id = this.read_string(this.camera_id);

                Main.WriteLine(this, " 摄像头 ID=" + camera_id);//这里有错误，就重启程序
                SDK.selectDevice(SDK.TY_INTERFACE_ALL, camera_id, "", 10, devs);
                int sz = devs.Count();

                List<string> list_id = new List<string>();
                string strCode = "";
                for (var i = 0; i < sz; i++)
                {
                    var item2 = devs[i];
                    list_id.Add(item2.id);
                    strCode += item2.id + "\r\n";
                    Main.WriteLine(this, "摄像头 ID=" + item2.id);
                }
                this.save_var(this.key_ids, "List<string>", list_id);

                File.WriteAllText(C_Vars.log_path + "\\摄像头ID_"+this.camera_id+".txt", strCode);

                if (devs.Count == 0)
                {
                    Main.speak_async("没找到摄像头");
                    Main.WriteLine(this, "error 没找到摄像头!");
                    return;
                }
                var item = devs[0];//选相机0
                var dev_info = item;

                dev_handle = new IntPtr();
                IntPtr iface_handle = new IntPtr();
                
                int a = SDK.TYOpenInterface(dev_info.iface.id, ref iface_handle);//打开接口
                Main.WriteLine(this,a + "");
                
                IntPtr errCode = IntPtr.Zero;
                SDK.TYOpenDevice(iface_handle, dev_info.id, ref dev_handle, ref errCode);//打开设备
                
                handle = dev_handle;

                Thread thread = new Thread(Loop_RGB_Process);//循环获取帧
                thread.IsBackground = true;
                thread.Start();

                TuYang_Init(camera_id, dev_handle, camera2);




                Task.Run(() =>
                {
                    Loop_TuYang_Camera(camera_id, dev_handle, camera2);

                    SDK.TYStopCapture(dev_handle);//停止采集
                    SDK.TYCloseDevice(dev_handle, false);//关闭设备与接口
                    SDK.TYCloseInterface(iface_handle);
                });
            }
            catch (Exception ex)
            {
                Main.WriteLine(this, ex.ToString());
            }
        }

        public void Loop_RGB_Process()
        {
            while (this.Stop == false)
            {
                if (space.vars.bClosingWindows) this.Stop = true;

                if (RGB_Array.Count == 0)
                {
                    Thread.Sleep(10);
                    continue;
                }
                Data_RGB? pData_RGB;
                lock (RGB_Array)
                {
                    if (RGB_Array.Count == 0) continue;
                    pData_RGB = RGB_Array[0];
                    RGB_Array.RemoveAt(0);
                }
                //RGB图处理(p1.pixel_arr, p1.img, p1.img.width, p1.img.height);

                this.读取2D图片模块.pLast2 = pData_RGB;
                this.读取2D图片模块.pixel_arr = pData_RGB.pixel_arr;
                this.读取2D图片模块.img = pData_RGB.img;
                this.读取2D图片模块.color_data = color_data;
                this.读取2D图片模块.pCamera = this;
                pData_RGB.pixel_arr = null;
                pData_RGB.img = null;



                Thread thread = new Thread(() =>
                {
                    this.读取2D图片模块.Run(pTrain);
                });
                thread.Start();

            }
        }

        public bool bStart = false;
        public void start()
        {
            bStart = true;
            Thread.Sleep(1000);
            {
                //Task.Run(() =>
                {
                    C_Camera_TuYang? camera2 = new C_Camera_TuYang();
                    lock (space.vars_new)
                    {
                        camera2 = (C_Camera_TuYang?)this.read_var(_key_camera, "C_Camera_TuYang");
                        if (camera2 == null)
                        {
                            camera2 = new C_Camera_TuYang();
                            this.save_var(_key_camera, "C_Camera_TuYang", camera2);
                        }
                        this.camera_const = camera2;
                    }
                    Start_Camera(camera2);
                }
                //);
            }
        }



        private void Camera_Receive(Object obj, C_Camera_TuYang const1)
        {
            ParametersMain psm = (ParametersMain)obj;

            TY_FRAME_DATA frame = new TY_FRAME_DATA();

            while (recvFlag)
            {
                Camera_Receive_Read = true;

                if (space.vars.bClosingWindows)
                {
                    break;
                }

                if (Pic_Count == -1)
                {
                    Thread.Sleep(10);
                    continue;
                }

                if (pTrain == null)
                {
                    Thread.Sleep(10);
                    continue;
                }

                try
                {
                    Main.WriteLine(this,"0-相机接收到图片数据 Pic_Count=" + Pic_Count);
                    

                    int ret = SDK.TYFetchFrame(psm.mHandle, frame, 2000);
                    if (ret < 0)
                    {
                        Main.speak_async("图像读取错误！ 返回值=" + ret);
                        Main.WriteLine(this, "图像读取错误！ 返回值=" + ret);
                        Main.WriteLine(this, "图像读取错误！ 返回值=" + ret);
                        continue;
                    }

                    Pic_Count++;
                    Main.WriteLine(this, "1-Camera_Receive Pic_Count=" + Pic_Count);

                    var images = frame.image;
                    for (uint idx = 0; idx < frame.validCount; idx++)
                    {
                        var img = images[idx];
                        if (img.componentID == SDK.TY_COMPONENT_DEPTH_CAM)
                        {
                            if (Pic_Count != 2) continue;
                            var pixel_arr_depth = uint16_t_ARRAY.FromVoidPtr(img.buffer, img.size / 2);

                            uint offset = (uint)(img.width * img.height / 2 + img.width / 2);
                            ushort distance = pixel_arr_depth[offset];

                            //this.save_var("#img_depth", "TY_IMAGE_DATA", img);

                            Thread_Depth_Img pImg = new Thread_Depth_Img(pTrain, const1, this, Pic_Count, pixel_arr_depth, img);
                            pImg.run();
                            Pic_Count = -1;

                        }
                        else if (img.componentID == SDK.TY_COMPONENT_RGB_CAM)
                        {
                            if (Pic_Count != 1) continue;
                            var pixel_arr = uint8_t_ARRAY.FromVoidPtr(img.buffer, img.size);
                            if (img.pixelFormat == SDK.TY_PIXEL_FORMAT_YVYU)
                            {
                                //SDK_ISP.ConvertYVYU2RGB(pixel_arr, color_data, img.width, img.height);

                                //Main.WriteLine(this,string.Format("Color Image Center Pixel value(YVYU):{0} {1} {2}", r, g, b));
                            }
                            else if (img.pixelFormat == SDK.TY_PIXEL_FORMAT_YUYV)
                            {
                                SDK_ISP.ConvertYUYV2RGB(pixel_arr, this.color_data, img.width, img.height);

                                //SWIGTYPE_p_void pointer = (SWIGTYPE_p_void)this.color_data.VoidPtr();
                                int offset = 3 * (img.width * img.height / 2 + img.width / 2);


                                this.src.width = this.color_width;
                                this.src.height = this.color_height;
                                this.src.size = 3 * this.color_width * this.color_height;
                                this.src.pixelFormat = (int)SDK.TY_PIXEL_FORMAT_RGB;
                                this.src.buffer = this.color_data.VoidPtr();

                                //this.MySaveBMP(pTrain, color_data, "test1", img.width, img.height);//, "d:/test1.png");

                                this.dst.width = img.width;
                                this.dst.height = img.height;
                                this.dst.size = 3 * img.width * img.height;
                                this.dst.pixelFormat = (int)SDK.TY_PIXEL_FORMAT_RGB;
                                this.dst.buffer = undistort_color_data.VoidPtr();

                                C_Camera_TuYang? camera1 = (C_Camera_TuYang?)this.read_var(this._key_camera, "C_Camera_TuYang");
                                if (camera1 == null)
                                {
                                    MessageBox.Show(this.Name + " key_camera 设置有问题！");
                                    return;
                                }
                                
                                Main.WriteLine("彩色图畸变矫正 Start! ");
                                SDK.TYUndistortImage(camera1.color_calib, this.src, null, this.dst);
                                
                                Main.WriteLine("彩色图畸变矫正 End! ");

                                //if (Pic_Count == 1)
                                {
                                    Data_RGB pData = new Data_RGB(Pic_Count, this.undistort_color_data, img);// this.color_data, img);
                                    RGB_Array.Add(pData);
                                }
                                //Main.WriteLine(this,string.Format("Color Image Center Pixel value(YUYV):{0} {1} {2}", r, g, b));
                            }
                            else if (img.pixelFormat == SDK.TY_PIXEL_FORMAT_BAYER8GB)
                            {
                                SWIGTYPE_p_void pointer = (SWIGTYPE_p_void)this.color_data.VoidPtr();
                                int offset = 3 * (img.width * img.height / 2 + img.width / 2);

                                TY_IMAGE_DATA out_buff = SDK.TYInitImageData((uint)psm.color_size, pointer, (uint)(img.width), (uint)(img.height));
                                out_buff.pixelFormat = (int)SDK.TY_PIXEL_FORMAT_BGR;

                                SDK.TYISPProcessImage(psm.color_isp_handle, img, out_buff);
                                SDK.TYISPUpdateDevice(psm.color_isp_handle);

                                //var color_pixel_arr = uint8_t_ARRAY.FromVoidPtr(out_buff.buffer, img.size * 3);
                                this.color_data2 = uint8_t_ARRAY.FromVoidPtr(out_buff.buffer, img.size * 3);
                                //IntPtr ptr2 = this.color_data.VoidPtr2();

                                this.src.width = this.color_width;
                                this.src.height = this.color_height;
                                this.src.size = 3 * this.color_width * this.color_height;
                                this.src.pixelFormat = (int)SDK.TY_PIXEL_FORMAT_RGB;
                                this.src.buffer = this.color_data2.VoidPtr();

                                //this.MySaveBMP(pTrain, color_data2, "test1", img.width, img.height, "d:/test1.png");

                                this.dst.width = img.width;
                                this.dst.height = img.height;
                                this.dst.size = 3 * img.width * img.height;
                                this.dst.pixelFormat = (int)SDK.TY_PIXEL_FORMAT_RGB;
                                this.dst.buffer = undistort_color_data.VoidPtr();

                                C_Camera_TuYang? camera1 = (C_Camera_TuYang?)this.read_var(this._key_camera, "C_Camera_TuYang");
                                if (camera1 == null)
                                {
                                    MessageBox.Show(this.Name + " key_camera 设置有问题！");
                                    return;
                                }
                                
                                Main.WriteLine("彩色图畸变矫正 Start! ");
                                SDK.TYUndistortImage(camera1.color_calib, this.src, null, this.dst);

                                uint8_t_ARRAY.ReleasePtr(this.color_data2);

                                Main.WriteLine("彩色图畸变矫正 End! ");
                                
                                //if (Pic_Count == 1)
                                {
                                    Data_RGB p1 = new Data_RGB(Pic_Count, undistort_color_data, img);
                                    RGB_Array.Add(p1);
                                }
                            }
                            else
                            {

                                Main.WriteLine(this,string.Format("Color Image Type:{0}", img.pixelFormat));
                            }
                            uint8_t_ARRAY.ReleasePtr(pixel_arr);//这里去掉会泄露
                        }
                    }

                    SDK.TYEnqueueBuffer(psm.mHandle, frame.userBuffer, (uint)frame.bufferSize);
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    Main.WriteLine(this,ex.ToString());
                }
            }

            Camera_Receive_Read = false;
        }


        public void TuYang_Init(string camera_id, IntPtr handle, C_Camera_TuYang camera2)
        {

            IntPtr color_isp_handle = new IntPtr();


            SDK.TYEnableComponents(handle, SDK.TY_COMPONENT_DEPTH_CAM);
            SDK.TYEnableComponents(handle, SDK.TY_COMPONENT_RGB_CAM);
            //set depth cam resolution
            int status = SDK.TYSetEnum(handle, SDK.TY_COMPONENT_DEPTH_CAM, SDK.TY_ENUM_IMAGE_MODE, (int)(SDK.TY_RESOLUTION_MODE_1280x960 | SDK.TY_PIXEL_FORMAT_DEPTH16));
            //set color cam resolution

            if (this.camera_format == "yuyv")
            {
                status = SDK.TYSetEnum(handle, SDK.TY_COMPONENT_RGB_CAM, SDK.TY_ENUM_IMAGE_MODE, (int)(SDK.TY_RESOLUTION_MODE_1280x960 | SDK.TY_PIXEL_FORMAT_YUYV));
            }
            else
            {
                status = SDK.TYSetEnum(handle, SDK.TY_COMPONENT_RGB_CAM, SDK.TY_ENUM_IMAGE_MODE, (int)(SDK.TY_RESOLUTION_MODE_1280x960 | SDK.TY_PIXEL_FORMAT_BAYER8GB));
            }


            SDK.TYISPCreate(ref color_isp_handle);
            //SDK_ISP.ColorIspInitSetting(color_isp_handle, handle);

            uint buff_sz;
            SDK.TYGetFrameBufferSize(handle, out buff_sz);


            //获取彩色图宽和高
            SDK.TYGetInt(handle, SDK.TY_COMPONENT_RGB_CAM, SDK.TY_INT_WIDTH, out this.color_width);
            SDK.TYGetInt(handle, SDK.TY_COMPONENT_RGB_CAM, SDK.TY_INT_HEIGHT, out this.color_height);

            //
            Main.WriteLine(this, this.Name + string.Format(" RGB 图像 大小:{0} {1}", this.color_width, this.color_height));

            //获取深度相机标定参数
            uint cal_size1 = camera2.depth_calib.CSize();
            SDK.TYGetStruct(handle, SDK.TY_COMPONENT_DEPTH_CAM, SDK.TY_STRUCT_CAM_CALIB_DATA, camera2.depth_calib.getCPtr(), cal_size1);

            //获取彩色相机标定参数																																																																			  																									  																								  
            uint cal_size2 = camera2.color_calib.CSize();
            SDK.TYGetStruct(handle, SDK.TY_COMPONENT_RGB_CAM, SDK.TY_STRUCT_CAM_CALIB_DATA, camera2.color_calib.getCPtr(), cal_size2);

            //SDK.TYSetEnum(handle, SDK.TY_COMPONENT_RGB_CAM,SDK.TYImageMode,)

            Main.WriteLine(this, string.Format("Depth calib inf width:{0} height:{1}", camera2.depth_calib.intrinsicWidth, camera2.depth_calib.intrinsicHeight));

            for (uint i = 0; i < 9; i++)
            {
                camera2.camera1.depth_calib.intrinsic.data[i] = camera2.depth_calib.intrinsic.data[i];
                camera2.camera1.color_calib.intrinsic.data[i] = camera2.color_calib.intrinsic.data[i];
            }

            Main.WriteLine("摄像头初始化完毕:" + camera_id);
            Main.speak_async("相机ready");

            uint color_size = (uint)this.color_width * (uint)this.color_height * 3;
            this.color_data = new uint8_t_ARRAY(color_size);
            this.undistort_color_data = new uint8_t_ARRAY(color_size);
            this.registration_depth_data = new uint16_t_ARRAY(color_size);

            buffer[0] = new uint8_t_ARRAY((uint)buff_sz);
            buffer[1] = new uint8_t_ARRAY((uint)buff_sz);
            buffer[2] = new uint8_t_ARRAY((uint)buff_sz);
            buffer[3] = new uint8_t_ARRAY((uint)buff_sz);

            SDK.TYEnqueueBuffer(handle, buffer[0].VoidPtr(), buff_sz);
            SDK.TYEnqueueBuffer(handle, buffer[1].VoidPtr(), buff_sz);
            SDK.TYEnqueueBuffer(handle, buffer[2].VoidPtr(), buff_sz);
            SDK.TYEnqueueBuffer(handle, buffer[3].VoidPtr(), buff_sz);

            //设置触发
            TY_TRIGGER_PARAM param = new TY_TRIGGER_PARAM();
            param.mode = SDK.TY_TRIGGER_MODE_SLAVE;//开启触发，支持软硬触发 SDK.TY_TRIGGER_MODE_OFF;//关闭触发，自由采集模式

            bool hasResend;
            SDK.TYHasFeature(handle, SDK.TY_COMPONENT_DEVICE, SDK.TY_BOOL_GVSP_RESEND, out hasResend);
            if (hasResend)
            {

                Main.WriteLine(this, "设置网络模式：有错误就重发" + this.Name);
                SDK.TYSetBool(handle, SDK.TY_COMPONENT_DEVICE, SDK.TY_BOOL_GVSP_RESEND, true);
            }

            //异步抓取
            SDK.TYSetEnum(handle, SDK.TY_COMPONENT_DEVICE, SDK.TY_ENUM_STREAM_ASYNC, (int)(SDK.TY_STREAM_ASYNC_RGB));

            SDK.TYSetStruct(handle, SDK.TY_COMPONENT_DEVICE, SDK.TY_STRUCT_TRIGGER_PARAM, param.getCPtr(), param.CSize());

            SDK.TYStartCapture(handle);

            recvFlag = true;

            Task.Run(() =>
            {
                ParametersMain psm = new ParametersMain(handle, (int)color_size, color_isp_handle);
                Camera_Receive(psm, camera2);
            });

        }

        public void Loop_TuYang_Camera(string camera_id,IntPtr handle, C_Camera_TuYang camera2)
        {



            while (this.Stop==false)
            {
                if (space.vars.bClosingWindows)
                {
                    this.Stop = true; //退出摄像头循环
                }

                if (Camera_Receive_Read == false) continue;

                Thread.Sleep(10);
                if (space.vars.bAutoMode == false)
                {
                    continue;
                }
                if (this.bCatching == false)
                {
                    continue;
                }
                else
                {
                    
                    Main.WriteLine(this,"TY软激活" + this.Name);

                    DateTime robot_time_f = new DateTime(DateTime.Now.Ticks);
                    this.save_var(this.key_camera_time, "DateTime", robot_time_f);

                    //SDK.TYClearBufferQueue(handle);

                    SDK.TYSendSoftTrigger(handle);
                    Pic_Count = 0;
                    Thread.Sleep(300);

                    this.bCatching = false;
                }
                Thread.Sleep(100);
            }

            recvFlag = false;//退出图片接收循环
            Thread.Sleep(100);
        }





        public static Bitmap ByteToBitmap(uint8_t_ARRAY color_pixel_arr, int width, int height)// byte[] imgBGR)
        {
            //Main.WriteLine(this,"ByteToBitmap Start",true);
            int len = width * height * 3;
            //构造一个位图数组进行数据存储
            byte[] rgbvalues = new byte[len];

            //Main.WriteLine(this,"开始内存复制图片1", true);
            {
                IntPtr ptr2 = color_pixel_arr.VoidPtr2();

                Marshal.Copy(ptr2, rgbvalues, 0, len);
            }
            //Main.WriteLine(this,"开始内存复制图片2", true);

            //位图矩形

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            //以可读写的方式将图像数据锁定
            BitmapData bmpdata = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            //得到图形在内存中的首地址
            IntPtr ptr = bmpdata.Scan0;

            //把处理后的图像数组复制回图像
            Marshal.Copy(rgbvalues, 0, ptr, len);
            //解锁位图像素
            bmp.UnlockBits(bmpdata);

            //Main.WriteLine(this,"ByteToBitmap End", true);
            return bmp;
        }


        public void 读取点云(
            TY_IMAGE_DATA img,
            I_Train pTrain)
        {
            this.读取3D点云模块.img = img;
            this.读取3D点云模块.width = color_width;
            this.读取3D点云模块.height = color_height;
            this.读取3D点云模块.glb_p3dArray = glb_p3dArray;
            this.读取3D点云模块.Image_Point_3D = Image_Point_3D;
            this.读取3D点云模块.registration_depth_data = this.registration_depth_data;
            this.读取3D点云模块.undistort_color_data = this.undistort_color_data;

            Thread thread = new Thread(() =>
            {
                this.读取3D点云模块.Run(this.pTrain);
            });
            thread.Start();
        }

    }
}
