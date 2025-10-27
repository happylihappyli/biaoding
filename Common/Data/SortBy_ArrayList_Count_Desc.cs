using System.Collections;

namespace Test1
{
    public class SortBy_ArrayList_Count_Desc : IComparer
    {
        public int Compare(object? x, object? y)
        {
            var tmp = (((ArrayList?)x)?.Count - ((ArrayList?)y)?.Count);

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



