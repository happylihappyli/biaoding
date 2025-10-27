using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections;
using System.IO;
using static System.IO.FileMode;

namespace Common_Robot2
{
    public class SecureTransfer : IDisposable
    {
        private readonly string Host;
        private readonly string User;
        private readonly int Port;
        public readonly SSHTransferProtocol Protocol;
        public string SrcFile;
        public string DstFile;
        public ScpClient ScpClt;
        public SftpClient SftpClt;
        public SftpUploadAsyncResult async_upload_result;
        public SftpDownloadAsyncResult async_download_result;

        public AsyncCallback asyncCallback;

        FileStream? stream_upload = null;
        FileStream? stream_download = null;

        public SecureTransfer()
        {

        }

        public SecureTransfer(string host, string user, int port, SSHTransferProtocol protocol)
        {
            Host = host;
            User = user;
            Port = port;
            Protocol = protocol;
        }

        public SecureTransfer(string host,
            string user,
            int port,
            SSHTransferProtocol protocol,
            string source,
            string dest)
        {
            Host = host;
            User = user;
            Port = port;
            Protocol = protocol;
            SrcFile = source;
            DstFile = dest;
        }

        public void Connect(string password)
        {
            switch (Protocol)
            {
                case SSHTransferProtocol.SCP:
                    ScpClt = new ScpClient(Host, Port, User, password);
                    ScpClt.Connect();
                    break;
                case SSHTransferProtocol.SFTP:
                    SftpClt = new SftpClient(Host, Port, User, password);
                    SftpClt.Connect();
                    break;
            }
        }


        public void Connect2(string private_file)
        {
            PrivateKeyFile pFile = new PrivateKeyFile(private_file);
            switch (Protocol)
            {
                case SSHTransferProtocol.SCP:
                    ScpClt = new ScpClient(Host, Port, User, pFile);
                    ScpClt.Connect();
                    break;
                case SSHTransferProtocol.SFTP:
                    SftpClt = new SftpClient(Host, Port, User, pFile);
                    SftpClt.Connect();
                    break;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (Protocol == SSHTransferProtocol.SCP)
                {
                    ScpClt.Disconnect();
                }

                if (Protocol == SSHTransferProtocol.SFTP)
                {
                    SftpClt.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public string List_File(string dir)
        {
            string[] strSplit = dir.Split('/');
            string path = "";
            for (var i = 0; i < strSplit.Length - 1; i++)
            {
                path += strSplit[i] + "/";
            }
            dir = strSplit[strSplit.Length - 1];

            ArrayList pList = new ArrayList();
            SftpClt.ChangeDirectory(path);// "/");

            string strReturn = "";
            foreach (var entry in SftpClt.ListDirectory(dir))
            {
                if (entry.IsDirectory)
                {
                    //ListDirectory(client, entry.FullName, ref files);
                }
                else
                {
                    strReturn += entry.FullName + "|";
                    //pList.Add(entry.FullName);
                }
            }
            if (strReturn.EndsWith("|"))
            {
                strReturn = strReturn.Substring(0, strReturn.Length - 1);
            }
            return strReturn;
        }

        public void Download()
        {
            if (Protocol == SSHTransferProtocol.SCP)
            {
                if (!ScpClt.IsConnected)
                {
                    return;
                }
                ScpClt.Download(SrcFile, new FileInfo(DstFile));
            }

            if (Protocol == SSHTransferProtocol.SFTP)
            {
                if (!SftpClt.IsConnected)
                {
                    return;
                }

                stream_download = new FileStream(DstFile, FileMode.Create);
                async_download_result =
                    (SftpDownloadAsyncResult)SftpClt.BeginDownloadFile(
                        SrcFile, stream_download,
                        asyncCallback);
            }
        }

        public void Upload()
        {
            if (Protocol == SSHTransferProtocol.SCP)
            {
                if (!ScpClt.IsConnected)
                {
                    //Runtime.MessageCollector.AddMessage(Messages.MessageClass.ErrorMsg,
                    //    Language.strSSHTransferFailed + Environment.NewLine +
                    //    "SCP Not Connected!");
                    return;
                }
                ScpClt.Upload(new FileInfo(SrcFile), $"{DstFile}");
            }

            if (Protocol == SSHTransferProtocol.SFTP)
            {
                if (!SftpClt.IsConnected)
                {
                    //Runtime.MessageCollector.AddMessage(Messages.MessageClass.ErrorMsg,
                    //    Language.strSSHTransferFailed + Environment.NewLine +
                    //    "SFTP Not Connected!");
                    return;
                }
                stream_upload = new FileStream(SrcFile, Open);
                async_upload_result =
                    (SftpUploadAsyncResult)SftpClt.BeginUploadFile(stream_upload, $"{DstFile}",
                        asyncCallback);
            }
        }

        public enum SSHTransferProtocol
        {
            SCP = 0,
            SFTP = 1
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (Protocol == SSHTransferProtocol.SCP)
            {
                ScpClt.Dispose();
            }

            if (Protocol == SSHTransferProtocol.SFTP)
            {
                SftpClt.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void End_Download()
        {

            SftpClt.EndDownloadFile(async_download_result);
            stream_download?.Flush();
            stream_download?.Close();
        }

        public void End_Upload()
        {
            SftpClt.EndUploadFile(async_upload_result);
            stream_upload?.Close();
        }
    }
}