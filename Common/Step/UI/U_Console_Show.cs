using Common_Robot2;
using System.Text;

namespace Test1
{
    public class U_Console_Show : C_Node
    {
        public TextBox tb_console1;
        public TextBox tb_console2;
        public TextBox tb_console3;
        public string key_ui= "ui_key_xxx";
        public string path_log = "";

        Thread th_showLog;
        public bool canShow = true; 

        public U_Console_Show(string name, C_Space space_parent, C_Space space) : 
            base(name, space_parent, space)
        {
        }


        public override void init()
        {
            if (space.vars_ui.ContainsKey(key_ui)==false)
            {
                MessageBox.Show("日志显示关联的ui组件有问题");
            }
            S_LogBox pItem = (S_LogBox)space.vars_ui[this.key_ui];
            tb_console1 = pItem.pText1;
            tb_console2 = pItem.pText2;
            tb_console3 = pItem.pText3;
            //C_Vars.log_path = this.path_log;
        }



        public override Task run_sub()
        {
            run_sub_main();
            return Task.CompletedTask;
        }

        private void run_sub_main()
        {
            if (th_showLog == null)
            {
                th_showLog = new Thread(Func_ShowLog) { IsBackground = true };
                th_showLog.Start();
            }
        }

        public string list_to_string(List<logData> list)
        {
            StringBuilder sb = new StringBuilder();

            for(var i = list.Count-1; i>=0; i--)
            {
                var item = list[i];
                sb.AppendLine(item.RecTime.Substring(14)+" "+item.Content);
            }
            return sb.ToString();
        }

        private void Func_ShowLog()
        {
            Action act1 = delegate () {
                List<logData> list= space.console.pLog.Read("1", 200, 0);
                if (list.Count > 0)
                {
                    tb_console1.Text = list_to_string(list);
                    tb_console1.SelectionStart = tb_console1.Text.Length;
                    tb_console1.ScrollToCaret();
                }
            };
            Action act2 = delegate () {
                List<logData> list = space.console.pLog.Read("2", 200, 0);
                if (list.Count > 0)
                {
                    tb_console2.Text = list_to_string(list);
                    tb_console2.SelectionStart = tb_console2.Text.Length;
                    tb_console2.ScrollToCaret();
                }
            };
            Action act3 = delegate () {
                List<logData> list = space.console.pLog.Read("3", 200, 0);
                if (list.Count > 0)
                {
                    tb_console3.Text = list_to_string(list);
                    tb_console3.SelectionStart = tb_console3.Text.Length;
                    tb_console3.ScrollToCaret();
                }
            };


            while (space.vars.bClosingWindows == false)
            {
                if (canShow)
                {
                    try
                    {
                        if (space.console.bChange1 && tb_console1 != null)
                        {
                            space.console.bChange1 = false;
                            tb_console1.BeginInvoke(act1, null);
                        }
                        if (space.console.bChange2 && tb_console2 != null)
                        {
                            space.console.bChange2 = false;
                            tb_console2.BeginInvoke(act2, null);
                        }
                        if (space.console.bChange3 && tb_console3 != null)
                        {
                            space.console.bChange3 = false;
                            tb_console3.BeginInvoke(act3, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                Thread.Sleep(1000);
            }
        }
         
    }
}
