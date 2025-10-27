using Common_Robot2;
using ConverxHull;
using Microsoft.Data.Sqlite;

namespace Test1
{
    /// <summary>
    /// 日志 操作类
    /// gaojundz@qq.com
    /// </summary>
    class Logger:ILog
    {
        // 用于存放写日志任务的队列
        private Queue<Action> _queue;

        // 用于写日志的线程
        private Thread _loggingThread;

        // 用于通知是否有新日志要写的“信号器”
        private ManualResetEvent _hasNew;

        // 用于数据库查询日志文件
        private static SqLiteHelper sql_log;
        private C_Space space;

        // 构造函数，初始化。
        private Logger(C_Space space)
        {
            this.space = space;
            _queue = new Queue<Action>();
            _hasNew = new ManualResetEvent(false);

            Task.Run(() =>
            {
                InitLog();
                _loggingThread = new Thread(Process);
                _loggingThread.IsBackground = true;
                _loggingThread.Start();
            });
        }
        private void InitLog()
        {
            DateTime now = DateTime.Now;
            string file= C_Vars.log_path + "\\debug2_" +now.Year + "-" + now.Month + "-" + now.Day + ".db";//+ "_" + now.Hour 
            sql_log = new SqLiteHelper(C_Vars.log_path,"data source=" + file);// Application.StartupPath + "\\log.db");

            for(var i = 1; i <= 10; i++)
            {
                sql_log.CreateTable("class_" + i, new string[] { "Guid", "Content", "Level", "RecTime", "Source", "Author", "Remark" },
                                              new string[] { "TEXT PRIMARY KEY NOT NULL", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT", "TEXT" });
            }
        }


        // 使用单例模式，保持一个Logger对象
        private static Logger? _logger;// = new Logger();
        public static Logger GetInstance(C_Space space)
        {
            _logger = new Logger(space);
            _logger.space = space;
            return _logger;
        }

        // 处理队列中的任务
        private void Process()
        {
            while (true)
            {
                // 等待接收信号，阻塞线程。
                _hasNew.WaitOne();

                // 接收到信号后，重置“信号器”，信号关闭。
                _hasNew.Reset();

                // 由于队列中的任务可能在极速地增加，这里等待是为了一次能处理更多的任务，减少对队列的频繁“进出”操作。
                Thread.Sleep(3000);

                // 开始执行队列中的任务。
                // 由于执行过程中还可能会有新的任务，所以不能直接对原来的 _queue 进行操作，
                // 先将_queue中的任务复制一份后将其清空，然后对这份拷贝进行操作。

                Queue<Action> queueCopy;
                lock (_queue)
                {
                    queueCopy = new Queue<Action>(_queue);
                    _queue.Clear();
                }

                foreach (var action in queueCopy)
                {
                    try
                    {
                        action();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void Write(string content, Level_Enum level, string table, string source, string author, string remark,string strdt)
        {
            lock (_queue)
            { // todo: 这里存在线程安全问题，可能会发生阻塞。
                // 将任务加到队列
                try
                {
                    //_queue.Enqueue(() => (File.AppendAllText(filepath, content, System.Text.Encoding.Unicode)));
                    //insert into table1 values("12345678",1,"","","2018-10-10 10:34:23");
                    _queue.Enqueue(() => sql_log.InsertValues(table, 
                            new string[] { System.Guid.NewGuid().ToString("N"), content, level+"", strdt, source, author, remark}));
                }
                catch
                {
                }
            }

            // 打开“信号”
            _hasNew.Set();
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="level">等级</param>
        /// <param name="classify">分类</param>
        /// <param name="source">来源</param>
        /// <param name="author">作者</param>
        /// <param name="remark">备注</param>

        public void WriteLine(string content, Level_Enum level, string classify, string source = "", string author = "", string remark = "")
        {

            // WriteLog 方法只是向队列中添加任务，执行时间极短，所以使用Task.Run。

            string strdt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string table = getTable(classify);
            this.Write(content, level, table, source, author, remark, strdt);
        }



        private static string getTable(string classify)
        {
            if (classify == "") classify = "1";
            string table = "class_" + classify;
            return table;
        }
        public List<logData> ReadLog(DateTime startDate,DateTime endDate,string level,string classify)
        {
            string table = getTable(classify);
            SqliteDataReader reader = sql_log.ExecuteQuery("select * from " + table + " where RecTime>='" + startDate
                                                            + "' and RecTime<='" + endDate
                                                            + "' and Level='" + level
                                                            +"'");
            List<logData> lslogData = new List<logData>();
            while (reader.Read())
            {
                logData LogData = new logData();
                LogData.GUID = reader.GetString(reader.GetOrdinal("GUID"));
                LogData.Content = reader.GetString(reader.GetOrdinal("Content"));
                LogData.Level = reader.GetString(reader.GetOrdinal("Level"));
                LogData.RecTime = reader.GetString(reader.GetOrdinal("RecTime"));
                LogData.Source = reader.GetString(reader.GetOrdinal("Source"));
                LogData.Author = reader.GetString(reader.GetOrdinal("Author"));
                LogData.Remark = reader.GetString(reader.GetOrdinal("Remark"));
                LogData.GUID = reader.GetString(reader.GetOrdinal("GUID"));

                lslogData.Add(LogData);
            }
            reader.Close();
            reader.Dispose();

            return lslogData;
        }
        public List<logData> Read(string classify, int count, int page)
        {
            string table = getTable(classify);
            SqliteDataReader reader = sql_log.ExecuteQuery(
                                        "select * from " + table +
                                        " Order by RecTime DESC LIMIT " + count +
                                        " OFFSET " + page * count);

            List<logData> lslogData = new List<logData>();
            while (reader.Read())
            {
                logData LogData = new logData();
                LogData.GUID = reader.GetString(reader.GetOrdinal("GUID"));
                LogData.Content = reader.GetString(reader.GetOrdinal("Content"));
                LogData.Level = reader.GetString(reader.GetOrdinal("Level"));
                LogData.RecTime = reader.GetString(reader.GetOrdinal("RecTime"));
                LogData.Source = reader.GetString(reader.GetOrdinal("Source"));
                LogData.Author = reader.GetString(reader.GetOrdinal("Author"));
                LogData.Remark = reader.GetString(reader.GetOrdinal("Remark"));
                LogData.GUID = reader.GetString(reader.GetOrdinal("GUID"));

                lslogData.Add(LogData);
            }
            reader.Close();
            reader.Dispose();

            return lslogData;
        }

    }
}
