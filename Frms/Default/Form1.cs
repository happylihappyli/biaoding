using Common_Robot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Common_Robot.C_Mp3;
using Test1.Common.Data;
using System.IO;
using System.Web;

namespace Test1
{
    public partial class Form1 : Form
    {
		C_Space space = new C_Space();
		C_Space space2 = new C_Space();
        C_Space space3 = new C_Space();
		public string unload_step="";
		public bool bEnv = false;


        public Form1()
        {
            InitializeComponent();
        }
        
        public void clear_module()
        {
        }

		
				public void env(C_Space space_parent,C_Space space,string space_name,string tag){
				space.finished=true;
		}



        public void load_init(C_Space space_parent,C_Space space,string space_name){
							{
				 S_Form pItem = new S_Form("form",space_parent,space); 
pItem.pFrm=this;
				 pItem.key="k4";
				 pItem.init();
				 space.vars.vars_ui.Add("k4",pItem);
			}
			{
				 S_TextBox pItem = new S_TextBox("命令",space_parent,space); 
				 pItem.set_area(0,50,600,50);
				 pItem.text=HttpUtility.UrlDecode("");
				 pItem.key="k1";
				 pItem.group="k4";
				 pItem.pFrm=this;
				 pItem.init();
				 space.vars.vars_ui.Add("k1",pItem);
			}
			{
				S_Button pItem = new S_Button("发送",space_parent,space); 
				 pItem.set_area(600,50,100,50);
				 pItem.text="发送";
				 pItem.key="k2";
				 pItem.group="k4";
				 pItem.pFrm=this;
				 pItem.init();
				 space.vars.vars_ui.Add("k2",pItem);
			}
			{
				 S_TextBox pItem = new S_TextBox("结果",space_parent,space); 
				 pItem.set_area(0,100,700,350);
				 pItem.text=HttpUtility.UrlDecode("");
				 pItem.key="k9";
				 pItem.group="k4";
				 pItem.pFrm=this;
				 pItem.init();
				 space.vars.vars_ui.Add("k9",pItem);
			}
			
			 Action act = delegate (){
				 this.Width =700+20;
				 this.Height =450+50;
				 this.Text ="form";
			};
                if (this != null)
                    this.BeginInvoke(act, null);
			{
				E_Button_Event p = new E_Button_Event("按钮事件",space_parent,space); 
				 p.key_ui="k2";
				 p.key="2";
				 space.vars.vars_step.Add("按钮事件",p);
			}
			{
				S_TextBox_Read p = new S_TextBox_Read("读取文本框",space_parent,space); 
				 p.key_ui="k1";
				 p.key_save="命令";
				 p.key="45";
				 space.vars.vars_step.Add("读取文本框",p);
			}
			{
				 E_Form_Load p= new E_Form_Load ("启动运行",space_parent,space);
				 space.vars.vars_step.Add("启动运行",p);
			}
			{
				S_Cmd p = new S_Cmd("命令行模块",space_parent,space); 
				 p.path="D:";
				 p.encode="gb2312";
				 p.command="cmd.exe";
				 p.all_active_mode=false;
				 p.key="-2";
				 space.vars.vars_step.Add("命令行模块",p);
			}
			{
				S_Cmd_Send p = new S_Cmd_Send("发送cmd命令",space_parent,space); 
				 p.key_step="命令行模块";
				 p.command="@命令";
				 p.wait="100";
				 p.all_active_mode=false;
				 p.key="-1";
				 space.vars.vars_step.Add("发送cmd命令",p);
			}
			{
				E_Cmd_Event p = new E_Cmd_Event("命令行事件",space_parent,space); 
				 p.key_step="命令行模块";
				 p.key_msg="返回消息";
				 p.all_active_mode=false;
				 p.key="-7";
				 space.vars.vars_step.Add("命令行事件",p);
			}
			{
				S_TextBox_Show p = new S_TextBox_Show("显示文本框",space_parent,space); 
				 p.key_ui="k9";
				 p.msg="@返回消息";
				 p.mode="append";
				 p.all_active_mode=false;
				 p.key="-9";
				 space.vars.vars_step.Add("显示文本框",p);
			}
			{
				E_Textbox_Event p = new E_Textbox_Event("文本框事件",space_parent,space); 
				 p.key_ui="k1";
				 p.all_active_mode=false;
				 p.key="-6";
				 space.vars.vars_step.Add("文本框事件",p);
			}

				 set_input(space,"按钮事件","文本框事件");
				 set_input(space,"读取文本框","按钮事件");
				 set_input(space,"命令行模块","启动运行");
				 set_input(space,"发送cmd命令","读取文本框");
				 set_input(space,"显示文本框","命令行事件");

			{
				 E_Button_Event p = (E_Button_Event)space.vars.vars_step["按钮事件"];
				 p.init();
			}
			{
				 E_Cmd_Event p = (E_Cmd_Event)space.vars.vars_step["命令行事件"];
				 p.init();
			}
			{
				 E_Textbox_Event p = (E_Textbox_Event)space.vars.vars_step["文本框事件"];
				 p.init();
			}

			{
				 var p_load=space.vars.vars_step["启动运行"];
				 Thread.Sleep(500);
				 p_load.Run(null);
			}

				space.vars.bClosingWindows = false;
				space.vars.bAutoMode = true;
				Task.Run(() => {
					space.检查步骤状态();
				});
        }
		
