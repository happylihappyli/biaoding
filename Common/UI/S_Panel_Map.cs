using Common_Robot2;
using ConverxHull;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test1
{
    public class S_Panel_Map : S_UI
    {
        public Panel panel;
        public string dock_rank = "";
        public S_Panel_Map(string name, C_Space space_parent, C_Space space) : 
            base(name, space_parent, space)
        {
        }

        public override Task run_sub()
        {
            return Task.CompletedTask;
        }

        public override void init()
        {
            space.vars_ui.TryAdd(this.key, this);
        }
    }
}
