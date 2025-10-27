using Common_Robot2;

namespace Test1
{
    public class S_TextBox : S_UI
    {
        public TextBox? pText1;
        public string text = "textbox1";
        public string read_key = "";

        public S_TextBox(string name, C_Space space_parent, C_Space space):
            base(name, space_parent, space)
        {
        }

        public override void init()
        {
            space.vars_ui.TryAdd(this.key, this);
            pText1 = new TextBox();
            pText1.Location = new System.Drawing.Point(x, y);
            pText1.Size = new Size(width, height);
            pText1.Multiline = true;
            if (height > 50)
            {
                pText1.ScrollBars = ScrollBars.Both;
            }
            else
            {
                pText1.AcceptsReturn = false;
            }
            var font_name = this.font_name;
            if (font_name == "")
            {
                font_name = "宋体";
            }
            var font_size = this.font_size;
            if (this.font_size == "")
            {
                font_size = "16";
            }
            pText1.Font = new Font(font_name, float.Parse(font_size),
                FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            pText1.BorderStyle = BorderStyle.Fixed3D;
            pText1.Text = text;


            if (text.StartsWith("@#"))
            {
                this.key_field = text.Substring(1);
                space.pUI_Update.TryAdd(this.key_field,this);
            }


            pText1.TextChanged += Text_Change;

            if (this.color_bg != "")
            {
                pText1.BackColor = ColorTranslator.FromHtml(this.color_bg);
            }
            if (this.color_font != "")
            {
                pText1.ForeColor = ColorTranslator.FromHtml(this.color_font);
            }


            Action act = delegate ()
            {
                if (pFrm != null)
                {
                    Main.Add_Panel(this.pFrm, dock, this.group, space, pText1);
                }
            };
            if (pFrm != null)
                pFrm.BeginInvoke(act, null);


            if (pFrm != null)
                pFrm.BeginInvoke(act, null);


            this.event_change_value += S_TextBox_event_change_value;
        }

        private void S_TextBox_event_change_value()
        {

            Action act = delegate ()
            {
                if (pFrm != null)
                {
                    pText1.Text = (string)this.data;
                }
            };
            if (pFrm != null)
                pFrm.BeginInvoke(act, null);
        }

        private void Text_Change(object? sender, EventArgs e)
        {
            if (this.key_field != "")
            {

                space.save_vars(pTrain,this, this.key_field, "string", pText1.Text);
            }
        }

        public override Task run_sub()
        {
            Main.WriteLine(this, " run");

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
