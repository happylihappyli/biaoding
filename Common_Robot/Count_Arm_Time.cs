using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common_Robot
{
    public class Count_Arm_Time
    {
        public static double Count = 0;
        public static double mSeconds = 0;//毫秒

        public static string avg()
        {
            if (Count == 0) return "动作个数=0";
            string strLine ="平均耗时：（毫秒）"+ mSeconds / Count;
            return strLine;
        }


        public static string avg_value()
        {
            if (Count == 0) return "动作个数=0";
            string strLine = ""+ Math.Round( mSeconds / Count/1000,1);
            return strLine;
        }

        public static void clear()
        {
            Count = 0;
            mSeconds = 0;
        }
    }
}
