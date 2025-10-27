
using System.Text;
using Common_Robot2;

namespace Test1
{
    public class W_TCP_Client_Send_Wait : C_Node_Send
    {

        public string key_control = "";
        public string msg = "";
        public string encode = "GBK";
        public string decode = "GBK";
        public string base64 = "";//iAEABwAE
        public string hex = "";
        public string? key_save="";
        public string key_msg="";

        public W_TCP_Client_Send_Wait(string name,
            C_Space space_parent, C_Space space) :
            base(name,space_parent, space)
        {
        }



        public override Task run_sub()
        {
            run_sub_main();
            return Task.CompletedTask;
        }



        public static byte[] hext_to_byte(string hex)
        {
            byte[] b = new byte[hex.Length / 2];
            for (var i = 0; i < hex.Length; i += 2)
            {
                string a = hex.Substring(i, 2);
                b[i / 2] = Convert.ToByte(a, 16);
            }
            return b;
        }

        public void run_sub_main()
        {
            W_TCP_Client? pClient = (W_TCP_Client?)Main.get_node_from_parent_vars_steps(space, key_control);
            pClient.decode = this.read_string(this.decode);

            bool bConnected = false;

            for (var i = 0; i < pClient?.clients.Count; i++)
            {
                C_TcpClient client = pClient.clients[i];
                try
                {
                    if (client.client != null && client.client.Connected)
                    {
                        bConnected = true;
                    }
                }
                catch (Exception ex)
                {
                    Main.WriteLine(this,ex.ToString());
                    client.client.Close();
                }
            }


            if (bConnected == false)
            {
                pClient?.run_sub_main();
                Thread.Sleep(1000);
            }

            if (hex != "")
            {
                string hex2 = space.read_string(this,this.hex);
                hex2 = hex2.Replace(",", "");
                hex2 = hex2.Replace(" ", "");
                if (hex2 == "")
                {
                    Main.WriteLine("hex2=空！");
                    return;
                }
                byte[] gbk = hext_to_byte(hex2);
                var hex3 = BitConverter.ToString(gbk, 0);
                Main.WriteLine(hex3);
                pClient?.Send(this, gbk);
            }
            else if (base64 != "")
            {
                byte[] gbk = Tools.Base64Decode(base64);
                pClient?.Send(this, gbk);
            }
            else
            {
                if (this.key_msg == "")
                {
                    string msg = this.read_string(this.msg);
                    Main.WriteLine(msg);
                    byte[] gbk = Encoding.GetEncoding(encode).GetBytes(msg);
                    var hex = BitConverter.ToString(gbk, 0);
                    pClient?.Send(this, gbk);
                }
                else
                {
                    string msg = this.read_string("@"+this.key_msg);
                    Main.WriteLine(msg);
                    byte[] gbk = Encoding.GetEncoding(encode).GetBytes(msg);
                    var hex = BitConverter.ToString(gbk, 0);
                    pClient?.Send(this, gbk);
                }
            }

            bReceive = false;
            int iCount = 0;
            while (bReceive == false)
            {
                iCount++;
                Thread.Sleep(100);      //200*100=20s
                if (iCount > 2000)
                {
                    this.Next_Step = Node_Next.True;// False;
                    Main.WriteLine(this, " 等待超时！");
                    return;
                }
            }
            pClient?.nodes.Remove(this);

            if (key_save != "" && key_save != "none")
            {
                this.save_var(this.key_save, "string", this.receive_msg);
            }

            this.Next_Step = Node_Next.False;
            Main.WriteLine(this, " 运行完毕！");
        }


    }


}
