using Common_Robot2;

namespace Test1
{
    public class C_Space_Load
    {
        public static Form form1;
        public void load(bool b_run_space,string space_head, 
                C_Space space, C_Space space_new, string key_space,string tag="")
        {
            var pMethod = typeof(FrmMain).GetMethod(key_space);
            if (pMethod == null)
            {
                MessageBox.Show("没有这个方法:"+key_space);
                return;
            }
            try{
                pMethod.Invoke(form1, new object[] {b_run_space, space_head, space, space_new, key_space, tag });
            }catch(Exception ex)
            {
                Main.WriteLine(null,ex.ToString());
            }
        }

       
    }
}