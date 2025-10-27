
using Test1;

namespace Common_Robot2
{
    public class C_Train_Factory : I_Train_Factory
    {
        public I_Train create(string ID)
        {
            return new C_Train(ID);
        }
    }
}
