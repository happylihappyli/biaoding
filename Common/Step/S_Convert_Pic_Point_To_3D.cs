
using Common_Robot2;
using ConverxHull;
using pcammls;

namespace Test1
{
    public class S_Convert_Pic_Point_To_3D : C_Node
    {
        public string key_read = "pic_key";
        public string key_save = "3d_key";
        public string camera_w = "1280";
        public string key_3d = "";

        public S_Convert_Pic_Point_To_3D(string name, C_Space space_parent, C_Space space) :
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
            TY_VECT_3F_ARRAY glb_p3dArray = (TY_VECT_3F_ARRAY)this.read_var(this.key_3d, "TY_VECT_3F_ARRAY");

            if (glb_p3dArray == null)
            {
                MessageBox.Show(this.Name + " key_3d 设置错误");
                Main.speak_async(this.Name + " 有错误");
                this.Next_Step = Node_Next.True;
                return;
            }

            C_Point3D? pPoint1= (C_Point3D?)this.read_var(key_read, "C_Point3D");
            int x = (int)Math.Round(pPoint1.x);
            int y = (int)Math.Round(pPoint1.y);
            C_Point3D pPoint2 = Main_Camera.图像转3D点云(glb_p3dArray, int.Parse(camera_w), x, y);
            if (pPoint2 == null)
            {

                Main.speak_async(this.Name + " 有错误");
                this.Next_Step = Node_Next.True;
                return;

            }

            space.save_vars(pTrain, this, key_save, "C_Point3D", pPoint2);

        }

    }
}
