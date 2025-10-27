
using Common_Robot2;
using pcammls;

namespace Test1
{
    //memory leak fixed
    public class Thread_Depth_Img
    {
        private uint16_t_ARRAY pixel_arr;
        private TY_IMAGE_DATA img;
        private I_Train pTrain;
        private S_Camera? pState_Camera = null;
        private C_Camera_TuYang? camera_const = null;
        private static TY_VECT_3F_ARRAY? p3dArray = null;

        public Thread_Depth_Img(
            I_Train pTrain,
            C_Camera_TuYang camera_const,
            S_Camera pState_Camera,
            int Pic_Count,
            uint16_t_ARRAY pixel_arr,
            TY_IMAGE_DATA img)
        {
            this.camera_const = camera_const;
            this.pState_Camera = pState_Camera;
            this.pixel_arr = pixel_arr;
            this.img = img;
            this.pTrain = pTrain;
            //this.Pic_Count = Pic_Count;
            if (p3dArray == null)
            {
                p3dArray = new TY_VECT_3F_ARRAY((uint)pState_Camera.camera_w * (uint)pState_Camera.camera_h);
            }
        }

        public void run()
        {
            Thread p = new Thread(Process);
            p.Start();
        }

        public void Process()
        {
            //定义点云分辨率
            pState_Camera?.深度图处理2(pState_Camera?.pLast, pTrain, camera_const, pixel_arr, p3dArray, img,
                img.width, img.height, img.width, img.height);
            uint16_t_ARRAY.ReleasePtr(this.pixel_arr);

        }
    }
}
