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
using System.IO;

//--------这里添加命名空间---------
using Test1.Common.Data;
using Common_Robot;
using static Common_Robot.C_Mp3;

namespace Test1
{
    public partial class Form1 : Form
    {
        C_Space space = new C_Space();
        C_Space space2 = new C_Space();
        C_Space space3 = new C_Space();

        public Form1()
        {
            InitializeComponent();
        }
        public void clear_module()
        {
        }

		
		public void env(C_Space space_parent,C_Space space,string space_name,string tag){
			{
				S_Form_Load p= new S_Form_Load ("启动运行",space_parent,space);
				 space.vars.vars_step.Add("启动运行",p);
			}
			{
				S_Var_Set p = new S_Var_Set("变量设置angle_dif",space_parent,space); 
				 p.key_save="#angle_dif";
				 p.value="-69";
				 p.all_active_mode=false;
				 p.key="-2";
				 space.vars.vars_step.Add("变量设置angle_dif",p);
			}
			{
				S_Read_Robot p = new S_Read_Robot("读取机器人信息",space_parent,space); 
				 p.key_save="#robot1";
				 p.oz="155";
				 p.dz="0";
				 p.c2d="614";
				 p.bc2="670.52218";
				 p.tools_len="315";
				 p.bottom0="0, 0, 0";
				 p.bottom1="1.55, 0, 0";
				 p.bottom2="1.55, 0, 6.14";
				 p.bottom3="1.55, 0, 8.14";
				 p.bottom4="7.95, 0, 8.14";
				 p.bottom5="8.95, 0, 8.14";
				 p.rotate0="0, 0, 1";
				 p.rotate1="0, 1, 0";
				 p.rotate2="0, -1, 0";
				 p.rotate3="-1, 0, 0";
				 p.rotate4="0, -1, 0";
				 p.rotate5="-1, 0, 0";
				 p.all_active_mode=false;
				 p.key="-9";
				 space.vars.vars_step.Add("读取机器人信息",p);
			}
			{
				S_Read_Bottom p = new S_Read_Bottom("读取底面信息",space_parent,space); 
				 p.key_save="#pPlanet_Bottom";
				 p.min="10";
				 p.key_camera="#camera1_const";
				 p.file_matrix="d:/data/camera_to_bottom.txt";
				 p.key_save_matrix="#camera_to_bottom";
				 p.file="D:/data/bottom.txt";
				 p.all_active_mode=false;
				 p.key="-11";
				 space.vars.vars_step.Add("读取底面信息",p);
			}
			{
				S_Read_Ini p = new S_Read_Ini("读取ini信息",space_parent,space); 
				 p.file="@/Config.ini";
				 p.all_active_mode=false;
				 p.key="-12";
				 p.code="[{\"section\":\"Robot1\",\"key\":\"tx_camera_id\",\"key_save\":\"#camera_id\"},{\"section\":\"Robot1\",\"key\":\"tx_log_path\",\"key_save\":\"#log_path\"},{\"section\":\"rect\",\"key\":\"x0\",\"key_save\":\"#rect1/1/x\"},{\"section\":\"rect\",\"key\":\"y0\",\"key_save\":\"#rect1/1/y\"},{\"section\":\"rect\",\"key\":\"x1\",\"key_save\":\"#rect1/2/x\"},{\"section\":\"rect\",\"key\":\"y1\",\"key_save\":\"#rect1/2/y\"},{\"section\":\"rect2\",\"key\":\"x0\",\"key_save\":\"#rect2/1/x\"},{\"section\":\"rect2\",\"key\":\"y0\",\"key_save\":\"#rect2/1/y\"},{\"section\":\"rect2\",\"key\":\"x1\",\"key_save\":\"#rect2/2/x\"},{\"section\":\"rect2\",\"key\":\"y1\",\"key_save\":\"#rect2/2/y\"}]";
				 space.vars.vars_step.Add("读取ini信息",p);
			}
			{
				S_Read_Camera_Const p = new S_Read_Camera_Const("读取摄像头旋转参数",space_parent,space); 
				 p.key_save="#camera1_const";
				 p.file="d:/data/xyz_to_x1y1z1.txt";
				 p.all_active_mode=false;
				 p.key="-10";
				 space.vars.vars_step.Add("读取摄像头旋转参数",p);
			}
			{
				S_Read_Rect p = new S_Read_Rect("读取矩形框1",space_parent,space); 
				 p.key_read1="#rect1/1";
				 p.key_read2="#rect1/2";
				 p.index="#sys.pRect1/1";
				 p.key="30";
				 space.vars.vars_step.Add("读取矩形框1",p);
			}
			{
				S_Read_Rect p = new S_Read_Rect("读取矩形框",space_parent,space); 
				 p.key_read1="#rect2/1";
				 p.key_read2="#rect2/2";
				 p.index="#sys.pRect1/2";
				 p.key="302";
				 space.vars.vars_step.Add("读取矩形框",p);
			}

				 set_input(space,"变量设置angle_dif","启动运行");
				 set_input(space,"读取机器人信息","启动运行");
				 set_input(space,"读取底面信息","读取摄像头旋转参数");
				 set_input(space,"读取ini信息","启动运行");
				 set_input(space,"读取摄像头旋转参数","读取ini信息");
				 set_input(space,"读取矩形框1","读取ini信息");
				 set_input(space,"读取矩形框","读取矩形框1");


			{
				 var p_load=space.vars.vars_step["启动运行"];
				 Thread.Sleep(1);
				 p_load.Run();
			}

if (Space_List.spaces.ContainsKey(space_name + tag)){
    Space_List.spaces[space_name + tag] = space;
}else{
    Space_List.spaces.Add(space_name + tag, space);
}
space.vars.bClosingWindows = false;
space.vars.bAutoMode = true;
Task.Run(() => {
    space.检查步骤状态();
}); 
}
public void calculate(C_Space space_parent,C_Space space,string space_name,string tag){
			{
				S_Form_Load p= new S_Form_Load ("启动运行",space_parent,space);
				 space.vars.vars_step.Add("启动运行",p);
			}
			{
				S_Main1 p = new S_Main1("无序抓取计算",space_parent,space); 
				 p.length="-50";
				 p.key_camera="#camera1_const";
				 p.key_robot="#robot1";
				 p.key_bottom="#pPlanet_Bottom";
				 p.all_active_mode=false;
				 p.key="-1";
				 space.vars.vars_step.Add("无序抓取计算",p);
			}
			{
				S_Collision p = new S_Collision("碰撞检测模块",space_parent,space); 
				 p.key_data="main";
				 p.all_active_mode=false;
				 p.key="-3";
				 space.vars.vars_step.Add("碰撞检测模块",p);
			}
			{
				S_Space_Finished p = new S_Space_Finished("设置空间循环结束",space_parent,space); 
				 p.all_active_mode=false;
				 p.key="-4";
				 space.vars.vars_step.Add("设置空间循环结束",p);
			}

				 set_input(space,"无序抓取计算","启动运行");
				 set_input(space,"碰撞检测模块","无序抓取计算");
				 set_input(space,"设置空间循环结束","碰撞检测模块");


			{
				 var p_load=space.vars.vars_step["启动运行"];
				 Thread.Sleep(1);
				 p_load.Run();
			}

if (Space_List.spaces.ContainsKey(space_name + tag)){
    Space_List.spaces[space_name + tag] = space;
}else{
    Space_List.spaces.Add(space_name + tag, space);
}
space.vars.bClosingWindows = false;
space.vars.bAutoMode = true;
Task.Run(() => {
    space.检查步骤状态();
}); 
}



