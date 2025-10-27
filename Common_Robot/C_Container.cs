using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common_Robot2
{

    //变量容器，每一种类型，只有一个
    public class C_Container
    {
        public string Name = "";

        public ConcurrentDictionary<string, C_Var> dic = new ConcurrentDictionary<string, C_Var>();

        public void save_var(string type, C_Var? pVar)
        {
            if (pVar != null)
            {
                dic[type] = pVar;
            }
        }

        public C_Var? read_var(string type)
        {
            if (dic.ContainsKey(type))
            {
                return dic[type];
            }
            return null;
        }

        public void remove_var(string str_type)
        {
            dic.TryRemove(str_type,out C_Var tmp);
        }

        public bool contains(string str_type)
        {
            return dic.ContainsKey(str_type);
        }
    }
}