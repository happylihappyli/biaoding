using Common_Robot2;
using System.Threading;

namespace Test1
{
    public class E_Button_Event : C_Node
    {
        public Button button;
        public Panel panel;
        public string text = "button1";
        
        private string _key_ui = "";

        public string key_ui { get => _key_ui; set => _key_ui = value; }

        public ParameterizedThreadStart hook = null;
        public E_Button_Event(string name, C_Space space_parent, C_Space space) : base(name,space_parent, space)
        {
        }

        public override void init()
        {
            if (key_ui == "")
            {
                MessageBox.Show(this.Name + "key_ui ==null ! " + Name);
                return;
            }
            if (space.vars_ui.ContainsKey(key_ui) == false)
            {
                MessageBox.Show(this.Name+ "没有这个按钮 key_ui=" + key_ui);
                return;
            }
            S_Button pItem = (S_Button)space.vars_ui[key_ui];

            button = pItem.button;
            if (button != null)
            {
                button.Click += Button_Click;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Main.WriteLine(this, " 事件激活 " + this.GetType().Name);
            if (hook != null)
            {
                Thread thread = new Thread(hook);
                Two_Space two = new Two_Space(space, space_parent);
                thread.Start(two);
                return;
            }

            if (this.tts == "%(调试.继续)%")
            {
                space.vars.debug_mode = false;
                space.step_stop = false;
            }

            if (this.tts == "%(显示调试窗口)%")
            {
                Main.pDebug.Show();
            }


            Random rnd = new Random();
            int train_id = rnd.Next(100, 900);
            pTrain = CommonMain.create_train("Button_" + train_id);

            Thread thread2 = new Thread(() =>
            {
                Run(pTrain);
            });
            thread2.Start();
        }

        public override Task run_sub()
        {
            return Task.CompletedTask;
        }
    }
}
