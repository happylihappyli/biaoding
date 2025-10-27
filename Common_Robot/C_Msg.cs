
using Newtonsoft.Json;

namespace Common_Robot2
{

    [JsonObject(MemberSerialization.OptIn)]
    public class C_Msg
    {
        public C_Check pCheck;

        [JsonProperty]
        public string type { get; set; }

        [JsonProperty]
        public string body { get; set; }

        [JsonProperty]
        public int cur { get; set; }

        [JsonProperty]
        public int max { get; set; }

        public C_Msg(
            C_Check pCheck,
            string type,
            string body,
            int cur, int max)
        {
            this.pCheck = pCheck;
            this.type = type;
            this.body = body;
            this.cur = cur;
            this.max = max;
        }
    }
}