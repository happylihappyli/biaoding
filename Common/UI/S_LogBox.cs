using Common_Robot2;
using ConverxHull;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test1
{
    public class S_LogBox : S_UI
    {
        public TabControl tabControl1;
        public TextBox pText1;
        public TextBox pText2;
        public TextBox pText3;
        public TabPage page1;
        public TabPage page2;
        public TabPage page3;
        public string text = "textbox1";
        public string read_key = "";
        public string save_key = "textbox1_value";
        public S_LogBox(string name, C_Space space_parent, C_Space space) :
            base(name, space_parent, space)
        { 
        }

        public override void init()
        {
            space.vars_ui.TryAdd(this.key, this);

            tabControl1 = new TabControl();
            page1 = new TabPage();
            page2 = new TabPage();
            page3 = new TabPage();

            page1.Text = "日志1";
            page2.Text = "日志2";
            page3.Text = "日志3";
            tabControl1.TabPages.Add(page1);
            tabControl1.TabPages.Add(page2);
            tabControl1.TabPages.Add(page3);

            tabControl1.Location = new System.Drawing.Point(x, y);
            tabControl1.Size = new Size(width, height);

            pText1 = new TextBox();
            if (height > 50)
            {
                pText1.Multiline = true;
                pText1.ScrollBars = ScrollBars.Both;
            }

            var font_name = this.font_name;
            if (font_name == "")
            {
                font_name = "宋体";
            }
            var font_size = this.font_size;
            if (this.font_size == "")
            {
                font_size = "12";
            }
            pText1.Font = new Font(font_name, float.Parse(font_size),
                FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));

            if (this.color_bg != "")
            {
                pText1.BackColor = ColorTranslator.FromHtml(this.color_bg);
            }
            if (this.color_font != "")
            {
                pText1.ForeColor = ColorTranslator.FromHtml(this.color_font);
            }
            pText1.BorderStyle = BorderStyle.Fixed3D;
            pText1.Text = text;
            page1.Controls.Add(pText1);
            pText1.Dock = DockStyle.Fill;



            pText2 = new TextBox();
            pText2.Location = new System.Drawing.Point(x, y);
            pText2.Size = new Size(width, height);
            if (height > 50)
            {
                pText2.Multiline = true;
                pText2.ScrollBars = ScrollBars.Both;
            }

            pText2.Font = new Font(font_name, float.Parse(font_size),
                FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));

            pText2.BorderStyle = BorderStyle.Fixed3D;

            if (this.color_bg != "")
            {
                pText2.BackColor = ColorTranslator.FromHtml(this.color_bg);
            }
            if (this.color_font != "")
            {
                pText2.ForeColor = ColorTranslator.FromHtml(this.color_font);
            }


            pText2.Text = text;
            page2.Controls.Add(pText2);
            pText2.Dock = DockStyle.Fill;





            pText3 = new TextBox();
            pText3.Location = new System.Drawing.Point(x, y);
            pText3.Size = new Size(width, height);
            if (height > 50)
            {
                pText3.Multiline = true;
                pText3.ScrollBars = ScrollBars.Both;
            }

            pText3.Font = new Font(font_name, float.Parse(font_size),
                FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));

            pText3.BorderStyle = BorderStyle.Fixed3D;

            if (this.color_bg != "")
            {
                pText3.BackColor = ColorTranslator.FromHtml(this.color_bg);
            }
            if (this.color_font != "")
            {
                pText3.ForeColor = ColorTranslator.FromHtml(this.color_font);
            }


            pText3.Text = text;
            page3.Controls.Add(pText3);
            pText3.Dock = DockStyle.Fill;

            Action act = delegate ()
            {
                if (pFrm != null)
                {
                    Main.Add_Panel(this.pFrm, dock, this.group, space, tabControl1);
                }
            };
            if (pFrm != null)
                pFrm.BeginInvoke(act, null);
        }


        public override Task run_sub()
        {
            //Main.WriteLine(this,this.Name + " run");

            string value = "";
            if (pTrain.get_Vars().ContainsKey(read_key))
            {
                var tmp = pTrain.get_Vars()[read_key].get_obj();
                if (tmp.GetType() == typeof(string))
                {
                    value = (string)tmp;
                }
                else
                {
                    value = tmp.ToString();
                }
            }

            Action act = delegate ()
            {
                if (pText1 != null)
                {
                    pText1.Text = value;
                }
            };
            if (pText1 != null)
                pText1.BeginInvoke(act, null);

            return Task.CompletedTask;
        }
    }
}
