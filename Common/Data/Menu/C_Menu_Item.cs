using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test1
{
    public class C_Menu_Item
    {
        public string key = "";
        public string name = "";
        public C_Menu_Item parent=null;
        public List<C_Menu_Item> sub_items=new List<C_Menu_Item>();
        public ToolStripMenuItem menu_item;

        public C_Menu_Item(string key,string name) { 
            this.key = key;
            this.name = name;
        }
    }
}
