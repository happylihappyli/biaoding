namespace Common_Robot2
{
    public class C_Bmp
    {
        public byte[] buffer;
        public int stride;
        public int width;
        public int height;

        public C_Bmp(byte[] buffer, int stride, int width, int height)
        {
            this.buffer = buffer;
            this.stride = stride;
            this.width = width;
            this.height = height;
        }
    }
}