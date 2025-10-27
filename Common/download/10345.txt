using Newtonsoft.Json.Linq;
using Common_Robot2;
using System.Drawing;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using ConverxHull;
using Point = System.Drawing.Point;

namespace Common_Robot2
{

    public class S_Draw
    {
        C_Space space;
        public S_Draw(C_Space space)
        {
            this.space = space;
        }

        public string draw_ploygen_to_bmp(Pen pPen, JArray pArray, Bitmap pBitmap)//Graphics g,Pen pPen,JArray pArray)//, Bitmap pBitmap)
        {


            Graphics g = Graphics.FromImage(pBitmap);

            FontFamily fontFamily = new FontFamily("Arial");
            Font font = new Font(fontFamily, 16, FontStyle.Regular, GraphicsUnit.Pixel);
            StringBuilder pStr = new StringBuilder();
            for (var i = 0; i < pArray.Count; i++)
            {
                JObject pItem = (JObject)pArray[i];
                JArray pJArray;
                //if (space.vars.server_url_index == 1)
                {
                    //pJArray = (JArray)pItem.SelectToken("contour");
                    pJArray = (JArray)pItem.SelectToken("vertex");
                }
                //else
                //{
                //    pJArray = (JArray)pItem.SelectToken("mask");
                //}

                if (pJArray != null)
                {
                    (C_Point3D p2, string draw_string) = draw_frame(pPen, g, pJArray, true);
                    //var p2 = tmp.Item1;
                    //C_Point3D pCenter = tmp.Item1;
                    pStr.Append("{ \"fill\":\"yellow\",\"stroke\":\"red\",\"strokeWidth\":3," +
                        "\"category\":\"PolygonDrawing\",\"key\":" + i + "," +
                        "\"geo\":\"F " + draw_string + "\",\"loc\":\"" + p2.x + " " + p2.y + "\"," +
                        "\"label\":\"box\"}");
                    if (i < pArray.Count - 1)
                    {
                        pStr.Append(",");
                    }
                }

                //g.DrawString("@" + i, font, Brushes.Red, (float)p2.x, (float)p2.y);
            }

            //\"position\": \" - 5 277\",
            string a = "{ " +
                  "\"model\": {" +
                  "              \"class\": \"GraphLinksModel\"," +
                  " \"nodeDataArray\": [" +
                  pStr.ToString() + "]," +
                 " \"linkDataArray\": []}" +
                 "       }";
            return a;
        }



        //public static void draw_frame2(Graphics g, JObject pItem)
        //{
        //    JArray pArray2 = (JArray)pItem.SelectToken("vertex");
        //    if (pArray2 == null) return;
        //    int x0 = (int)(double.Parse(pArray2[0].ToString()) * space.vars.draw_scale_hk + space.vars.draw_offset_x_hk);
        //    int y0 = (int)(double.Parse(pArray2[1].ToString()) * space.vars.draw_scale_hk + space.vars.draw_offset_y_hk);
        //    int x1 = (int)(double.Parse(pArray2[2].ToString()) * space.vars.draw_scale_hk + space.vars.draw_offset_x_hk); ;
        //    int y1 = (int)(double.Parse(pArray2[3].ToString()) * space.vars.draw_scale_hk + space.vars.draw_offset_y_hk); ;

        //    g.DrawRectangle(new Pen(Color.Blue, 3), x0, y0, x1 - x0, y1 - y0);
        //}


        public (C_Point3D, string) draw_frame(Pen pPen, Graphics g, JArray pJArray, bool bModify = true)
        {
            StringBuilder content = new StringBuilder();
            List<C_Point3D> Points = CommonMain.简化边框读取边框里的点(space,pJArray, bModify);

            double sum_x = 0;
            double sum_y = 0;
            int x0 = 0;
            int y0 = 0;
            for (var j = 0; j < Points.Count; j++)
            {
                int x1 = (int)((int)Points[j].x);
                int y1 = (int)((int)Points[j].y);
                sum_x += x1;
                sum_y += y1;

                if (j > 0)
                {
                    g.DrawLine(pPen, new Point(x0, y0), new Point(x1, y1));
                }
                x0 = x1;
                y0 = y1;
            }

            int x = (int)((int)Points[0].x);
            int y = (int)((int)Points[0].y);

            g.DrawLine(pPen, new Point(x0, y0), new Point(x, y));

            C_Point3D p1 = new C_Point3D(sum_x / Points.Count, sum_y / Points.Count, 0);


            for (var j = 0; j < Points.Count; j++)
            {
                int x1 = (int)((int)Points[j].x - p1.x);
                int y1 = (int)((int)Points[j].y - p1.y);
                if (j > 0)
                {
                    if (j == Points.Count - 1)
                    {
                        content.Append("L" + x1 + " " + y1 + "z");
                    }
                    else
                    {
                        content.Append("L" + x1 + " " + y1 + " ");
                    }
                }
                else
                {
                    content.Append("M" + x1 + " " + y1 + " ");
                }
            }


            return (p1, content.ToString());
        }



