using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test1;

namespace palletizing
{
    public class C_Block_Data
    {
        public int x_max = 0;
        public int y_max = 0;
        public int z_max = 0;
        public int palletize_Width = 120;
        //public C_Method_New pMethod = new C_Method_New(null);

        public ArrayList pDNA_Pool = new ArrayList();
        public ArrayList pArrayMethod = new ArrayList();//所有的放置方法

        public List<C_Box> pArrayBoxs = new List<C_Box>();//需要放置的盒子
        public List<C_Box_Type> pArrayBoxs_Type = new List<C_Box_Type>();//需要放置的盒子的类型
        
        public List<C_Block_Simple> block_list_simple = new List<C_Block_Simple>();
        public List<C_Block_Complex> block_list_complex2 = new List<C_Block_Complex>();//几个简单方块组合而成
        public List<C_Block_Complex3> block_list_complex = new List<C_Block_Complex3>();

        public void rnd_box(int count, List<C_Box> pList)
        {
            Random rnd = new Random();
            for (var i = 0; i < count; i++)
            {
                var index = rnd.Next(pList.Count);
                C_Box pBox = pList[index];
                pArrayBoxs.Add(
                    new C_Box(pBox.Type, pBox.x_len, pBox.y_len, pBox.z_len, 0, 0, 0,0,0));
            }
        }



        public float get_max_x_len(List<C_Box> pArrayBoxs)
        {
            float max = 0;
            for(var i=0; i<pArrayBoxs.Count; i++)
            {
                C_Box item=pArrayBoxs[i];
                if (item.x_len > max)
                {
                    max=item.x_len;
                }
            }
            return max;
        }


        public string load_rnd_box()
        {
            this.pArrayBoxs_Type.Clear();
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "box file|*.box|all file|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                string str_type = "";
                string fileName = open.FileName;
                return File.ReadAllText(fileName);
            }
            return "";
        }

        public void init_space(string strSpace)
        {
            string[] strSplit = strSpace.Split(',');

            this.x_max = int.Parse(strSplit[0]);
            this.y_max = int.Parse(strSplit[1]);
            this.z_max = int.Parse(strSplit[2]);

        }






        public C_Block_Data Clone()
        {
            C_Box_Type box_Type;
            C_Block_Data data_new = new C_Block_Data();
            for(var i=0;i< block_list_simple.Count; i++)
            {
                box_Type = block_list_simple[i].pBox_Type;
                data_new.block_list_simple.Add(block_list_simple[i].Clone());
            }

            for (var i = 0; i < block_list_complex2.Count; i++)
            {
                box_Type = block_list_complex2[i].pBox_Type;
                data_new.block_list_complex2.Add(block_list_complex2[i].Clone());
            }


            for (var i = 0; i < pArrayBoxs_Type.Count; i++)
            {
                box_Type = pArrayBoxs_Type[i];
                data_new.pArrayBoxs_Type.Add(box_Type.Clone());
            }

            for (var i = 0; i < pArrayBoxs.Count; i++)
            {
                C_Box box = pArrayBoxs[i];
                data_new.pArrayBoxs.Add(box.Clone());
            }



            foreach (var item in dic_count){
                data_new.set_count(item.Key, item.Value);
            }
            return data_new;
        }



        public Dictionary<int, long> dic_count = new Dictionary<int, long>();
        public void set_count(int type_index, long count)
        {
            if (dic_count.ContainsKey(type_index))
            {
                dic_count[type_index]= count;
            }
            else
            {
                dic_count.Add(type_index, count);
            }
        }

        public long get_count(int type_index)
        {
            if (dic_count.ContainsKey(type_index))
            {
                return dic_count[type_index];
            }
            return 0;
        }

        public void Refresh_Type(C_Box_Type pBox_Type)
        {
            for (var i = 0; i < pBox_Type.总个数; i++)
            {
                C_Box pBox = new C_Box(pBox_Type.type_index, pBox_Type.x_len, pBox_Type.y_len, pBox_Type.z_len, 0, 0, 0, 0, 0);
                this.pArrayBoxs.Add(pBox);
                pBox.pBox_Type = pBox_Type;
            }
        }
    }
}
