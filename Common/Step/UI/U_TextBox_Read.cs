
using Common_Robot2;
using ConverxHull;

namespace Test1
{
    public class U_TextBox_Read : C_Node
    {
        public TextBox? pText1;
        public string key_ui = "";
        public string key_save = "textbox1_value";
        public string? value = "";
        public string? read_type = "string"; //double

        public U_TextBox_Read(string name,C_Space space_parent, C_Space space) : base(name,space_parent, space)
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
            S_TextBox? pItem = null;

            pItem = (S_TextBox?)Main.get_ui_from_parent_vars_steps(space, key_ui);
            if (pItem == null)
            {
                MessageBox.Show(this.Name + "UI没有这个控件");
                return;
            }
            pText1 = pItem?.pText1;
            Main.WriteLine(this," run");


            int count = 0;
            Action act = delegate ()
            {
                if (pText1 != null)
                {
                    value = pText1.Text;
                    if (read_type == "double")
                    {
                        if (value == "-") value = "0";
                        if (value == "") value = "0";
                        this.save_var(key_save, read_type, double.Parse(value));
                    }
                    else if (read_type.ToLower()== "C_Point3D".ToLower())
                    {
                        this.save_var(key_save, "C_Point3D",new C_Point3D(value,",",false));
                    }
                    else
                    {
                        this.save_var(key_save, read_type, value);
                    }
                    //call_next(pTrain);
                    count = 1000;
                }
            };
            if (pText1 != null)
            {
                pText1.BeginInvoke(act, null);
            }

            while (count < 300)
            {
                count++;
                Thread.Sleep(10);
            }
        }
    }
}
