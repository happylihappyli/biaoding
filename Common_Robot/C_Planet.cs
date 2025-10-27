using ConverxHull;
using MathNet.Spatial.Units;
using System;

namespace Common_Robot2
{
    public class C_Planet
    {
        public int Group_ID = -1;//-1代表没有JBox分割框，否则为JBox的下标

        public int x_index = 0;
        public int y_index = 0;
        public int z_index = 0;
        public C_Point3D x0y0 = new C_Point3D(0, 0, 0);
        public C_Point3D x1y0 = new C_Point3D(0, 0, 0);
        public C_Point3D x0y1 = new C_Point3D(0, 0, 0);
        public C_Point3D x1y1 = new C_Point3D(0, 0, 0);

        public C_Point3D center = new C_Point3D(0, 0, 0);//中心位置
        public C_Point3D z_faxiangliang = new C_Point3D(0, 0, 0);//飞机z轴，也就是飞机背面的法向量
        public C_Point3D x_head = new C_Point3D(0, 0, 0);//飞机x轴也就是飞机头的方向
        public C_Point3D y_Direction = new C_Point3D(0, 0, 0);//飞机y轴

        public Angle z_rotate;
        public double percent = 0; //匹配度
        public double distance_to_one_point = 0;//到某个点的距离，排序用！

        public C_Planet(int Group_ID)
        {
            this.Group_ID = Group_ID;
        }

        public C_Planet(int Group_ID,C_Point3D center,C_Point3D z_faxiangliang)
        {
            this.Group_ID = Group_ID;
            this.center = center;
            this.z_faxiangliang = z_faxiangliang.Clone().normalize();
        }

        public C_Point3D 投影到一个面后的坐标(C_Point3D p1)
        {
            C_Point3D v1=p1-center;

            float len_z =v1*z_faxiangliang;
            C_Point3D vz = z_faxiangliang.Clone().normalize()*(len_z);
            return v1-(vz)+(center);
        }
    }
}
