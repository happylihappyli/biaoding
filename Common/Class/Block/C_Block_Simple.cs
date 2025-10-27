using ConverxHull;
using Test1;

namespace palletizing
{

    /// <summary>
    /// 简单块
    /// </summary>
    public class C_Block_Simple:I_Size
    {
        public C_Point3D pos = new C_Point3D(0,0,0);//这个位置是相对所放置空间的

        public long nx, ny, nz;
        public bool 用完 = false;

        public C_Box_Type pBox_Type;
        public C_Block_Simple Roate_Z90=null;//旋转90度的形状
        public double x_split_space = 0;//箱子和箱子的额外空隙，这个空隙是因为放不满为了保证上层的稳定性，自动加的
        public double y_split_space = 0;//箱子和箱子的额外空隙，这个空隙是因为放不满为了保证上层的稳定性，自动加的

        public C_Block_Simple(
            C_Box_Type box_Type,
            long x_len, long y_len, long z_len,
            long nx, long ny, long nz)
        {
            this.pBox_Type=box_Type;

            this.x_len = x_len;
            this.y_len = y_len;
            this.z_len = z_len;
            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
        }

        public long count()
        {
            return nx * ny * nz;
        }

        public double space()
        {
            return x_len * y_len * z_len;
        }

        public long space_used()
        {
            return pBox_Type.x_len * pBox_Type.y_len * pBox_Type.z_len*this.count();
        }


        public C_Block_Simple Clone()
        {
            C_Box_Type box_type = this.pBox_Type;
            C_Block_Simple p = new C_Block_Simple(box_type, x_len,y_len,z_len,nx,ny,nz);
            p.pos = new C_Point3D(this.pos.x, this.pos.y, this.pos.z);
            p.x_split_space = this.x_split_space;
            p.y_split_space = this.y_split_space;
            return p;
        }
    }
}