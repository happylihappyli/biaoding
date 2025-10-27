namespace Common_Robot2
{
    public class Pic_Token
    {
        public static int Pic_Token_Count=1;


        public Pic_Token()
        {
            Pic_Token_Count++;
            this.Token_ID = Pic_Token_Count;
        }

        public int Token_ID { get; private set; }
    }
}