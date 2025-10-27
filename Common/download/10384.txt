using System.Globalization;
using Test1;

namespace Common_Robot
{
    public class C_CallBack_Item
    {
        public string name = "";
        public string msg = "";
        public string hex = "";
        public byte[]? bytes;

        public C_CallBack_Item(string name, string msg, string hex)
        {
            this.name = name;
            this.msg = msg;

            //88 01 00 00 16

            if (hex != "")
            {
                this.hex = hex;
                string hex2 = hex;
                hex2 = hex2.Replace(",", " ");
                string[] strSplit = hex2.Split(" ");
                this.bytes = Main.ConvertHexStringToByteArray(strSplit);
            }
        }

    }
}