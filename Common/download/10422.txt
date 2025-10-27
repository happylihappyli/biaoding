using Common_Robot2;
using dll_ssh;
using Newtonsoft.Json.Linq;

namespace Test1
{
    public class S_SSH : C_Node
    {
        public string ip  = "";
        public string port = "";
        public string user = "";
        public string str_commands  = "";
        public string file  = "";
        public string password = "";

        public c_ssh p_ssh;

        public S_SSH(string name, C_Space space_parent, C_Space space) :
            base(name,space_parent, space)
        {
            p_ssh = new c_ssh();
        }
        public override void init()
        {

        }
        public override Task run_sub()
        {
            var ip_new = this.read_string(this.ip);
            var port_new = this.read_string(this.port);
            var user_new = this.read_string(this.user);
            var file_new = this.read_string(this.file);
            var password_new = this.read_string(this.password);

            //c_string pString = new c_string();
            //var pass1 = pString.default_password;
            //string password_aes = File.ReadAllText(file_new);
            //var str_password = pString.aes_decrypt(password_aes, pass1);

            p_ssh.connect(ip_new, port_new, user_new, password_new, "");


            p_ssh.stream();


            JArray pArray = JArray.Parse(str_commands);
            for (var i = 0; i < pArray.Count; i++)
            {
                JObject? item = (JObject?)pArray[i];
                if (item != null)
                {
                    if (item.ContainsKey("expect"))
                    {
                        string? cmd = item.GetValue("cmd")?.ToString();

                        string cmd_new = this.read_string(cmd);

                        string? expect = item?.GetValue("expect")?.ToString();
                        string expect_new = this.read_string(expect);

                        string? wait = item?.GetValue("wait")?.ToString();
                        string wait_new = this.read_string(wait);

                        p_ssh.stream_expect("",
                            expect_new,
                            cmd_new,
                            int.Parse(wait_new));
                    }
                    else
                    {
                        string? cmd = item?.GetValue("cmd")?.ToString();
                        if (cmd == "@cmd2")
                        {
                            Main.WriteLine(this,"error");
                        }
                        string cmd_new = this.read_string(cmd);

                        p_ssh.stream_command("cmd", cmd_new);
                    }
                }
            }

            p_ssh.stream_result(2);

            return Task.CompletedTask;
        }

    }

    
}
