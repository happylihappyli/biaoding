
using Common_Robot;
using Common_Robot2;
using ConverxHull;
using System.Collections;

namespace Test1
{
    public class SortBy_Catch_arm_a_Z_Desc : IComparer
    {
        public int Compare(object? x, object? y)
        {
            C_Point3D? p1 = ((C_Planet_Catch?)x)?.arm_a;
            C_Point3D? p2 = ((C_Planet_Catch?)y)?.arm_a;

            var tmp = (p1?.z - p2?.z);// - p1.x*0.1 - p1.y*0.1

            if (tmp > 0)
            {
                return -1;
            }
            else if (tmp < 0)
            {
                return 1;
            }
            return 0;
        }
    }
}



