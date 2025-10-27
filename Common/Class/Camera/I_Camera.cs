using Common_Robot2;
using pcammls;

namespace Test1
{
    public interface I_Camera
    {
        public void SaveBMP(I_Train? pTrain, uint8_t_ARRAY? pixel_arr, string key_save, int width, int height);
    }
}