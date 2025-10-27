using MathNet.Spatial.Euclidean;
using System.Numerics;
//using Three.Net.Math;

namespace Test1
{
    public static class Main_Quaternion
    {

        public static System.Numerics.Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
        {
            double halfAngle = angle * 0.5;
            double s = Math.Sin(halfAngle);
            System.Numerics.Quaternion q;
            q.X = (float)(axis.X * s);
            q.Y = (float)(axis.Y * s);
            q.Z = (float)(axis.Z * s);
            q.W = (float)Math.Cos(halfAngle);
            return q;
        }

        public static Vector3 Transform(Vector3 v, System.Numerics.Quaternion rotation)
        {
            TransformWithoutOverlap(v, rotation, out var toReturn);
            return toReturn;
        }


        /// <summary>
        /// 四元数到旋转矩阵 
        /// https://www.songho.ca/opengl/gl_quaternion.html
        /// </summary>
        /// <param name="v"></param>
        /// <param name="rotation"></param>
        /// <param name="result"></param>
        public static void TransformWithoutOverlap(Vector3 v, System.Numerics.Quaternion rotation, out Vector3 result)
        {
            //This operation is an optimized-down version of v' = q * v * q^-1.
            //The expanded form would be to treat v as an 'axis only' quaternion
            //and perform standard quaternion multiplication.  Assuming q is normalized,
            //q^-1 can be replaced by a conjugation.
            float x2 = rotation.X + rotation.X;
            float y2 = rotation.Y + rotation.Y;
            float z2 = rotation.Z + rotation.Z;
            float xx2 = rotation.X * x2;
            float xy2 = rotation.X * y2;
            float xz2 = rotation.X * z2;
            float yy2 = rotation.Y * y2;
            float yz2 = rotation.Y * z2;
            float zz2 = rotation.Z * z2;
            float wx2 = rotation.W * x2;
            float wy2 = rotation.W * y2;
            float wz2 = rotation.W * z2;
            //Defer the component setting since they're used in computation.
            result.X = v.X * (1f - yy2 - zz2) + v.Y * (xy2 - wz2) + v.Z * (xz2 + wy2);
            result.Y = v.X * (xy2 + wz2) + v.Y * (1f - xx2 - zz2) + v.Z * (yz2 - wx2);
            result.Z = v.X * (xz2 - wy2) + v.Y * (yz2 + wx2) + v.Z * (1f - xx2 - yy2);
        }
    }
}
