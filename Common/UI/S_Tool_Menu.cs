using Common_Robot2;

namespace Test1
{
    public class S_Tool_Menu : S_UI
    {
        public MenuStrip main_menu;
        public string text="";

        public S_Tool_Menu(string Name, C_Space space_parent, C_Space space) : 
            base(Name, space_parent, space)
        {
            this.Name = Name;
        }

        public override Task run_sub()
        {
            return Task.CompletedTask;
        }

        public void init()
        {
            space.vars_ui.TryAdd(this.key, this);

            main_menu = new MenuStrip();
            main_menu.Location = new Point(x, y);
            main_menu.Size = new Size(width, height);

            if (this.padding != "")
            {
                int iPadding = int.Parse(this.padding);
                main_menu.Padding = new Padding(iPadding);
            }


            if (this.color_bg != "")
            {
                main_menu.BackColor = ColorTranslator.FromHtml(this.color_bg);
            }
            if (this.color_font != "")
            {
                main_menu.ForeColor = ColorTranslator.FromHtml(this.color_font);
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
                    main_menu.BackgroundImage = Bitmap.FromFile(picture);
                }
                else
                {
                    main_menu.BackColor = ColorTranslator.FromHtml(this.color_bg);
                }
            }

            if (pFrm != null)
            {
                Action act = delegate ()
                {
                    if (pFrm != null)
                    {
                        Main.Add_Panel(this.pFrm, dock, this.group, space, main_menu);


                        //Main.WriteLine(this,this.Name);
                        //if (space.vars_ui.ContainsKey(this.group) == false)
                        //{
                        //    MessageBox.Show("没有这个group=" + this.group);
                        //    return;
                        //}
                        //S_UI pParent = space.vars_ui[this.group];
                        //if (pParent.GetType().Name == "S_Form")
                        //{
                        //    pFrm.Controls.Add(main_menu);
                        //}
                        //else if (pParent.GetType().Name == "S_Panel_Map")
                        //{
                        //    if (space.vars_ui.ContainsKey(pParent.key_parent) == false)
                        //    {
                        //        MessageBox.Show("没有这个group=" + pParent.key_parent);
                        //        return;
                        //    }
                        //    var pParent2 = space.vars_ui[pParent.key_parent];
                        //    S_Panel pParent3 = (S_Panel)pParent2;
                        //    pParent3.panel.Controls.Add(main_menu);
                        //}

                        //main_menu.BringToFront();
                        //switch (this.dock)
                        //{
                        //    case "left":
                        //        main_menu.Dock = DockStyle.Left;
                        //        break;
                        //    case "right":
                        //        main_menu.Dock = DockStyle.Right;
                        //        break;
                        //    case "top":
                        //        main_menu.Dock = DockStyle.Top;
                        //        break;
                        //    case "bottom":
                        //        main_menu.Dock = DockStyle.Bottom;
                        //        break;
                        //    case "fill":
                        //        main_menu.Dock = DockStyle.Fill;
                        //        break;
                        //} 
                        
                    }
                };
                if (pFrm != null)
                    pFrm.BeginInvoke(act, null);
            }
        }
    }
}
