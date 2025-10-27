
using Common_Robot2;
using System.Collections;

namespace Test1
{
    public class SortBy_Z_Asc : IComparer
    {
        public int Compare(object? x, object? y)
        {

            C_Planet_Catch? s1 = (C_Planet_Catch?)x;
            C_Planet_Catch? s2 = (C_Planet_Catch?)y;

            int tmp = (int)(s1?.pData?.pCenter?.z - s2?.pData?.pCenter?.z);

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



