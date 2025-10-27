using Common_Robot2;
using System.Reflection;

namespace Test1
{
    public class S_Var_Set : C_Node
    {

        public string type = "";
        public string type_group = "";

        private string _key_save = "var1";
        public string key_save { get => _key_save; set => _key_save = value; }

        private string _value = "";
        public string value { get => _value; set => this._value = value; }

        public S_Var_Set(string name, C_Space space_parent,C_Space space):
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

        public void run_sub_main()
        {
            if (value!=null && value.StartsWith("@"))
            {
                string[] strSplit = value.Split("@");
                object? obj = this.read_var(strSplit[1], "");
                PropertyInfo? property = null;
                if (strSplit.Length > 2)
                {
                    if (obj != null)
                    {
                        Type? type = obj.GetType();
                        if (type != null)
                        {
                            property = type.GetProperty(strSplit[2]);
                        }
                    }
                }
                if (property != null)
                {
                    object? obj2 = property.GetValue(obj);
                    if (obj2 != null)
                    {
                        this.save_var(key_save, type, obj2);
                    }
                }
                else
                {
                    this.save_var(key_save, type, obj);
                }

            }
            else
            {
                if (type == "double")
                {
                    double? value_double = double.Parse(value);
                    this.save_var(key_save, type, value_double);
                }
                else
                {
                    this.save_var(key_save, "string", value);
                }
            }


        }
    }
}
