using Common_Robot2;
using ConverxHull;
using pcammls;

namespace Test1
{
    public class C_Camera_TuYang:C_Matrix
    {
        public TY_CAMERA_CALIB_INFO depth_calib = new TY_CAMERA_CALIB_INFO();//深度相机标定参数
        public TY_CAMERA_CALIB_INFO color_calib = new TY_CAMERA_CALIB_INFO();//彩色相机标定参数
        public C_Camera_Const camera1=new C_Camera_Const();
        public string file;

        public C_Camera_TuYang()
        {

        }
    }
}