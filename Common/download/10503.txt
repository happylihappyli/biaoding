using Common_Robot2;
using ConverxHull;

namespace Test1
{
    public class C_BoxInfo
    {
        public C_Point3D center;
        public C_Data pData;
        public C_Point3D faxiangliang;//法向量
        public int Group_ID=-1;
        public string direction="";

        public float x_min;
        public float x_max;
        public float y_min;
        public float y_max;
        public float z_min;
        public float z_max;
        public C_BoxInfo? extend=null;

        public C_BoxInfo(C_Point3D center, C_Data pData, C_Point3D fa,string direction)
        {
            pData.pBoxInfo = this;
            this.center = center;
            this.pData=pData;
            this.faxiangliang = fa;
            this.Group_ID = pData.Group_ID;
            this.direction = direction;
        }
    }
}
