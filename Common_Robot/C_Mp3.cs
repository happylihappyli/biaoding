
using NAudio.Wave;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Common_Robot2
{
    /*
     * 调用API函数mciSendString播放音频文件
     * 主要包括按指定次数播放以及循环播放
     * 作者：Zhy
     * 时间：2015-7-21
     */
    public class C_Mp3
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(
            string command,      //MCI命令字符串
            string returnString, //存放反馈信息的缓冲区
            int returnSize,      //缓冲区的长度
            IntPtr hwndCallback  //回调窗口的句柄，一般为NULL
            );                   //若成功则返回0，否则返回错误码。

        C_Space space;

        public C_Mp3(C_Space space)
        {
            this.space = space;
        }


        /// <summary>
        /// 按指定次数播放
        /// </summary>
        /// <param name="file"></param>
        private void PlayWait(string file)
        {
            /*
             * open device_name type device_type alias device_alias  打开设备
             * device_name　 　　　要使用的设备名，通常是文件名。
             * type device_type　　设备类型，例如mpegvideo或waveaudio，可省略。
             * alias device_alias　设备别名，指定后可在其他命令中代替设备名。
             */
            mciSendString(string.Format("open \"{0}\" type mpegvideo alias media", file), null, 0, IntPtr.Zero);

            /*
             * play device_alias from pos1 to pos2 wait repeat  开始设备播放
             * 若省略from则从当前磁道开始播放。
             * 若省略to则播放到结束。
             * 若指明wait则等到播放完毕命令才返回。即指明wait会产生线程阻塞，直到播放完毕
             * 若指明repeat则会不停的重复播放。
             * 若同时指明wait和repeat则命令不会返回，本线程产生堵塞，通常会引起程序失去响应。
             */
            mciSendString("play media wait", null, 0, IntPtr.Zero);

            /*
             * close　　　 关闭设备
             */
            mciSendString("close media", null, 0, IntPtr.Zero);
        }

        /// <summary>
        /// 循环播放
        /// </summary>
        /// <param name="file"></param>
        private void PlayRepeat(string file)
        {
            mciSendString(string.Format("open \"{0}\" type mpegvideo alias media", file), null, 0, IntPtr.Zero);
            mciSendString("play media repeat", null, 0, IntPtr.Zero);
        }

        private Thread thread;
        /// <summary>
        /// 播放音频文件
        /// </summary>
        /// <param name="file">音频文件路径</param>
        /// <param name="times">播放次数，0：循环播放 大于0：按指定次数播放</param>
        public void Play(string file, int times)
        {
            //用线程主要是为了解决在播放的时候指定wait时产生线程阻塞,从而导致界面假死的现象
            Thread thread = new Thread(() =>
              {
                  //PlayWait(file);

                  if (times == 0)
                  {
                      PlayRepeat(file);
                  }
                  else if (times > 0)
                  {
                      for (int i = 0; i < times; i++)
                      {
                          PlayWait(file);
                      }
                  }
              });//.Start();

            //  线程必须为单线程
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 结束播放的线程
        /// </summary>
        public void Exit()
        {
            if (thread != null)
            {
                try
                {
                    thread.Interrupt();
                }
                catch { }
                thread = null;
            }
        }


        public enum Play_State
        {
            none,
            启动机械臂,
            未识别到包裹,
            y倾角超出范围,
            底座超出范围,
            面积匹配度太低,
            抓取匹配度太低,
            发送消息给传输带,
            发送消息给翻斗机,
            点云宽度太小,
            点云高度太小,
            连续重复两次,
            超出机械臂的长度,
            机械臂可能会碰撞,
            机械臂可能会碰撞障碍物,
            用上次的计算结果,
            开始抓取图片,
        }

        public void play_mp3(C_Node pNode, string path_mp3,Play_State name)
        {
            if (path_mp3 == null) return;
            if (path_mp3 == "") return;

            space.console.WriteLine(name.ToString(), Level_Enum.Info, pNode.log_index, pNode.Name);
            //Task.Run(() =>
            {
                if (name== C_Mp3.Play_State.机械臂可能会碰撞)
                {
                    Console.Beep(6000, 100);
                    Console.Beep(7000, 100);
                    return;
                }
                string file_mp3 = name.ToString();
                switch (name)
                {
                    case Play_State.none:
                        file_mp3 = "";
                        return;
                }

                try
                {


                    file_mp3 =  path_mp3 + "\\" + file_mp3 + ".mp3";//mp3\\
                    if (File.Exists(file_mp3))
                    {
                        var reader = new Mp3FileReader(file_mp3);// "test.mp3");
                        var waveOut = new WaveOut(); // or WaveOutEvent()
                        waveOut.Init(reader);
                        waveOut.Play();
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(100);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
        }


    }
}