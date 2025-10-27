using Common_Robot;
using Common_Robot2;
using System.Collections;

namespace Test1
{
    public class SortBy_Points_Count : IComparer
    {
        public int Compare(object? x, object? y)
        {
            var tmp = (((C_Data?)x)?.list_3D_Point.Count - ((C_Data?)y)?.list_3D_Point.Count);

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
