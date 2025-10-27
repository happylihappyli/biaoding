using System.Net.Sockets;
using System.Threading;

namespace Test1
{
    public class C_TcpClient
    {
        public string Name = "";
        public TcpClient client;
        public NetworkStream clientStream;
        public Thread Thread;
    }
}