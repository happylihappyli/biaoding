namespace Test1
{
    public class C_Height
    {
        public int x = 0;
        public int y = 0;
        public int height = 0;
        public double rank = 0;//优先级
        public string index = "";
        public int index_i = 0;
        public int index_j = 0;

        public C_Height(string index, int index_i,int index_j,int x, int y, int height)
        {
            this.index = index;
            this.index_i = index_i;
            this.index_j = index_j;

            this.x = x;
            this.y = y;
            this.height = height;
        }
    }
}