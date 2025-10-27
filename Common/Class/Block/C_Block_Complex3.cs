namespace palletizing
{
    public class C_Block_Complex3
    {
        public C_Block_Simple block;


        public C_Block_Complex3 block1;
        public C_Block_Complex3 block2;
        public int level = 0;
        public long m_x_len = 0;
        public long m_y_len = 0;
        public long m_z_len = 0;
        public long x = 0;
        public long y = 0;
        public long z = 0;
        public bool 用完;

        public long x_len
        {
            get { 
                if (block != null)
                {
                    return block.x_len;
                }
                return m_x_len; 
            }
            set { 
                m_x_len = value; 
            }
        }
        public long y_len
        {
            get
            {
                if (block != null)
                {
                    return block.y_len;
                }
                return m_y_len;
            }
            set
            {
                m_y_len = value;
            }
        }
        public long z_len
        {
            get
            {
                if (block != null)
                {
                    return block.z_len;
                }
                return m_z_len;
            }
            set
            {
                m_z_len = value;
            }
        }

        public C_Block_Complex3(C_Block_Simple b1)
        {
            this.block = b1;
            this.level = 1;
        }


        public C_Block_Complex3(C_Block_Complex3 b1, C_Block_Complex3 b2)
        {
            this.block1 = b1;
            this.block2 = b2;
            this.level=Math.Max(b1.level,b2.level)+1;
        }

        public long space()
        {
            return this.x_len*this.y_len*this.z_len;
        }

        public double contains()
        {
            if (this.block != null)
            {
                return this.block.space();
            }
            else
            {
                return this.block1.contains()+this.block2.contains();
            }
        }

        public void put_x()
        {
            this.x_len = block1.x_len + block2.x_len;
            this.y_len = Math.Max(block1.y_len, block2.y_len);
            this.z_len = Math.Max(block1.z_len, block2.z_len);

            block2.x = block1.x_len;
            block2.y = 0;
            block2.z = 0;
        }

        public void put_y()
        {
            this.x_len = Math.Max(block1.x_len, block2.x_len);
            this.y_len = block1.y_len + block2.y_len;
            this.z_len = Math.Max(block1.z_len, block2.z_len);

            block2.x = 0;
            block2.y = block1.y_len;
            block2.z = 0;
        }
        public void put_z()
        {
            this.x_len = Math.Max(block1.x_len, block2.x_len);
            this.y_len = Math.Max(block1.y_len, block2.y_len);
            this.z_len = block1.z_len + block2.z_len;

            block2.x = 0;
            block2.y = 0;
            block2.z = block1.z_len;
        }
    }
}