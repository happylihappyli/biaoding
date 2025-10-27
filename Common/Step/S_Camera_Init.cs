using Common_Robot2;
using ConverxHull;

namespace Test1
{

    /// <summary>
    /// 拍照模块初始化
    /// </summary>
    public class S_Camera_Init : C_Node
    {

        public string key_step = "";

        public S_Camera_Init(string name,C_Space space_parent,C_Space space) : base(name,space_parent, space)
        { 
        }

        public override void init()
        {

        }

        public override Task run_sub()
        {
            run_sub_main();
            return Task.CompletedTask;
        }

        private void run_sub_main()
        {
            S_Camera? pItem = (S_Camera?)space.vars_step["main" + key_step];
            if (pItem != null)
            {
                pItem.pTrain = this.pTrain;
                pItem.start();
            }
        }
    }
}
