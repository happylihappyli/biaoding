
using ConverxHull;

namespace Test1
{
    public class C_Box
    {
        public int space_index=0;
        public long waist=0;

        public string index = "";
        public int index_i;
        public int index_j;
        public int index_k;
        public int Type = 0;
        public float x_len = 0;
        public float y_len = 0;
        public float z_len = 0;

        public C_Point3D pos = new C_Point3D(0,0,0);
        public C_Box_Type pBox_Type;
        public float z_index;

        public C_Box(int Type, float x_len, float y_len, float z_len,
            float x, float y, float z, long waist, int space_index)
        {
            this.space_index = space_index;
            this.Type = Type;
            this.x_len = x_len;
            this.y_len = y_len;
            this.z_len = z_len;

            pos.x = x;//代表盒子区域最小的x，
            pos.y = y;//代表盒子区域最小的y，
            pos.z = z;//代表盒子区域最小的z，

            this.waist = waist;//这个盒子放上去会浪费多少空间
        }



        public C_Box(string index, string strLine)
        {
            this.index = index;
            var strSplit = strLine.Split(',');
            if (strSplit.Length > 3)
            {
                this.Type = int.Parse(strSplit[0]);
                this.x_len = int.Parse(strSplit[1]);
                this.y_len = int.Parse(strSplit[2]);
                this.z_len = int.Parse(strSplit[3]);

                if (strSplit.Length > 6)
                {
                    pos.x = int.Parse(strSplit[4]);
                    pos.y = int.Parse(strSplit[5]);
                    pos.z = int.Parse(strSplit[6]);
                }
            }
        }

        public C_Box(string index,
            int index_i, int index_j, int index_k,
            int Type, float x_len, float y_len, float z_len, float x, float y, float z)
        {
            this.index = index;

            this.index_i = index_i;
            this.index_j = index_j;
            this.index_k = index_k;

            this.Type = Type;
            this.x_len = x_len;
            this.y_len = y_len;
            this.z_len = z_len;

            pos.x = x;//代表盒子区域最小的x，
            pos.y = y;//代表盒子区域最小的y，
            pos.z = z;//代表盒子区域最小的z，
        }

        //x,y交换一下
        public C_Box rotate90()
        {
            C_Box pNew = new C_Box(index, -Type,
                this.index_i, this.index_j, this.index_k,
                this.y_len, this.x_len, this.z_len, 0, 0, 0);
            return pNew;
        }

        public C_Box Clone()
        {
            C_Box pNew = new C_Box(index, -Type,
                this.index_i, this.index_j, this.index_k,
                this.x_len, this.y_len, this.z_len,pos.x,pos.y,pos.z);
            return pNew;
        }

        //x,y交换一下
        public C_Box rotate90_z()
        {
            C_Box pNew = new C_Box(Type + 1000, this.y_len, this.x_len, this.z_len, 0, 0, 0, 0, 0);
            return pNew;
        }


        //z,y交换一下
        public C_Box rotate90_x()
        {
            C_Box pNew = new C_Box(Type + 2000, this.x_len, this.z_len, this.y_len, 0, 0, 0, 0, 0);
            return pNew;
        }

        public C_Box rotate90_y()
        {
            C_Box pNew = new C_Box(Type + 2000, this.z_len, this.y_len, this.x_len, 0, 0, 0, 0, 0);
            return pNew;
        }

    }
}
