using Common_Robot2;
using ConverxHull;

namespace Test1
{
    public class C_DrawBox
    {
        public C_Rect pRect1 = new C_Rect(new C_Point3D(0, 0, 0), new C_Point3D(100, 100, 0));
        public C_Rect pRect2 = new C_Rect(new C_Point3D(0, 0, 0), new C_Point3D(100, 100, 0));//工具底座范围
        public C_Point3D pBox_Draw;
    }
}