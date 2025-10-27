
using MathNet.Numerics;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Common_Robot2
{
    public class C_Console
    {
        //public ArrayList pList1 = new ArrayList();
        //public ArrayList pList2 = new ArrayList();
        //public ArrayList pList3 = new ArrayList();

        public bool bChange1 = false;
        public bool bChange2 = false;
        public bool bChange3 = false;

        public ILog pLog = null;

        C_Space space;
        public C_Console(C_Space space)
        {
            this.space = space;
        }



        public void WriteLine(string content,
            Level_Enum level, string classify, 
            string source, string author = "", string remark = "")
        {
            switch(classify)
            {
                case "":
                case "1":
                    this.bChange1 = true;
                    break;
                case "2":
                    this.bChange2 = true;
                    break;
                case "3":
                    this.bChange3 = true;
                    Console.WriteLine(content);
                    break;
            }

            if (this.pLog != null)
            {
                this.pLog.WriteLine(content, level, classify, source, author, remark);
            }
            else
            {
                Console.WriteLine("pLog=null: "+content);
            }
        }


        public void newline(int index)
        {
            if (index == 1)
            {
                bChange1 = true;
                //pList1.Add("");
            }
            else if (index == 2)
            {
                bChange2 = true;
                //pList2.Add("");
            }
            else if (index == 3)
            {
                bChange3 = true;
                //pList3.Add("");
            }
        }
    }
}
