
using B2_Treap.Funny;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Common_Robot2
{
    public class C_Check
    {
        public string user = "";
        public int user_id = 0;
        public string password = "";
        public bool check = true;
    }
    public partial class C_SYS
    {

        public StringBuilder pHTML = new StringBuilder();
        //public static Thread_Msg_Sender thread_sender = new Thread_Msg_Sender();

        public static string Path_Ini = "D:\\";//应用程序所在路径

        C_Space space;
        public C_SYS(C_Space space)
        {
            this.space = space;
            //单线程,所以不需要pFrmApp
            //this.pFrmApp = FrmApp;
        }




        public delegate void Init_Delegate(
            string url,
            string callback_Connect,
            string callback_chat_event);




        public void Log(string strLine)
        {
            Console.WriteLine(strLine);
        }





        public void run_app(C_Node pNode, string cmds, string args, bool show_error = true)
        {
            try
            {

                string strPath = cmds;
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = strPath;
                    process.StartInfo.Arguments = args;
                    process?.Start();
                }
            }
            catch (Exception ex)
            {
                if (show_error)
                    space.console.WriteLine(ex.ToString(), Level_Enum.Info, pNode.log_index, pNode.Name);
            }
        }


        public string run(string strCommand)
        {

            //strCmdText = "/C copy /b Image1.jpg + Archive.rar Image2.jpg";
            Process p = Process.Start("cmd.exe", "/c " + strCommand);
            //string output = p.
            return "";
        }


        public void write_html(string str)
        {
            pHTML.Append(str);
            //Console.WriteLine(str);
        }


        public void println(string str)
        {
            Console.WriteLine(str);
        }

        public string power_shell(string cmds)
        {
            return "";
            // create Powershell runspace

            //Runspace runspace = RunspaceFactory.CreateRunspace();

            //// open it

            //runspace.Open();

            //// create a pipeline and feed it the script text

            //Pipeline pipeline = runspace.CreatePipeline();
            //string[] cmd = cmds.Split('\n');
            //for (int i = 0; i < cmd.Length; i++)
            //{
            //    pipeline.Commands.AddScript(cmd[i]);
            //}

            //// add an extra command to transform the script
            //// output objects into nicely formatted strings

            //// remove this line to get the actual objects
            //// that the script returns. For example, the script

            //// "Get-Process" returns a collection
            //// of System.Diagnostics.Process instances.

            //pipeline.Commands.Add("Out-String");

            //// execute the script

            //Collection<PSObject> results = pipeline.Invoke();

            //// close the runspace
            //runspace.Close();

            //// convert the script result into a single string

            //StringBuilder stringBuilder = new StringBuilder();
            //foreach (PSObject obj in results)
            //{
            //    stringBuilder.AppendLine(obj.ToString());
            //}

            //return stringBuilder.ToString();
        }

        public string power_shell2(string cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "PowerShell.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.Verb = "runas";
            p.StartInfo.CreateNoWindow = false;
            p.Start();
            p.StandardInput.WriteLine(cmd);// "set-ExecutionPolicy RemoteSigned");
            //p.StandardInput.WriteLine("y");
            //p.StandardInput.WriteLine("cd SVC_Tool_1.0.0.0_Master_Test");
            //p.StandardInput.WriteLine(".\\Add-AppDevPackage.ps1");
            p.StandardInput.AutoFlush = true;
            p.WaitForExit();
            p.Close();
            return "";
        }

        //PSDataCollection<PSObject> outputCollection;// = new PSDataCollection<PSObject>();


        public C_Check? pCheck = null;
        public delegate void SendMsg(C_Msg pMsg);

        public event SendMsg Event_SendMsg;

        public void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Process p = sender as Process;
            if (p == null)
                return;
            Console.WriteLine(e.Data);
            if (Event_SendMsg != null)
                Event_SendMsg(new C_Msg(pCheck, "error", e.Data, 0, 0));
        }

        public void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Process p = sender as Process;
            if (p == null)
                return;
            Console.WriteLine(e.Data);
            if (Event_SendMsg != null)
                Event_SendMsg(new C_Msg(pCheck, "data", e.Data, 0, 0));
        }


        public void run_cmd(string cmds)
        {
            try
            {
                using (Process p = new Process())
                {
                    // set start info
                    p.StartInfo = new ProcessStartInfo("cmd.exe")
                    {
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        //WorkingDirectory = @"d:\"
                    };
                    // event handlers for output & error
                    p.OutputDataReceived += p_OutputDataReceived;
                    p.ErrorDataReceived += p_ErrorDataReceived;

                    // start process
                    p.Start();
                    // send command to its input
                    string[] strSplit = cmds.Split('\n');
                    for (var i = 0; i < strSplit.Length; i++)
                    {
                        string strLine = strSplit[i];
                        if (strLine.EndsWith("\r"))
                        {
                            strLine = strLine.Substring(0, strSplit[i].Length - 1);
                        }
                        p.StandardInput.Write(strLine + p.StandardInput.NewLine);
                    }
                    p.StandardInput.WriteLine("exit");
                    p.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }




        public string ssh_key(string cmds)
        {

            //Generate a public/private key pair.  
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            //Save the public key information to an RSAParameters structure.  
            RSAParameters rsaKeyInfo = rsa.ExportParameters(false);

            try
            {
                // 创建进程 C:/Windows/System32/OpenSSH/
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "ssh-keygen";// C:/Windows/System32/OpenSSH/ssh-keygen.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                //process.StartInfo.Arguments = cmds;// "-t rsa -N \"\" -b 2048 -C \"test\" -f D:/Net/Web/key";
                process.Start();
                process.StandardInput.AutoFlush = true;
                //string[] cmd = cmds.Split('\n');
                //for (int i = 0; i < cmd.Length; i++) {
                //    process.StandardInput.WriteLine(cmd[i]);
                //}
                Thread.Sleep(1000);
                process.StandardInput.WriteLine("exit");
                // 执行进程
                string standardOutput = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                return standardOutput;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        //public void Exit()
        //{
        //    Application.Exit();
        //}

        public void value_save(string key, string value)
        {

            //FrmApp.pMap.insert("value:" + key, value);
        }

        public string value_read(string key)
        {
            return "";
            //object pObj = FrmApp.pMap.find("value:" + key);
            //if (pObj == null) return "";
            //return (string)pObj;
        }

        public string strFile_JS = "";
        public string call_back_msg = "";//Thread 事件回调函数
        public string call_back_event = "";


        public void hook_event(string call_back_msg)
        {
            this.call_back_msg = call_back_msg;
        }



        public void thread_raise_event(string data)
        {
            //thread_sender.Raise_Event(this.call_back_msg, data);
        }



        ///// <summary>
        ///// 程序启动路径
        ///// </summary>
        ///// <returns></returns>
        //public string path_app()
        //{
        //    return Application.StartupPath;
        //}




        public string UserProfile()
        {
            var pathWithEnv = @"%USERPROFILE%";
            var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);
            return filePath;
        }


        public void sleep(double iSecond)
        {
            double time = 1000 * iSecond;
            Thread.Sleep((int)time);
        }


        public Treap<C_Var> pTreap_Var = new Treap<C_Var>();
        public Treap<C_Var> pTreap_Array = new Treap<C_Var>();

        public int var_array_count()
        {
            return pTreap_Array.Size();
        }

        public string var_array(string name)
        {
            if (pTreap_Array.find(name) != null)
            {
                string strValue = pTreap_Array.find(name).value;
                return strValue.Substring("(@array)".Length);
            }
            return "";
        }


        public string var_read(string name)
        {
            return pTreap_Var.find(name).value;
        }

        public string var_replace(string strLine)
        {
            TreapEnumerator p = pTreap_Var.GetEnumerator();
            while (p.HasMoreElements())
            {
                C_Var pNext = (C_Var)p.NextElement();
                if (pNext.value != "(@array)")
                {
                    strLine = strLine.Replace("(" + pNext.name + ")", pNext.value);
                }
            }
            return strLine;
        }


        public string compile_cs(
            string code,
            string strClass,
            string method,
            string value)
        {
            var options = new CompilerParameters();
            options.GenerateExecutable = false;
            options.GenerateInMemory = false;

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults compile = provider.CompileAssemblyFromSource(options, code);

            Type type = compile.CompiledAssembly.GetType(strClass);
            var abc = Activator.CreateInstance(type);

            MethodInfo method2 = type?.GetMethod(method);
            //object[] strSplit = value.Split('\t');
            string result = (string)method2?.Invoke(abc, null);// new object[] { value } );

            return result;
        }
    }
}
