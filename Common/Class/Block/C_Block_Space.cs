namespace palletizing
{

    /// <summary>
    /// 集装箱分割的一个空间
    /// </summary>
    public class C_Block_Space
    {
        public static int index_count = 0;
        public int index = 0;


        public long x_len = 0;
        public long y_len = 0;
        public long z_len = 0;
        
        public long x0 = 0;
        public long y0 = 0;
        public long z0 = 0;

        public bool 被占用 = false;
        public C_Block_Simple block_simple;
        public C_Block_Complex block;

        public void new_index()
        {
            this.index = C_Block_Space.index_count;
            C_Block_Space.index_count++;
        }


        /// <summary>
        /// space init
        /// </summary>
        /// <param name="line">x_len,y_len,z_len</param>
        public C_Block_Space(string line)
        {
            new_index();

            string[] strSplit=line.Split(',');
            if (strSplit.Length > 2)
            {
                this.x_len = long.Parse(strSplit[0]);
                this.y_len = long.Parse(strSplit[1]);
                this.z_len = long.Parse(strSplit[2]);
            }
        }


        public C_Block_Space(long x_len, long y_len, long z_len,
                long x0, long y0, long z0)
        {
            new_index();
            this.x_len = x_len;
            this.y_len = y_len;
            this.z_len = z_len;
            this.x0 = x0;
            this.y0 = y0;
            this.z0 = z0;
        }


        /// <summary>
        /// 放置积木
        /// </summary>
        /// <param name="block_data"></param>
        /// <param name="block"></param>
        /// <returns>true 代表放置成功 </returns>
        public bool 放置积木(C_Block_Data block_data, C_Block_Complex block)
        {
            if (block.x_len > this.x_len || block.y_len > this.y_len || block.z_len > this.z_len)
            {
                return false;
            }
            long sum1 = block.count(); //0;
            //for (var i = 0; i < block.blocks.Count; i++)
            //{
            //    C_Block_Simple sim= block.blocks[i];
            //    sum1 += sim.nx * sim.ny * sim.nz;
            //}

            int type_index = block.pBox_Type.type_index;
            long 总个数 = block_data.get_count(type_index);

            if (sum1 > 总个数)
            {
                return false;
            }
            this.被占用 = true;
            this.block = block;

            block_data.set_count(type_index, block_data.get_count(type_index) - sum1);

            for (var i = 0; i < block_data.block_list_complex2.Count; i++)
            {
                C_Block_Complex block2 = block_data.block_list_complex2[i];

                if (block2.pBox_Type == block.pBox_Type)
                {
                    if (block2.count() > 总个数)
                    {
                        block2.用完 = true;
                    }
                }
            }
            return true;
        }







        public bool 放置积木Simple(C_Block_Data block_data, C_Block_Simple block)
        {
            if (block.x_len > this.x_len || block.y_len > this.y_len || block.z_len > this.z_len)
            {
                return false;
            }
            long sum1 = block.count();

            int type_index = block.pBox_Type.type_index;
            long 总个数 = block_data.get_count(type_index);

            if (sum1 > 总个数)
            {
                return false;
            }
            this.被占用 = true;
            this.block_simple = block;

            block_data.set_count(type_index, block_data.get_count(type_index) - sum1);

            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        /// <param name="type"></param>
        /// <param name="next_z">下一批箱子的最大高度</param>
        /// <returns></returns>
        public List<C_Block_Space> 空间切割(I_Size block, int type,int next_z)
        {
            List<C_Block_Space> list = new List<C_Block_Space>();
            if (type == -1)
            {
                if (z_len - block.z_len > 0)
                {
                    list.Add(new C_Block_Space(x_len, y_len, z_len - block.z_len, this.x0, this.y0, this.z0 + block.z_len));
                }
                if (y_len - block.y_len > 0)
                {
                    list.Add(new C_Block_Space(block.x_len, y_len - block.y_len, block.z_len, this.x0, this.y0 + block.y_len, this.z0));
                }
                if (x_len - block.x_len > 0)
                {
                    list.Add(new C_Block_Space(x_len - block.x_len, y_len, block.z_len, this.x0 + block.x_len, this.y0, this.z0));
                }

            }
            else if (type == 1)
            {
                split1(list,next_z);
            }
            else if (type == 2)
            {
                split2(list);

            }
            else if (type == 3)
            {
                Random rand = new Random();
                if (rand.NextDouble() > 0.5)
                {
                    split1(list, next_z);
                }
                else
                {
                    split2(list);
                }
            }
            else if (type == 4)
            {
                List<long> pList = new List<long>();
                pList.Add(x_len - block.x_len);
                pList.Add(x_len);
                pList.Add(y_len);
                pList.Add(z_len);
                pList.Add(block.y_len);
                pList.Add(z_len - block.z_len);
                pList.Add(block.x_len);
                pList.Add(y_len - block.y_len);


                List<long> pList2 = new List<long>();
                pList2.Add(x_len - block.x_len);
                pList2.Add(x_len);
                pList2.Add(y_len);
                pList2.Add(z_len);
                pList2.Add(block.y_len);
                pList2.Add(z_len - block.z_len);
                pList2.Add(block.x_len);
                pList2.Add(y_len - block.y_len);


                pList.Sort(delegate (long x, long y)
                {
                    return Math.Sign(y - x);
                });
                pList2.Sort(delegate (long x, long y)
                {
                    return Math.Sign(y - x);
                });


                if (pList[0] < pList2[0])
                {
                    split1(list, next_z);
                }
                else
                {
                    split2(list);
                }
            }
            return list;
        }

        public void split1(List<C_Block_Space> list, int next_z)
        {
            if (block == null)
            {
                if (x_len - block_simple.x_len > 0)
                {
                    list.Add(new C_Block_Space(x_len - block_simple.x_len, y_len, z_len, this.x0 + block_simple.x_len, this.y0, this.z0));
                }

                //if (z_len- block_simple.z_len >= next_z) //高度大于下一拨的箱子的高度，这样切割才有意义
                //{
                //    if (y_len - block_simple.y_len > 0)
                //    {
                //        list.Add(new C_Block_Space(block_simple.x_len, y_len - block_simple.y_len, block_simple.z_len, this.x0, this.y0 + block_simple.y_len, this.z0));
                //    }
                //    if (z_len - block_simple.z_len > 0)
                //    {
                //        //list.Add(new C_Block_Space(block_simple.x_len, block_simple.y_len, z_len - block_simple.z_len, this.x0, this.y0, this.z0 + block_simple.z_len));
                //        list.Add(new C_Block_Space(block_simple.x_len, y_len, z_len - block_simple.z_len, this.x0, this.y0, this.z0 + block_simple.z_len));
                //    }
                //}
                //else
                {
                    if (y_len - block_simple.y_len > 0)
                    {
                        list.Add(new C_Block_Space(block_simple.x_len, y_len - block_simple.y_len, z_len, this.x0, this.y0 + block_simple.y_len, this.z0));
                    }
                    if (z_len - block_simple.z_len > 0)
                    {
                        list.Add(new C_Block_Space(block_simple.x_len, block_simple.y_len, z_len - block_simple.z_len, this.x0, this.y0, this.z0 + block_simple.z_len));
                        //list.Add(new C_Block_Space(block_simple.x_len, y_len, z_len - block_simple.z_len, this.x0, this.y0, this.z0 + block_simple.z_len));
                    }
                }
            }
            else
            {
                if (x_len - block.x_len > 0)
                {
                    list.Add(new C_Block_Space(x_len - block.x_len, y_len, z_len, this.x0 + block.x_len, this.y0, this.z0));
                }

                if (z_len - block.z_len > 0)
                {
                    list.Add(new C_Block_Space(block.x_len, block.y_len, z_len - block.z_len, this.x0, this.y0, this.z0 + block.z_len));
                }
                if (y_len - block.y_len > 0)
                {
                    list.Add(new C_Block_Space(block.x_len, y_len - block.y_len, z_len, this.x0, this.y0 + block.y_len, this.z0));
                }
            }
            
        }
        public void split2(List<C_Block_Space> list)
        {
            if (block == null)
            {
                if (y_len - block_simple.y_len > 0)
                {
                    list.Add(new C_Block_Space(x_len, y_len - block_simple.y_len, z_len, this.x0, this.y0 + block_simple.y_len, this.z0));
                }
                if (z_len - block_simple.z_len > 0)
                {
                    list.Add(new C_Block_Space(block_simple.x_len, block_simple.y_len, z_len - block_simple.z_len, this.x0, this.y0, this.z0 + block_simple.z_len));
                }
                if (x_len - block_simple.x_len > 0)
                {
                    list.Add(new C_Block_Space(x_len - block_simple.x_len, block_simple.y_len, z_len, this.x0 + block_simple.x_len, this.y0, this.z0));
                }
            }
            else
            {
                if (y_len - block.y_len > 0)
                {
                    list.Add(new C_Block_Space(x_len, y_len - block.y_len, z_len, this.x0, this.y0 + block.y_len, this.z0));
                }

                if (z_len - block.z_len > 0)
                {
                    list.Add(new C_Block_Space(block.x_len, block.y_len, z_len - block.z_len, this.x0, this.y0, this.z0 + block.z_len));
                }
                if (x_len - block.x_len > 0)
                {
                    list.Add(new C_Block_Space(x_len - block.x_len, block.y_len, z_len, this.x0 + block.x_len, this.y0, this.z0));
                }
            }
                
        }

        public long space()
        {
            return x_len * y_len * z_len;
        }
        public double space_mi()
        {
            return x_len/1000 * y_len / 1000 * z_len / 1000;
        }

        public override string ToString()
        {

            return x_len +","+ y_len + "," + z_len;
        }

        public C_Block_Space Clone()
        {
            return new C_Block_Space(this.x_len, this.y_len, this.z_len, this.x0, this.y0, this.z0);
        }

    }
}