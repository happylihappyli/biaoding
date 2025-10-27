using ConverxHull;
using System;

namespace Common_Robot2
{
    public class C_Rect
    {
        public C_Point3D pPoint1 = new C_Point3D(0, 0, 0);
        public C_Point3D pPoint2 = new C_Point3D(0, 0, 0);

        public C_Rect(C_Point3D pPoint1, C_Point3D pPoint2)
        {
            this.pPoint1 = pPoint1;
            this.pPoint2 = pPoint2;
        }

        public int width()
        {
            return (int)(pPoint2.x - pPoint1.x);
        }

        public int height()
        {
            return (int)(pPoint2.y - pPoint1.y);
        }
    }
}
