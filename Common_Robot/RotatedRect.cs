//using EmbedIO;
//using EmbedIO.Actions;
//using EmbedIO.Files;
//using EmbedIO.Security;
//using EmbedIO.WebApi;
using ConverxHull;
using System;
using System.Drawing;

namespace Common_Robot2
{
    public class RotatedRect
    {
        public double Angle=0;
        public double width = 0;
        public double height = 0;

        private C_Point3D[] pList=new C_Point3D[4];

        public RotatedRect(C_Point3D a, C_Point3D b, C_Point3D c, C_Point3D d)
        {
            pList[0] = a;
            pList[1] = b;
            pList[2] = c;
            pList[3] = d;


            double l1= a.distance(b);
            double l2 = c.distance(b);

            if (l1 > l2)
            {
                this.width = l1;
                this.height = l2;
                C_Point3D p= b-a;
                Angle = Math.Atan2(p.y, p.x) * 180 / 3.1415;
            }
            else
            {
                this.width = l2;
                this.height = l1;
                C_Point3D p = b-c;
                Angle = Math.Atan2(p.y, p.x) * 180 / 3.1415;
            }

        }

        public C_Point3D[] Points()
        {
            return pList;
        }
    }
}