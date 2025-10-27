namespace Common_Robot2
{

    public struct logData
    {
        //"Guid", "Content", "Level", "RecTime", "Source", "Author", "Remark"
        public string GUID;
        public string Content;
        public string Level;
        public string RecTime;
        public string Source;
        public string Author;
        public string Remark;
    }
    public enum Level_Enum
        {
            Info,
            Warning,
            Error
        };

    public interface ILog
    {
        public void WriteLine(string content, Level_Enum level, string classify, 
            string source = "", string author = "", string remark = "");

        public List<logData> Read(string classify, int count, int page);
    }
}