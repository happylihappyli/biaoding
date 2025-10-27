using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Common_Robot2
{
    public class TCPClient
    {
        /// <summary>
        /// 委托参数
        /// 1. 传输的数据
        /// 2. 状态，复数异常，0接受到数据，正数连接正常
        /// 3. 描述msg
        /// </summary>
        private Action<byte[], int, string>? RxRecived_CallbackFunc = null;
        Socket? socket;

        int ServerPort;
        IPEndPoint? remoteIpep;

        bool Status = false;
        bool Process = true;
        private Action<IAsyncResult>? TCP_Callback;

        public bool GetStatus()
        {
            return Status;
        }

        public bool GetSocketStatus()
        {
            if (socket == null)
            {
                return false;
            }

            return socket.Connected;
        }

        public TCPClient(Action<byte[], int, string> callBackFunc)
        {
            RxRecived_CallbackFunc = callBackFunc;
        }

        public TCPClient(Action<IAsyncResult> tCP_Callback)
        {
            this.TCP_Callback = tCP_Callback;
        }

        private void ThreadRXHY_SOCKET()
        {

            while (Process)
            {
                if (!Status)
                {
                    Status = InitConnect();
                }
                else
                {
                    //RXHYStatus = SendSocket("{\"message\":\"ping\"}");
                }

                Thread.Sleep(1000);
            }
        }

        public void InitSocket(string IP, int PORT)
        {
            //ServerIP = IP;
            ServerPort = PORT;
            remoteIpep = new IPEndPoint(IPAddress.Parse(IP), PORT); // 发送到的IP地址和端口号  
            
            Thread? thrRXHY = new Thread(ThreadRXHY_SOCKET);
            thrRXHY.Start();
        }

        private bool InitConnect()
        {
            bool bRet = false;
            try
            {
                {

                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//初始化一个Socket对象
                    
                    if (remoteIpep!=null)
                    {
                        socket.Connect(remoteIpep);
                        //连接指定IP&端口
                        //------------------------------------------
                        //labNET.Text = "TCP:√";
                        //labNET.BackColor = System.Drawing.Color.Lime;

                        //WriteLog("开启TCP通讯");
                        if (null != RxRecived_CallbackFunc)
                        {
                            RxRecived_CallbackFunc(new byte[] { 0x00 },
                                                    1,
                                                    string.Format("client connect success TCP")
                                                    );
                        }
                    }
                    
                }
                //-----------------------------------------
                Process = true;
                Thread? sck = new Thread(ThreadSocketRecv);
                sck.Start();
                bRet = true;
            }
            catch(Exception ex)
            {
                if (null != RxRecived_CallbackFunc)
                {
                    RxRecived_CallbackFunc(new byte[] { 0x00 },
                                            -1,
                                            string.Format("client connect fail:"+ex.ToString())
                                            );
                }
            }
            return bRet;
        }

        /// <summary>
        /// 监听客户端连接
        /// </summary>
        private void Func_ListenClient(object o)
        {
            Socket? server = o as Socket;
            Socket? remoteClient = server?.Accept();
            byte[]? buff = new byte[1024];
            while (true)
            {
                if (remoteClient?.Available > 0)
                {
                    int i = remoteClient.Receive(buff);
                    
                    RxRecived_CallbackFunc?.Invoke(buff.Take(i).ToArray(), 0, "接收到客户端数据");
                }
                Thread.Sleep(5);
                
            }
            
        }

        private void ThreadSocketRecv()
        {
            while (Process && socket!=null)//定义一个循环接收返回数据
            {
                try
                {
                    byte[] data = new byte[2048];
                    int len = socket.Receive(data);//接收返回数据
                    byte[] msg = new byte[len];
                    
                    Buffer.BlockCopy(data, 0, msg, 0, len);
                    //------------------------------
                    if (null != RxRecived_CallbackFunc)
                    {
                        RxRecived_CallbackFunc(msg,
                            0,
                            "receive data"
                            );
                    }
                }
                catch(Exception ex)
                {
                    //WriteLog("ThreadSocketRecv异常：" + ex.ToString());
                    if (null != RxRecived_CallbackFunc)
                    {
                        RxRecived_CallbackFunc(new byte[] { 0x00 },
                                                -2,
                                                "receive data fail:"+ex.ToString()
                                                );
                    }
                    Status = false;
                    break;
                }
                Thread.Sleep(5);
            }
        }

        public void Send(byte[] data)
        {
            if (Status && socket.Connected)
            {
                socket.Send(data);
            }
            else
            {
                Status = false;
            }
        }

        public void AsServerSend(byte[] data)
        {
            //remoteClient.Send(data);
        }
        public void Send(string data)
        {
            byte[] gbk = Encoding.GetEncoding("GBK").GetBytes(data);
            Send(gbk);
        }
        public void Clost()
        {
            socket.Close();
        }
    }
}
