using Test1;

namespace palletizing
{
    public class C_Block_Complex:I_Size
    {
        public List<C_Block_Simple> blocks=new List<C_Block_Simple>();

        public C_Box_Type pBox_Type;

        public int x = 0;
        public int y = 0;
        public int z = 0;
        public bool 用完=false;

        public C_Block_Space? remain_space = null; //剩下的空间，回字形，剩下中间的空间


        public C_Block_Complex(
            C_Box_Type box_Type,
            long x_len, long y_len, long z_len)
        {
            this.pBox_Type = box_Type;
            this.x_len = x_len;
            this.y_len = y_len;
            this.z_len = z_len;
        }

        public long count()
        {
            long block_count = 0;
            for(var i = 0; i < blocks.Count; i++)
            {
                block_count += blocks[i].count();
            }
            return block_count;
        }


        public long space()
        {
            return this.x_len * this.y_len * this.z_len;
        }

        public long space_used()
        {
            long used = 0;
            for(var i = 0; i < blocks.Count; i++)
            {
                used+=blocks[i].space_used();
            }
            return used;
        }

        public C_Block_Complex Clone()
        {
            C_Box_Type box_type = this.pBox_Type;
            C_Block_Complex p = new C_Block_Complex(box_type, x_len, y_len, z_len);
            for (var i = 0; i < blocks.Count; i++)
            {
                p.blocks.Add(blocks[i].Clone());  
            }
            if (remain_space!=null)
                p.remain_space = new C_Block_Space(remain_space.x_len, remain_space.y_len, remain_space.z_len, remain_space.x0, remain_space.y0, remain_space.z0);
            return p;
        }
    }
}