		public void set_input(C_Space space, string name, string from,bool true_false=true)
		{
			var p1 = space.vars.vars_step[name];
			var p2 = space.vars.vars_step[from];
			p1.init_input(p2, true_false);
		}
		

        private void Form1_Load(object sender, EventArgs e)
        {
		
			EncodingProvider provider = CodePagesEncodingProvider.Instance;
			Encoding.RegisterProvider(provider);

			string name = Environment.MachineName;
			string strFile = Application.StartupPath + "\\set_" + name + ".ini";
			if (File.Exists(strFile) == false)
			{
				string file1 = Application.StartupPath + "\\set.ini";
				if (File.Exists(file1))
					File.Copy(file1, strFile);
			}

			IniFile myIni = new IniFile(strFile);
			string serial = myIni.ReadString("main", "serial", "");
			
			C_Space_Load.form1 = this;

			m1.Path_Main= Application.StartupPath;
            
            Label pLabel=new Label();
			
			Console.WriteLine("x 正在加载数据");
			pLabel.Text = "正在加载数据...";
			pLabel.Location = new Point(200, 200);
			this.Controls.Add(pLabel);

			space.console.pLog = (ILog)Logger.GetInstance(space);
            space2.console.pLog = (ILog)Logger.GetInstance(space2);
            space3.console.pLog = (ILog)Logger.GetInstance(space3);

			space.vars.save_vars(null, "#serial", "string", serial);
			space2.vars.save_vars(null, "#serial", "string", serial);


			Task.Run(() =>
			{
				space2.vars.copy_from(space);
				try
				{
					Console.WriteLine("x 正在env");
					env(space, space2, "env","");
					Console.WriteLine("x end env");
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
					Console.WriteLine(ex.ToString());
				}
				
				//if (this.bEnv)
                {
                    int iCount = 0;
                    while (space2.finished == false)
                    {
                        iCount++;
                        if (iCount > 30)
                        {
                            break;
                        }
                        Thread.Sleep(100);
                    }
                    space2.vars.bClosingWindows = true;
                    space.vars.copy_from(space2);
                }


				try
				{
					space3.vars.save_vars(null, "#serial", "string",serial);
					space3.vars.copy_from(space);
					Console.WriteLine("x 正在load_init");
					load_init(space, space3, "main");
					Console.WriteLine("x end load_init");
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
					Console.WriteLine(ex.ToString());
				}

				
				Action act = delegate ()
				{
					this.Controls.Remove(pLabel);
					this.CenterToScreen();
				};
				if (this != null)
					this.BeginInvoke(act, null);

			});


            
        }

        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
			if (this.unload_step!=""){
				var p1 = space3.vars.vars_step[this.unload_step];
				p1.Run(null);
			}
			Thread.Sleep(500);

			
			space.vars.bClosingWindows = true;
			space2.vars.bClosingWindows = true;
			space3.vars.bClosingWindows = true;


			
		}
    }
}
