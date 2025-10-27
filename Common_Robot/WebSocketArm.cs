
using Common_Robot2;
using EmbedIO.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Common_Robot2
{
    /// <summary>
    /// 命令行工具
    /// </summary>
    public class WebSocketArm : WebSocketModule
    {
        //private readonly ConcurrentDictionary<IWebSocketContext, Process>
        //    processes = new ConcurrentDictionary<IWebSocketContext, Process>();


        private ConcurrentDictionary<IWebSocketContext, C_Check>
            rights = new ConcurrentDictionary<IWebSocketContext, C_Check>();

        private ConcurrentDictionary<IWebSocketContext, C_Command>
            commands = new ConcurrentDictionary<IWebSocketContext, C_Command>();

        //public FrmSet pFrmMain = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketArm"/> class.
        /// </summary>
        /// <param name="urlPath">路径</param>
        public WebSocketArm( string urlPath) //FrmSet pFrmMain,
            : base(urlPath, true)
        {
            //this.pFrmMain = pFrmMain;
        }

        /// <inheritdoc />
        protected override Task OnMessageReceivedAsync(
            IWebSocketContext context, byte[] buffer,
            IWebSocketReceiveResult result)
        {
            if (
                rights.TryGetValue(context, out C_Check? pCheck) &&
                commands.TryGetValue(context, out C_Command? pCommand))
            {
                try
                {
                    string cmd = Encoding.GetString(buffer);
                    pCommand.arm_process(pCheck, cmd);
                }
                catch (Exception ex)
                {
                    string msg = ex.ToString();
                    Console.WriteLine(ex?.Source?.ToString());
                    SendBufferAsync(pCheck, msg);
                }
            }
            return Task.CompletedTask;
        }


        public string read_ini(string key, string key2)
        {
            string file = C_SYS.Path_Ini + "\\data\\main.ini";
            string Path_Ini = Tools.ini_read(file, "main", "path_ini");
            string file2 = Path_Ini + "\\main.ini";

            key = "s_" + key.Replace(".", "_");
            string result = Tools.ini_read(file2, key, key2);// C_File.ini_read(file2, key,key2);
            return result;
        }

        public void On_Receive_Msg(C_Msg pMsg)
        {
            Console.WriteLine(pMsg.type + "," + pMsg.cur);
            string msg = JsonConvert.SerializeObject(pMsg);
            SendBufferAsync(pMsg.pCheck, msg);
        }

        public Task Broad_Msg(string strMsg)
        {
            return BroadcastAsync(strMsg);//, c => c != context);
        }



        /// <inheritdoc />
        //protected override Task OnClientConnectedAsync(IWebSocketContext context)
        //{
        //    //processes.TryAdd(context, process2);
        //    commands.TryAdd(context, new C_Command());
        //    rights.TryAdd(context, new C_Check());
        //    //process2.Start();
        //    //process2.BeginErrorReadLine();
        //    //process2.BeginOutputReadLine();
        //    return Task.CompletedTask;
        //}

        /// <inheritdoc />
        protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
        {
            if (rights.TryRemove(context, out var pCheck))
            {
                //if (!process.HasExited)
                //    process.Kill();

                //process.Dispose();
            }

            return Task.CompletedTask;
        }

        private IWebSocketContext FindContext(C_Check pCheck)
        {
            return rights.FirstOrDefault(kvp => kvp.Value == pCheck).Key;
        }

        public Task SendBufferAsync(C_Check pCheck, string buffer)
        {

            try
            {
                //if (pCheck.HasExited)
                //{
                //    return Task.CompletedTask;
                //}

                var context = FindContext(pCheck);
                return context?.WebSocket?.State == WebSocketState.Open
                    ? SendAsync(context, buffer)
                    : Task.CompletedTask;
            }
            catch
            {
                // ignore process teermination
                return Task.CompletedTask;
            }
        }

        internal void SendBufferAsync(object pCheck, string msg)
        {
            throw new NotImplementedException();
        }
    }
}