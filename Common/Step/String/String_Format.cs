
using Common_Robot2;
using Newtonsoft.Json.Linq;

namespace Test1
{
    public class String_Format : C_Node
    {
        public string template = "";
        public string keys_read = "";
        public string key_save = "";
        public string list_read = "";

        public String_Format(string name, C_Space space_parent, C_Space space) : 
            base(name,space_parent, space)
        {
        }


        public override Task run_sub()
        {
            run_sub_main();
            return Task.CompletedTask;
        }

        public void run_sub_main()
        {

            string str_template = this.read_string(this.template);

            string value_all = str_template;
            if (this.list_read != "")
            {
                List<string>? list = (List<string>?)this.read_var(this.list_read, "");
                if (list == null)
                {
                    MessageBox.Show(this.Name + " list_read 读取的list为空！");
                    return;
                }

                value_all = "";
                for (var i = 0; i < list.Count; i++)
                {
                    string? value = list[i];
                    string line = str_template;
                    if (value != null)
                    {
                        line = line.Replace("{0}", value);
                        value_all += line + "\r\n";
                    }
                    else
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            else
            {
                string[] strSplit = keys_read.Split(',');

                for (var i = 0; i < strSplit.Length; i++)
                {
                    string? value = (string?)this.read_string("@" + strSplit[i]);
                    if (value != null)
                        value_all = value_all.Replace("{" + i + "}", value);
                }
            }
            

            value_all = value_all.Replace("{year}", DateTime.Now.ToString("yyyy")); 
            value_all = value_all.Replace("{month}",DateTime.Now.ToString("MM"));
            value_all = value_all.Replace("{day}", DateTime.Now.ToString("dd"));
            value_all = value_all.Replace("{hour}", DateTime.Now.ToString("HH"));
            value_all = value_all.Replace("{minute}", DateTime.Now.ToString("mm"));
            value_all = value_all.Replace("{second}", DateTime.Now.ToString("ss"));

            this.save_var(this.key_save, "string", value_all);
        }

        public override void init()
        {

        }
    }
}
