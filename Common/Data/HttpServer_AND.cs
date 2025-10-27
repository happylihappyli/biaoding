using Common_Robot2;
using ConverxHull;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using System.Drawing;
using System.IO.Compression;
using Common_Robot;
using System.Collections.Generic;

namespace Visual_Robot6
{
    public class HttpProcessor
    {
        public TcpClient socket;
        public HttpServer srv;

        private Stream? inputStream;
        public NetworkStream? outStream;

        public string? http_method;
        public string? http_url;
        public string? http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();

        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        public HttpProcessor(TcpClient s, HttpServer srv)
        {
            this.socket = s;
            this.srv = srv;
        }


        private string streamReadLine(Stream? inputStream)
        {
            if (inputStream == null) {
                return "";
            }
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }
        public void process()
        {
            try
            {
                inputStream = new BufferedStream(socket.GetStream());
                outStream = socket.GetStream();
                // we probably shouldn't be using a streamwriter for all output from handlers either
                //outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
                parseRequest();
                readHeaders();
                if (http_method!=null && http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method != null && http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
                //outputStream.Flush();
                outStream.Close();
                // bs.Flush(); // flush any remaining output
                inputStream = null;
                //outputStream = null; // bs = null;
                outStream = null;
                socket.Close();
            }
            catch //(Exception e)
            {
                //Main.WriteLine("Exception: " + e.ToString());
                writeFailure();
            }
        }

        public void parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];

            //Main.WriteLine("starting: " + request);
        }

        public void readHeaders()
        {
            //Main.WriteLine("readHeaders()");
            String line;
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    //Main.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                //Main.WriteLine("header: {0}:{1}", name, value);
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            srv.handleGETRequest(this);
        }

        private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            //Main.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0 && this.inputStream!=null)
                {
                    //Main.WriteLine("starting Read, to_read={0}", to_read);

                    int n = Math.Min(BUF_SIZE, to_read);
                    int numread = this.inputStream.Read(buf, 0, n);

                    if (numread == 0)// || numread==null)
                    {
                        throw new Exception("client disconnected during post");
                    }
                    to_read -= numread;
                    //if (numread != null)
                    {
                        ms.Write(buf, 0, (int)numread);
                    }
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            //Main.WriteLine("get post data end");
            srv.handlePOSTRequest(this, new StreamReader(ms));

        }

        public void writeSuccess(string ContentType)
        {


            write_line("HTTP/1.0 200 OK");
            write_line("Content-Type: " + ContentType + ";charset=utf-8");
            write_line("Connection: close");
            //outputStream.WriteLine("Content-Encoding: gzip");
            write_line("");
        }

