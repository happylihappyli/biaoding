using Common_Robot;
using pcammls;

namespace Test1
{
    public class Data_Depth
    {
        public C_Train pTrain;
        public int pic_Count;
        public uint16_t_ARRAY pixel_arr_depth;
        public TY_IMAGE_DATA img;

        public Data_Depth(
            C_Train pPicture,
            int pic_Count,
            uint16_t_ARRAY pixel_arr_depth,
            TY_IMAGE_DATA img)
        {
            this.pTrain = pPicture;
            this.pic_Count = pic_Count;
            this.pixel_arr_depth = pixel_arr_depth;
            this.img = img;
        }
    }
}