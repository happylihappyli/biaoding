namespace Common_Robot2
{

    public class C_Var
    {
        public string name = "";
        public string type = "";
        public string value = "";
        private object? pObj = null;

        public C_Var(string name, string value,string type,object? obj)
        {
            this.name = name;
            this.value = value;
            this.type = type;
            this.pObj = obj;
        }

        public object get_obj()
        {
            return pObj;
        }
    }
}
