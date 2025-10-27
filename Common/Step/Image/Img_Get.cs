using Common_Robot2;
using pcammls;
using Test1.Common.Data;
using SDK = pcammls.pcammls;
using SDK_ISP = pcammls_isp.pcammls_isp_api;//TYISP.cs

namespace Test1
{
    public class Img_Get : C_Node
    {
        public I_Camera? pCamera = null;
        public uint8_t_ARRAY? pixel_arr = null;
        public TY_IMAGE_DATA? img = null;
        public uint8_t_ARRAY color_data;//彩色原图
        public string data_out = "key_rgb";
        public string key_save = "";
        public Data_RGB? pLast2;



        public Img_Get(string name, C_Space space_parent, C_Space space) :
            base(name, space_parent, space)
        {
        }

        public override Task run_sub()
        {
            run_sub_main();
            return Task.CompletedTask;
        }

        private void run_sub_main()
        {
            RGB图处理(this.pixel_arr, this.img);
        }

        public void RGB图处理(
            uint8_t_ARRAY? pixel_arr,
            TY_IMAGE_DATA? img)
        {
            if (pCamera == null)
            {
                Main.WriteLine(this, "pCamera==null");
                return;
            }
            try
            {
                Main.WriteLine(this,">>>RGB图片处理");
                if (img?.pixelFormat == SDK.TY_PIXEL_FORMAT_YVYU)
                {
                    Main.WriteLine(this,"RGB 格式 YVYU ");
                    SDK_ISP.ConvertYVYU2RGB(pixel_arr, this.color_data, img.width, img.height); //YVYU转成RGB
                }
                else if (img?.pixelFormat == SDK.TY_PIXEL_FORMAT_YUYV)
                {
                    Main.WriteLine(this,"Get RGB 格式 YUYV start" );
                    if (this.key_save != "")
                        pCamera.SaveBMP(pTrain, pixel_arr, this.key_save, img.width, img.height);
                    pCamera.SaveBMP(pTrain, pixel_arr, this.data_out, img.width, img.height);
                    
                    Main.WriteLine(this,"Get RGB 格式 YUYV finished ");
                }
                else if (img.pixelFormat == SDK.TY_PIXEL_FORMAT_BAYER8GB)
                {
                    
                    Main.WriteLine(this,"RGB 格式 BAYER8GB start ");
                    if (this.key_save != "")
                        pCamera.SaveBMP(pTrain, pixel_arr, this.key_save, img.width, img.height);
                    pCamera.SaveBMP(pTrain, pixel_arr, this.data_out, img.width, img.height);
                    
                    Main.WriteLine(this,"RGB 格式 BAYER8GB finished");
                }
                else
                {
                    Main.WriteLine(this,string.Format("RGB格式:{0}", img.pixelFormat) );
                }
                Main.WriteLine(this,">>>RGB图片处理结束");
            }
            catch (Exception e)
            {
                Main.WriteLine(this,e.ToString());
            }
        }

        public override void init()
        {
        }
    }
}