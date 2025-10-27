using Common_Robot2;
using ConverxHull;
using Newtonsoft.Json.Linq;
using Swan;
using System.Collections;

namespace Test1
{

    public class F_Call_Space : C_Node
    {

        public string space_name = "";
        public string key_array = "";
        public string item_type = "";
        public string jarray = "";
        public string map = "";
        public string partial = "";

        public string var1_name = "";
        public string var1_value = "";
        public string var1_type = "";


        public string var2_name = "";
        public string var2_value = "";
        public string var2_type = "";

        public string var3_name = "";
        public string var3_value = "";
        public string var3_type = "";

        public string var1_return_name = "";
        public string var1_return_var = "";

        public string key_return_array = "";


        public F_Call_Space(string name, C_Space space_parent, C_Space space) :
            base(name,space_parent, space)
        {
        }



        public override Task run_sub()
        {
            run_sub_main();
            return Task.CompletedTask;
        }


        public void run_sub_main()
        {
            Main.WriteLine(this, " F_Call_Space 开始");

            List<C_Space> spaces = new List<C_Space>();

            JArray arr = null;
            if (this.jarray != "")
            {
                if (this.jarray.StartsWith("["))
                {
                    arr = JArray.Parse(this.jarray);
                }
                else
                {
                    arr = new JArray();
                    string data = space.read_string(this,this.jarray);
                    string data2 = data;
                    data2 = data2.Replace("\r", ";");
                    data2 = data2.Replace("\n", ";");
                    string[] strSplit = data2.Split(";");
                    for(var i=0;i<strSplit.Length;i++)
                    {
                        string[] strSplit2 = strSplit[i].Split("=");
                        if (strSplit2.Length == 2)
                        {
                            JObject obj = new JObject();
                            obj["name"] = strSplit2[0];
                            obj["value"] = strSplit2[1];
                            arr.Add(obj);
                        }
                    }
                }
            }

            if (this.var1_name != "")
            {
                JObject obj = new JObject();
                obj["name"] = this.var1_name;
                obj["value"] = this.var1_value;
                obj["type"] = this.var1_type;
                arr.Add(obj);
            }

            if (this.var2_name != "")
            {
                JObject obj = new JObject();
                obj["name"] = this.var2_name;
                obj["value"] = this.var2_value;
                obj["type"] = this.var2_type;
                arr.Add(obj);
            }

            if (this.var3_name != "")
            {
                JObject obj = new JObject();
                obj["name"] = this.var3_name;
                obj["value"] = this.var3_value;
                obj["type"] = this.var3_type;
                arr.Add(obj);
            }

            C_Thread_Data? pThread_Data=null;

            bool bPartial = false;
            if (this.partial == "1")
            {
                bPartial = true;
            }

            if (this.key_array=="")
            {
                C_Data pData2 = new C_Data(space);
                pThread_Data = new C_Thread_Data(bPartial,this, spaces, space,space_parent, pData2, space_name, "0");
                pThread_Data.space_new.copy_from(space, this);
                pThread_Data.space_new.save_vars(pThread_Data.space_new.pTrain, this, "_data", "C_Data", pData2);
                
                if (arr != null)
                {
                    for (int i = 0; i < arr.Count; i++)
                    {
                        JObject obj = (JObject)arr[i];
                        string? name = obj["name"]?.ToString();
                        string? value = obj["value"]?.ToString();
                        string? v_type = obj["type"]?.ToString();
                        if (value.StartsWith("@"))
                        {
                            var obj2 = this.read_var(value.Substring(1), "");
                            pThread_Data.space_new.save_vars(pThread_Data.space_new.pTrain, this, name, "", obj2);
                            this.space.save_vars(pThread_Data.space_new.pTrain, this, name, "", obj2);
                        }
                        else
                        {
                            if (v_type == "C_Point3D")
                            {
                                var obj2 = new C_Point3D(value, ",", false);
                                pThread_Data.space_new.save_vars(pThread_Data.space_new.pTrain, this, name, "C_Point3D", obj2);
                                this.space.save_vars(pThread_Data.space_new.pTrain, this, name, "C_Point3D", obj2);
                            }
                            else
                            {
                                pThread_Data.space_new.save_vars(pThread_Data.space_new.pTrain, this, name, "", value);
                                this.space.save_vars(pThread_Data.space_new.pTrain, this, name, "", value);
                            }
                        }
                    }
                }

                Thread thread = new Thread(pThread_Data.ThreadFun);
                thread.Start();

                C_Data.threads.Add(thread);
            }
            else
            {
                IList? pList = (IList?)this.read_var(this.key_array, "");
                if (pList == null)
                {
                    MessageBox.Show(this.Name+ " error key_array没有数据！");
                    return;
                }
                for (var i = 0; i < pList.Count; i++)
                {
                    var item = pList[i];
                    pThread_Data = new C_Thread_Data(bPartial,this, spaces, space, space_parent, null, space_name, i + "");
                    pThread_Data.space_new.copy_from(space, this);
                    pThread_Data.space_new.save_vars(pThread_Data.space_new.pTrain, this, "_data",this.item_type, item);
                    if (arr != null)
                    {
                        for (int j = 0; i < arr.Count; i++)
                        {
                            JObject obj = (JObject)arr[j];
                            string? name = obj["name"]?.ToString();
                            string? value = obj["value"]?.ToString();
                            pThread_Data.space_new.save_vars(pThread_Data.space_new.pTrain, this, name, "string", value);
                            this.space.save_vars(pThread_Data.space_new.pTrain, this, name, "string", value);
                        }
                    }

                    Thread thread = new Thread(pThread_Data.ThreadFun);
                    thread.Start();
                    C_Data.threads.Add(thread);
                }
            }

            Thread.Sleep(1);

            while (check_finished(spaces) == false)
            {
                Thread.Sleep(1);
            }

            space.copy_from(pThread_Data.space_new,this);

            if (pThread_Data != null)
            {
                this.save_var("_log_file", "", pThread_Data.space_new.file_log);
            }

            if (var1_return_var != "")
            {
                this.save_var(var1_return_var, "", this.read_var(this.var1_return_name, ""));
            }
            if (key_return_array != "")
            {
                IList pArray = (IList)space.read_vars(pTrain, this, this.key_return_array, "List<T>");
                if (pArray == null)
                {
                    if (item_type == "C_Point3D")
                    {
                        pArray = new List<C_Point3D>();
                    }
                    else
                    {
                        pArray = new List<string>();
                    }
                    space.save_vars(pTrain, this, this.key_return_array, "List<T>", pArray);
                }

                pArray.Add(this.read_var(this.var1_return_name, ""));
            }

            Main.WriteLine(this,Name + " F_Call_Space 结束");
        }


        public bool check_finished(List<C_Space> spaces)
        {
            for (var i = 0; i < spaces.Count; i++)
            {
                var space2 = spaces[i];
                if (space2 != null && space2.finished == false)
                {
                    return false;
                }
            }
            return true;
        }

        public override void init()
        {
            
        }
    }
}
