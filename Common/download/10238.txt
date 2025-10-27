using Common_Robot;
using Common_Robot2;
using ConverxHull;

namespace Test1
{
    public class String_From_Point : C_Node
    {

        public string key_read = "";//string
        public string key_save = "";//C_Point3D
        public String_From_Point(string name, C_Space space_parent, C_Space space) : base(name,space_parent, space)
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
            C_Point3D? pPoint = (C_Point3D?)this.read_var(this.key_read, "C_Point3D");
            if (pPoint!=null)
            this.save_var(key_save, "string", pPoint.ToString());

        }
    }
}
