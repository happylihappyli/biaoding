using Common_Robot2;

namespace Test1
{
    public class E_Cmd_Event : C_Node
    {
        public string key_step = "";
        public string key_msg = ""; //消息变量
        public S_Cmd? pCmd;
        public string msg2="";

        public E_Cmd_Event(string name, C_Space space_parent, C_Space space) : base(name,space_parent, space)
        {
        }

        public override void init()
        {
            if (key_step == "")
            {
                MessageBox.Show("key_step == null ! " + Name);
                return;
            }
            if (space.vars_step.ContainsKey(space.Name + key_step) == false)
            {
                MessageBox.Show("没有这个 key_step=" + key_step);
                return;
            }

            pCmd = (S_Cmd?)space.vars_step[space.Name + key_step];
            if (pCmd == null)
            {
                MessageBox.Show("pCmd ==  null");
            }
            else
            {
                pCmd.pEvent = this;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Main.WriteLine(this," 事件激活 " + this.GetType().Name);

            Random rnd = new Random();
            int train_id = rnd.Next(100, 900);
            pTrain = CommonMain.create_train("Cmd_" + train_id);

            Run(pTrain);
        }


        //public void run_sub2(string msg)
        //{
        //    try
        //    {
        //        run_sub_main(msg);
        //    }
        //    catch (Exception ex)
        //    {
        //        Main.WriteLine(this,ex.ToString());
        //    }
        //    //if (msg!=""){
        //    //    Console.WriteLine("train.id=" + pTrain.get_ID());
        //    //    call_next(pTrain);
        //    //}
        //}


        public override Task run_sub()
        {
            run_sub_main(this.msg2);
            return Task.CompletedTask;
        }

        private void run_sub_main(string msg)
        {
            Random random = new Random();
            pTrain = new C_Train("" + random.Next(100, 900));

            Main.WriteLine(this,"=>" + msg + " pTrain.ID=" + pTrain.get_ID());
            this.save_var(key_msg, "string", msg);
        }
    }
}
