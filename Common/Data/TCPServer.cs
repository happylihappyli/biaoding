using Common_Robot2;
using ConverxHull;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Common_Robot
{
    public class TCPServer
    {
        private Action<byte[], int, string>? RxRecived_CallbackFunc = null;
        private TcpListener? Listener;
        private Thread? listenThread;

        private string? ip;
        private int port;

        List<TcpClient> pList=new List<TcpClient>();
        C_Space space;
        C_Node pNode;

        public TCPServer(C_Space space,C_Node pNode, Action<byte[], int, string> callBackFunc)
        {
            this.space = space;
            this.pNode = pNode;

            RxRecived_CallbackFunc = callBackFunc;
        }

        public void InitServer(string IP, int PORT)
        {
            this.ip = IP;
            this.port = PORT;

            Console.WriteLine(ip + ":" + port);
            try
            {
                if (Listener == null)
                {
                    Listener = new TcpListener(IPAddress.Parse(IP), PORT);
                    listenThread = new Thread(new ThreadStart(ListenForClients));
                    listenThread.Start();

                }
                else
                {
                    Console.WriteLine("已经启动");
                }
            }
            catch (Exception ex)
            {
                if (null != RxRecived_CallbackFunc)
                {
                    RxRecived_CallbackFunc(new byte[] { 0x00 },
                        -2,
                        ex.ToString());
                }
            }
        }



        public void ListenForClients()
        {
            try
            {
                Listener?.Start();
                while (true && Listener!=null)
                {
                    TcpClient client = Listener.AcceptTcpClient();
                    pList.Add(client);
                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(client);
                }
            }
            catch (Exception ex)
            {
                if (null != RxRecived_CallbackFunc)
                {
                    Console.WriteLine("错误1");
                    Console.WriteLine(ex.ToString());
                    RxRecived_CallbackFunc(new byte[] { 0x00 }, -2, ex.ToString());
                }
            }
        }

        public void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;

            NetworkStream clientStream = tcpClient.GetStream();


            if (null != RxRecived_CallbackFunc)
            {
                RxRecived_CallbackFunc(new byte[] { 0x00 },
                                        1,
                                        string.Format("Client @[{0}] connected @{1}", ((TcpClient)client).Client.RemoteEndPoint, DateTime.Now.ToString())
                                        );
            }

            int msglen = 4096;
            byte[] message = new byte[msglen];
            int bytesRead = 0;

            while (true)
            {
                bytesRead = 0;
                try
                {
                    if (clientStream.DataAvailable)
                    {
                        bytesRead = clientStream.Read(message, 0, msglen);
                        // Main.WriteLine("数据读取：" + bytesRead);
                    }
                    else
                    {
                        ;// Main.WriteLine("none");
                    }
                }
                catch (Exception ex)
                {
                    if (null != RxRecived_CallbackFunc)
                    {
                        Console.WriteLine("错误2：" + ex.Message);
                        RxRecived_CallbackFunc(new byte[] { 0x00 },
                            -2,
                            "Error:receive msg error"
                            );
                    }
                    break;
                }

                if (bytesRead == 0)
                {
                    if (null != RxRecived_CallbackFunc)
                    {
                        if (tcpClient.Connected == false)
                        {
                            RxRecived_CallbackFunc(new byte[] { 0x00 },
                                                    -1,
                                                    "the client has disconnected from the server"
                                                    );
                            break;
                        }
                    }
                }
                else
                {
                    ASCIIEncoding encoder = new ASCIIEncoding();
                    byte[] bData = new byte[bytesRead];
                    for (int i = 0; i < bytesRead; i++)
                    {
                        bData[i] = message[i];
                    }
                    if (null != RxRecived_CallbackFunc)
                    {
                        RxRecived_CallbackFunc(bData,
                            0,
                            "receive data"
                            );
                    }
                }
            }
        }

        public bool Send(byte[] bt, ref string message)
        {
            bool bRet = false;
            message = "";
            try
            {
                for(var i = 0; i < pList.Count; i++)
                {
                    TcpClient client = (TcpClient)pList[i];
                    if (client.Connected)
                    {
                        NetworkStream clientStream = client.GetStream();
                        clientStream.Write(bt, 0, bt.Length);
                        clientStream.Flush();
                        bRet = true;
                    }
                }
                for (var i = pList.Count-1; i>=0; i--)
                {
                    TcpClient? client = (TcpClient?)pList[i];
                    if (client?.Connected==false)
                    {
                        pList.Remove(client);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                bRet = false;
            }

            return bRet;
        }

    }
}
