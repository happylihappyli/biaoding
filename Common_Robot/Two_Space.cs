
using Common_Robot2;

namespace Common_Robot2
{
    public class Two_Space
	{
		public C_Space space_parent;
		public C_Space space;
		public Two_Space(C_Space space,C_Space space_parent)
        {
            this.space = space;
            this.space_parent = space_parent;
        }
    }
}
