using Common_Robot2;
using ConverxHull;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Common_Robot2
{
    public class Tools_Cloud_Point
    {
        C_Space space;
        public Tools_Cloud_Point(C_Space space)
        {
            this.space = space;
        }

        //和  depth_to_3DPoint2 是一样的算法
        //public static C_Point3D depth_to_3DPoint(
        //    TY_VECT_3F_ARRAY p3dArray,
        //    int img_width,
        //    int u, int v)
        //{

        //    int offset = img_width * v + u;

        //    //int offset = img_width * v + u;
        //    //读取点云数据
        //    float p3d_fx = p3dArray.getitem(offset).x;
        //    float p3d_fy = p3dArray.getitem(offset).y;
        //    float p3d_fz = p3dArray.getitem(offset).z;

        //    return new C_Point3D(p3d_fx, p3d_fy, p3d_fz);
        //}



        public static C_Point3D depth_to_3DPoint2(C_Camera_Const p1, C_Point3D pPoint)
        {
            //TY_CAMERA_CALIB_INFO src_calib = space.vars.depth_calib;
            C_Camera_Calib src_calib = p1.color_calib;// .depth_calib;
            //相机内参
            double cam_cx = src_calib.intrinsic.data[2];// 6.52000427e+02;// * 640 / 1280;
            double cam_cy = src_calib.intrinsic.data[5];//5.06252594e+02;// * 480 / 960;
            double cam_fx = src_calib.intrinsic.data[0];// 1.09007092e+03;
            double cam_fy = src_calib.intrinsic.data[4];//1.09046729e+03;
            double factor = 1;

            //逐点处理，此过程可以使用numpy优化
            double p_z = pPoint.z / factor;
            double p_x = (pPoint.x - cam_cx) * p_z / cam_fx;
            double p_y = (pPoint.y - cam_cy) * p_z / cam_fy;
            return new C_Point3D(p_x, p_y, p_z);
        }


        public List<C_Point3D> 简化边框读取边框里的点(
            JArray pJArray,
            bool bModify = true)
        {
            List<C_Point3D> pList_Points = new List<C_Point3D>();

            for (var j = 0; j < pJArray.Count; j++)
            {
                int x1 = (int)pJArray[j][0];
                int y1 = (int)pJArray[j][1];

                if (bModify)
                {
                    x1 = (int)Math.Round(x1 / space.vars.draw_scale_tuyang + space.vars.draw_offset_x_tuyang);
                    y1 = (int)Math.Round(y1 / space.vars.draw_scale_tuyang + space.vars.draw_offset_y_tuyang);
                }
                C_Point3D pPoint = new C_Point3D(x1, y1, 0);
                pList_Points.Add(pPoint);
            }

            return pList_Points;

            ////化简点
            ////if (space.vars.b_simplify)
            //{
            //    for (var j = 0; j < pList_Points.Count - 2; j++)
            //    {
            //        var x1 = ((C_Point3D)pList_Points[j]).x;
            //        var y1 = ((C_Point3D)pList_Points[j]).y;

            //        var x2 = ((C_Point3D)pList_Points[j + 1]).x;
            //        var y2 = ((C_Point3D)pList_Points[j + 1]).y;

            //        var x3 = ((C_Point3D)pList_Points[j + 2]).x;
            //        var y3 = ((C_Point3D)pList_Points[j + 2]).y;

            //        var area = 0.5 * Math.Abs((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1));

            //        if (area < space.vars.max_area)
            //        {
            //            pList_Points.RemoveAt(j + 1);
            //            j -= 1;
            //        }
            //    }
            //}

            //C_Point3D[] Points = new C_Point3D[pList_Points.Count];
            //for (var j = 0; j < pList_Points.Count; j++)
            //{
            //    C_Point3D pPoint = (C_Point3D)pList_Points[j];

            //    var x1 = pPoint.x;
            //    var y1 = pPoint.y;

            //    Points[j] = new C_Point3D(x1, y1, 0);

            //}
            //return Points;
        }
    }
}
