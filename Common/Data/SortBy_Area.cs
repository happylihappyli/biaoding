using Common_Robot;
using Common_Robot2;
using System.Collections;

namespace Test1
{
    public class SortBy_Area : IComparer
    {
        public int Compare(object? x, object? y)
        {
            C_Data? p1 = (C_Data?)x;
            C_Data? p2 = (C_Data?)y;

            double? d1 = p1?.calculate_area();
            double? d2 = p2?.calculate_area();

            double? tmp = d1 - d2;

            if (tmp > 0)
            {
                return 1;
            }
            else if (tmp < 0)
            {
                return -1;
            }
            return 0;
        }
    }
}
