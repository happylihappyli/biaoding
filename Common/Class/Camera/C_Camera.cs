using Common_Robot;
using MathNet.Numerics.LinearAlgebra;
//using MvCamCtrl.NET;
using pcammls;
using Test1;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDK = pcammls.pcammls;
using SDK_ISP = pcammls_isp.pcammls_isp_api;


namespace Test1
{
    public class C_Camera
    {

        // 海康相机 start
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

         


        //public static FrmSet pFrmSet = null;
        //public static void 摄像头校对3()
        //{
        //    if (pFrmSet==null) return;
        //    double[] data;
        //    int rows = int.Parse(pFrmSet.tx_rows.Text);
        //    int cols = int.Parse(pFrmSet.tx_cols.Text);
        //    int grid_size = int.Parse(pFrmSet.tx_grid_size.Text);

        //    try
        //    {
        //        string[] files = Directory.GetFiles(pFrmSet.tx_calibration_path.Text);
        //        //图像数量
        //        int image_count = 0;
        //        OpenCvSharp.Size image_size = new OpenCvSharp.Size(640, 480);  // 图像的尺寸
        //        OpenCvSharp.Size board_size = new OpenCvSharp.Size(rows, cols); // 标定板角点的行列个数
        //        int CornerNum = board_size.Width * board_size.Height; //每张图片上总的角点数
        //        Mat[] image_points_seqmat = new Mat[files.Length]; // 存储所有图片的角点
        //        Mat image_points_bufmat = new Mat();
        //        Mat view_gray = new Mat();// 获取灰度图

        //        for (int s = 0; s < files.Length; s++)
        //        {
        //            Mat imageInput = new Mat(files[s], ImreadModes.AnyColor);
        //            if (image_count == 0)  // 获得图像尺寸
        //            {
        //                image_size.Width = imageInput.Cols;
        //                image_size.Height = imageInput.Rows;
        //            }

        //            //输入的图像 角点行列个数 输出的图像
        //            //如果检测到了与原图有不相同的角点，结束当前函数
        //            var corners = new Mat<Point2f>();
        //            if (!Cv2.FindChessboardCorners(imageInput, board_size, corners))//image_points_bufmat))
        //            {
        //                return;
        //            }
        //            else
        //            {
        //                image_points_bufmat = corners;
        //                var data2 = corners.ToArray();
        //                //image_points_bufmat.GetArray<CV_32FC2 > (out var data2);
        //                space.console.WriteLine("image_points_bufmat");
        //                for (var k = 0; k < data2.Length; k++)
        //                {
        //                    space.console.WriteLine(data2[k] + ",");
        //                }
        //                space.console.WriteLine("image_points_bufmat");

        //                Cv2.CvtColor(imageInput, view_gray, ColorConversionCodes.RGB2GRAY);
        //                // 对粗提取的角点进行亚像素精确化
        //                Cv2.Find4QuadCornerSubpix(view_gray, image_points_bufmat, new OpenCvSharp.Size(11, 11));
        //                image_points_seqmat[s] = (image_points_bufmat);
        //                Cv2.CvtColor(view_gray, view_gray, ColorConversionCodes.GRAY2RGB);
        //                // 在图像上显示角点位置
        //                Cv2.DrawChessboardCorners(view_gray, board_size, image_points_bufmat, true); //用于在图片中标记角点
        //            }
        //            image_count++;
        //        }
        //        OpenCvSharp.Size square_size = new OpenCvSharp.Size(grid_size, grid_size);  // 世界坐标系下 黑白块宽度  30mm  3cm


        //        Mat[] object_pointsmat = new Mat[image_count];
        //        Mat[] rotationVectors;  // 旋转向量
        //        Mat[] translationVectors; // 平移矩阵
        //        int i, j, t;

        //        for (t = 0; t < image_count; t++)//6张图
        //        {
        //            List<Point3f> tempPointSet = new List<Point3f>();
        //            for (i = 0; i < board_size.Height; i++)//6列
        //            {
        //                for (j = 0; j < board_size.Width; j++)//4行
        //                {
        //                    Point3f realPoint = new Point3f();
        //                    //假设标定板放在世界坐标系中z=0的平面上
        //                    realPoint.X = j * square_size.Width;
        //                    realPoint.Y = i * square_size.Height;
        //                    realPoint.Z = 0;

        //                    //zzz happyli add
        //                    //mm.Add(ToMatPoint3f(realPoint));
        //                    tempPointSet.Add(realPoint);
        //                }
        //            }