        public void write_line(string line)
        {
            byte[] data = Encoding.UTF8.GetBytes(line + "\n");
            outStream?.Write(data, 0, data.Length);
        }
        public void writeSuccess_gzip()
        {
            write_line("HTTP/1.0 200 OK");
            write_line("Content-Type: application/octet-stream");
            write_line("Connection: close");
            write_line("Content-Encoding: gzip");
            write_line("");
        }
        public void writeFailure()
        {
            write_line("HTTP/1.0 404 File not found");
            write_line("Connection: close");
            write_line("");
        }
    }

    public abstract class HttpServer
    {
        private C_Space space;
        protected int port;
        public TcpListener listener;
        public C_Node pNode;
        public bool bRet = false;
        public HttpServer(int port,C_Space space,C_Node pNode)
        {
            this.space = space;
            this.port = port;
            this.pNode = pNode;
        }

        public void listen()
        {
            if (listener == null)
            {
                IPAddress MyIP = IPAddress.Any;
                listener = new TcpListener(MyIP, port);
                listener.Start();

                Console.WriteLine("监听端口："+MyIP + ":" + port);
                while (space.vars.bClosingWindows==false)
                {
                    TcpClient s = listener.AcceptTcpClient();
                    HttpProcessor processor = new HttpProcessor(s, this);
                    Thread thread = new Thread(new ThreadStart(processor.process));
                    thread.Start();
                    Thread.Sleep(1);
                }
            }
        }

        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }

    public class MyHttpServer : HttpServer
    {
        public string path_tmp="D:/Test/tmp/";
        public string path_root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\web";


        string isLog = "0";
        StreamWriter log;
        private string ContentType;
        public C_Space space;

        public MyHttpServer(int port,C_Space space,C_Node pNode)
            : base(port,space,pNode)
        {
            this.space = space;
            IniFile myIni = new IniFile(Application.StartupPath + "\\Config.ini");
            isLog = myIni.ReadString("SYSTEM", "isLog", "0");

            CreateDirectory("HTTPlog");
            log = new StreamWriter("System\\HTTPlog\\HTTPlog" + DateTime.Now.ToString("_yyyyMMdd_HHmmss") + ".txt", true, Encoding.Unicode);
            WriteLog("HTTP服务器启动:" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            bRet = false;

        }
        private void CreateDirectory(string name)
        {
            if (!Directory.Exists("System\\" + name))
            {
                Directory.CreateDirectory("System\\" + name);  //创建目录
            }
        }

        private static IDictionary<string, string> _mimeTypeMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region extension to MIME type list
            {".asf", "video/x-ms-asf"},
            {".asx", "video/x-ms-asf"},
            {".avi", "video/x-msvideo"},
            {".bin", "application/octet-stream"},
            {".cco", "application/x-cocoa"},
            {".crt", "application/x-x509-ca-cert"},
            {".css", "text/css"},
            {".deb", "application/octet-stream"},
            {".der", "application/x-x509-ca-cert"},
            {".dll", "application/octet-stream"},
            {".dmg", "application/octet-stream"},
            {".ear", "application/java-archive"},
            {".eot", "application/octet-stream"},
            {".exe", "application/octet-stream"},
            {".flv", "video/x-flv"},
            {".gif", "image/gif"},
            {".hqx", "application/mac-binhex40"},
            {".htc", "text/x-component"},
            {".htm", "text/html"},
            {".html", "text/html"},
            {".ico", "image/x-icon"},
            {".img", "application/octet-stream"},
            {".iso", "application/octet-stream"},
            {".jar", "application/java-archive"},
            {".jardiff", "application/x-java-archive-diff"},
            {".jng", "image/x-jng"},
            {".jnlp", "application/x-java-jnlp-file"},
            {".jpeg", "image/jpeg"},
            {".jpg", "image/jpeg"},
            {".js", "application/x-javascript"},
            {".mml", "text/mathml"},
            {".mng", "video/x-mng"},
            {".mov", "video/quicktime"},
            {".mp3", "audio/mpeg"},
            {".mpeg", "video/mpeg"},
            {".mpg", "video/mpeg"},
            {".msi", "application/octet-stream"},
            {".msm", "application/octet-stream"},
            {".msp", "application/octet-stream"},
            {".pdb", "application/x-pilot"},
            {".pdf", "application/pdf"},
            {".pem", "application/x-x509-ca-cert"},
            {".pl", "application/x-perl"},
            {".pm", "application/x-perl"},
            {".png", "image/png"},
            {".prc", "application/x-pilot"},
            {".ra", "audio/x-realaudio"},
            {".rar", "application/x-rar-compressed"},
            {".rpm", "application/x-redhat-package-manager"},
            {".rss", "text/xml"},
            {".run", "application/x-makeself"},
            {".sea", "application/x-sea"},
            {".shtml", "text/html"},
            {".sit", "application/x-stuffit"},
            { ".svg","image/svg+xml"},
            {".swf", "application/x-shockwave-flash"},
            {".tcl", "application/x-tcl"},
            {".tk", "application/x-tcl"},
            {".txt", "text/plain"},
            {".war", "application/java-archive"},
            {".wbmp", "image/vnd.wap.wbmp"},
            {".wmv", "video/x-ms-wmv"},
            {".xml", "text/xml"},
            {".xpi", "application/x-xpinstall"},
            {".zip", "application/zip"},
            #endregion
        };

        public override void handleGETRequest(HttpProcessor p)
        {
            Console.WriteLine("请求：" + p.http_url);
            string[] strInfo = p.http_url.Split('?');
            string filename = strInfo[0];


            string file2 = filename.Replace("/", "\\"); ;
            if (filename == "/")
            {
                file2 = path_root + "\\index.html";
            }
            else
            {
                file2 = path_root + file2;
            }

            if (File.Exists(file2) == false)
            {
                p.writeSuccess("text/html");
                p.write_line("404");
                return;
            }


            if (file2.EndsWith(".js") || file2.EndsWith(".html"))
            {
                string pageData = File.ReadAllText(file2, Encoding.UTF8);
                string strBody2 = Tools.process_html(file2, pageData);

                //byte[] data = Encoding.UTF8.GetBytes(strBody2);

                if (file2.EndsWith(".html"))
                {
                    this.ContentType = "text/html";
                }
                else
                {
                    this.ContentType = "text/javascript";
                }
                p.writeSuccess(this.ContentType);
                p.write_line(strBody2);
            }
            else
            {
                Stream input = new FileStream(file2, FileMode.Open);

                //HttpListenerResponse response = ctx.Response;//响应

                string mime;
                this.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(file2), out mime) ? mime : "application/octet-stream";
                
                p.writeSuccess(this.ContentType);// "application/octet-stream");
                byte[] buffer = new byte[1024 * 16];
                int nbytes;
                while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    p.outStream?.Write(buffer, 0, nbytes);
                }
                input.Close();
            }

        }
        /// <summary>
        /// GZip压缩函数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] GZipCompress(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress))
                {
                    gZipStream.Write(data, 0, data.Length);
                    gZipStream.Close();
                }
                return stream.ToArray();
            }
        }
        public void WriteLog(string strLog)
        {
            if (isLog == "0") return;
            try
            {
                //log.WriteLine("[" + System.Environment.TickCount.ToString() + "]" + strLog);
                log.WriteLine("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "]" + strLog);
                log.Flush();
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }
        }
        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            //Main.WriteLine("POST request: {0}", p.http_url);
            string data = inputData.ReadToEnd();

            p.write_line("<html><body><h1>test server</h1>");
            p.write_line("<a href=/test>return</a><p>");
            p.write_line("postbody: <pre>" + data + "</pre>");


        }
    }
}
