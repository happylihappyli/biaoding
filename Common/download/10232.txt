
using Common_Robot2;


namespace Test1
{
    public class U_PictureBox_Show : C_Node
    {
        public PictureBox? picture_box;
        public Panel? panel;

        private string _url = "";
        private string _key_ui = "";
        private string _data_in = "";

        private string _key_pic = "";
        private string _scroll_x = "";
        private string _scroll_y = "";

        public string key_ui { get => _key_ui; set => _key_ui = value; }
        public string url { get => _url; set => _url = value; }
        public string data_in { get => _data_in; set => _data_in = value; }
        public string key_pic { get => _key_pic; set => _key_pic = value; }
        public string scroll_x { get => _scroll_x; set => _scroll_x = value; }
        public string scroll_y { get => _scroll_y; set => _scroll_y = value; }

        public U_PictureBox_Show(string name, C_Space space_parent, C_Space space) :
            base(name,space_parent, space)
        {
        }

        public override void init()
        {
            if (key_ui == "")
            {
                MessageBox.Show("S_PictureBox_Show key_ui==null!");
                return;
            }


            S_PictureBox? pItem=null;
            pItem = (S_PictureBox?)Main.get_ui_from_parent_vars_steps(space, key_ui);
            if (pItem == null)
            {
                MessageBox.Show(this.Name + "UI没有这个控件");
                return;
            }


            picture_box = pItem?.picture_box;


            panel = pItem.panel;

        }

        public override Task run_sub()
        {
            run_sub_main();
            return Task.CompletedTask;
        }

        private void run_sub_main()
        {
            Action? act = delegate ()
            {
                if (picture_box != null)
                {
                    if (data_in != "")
                    {
                        key_pic = data_in;
                    }
                    if (key_pic != "")
                    {
                        if (space.contains(pTrain, this, key_pic, "Bitmap"))
                        {
                            Bitmap? bmp = (Bitmap?)this.read_var(key_pic, "Bitmap");
                            if (bmp != null)
                                lock(bmp)
                                {
                                    try
                                    {
                                        Bitmap? bmp2 = Main.CopyBmp(bmp);
                                        lock (bmp2)
                                        {
                                            picture_box.Image = bmp2;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Main.WriteLine(this,ex.ToString());
                                    }
                                }
                        }
                    }
                    else
                    {
                        if (_url.StartsWith("@"))
                        {
                            var key = _url.Substring(1);
                            if (space.contains(pTrain, this, key, "Bitmap"))
                            {
                                Bitmap? bmp = (Bitmap?)this.read_var(key, "Bitmap");
                                try
                                {
                                    if (bmp != null)
                                    {
                                        Bitmap? bmp2 = Main.CopyBmp(bmp);
                                        lock (bmp2)
                                        {
                                            picture_box.Image = bmp2;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Main.WriteLine(this,ex.ToString());
                                }
                            }
                        }
                        else
                        {
                            picture_box.ImageLocation = _url;
                            picture_box.Refresh();
                            picture_box.SizeMode = PictureBoxSizeMode.AutoSize;
                            picture_box.Parent.Refresh();
                        }
                    }
                }
                if (panel != null )
                {
                    if (scroll_x != "")
                    {
                        string? str_x1 = this.read_string(scroll_x);
                        int x1 = 0;
                        if (str_x1 != null && str_x1 != "") x1 = int.Parse(str_x1);

                        if (x1 > 0 && picture_box!=null)
                        {
                            if (x1 > panel.HorizontalScroll.Maximum)
                            {
                                panel.HorizontalScroll.Value = panel.HorizontalScroll.Maximum;    // dy;
                                picture_box.Left = -panel.HorizontalScroll.Maximum;
                            }
                            else
                            {
                                panel.HorizontalScroll.Value = x1;    // dx;
                                picture_box.Left = -x1;
                            }
                        }
                    }

                    if (scroll_y != "")
                    {
                        string? str_y1 = this.read_string(scroll_y);
                        int y1 = 0;
                        if (str_y1 != null && str_y1 != "") y1 = int.Parse(str_y1);

                        if (y1 > 0 && picture_box != null)
                        {
                            if (y1 > panel.VerticalScroll.Maximum)
                            {
                                panel.VerticalScroll.Value = panel.VerticalScroll.Maximum;    // dy;
                                picture_box.Top = -panel.VerticalScroll.Maximum;
                            }
                            else
                            {
                                panel.VerticalScroll.Value = y1;    // dy;
                                picture_box.Top = -y1;
                            }
                        }
                    }
                }
            };
            if (picture_box != null)
                picture_box.BeginInvoke(act, null);
        }

    }
}
