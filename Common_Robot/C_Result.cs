
using ConverxHull;

namespace Common_Robot2
{
    public class C_Result
    {
        public string strLine = "";
        public string error_msg = "";
         
        public C_Point3D? A = null;
        public C_Point3D? B = null;
        public C_Point3D? C2 = null;
        public C_Point3D? C = null;
        public C_Point3D? D = null;


        public C_Result(string line, 
            C_Point3D A, C_Point3D B,
            C_Point3D C2, C_Point3D C, C_Point3D D,
            string error_msg="")
        {
            this.strLine = line;
            this.A = A;
            this.B = B;
            this.C2 = C2;
            this.C = C;
            this.D = D;
            this.error_msg = error_msg;
        }
    }
}
