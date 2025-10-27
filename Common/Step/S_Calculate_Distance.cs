using Common_Robot2;
using ConverxHull;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test1
{
    public class S_Calculate_Distance : C_Node
    {
        public string key_position1 = "";
        public string key_position2= "";
        public string key_save = "";

        public S_Calculate_Distance(string name,C_Space space_parent,C_Space space) : base(name,space_parent,space)
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

        public void run_sub_main() {

            string? line = (string?)this.read_var(this.key_position1, "string");
            if (line == null)
            {
                return;
            }
            string[] strSplit = line.Split(",");

            if (strSplit.Length > 2)
            {
                float x1 = float.Parse(strSplit[0]);
                float y1 = float.Parse(strSplit[1]);
                float z1 = float.Parse(strSplit[2]);
                C_Point3D p1 = new C_Point3D(x1, y1, z1);


                strSplit = ((string?)this.read_var(this.key_position2, "string")).Split(",");

                if (strSplit.Length > 2)
                {
                    float x2 = float.Parse(strSplit[0]);
                    float y2 = float.Parse(strSplit[1]);
                    float z2 = float.Parse(strSplit[2]);
                    C_Point3D p2 = new C_Point3D(x2, y2, z2);

                    double d=p1.distance(p2);
                    this.save_var(this.key_save, "string", d + "");
                }
            }
        }


    }
}