        public void load_init(C_Space space_parent,C_Space space,string space_name){
							{
				 S_Panel_Map pItem = new S_Panel_Map("k11",space_parent,space); 
				 pItem.key="k9";
				 space.vars.vars_ui.Add("k9",pItem);
			}
			{
				 S_Panel_Map pItem = new S_Panel_Map("k8",space_parent,space); 
				 pItem.key="G1";
				 space.vars.vars_ui.Add("G1",pItem);
			}
			{
				 S_Form pItem = new S_Form("动态抓取",space_parent,space); 
pItem.pFrm=this;
				 pItem.key="G2";
				 space.vars.vars_ui.Add("G2",pItem);
			}
			{
				 S_Panel pItem = new S_Panel("1:顶部面板",space_parent,space); 
				 pItem.set_area(100,0,1100,100);
				 pItem.key="k8";
				 pItem.dock="top";
				 pItem.padding="10";
				 pItem.group="G2";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("k8",pItem);
			}
			{
				 S_Panel pItem = new S_Panel("2:左边面板",space_parent,space); 
				 pItem.set_area(0,100,300,550);
				 pItem.key="k11";
				 pItem.dock="left";
				 pItem.group="G2";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("k11",pItem);
			}
			{
				S_PictureBox pItem = new S_PictureBox("3:图片",space_parent,space); 
				 pItem.set_area(300,100,800,550);
				 pItem.url="";
				 pItem.key="t2224";
				 pItem.dock="fill";
				 pItem.picture="http://www.sndvision.cn/upload/url.php?id=24571";
				 pItem.group="G2";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("t2224",pItem);
			}
			{
				S_Button pItem = new S_Button("2:启动",space_parent,space); 
				 pItem.set_area(1500,0,100,50);
				 pItem.text="2:启动";
				 pItem.key="k12";
				 pItem.color_font="#0000ff";
				 pItem.font_size="22";
				 pItem.dock="right";
				 pItem.group="G1";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("k12",pItem);
			}
			{
				S_Button pItem = new S_Button("3:停止",space_parent,space); 
				 pItem.set_area(1400,0,100,50);
				 pItem.text="3:停止";
				 pItem.key="k6";
				 pItem.color_font="#0000ff";
				 pItem.font_size="22";
				 pItem.dock="right";
				 pItem.group="G1";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("k6",pItem);
			}
			{
				S_Button pItem = new S_Button("4:设置",space_parent,space); 
				 pItem.set_area(1300,0,100,50);
				 pItem.text="4:设置";
				 pItem.key="k14";
				 pItem.color_font="#0000ff";
				 pItem.font_size="22";
				 pItem.dock="right";
				 pItem.group="G1";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("k14",pItem);
			}
			{
				 S_TextBox pItem = new S_TextBox("5:@#exposure",space_parent,space); 
				 pItem.set_area(1100,0,200,50);
				 pItem.text="@#exposure";
				 pItem.key="k3";
				 pItem.color_bg="#333333";
				 pItem.color_font="#ffffff";
				 pItem.font_size="42";
				 pItem.dock="right";
				 pItem.group="G1";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("k3",pItem);
			}
			{
				 S_Label pItem = new S_Label("6:曝光度",space_parent,space); 
				 pItem.set_area(900,0,200,50);
				pItem.text="6:曝光度";
				 pItem.key="k13";
				 pItem.color_bg="#ddddff";
				 pItem.font_size="42";
				 pItem.dock="right";
				 pItem.group="G1";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("k13",pItem);
			}
			{
				 S_TextBox pItem = new S_TextBox("7:",space_parent,space); 
				 pItem.set_area(800,0,100,50);
				 pItem.text="";
				 pItem.key="k10";
				 pItem.color_bg="#333333";
				 pItem.color_font="#ffffff";
				 pItem.font_size="42";
				 pItem.dock="right";
				 pItem.group="G1";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("k10",pItem);
			}
			{
				 S_Label pItem = new S_Label("8:次数",space_parent,space); 
				 pItem.set_area(600,0,200,50);
				pItem.text="8:次数";
				 pItem.key="k4";
				 pItem.color_bg="#ddddff";
				 pItem.font_size="42";
				 pItem.dock="right";
				 pItem.group="G1";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("k4",pItem);
			}
			{
				 S_TextBox pItem = new S_TextBox("1:显示结果",space_parent,space); 
				 pItem.set_area(0,50,300,200);
				 pItem.text="显示结果";
				 pItem.key="t2";
				 pItem.color_bg="#333333";
				 pItem.color_font="#ffffff";
				 pItem.dock="top";
				 pItem.group="k9";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("t2",pItem);
			}
			{
				 S_LogBox pItem = new S_LogBox("2:日志",space_parent,space); 
				 pItem.x=0;
				 pItem.y=300;
				 pItem.width=300;
				 pItem.height=350;
				 pItem.text="2:日志";
				 pItem.key="k5";
				 pItem.color_bg="#333333";
				 pItem.color_font="#ffffff";
				 pItem.dock="fill";
				 pItem.group="k9";
				 pItem.pFrm=this;
				 space.vars.vars_ui.Add("k5",pItem);
			}
			
			 Action act = delegate (){
				 this.Width =1200+20;
				 this.Height =700+50;
				 this.Text ="动态抓取";
			};
                if (this != null)
                    this.BeginInvoke(act, null);
			{
				S_Cloud_Get p = new S_Cloud_Get("读取3D点云模块",space_parent,space); 
				 p.step_x="6";
				 p.step_y="6";
				 p.key_cloud="#cloud1";
				 p.key_bottom="#pPlanet_Bottom";
				 p.filter_rect="1";
				 p.filter_bottom="1";
				 p.key_cloud_save="#cloud1";
				 p.key="7276";

				 p.key_cloud="7276_7277_out_key_cloud_in_key_cloud";
				 space.vars.vars_step.Add("读取3D点云模块",p);
			}
			{
				S_Cloud_Split_OpenCL p = new S_Cloud_Split_OpenCL("点云切割模块",space_parent,space); 
				 p.key_rect="#sys.pRect1/1";
				 p.no_cover="1";
				 p.key_jarray="#json_boxs";
				 p.key_bottom="#pPlanet_Bottom";
				 p.key_camera="#camera1_const";
				 p.key_main="mains";
				 p.key_save="cloud_split";
				 p.key_cloud="#cloud1";
				 p.key="7277";

				 p.key_cloud="7276_7277_out_key_cloud_in_key_cloud";
				 space.vars.vars_step.Add("点云切割模块",p);
			}
			{
				S_Button_Event p = new S_Button_Event("点击拍照按钮",space_parent,space); 
				 p.key_ui="k12";
				 p.key="7293";
				 space.vars.vars_step.Add("点击拍照按钮",p);
			}
			{
				S_Form_Unload p = new S_Form_Unload("卸载窗口",space_parent,space); 
				 p.all_active_mode=false;
				 p.key="9604";
				 space.vars.vars_step.Add("卸载窗口",p);
			}
			{
				S_Var_Set p = new S_Var_Set("全局暂停变量=1",space_parent,space); 
				 p.key_save="#stop";
				 p.value="1";
				 p.key="-2";
				 space.vars.vars_step.Add("全局暂停变量=1",p);
			}
			{
				S_Button_Event p = new S_Button_Event("点击暂停",space_parent,space); 
				 p.key_ui="k6";
				 p.key="-1";
				 space.vars.vars_step.Add("点击暂停",p);
			}
			{
				S_Camera_Init p = new S_Camera_Init("相机初始化",space_parent,space); 
				 p.key_step="照相机模块";
				 p.key="-3";
				 space.vars.vars_step.Add("相机初始化",p);
			}
			{
				S_Var_Set p = new S_Var_Set("设置全局暂停变量=0",space_parent,space); 
				 p.key_save="#stop";
				 p.value="0";
				 p.key="-62";
				 space.vars.vars_step.Add("设置全局暂停变量=0",p);
			}
			{
				S_Draw_Rect p = new S_Draw_Rect("绘制矩形框模块1",space_parent,space); 
				 p.color="red";
				 p.pic_read="2d_pic";
				 p.pic_save="2d_pic_rect";
				 p.key_read1="#rect1/1";
				 p.key_read2="#rect1/2";
				 p.pen_width="3";
				 p.key="-10";
				 space.vars.vars_step.Add("绘制矩形框模块1",p);
			}
			{
				S_Draw_Rect p = new S_Draw_Rect("绘制矩形框模块",space_parent,space); 
				 p.color="yellow";
				 p.pic_read="2d_pic_rect";
				 p.pic_save="2d_pic_rect2";
				 p.key_read1="#rect2/1";
				 p.key_read2="#rect2/2";
				 p.pen_width="3";
				 p.key="-64";
				 space.vars.vars_step.Add("绘制矩形框模块",p);
			}
			{
				S_Draw_Outline p = new S_Draw_Outline("绘制json_boxs",space_parent,space); 
				 p.draw_scale="0.5";
				 p.key_boxs="#json_boxs";
				 p.key_read="2d_pic_rect2";
				 p.key_save="2d_pic_show";
				 p.key="-11";
				 space.vars.vars_step.Add("绘制json_boxs",p);
			}
			{
				S_PictureBox_Show p = new S_PictureBox_Show("图片显示",space_parent,space); 
				 p.scroll_x="0";
				 p.scroll_y="0";
				 p.url="@2d_pic_show";
				 p.key_ui="t2224";
				 p.all_active_mode=false;
				 p.key="-14";
				 space.vars.vars_step.Add("图片显示",p);
			}
			{
				S_If p = new S_If("判断是否暂停",space_parent,space); 
				 p.left="@#stop";
				 p.right="0";
				 p.str_operator="=";
				 p.key_type="int";
				 p.key="-22";
				 space.vars.vars_step.Add("判断是否暂停",p);
			}
			{
				S_Form_Load p= new S_Form_Load ("启动运行",space_parent,space);
				 space.vars.vars_step.Add("启动运行",p);
			}
			{
				S_Console_Show p = new S_Console_Show("日志显示模块",space_parent,space); 
				 p.key_ui="k5";
				 p.path_log="D:/data/logs";
				 p.all_active_mode=false;
				 p.key="-78";
				 space.vars.vars_step.Add("日志显示模块",p);
			}
			{
				S_MP3 p = new S_MP3("提示音:启动机械臂",space_parent,space); 
				 p.key_read="启动机械臂";
				 p.all_active_mode=false;
				 p.key="-80";
				 space.vars.vars_step.Add("提示音:启动机械臂",p);
			}
			{
				S_RGB_Get p = new S_RGB_Get("读取2D图片模块",space_parent,space); 
				 p.key_rgb="2d_pic";
				 p.key_save="2d_pic";
				 p.all_active_mode=false;
				 p.key="-58";

				 p.key_rgb="-58_-83_out_key_rgb_in_key_rgb";
				 space.vars.vars_step.Add("读取2D图片模块",p);
			}
			{
				S_RGB_Split p = new S_RGB_Split("图片切割模块",space_parent,space); 
				 p.draw_scale="0.5";
				 p.pic_read="2d_pic";
				 p.box_save="#json_boxs";
				 p.server_url1="http://127.0.0.1:9000/predict";
				 p.key="-83";

				 p.key_rgb="-58_-83_out_key_rgb_in_key_rgb";
				 space.vars.vars_step.Add("图片切割模块",p);
			}
			{
				S_Button_Event p = new S_Button_Event("点击设置曝光",space_parent,space); 
				 p.key_ui="k14";
				 p.key="2";
				 space.vars.vars_step.Add("点击设置曝光",p);
			}
			{
				S_Camera_Set p = new S_Camera_Set("设置曝光度",space_parent,space); 
				 p.exposure_time="@#exposure";
				 p.key_step="照相机模块";
				 p.key="22";
				 space.vars.vars_step.Add("设置曝光度",p);
			}
			{
				S_Var_Set p = new S_Var_Set("变量设置",space_parent,space); 
				 p.key_save="#exposure";
				 p.value="0.7";
				 p.all_active_mode=false;
				 p.key="-56";
				 space.vars.vars_step.Add("变量设置",p);
			}
			{
				S_TCP_Server p = new S_TCP_Server("TCP服务器arm",space_parent,space); 
				 p.read_start="0";
				 p.ip="192.168.255.100";
				 p.port="15000";
				 p.all_active_mode=false;
				 p.key="-57";
				 space.vars.vars_step.Add("TCP服务器arm",p);
			}
			{
				S_TCP_Server_Send p = new S_TCP_Server_Send("TCP发送位置信息",space_parent,space); 
				 p.msg="@msg2";
				 p.key_control="TCP服务器arm";
				 p.key="-79";
				 space.vars.vars_step.Add("TCP发送位置信息",p);
			}
			{
				S_TCP_CallBack p = new S_TCP_CallBack("TCP收到get_ready消息",space_parent,space); 
				 p.key_control="TCP服务器arm";
				 p.msg="get_ready";
				 p.all_active_mode=false;
				 p.key="-81";
				 space.vars.vars_step.Add("TCP收到get_ready消息",p);
			}
			{
				S_TCP_CallBack p = new S_TCP_CallBack("TCP收到take_picture",space_parent,space); 
				 p.key_control="TCP服务器arm";
				 p.msg="take_picture";
				 p.all_active_mode=false;
				 p.key="-85";
				 space.vars.vars_step.Add("TCP收到take_picture",p);
			}
			{
				S_Next p = new S_Next("准备拍照2",space_parent,space); 
				 p.wait="1";
				 p.key="-67";
				 space.vars.vars_step.Add("准备拍照2",p);
			}
			{
				S_Next p = new S_Next("准备拍照1",space_parent,space); 
				 p.wait="1";
				 p.all_active_mode=false;
				 p.key="-68";
				 space.vars.vars_step.Add("准备拍照1",p);
			}
			{
				S_Call p = new S_Call("调用准备拍照1.3",space_parent,space); 
				 p.key_read="准备拍照1";
				 p.str_new="1";
				 p.wait="600";
				 p.all_active_mode=false;
				 p.key="-76";
				 space.vars.vars_step.Add("调用准备拍照1.3",p);
			}
			{
				S_TextBox_Show p = new S_TextBox_Show("显示数量",space_parent,space); 
				 p.key_ui="k10";
				 p.msg="@#count";
				 p.all_active_mode=false;
				 p.key="-88";
				 space.vars.vars_step.Add("显示数量",p);
			}
			{
				S_Var_Set p = new S_Var_Set("初始计数",space_parent,space); 
				 p.key_save="@#count";
				 p.value="0";
				 p.all_active_mode=false;
				 p.key="-90";
				 space.vars.vars_step.Add("初始计数",p);
			}
			{
				S_Math_Add p = new S_Math_Add("+1",space_parent,space); 
				 p.left="@#count";
				 p.right="1";
				 p.key_save="#count";
				 p.all_active_mode=false;
				 p.key="-91";
				 space.vars.vars_step.Add("+1",p);
			}
			{
				S_Date_Stop p = new S_Date_Stop("停止计时",space_parent,space); 
				 p.key_read="#timeout";
				 p.all_active_mode=false;
				 p.key="-92";
				 space.vars.vars_step.Add("停止计时",p);
			}
			{
				S_Date_Start p = new S_Date_Start("开始计时",space_parent,space); 
				 p.key_save="#timeout";
				 p.all_active_mode=false;
				 p.key="-89";
				 space.vars.vars_step.Add("开始计时",p);
			}
			{
				S_Calculate_Position_Angle p = new S_Calculate_Position_Angle("计算坐标和欧拉角",space_parent,space); 
				 p.length="50";
				 p.key_robot="#robot1";
				 p.key_camera="#camera1_const";
				 p.key_six="msg";
				 p.key_planet="#抓取面1";
				 p.angle_z="-65";
				 p.all_active_mode=false;
				 p.key="-48";
				 space.vars.vars_step.Add("计算坐标和欧拉角",p);
			}
			{
				S_Next_If_Contain p = new S_Next_If_Contain("判断是否包含某个变量",space_parent,space); 
				 p.var="#抓取面1";
				 p.all_active_mode=false;
				 p.key="-46";
				 space.vars.vars_step.Add("判断是否包含某个变量",p);
			}
			{
				S_Next p = new S_Next("跳转到下一步",space_parent,space); 
				 p.all_active_mode=false;
				 p.key="-47";
				 space.vars.vars_step.Add("跳转到下一步",p);
			}
			{
				S_Call p = new S_Call("调用准备拍照2.1",space_parent,space); 
				 p.key_read="准备拍照2";
				 p.str_new="1";
				 p.wait="1";
				 p.all_active_mode=false;
				 p.key="-49";
				 space.vars.vars_step.Add("调用准备拍照2.1",p);
			}
			{
				S_If p = new S_If("判断是否暂停2",space_parent,space); 
				 p.left="@#stop";
				 p.right="0";
				 p.str_operator="=";
				 p.key_type="int";
				 p.all_active_mode=false;
				 p.key="-50";
				 space.vars.vars_step.Add("判断是否暂停2",p);
			}
			{
				S_New_Train p = new S_New_Train("新建小火车模块",space_parent,space); 
				 p.key="-44";
				 space.vars.vars_step.Add("新建小火车模块",p);
			}
			{
				S_Draw_Planet p = new S_Draw_Planet("绘制抓取或者放置位置",space_parent,space); 
				 p.pic_read="2d_pic_show";
				 p.pic_save="2d_pic_show2";
				 p.key_planet="#抓取面1";
				 p.key_camera="#camera1_const";
				 p.key="-45";
				 space.vars.vars_step.Add("绘制抓取或者放置位置",p);
			}
			{
				S_PictureBox_Show p = new S_PictureBox_Show("图片显示2",space_parent,space); 
				 p.url="@2d_pic_show2";
				 p.key_ui="t2224";
				 p.all_active_mode=false;
				 p.key="-52";
				 space.vars.vars_step.Add("图片显示2",p);
			}
			{
				S_Calculate3 p = new S_Calculate3("动态抓取位置计算",space_parent,space); 
				 p.key_array_main="mains";
				 p.key_space="calculate";
				 p.key_save="#抓取面1";
				 p.key_cloud="#cloud1";
				 p.fa_x_min="-0.4";
				 p.fa_x_max="0.4";
				 p.fa_y_min="-0.4";
				 p.fa_y_max="0.4";
				 p.all_active_mode=false;
				 p.key="-53";
				 space.vars.vars_step.Add("动态抓取位置计算",p);
			}
			{
				S_If p = new S_If("判断模块",space_parent,space); 
				 p.left="@mains/count";
				 p.right="0";
				 p.str_operator="=";
				 p.key_type="string";
				 p.all_active_mode=false;
				 p.key="-54";
				 space.vars.vars_step.Add("判断模块",p);
			}
			{
				S_RGB_Save p = new S_RGB_Save("图片保存",space_parent,space); 
				 p.key_read="2d_pic";
				 p.file="D:/save/@.png";
				 p.all_active_mode=false;
				 p.key="-55";
				 space.vars.vars_step.Add("图片保存",p);
			}
			{
				S_Calculate_X_By_Time p = new S_Calculate_X_By_Time("计算x的偏移位置",space_parent,space); 
				 p.x_min="-360";
				 p.key_read="msg";
				 p.key_save="msg2";
				 p.key_time="time_camera";
				 p.speed="490";
				 p.key="-59";
				 space.vars.vars_step.Add("计算x的偏移位置",p);
			}
			{
				S_Var_Set p = new S_Var_Set("变量设置Path.Mp3",space_parent,space); 
				 p.key_save="#path.mp3";
				 p.value="D:/data/mp3";
				 p.all_active_mode=false;
				 p.key="-51";
				 space.vars.vars_step.Add("变量设置Path.Mp3",p);
			}
			{
				S_Camera p = new S_Camera("照相机模块",space_parent,space); 
				 p.key_3d="#camera1_3dArray";
				 p.control_2d="读取2D图片模块";
				 p.control_3d="读取3D点云模块";
				 p.key_camera="#camera1_const";
				 p.wait="1000";
				 p.key_camera_time="time_camera";
				 p.key_compare="#compare";
				 p.all_active_mode=false;
				 p.key="-63";
				 space.vars.vars_step.Add("照相机模块",p);
			}

				 set_input(space,"点云切割模块","图片切割模块");
				 set_input(space,"点云切割模块","读取3D点云模块");
				 set_input(space,"全局暂停变量=1","点击暂停");
				 set_input(space,"相机初始化","日志显示模块");
				 set_input(space,"设置全局暂停变量=0","点击拍照按钮");
				 set_input(space,"照相机模块","新建小火车模块");
				 set_input(space,"绘制矩形框模块1","图片切割模块");
				 set_input(space,"绘制矩形框模块","绘制矩形框模块1");
				 set_input(space,"绘制json_boxs","绘制矩形框模块");
				 set_input(space,"图片显示","绘制json_boxs");
				 set_input(space,"判断是否暂停","计算坐标和欧拉角");
				 set_input(space,"日志显示模块","启动运行");
				 set_input(space,"提示音:启动机械臂","变量设置Path.Mp3");
				 set_input(space,"图片切割模块","读取2D图片模块");
				 set_input(space,"设置曝光度","点击设置曝光");
				 set_input(space,"变量设置","提示音:启动机械臂");
				 set_input(space,"TCP服务器arm","启动运行");
				 set_input(space,"TCP发送位置信息","计算x的偏移位置");
				 set_input(space,"TCP收到take_picture","点击拍照按钮");
				 set_input(space,"准备拍照2","准备拍照1");
				 set_input(space,"准备拍照2","TCP收到take_picture");
				 set_input(space,"准备拍照1","设置全局暂停变量=0");
				 set_input(space,"调用准备拍照1.3","TCP发送位置信息");
				 set_input(space,"显示数量","+1");
				 set_input(space,"初始计数","日志显示模块");
				 set_input(space,"+1","跳转到下一步");
				 set_input(space,"停止计时","调用准备拍照1.3");
				 set_input(space,"开始计时","TCP收到get_ready消息");
				 set_input(space,"计算坐标和欧拉角","跳转到下一步");
				 set_input(space,"判断是否包含某个变量","动态抓取位置计算");
				 set_input(space,"跳转到下一步","判断是否包含某个变量");
				 set_input(space,"调用准备拍照2.1","判断是否暂停2");
				 set_input(space,"调用准备拍照2.1","计算x的偏移位置",false);
				 set_input(space,"判断是否暂停2","判断是否包含某个变量",false);
				 set_input(space,"判断是否暂停2","动态抓取位置计算",false);
				 set_input(space,"判断是否暂停2","判断模块");
				 set_input(space,"新建小火车模块","准备拍照2");
				 set_input(space,"绘制抓取或者放置位置","图片显示");
				 set_input(space,"绘制抓取或者放置位置","判断是否包含某个变量");
				 set_input(space,"图片显示2","绘制抓取或者放置位置");
				 set_input(space,"动态抓取位置计算","判断模块",false);
				 set_input(space,"判断模块","点云切割模块");
				 set_input(space,"图片保存","读取2D图片模块");
				 set_input(space,"计算x的偏移位置","判断是否暂停");
				 set_input(space,"计算x的偏移位置","开始计时");
				 set_input(space,"变量设置Path.Mp3","日志显示模块");

			{
				 S_Button_Event p = (S_Button_Event)space.vars.vars_step["点击拍照按钮"];
				 p.init();
			}
			{
				 S_Button_Event p = (S_Button_Event)space.vars.vars_step["点击暂停"];
				 p.init();
			}
			{
				 S_PictureBox_Show p = (S_PictureBox_Show)space.vars.vars_step["图片显示"];
				 p.init();
			}
			{
				 S_Console_Show p = (S_Console_Show)space.vars.vars_step["日志显示模块"];
				 p.init();
			}
			{
				 S_Button_Event p = (S_Button_Event)space.vars.vars_step["点击设置曝光"];
				 p.init();
			}
			{
				 S_TCP_CallBack p = (S_TCP_CallBack)space.vars.vars_step["TCP收到get_ready消息"];
				 p.init();
			}
			{
				 S_TCP_CallBack p = (S_TCP_CallBack)space.vars.vars_step["TCP收到take_picture"];
				 p.init();
			}
			{
				 S_PictureBox_Show p = (S_PictureBox_Show)space.vars.vars_step["图片显示2"];
				 p.init();
			}
			{
				 S_Camera p = (S_Camera)space.vars.vars_step["照相机模块"];
				 p.init();
			}

			{
				 var p_load=space.vars.vars_step["启动运行"];
				 Thread.Sleep(500);
				 p_load.Run();
			}

			{
				S_PictureBox_Show pState1 = (S_PictureBox_Show)space.vars.vars_step["图片显示"];
				pState1.picture_box = pBoxView;

				S_PictureBox_Show pState2 = (S_PictureBox_Show)space.vars.vars_step["图片显示2"];
				pState2.picture_box = pBoxView;

				MessageBox.Show("loading finished");
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
            //====================gaojundz@qq.com==================
            pBoxSet.Parent = pBoxBack;
            pBoxStart.Parent = pBoxBack;
            pBoxStop.Parent = pBoxBack;
            asc.controllInitializeSize(this);

            RectpBoxBack = new Rectangle(0, 0, 1146, 613);
            RectpBoxSet = new Rectangle(786, 5, 100, 50);
            RectpBoxStart = new Rectangle(905, 5, 100, 50);
            RectpBoxStop = new Rectangle(1024, 5, 100, 50);
            //=====================================================
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
            
			//0b8a479f86c482f6545b7af602085d41
            space.vars.save_vars(null, "#serial", "string", serial);
            space2.vars.save_vars(null, "#serial", "string", serial);


			Task.Run(() =>
			{
				space2.vars.copy_from(space);
				try
				{
					env(space, space2, "env","");
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
					Console.WriteLine(ex.ToString());
				}
				
				int iCount = 0;
                while (space2.bExit_Loop==false)
                {
					iCount++;
					if (iCount >50)
                    {
						break;
					}
					Thread.Sleep(100);
				}
				space2.vars.bClosingWindows = true;
				space.vars.copy_from(space2);


				try
				{
					space3.vars.save_vars(null, "#serial", "string",serial);
					space3.vars.copy_from(space);
					load_init(space, space3, "main");
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
					Console.WriteLine(ex.ToString());
				}

				
				Action act = delegate ()
				{
					//this.Controls.Remove(pLabel);
					this.CenterToScreen();
				};
				if (this != null)
					this.BeginInvoke(act, null);

               });


            Frm_Debug pDebug = new Frm_Debug();
pDebug.space = space3; 
pDebug.Show(); 

        }

        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            space.vars.bClosingWindows = true;
            space2.vars.bClosingWindows = true;
            space3.vars.bClosingWindows = true;
			try{
    S_TCP_Server a = (S_TCP_Server)space2.vars.vars_step["TCP服务器arm"];
    a.Listener.Stop();
}catch(Exception ex){
}

		}

