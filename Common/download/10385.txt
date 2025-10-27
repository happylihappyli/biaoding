using Common_Robot2;

namespace Test1
{
    public class C_Node_Send : C_Node
    {
        public bool bReceive = false;
        public string receive_hex="";
        public byte[]? bytes;
        public string receive_msg;//收到的消息
        public string match_type="none";//要匹配的类型 {"type":"test","value":"1"}

        public C_Node_Send(string name,C_Space space_parent, C_Space space) : base(name, space_parent, space)
        {

        }

        public override void init()
        {
            if (receive_hex != "")
            {
                string hex2 = receive_hex;
                hex2 = hex2.Replace(",", " ");
                string[] strSplit = hex2.Split(" ");
                this.bytes = Main.ConvertHexStringToByteArray(strSplit);
            }
        }

        public override Task run_sub()
        {
            return Task.CompletedTask;
        }
    }
}