using palletizing;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace container_loading
{
    public class C_Block_Loading_Run
    {
        public C_Block_Loading_Run()//C_Main pMain)
        {
        }



        public int comparison2(C_Block_Complex x, C_Block_Complex y)
        {
            double a = x.space_used();// x.x_len * x.y_len * x.z_len;
            double b = y.space_used(); //y.x_len * y.y_len * x.z_len;

            return Math.Sign(b-a);
        }


        public bool 是否有更多盒子(C_Block_Data block_data)
        {

            List<C_Block_Complex> pList = block_data.block_list_complex2;

            pList.Sort(comparison2);

            for (var i = 0; i < pList.Count; i++)
            {
                C_Block_Complex block = pList[i];
                long count = block_data.get_count(block.pBox_Type.type_index);
                if (count == 0) continue;
                return count>0;
            }
            return false;
        }



        /// <summary>
        /// 放置复杂block
        /// </summary>
        /// <param name="pMain"></param>
        /// <param name="space_root"></param>
        /// <param name="first_index"></param>
        /// <param name="second_index"></param>
        /// <param name="split_type"></param>
        /// <param name="path_output"></param>
        /// <returns></returns>
        public (List<C_Block_Space> space_list,double space_remain_all, 
            int first_index, int second_index)
            测试这组策略(
            C_Block_Data block_data,
            C_Block_Space space_root,
            int first_index,
            int second_index,
            int split_type)
        {
            List<C_Block_Space> space_list = new List<C_Block_Space>
            {
                space_root
            };
            block_data.block_list_complex2.Sort(comparison2);

            long space_taken = 0;//占用的空间
            long space_remain_all = long.MaxValue;

            C_Block_Complex complex_block;
            complex_block = block_data.block_list_complex2[first_index];
            (space_taken, space_remain_all) = 选择空间放置积木(space_list, space_root, block_data, complex_block, space_taken, space_remain_all, split_type);

            complex_block = block_data.block_list_complex2[second_index];
            (space_taken, space_remain_all) = 选择空间放置积木(space_list, space_root, block_data, complex_block, space_taken, space_remain_all, split_type);

            for (var i = 0; i < block_data.block_list_complex2.Count; i++)
            {
                complex_block = block_data.block_list_complex2[i];
                (space_taken, space_remain_all) = 选择空间放置积木(space_list, space_root,block_data, complex_block, space_taken, space_remain_all, split_type);
            }
            return (space_list,space_remain_all, first_index, second_index);
        }





        private (long space_taken, long space_remain_all) 
            选择空间放置积木(List<C_Block_Space> 需要放置的空间, 
            C_Block_Space space_root,C_Block_Data block_data, C_Block_Complex complex_block,
            long space_taken, long space_remain_all, int split_type)
        {
            if (complex_block.用完) return (space_taken, space_remain_all);

            double waist_min = double.MaxValue;

            C_Block_Space space_selected = null;

            for (var j = 0; j < 需要放置的空间.Count; j++)
            {
                C_Block_Space space = 需要放置的空间[j];
                if (space.被占用) continue;

                double waist = space.space() - complex_block.space_used();
                if (waist < waist_min)
                {
                    if (complex_block.x_len <= space.x_len &&
                       complex_block.y_len <= space.y_len &&
                       complex_block.z_len <= space.z_len)
                    {
                        waist_min = waist;
                        space_selected = space;
                    }
                }
            }


            if (space_selected != null)
            {
                if (space_selected.放置积木(block_data, complex_block))
                {
                    space_taken += complex_block.space_used();
                    List<C_Block_Space> 剩余空间 = space_selected.空间切割(complex_block, split_type,100);
                    if (complex_block.remain_space != null)
                    {
                        var s = complex_block.remain_space;
                        剩余空间.Add(new C_Block_Space(s.x_len,s.y_len,s.z_len, space_selected.x0+s.x0, space_selected.y0 + s.y0, space_selected.z0 + s.z0));
                    }
                    for (var k = 0; k < 剩余空间.Count; k++)
                    {
                        C_Block_Space space_remain = 剩余空间[k];
                        需要放置的空间.Add(space_remain);
                    }
                    space_remain_all = (space_root.space() - space_taken);
                }
            }
            return (space_taken, space_remain_all);
        }
    }
}
