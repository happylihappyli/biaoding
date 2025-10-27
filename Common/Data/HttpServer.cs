using Common_Robot;
using Common_Robot2;
using ConverxHull;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Test1;

namespace Funny
{
    public class HttpServer
    {

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

        public ArrayList pCallBack = new ArrayList();
        public int port;
        public string path_root = "D:\\Data\\www\\";
        public string path_tmp = "D:\\Test\\";

        public C_Node pNode;
        public C_Space space;
        public bool runServer = true;
        public string url = "";
        public HttpListener listener;
        public int pageViews = 0;
        public int requestCount = 0;
        public string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";

        public HttpServer(int iPort,C_Space space,C_Node pNode)
        {
            this.port = iPort;
            this.space = space;
            this.pNode = pNode;
        }
        public async Task HandleIncomingConnections()
        {

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                if (req!= null)
                {
                    string file = req.Url.AbsolutePath;
                    // Print out some info about the request
                    Console.WriteLine("Request #: {0}", ++requestCount);
                    Console.WriteLine(req.Url.ToString());
                    Console.WriteLine(req.HttpMethod);
                    Console.WriteLine(req.UserHostName);
                    Console.WriteLine(req.UserAgent);
                    Console.WriteLine(file);
                    //Main.WriteLine();
                }




            }
        }

        private void Save_Label(string vFile, string strType, string strposition)
        {
            string file = path_root + "\\" + vFile;
            string strContent = File.ReadAllText(file);

            Random p = new Random();
            int id = 1000 + p.Next(1, 1000000);
            string file2 = this.path_tmp + "\\" + id + ".txt";
            while (File.Exists(file2))
            {
                id = 1000 + p.Next(1, 1000000);
                file2 = this.path_tmp + "\\" + id + ".txt";
            }

            File.WriteAllText(file2, strContent);
            File.WriteAllText(file2 + ".tag.txt", strType + "," + strposition);
        }


