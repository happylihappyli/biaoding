using Common_Robot2;
using System.Speech.Synthesis;

namespace Test1
{
    public class S_TTS : C_Node
    {
        public string msg = "";

        public S_TTS(string name, C_Space space_parent, C_Space space) :
            base(name, space_parent, space)
        { 
        }
        public override void init()
        {

        }
        public override Task run_sub()
        {
            run_sub_main();
            return Task.CompletedTask;
        }

        public void run_sub_main()
        {
            string msg2 = this.read_string(this.msg);
            Main.WriteLine(this,msg2);
            Main.speak(msg2);
        }

    }
}
