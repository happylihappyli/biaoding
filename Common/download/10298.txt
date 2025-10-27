using Common_Robot2;
using ConverxHull;

namespace Test1
{
    public class S_Cmd : C_Node
    {
        public string path = "";
        public string encode = "";
        public string command = "powershell.exe";//powershell.exe
        public ProcessLauncher launcher;

        public delegate void AppendConsoleText(string text);
        public AppendConsoleText callback_cmd;
        public E_Cmd_Event pEvent = null;

        public S_Cmd(string name, C_Space space_parent, C_Space space) :
            base(name,space_parent, space)
        {
        }
        public override void init()
        {

        }
        public override Task run_sub()
        {
            run_sub_main();
            return Task.CompletedTask;
        }

        private void run_sub_main()
        {
            launcher = new ProcessLauncher(this,this.path,this.encode);
            launcher.Start(this.command);

            callback_cmd = new AppendConsoleText(textBox1_AppendConsoleText);
        }

        private void textBox1_AppendConsoleText(string text)
        {
            if (pEvent == null)
            {
                Main.WriteLine(this, "pEvent == null");
                return;
            }
            pEvent.msg2 = text;
            //space.save_vars(pTrain, this, "%msg", "string",text);
            pEvent.Run(pTrain);// .run_sub2(text);
        }


        public void Send(string command)
        {
            if (launcher == null)
            {
                Main.WriteLine(this,"launcher == null");
                return;
            }
            launcher.SendInput(command + "\n");
        }
    }

    
}
