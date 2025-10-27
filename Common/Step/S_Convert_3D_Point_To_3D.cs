using Common_Robot;
using Common_Robot2;
using ConverxHull;
using pcammls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test1
{
    public class S_Convert_3D_Point_To_3D : C_Node
    {
        public string key_read = "3d_key1";
        public string key_save = "3d_key2";
        public string key_camera = "camera_key";

        public S_Convert_3D_Point_To_3D(string name, C_Space space_parent, C_Space space) :
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
            C_Point3D? pPoint2 = (C_Point3D?)this.read_var(this.key_read, "C_Point3D");
            C_Camera_TuYang? camera1 = (C_Camera_TuYang?)this.read_var(this.key_camera, "C_Camera_TuYang");

            if (camera1 != null && pPoint2 != null)
            {
                C_Point3D? pPoint3 = Main_Robot.摄像头坐标转到机械臂坐标(camera1, pPoint2);
                if (pPoint3 == null)
                {
                    Main.speak_async(this.Name + " 有错误");
                    this.Next_Step = Node_Next.True;
                }
                else
                {
                    this.save_var(this.key_save, "C_Point3D", pPoint3);
                }
            }
            else
            {
                Main.speak_async(this.Name + " 有错误");
                this.Next_Step = Node_Next.True;
            }
        }
    }
}
