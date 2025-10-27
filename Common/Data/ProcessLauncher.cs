using Common_Robot;
using System;
using System.Diagnostics;
using System.Text;

namespace Test1
{

    public class ProcessLauncher : IDisposable
    {
        private S_Cmd pCmd;
        private Process process;
        private bool running;

        public bool InteractiveMode
        {
            get;
            private set;
        }

        public ProcessLauncher(S_Cmd pCmd, string path,string encode)
        {
            this.pCmd = pCmd;

            process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WorkingDirectory = path;// mmm.path_template.Substring(0, 3);// @"D:\";
            process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");

            // Redirects the standard input so that commands can be sent to the shell.
            process.StartInfo.RedirectStandardInput = true;

            process.StartInfo.StandardOutputEncoding =  Encoding.GetEncoding(encode);

            process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(process_ErrorDataReceived);
            process.Exited += new EventHandler(process_Exited);
        }

        public void Start(string line="")
        {
            if (running == false)
            {
                running = true;
                InteractiveMode = true;

                // Runs the specified command and exits the shell immediately upon completion.
                process.StartInfo.Arguments = line;// "cmd.exe";// @"/c ""D:\code\python3\python.exe -i""";

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
        }

        public void Start_Python()
        {
            if (running == false)
            {
                running = true;
                InteractiveMode = true;

                // Runs the specified command and exits the shell immediately upon completion.
                process.StartInfo.Arguments = @"/c ""D:\code\python3\python.exe -i""";

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
        }


        public void Start_Script(string scriptFileName)
        {
            if (running == false)
            {
                running = true;
                InteractiveMode = false;

                // Runs the specified command and exits the shell immediately upon completion.
                process.StartInfo.Arguments = string.Format(@"/c ""C:\python27\python.exe ""{0}""""", scriptFileName);
            }
        }

        public void Abort()
        {
            process.Kill();
        }

        public void SendInput(string input)
        {
            process.StandardInput.Write(input);
            process.StandardInput.Flush();
        }

        private void process_OutputDataReceived(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
            {
                pCmd.callback_cmd(outLine.Data);
            }
        }

        private void process_ErrorDataReceived(object sendingProcess, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
            {
                pCmd.callback_cmd(outLine.Data);
            }
        }

        private void process_Exited(object sender, EventArgs e)
        {
            running = false;
        }

        public void Dispose()
        {
            if (process != null)
            {
                process.Dispose();
            }
        }
    }
}