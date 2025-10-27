
using Common_Robot2;

namespace Test1
{
    public class W_TCP_Client : I_TCP
    {

        public bool b_loop = false;
        public W_TCP_Client(string name, C_Space space_parent, C_Space space) :
            base(name, space_parent, space)
        {
        }

        public override Task run_sub()
        {
            b_loop = space.vars.bClosingWindows;
            run_sub_main();
            //Thread.Sleep(1000);
            return Task.CompletedTask;
        }

        public void run_sub_main()
        {

            try
            {
                connect_server();
            }
            catch (Exception ex)
            {
                if (null != Received_CallbackFunc)
                {
                    Received_CallbackFunc(new byte[] { 0x00 },
                        -2,
                        ex.ToString()
                        );
                }
            }
        }

    }


}
