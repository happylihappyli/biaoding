using pcammls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test1
{
    public class Data_RGB
    {
        public uint8_t_ARRAY? pixel_arr;
        public uint8_t_ARRAY? pixel_arr_undistort;
        public TY_IMAGE_DATA? img;
        public int Pic_Count = 0;

        public Data_RGB(
            int Pic_Count,
            uint8_t_ARRAY pixel_arr,
            TY_IMAGE_DATA img)
        {
            this.pixel_arr = pixel_arr;
            this.img = img;
            this.Pic_Count = Pic_Count;
        }

    }
}
