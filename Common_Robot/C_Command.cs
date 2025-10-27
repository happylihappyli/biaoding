
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Common_Robot2
{
    public class C_Command
    {
        public ConcurrentDictionary<string, C_Net>
            pDic_Net = new ConcurrentDictionary<string, C_Net>();

        C_Space space;
        public C_Command(C_Space space)
        {
            this.space = space;
        }

        public void On_Receive_Msg(C_Msg pMsg)
        {
            Console.WriteLine(pMsg.type + "," + pMsg.cur);
            string msg = JsonConvert.SerializeObject(pMsg);

            //pSocket.SendBufferAsync(pMsg.pCheck, msg);
        }


        private void password_process(
            C_Check pCheck,
            Process process,
            string cmd)
        {
            string[] strSplit = cmd.Split(' ');
            if (strSplit.Length > 2)
            {

                string data = "email=" + System.Web.HttpUtility.UrlEncode(strSplit[1])
                    + "&password=" + System.Web.HttpUtility.UrlEncode(strSplit[2]);
                //string url = "https://www.funnyai.com/login_check_md5.php";
                string content = "";// S_Net.http_post("", url, data, "POST", "utf-8", "");

                var strSplit2 = content.Split(';');

                if (strSplit2.Length > 1)
                {
                    var user_id = strSplit2[1];
                    if (strSplit2[0] == "登录成功")
                    {
                        pCheck.user = strSplit[1];
                        pCheck.check = true;
                        pCheck.user_id = Int32.Parse(user_id);
                        //string msg = "登录成功！";
                        //pSocket.SendBufferAsync(process, msg);
                    }
                    else
                    {
                        //string msg = "error check email password";
                        //pSocket.SendBufferAsync(process, msg);
                    }
                }
                else
                {
                    Console.WriteLine("content=");
                    Console.WriteLine(content);
                }
            }
            else
            {
                //string msg = "email md5";
                //pSocket.SendBufferAsync(process, msg);
            }
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



        public Task ftp_process(
            C_Net pNet,
            string cmd)
        {
            string[] strSplit = cmd.Split(' ');
            if (strSplit.Length > 5)
            {
                var hosts = strSplit[2];
                var port = int.Parse(strSplit[3]);
                var user = strSplit[4];

                string password_aes = read_ini(hosts, "password_aes");


                //var key = "funnyai.com";
                //AES_v2 p = new AES_v2(key);
                var password = "";// p.Decrypt(password_aes);

                var file_source = strSplit[5];
                var file_local = "";
                if (strSplit.Length > 6)
                {
                    file_local = strSplit[6];
                }

                switch (strSplit[1])
                {
                    case "download":
                        pNet.Event_SendMsg += new C_Net.SendMsg(On_Receive_Msg);
                        pNet.ftp_download(hosts, port, user, password,
                            "", file_source, file_local);
                        break;
                    case "upload":
                        pNet.Event_SendMsg += new C_Net.SendMsg(On_Receive_Msg);

                        pNet.ftp_upload(hosts, port, user, password,
                            "", file_source, file_local);
                        break;
                    case "list":
                        pNet.Event_SendMsg += new C_Net.SendMsg(On_Receive_Msg);
                        pNet.ftp_list(hosts, port, user, password, file_source);
                        break;
                }
            }

            return Task.CompletedTask;
        }


        public Task ssh_process(
            C_Net pNet,
            string cmd)
        {
            string[] strSplit = cmd.Split('.');
            if (strSplit.Length > 1)
            {

                switch (strSplit[1])
                {
                    case "connect":
                        strSplit = cmd.Split(' ');
                        if (strSplit.Length > 3)
                        {
                            var hosts = strSplit[1];
                            var port = int.Parse(strSplit[2]);
                            var user = strSplit[3];

                            string password_aes = read_ini(hosts, "password_aes");

                            //var key = "funnyai.com";
                            //AES_v2 p = new AES_v2(key);
                            var password = "";// p.Decrypt(password_aes);

                            pNet.ssh_connect(hosts, port, user, password);
                        }
                        break;
                    case "stream":
                        pNet.Send_Event("stream.start", "", 0, 0);
                        pNet.ssh_stream();
                        break;
                    case "send":
                        if (strSplit.Length > 3)
                        {
                            var cmd2 = cmd.Substring("ssh.send.".Length + strSplit[2].Length + 2);
                            pNet.ssh_stream_command(strSplit[2], cmd2);
                        }
                        else
                        {
                            pNet.Send_Event("send.error", "至少3个参数", 0, 0);
                        }
                        break;
                }
            }

            return Task.CompletedTask;
        }


        public void arm_process(C_Check pCheck, string cmd)
        {
            string[] strSplit = cmd.Split('.');
            if (strSplit.Length > 1)
            {
                var type = strSplit[1];

                switch (type)
                {
                    case "start":
                        {
                            //cs1.bAutoMode = true;
                            //pSocket.SendBufferAsync(pCheck, "{\"type\":\"message\",\"data\":\"已经启动！\"}\n");
                        }
                        break;
                    case "stop":
                        {
                            //cs1.bAutoMode = false;
                            //pSocket.SendBufferAsync(pCheck, "{\"type\":\"message\",\"data\":\"已经停止！\"}\n");
                        }
                        break;
                    case "next":
                        {
                            //C_Main.第一步抓取图片();
                            //pSocket.SendBufferAsync(pCheck, "{\"type\":\"message\",\"data\":\"点击下一步！\"}\n");
                        }
                        break;
                    case "rect":
                        {
                            //string line = "[[" + cs1.pRect1.pPoint1.x + "," + cs1.pRect1.pPoint1.y + "],["
                            //    + cs1.pRect1.pPoint2.x + "," + cs1.pRect1.pPoint2.y + "]]";

                            //pSocket.SendBufferAsync(pCheck, "{\"type\":\"rect\",\"data\":" + line + "}\n");
                        }
                        break;
                }
            }

        }


        private void sys_process(C_Node pNode,C_SYS pSYS, string cmd)
        {
            string[] strSplit = cmd.Split('.');
            if (strSplit.Length > 1)
            {
                var type = strSplit[1];
                switch (type)
                {
                    case "run":
                        {
                            var cmd2 = cmd.Substring(strSplit[0].Length + strSplit[1].Length + 2);
                            pSYS.run_cmd(cmd2);
                        }
                        break;
                    case "run_app":
                        {
                            var cmd2 = cmd.Substring(strSplit[0].Length + strSplit[1].Length + 2);
                            var strSplit2 = cmd2.Split(' ');
                            pSYS.run_app(pNode,strSplit2[0], strSplit2[1]);
                        }
                        break;
                }
            }
        }

        //public void process(
        //    WebSocketArm pSocket,
        //    C_Check pCheck,
        //    IWebSocketContext context, string cmd)
        //{
        //    this.pSocket = pSocket;

        //    if (cmd.StartsWith("ssh."))
        //    {
        //        C_Net pNet = null;
        //        if (pDic_Net.ContainsKey(pCheck.user))
        //        {
        //            pNet = pDic_Net[pCheck.user];
        //        }
        //        else
        //        {
        //            pNet = new C_Net(space);
        //            pDic_Net.TryAdd(pCheck.user, pNet);
        //            pNet.Event_SendMsg += new C_Net.SendMsg(On_Receive_Msg);
        //        }
        //        pNet.pCheck = pCheck;

        //        new Task(() =>
        //        {
        //            ssh_process(pNet, cmd);
        //        }).Start();
        //    }
        //    else if (cmd.StartsWith("sys."))
        //    {
        //        C_SYS pSYS = new C_SYS(space);
        //        pSYS.pCheck = pCheck;

        //        new Task(() =>
        //        {
        //            sys_process(pSYS, cmd);
        //        }).Start();
        //    }
        //    else if (cmd.StartsWith("arm."))
        //    {
        //        arm_process(pCheck, cmd);
        //    }
        //    else
        //    {
        //        //不管什么命令都变成cd
        //        //process.StandardInput.WriteLineAsync("cd");
        //    }
        //}
    }
}
