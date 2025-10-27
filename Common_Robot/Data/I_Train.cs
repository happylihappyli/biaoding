
using Common_Robot2;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common_Robot2
{
    public interface I_Train
    {
        string mode { get; set; }

        public string get_ID();// { get => id; set => id = value; }
        public void set_ID(string ID);


        public List<C_Node> get_ran_step();// = "";
        public void set_ran_step(List<C_Node> ID);

        public string get_Time_ID();// = "";
        public void set_Time_ID(string ID);

        /// <summary>
        /// 小火车变量
        /// </summary>
        public ConcurrentDictionary<string, C_Var> get_Vars();// = new Dictionary<string, C_Var>();
        public void set_Vars(ConcurrentDictionary<string, C_Var> vars);


    }
}