        //            var mm2 = Mat<Point3f>.FromArray(tempPointSet);
        //            object_pointsmat[t] = (mm2);
        //        }


        //        //object_points 世界坐标系下的点 imagePoints 图像坐标系下的点
        //        //图像中的点的位置 内参矩阵 畸变矩阵 内参数的输出向量 外参数的输出向量 每个标定图片的重投影均方根误差的输出向量
        //        var rms = Cv2.CalibrateCamera(object_pointsmat, image_points_seqmat
        //            , image_size, cs1.cameraMatrixMat, cs1.distCoeffsMat, out rotationVectors, out translationVectors,
        //            CalibrationFlags.None
        //            );


        //        space.console.WriteLine("=================相机标定结果");
        //        space.console.WriteLine(rms + "");

        //        space.console.WriteLine("distCoeffsMat=");
        //        cs1.distCoeffsMat.GetArray<double>(out data);
        //        for (j = 0; j < data.Length; j++)
        //        {
        //            space.console.WriteLine(data[j] + ",");
        //        }

        //        space.console.WriteLine("cameraMatrixMat=");
        //        cs1.cameraMatrixMat.GetArray<double>(out data);
        //        for (j = 0; j < data.Length; j++)
        //        {
        //            space.console.WriteLine(data[j] + ",");
        //        }

        //        space.console.WriteLine("rotationVectors");
        //        for (i = 0; i < rotationVectors.ToArray().Length; i++)
        //        {
        //            Mat p = rotationVectors.ToArray()[i];
        //            p.GetArray<double>(out data);
        //            for (j = 0; j < data.Length; j++)
        //            {
        //                space.console.WriteLine(data[j] + ",");
        //            }
        //        }
        //        space.console.WriteLine(translationVectors + "");


        //        for (i = 0; i < translationVectors.ToArray().Length; i++)
        //        {
        //            Mat p = translationVectors.ToArray()[i];
        //            p.GetArray<double>(out data);
        //            for (j = 0; j < data.Length; j++)
        //            {
        //                space.console.WriteLine(data[j] + ",");
        //            }
        //        }




        //        //*********************  检验标定效果  **********************
        //        double total_err = 0.0; // 所有图像的平均误差的总和
        //        double err = 0.0; // 每幅图像的平均误差
        //        int count = 0;
        //        for (i = 0; i < image_count; i++)
        //        {
        //            Mat image_points = new Mat();
        //            Cv2.ProjectPoints(
        //                object_pointsmat[i], rotationVectors[i], translationVectors[i]
        //                 , cs1.cameraMatrixMat, cs1.distCoeffsMat,
        //                 image_points
        //              );
        //            err = Cv2.Norm(image_points, image_points_seqmat[i], NormTypes.L2);
        //            total_err += err /= CornerNum;
        //            space.console.WriteLine(i + 1 + "幅图像的平均误差：" + err + "像素");
        //        }
        //        for (i = 0; i < image_count; i++)
        //        {
        //            if (count > 0)
        //                break;
        //            //Mat rotation_matrixmat = new Mat();
        //            //Cv2.Rodrigues(rotationVectors[i], rotation_matrixmat);
        //            Mat imageSource = new Mat(files[i]);
        //            //pictureBox1.Image = imageSource.ToBitmap();
        //            Mat newimage = new Mat();
        //            //原图 矫正后图像 内参矩阵 畸变系数
        //            Cv2.Undistort(imageSource, newimage, cs1.cameraMatrixMat, cs1.distCoeffsMat);
        //            //pictureBox2.Image = newimage.ToBitmap();
        //            count++;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}


        //public static Mat img_rotate(Mat src, float angle)
        //{
        //    Mat dst = new Mat();
        //    Point2f center = new Point2f(src.Cols / 2, src.Rows / 2);
        //    Mat rot = Cv2.GetRotationMatrix2D(center, angle, 1);
        //    Size2f s2f = new Size2f(src.Size().Width, src.Size().Height);
        //    Rect box = new RotatedRect(new Point2f(0, 0), s2f, angle).BoundingRect();
        //    double xx = rot.At<double>(0, 2) + box.Width / 2 - src.Cols / 2;
        //    double zz = rot.At<double>(1, 2) + box.Height / 2 - src.Rows / 2;
        //    rot.Set(0, 2, xx);
        //    rot.Set(1, 2, zz);
        //    Cv2.WarpAffine(src, dst, rot, box.Size);
        //    return dst;
        //}



        public static byte[]? ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[]?)converter.ConvertTo(img, typeof(byte[]));
        }



    }
}
