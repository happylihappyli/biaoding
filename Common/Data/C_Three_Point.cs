using ConverxHull;
using MathNet.Spatial.Euclidean;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinRT;

namespace Test1.Common.Data
{
    public class C_Three_Point
    {
        public C_Point3D A;
        public C_Point3D B;
        public C_Point3D P7; //K  not used
        public string six = "";
        public string memo = "";
        public C_Three_Point(C_Point3D? a_start, C_Point3D? b_start, C_Point3D P7, string six)
        {
            this.A = a_start;
            this.B = b_start;
            this.P7 = P7;
            this.six = six;
        }

        public double distance(C_Three_Point p2)
        {
            double len1 = this.A.distance(p2.A);
            p2.B = p2.A + (p2.B - p2.A).normalize() * (this.B-this.A).length();
            double len2 = this.B.distance(p2.B);
            p2.P7 = p2.A + (p2.P7 - p2.A).normalize() * (this.P7 - this.A).length();
            double len3 = this.P7.distance(p2.P7);

            return Math.Max(len1,Math.Max(len2,len3));
        }

        public C_Three_Point add(C_Point3D c_Point3D)
        {
            return new C_Three_Point(
                    A + c_Point3D,B + c_Point3D,P7 + c_Point3D,this.six);
        }

        public C_Three_Point Clone()
        {
            return new C_Three_Point(A.Clone(), B.Clone(), P7.Clone(), this.six);
        }
    }
}
