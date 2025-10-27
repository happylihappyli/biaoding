
using Common_Robot;
using Common_Robot2;
using System.Collections;

namespace Test1
{
    public class SortBy_Distance : IComparer
    {
        public int Compare(object? x, object? y)
        {
            double? tmp = ((C_Planet?)x)?.distance_to_one_point - ((C_Planet?)y)?.distance_to_one_point;

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



