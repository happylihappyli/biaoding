using Common_Robot;
using Common_Robot2;
using ConverxHull;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test1
{
    public class S_Form : S_UI
    {
        //public int x = 0;
        //public int y = 0;
        //public int width = 100;
        //public int height = 50;
        //public string css = "left";

        public S_Form(string Name, C_Space space_parent, C_Space space) :
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

            if (this.color_bg != "")
            {
                pFrm.BackColor = ColorTranslator.FromHtml(this.color_bg);
            }
            if (this.color_font != "")
            {
                pFrm.ForeColor = ColorTranslator.FromHtml(this.color_font);
            }
            if (this.padding != "")
            {
                int iPadding = int.Parse(this.padding);
                pFrm.Padding = new Padding(iPadding);
            }


            if (this.picture.StartsWith("http:") || this.picture.StartsWith("https:"))
            {
                string url = this.picture;// "http://www.sndvision.cn/upload/url.php?id=12261";
                string file = Application.StartupPath + "\\tmp_" + this.key + ".png";

                if (File.Exists(file) == false)
                {
                    Tools.download_file(url, file);
                }

                this.picture = file;

                if (this.picture != "")
                {
                    pFrm.BackgroundImage = Bitmap.FromFile(picture);
                }
                else
                {
                    pFrm.BackColor = ColorTranslator.FromHtml(this.color_bg);
                }
            }
        }
    }
}
