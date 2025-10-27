using Common_Robot2;
using FunnyMath;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Three.Net.Objects;

namespace Test1.Common.Data
{
    public class Quick
    {
        public C_Space space;
        public C_Space space_parent;

        public Quick(object a)
        {
            this.space = ((Two_Space)a).space;
            this.space_parent = ((Two_Space)a).space_parent;
            
        }

        public void 弹出消息(string msg="")
        {
            MessageBox.Show(msg);
        }

        public void 拍照(string msg = "")
        {

        }

        //public void 读取文本框(string key_ui,string key_save)
        //{
        //    S_TextBox pText= (S_TextBox)space.vars_ui[key_ui];
        //    string value=pText.pText1.Text;
        //    space.save_vars(null, null, key_save, "string", value);
        //}

        public void 字符串格式化(string key_save, string template, string keys)
        {
            string line = template;
            string[] strSplit = keys.Split(",");
            for(var i=0;i< strSplit.Length; i++)
            {
                string key = strSplit[i];
                string value=space.read_string(null,"@"+key);
                line= line.Replace("{"+i+"}", value);
            }

            space.save_vars(null, null, key_save, "string", line);
        }

        public void 过滤回车换行(string key_read,string key_save)
        {

            string line = (string)space.read_vars(space.pTrain,null, key_read);

            line = line.Replace("\r", "");
            line = line.Replace("\n", "");

            space.save_vars(space.pTrain, null,key_save, "string", line);
        }


        public void 调用(string name,string wait,string str_new)
        {
            if (wait != "")
            {
                Thread.Sleep(int.Parse(wait));
            }
            if (space.vars_step.ContainsKey(space.Name + name) == false)
            {
                return;
            }
            C_Node pStep = space.vars_step[space.Name + name];
            if (str_new == "1")
            {
                Random rnd = new Random();
                int train_id = rnd.Next(100, 900);
                space.pTrain = CommonMain.create_train("Call_" + train_id);

                pStep.Run(space.pTrain);
            }
            else
            {
                pStep.Run(space.pTrain);
            }
        }


        //public void 列表组件设置列(string key_ui, string line)
        //{
        //    S_List_View control = (S_List_View)space.vars_ui[key_ui];
        //    ListView ui_item = control.ui_item;

        //    string[] strSplit = line.Split(";");

        //    Action act = delegate ()
        //    {
        //        if (ui_item != null)
        //        {
        //            ui_item.View = View.Details;
        //            ui_item.FullRowSelect = true;

        //            ui_item.Columns.Clear();


        //            for (var i = 0; i < strSplit.Length; i++)
        //            {
        //                var line = strSplit[i];
        //                var strSplit2 = line.Split(",");
        //                var col = ui_item.Columns.Add(strSplit2[0]);
        //                if (strSplit2.Length > 1)
        //                {
        //                    try
        //                    {
        //                        col.Width = int.Parse(strSplit2[1]);
        //                    }
        //                    catch (Exception ex)
        //                    {

        //                    }
        //                }
        //            }

        //        }
        //    };
        //    if (ui_item != null)
        //        ui_item.BeginInvoke(act, null);
        //}


        //public void 列表添加内容(string key_ui, string text, string split)
        //{
        //    S_List_View control = (S_List_View)space.vars_ui[key_ui];
        //    ListView ui_item = control.ui_item;


        //    Action act = delegate ()
        //    {
        //        if (ui_item != null)
        //        {
        //            string line = space.read_string(null, text);

        //            string[] strSplit = line.Split(split);
        //            ListViewItem item = ui_item.Items.Add(strSplit[0]);

        //            for (var i = 1; i < strSplit.Length; i++)
        //            {
        //                item.SubItems.Add(strSplit[i]);
        //            }
        //        }
        //    };
        //    if (ui_item != null)
        //        ui_item.BeginInvoke(act, null);

        //}

        //public void 读取列表内容(string key_ui, string key_save, string split)
        //{
        //    S_List_View control = (S_List_View)space.vars_ui[key_ui];
        //    ListView ui_item = control.ui_item;


        //    int icount = 0;
        //    Action act = delegate ()
        //    {
        //        if (ui_item != null)
        //        {
        //            if (ui_item.SelectedItems.Count > 0)
        //            {
        //                ListViewItem item = ui_item.SelectedItems[0];
        //                string line = item.Text;

        //                if (key_save.IndexOf(",") > -1)
        //                {
        //                    string[] strSplit = key_save.Split(',');
        //                    space.save_vars(space.pTrain, null, strSplit[0], "string", line);

        //                    for (var i = 1; i < strSplit.Length; i++)
        //                    {
        //                        if (i < item.SubItems.Count)
        //                        {
        //                            line = item.SubItems[i].Text;
        //                            space.save_vars(space.pTrain, null, strSplit[i], "string", line);
        //                        }
        //                        else
        //                        {
        //                            space.save_vars(space.pTrain, null, strSplit[i], "string", "");
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    for (var i = 1; i < item.SubItems.Count; i++)
        //                    {
        //                        line += split + item.SubItems[i].Text;
        //                    }
        //                    space.save_vars(space.pTrain, null, key_save, "string", line);
        //                }
        //                icount = 10000;
        //            }
        //        }
        //    };
        //    if (ui_item != null)
        //        ui_item.BeginInvoke(act, null);

        //    while (icount < 1000)
        //    {
        //        icount++;
        //        Thread.Sleep(10);
        //    }
        //}


        //public void 显示文本框(string key_ui, string string_read, string digit)
        //{
        //    S_TextBox control = (S_TextBox)space.vars_ui[key_ui];
        //    var ui_item = control.pText1;

        //    Action act = delegate ()
        //    {
        //        if (ui_item != null)
        //        {
        //            string value = (string)space.read_string(null, "@"+string_read);
        //            ui_item.Text = value;

        //        }
        //    };
        //    if (ui_item != null)
        //        ui_item.BeginInvoke(act, null);
        //}

        public void 表达式计算(string key_save, string string_read)
        {
            string line = (string)space.read_vars(space.pTrain, null, string_read);

            try
            {
                var ctx = new FunnyMath_Context(space,null);
                double a = Parser.Parse(line).Eval(ctx);//"2 * A * 矩形面积(10,20)"

                space.save_vars(space.pTrain,null, key_save, "double", a);
            }
            catch (Exception ex)
            {
                Main.WriteLine(ex.ToString());
            }
        }

        public void 显示隐藏面板(string key_ui)
        {
            S_Panel? pItem=null;
            if (space.vars_ui.ContainsKey(key_ui))
            {
                pItem = (S_Panel)space.vars_ui[key_ui];
            }
            else
            {
                MessageBox.Show(key_ui + "UI没有这个控件");
            }

            if (pItem == null)
            {
                return;
            }

            Action act = delegate ()
            {
                if (pItem.panel != null)
                {
                    pItem.panel.Visible = !pItem.panel.Visible;
                }
            };
            if (pItem.panel != null)
                pItem.panel.BeginInvoke(act, null);
        }
    }
}
