using Renci.SshNet.Security;

namespace Test1
{
    public class C_Grid
    {
        public int rows=0;
        public string cols = "";

        Dictionary<string, string[]> data=new Dictionary<string, string[]>();

        //string[,] data = new string[2,2];
        public C_Grid(int rows,string cols)
        {
            this.rows = rows;
            this.cols = cols;

            string[] strSplit= cols.Split(',');

            for(var i = 0; i < strSplit.Length; i++)
            {
                string key = strSplit[i];
                data.Add(key, new string[rows]);
            }
        }

        public int count_row(int row,string value)
        {
            int count = 0;
            string[] strSplit = cols.Split(',');

            for (var i = 0; i < strSplit.Length; i++)
            {
                string key = strSplit[i];
                if (data[key][row] == value)
                {
                    count += 1;
                }
            }
            return count;
        }

        public void save(int i_row, string str_col, string value)
        {
            data[str_col][i_row] = value;
        }

        public string read(int i_row, string str_col)
        {
            return data[str_col][i_row];
        }
    }
}