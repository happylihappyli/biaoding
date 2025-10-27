using ConverxHull;

namespace Test1.Common.Data
{

    /// <summary>
    /// 盒子的八个点，ABCD,EFGH
    /// </summary>
    public class C_Box_Eight
    {
        public C_Point3D A = null;
        public C_Point3D B = null;
        public C_Point3D C = null;
        public C_Point3D D = null;
        public C_Point3D E = null;
        public C_Point3D F = null;
        public C_Point3D G = null;
        public C_Point3D H = null;

        public List<C_Point3D> toList()
        {
            List<C_Point3D> list = new List<C_Point3D>();
            list.Add(A);
            list.Add(B);
            list.Add(C);
            list.Add(D);
            list.Add(E);
            list.Add(F);
            list.Add(G);
            list.Add(H);

            return list;
        }
    }
}
