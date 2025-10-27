using Newtonsoft.Json.Linq;

namespace Test1
{
    public static class JSON_Tools
    {
        public static string? read(this JObject obj,string key)
        {
            if (obj.ContainsKey(key))
            {
                return obj[key]?.ToString();
            }
            else
            {
                return "";
            }
        }
        public static int read_int(this JObject obj, string key)
        {
            if (obj.ContainsKey(key))
            {
                string? line = obj[key]?.ToString();
                if (line == null)
                {
                    return 0;
                }
                return int.Parse(line);
            }
            else
            {
                return 0;
            }
        }
    }
}