        //=====================gaojundz@qq.com==========================
        #region 自适应类
        AutoSizeFormClass asc = new AutoSizeFormClass();
        class AutoSizeFormClass
        {
            //(1).声明结构,只记录窗体和其控件的初始位置和大小。  
            public struct controlRect
            {
                public int Left;
                public int Top;
                public int Width;
                public int Height;
            }
            //(2).声明 1个对象  
            //注意这里不能使用控件列表记录 List<Control> nCtrl;，因为控件的关联性，记录的始终是当前的大小。  
            public List<controlRect> oldCtrl;
            int ctrl_first = 0;
            //(3). 创建两个函数  
            //(3.1)记录窗体和其控件的初始位置和大小,  
            public void controllInitializeSize(Form mForm)
            {
                if (ctrl_first == 0)
                {
                    ctrl_first = 1;
                    oldCtrl = new List<controlRect>();
                    controlRect cR;
                    cR.Left = mForm.Left; cR.Top = mForm.Top; cR.Width = mForm.Width; cR.Height = mForm.Height;
                    oldCtrl.Add(cR);
                    foreach (Control c in mForm.Controls)
                    {
                        controlRect objCtrl;
                        objCtrl.Left = c.Left; objCtrl.Top = c.Top; objCtrl.Width = c.Width; objCtrl.Height = c.Height;
                        oldCtrl.Add(objCtrl);
                    }
                }
                //mForm.WindowState = (System.Windows.Forms.FormWindowState)(2);//记录完控件的初始位置和大小后，再最大化  
                //0 - Normalize , 1 - Minimize,2- Maximize  
            }
            //(3.2)控件自适应大小,  
            public void controlAutoSize(Form mForm, int WinY)
            {
                //int wLeft0 = oldCtrl[0].Left; ;//窗体最初的位置  
                //int wTop0 = oldCtrl[0].Top;  
                ////int wLeft1 = this.Left;//窗体当前的位置  
                //int wTop1 = this.Top;  
                float wScale = (float)mForm.Width / (float)oldCtrl[0].Width;//新旧窗体之间的比例，与最早的旧窗体  
                float hScale = (float)(mForm.Height + WinY) / (float)oldCtrl[0].Height;//.Height;  
                int ctrLeft0, ctrTop0, ctrWidth0, ctrHeight0;
                int ctrlNo = 1;//第1个是窗体自身的 Left,Top,Width,Height，所以窗体控件从ctrlNo=1开始  
                foreach (Control c in mForm.Controls)
                {
                    ctrLeft0 = oldCtrl[ctrlNo].Left;
                    ctrTop0 = oldCtrl[ctrlNo].Top;
                    ctrWidth0 = oldCtrl[ctrlNo].Width;
                    ctrHeight0 = oldCtrl[ctrlNo].Height;
                    //c.Left = (int)((ctrLeft0 - wLeft0) * wScale) + wLeft1;//新旧控件之间的线性比例  
                    //c.Top = (int)((ctrTop0 - wTop0) * h) + wTop1;  
                    c.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1  
                    c.Top = (int)((ctrTop0) * hScale);//  
                    c.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);  
                    c.Height = (int)(ctrHeight0 * hScale);//  
                    ctrlNo += 1;
                }
            }

        }
        #endregion

