namespace Test1
{
    public class C_Box_Type
    {
        public static int ID_Count = 0;

        public int ID = 1;

        public int type_index = 0;
        public long x_len = 0;
        public long y_len = 0;
        public long z_len = 0;


        public int index = 0;
        public int count = 0;//某种类型箱子的总个数
        public int max=0;
        public string file = "";

        public int group_id = -1;

        private int m_总个数 = 0;//某种类型箱子的总个数

        public int 总个数
        {
            get
            {
                return m_总个数;
            }
            private set
            {
                m_总个数 = value;
            }
        }




        public C_Box_Type(
            int group_id,
            int type_index,
            long x_len, long y_len, long z_len,
            int index)
        {
            C_Box_Type.ID_Count++;
            this.ID = C_Box_Type.ID_Count;
            this.type_index = type_index;

            this.x_len = x_len;
            this.y_len = y_len;
            this.z_len = z_len;
            this.group_id = group_id;
            this.index = index;
        }

        public C_Box_Type Clone()
        {
            C_Box_Type pType = new C_Box_Type(group_id, type_index, x_len, y_len, z_len, index);
            pType.count = count;
            pType.file = file;
            return pType;
        }
    }
}