using ConverxHull;
using System.Drawing.Drawing2D;
using System.Numerics;
using Three.Net.Math;
using Vector3 = System.Numerics.Vector3;

namespace Test1
{
    public static class Main_Point
    {
        public static C_Point3D Multiply_Point(this Matrix4 m4,C_Point3D p1)
        {
            Vector3 v = new Vector3(p1.x, p1.y, p1.z);

            Matrix4x4 m44 = ConvertToMatrix4x4(m4);
            var m44_b = Matrix4x4.Transpose(m44);
            //v.Apply();

            //Vector3.Transform 是向量乘以矩阵，向量在左边，是行向量
            Vector3 v2 =Vector3.Transform(v, m44_b); 

            return new C_Point3D(v2.X, v2.Y, v2.Z);
        }

        public static C_Point3D Vector3_To_Point(Vector3 v)
        {
            return new C_Point3D(v.X, v.Y, v.Z);
        }

        public static Matrix4x4 ConvertToMatrix4x4(Matrix4 matrix4)
        {
            float[] m = matrix4.ToArray();

            return new Matrix4x4(
                m[0], m[4], m[8], m[12],
                m[1], m[5], m[9], m[13],
                m[2], m[6], m[10], m[14],
                m[3], m[7], m[11], m[15]
            );
        }
    }
}
