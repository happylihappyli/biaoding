
using Common_Robot2;
using System.Text;
using System.Xml.Linq;

namespace Test1
{
    public class File_Append : C_Node
    {
        public string file = "";
        public string key_read = "";

        public File_Append(string name, C_Space space_parent, C_Space space) :
            base(name, space_parent, space)
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

            lock (this.file)
            {
                string file_vector = this.read_string(this.file);


                FileStream fs_vector = new FileStream(file_vector, FileMode.Append, FileAccess.Write);
                StreamWriter w_vector = new StreamWriter(fs_vector, Encoding.UTF8);

                string? strLine = (string?)this.read_var(this.key_read, "string");
                w_vector.WriteLine(strLine);


                w_vector.Close();
                fs_vector.Close();
            }

        }
    }

}
