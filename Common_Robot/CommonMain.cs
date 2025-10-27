using ConverxHull;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common_Robot2
{
    public class CommonMain
    {
        public delegate void speak_async(string str);
        public static speak_async i_speak = null;

        public static bool bDebug_Voice=false;

        public static I_Train_Factory Train_Factory = null;
        public static int Voice_Time=1000; //毫秒


        public static I_Train create_train(string ID)
        {
            if (Train_Factory != null)
            {
                return Train_Factory.create(ID);
            }
            else
            {
                return null;
            }
        }


        public static void WriteLine(string strLine)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            Console.WriteLine(time + " " + strLine);
        }
        public static List<C_Point3D> 简化边框读取边框里的点(
            C_Space space,
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
        }



    }
}
