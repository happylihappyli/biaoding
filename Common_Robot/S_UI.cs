
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Common_Robot2
{
    public class S_UI : C_Node
    {
        public Form? pFrm = null;
        
        public string key_field = "";//要读取的变量
        public string key_parent = "";//要读取的变量


        public object? data = ""; //读取的变量数据

        public string color_bg = "";
        public string color_font = "";

        public string font_name = "";
        public string font_size = "";

        public string dock = "";
        public string padding = "";

        public string picture = "";

        public int x = 0;
        public int y = 0;
        public int width = 100;
        public int height = 50;



        public delegate void Update_Value_Handle();


        public event Update_Value_Handle? event_change_value;

        public override void init()
        {

        }
        public S_UI(string name, C_Space space_parent,C_Space space):base(name, space_parent,space)
        {
        }

        public override Task run_sub()
        {
            return Task.CompletedTask;
        }

        public void set_area(int x,int y,int width,int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public void update(Object? data)
        {
            this.data = data;
            if (event_change_value != null)
            {
                event_change_value();
            }
        }
    }
}
