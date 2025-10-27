
using Common_Robot2;


namespace Test1
{
    public class E_Form_Load : C_Node
    {
        public string wait = "0";
        public ParameterizedThreadStart hook;

        public E_Form_Load(string name, C_Space space_parent, C_Space space) :
            base(name,space_parent, space)
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
            int i_wait=int.Parse(wait); 
            Thread.Sleep(i_wait);

            if (hook != null)
            {
                Thread thread = new Thread(hook);
                Two_Space two = new Two_Space(space, space_parent);
                thread.Start(two);
                return;
            }
        }
    }
}
