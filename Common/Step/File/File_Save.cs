
using Common_Robot2;
using System.Text;

namespace Test1
{
    public class File_Save: C_Node
    {
        public string file = "";
        public string file_read = "";
        public string string_read = "";
        public string encode = "utf-8";
        public string key_time = "";

        public File_Save(string name, C_Space space_parent,C_Space space) : base(name, space_parent,space)
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
            string str_encode = space.read_string(this,this.encode);
            if (str_encode == "")
            {
                str_encode = "utf-8";
            }

            string file_save;
            if (this.file_read != "")
            {
                file_save = this.read_var_string(this.file_read);
            }
            else
            {
                file_save = space.read_string(this,this.file);
            }

            string time_id = space.read_string(this,this.key_time);

            file_save = file_save.Replace("${time}", time_id);

            string? path_dir = Path.GetDirectoryName(file_save);
            if (Directory.Exists(path_dir) == false)
            {
                Directory.CreateDirectory(path_dir);
            }
            using (FileStream fs_vector = new FileStream(file_save, FileMode.Create, FileAccess.Write))
            {
                lock (fs_vector)
                {
                    StreamWriter w_vector = new StreamWriter(fs_vector, Encoding.GetEncoding(str_encode));
                    string? strLine = this.read_var_string(this.string_read);
                    w_vector.Write(strLine);
                    w_vector.Close();
                }
            }
        }
    }

}
