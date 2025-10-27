using Common_Robot2;

namespace Test1
{
    public class S_Panel : S_UI
    {
        public Panel panel;

        public S_Panel(string Name, C_Space space_parent, C_Space space) : 
            base(Name, space_parent, space)
        {
            this.Name = Name;
        }

        public override Task run_sub()
        { 
            return Task.CompletedTask;
        }

        public override void init()
        {
            space.vars_ui.TryAdd(this.key, this);

            panel = new Panel();
            panel.Location = new System.Drawing.Point(x, y);
            panel.Size = new Size(width, height);

            if (this.padding != "")
            {
                int iPadding = int.Parse(this.padding);
                panel.Padding = new System.Windows.Forms.Padding(iPadding);
            }


            if (this.color_bg != "")
            {
                panel.BackColor = ColorTranslator.FromHtml(this.color_bg);
            }
            if (this.color_font != "")
            {
                panel.ForeColor = ColorTranslator.FromHtml(this.color_font);
            }

            if (this.picture.StartsWith("http:") || this.picture.StartsWith("https:"))
            {
                string url = this.picture;
                string file = Application.StartupPath + "\\tmp_" + this.key + ".png";

                if (File.Exists(file) == false)
                {
                    Tools.download_file(url, file);
                }

                this.picture = file;

                if (this.picture != "")
                {
                    panel.BackgroundImage = Bitmap.FromFile(picture);
                }
                else
                {
                    panel.BackColor = ColorTranslator.FromHtml(this.color_bg);
                }
            }

            if (pFrm != null)
            {
                Action act = delegate ()
                {
                    if (pFrm != null)
                    {
                        Main.Add_Panel(this.pFrm, dock, this.group, space, panel);

                    }
                };
                if (pFrm != null)
                    pFrm.BeginInvoke(act, null);
            }
        }
    }
}
