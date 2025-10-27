using Common_Robot2;
using ConverxHull;

namespace Test1
{
    public class C_Robot_Command
    {
        //第1个点的轴位置信息
        public string P1_Angle { get; set; }
        
        //第2个点的轴位置信息
        public string P2_Angle { get; set; }

        public int IO_1 { get; set; }
        public C_Point3D A = null;
        public Command_Type type = 0;

        public enum Command_Type
        {
            Robot=0,
            PLC_PIDAI = 1,
            PLC_FANDOU = 2,
        }
        public C_Robot_Command(
            Command_Type type,
            int IO_1, C_Point3D a,
            string P1_Angle,string P2_Angle)
        {
            this.type = type;
            this.P1_Angle = P1_Angle;
            this.P2_Angle = P2_Angle;

            this.IO_1 = IO_1;

            this.A = a;
        }

    }
}
