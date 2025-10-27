
using Common_Robot;
using Common_Robot2;
using System.Collections;

namespace Test1
{
    public class SortBy_Main_Z_Asc : IComparer
    {
        public int Compare(object? x, object? y)
        {
            C_Data? s1 = (C_Data?)x;
            C_Data? s2 = (C_Data?)y;

            float? tmp =s1?.pCenter?.z - s2?.pCenter?.z;

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



