using Common_Robot2;
using ConverxHull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test1
{
    public class C_Thread_Data
    {
        public List<C_Space> spaces;
        public C_Space space_parent;
        public C_Data pData;
        public string space_name;
        public string tag;
        public C_Space space_new = new C_Space();
        public C_Node pNode = null;
        public bool bPartial = false;

        public C_Thread_Data(
            bool bPartial,
            C_Node pNode,
            List<C_Space> spaces,
            C_Space space,
            C_Space space_parent,
            C_Data pData,
            string space_name,
            string tag)
        {
            this.space_name = space_name;
            this.tag = tag;


            this.pNode = pNode;
            this.spaces = spaces;
            this.pData = pData;
            this.bPartial = bPartial;

            if (bPartial)
            {
                this.space_new = space;
                this.space_parent = space_parent;
            }
            else
            {
                this.space_new = new C_Space();
                this.space_parent = space;
                space_new.Name = this.space_name;
            }
            space_new.parent_space = this.space_parent;
            space_new.tag = tag;
            space_new.pTrain = new C_Train(tag);
            spaces.Add(space_new);
        }

        public void ThreadFun()
        {
            Main.WriteLine("调用子空间 step1 tag=" + this.space_name +"," + tag);
            
            C_Space_Load pLoad = new C_Space_Load();
            //space_new.copy_from(space_parent, pNode);
            space_new.finished_exit = true;
            Main.WriteLine("调用子空间 step2 tag=" + this.space_name +","+ tag);

            pLoad.load(this.bPartial==false, this.space_name, space_parent, space_new, this.space_name, tag);

        }

    }
}
