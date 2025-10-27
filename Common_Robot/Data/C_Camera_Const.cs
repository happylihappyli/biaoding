using ConverxHull;
using MathNet.Numerics.LinearAlgebra.Double;
//using pcammls;

namespace Common_Robot2
{
    public class C_Camera_Const: C_Matrix
    {
        //public TY_CAMERA_CALIB_INFO depth_calib = new TY_CAMERA_CALIB_INFO();//深度相机标定参数
        //public TY_CAMERA_CALIB_INFO color_calib = new TY_CAMERA_CALIB_INFO();//彩色相机标定参数

        public C_Camera_Calib depth_calib = new C_Camera_Calib();//深度相机标定参数
        public C_Camera_Calib color_calib = new C_Camera_Calib();//彩色相机标定参数

        public C_Camera_Const()
        {

        }

    }


    public class C_Matrix
    {
        public DenseMatrix M_Rotate;//旋转 矩阵 //坐标变换矩阵，是抓取的还是放置的，由照相机来决定
        public C_Point3D p1_center;//
        public C_Point3D p2_center;//

        public C_Matrix()
        {

        }

    }
}
