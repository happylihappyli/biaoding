using Common_Robot;
using Common_Robot2;
using ConverxHull;
using Newtonsoft.Json.Linq;
using ToastNotifications;
namespace Test1
{
    public partial class Main
    {
        public static void PlayNotificationSound(string sound)
        {
            var soundsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Common\\Class\\Notification\\Sounds");
            var soundFile = Path.Combine(soundsFolder, sound + ".wav");

            using (var player = new System.Media.SoundPlayer(soundFile))
            {
                player.Play();
            }
        }

        public static void show_tip(C_Node pNode,string title,string msg,int time=3)
        {
            Task.Run(() =>
            {
                Main.WriteLine(pNode, " ============Tip============ " + title);
                Main.WriteLine(pNode,  msg);
                Main.WriteLine(pNode, " ============Tip============ ");

                Notification toastNotification = new Notification(
                    title, msg,time, FormAnimator.AnimationMethod.Slide, FormAnimator.AnimationDirection.Up);
                toastNotification.Show();

            });
        }

        public static void show_tip2()
        {


            try
            {
                //new ToastContentBuilder();
                //    .AddArgument("action", "viewConversation")
                //    .AddArgument("conversationId", 9813)
                //    .AddText(title)
                //    .AddText(msg)
                //    .Show();
            }
            catch (Exception ex)
            {
                Main.WriteLine(ex.Message);
            }
        }

    }
}