        private static void AddFireWallPort(int port)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.StandardInput.WriteLine(@"
@echo Enabling SQLServer default instance port " + port + @"
netsh advfirewall firewall add rule name =""http"" dir =in action = allow protocol = TCP localport = " + port + @"
@echo Enabling Dedicated Admin Connection port " + port);
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
            Main.WriteLine(process.StandardOutput.ReadToEnd());
            //Console.ReadKey();
            //MessageBox.Show("Succesful!", "Succesful!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        public void Listen()
        {
            AddFireWallPort(port);

            //如果有权限问题，参考： https://www.cpming.top/p/httplistener-access-denied
            listener = new HttpListener();
            listener.Prefixes.Add("http://127.0.0.1:" + port + "/");
            listener.Prefixes.Add("http://+:" + port + "/");
            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                if (MessageBox.Show("启动有错误，需要管理员权限启动！要查看更多帮助信息吗？", "",MessageBoxButtons.YesNo,MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    Process.Start("https://www.cpming.top/p/httplistener-access-denied");
                    MessageBox.Show("如果有权限问题，参考： https://www.cpming.top/p/httplistener-access-denied");
                    MessageBox.Show(ex.ToString());
                }
                return;
            }

            while (true)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    Process_Request(context);
                }
                catch (Exception ex)
                {
                    Main.WriteLine(ex.ToString());
                    //break;
                }
            }
        }


        private void Process_Request(
            HttpListenerContext context)
        {
            if (context == null) return;

            string? file = context?.Request?.Url?.AbsolutePath;
            Main.WriteLine(file);
            file = file?.Substring(1);

            string filename = Path.Combine(path_root, file==null?"":file);


            var ctx = context;
            var req = context?.Request;
            var resp = context?.Response;

            if (req.HttpMethod == "GET")
            {
                if (File.Exists(filename) == false)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.OutputStream.Close();
                    return;
                }


                string file2 = filename.Replace("/", "\\"); ;
                if (file == "/")
                {
                    file2 = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\index.html";
                }
                else if (file == "/color.html")
                {
                    file2 = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\color.html";
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (req?.Url?.AbsolutePath != "/favicon.ico")
                    pageViews += 1;

                if (File.Exists(file2))
                {
                    if (file2.EndsWith(".js") || file2.EndsWith(".html"))
                    {
                        pageData = File.ReadAllText(file2, Encoding.UTF8);
                        string strBody2 = Tools.process_html(file2, pageData);

                        byte[] data = Encoding.UTF8.GetBytes(strBody2);

                        if (file2.EndsWith(".html"))
                        {
                            resp.ContentType = "text/html";
                        }
                        else
                        {
                            resp.ContentType = "text/javascript";
                        }
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = data.LongLength;
                        resp.OutputStream.Write(data, 0, data.Length);
                        resp.Close();
                    }
                    else
                    {
                        Stream input = new FileStream(file2, FileMode.Open);

                        HttpListenerResponse response = ctx.Response;//响应

                        string? mime;
                        response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(file2), out mime) ? mime : "application/octet-stream";
                        response.ContentLength64 = input.Length;
                        response.AddHeader("Date", DateTime.Now.ToString("r"));
                        response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(file2).ToString("r"));

                        byte[] buffer = new byte[1024 * 16];
                        int nbytes;
                        while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                            response.OutputStream.Write(buffer, 0, nbytes);
                        input.Close();

                        response.StatusCode = (int)HttpStatusCode.OK;
                        response.OutputStream.Flush();
                    }
                }
                else
                {
                    pageData = "404";
                    byte[] data = Encoding.UTF8.GetBytes(pageData);
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                    resp.OutputStream.WriteAsync(data, 0, data.Length);
                    resp.Close();
                }
            }
            else if (req.HttpMethod == "POST")
            {
                Dictionary<string, string> postParams = new Dictionary<string, string>();
                foreach (string key in ctx.Request.QueryString)
                {
                    postParams.Add(key, ctx.Request.QueryString.Get(key));
                }
                for (var i = 0; i < pCallBack.Count; i++)
                {
                    C_CallBack_Item item = (C_CallBack_Item)pCallBack[i];
                    if (req?.Url?.AbsolutePath == item.msg)
                    {
                        C_Node pStep = space.vars_step[space.Name+ item.name];

                        space.save_vars(pStep.pTrain,pNode, "sys_post", "Dictionary<string, string>", postParams);

                        HttpListenerResponse? that = resp;
                        //Task.Run(() =>
                        {
                            pStep.Run(null);

                            while (pStep.pTrain.get_Vars().ContainsKey("http_result") == false)
                            {
                                Thread.Sleep(10);
                            }
                            string? pageData = (string?)pStep?.pTrain?.get_Vars()?["http_result"]?.get_obj();

                            byte[] data2 = Encoding.UTF8.GetBytes(pageData);// String.Format(pageData, pageViews, disableSubmit));
                            that.ContentType = "text/html";

                            that.ContentEncoding = Encoding.UTF8;
                            that.ContentLength64 = data2.LongLength;

                            that.OutputStream.WriteAsync(data2, 0, data2.Length);
                            that.Close();
                        }
                        //);
                    }
                }

                //if (req.Url.AbsolutePath == "/save")
                //{
                //    string text;
                //    using (var reader = new StreamReader(
                //        ctx.Request.InputStream, ctx.Request.ContentEncoding))
                //    {
                //        text = reader.ReadToEnd();
                //        Main.WriteLine(text);
                //    }
                //    string[] rawParams = text.Split('&');
                //    foreach (string param in rawParams)
                //    {
                //        string[] kvPair = param.Split('=');
                //        string key = kvPair[0];
                //        string value = HttpUtility.UrlDecode(kvPair[1]);
                //        postParams.Add(key, value);
                //    }
                //    string strType = postParams["type"];
                //    string strposition = postParams["position"];
                //    Save_Label("points_web.xyz.txt", strType, strposition);

                //    string pageData = "保存成功！";

                //    byte[] data2 = Encoding.UTF8.GetBytes(pageData);// String.Format(pageData, pageViews, disableSubmit));
                //    resp.ContentType = "text/html";

                //    resp.ContentEncoding = Encoding.UTF8;
                //    resp.ContentLength64 = data2.LongLength;

                //    // Write out to the response stream (asynchronously), then close it
                //    resp.OutputStream.WriteAsync(data2, 0, data2.Length);
                //    resp.Close();
                //}
                //runServer = false;
            }

            //string filename = context.Request.Url.AbsolutePath;
            //Main.WriteLine(filename);
            //filename = filename.Substring(1);

            //filename = Path.Combine(root_path, filename);



            //if (context.Request.HttpMethod == "POST")
            //{
            //    string text;
            //    using (var reader = new StreamReader(
            //        context.Request.InputStream, context.Request.ContentEncoding))
            //    {
            //        text = reader.ReadToEnd();
            //        Main.WriteLine(text);
            //    }
            //    string[] rawParams = text.Split('&');
            //    foreach (string param in rawParams)
            //    {
            //        string[] kvPair = param.Split('=');
            //        string key = kvPair[0];
            //        string value = System.Web.HttpUtility.UrlDecode(kvPair[1]);
            //        postParams.Add(key, value);
            //    }
            //}

            //string user = "";
            //if (postParams.ContainsKey("sys_user"))
            //{
            //    user = postParams["sys_user"];
            //}
            //string md5 = "";
            //if (postParams.ContainsKey("sys_md5"))
            //{
            //    md5 = postParams["sys_md5"];
            //}
            //string ip = context.Request.RemoteEndPoint.Address.ToString();
            //if (postParams.ContainsKey("sys_ip"))
            //{
            //    ip = postParams["sys_ip"];
            //}

            //string content = File.ReadAllText(filename, Encoding.UTF8);
            //FrmApp pFrmApp = new FrmApp(pFrmMain);
            //JS_Engine pEngine = null;

            //while (true)
            //{
            //    Random p = new Random();
            //    int port = 8880 + p.Next(1, 1000);
            //    pEngine = pTreap.find(new C_K_ID(port));
            //    if (pEngine == null)
            //    {
            //        pEngine = new JS_Engine(port);
            //        pEngine.Time = DateTime.Now;
            //        break;
            //    }
            //    else
            //    {
            //        double seconds = DateTime.Now.Subtract(pEngine.Time).TotalSeconds;
            //        if (seconds > 300)
            //        {
            //            pEngine = new JS_Engine(port);
            //            pEngine.Time = DateTime.Now;
            //            break;
            //        }
            //    }

            //}
            //pFrmApp.pJS = pEngine;

            //FrmMain pDebug = pFrmMain;
            //string strContent = pFrmApp.pJS.Run_Code(
            //    filename, pFrmApp, pDebug, content, postParams);

            //Random pRnd = new Random();
            //string strFile = Path_Tmp + "\\" + pRnd.NextDouble() + ".html";
            //S_File_Text.Write(strFile, strContent, false);
            //output(strFile, context);

            //}
            //else
            //{
            //    output(filename, context);
            //    context.Response.OutputStream.Close();
            //}

        }



        public void run()
        {
            if (listener == null)
            {
                // Create a Http server and start listening for incoming connections
                listener = new HttpListener();
                listener.Prefixes.Add(url);
                listener.Start();
                Main.WriteLine("http服务 监听： " + url);

                // Handle requests
                Task listenTask = HandleIncomingConnections();
                listenTask.GetAwaiter().GetResult();

                // Close the listener
                listener.Close();

            }
        }

    }
}
