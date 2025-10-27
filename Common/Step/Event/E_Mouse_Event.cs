using Common_Robot;
using Common_Robot2;
using ConverxHull;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Test1;

namespace Test1
{
    public class E_Mouse_Event : C_Node
    {
        public PictureBox picture;

        public string event_type = "";
        public string key_save = "mouse_down";
        public string key_ui = "key_ui";
        public MouseEventArgs mouse;
        public int rank = 0;

        public E_Mouse_Event(string name, C_Space space_parent, C_Space space) :
            base(name,space_parent, space)
        {
        }

        public override void init()
        {
            if (space.vars_ui.ContainsKey(key_ui) == false)
            {
                MessageBox.Show(Name + "需要设置key_ui");
                return;
            }
            S_PictureBox pItem = (S_PictureBox)space.vars_ui[key_ui];
            picture = pItem.picture_box;
            if (picture == null) return;
            switch (event_type)
            {
                case "mouse_down":
                    picture.MouseDown += new MouseEventHandler(mouse_event);
                    break;
                case "mouse_move":
                    picture.MouseMove += new MouseEventHandler(mouse_event);
                    break;
                case "mouse_up":
                    picture.MouseUp += new MouseEventHandler(mouse_event);
                    break;
            }
        }

        private void mouse_event(object sender, MouseEventArgs e)
        {
            mouse = e;

            Main.WriteLine(this, Name + " 事件激活 " + this.GetType().Name);

            Random rnd = new Random();
            int train_id = rnd.Next(100, 900);
            pTrain = CommonMain.create_train("Mouse_" + train_id);

            Run(pTrain);
        }

        public override Task run_sub()
        {
            run_sub_main();
            return Task.CompletedTask;
        }

        private void run_sub_main()
        {
            if (mouse != null)
            {
                this.save_var(key_save, "C_Point3D", new C_Point3D(mouse.X, mouse.Y, 0));
            }
        }
    }
}
