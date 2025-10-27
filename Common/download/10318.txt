
using Common_Robot2;
using System.Collections.Concurrent;

namespace Test1
{
    public class C_Train : I_Train
    {

        private string ID = "";
        private string time_id = "";

        private ConcurrentDictionary<string, C_Var> vars = new ConcurrentDictionary<string, C_Var>();
        private List<C_Node> ran_step=new List<C_Node>();


        public static ConcurrentDictionary<string, I_Train> dic_trains = new ConcurrentDictionary<string, I_Train>();
        public static List<I_Train> list_trains = new List<I_Train>();
        public static int id_count = 1;

        private string _mode = "";
        public string mode
        {
            get
            {
                return this._mode;
            }

            set
            {
                _mode = value;
            }
        }

        public C_Train(string ID)
        {
            this.set_ID(ID);


            if (this.ID == "" || this.ID == null)
            {
                this.ID = "" + id_count;
                id_count++;
            }
            I_Train? pTrain2;
            if (list_trains.Count > 10)
            {
                lock (list_trains)
                {
                    I_Train item = list_trains[0];
                    if (item != null)
                    {
                        dic_trains.TryRemove(item.get_ID(), out pTrain2);
                        list_trains.Remove(item);
                    }
                }
            }

            if (dic_trains.ContainsKey(this.ID))
            {
                dic_trains.TryRemove(this.ID, out pTrain2);
            }
            bool bSucess=dic_trains.TryAdd(this.ID, this);
            if (bSucess==false)
            {
                dic_trains.TryRemove(this.ID,out pTrain2);
                dic_trains.TryAdd(this.ID, this);
            }
            list_trains.Add(this);
        }


        public string get_ID()
        {
            return ID;
        }

        public string get_Time_ID()
        {
            return time_id;
        }

        public ConcurrentDictionary<string, C_Var> get_Vars()
        {
            return this.vars;
        }


        public void set_ID(string value)
        {
            this.ID= value;   
        }

        public void set_Time_ID(string value)
        {
            this.time_id= value;
        }

        public void set_Vars(ConcurrentDictionary<string, C_Var> vars)
        {
            this.vars = vars;
        }

        public List<C_Node> get_ran_step()
        {
            return this.ran_step;
        }

        public void set_ran_step(List<C_Node> value)
        {
            this.ran_step = value;
        }


    }
}