        Rectangle RectpBoxBack;
        Rectangle RectpBoxSet;
        Rectangle RectpBoxStart;
        Rectangle RectpBoxStop;
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                asc.controlAutoSize(this, 0);
                pBoxStop.Width = RectpBoxStop.Width * pBoxBack.Width / RectpBoxBack.Width; pBoxStop.Height = RectpBoxStop.Height * pBoxBack.Height / RectpBoxBack.Height;
                pBoxStart.Width = RectpBoxStart.Width * pBoxBack.Width / RectpBoxBack.Width; pBoxStart.Height = RectpBoxStart.Height * pBoxBack.Height / RectpBoxBack.Height;
                pBoxSet.Width = RectpBoxSet.Width * pBoxBack.Width / RectpBoxBack.Width; pBoxSet.Height = RectpBoxSet.Height * pBoxBack.Height / RectpBoxBack.Height;
            }
            catch// (Exception ex)
            {
                //MessageBox.Show("asc.controlAutoSize异常：" + ex.ToString());
            }

            pBoxStop.Location = new Point(pBoxBack.Width - pBoxStop.Width, RectpBoxStop.Y);
            pBoxStart.Location = new Point(pBoxBack.Width - pBoxStart.Width - pBoxStop.Width, RectpBoxStart.Y);
            pBoxSet.Location = new Point(pBoxBack.Width - pBoxStart.Width - pBoxStop.Width - pBoxSet.Width, RectpBoxSet.Y);
        }

        private void pBoxStart_Click(object sender, EventArgs e)
        {
			C_State pState = (C_State)space3.vars.vars_step["点击拍照按钮"];
			pState.Run();
		}

        private void pBoxStop_Click(object sender, EventArgs e)
        {

			C_State pState = (C_State)space3.vars.vars_step["点击暂停"];
			pState.Run();
		}
        //==============================================================
    }
}
