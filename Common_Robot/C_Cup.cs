namespace Test1
{
    /// <summary>
    /// 吸嘴
    /// </summary>
    public class C_Cup
    {
        public float x;
        public float y;
        public float z;
        public string region = "0";//0,1,2,3,4吸盘的分区

        public C_Cup(float x, float y,float z, string region)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.region = region;
        }
    }
}