        public void pic_paint(C_Node pNode, C_Camera_Const camera1 ,Graphics g, I_Train pTrain, C_Data pMain)
        {
            if (pMain == null || pTrain==null)
            {
                return;
            }

            for (var i = 0; i < pMain.Draw_Circle.Count; i++)
            {
                C_Point3D pPoint = (C_Point3D)pMain.Draw_Circle[i];
                int u = (int)pPoint.x;
                int v = (int)pPoint.y;

                g.DrawEllipse(new Pen(Color.Green, 2), u - 10, v - 10, 20, 20);
            }


            //if (space.vars.b_draw_camera_xyz)
            //{
            //    double x0 = space.vars.camera_v_x;
            //    double y0 = space.vars.camera_v_y;
            //    double z0 = space.vars.camera_v_z;
            //    var len = 100;
            //    C_Point3D pCenter_xyz = new C_Point3D(x0, y0, z0);
            //    C_Point3D pFaxiangliang = new C_Point3D(x0 + len, y0, z0);

            //    C_Point3D pCenter = Tools.f_3DPoint_to_depth(camera1.color_calib, pCenter_xyz);
            //    C_Point3D pDraw1 = Tools.f_3DPoint_to_depth(camera1.color_calib, pFaxiangliang);


            //    g.DrawLine(new Pen(Color.Red, 2),
            //              (float)pCenter.x, (float)pCenter.y,
            //              (float)pDraw1.x, (float)pDraw1.y);


            //    pFaxiangliang = new C_Point3D(x0, y0 + len, z0);

            //    pDraw1 = Tools.f_3DPoint_to_depth(camera1.color_calib, pFaxiangliang);


            //    g.DrawLine(new Pen(Color.Green, 2),
            //              (float)pCenter.x, (float)pCenter.y,
            //              (float)pDraw1.x, (float)pDraw1.y);

            //    pFaxiangliang = new C_Point3D(x0, y0, z0 + len);

            //    pDraw1 = Tools.f_3DPoint_to_depth(camera1.color_calib, pFaxiangliang);


            //    g.DrawLine(new Pen(Color.Blue, 2),
            //              (float)pCenter.x, (float)pCenter.y,
            //              (float)pDraw1.x, (float)pDraw1.y);
            //}

            //if (space.vars.b_draw_arm_xyz)
            {
                //double x0 = space.vars.arm_v_x;
                //double y0 = space.vars.arm_v_y;
                //double z0 = space.vars.arm_v_z;
                //C_Point3D pCenter_xyz0 = new C_Point3D(x0, y0, z0);
                //C_Point3D pCenter_xyz = Tools.机械臂坐标到摄像头坐标(camera1, pCenter_xyz0);

                //var len = 100;
                //C_Point3D pFaxiangliang0 = new C_Point3D(x0 + len, y0, z0);
                //C_Point3D pFaxiangliang = Tools.机械臂坐标到摄像头坐标(camera1, pFaxiangliang0);

                //C_Point3D pCenter = Tools.f_3DPoint_to_depth(camera1.color_calib, pCenter_xyz);

                //C_Point3D pDraw1 = Tools.f_3DPoint_to_depth(camera1.color_calib, pFaxiangliang);

                //g.DrawLine(new Pen(Color.Red, 2),
                //          (float)pCenter.x, (float)pCenter.y,
                //          (float)pDraw1.x, (float)pDraw1.y);

                //pFaxiangliang0 = new C_Point3D(x0, y0 + len, z0);
                //pFaxiangliang = Tools.机械臂坐标到摄像头坐标(camera1, pFaxiangliang0);

                //pDraw1 = Tools.f_3DPoint_to_depth(camera1.color_calib, pFaxiangliang);

                //g.DrawLine(new Pen(Color.Green, 2),
                //          (float)pCenter.x, (float)pCenter.y,
                //          (float)pDraw1.x, (float)pDraw1.y);

                //pFaxiangliang0 = new C_Point3D(x0, y0, z0 + len);
                //pFaxiangliang = Tools.机械臂坐标到摄像头坐标(camera1, pFaxiangliang0);

                //pDraw1 = Tools.f_3DPoint_to_depth(camera1.color_calib, pFaxiangliang);

                //g.DrawLine(new Pen(Color.Blue, 2),
                //          (float)pCenter.x, (float)pCenter.y,
                //          (float)pDraw1.x, (float)pDraw1.y);
            }

            List<C_Data> pArray = (List<C_Data>)space.read_vars(pTrain,pNode, "Mains", "List<C_Main>");

            //if (space.vars.pList_Collision.Count > 0) //如果有碰撞的点
            {
                for (var k = 0; k < pArray.Count; k++)
                {
                    C_Data pMain2 = (C_Data)pArray[k];
                    if (pMain2 == null) continue;

                }
            }

        }
    }
}


