
using Common_Robot2;

namespace Test1
{
    public class S_Cmd_View : S_Panel
    {
        public ConsoleControl.ConsoleControl? ui_item;
        
        public string color1 = "white";
        public string color2 = "white";
        public string text="";

        public S_Cmd_View(string Name, C_Space space_parent, C_Space space):
            base(Name, space_parent, space)
        {
            this.Name = Name; 
        }

        public new void init()
        {
            space.vars_ui.TryAdd(this.key, this);

            ui_item = new ConsoleControl.ConsoleControl();
            ui_item.Location = new System.Drawing.Point(x, y);
            space.vars_ui["#ui_console"] = this;

            this.ui_item.Dock = DockStyle.Fill;
            this.ui_item.Location = new System.Drawing.Point(0, 0);
            this.ui_item.Size = new Size(800, 600);
            this.ui_item.TabIndex = 0;


            Action act = delegate ()
            {
                if (pFrm != null)
                {
                    Main.Add_Panel(this.pFrm, dock, this.group, space, ui_item);


                }
            };
            if (pFrm != null)
                pFrm.BeginInvoke(act, null);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            this.Run(pTrain);
        }

        public override Task run_sub()
        {
            return Task.CompletedTask;
        }
    }

}
