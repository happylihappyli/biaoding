
using Common_Robot2;
using ConverxHull;

namespace Test1
{
    public class String_To_Point : C_Node
    {

        public string line = "";
        public string key_read = "";//string
        public string key_save = "";//C_Point3D
        public string index = "0";

        public String_To_Point(string name, C_Space space_parent, C_Space space) : base(name,space_parent, space)
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
            string? line2 =this.read_string( line);

            if (this.key_read != "")
            {
                line2 = (string?)space.read_vars(pTrain, this, this.key_read, "string");
            }
            if (line2 == null)
            {
                return;
            }
            string[] strSplit = line2.Split(',');

            if (this.index == "")
            {
                this.index = "0";
            }
            int index_start = int.Parse(this.index);
            if (strSplit!=null &&  strSplit.Length> index_start+2)
            {
                float x = float.Parse(strSplit[index_start]);
                float y = float.Parse(strSplit[index_start+1]);
                float z = float.Parse(strSplit[index_start+2]);
                C_Point3D pPoint = new C_Point3D(x, y, z);
                this.save_var(key_save, "C_Point3D", pPoint);
            }
            else
            {
                Main.WriteLine(this,"格式不对！");
                MessageBox.Show(this.Name + " 格式不对!");

            }

        }
    }
}
