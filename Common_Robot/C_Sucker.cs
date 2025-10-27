using System.Collections.Concurrent;

namespace Test1
{
    /// <summary>
    /// 吸盘
    /// </summary>
    public class C_Sucker
    {
        public float dx = 0;
        public float dy = 0;
        public float dz = 0;

        public float x = 0;
        public float y = 0;
        public float z = 0;



        public ConcurrentDictionary<string, List<C_Cup>> regions=new ConcurrentDictionary<string, List<C_Cup>>();

        public void add(string region, C_Cup cup)
        {
            if (regions.ContainsKey(region) == false)
            {
                regions.TryAdd(region, new List<C_Cup>());
            }
            regions[region].Add(cup);
        }

        public void set_first_region_center(float x, float y,float z)
        {
            this.x = x + dx;
            this.y = y + dy;
            this.z = z + dz;
        }

        public int regions_count()
        {
            return regions.Count;
        }
    }
}
