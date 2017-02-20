using HAOCommon;
using HAOCommon.PI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WinHost;
using YHYXYH.Tool;
using YHYXYH.YXYH;

namespace KSPrj
{
    public partial class FCZFormSix : Form
    { 
        DateTime timeGCCollect = DateTime.Now;//系统垃圾回收时间 

        //当前机组的所以测点 
        public static DataTable dtCurrentUnitTag;
        double douBar1Value = 50;
        private bool isminReturn = false; 
        private Control control;
        private string rightBottId = "";  
        System.Timers.Timer timerSound = new System.Timers.Timer(100);//报警的定时器
        SystemGraph systemGraph = new SystemGraph();
        bool isFirstRun = true;
        double panel10Scalex = 1; 

        public FCZFormSix()
        {
            InitializeComponent(); 
            AddConfigedControl();
            AddPictureControls();  

            init();

            if (GlobalVariables.IsCanAddLabel)
            { 
                this.btnAdd2cTag.Visible = true;
                this.btnAdd2tTag.Visible = true; 
            }
            else
            { 
                this.btnAdd2cTag.Visible = false;
                this.btnAdd2tTag.Visible = false; 
            }

            SetBindingLabelsText();
            
            InitCharts(); //初始化Chart2 Chart3 BY ZZH  
            Chart1Bind(); //Chart1数据绑定（仅用在初始化的时候） BY ZZH  
            Chart3Bind(); //Chart3数据绑定 BY ZZH 

            timerSound.Elapsed += new ElapsedEventHandler(timerSound_Elapsed);

            button7_Click(null, null);

            //LabelZYGYC = this.panelChild.Controls.Find("LabelZYGYC", false)[0] as Label;

            this.timer2.Interval = GlobalVariables.RefIntvel;
            this.timer2.Enabled = false;

            //LabelBindDoubleClick(this.label36, 2323, "低压缸单侧排汽容积流量");
            //LabelBindDoubleClick(this.label38, 2320, "低压缸排汽流速");
            //LabelBindDoubleClick(this.label40, 2329, "相对容积流量比");
            LabelBindDoubleClick(this.label27, 2324, "当前低压缸排汽流量");
            //LabelBindDoubleClick(this.label3, 2111, "当前低压缸进汽压力（MPa）");
            LabelBindDoubleClick(this.label28, 3126, "当前低压缸排汽压力（MPa）");
            LabelBindDoubleClick(this.labelSub1, 3002, "排汽压力限制");

            foreach (Control ctrl1 in this.panel10.Controls)
            { 
                ctrl1.ContextMenuStrip = contextMenuStrip_Label;
                ctrl1.MouseDown += control_MouseDown; 
            } 
        }

        private void FCZForm_Load(object sender, EventArgs e)
        {
            this.timer2.Enabled = false;

            NoticeSix.OnMessageRecieved += OnMessageReceived;
            upd = new UpdateControls(ChangeLabels);
        }

        private void init()
        { 
            //初始化Chart1 BY ZZH 
            chart1.Series.Add("F2128");
            chart1.Series[0].Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            chart1.Series[0].LegendText = "凝结水量";
            chart1.Series[0].ChartType = SeriesChartType.Line; //图表类型为曲线
            chart1.Series[0].XValueType = ChartValueType.DateTime; 

            for (int ii = 0; ii < chart1.Series.Count; ii++)
            {
                chart1.Series[ii].XValueMember = "valuetime";
                chart1.Series[ii].YValueMembers = chart1.Series[ii].Name;
            } 
        }

        #region Resize事件 
        //控件的Resize事件 
        private void Control_Resize(object sender, EventArgs e)
        {
            Rectangle rect;
            LabelTag labelTag = null;
            //this.Visible = false;
            Control parentControl = sender as Control;
            parentControl.Visible = false;
            //foreach (Control control in parentControl.Controls)
            //{
            for (int i = 0; i < parentControl.Controls.Count; i++) 
            {
                Control control = parentControl.Controls[i];
                if (control.Tag == null)
                    continue;
                try
                {
                    labelTag = (LabelTag)(control.Tag);
                    rect = new Rectangle();
                    rect.X = Convert.ToInt32(1.0 * labelTag.ControlX / labelTag.ParentWidth * parentControl.Width);
                    rect.Y = Convert.ToInt32(1.0 * labelTag.ControlY / labelTag.ParentHeight * parentControl.Height);
                    if (control.GetType() == typeof(PictureBox))
                    {
                        if (parentControl.Name == "panel10") 
                        { 
                            panel10Scalex = Convert.ToDouble(1.0 * parentControl.Width / labelTag.ParentWidth); 
                            rect.Width = Convert.ToInt32(1.0 * labelTag.Width / labelTag.ParentWidth * parentControl.Width);
                            rect.Height = Convert.ToInt32(1.0 * labelTag.Height / labelTag.ParentHeight * parentControl.Height);
                        }
                        else if (parentControl.Name == "groupBox2") 
                        {
                            // 注意这里，如果需要缩放图片的大小，可以将空冷岛图片保存到controlconfig_pic里面，然后把这段删掉 ZZH
                            rect.Width = 12;
                            rect.Height = 18;
                        }
                    }
                    control.Bounds = rect; 
                }
                catch { }
            }
            parentControl.Visible = true;
        }

        #endregion

        #region 委托接收并更新页面数据 
        //定义委托 BY ZZH 
        public delegate void UpdateControls(DataTable dt);
        UpdateControls upd = null;

        public void OnMessageReceived(DataTable calDataTable)
        {
            this.BeginInvoke(upd, calDataTable);
        }

        private void ChangeLabels(DataTable dt)
        { 
            #region 绑定chart数据 
            try
            {
                dataBindChart(); //绑定chart 数据
                Application.DoEvents(); 
                Chart3Bind();
                Chart2Bind();
                Application.DoEvents(); 
                PicChartBind();
                //KLDStateCheck();
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs("绑定chart 数据报错" + ex.ToString());
            } 
            #endregion

            #region 给每个label赋值 
            try
            {
                SetBindingLabelsText();
                Application.DoEvents();  
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs("Label赋值报错" + ex.ToString());
            }
            #endregion

            #region 报警检查 
            //try
            //{
            //    double fuhe = 0;
            //    fuhe = CalTree.GetFinishedTagValue("F2145"); 
            //    //负荷小于50  就不做报警判断了
            //    if (fuhe != GlobalVariables.BadValue && fuhe > 100)
            //    { 
            //        ZYGCYCheck(); 
            //    }
            //    else
            //    {
            //        //如果不需要报警，就隐藏弹出框，取消文本闪烁  
            //        if (ZYGYCboolAlarm)
            //        {
            //            ZYGYCboolAlarm = false;
            //            if (ZYGCYAlarm.Visible == true)
            //                ZYGCYAlarm.Visible = false;
            //        }  
            //    }
            //}
            //catch (Exception ex)
            //{ WriteLog.WriteLogs(ex.ToString()); }
            #endregion

            #region 检查实时测点是否有不正确的
            try
            {
                //原始数据若有不合法，开始报警
                if (!CheckTags(GlobalVariables.dtOneRowDataSix))
                    btnTagBool = true;
                else
                {
                    btnTagBool = false;
                    this.btnTags.ForeColor = Color.White;
                }
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
            #endregion

            isFirstRun = false;

            if ((DateTime.Now - timeGCCollect).TotalHours > 2.3)
            {
                timeGCCollect = DateTime.Now;
                GC.Collect();
            } 
        }

        #endregion 

        #region 棒状图绑定 
        /// <summary>
        /// 棒状图绑定 
        /// </summary>
        private void PicChartBind()
        {
            try 
	        {	   
                double tdouBar1Value = TagValue.GetFinishedTagValueSix("F4002"); 
                if (tdouBar1Value != GlobalVariables.ReplaceValue)
                {
                    douBar1Value = tdouBar1Value;
                }
                douBar1Value = System.Math.Round(douBar1Value, 2);

                ////下面注释是棒状图移动数值 
                //int pointX = 0;
                //int pointY = Convert.ToInt32(70 * scaley); 
                labelSub1.Text = "当前值：" + douBar1Value;
                //if (douBar1Value < 10)
                //{
                //    this.labelSub1.Location = new Point(Convert.ToInt32(115 * scalex), pointY);  
                //}
                //else
                //{
                //    if (douBar1Value <= 100)
                //        pointX = Convert.ToInt32(douBar1Value * 4 * scalex);
                //    else if (douBar1Value > 100 && douBar1Value <= 110)
                //        pointX = Convert.ToInt32(((douBar1Value - 100) * 10 * scalex) + 400 * scalex);
                //    else if (douBar1Value > 110 && douBar1Value <= 135)
                //        pointX = Convert.ToInt32(((douBar1Value - 110) * 4 * scalex) + 500 * scalex);
                //    else if (douBar1Value > 135)
                //        pointX = Convert.ToInt32((25 * 4 * scalex) + 500 * scalex);
                //    this.labelSub1.Location = new Point(pointX, pointY);
                //} 
                
                //绑定棒状图上的Label
                PicChartLabelBind();

                //第一条棒图
                if (douBar1Value <= 100)
                {
                    this.labelSub1.ForeColor = Color.Green;

                    picGreen1.Visible = true;
                    this.picGreen1.BackColor = Color.Green;
                    if (douBar1Value < 10)//如果低于10，则按照10的长度显示
                    {
                        this.picGreen1.Width = Convert.ToInt32(10 * 4 * panel10Scalex);
                    }
                    else
                    {
                        this.picGreen1.Width = Convert.ToInt32(douBar1Value * 4 * panel10Scalex);
                    }
                    this.picYellow1.Visible = false;
                    //this.picOrange1.Visible = false;
                    this.picRed1.Visible = false;
                }
                else if ((douBar1Value > 100) && (douBar1Value <= 125))
                {
                    this.labelSub1.ForeColor = Color.Yellow;

                    this.picGreen1.Visible = true;
                    this.picGreen1.BackColor = Color.Yellow;
                    this.picGreen1.Width = (int)(400 * panel10Scalex); // Convert.ToInt32(douData * 10) - 1000;

                    this.picYellow1.Visible = true;
                    this.picYellow1.BackColor = Color.Yellow;
                    this.picYellow1.Width = (int)((douBar1Value - 100) * 4 * panel10Scalex);

                    //this.picOrange1.Visible = false;
                    this.picRed1.Visible = false;
                }
                //else if ((douBar1Value > 112.5) && (douBar1Value <= 125))
                //{
                //    this.labelSub1.ForeColor = Color.Orange;

                //    this.picGreen1.Visible = true;
                //    this.picGreen1.BackColor = Color.Orange;
                //    this.picGreen1.Width = Convert.ToInt32(400 * panel10Scalex); 

                //    this.picYellow1.Visible = true;
                //    this.picYellow1.BackColor = Color.Orange;
                //    this.picYellow1.Width = Convert.ToInt32(50 * panel10Scalex);

                //    this.picOrange1.Visible = true;
                //    this.picOrange1.BackColor = Color.Orange;
                //    this.picOrange1.Width = (int)((douBar1Value - 112.5) * 4 * panel10Scalex);

                //    this.picRed1.Visible = false; 
                //}
                else if (douBar1Value > 125)
                {
                    this.labelSub1.ForeColor = Color.Red;

                    this.picGreen1.Visible = true;
                    this.picGreen1.BackColor = Color.Red;
                    this.picGreen1.Width = Convert.ToInt32(400 * panel10Scalex);

                    this.picYellow1.Visible = true;
                    this.picYellow1.BackColor = Color.Red;
                    this.picYellow1.Width = Convert.ToInt32(100 * panel10Scalex);

                    //this.picOrange1.Visible = true;
                    //this.picOrange1.BackColor = Color.Red;
                    //this.picOrange1.Width = Convert.ToInt32(50 * panel10Scalex);

                    this.picRed1.Visible = true;
                    this.picRed1.BackColor = Color.Red;
                    int len = Convert.ToInt32((douBar1Value - 125) * 4);
                    if (len > 100)
                        this.picRed1.Width = Convert.ToInt32(100 * panel10Scalex);
                    else
                        this.picRed1.Width = Convert.ToInt32(len * panel10Scalex);
                }
                Application.DoEvents();

                //棒状图报警检查 
                PicChartChecking(douBar1Value); 
            }
	        catch (Exception)
	        { }
        }

        #endregion 

        #region 控件加载与绑定

        string sNewLabel = "";
        DataTable dtControls = TableControlConfig.getDataTableByRowFilter("[Parent] like 'FCZFormSix.%'");
        TagLabelBinding tagLabelBinding = new TagLabelBinding(6);
        DataTable picControls = SQLHelper.ExecuteDt("select * from ControlConfig_PIC");
        /// <summary>
        /// 加载程序运行中配置的控件
        /// </summary>
        void AddConfigedControl()
        {
            try
            {
                //int i;
                Control parent = null;
                Control control = null;
                Rectangle rect;
                LabelTag labelTag = null;
                foreach (DataRow row in dtControls.Rows)
                {

                    parent = TagLabelBinding.GetParentControl(this, row["Parent"].ToString());
                    try
                    {
                        if (row["type"].ToString() == "PictureBox")
                        {
                            int controlX = int.Parse(row["ControlX"].ToString());
                            int controlY = int.Parse(row["ControlY"].ToString());
                            control = new PictureBox();
                            ((PictureBox)control).BackgroundImage = global::KSPrj.Properties.Resources.verticalGreen;
                            ((PictureBox)control).BackgroundImageLayout = ImageLayout.Stretch;
                            control.Name = row["ControlName"].ToString();
                            rect = new Rectangle();
                            rect.X = controlX;
                            rect.Y = controlY;
                            labelTag = new LabelTag();
                            labelTag.ParentHeight = int.Parse(row["ParentHeight"].ToString());
                            labelTag.ParentWidth = int.Parse(row["ParentWidth"].ToString());
                            labelTag.ControlX = controlX;
                            labelTag.ControlY = controlY;
                            labelTag.UnitNo = byte.Parse(row["UnitNo"].ToString());
                            control.Tag = labelTag;

                            control.Bounds = rect;
                            control.Size = new System.Drawing.Size(12, 18);
                            control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
                            if (GlobalVariables.IsCanMoveLabel)
                            {
                                control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
                                control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
                                control.ContextMenuStrip = contextMenuStrip_Label;//添加快捷菜单
                            }
                            parent.Controls.Add(control);
                            control.Refresh();
                            continue;
                        }

                        //这里先以Label写死；如果以后还有其他类型的控件，则在这里枚举
                        control = new Label();
                        ((Label)control).AutoSize = true;
                        control.Name = row["ControlName"].ToString();
                        control.Text = control.Name;
                        if (row["ControlText"].ToString().Length > 1)
                            control.Text = row["ControlText"].ToString();
                        if (row["FontSize"].ToString().Length > 1)
                            control.Font = new System.Drawing.Font("宋体", float.Parse(row["FontSize"].ToString()), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                        rect = new Rectangle();
                        rect.X = int.Parse(row["ControlX"].ToString());
                        rect.Y = int.Parse(row["ControlY"].ToString());
                        control.Bounds = rect;
                        try { control.ForeColor = Color.FromArgb(int.Parse(row["ForeColor"].ToString())); }
                        catch { }
                        try { control.BackColor = Color.FromArgb(int.Parse(row["BackColor"].ToString())); }
                        catch { }
                        control.BackColor = Color.Transparent;
                        labelTag = new LabelTag();
                        labelTag.ControlX = rect.X;
                        labelTag.ControlY = rect.Y;
                        labelTag.ParentHeight = int.Parse(row["ParentHeight"].ToString());
                        labelTag.ParentWidth = int.Parse(row["ParentWidth"].ToString());
                        labelTag.UnitNo = byte.Parse(row["UnitNo"].ToString());
                        control.Tag = labelTag;
                        control.MouseDoubleClick += new MouseEventHandler(Label_DoubleClick);//双击事件  改动 BY ZZH 
                        control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
                        if (GlobalVariables.IsCanMoveLabel)
                        {
                            control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
                            control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
                            control.ContextMenuStrip = contextMenuStrip_Label;//添加快捷菜单
                        }
                        parent.Controls.Add(control);
                    }
                    catch { }
                }
                dtControls.Dispose();
                dtControls = null;
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }

        }

        private void AddPictureControls() 
        {
            Control parent = null;
            Control control = null;
            Rectangle rect;
            LabelTag labelTag = null;
            foreach (DataRow row in picControls.Rows)
            { 
                parent = TagLabelBinding.GetParentControl(this, row["Parent"].ToString());
                try
                {
                    if (row["type"].ToString() == typeof(PictureBox).ToString())
                    {   
                        control = parent.Controls.Find(row["ControlName"].ToString(), false)[0];
                        rect = new Rectangle();
                        rect.X = int.Parse(row["ControlX"].ToString());
                        rect.Y = int.Parse(row["ControlY"].ToString());
                        rect.Width = int.Parse(row["ControlWidth"].ToString());
                        rect.Height = int.Parse(row["ControlHeight"].ToString());
                        labelTag = new LabelTag();
                        labelTag.ParentHeight = int.Parse(row["ParentHeight"].ToString());
                        labelTag.ParentWidth = int.Parse(row["ParentWidth"].ToString());
                        labelTag.ControlX = rect.X;
                        labelTag.ControlY = rect.Y;
                        labelTag.Width = rect.Width;
                        labelTag.Height = rect.Height;
                        labelTag.UnitNo = byte.Parse(row["UnitNo"].ToString());
                        control.Tag = labelTag; 
                        control.Bounds = rect; 
                    }
                    else if (row["type"].ToString() == typeof(Label).ToString()) 
                    { 
                        //((Label)control).AutoSize = true;
                        control = parent.Controls.Find(row["ControlName"].ToString(), false)[0]; 
                        control.Text = control.Name;
                        if (row["ControlText"].ToString().Length > 1)
                            control.Text = row["ControlText"].ToString();
                        if (row["FontSize"].ToString().Length > 1)
                            control.Font = new System.Drawing.Font("宋体", float.Parse(row["FontSize"].ToString()), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                        rect = new Rectangle();
                        rect.X = int.Parse(row["ControlX"].ToString());
                        rect.Y = int.Parse(row["ControlY"].ToString());
                        control.Bounds = rect; 
                        control.BackColor = Color.Transparent;
                        labelTag = new LabelTag();
                        labelTag.ControlX = rect.X;
                        labelTag.ControlY = rect.Y;
                        labelTag.ParentHeight = int.Parse(row["ParentHeight"].ToString());
                        labelTag.ParentWidth = int.Parse(row["ParentWidth"].ToString());
                        labelTag.UnitNo = byte.Parse(row["UnitNo"].ToString());
                        control.Tag = labelTag;
                        control.MouseDoubleClick += new MouseEventHandler(Label_DoubleClick);//双击事件  改动 BY ZZH 
                    }
                    //try { control.ForeColor = Color.FromArgb(int.Parse(row["ForeColor"].ToString())); }
                    //catch { }
                    //try { control.BackColor = Color.FromArgb(int.Parse(row["BackColor"].ToString())); }
                    //catch { }
                    control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
                    if (GlobalVariables.IsCanMoveLabel)
                    {
                        control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
                        control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
                        control.ContextMenuStrip = contextMenuStrip_Label;//添加快捷菜单
                    }
                    //parent.Controls.Add(control);
                    control.Refresh(); 
                }
                catch { }
            }
            picControls = null;
        }

        /// <summary>
        /// 设定绑定标签的Text（测点值+测点单位）。
        /// </summary>
        void SetBindingLabelsText()
        {
            try
            {
                Control control, parent;
                LabelTag labelTag = null;
                DataTable dtTagLabelBinding = TableTagLabelBinding.getDataTableByRowFilter("[Parent] like 'FCZFormSix.%'");
                foreach (DataRow row in dtTagLabelBinding.Rows)
                {
                    try
                    {
                        parent = TagLabelBinding.GetParentControl(this, row["Parent"].ToString());
                        //由父控件找到本Label
                        control = parent.Controls.Find(row["LabelName"].ToString(), false)[0];
                        labelTag = (LabelTag)control.Tag;

                        try
                        {

                            if (labelTag.IsSetToolTipText == false)
                            {
                                toolTip1.SetToolTip(control, row["TagDesc"].ToString());
                                labelTag.IsSetToolTipText = true;
                                labelTag.TagID = int.Parse(row["TagID"].ToString());
                                labelTag.TagDesc = row["TagDesc"].ToString();
                                labelTag.TagUnit = row["unit"].ToString();
                                control.Tag = labelTag;
                            }
                        }
                        catch
                        {
                            control.Tag = new LabelTag(true, int.Parse(row["TagID"].ToString()), row["TagDesc"].ToString(), row["unit"].ToString());
                            toolTip1.SetToolTip(control, row["TagDesc"].ToString());
                        }

                        try
                        {
                            control.Text = TagValue.GetShowValueSix(int.Parse(row["TagID"].ToString()), labelTag.TagUnit, row["adjustValue"].ToString());
                        }
                        catch { }
                        control.Refresh();


                    }
                    catch (Exception ex)
                    {
                        //WriteLog.WriteLogs(ex.ToString());
                    }
                }
                //lblRunOptimizePromptText.Text = RunOptimizePrompt.GetPromptText();
                //lblRunOptimizePromptText.ForeColor = RunOptimizePrompt.GetPromptTextForeColor();
                dtTagLabelBinding.Dispose();
                dtTagLabelBinding = null;
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
        }
        


        bool isDrag = false;
        string sSql = "";
        Point point = new Point();
        //保存鼠标按下时的坐标。这个坐标是相对于所点击的标签的坐标；在鼠标拖动时要减去这个坐标，要不然拖动时鼠标会一直处于标签的左上角
        Point pointMouseDown = new Point();
        //Control control;
        void control_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if ((sender as Control).GetType() == typeof(PictureBox))
                {
                    control = (PictureBox)sender;
                }
                else if ((sender as Control).GetType() == typeof(Label))
                {
                    control = (Label)sender;
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                pointMouseDown.X = e.X;
                pointMouseDown.Y = e.Y;
            }

        }
        void control_MouseMove(object sender, MouseEventArgs e)
        {
            if ((sender as Control).GetType() == typeof(PictureBox))
            {
                control = (PictureBox)sender;
            }
            else if ((sender as Control).GetType() == typeof(Label))
            {
                control = (Label)sender;
            }
            if (e.Button == MouseButtons.Left)
            {
                isDrag = true;
                point.X = control.Location.X + e.X - pointMouseDown.X;
                point.Y = control.Location.Y + e.Y - pointMouseDown.Y;
                control.Location = point;
                control.Refresh();
            }
        }
        void control_MouseUp(object sender, MouseEventArgs e)
        {
            if ((sender as Control).GetType() == typeof(PictureBox))
            {
                control = (PictureBox)sender;
            }
            else if ((sender as Control).GetType() == typeof(Label))
            {
                control = (Label)sender;
            }
            string tabelName = " ControlConfig ";
            if (control.Parent.Name == "panel10")
                tabelName = " ControlConfig_PIC ";
            if (e.Button == MouseButtons.Left)
            {
                if (isDrag)
                {
                    isDrag = false;
                    sSql = "update " + tabelName + " set ControlX=" + control.Location.X + ",ControlY=" + control.Location.Y
                        + ",ParentHeight=" + control.Parent.Height + ",ParentWidth=" + control.Parent.Width
                        + " where ControlName='" + control.Name + "'";
                    SQLHelper.ExecuteSql(sSql);
                    control.Location = point;
                    control.Refresh();
                    LabelTag labelTag = null;
                    try
                    {
                        labelTag = (LabelTag)control.Tag;
                        if (labelTag == null)
                            labelTag = new LabelTag();
                    }
                    catch
                    { labelTag = new LabelTag(); }

                    labelTag.ControlX = point.X;
                    labelTag.ControlY = point.Y;
                    labelTag.ParentHeight = control.Parent.Height;
                    labelTag.ParentWidth = control.Parent.Width;
                    control.Tag = labelTag;
                }
            }
        }



        #endregion

        #region 顶上的按钮 
        //声音测试 
        private void btnAlarmTest_Click(object sender, EventArgs e)
        {
            timerSound.Enabled = true; 
        }
        private void timerSound_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerSound.Enabled = false;
            KSPrj.Alert.goSoundTest();
        }

        ////抽汽限制图 
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    ChartCQXZ cqxz = new ChartCQXZ();
        //    cqxz.Show();
        //}

        //实时测点 
        private void btnTags_Click(object sender, EventArgs e)
        {
            FrmTagCheck frm1 = new FrmTagCheck();
            frm1.Show();
        }

        //查看历史 
        private void btnHis1_Click(object sender, EventArgs e)
        {
            //SelectTagDate selectTagDate = new SelectTagDate(6, "FCZ");
            //HistoryLines historyLines = new HistoryLines(6);
            //selectTagDate.ShowDialog();
            //if (selectTagDate.bIsClose == false)
            //{
            //    if (selectTagDate.sExcelCells.Length > 0)
            //    {
            //        historyLines.SetBeginTime(selectTagDate.BeginTime);
            //        historyLines.SetEndTime(selectTagDate.EndTime);
            //        historyLines.SetExcelCells(selectTagDate.sExcelCells);
            //        historyLines.SetCheckedTags(selectTagDate.dtCheckedTags);
            //    }
            //    else
            //    {
            //        historyLines.seriesCollection = (chart1).Series;
            //    }
            //    historyLines.ShowDialog();
            //}
            //selectTagDate.Dispose();
            //historyLines.Dispose();


            //SelectTagDate在生成对象调用时，由ShowDialog()改成了Show()；
            //因为用ShowDialog()时也会造成窗口无反应的假死现象，所以注释了上面的语句，加了下面的语句。add by hlt 2017-1-18
            SelectTagDate selectTagDate = new SelectTagDate(6,chart1, "FCZ");
            selectTagDate.Show();

        }

        //报警历史 
        private void btnAlarmConfirm_Click(object sender, EventArgs e)
        {
            KSPrj.Tool.AlarmList aList = new KSPrj.Tool.AlarmList("FCZ", 6);
            aList.Show();
        }

        //坏点替代 
        private void btnOrgTD_Click(object sender, EventArgs e)
        {
            TagReplace tr = new TagReplace(6);
            tr.Show();
        }

        //停止 
        private void button2_Click(object sender, EventArgs e)
        {
            if (PWDCheck("rg"))
            {
                if (this.button2.Text.Replace(" ", "").Trim() == "运行")
                {
                    NoticeSix.OnMessageRecieved += OnMessageReceived; 
                    this.button2.Text = "停  止";
                }
                else
                {
                    NoticeSix.OnMessageRecieved -= OnMessageReceived; 
                    this.button2.Text = "运  行";
                }
            } 
        }

        //运行优化 
        private void button6_Click(object sender, EventArgs e)
        {
            Program.yxyh2.Show(); 
            this.Hide();
        }
        #endregion 

        #region 添加测点

        //系统图添加测点 
        private void btnAdd2tTag_Click(object sender, EventArgs e)
        {
            int iUnitNo = 2; // int.Parse(((Button)sender).Tag.ToString());
            control = new Label();
            control.Name = "L" + DateTime.Now.ToString("yyyyMMddHHmmssfff"); //设置label的唯一标示，用创建时间表示
            sNewLabel = control.Name;
            control.ForeColor = System.Drawing.Color.FromName("Lime");
            control.Text = DateTime.Now.ToString("New Label");
            control.AutoSize = true;
            control.Top = 100;
            control.Left = 100;
            control.BackColor = System.Drawing.Color.Black;//设置背景色透明
            control.MouseDoubleClick += new MouseEventHandler(Label_DoubleClick); //双击事件，选择测点。
            control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
            control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
            control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
            control.ContextMenuStrip = contextMenuStrip_Label;
            this.groupBox2.Controls.Add(control);
            LabelTag labelTag = new LabelTag();
            labelTag.ControlX = control.Left;
            labelTag.ControlY = control.Top;
            labelTag.ParentHeight = control.Parent.Height;
            labelTag.ParentWidth = control.Parent.Width;
            labelTag.UnitNo = (byte)iUnitNo;
            control.Tag = labelTag;
            sSql = "insert into ControlConfig(ControlName,ControlX,ControlY,ForeColor,BackColor,Type,Parent,ParentHeight,ParentWidth,UnitNo) values('"
                + control.Name + "'," + control.Location.X + "," + control.Location.Y + "," + control.ForeColor.ToArgb() + ","
                + control.BackColor.ToArgb() + ",'" + control.GetType().Name + "','" + TagLabelBinding.GetParents(control) + "',"
                + control.Parent.Height + "," + control.Parent.Width + "," + iUnitNo + ")";
            SQLHelper.ExecuteSql(sSql);
            TableControlConfig.setFill();
        }

        //参数添加测点
        private void btnAdd2cTag_Click(object sender, EventArgs e)
        {
            int iUnitNo = 2; // int.Parse(((Button)sender).Tag.ToString());
            control = new Label();
            control.Name = "L" + DateTime.Now.ToString("yyyyMMddHHmmssfff"); //设置label的唯一标示，用创建时间表示
            sNewLabel = control.Name;
            control.ForeColor = System.Drawing.Color.FromName("Lime");
            control.Text = DateTime.Now.ToString("New Label");
            control.AutoSize = true;
            control.Top = 100;
            control.Left = 100;
            control.BackColor = System.Drawing.Color.Black;//设置背景色透明
            control.MouseDoubleClick += new MouseEventHandler(Label_DoubleClick); //双击事件，选择测点。
            control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
            control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
            control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
            control.ContextMenuStrip = contextMenuStrip_Label;
            this.panelChild.Controls.Add(control);
            LabelTag labelTag = new LabelTag();
            labelTag.ControlX = control.Left;
            labelTag.ControlY = control.Top;
            labelTag.ParentHeight = control.Parent.Height;
            labelTag.ParentWidth = control.Parent.Width;
            labelTag.UnitNo = (byte)iUnitNo;
            control.Tag = labelTag;
            sSql = "insert into ControlConfig(ControlName,ControlX,ControlY,ForeColor,BackColor,Type,Parent,ParentHeight,ParentWidth,UnitNo) values('"
                + control.Name + "'," + control.Location.X + "," + control.Location.Y + "," + control.ForeColor.ToArgb() + ","
                + control.BackColor.ToArgb() + ",'" + control.GetType().Name + "','" + TagLabelBinding.GetParents(control) + "',"
                + control.Parent.Height + "," + control.Parent.Width + "," + iUnitNo + ")";
            SQLHelper.ExecuteSql(sSql);
            TableControlConfig.setFill();
        }

        #endregion 

        #region 左下系统图 
        //报警视图（棒状图）
        private void button7_Click(object sender, EventArgs e)
        {
            this.panel10.Visible = true;
            this.panel11.Visible = false;
            this.panel8.Visible = false;
            this.button7.ForeColor = Color.Red;
            this.button7.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134))); 
            this.button4.ForeColor = Color.Lime;
            this.button4.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = Color.Lime;
            this.button1.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        //相对容积流量（抛物线）
        private void button1_Click(object sender, EventArgs e)
        {
            this.panel8.Visible = true;
            this.panel10.Visible = false;
            this.panel11.Visible = false;
            this.button1.ForeColor = Color.Red;
            this.button1.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.ForeColor = Color.Lime;
            this.button4.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button7.ForeColor = Color.Lime;
            this.button7.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        //限制曲线（折线）
        private void button4_Click(object sender, EventArgs e)
        {
            this.panel11.Visible = true;
            this.panel8.Visible = false;
            this.panel10.Visible = false;
            this.button4.ForeColor = Color.Red;
            this.button4.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button7.ForeColor = Color.Lime;
            this.button7.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = Color.Lime;
            this.button1.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134))); 
        }

        /// <summary>
        /// 初始化左下角Chart图表
        /// </summary>
        private void InitCharts()
        { 

            Font fontStyle = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

            //Chart2 绘抛物线图 
            ChartTools ct2 = new ChartTools(this.chart2);
            this.chart2.BackColor = Color.Black;
            ct2.SetChartArea(Color.Transparent);
            ct2.SetAxisX(0, 1.0, 0.1, "相对容积流量", StringAlignment.Far, false, Color.LightGray, Color.LightGray, Color.LightGray, Color.LightGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            ct2.SetAxisY(0, 1.4, 0.2, "叶片相对动应力", StringAlignment.Far, false, Color.LightGray, Color.LightGray, Color.LightGray, Color.LightGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");

            this.chart2.ChartAreas[0].AxisX.TitleFont = fontStyle;
            this.chart2.ChartAreas[0].AxisY.TitleFont = fontStyle;

            //绘制阴影
            ct2.SetSeries("", new double[] { 0, 0.05 }, new double[] { 1.4, 1.4 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.LightGreen, 0, "");
            ct2.SetSeries("", new double[] { 0.05, 0.1 }, new double[] { 1.4, 1.4 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Orange, 0, "");
            ct2.SetSeries("", new double[] { 0.1, 0.27 }, new double[] { 1.4, 1.4 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Red, 0, "");
            ct2.SetSeries("", new double[] { 0.27, 0.3 }, new double[] { 1.4, 1.4 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Orange, 0, "");
            ct2.SetSeries("", new double[] { 0.3, 0.4479 }, new double[] { 1.4, 1.4 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.LightGreen, 0, "");
            ct2.SetSeries("", new double[] { 0.4479, 1.0 }, new double[] { 1.4, 1.4 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.LightGreen, 0, "");

            //五条竖线 从左至右
            ct2.SetSeries("", new double[] { 0.05, 0.05 }, new double[] { 0, 1.4 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct2.SetSeries("", new double[] { 0.1, 0.1 }, new double[] { 0, 1.4 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct2.SetSeries("", new double[] { 0.27, 0.27 }, new double[] { 0, 1.4 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct2.SetSeries("", new double[] { 0.3, 0.3 }, new double[] { 0, 1.4 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ////末级叶片零功率
            //ct2.SetSeries("mjyplgl", new double[] { 0.4479, 0.4479 }, new double[] { 0, 1.4 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            //this.chart2.Series["mjyplgl"].Points[1].Label = "末级叶片零功率";

            //横线
            //ct2.SetSeries("", new double[] { 0.1, 0.3 }, new double[] { 1.35, 1.35 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");

            //画曲线
            ct2.SetSeries("", new double[] { 0, 0.1, 0.156, 0.27 }, new double[] { 0, 0.45, 1.35, 0.45 }, SeriesChartType.Spline, ChartHatchStyle.None, Color.OrangeRed, 2, "");
            ct2.SetSeries("", new double[] { 0.27, 0.3 }, new double[] { 0.45, 0.28 }, SeriesChartType.Line, ChartHatchStyle.None, Color.OrangeRed, 2, "");
            ct2.SetSeries("", new double[] { 0.3, 0.33, 0.36, 0.38, 0.39, 0.4 }, new double[] { 0.28, 0.2, 0.16, 0.14, 0.13, 0.125 }, SeriesChartType.Spline, ChartHatchStyle.None, Color.OrangeRed, 2, "");
            ct2.SetSeries("", new double[] { 0.4, 1.0 }, new double[] { 0.12, 0.225 }, SeriesChartType.Line, ChartHatchStyle.None, Color.OrangeRed, 2, "");

            ct2.SetLegend(false, StringAlignment.Far, StringAlignment.Far);

            //Chart2 数据绑定Series 
            Series series2 = new Series();
            series2.Name = "chart2Series";
            series2.LabelForeColor = Color.White;
            series2.ChartType = SeriesChartType.Line;
            series2.BorderWidth = 3;
            //series2.Label = "#VALX";
            series2.Color = Color.Black;
            series2.Font = fontStyle;
            chart2.Series.Add(series2);

            //Chart3 绘折线图 
            ChartTools ct3 = new ChartTools(this.chart3);
            this.chart3.BackColor = Color.Black;
            ct3.SetChartArea(Color.Transparent);

            //ct3.SetAxisX(0, 0.7, 0.1, "低压缸进汽压力（绝对MPa）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            ct3.SetAxisX(0, 700, 100, "低压缸排汽流量（t/h）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            ct3.SetAxisY(0, 70, 10, "背压（绝对KPa）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            ct3.SetAxisY2(24.5, 94.5, 10, "真空（KPa）", StringAlignment.Near, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Dash, AxisArrowStyle.None, true, "");

            this.chart3.ChartAreas[0].AxisX.TitleFont = fontStyle;
            this.chart3.ChartAreas[0].AxisY.TitleFont = fontStyle;
            this.chart3.ChartAreas[0].AxisY2.TitleFont = fontStyle;

            //ct3.SetAxisX2(120, 190, 5, "低压缸排气流量（绝对）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.Triangle, "");

            //阴影
            ct3.SetSeries("", new double[] { 0, 700 }, new double[] { 70, 70 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Red, 0, "");
            ct3.SetSeries("", new double[] { 0, 136.1, 429.5, 700 }, new double[] { 25, 25, 48, 48 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.DarkOrange, 0, "");
            ct3.SetSeries("", new double[] { 0, 136.1, 429.5, 700 }, new double[] { 20, 20, 43, 43 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Green, 0, "");

            //两条直线 红色 粉色     跳机点：#VALY" + "KP
            ct3.SetSeries("tjd", new double[] { 0, 700 }, new double[] { 65, 65 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct3.SetSeries("", new double[] { 0, 700 }, new double[] { 33, 33 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Yellow, 2, "");
            this.chart3.Series["tjd"].Points[0].Label = "跳机点：#VALY" + "KP                 ";
            
            //两条红色折线 
            ct3.SetSeries("", new double[] { 0, 136.1, 429.5, 700 }, new double[] { 20, 20, 43, 43 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct3.SetSeries("", new double[] { 0, 136.1, 429.5, 700 }, new double[] { 25, 25, 48, 48 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");

            ct3.SetLegend(false, StringAlignment.Far, StringAlignment.Far);

            //Chart3 数据绑定Series 
            Series series3 = new Series();
            series3.Name = "chart3Series";
            series3.LabelForeColor = Color.White;
            series3.ChartType = SeriesChartType.Point;
            series3.MarkerSize = 16;
            series3.MarkerStyle = MarkerStyle.Circle;
            series3.Font = fontStyle;
            series3.Color = Color.White;
            chart3.Series.Add(series3);


            ////Chart3 绘折线图 
            //ChartTools ct3 = new ChartTools(this.chart3);
            //this.chart3.BackColor = Color.Black;
            //ct3.SetChartArea(Color.Transparent);

            //ct3.SetAxisX(0.0, 1.0, 0.1, "低压缸进汽压力（绝对MPa）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            //ct3.SetAxisY(2, 62, 10, "背压（绝对KPa）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            //ct3.SetAxisY2(24.5, 94.5, 10, "真空（KPa）", StringAlignment.Near, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Dash, AxisArrowStyle.None, true, "");

            //this.chart3.ChartAreas[0].AxisX.TitleFont = fontStyle;
            //this.chart3.ChartAreas[0].AxisY.TitleFont = fontStyle;
            //this.chart3.ChartAreas[0].AxisY2.TitleFont = fontStyle;

            ////ct3.SetAxisX2(120, 190, 5, "低压缸排气流量（绝对）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.Triangle, "");

            ////阴影
            //ct3.SetSeries("", new double[] { 0.0, 1.0 }, new double[] { 62, 62 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Red, 0, "");
            //ct3.SetSeries("", new double[] { 0.0, 0.3897, 0.62, 1.0 }, new double[] { 25, 25, 50, 50 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.DarkOrange, 0, "");
            //ct3.SetSeries("", new double[] { 0.0, 0.362, 0.62, 1.0 }, new double[] { 20, 20, 48, 48 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Green, 0, "");

            ////两条直线 红色 粉色     跳机点：#VALY" + "KP
            //ct3.SetSeries("tjd", new double[] { 0.0, 1.0 }, new double[] { 55, 55 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            //ct3.SetSeries("", new double[] { 0.5915, 1.0 }, new double[] { 45, 45 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Yellow, 2, "");
            //this.chart3.Series["tjd"].Points[0].Label = "跳机点：#VALY" + "KP";

            ////两条红色折线 
            //ct3.SetSeries("", new double[] { 0.0, 0.3897, 0.62, 1.0 }, new double[] { 25, 25, 50, 50 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            //ct3.SetSeries("", new double[] { 0.0, 0.362, 0.62, 1.0 }, new double[] { 20, 20, 48, 48 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");

            //ct3.SetLegend(false, StringAlignment.Far, StringAlignment.Far);

            ////Chart3 数据绑定Series 
            //Series series3 = new Series();
            //series3.Name = "chart3Series";
            //series3.LabelForeColor = Color.White;
            //series3.ChartType = SeriesChartType.Point;
            //series3.MarkerSize = 16;
            //series3.MarkerStyle = MarkerStyle.Circle;
            //series3.Font = fontStyle;
            //series3.Color = Color.White;
            //chart3.Series.Add(series3);
        }

        /// <summary>
        /// Chart1绑定
        /// </summary>
        private void dataBindChart()
        {
            try
            {
                string ss = chart1.Series[0].Name;
                if (string.IsNullOrEmpty(ss))
                {
                    ss = "F2128";
                    chart1.Series[0].YValueMembers = ss;
                    chart1.Series[0].Name = ss;
                }
            }
            catch { }

            chart1.DataSource = GlobalVariables.dtChartDataSix;
            chart1.DataBind();
            //chart1.Refresh();
        }

        /// <summary>
        /// Chart1数据绑定/凝结水量（仅用在初始化的时候绑定） BY ZZH 
        /// </summary>
        private void Chart1Bind()
        {
            if (GlobalVariables.dtChartDataSix.Rows.Count > 0)
            {
                this.chart1.DataSource = GlobalVariables.dtChartDataSix;
                this.chart1.DataBind();
            }
        }

        /// <summary>
        /// 棒状图Label绑定
        /// 低压缸排气容积流量F1323/F2323 m^3/s
        /// 流速 m/s  1320/2320 
        /// 相对容积流量F1329/F2329
        /// </summary>
        private void PicChartLabelBind()
        {
            //低压缸排气容积流量   label36
            //流速   label38
            //相对容积流量   label40 
            //this.label36.Text = CalTree.GetShowValue(2323, "m³/s", "");
            //this.label38.Text = CalTree.GetShowValue(2320, "m³/s", "");
            //this.label40.Text = CalTree.GetShowValue(2329, "", "");
        }

        /// <summary>
        /// 抛物线图表 绑定
        /// </summary>
        private void Chart2Bind()
        {
            double xValue = TagValue.GetFinishedTagValueSix("F2329"); 
            if (xValue == GlobalVariables.BadValue)
                xValue = TagValue.GetSetValueSix("2329", "SetValue");
            
            this.chart2.Series["chart2Series"].Points.Clear();
            this.chart2.Series["chart2Series"].Points.AddXY(xValue, 1.4);
            this.chart2.Series["chart2Series"].Points.AddXY(xValue, 0);
            this.chart2.Series["chart2Series"].Points[0].Label = xValue + "";
            //报警监测
            Chart2Checking(xValue);
        }

        /// <summary>
        /// 折线图表 绑定
        /// </summary>
        private void Chart3Bind()
        {
            double pointX = 550;
            double pointY = 25;
            string dygpqll = "--"; //低压缸排气流量1324/2324
            double dqy = 0.1; //大气压 
            //string dygjqyl = "--"; //低压缸进汽压力1111/2111 
            //string zk = "--"; //真空 

            try
            {
                //这里添加#2机组的XY坐标
                // X坐标修改成低压缸排汽流量 ZZH 20160830
                pointX = TagValue.GetFinishedTagValueSix("F2324");//double.Parse(CalDataTable.Rows[0]["F2169"].ToString()); 
                if (pointX == GlobalVariables.BadValue)
                {
                    pointX = TagValue.GetSetValueSix("2324", "SetValue");
                }
                dygpqll = TagValue.GetShowValueSix(2324, "t/h", ""); //CalDataTable.Rows[0]["F2324"].ToString(); 
                //dygjqyl = CalTree.GetShowValue(2111, "MPa(a)", "");
            }
            catch { }

            try
            {
                pointY = TagValue.GetFinishedTagValueSix("F4126") * 1000;//double.Parse(CalDataTable.Rows[0]["F3126"].ToString()) * 1000;
                dqy = TagValue.GetFinishedTagValueSix("F4004") * 1000;//double.Parse(CalDataTable.Rows[0]["F3004"].ToString()) * 1000;
            }
            catch { }

            //判断XY轴的值

            if (pointY == GlobalVariables.BadValue * 1000)
            {
                pointY = TagValue.GetSetValueSix("4126", "SetValue") * 1000;
            }
            if (dqy == GlobalVariables.BadValue * 1000)
            {
                dqy = TagValue.GetSetValueSix("4004", "SetValue") * 1000;
            }

            //绑定低压缸排气流量和相对容积流量
            this.label27.Text = dygpqll;
            this.label28.Text = pointY + " KPa";
            //this.label3.Text = dygjqyl;
            //真空 F3004*1000 - F3126*1000
            this.label43.Text = Math.Round(dqy - pointY, 2) + " KPa";
            try
            {
                //Y2轴 真空绑定
                double max = dqy - 2;
                double min = dqy - 62;
                this.chart3.ChartAreas[0].AxisY2.Minimum = min;
                this.chart3.ChartAreas[0].AxisY2.Maximum = max;
            }
            catch { }


            //闪点绑定
            this.chart3.Series["chart3Series"].Points.Clear();
            this.chart3.Series["chart3Series"].Points.AddXY(pointX, pointY);
            //this.chart3.Series["chart3Series"].Label = "低压缸排气流量：" + dygpqll + "\r\n低压缸进汽压力：" + dygjqyl + "\r\n真空：" + (dqy - pointY) + "KPa";

            //报警监测
            Chart3Checking(pointX, pointY);
        }

        #endregion 

        #region 报警

        AlarmReport chart1Alarm = new AlarmReport("排汽压力限制报警", 6);
        int chart1AlarmIsShow = 0;
        AlarmReport chart2Alarm = new AlarmReport("相对动应力显示报警", 6);
        int chart2AlarmIsShow = 0;
        AlarmReport chart3Alarm = new AlarmReport("背压限制报警", 6);
        int chart3AlarmIsShow = 0;
        AlarmReport ZYGCYAlarm = new AlarmReport("中压缸压差报警", 6);
        int zygcyAlarmIsShow = 0; 

        /// <summary>
        /// 棒状图报警检查
        /// </summary>
        /// <param name="values"></param>
        private void PicChartChecking(double values)
        {
            if (isFirstRun)
                return;
            //100  125
            if (values > 100 && values <= 112.5)
            {
                //黄色报警
                if (chart1AlarmIsShow == 0)
                {
                    chart1Alarm.SetText("排汽压力限制报警图黄色报警", chart1Alarm.NoticeLabel_PICYellow, AlarmType.AlarmYellow, 500, false, 0);
                    chart1Alarm.Show();
                    chart1AlarmIsShow = 1;
                }
                else
                {
                    //if (chart1Alarm.Visible == false)
                    chart1Alarm.SetText("排汽压力限制报警图黄色报警", chart1Alarm.NoticeLabel_PICYellow, AlarmType.AlarmYellow, 500, false, 0);
                    //else
                    if (chart1Alarm.Visible == true)
                        chart1Alarm.Activate();
                }
                picChart = true;
                picColor = Color.Yellow;
            }
            else if (values > 112.5 && values <= 125)
            {
                //橙色报警
                if (chart1AlarmIsShow == 0)
                {
                    chart1Alarm.SetText("排汽压力限制报警图橙色报警", chart1Alarm.NoticeLabel_PICOrange, AlarmType.AlarmOrange, 500, false, 0);
                    chart1Alarm.Show();
                    chart1AlarmIsShow = 1;
                }
                else
                {
                    //if (chart1Alarm.Visible == false)
                    chart1Alarm.SetText("排汽压力限制报警图橙色报警", chart1Alarm.NoticeLabel_PICOrange, AlarmType.AlarmOrange, 500, false, 0);
                    //else
                    if (chart1Alarm.Visible == true)
                        chart1Alarm.Activate();
                }
                picChart = true;
                picColor = Color.DarkOrange;
            }
            else if (values > 125)
            {
                //红色报警 
                if (chart1AlarmIsShow == 0)
                {
                    chart1Alarm.SetText("排汽压力限制报警图红色报警", chart1Alarm.NoticeLabel_PICRed, AlarmType.AlarmRed, 500, false, 0);
                    chart1Alarm.Show();
                    chart1AlarmIsShow = 1;
                }
                else
                {
                    //if (chart1Alarm.Visible == false)
                    chart1Alarm.SetText("排汽压力限制报警图红色报警", chart1Alarm.NoticeLabel_PICRed, AlarmType.AlarmRed, 500, false, 0);
                    //else
                    if (chart1Alarm.Visible == true)
                        chart1Alarm.Activate();
                }
                picChart = true;
                picColor = Color.Red;
            }
            else
            {
                chart1Alarm.SetText("", "", AlarmType.AlarmSafe, 0, false, 0);
                picChart = false;
            }
        }

        /// <summary>
        /// 抛物线图表报警检查
        /// </summary>
        private void Chart2Checking(double xValue)
        {
            if (isFirstRun)
                return;
            //红色报警阶段
            if (xValue > 0.1 && xValue < 0.27)
            {
                //红色警报
                if (chart2AlarmIsShow == 0)
                {
                    chart2Alarm.SetText("相对动应力显示图表红色警报", chart2Alarm.NoticeLabel_PWXRed, AlarmType.AlarmRed, 500, false, 0);
                    chart2Alarm.Show();
                    chart2AlarmIsShow = 1;
                }
                else
                {
                    //if (chart2Alarm.Visible == false)
                    chart2Alarm.SetText("相对动应力显示图表红色警报", chart2Alarm.NoticeLabel_PWXRed, AlarmType.AlarmRed, 500, false, 0);
                    //else
                    if (chart2Alarm.Visible == true)
                        chart2Alarm.Activate();
                }
                parabolaChart = true;
                parabolaColor = Color.Red;
            }
            else if (xValue >= 0.27 && xValue < 0.3) //(xValue > 0.05 && xValue <= 0.1) || 
            {
                //橙色警报
                if (chart2AlarmIsShow == 0)
                {
                    chart2Alarm.SetText("相对动应力显示图表橙色警报", chart2Alarm.NoticeLabel_PWXOrange, AlarmType.AlarmOrange, 500, false, 0);
                    chart2Alarm.Show();
                    chart2AlarmIsShow = 1;
                }
                else
                {
                    //if (chart2Alarm.Visible == false)
                    chart2Alarm.SetText("相对动应力显示图表橙色警报", chart2Alarm.NoticeLabel_PWXOrange, AlarmType.AlarmOrange, 500, false, 0);
                    //else
                    if (chart2Alarm.Visible == true)
                        chart2Alarm.Activate();
                }
                parabolaChart = true;
                parabolaColor = Color.DarkOrange;
            }
            else if (xValue >= 0.3 && xValue <= 0.4479) //(xValue < 0.05) || 
            {
                //这里应该是黄色警告
                if (chart2AlarmIsShow == 0)
                {
                    chart2Alarm.SetText("相对动应力显示图表黄色警报", chart2Alarm.NoticeLabel_PWXYellow, AlarmType.AlarmYellow, 500, true, 0);
                    chart2Alarm.Show();
                    chart2AlarmIsShow = 1;
                }
                else
                {
                    //if (chart2Alarm.Visible == false)
                    chart2Alarm.SetText("相对动应力显示图表黄色警报", chart2Alarm.NoticeLabel_PWXYellow, AlarmType.AlarmYellow, 500, true, 0);
                    //else
                    if (chart2Alarm.Visible == true)
                        chart2Alarm.Activate();
                }
                parabolaChart = false;
                //parabolaColor = Color.Yellow;
            }
            else
            {
                chart2Alarm.SetText("", "", AlarmType.AlarmSafe, 0, false, 0);
                parabolaChart = false;
            }
        }

        /// <summary>
        /// 折线图表报警检查
        /// </summary>
        private void Chart3Checking(double xValue, double yValue)
        {
            if (isFirstRun)
                return;
            //第一阶段
            if (xValue <= 0.1361)
            {
                if (yValue > 20 && yValue <= 25)
                {
                    //橙色警报
                    if (chart3AlarmIsShow == 0)
                    {
                        chart3Alarm.SetText("背压限制图表橙色警报", chart3Alarm.NoticeLabel_ZXOrange, AlarmType.AlarmOrange, 500, false, 0);
                        chart3Alarm.Show();
                        chart3AlarmIsShow = 1;
                    }
                    else
                    {
                        //if (chart3Alarm.Visible == false)
                        chart3Alarm.SetText("背压限制图表橙色警报", chart3Alarm.NoticeLabel_ZXOrange, AlarmType.AlarmOrange, 500, false, 0);
                        //else
                        if (chart3Alarm.Visible == true)
                            chart3Alarm.Activate();
                    }
                    brokenChart = true;
                    brokenColor = Color.DarkOrange;
                }
                else if (yValue > 25)
                {
                    //红色警报
                    if (chart3AlarmIsShow == 0)
                    {
                        chart3Alarm.SetText("背压限制图表红色警报", chart3Alarm.NoticeLabel_ZXRed, AlarmType.AlarmRed, 500, false, 0);
                        chart3Alarm.Show();
                        chart3AlarmIsShow = 1;
                    }
                    else
                    {
                        //if (chart3Alarm.Visible == false)
                        chart3Alarm.SetText("背压限制图表红色警报", chart3Alarm.NoticeLabel_ZXRed, AlarmType.AlarmRed, 500, false, 0);
                        //else
                        if (chart3Alarm.Visible == true)
                            chart3Alarm.Activate();
                    }
                    brokenChart = true;
                    brokenColor = Color.Red;
                }
                else
                {
                    chart3Alarm.SetText("", "", AlarmType.AlarmSafe, 0, false, 0);
                    brokenChart = false;
                }
            }
            //斜线阶段
            else if (xValue > 0.1361 && xValue < 0.4295)
            {
                double minAlarm = xValue * 78.39 + 9.33;
                double maxAlarm = xValue * 78.39 + 14.33;
                if (yValue > minAlarm && yValue <= maxAlarm)
                {
                    //橙色警报
                    if (chart3AlarmIsShow == 0)
                    {
                        chart3Alarm.SetText("背压限制图表橙色警报", chart3Alarm.NoticeLabel_ZXOrange, AlarmType.AlarmOrange, 500, false, 0);
                        chart3Alarm.Show();
                        chart3AlarmIsShow = 1;
                    }
                    else
                    {
                        //if (chart3Alarm.Visible == false)
                        chart3Alarm.SetText("背压限制图表橙色警报", chart3Alarm.NoticeLabel_ZXOrange, AlarmType.AlarmOrange, 500, false, 0);
                        //else
                        if (chart3Alarm.Visible == true)
                            chart3Alarm.Activate();
                    }
                    brokenChart = true;
                    brokenColor = Color.DarkOrange;
                }
                else if (yValue > maxAlarm)
                {
                    //红色警报
                    if (chart3AlarmIsShow == 0)
                    {
                        chart3Alarm.SetText("背压限制图表红色警报", chart3Alarm.NoticeLabel_ZXRed, AlarmType.AlarmRed, 500, false, 0);
                        chart3Alarm.Show();
                        chart3AlarmIsShow = 1;
                    }
                    else
                    {
                        //if (chart3Alarm.Visible == false)
                        chart3Alarm.SetText("背压限制图表红色警报", chart3Alarm.NoticeLabel_ZXRed, AlarmType.AlarmRed, 500, false, 0);
                        //else
                        if (chart3Alarm.Visible == true)
                            chart3Alarm.Activate();
                    }
                    brokenChart = true;
                    brokenColor = Color.Red;
                }
                else
                {
                    chart3Alarm.SetText("", "", AlarmType.AlarmSafe, 0, false, 0);
                    brokenChart = false;
                }
            }
            //最后阶段
            else if (xValue >= 0.4295)
            {
                if (yValue > 43 && yValue <= 48)
                {
                    //橙色警报
                    if (chart3AlarmIsShow == 0)
                    {
                        chart3Alarm.SetText("背压限制图表橙色警报", chart3Alarm.NoticeLabel_ZXOrange, AlarmType.AlarmOrange, 500, false, 0);
                        chart3Alarm.Show();
                        chart3AlarmIsShow = 1;
                    }
                    else
                    {
                        //if (chart3Alarm.Visible == false)
                        chart3Alarm.SetText("背压限制图表橙色警报", chart3Alarm.NoticeLabel_ZXOrange, AlarmType.AlarmOrange, 500, false, 0);
                        //else
                        if (chart3Alarm.Visible == true)
                            chart3Alarm.Activate();
                    }
                    brokenChart = true;
                    brokenColor = Color.DarkOrange;
                }
                else if (yValue > 48)
                {
                    //红色警报
                    if (chart3AlarmIsShow == 0)
                    {
                        chart3Alarm.SetText("背压限制图表红色警报", chart3Alarm.NoticeLabel_ZXRed, AlarmType.AlarmRed, 500, false, 0);
                        chart3Alarm.Show();
                        chart3AlarmIsShow = 1;
                    }
                    else
                    {
                        //if (chart3Alarm.Visible == false)
                        chart3Alarm.SetText("背压限制图表红色警报", chart3Alarm.NoticeLabel_ZXRed, AlarmType.AlarmRed, 500, false, 0);
                        //else
                        if (chart3Alarm.Visible == true)
                            chart3Alarm.Activate();
                    }
                    brokenChart = true;
                    brokenColor = Color.Red;
                }
                else
                {
                    chart3Alarm.SetText("", "", AlarmType.AlarmSafe, 0, false, 0);
                    brokenChart = false;
                }
            }
        }
        


        /// <summary>
        /// 中压缸压差报警
        /// F1021, F2021(热再热压力)-F1165, F2165(中压缸排气压力)
        /// 前后压差不得大于3.2MPa
        /// 请减少抽气量 
        /// F1409,F2409 中压缸压差值
        /// </summary>
        //private void ZYGCYCheck()
        //{
        //    double cy = 0;

        //    cy = CalTree.GetFinishedTagValue("F2409");
        //    if (cy == GlobalVariables.BadValue)
        //        return;

        //    if (cy > 2.276)
        //    {
        //        if (zygcyAlarmIsShow == 0)
        //        {
        //            ZYGCYAlarm.SetText("中压缸压差报警", "前后压差不得大于2.276MPa，请减少抽气量", AlarmType.AlarmWhite, 500, true, 1);
        //            ZYGCYAlarm.Show();
        //            zygcyAlarmIsShow = 1;
        //        }
        //        else
        //        {
        //            ZYGCYAlarm.SetText("中压缸压差报警", "前后压差不得大于2.276MPa，请减少抽气量", AlarmType.AlarmWhite, 500, true, 1);
        //            if (ZYGCYAlarm.Visible == true)
        //                ZYGCYAlarm.Activate();
        //        }
        //        ZYGYCboolAlarm = true;
        //    }
        //    else
        //    {
        //        ZYGCYAlarm.SetText("", "", AlarmType.AlarmSafe, 500, true, 0);
        //        ZYGYCboolAlarm = false;
        //    }
        //}


        #endregion

        #region 页面上所有 Lebel 闪烁报警

        bool btnTagBool = false;

        public bool picChart = false; //棒状图是否报警
        public bool parabolaChart = false; //抛物线是否报警
        public bool brokenChart = false; //折线图是否报警  
        bool ZYGYCboolAlarm = false; //中压缸压差报警 
        public Color picColor;
        public Color parabolaColor;
        public Color brokenColor; 

        Label LabelZYGYC; 
        /// <summary>
        /// 报警提示（闪烁）
        /// </summary>
        private void AlarmHint()
        {
            //坏点检查 
            if (btnTagBool)
            {
                if (this.btnTags.ForeColor == Color.Red)
                {
                    this.btnTags.ForeColor = Color.White;
                }
                else
                { this.btnTags.ForeColor = Color.Red; }
            }
            
            //抛物线图
            if (this.chart2.Series["chart2Series"].Color != Color.White)
            {
                this.chart2.Series["chart2Series"].Color = Color.White;
            }
            else
            {
                this.chart2.Series["chart2Series"].Color = Color.Black;
            }

            //折线图闪点
            if (this.chart3.Series["chart3Series"].Color != Color.Blue)
            {
                this.chart3.Series["chart3Series"].Color = Color.Blue;
            }
            else
            {
                this.chart3.Series["chart3Series"].Color = Color.Red;
            }


            //棒状图
            if (picChart)
            {
                if (this.button7.ForeColor == Color.DarkBlue)
                    this.button7.ForeColor = picColor;
                else
                    this.button7.ForeColor = Color.DarkBlue;
            }
            else
            {
                if (this.panel10.Visible == false)
                    this.button7.ForeColor = Color.Lime;
                else
                    this.button7.ForeColor = Color.Red;
            }

            //抛物线
            if (parabolaChart)
            {
                if (this.button1.ForeColor == Color.DarkBlue)
                    this.button1.ForeColor = parabolaColor;
                else
                    this.button1.ForeColor = Color.DarkBlue;
            }
            else
            {
                if (this.panel8.Visible == false)
                    this.button1.ForeColor = Color.Lime;
                else
                    this.button1.ForeColor = Color.Red;
            }

            //折线
            if (brokenChart)
            {
                if (this.button4.ForeColor == Color.DarkBlue)
                    this.button4.ForeColor = brokenColor;
                else
                    this.button4.ForeColor = Color.DarkBlue;
            }
            else
            {
                if (this.panel11.Visible == false)
                    this.button4.ForeColor = Color.Lime;
                else
                    this.button4.ForeColor = Color.Red;
            }

            ////中压缸压差报警
            //if (ZYGYCboolAlarm)
            //{
            //    if (LabelZYGYC.ForeColor == Color.DodgerBlue)
            //        LabelZYGYC.ForeColor = Color.Red;
            //    else
            //        LabelZYGYC.ForeColor = Color.DodgerBlue;
            //}
            //else
            //    LabelZYGYC.ForeColor = Color.DodgerBlue;

        }

        #endregion

        #region 空冷岛阀门状态绑定
        /// <summary>
        /// 空冷岛阀门状态
        /// 
        /// #1机组以1结尾
        /// picBoxA11/picBoxB11/picBoxC11
        /// picBoxA21/picBoxB21/picBoxC21
        /// 
        /// #2机组以2结尾
        /// picBoxA12/picBoxB12/picBoxC12
        /// picBoxA22/picBoxB22/picBoxC22 
        /// </summary>
        //private void KLDStateCheck()
        //{
        //    if (GlobalVariables.UnitNumber == 1)
        //    {
        //        int A1 = (int)CalTree.GetFinishedTagValue("F1401");
        //        int B1 = (int)CalTree.GetFinishedTagValue("F1402");
        //        int C1 = (int)CalTree.GetFinishedTagValue("F1403");
        //        int A2 = (int)CalTree.GetFinishedTagValue("F1404");
        //        int B2 = (int)CalTree.GetFinishedTagValue("F1405");
        //        int C2 = (int)CalTree.GetFinishedTagValue("F1406");

        //        RedGreen("picBoxA11", A1);
        //        RedGreen("picBoxB11", B1);
        //        RedGreen("picBoxC11", C1);
        //        RedGreen("picBoxA21", A2);
        //        RedGreen("picBoxB21", B2);
        //        RedGreen("picBoxC21", C2);
        //    }
        //    else
        //    {
        //        int A1 = (int)CalTree.GetFinishedTagValue("F2401");
        //        int B1 = (int)CalTree.GetFinishedTagValue("F2402");
        //        int C1 = (int)CalTree.GetFinishedTagValue("F2403");
        //        int A2 = (int)CalTree.GetFinishedTagValue("F2404");
        //        int B2 = (int)CalTree.GetFinishedTagValue("F2405");
        //        int C2 = (int)CalTree.GetFinishedTagValue("F2406");

        //        RedGreen("picBoxA12", A1);
        //        RedGreen("picBoxB12", B1);
        //        RedGreen("picBoxC12", C1);
        //        RedGreen("picBoxA22", A2);
        //        RedGreen("picBoxB22", B2);
        //        RedGreen("picBoxC22", C2);
        //    }
        //}
        private void RedGreen(string picName, int open)
        {
            PictureBox pic = null;
            try
            {
                pic = this.groupBox2.Controls.Find(picName, false)[0] as PictureBox;
                if (open == 1)
                    pic.BackgroundImage = global::KSPrj.Properties.Resources.verticalRed;
                else
                    pic.BackgroundImage = global::KSPrj.Properties.Resources.verticalGreen;
            }
            catch { pic.BackgroundImage = global::KSPrj.Properties.Resources.verticalGreen; }
        }

        #endregion

        #region 图表双击事件

        private void chart1_DoubleClick(object sender, EventArgs e)
        {
            FrmShowHis myfrmshouHis = new FrmShowHis(6);
            try
            {
                myfrmshouHis.isfromZhu = true;
                myfrmshouHis.stempTags = chart1.Series[0].Name;
                rightBottId = chart1.Series[0].Name;
                myfrmshouHis.stempSTags = new string[]{
                                             chart1.Series[0].LegendText+"/"+chart1.Series[0].Name};
                myfrmshouHis.ShowDialog();
                if (myfrmshouHis.ismin)
                {
                    //chart1.Series[0].Name = rightBottId;
                    //this.chart1.Series[0].YValueMembers = rightBottId;
                    //this.chart1.Series[0].LegendText = ((LabelTag)label.Tag).TagDesc.ToString();
                    //isminReturn = true;
                    //dataBindChart();
                    //isminReturn = false;
                    chart1.Refresh();
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs(ex.ToString());
            }
        } 

        //棒状图双击事件 
        private void panel10_DoubleClick(object sender, EventArgs e)
        {
            ChartPicLine cpl = new ChartPicLine(6);
            cpl.ShowDialog();
            cpl.Dispose();
        }

        //折线图双击事件 
        private void chart3_DoubleClick_1(object sender, EventArgs e)
        {
            ChartBrokenline ct3 = new ChartBrokenline(6);
            ct3.ShowDialog();
            ct3.Dispose();
        }

        //抛物线图双击事件 
        private void chart2_DoubleClick(object sender, System.EventArgs e)
        {
            ChartParabola ct2 = new ChartParabola(6);
            ct2.ShowDialog();
            ct2.Dispose();
        }

        #endregion

        #region 右键菜单
        //删除 
        private void contextMenuStrip_Label_del_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除此测点吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    //注意这里，可能需要调整ZZH   20160829
                    if (control.GetType() == typeof(PictureBox) || control.Parent.Name == "panel10")
                    { 
                        SQLHelper.ExecuteSql("delete [ControlConfig_PIC] where ControlName='" + control.Name + "'");
                    }

                    ArrayList listSql = new ArrayList();
                    listSql.Add("delete [ControlConfig] where ControlName='" + control.Name + "'");
                    listSql.Add("delete [TagLabelBinding] where LabelName='" + control.Name + "'");
                    SQLHelper.ExecuteSql(listSql);
                    tagLabelBinding.m_viewTagLabelBinding.RowFilter = "LabelName='" + control.Name + "'";
                    if (tagLabelBinding.m_viewTagLabelBinding.Count > 0)
                        tagLabelBinding.m_viewTagLabelBinding[0].Delete();
                    control.Visible = false;
                    control.Dispose();
                }
                catch (Exception ex)
                { WriteLog.WriteLogs(ex.ToString()); }
            }
        }

        //绑定测点 
        private void contextMenuStrip_Label_DataBinding_Click(object sender, EventArgs e)
        {
            tagLabelBinding.label = (Label)this.control;
            tagLabelBinding.ShowDialog();
        }

        //保存测点位置 
        private void contextMenuStrip_Label_SaveLabel_Click(object sender, EventArgs e)
        {
            Control parentControl = (sender as Control).Parent;
            LabelTag labelTag = null;
            ArrayList lstSql = new ArrayList();
            Rectangle rect;
            foreach (Control control in parentControl.Controls)
            {
                try
                {
                    labelTag = (LabelTag)(control.Tag);
                    rect = control.Bounds;
                    lstSql.Add("update ControlConfig set ControlX=" + rect.X + ",ControlY=" + rect.Y + ",ParentHeight=" + parentControl.Height
                        + ",ParentWidth=" + parentControl.Width + " where ControlName='" + control.Name + "'");
                }
                catch { }
            }
            SQLHelper.ExecuteSql(lstSql);
            MessageBox.Show("保存完成！");
        }

        //保存单个测点位置 
        private void contextMenuStrip_Label_SaveOneLabel_Click(object sender, EventArgs e)
        {
            Control parentControl = control.Parent;

            string sSql = "insert into [ControlConfig](ControlName,ControlX,ControlY,Parent,Type,ForeColor,BackColor,ParentHeight,ParentWidth,ControlText) values(";
            string sSelect = "select * from [ControlConfig] where ControlName='" + control.Name + "'";
            DataTable dt = SQLHelper.ExecuteDt(sSelect);
            if (dt.Rows.Count > 0)
                sSql = "update ControlConfig set ControlX=" + control.Bounds.X + ",ControlY=" + control.Bounds.Y + ",ParentHeight="
                    + parentControl.Height + ",ParentWidth=" + parentControl.Width + ",ControlText='" + control.Text + "'  where ControlName='" + control.Name + "'";
            else
                sSql += "'" + control.Name + "'," + control.Bounds.X + "," + control.Bounds.Y + ",'Form1." + control.Parent.Name + "','Label',"
                    + control.ForeColor.ToArgb() + "," + control.BackColor.ToArgb() + "," + control.Parent.Height + ","
                    + control.Parent.Width + ",'" + control.Text + "')";
            SQLHelper.ExecuteDt(sSql);
            MessageBox.Show("保存完成！");
        }

        //保存大字体 
        private void contextMenuStrip_Label_SetBigFont_Click(object sender, EventArgs e)
        {
            SQLHelper.ExecuteSql("update ControlConfig set FontSize=16 where ControlName='" + control.Name + "'");
            control.Font = new System.Drawing.Font("宋体", 16, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        //保存小字体 
        private void contextMenuStrip_Label_SetSmallFont_Click(object sender, EventArgs e)
        {
            SQLHelper.ExecuteSql("update ControlConfig set FontSize=NULL where ControlName='" + control.Name + "'");
            control.Font = Font;
        }

        //保存图片位置 
        private void savePicLocation_Click(object sender, EventArgs e)
        {
            Control parentControl = control.Parent;
            string path = PublicFunction.GetControlParents(control, "");
            int ControlWidth = 0;
            int ControlHeight = 0;
            if (control.GetType() == typeof(PictureBox))
            {
                ControlWidth = control.Width;
                ControlHeight = control.Height;
            }
            int a2 = control.ForeColor.ToArgb();
            int a1 = control.BackColor.ToArgb();
            string sSql = "insert into [ControlConfig_PIC](ControlName,ControlX,ControlY,ControlWidth,ControlHeight,Parent,Type,ForeColor,BackColor,ParentHeight,ParentWidth,ControlText) values(";
            string sSelect = "select * from [ControlConfig_PIC] where ControlName='" + control.Name + "'";
            DataTable dt = SQLHelper.ExecuteDt(sSelect);
            if (dt.Rows.Count > 0)
                sSql = "update ControlConfig_PIC set ControlX=" + control.Bounds.X + ",ControlY=" + control.Bounds.Y + ",ForeColor=" + control.ForeColor.ToArgb() + ",BackColor=" + control.BackColor.ToArgb() + ",ControlWidth=" + ControlWidth + ",ControlHeight=" + ControlHeight + ",Parent='" + path + "',ParentHeight="
                    + parentControl.Height + ",ParentWidth=" + parentControl.Width + ",ControlText='" + control.Text + "'  where ControlName='" + control.Name + "'";
            else
                sSql += "'" + control.Name + "'," + control.Bounds.X + "," + control.Bounds.Y + "," + ControlWidth + "," + ControlHeight + ",'" + path + "','" + control.GetType() + "',"
                    + control.ForeColor.ToArgb() + "," + control.BackColor.ToArgb() + "," + control.Parent.Height + ","
                    + control.Parent.Width + ",'" + control.Text + "')";
            SQLHelper.ExecuteDt(sSql);
            MessageBox.Show("保存完成！");
        }
        #endregion 

        public bool PWDCheck(string name)
        { 
            string pwd = SQLHelper.ExecuteScalar("select pws from SysPara where name='" + name + "'"); 
            FrmInputPws fip = new FrmInputPws(name == "rg" ? "请输入(热工检修人员)密码" : "请输入(运行人员)密码");
            fip.ShowDialog();
            string strRet = fip.Text;

            string[] strRetA = strRet.Split(',');
            if (strRet.Length == 0)
                return false;
            if (strRetA[1] == "ok")
            {
                if (pwd == strRetA[0])
                {
                    fip.Dispose();
                    return true;
                }
                else
                {
                    MessageBox.Show("密码错误！");
                    fip.Dispose();
                    return false;
                }
            }
            return false;
        }

        //检查实时测点是否有不正常的
        /// <summary>
        /// 检查实时测点是否有不正常的
        /// </summary>
        /// <param name="dttags"></param>
        /// <returns></returns>
        private static bool CheckTags(DataTable dttags)
        {
            bool bRet = true;
            if (dttags == null || dttags.Rows.Count < 1)
            {
                return bRet;
            }
            if (dtCurrentUnitTag == null || dtCurrentUnitTag.Rows.Count <= 0)
            {
                GlobalVariables.dtTagsSetSix.DefaultView.RowFilter = "datasourcesNo=1";
                dtCurrentUnitTag = GlobalVariables.dtTagsSetSix.DefaultView.ToTable();
                GlobalVariables.dtTagsSetSix.DefaultView.RowFilter = "";
            }
            for (int i = 0; i < dtCurrentUnitTag.Rows.Count; i++)
            {
                string cell = dtCurrentUnitTag.Rows[i]["ExcelCell"].ToString();
                string sss = dttags.Rows[dttags.Rows.Count - 1][cell].ToString();
                //string s1 = GlobalVariables.ReplaceValue.ToString();
                //只要发现一个不合格的数据，就报警
                //if (dttags.Rows[dttags.Rows.Count - 1][cell].ToString().Equals(GlobalVariables.ReplaceValue.ToString()))
                if (string.IsNullOrEmpty(sss))
                {
                    bRet = false;
                    break;
                }
            }
            return bRet;
        }

        #region Timer事件 
        private void timer2_Tick(object sender, EventArgs e)
        {
            ChangeLabels(null);
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                this.label6.Text = string.Format("{0:F}", DateTime.Now);// System.DateTime.Now.ToLongTimeString();//+ "OPC取数用时："  + " 000 秒"
                if (PIRead.IsOperatingPI && (DateTime.Now - PIRead.TimeOfBeginOperatingPI).TotalSeconds > 4)
                    this.label6.Text = DateTime.Now.ToLongDateString() + "  读PI已" + (DateTime.Now - PIRead.TimeOfBeginOperatingPI).TotalSeconds.ToString("0.0") + "秒！";
                //报警检查 
                AlarmHint();
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
        }
        #endregion 

        #region Label双击事件
        /// <summary>
        /// Label双击显示曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Label_DoubleClick(object sender, MouseEventArgs e)
        {
            FrmShowHis myfrmshouHis = new FrmShowHis(6);
            try
            {
                Label label = (Label)sender;
                string labelId = ((LabelTag)label.Tag).TagID.ToString();
                myfrmshouHis.isfromZhu = true;
                myfrmshouHis.stempTags = "F" + labelId;
                rightBottId = "F" + labelId;
                //如果调整值带有F3004，需要加上（绝对） 
                string str = TagValue.GetSetValueSix(labelId, "adjustValue")+"";
                if (str != null && str.Contains("F4004"))
                    myfrmshouHis.stempSTags = new string[] { ((LabelTag)label.Tag).TagDesc.ToString() + "（绝对）/F" + labelId };
                else
                    myfrmshouHis.stempSTags = new string[] { ((LabelTag)label.Tag).TagDesc.ToString() + "/F" + labelId };

                myfrmshouHis.ShowDialog();
                if (myfrmshouHis.ismin)
                {
                    chart1.Series[0].Name = rightBottId;
                    this.chart1.Series[0].YValueMembers = rightBottId;
                    this.chart1.Series[0].LegendText = ((LabelTag)label.Tag).TagDesc.ToString();
                    isminReturn = true;
                    dataBindChart();
                    isminReturn = false;
                    chart1.Refresh();
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs(ex.ToString());
            }
        }

        /// <summary>
        /// Label绑定双击事件 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="id"></param>
        /// <param name="desc"></param>
        private void LabelBindDoubleClick(Label label, int id, string desc)
        {
            LabelTag labelTag = new LabelTag();
            labelTag.TagID = id;
            labelTag.TagDesc = desc;
            label.Tag = labelTag;
            label.MouseDoubleClick -= Label_DoubleClick;
            label.MouseDoubleClick += Label_DoubleClick;
        }


        private void label1_DoubleClick(object sender, EventArgs e)
        {
            ContactUs cu = new ContactUs();
            cu.ShowDialog();
            cu.Dispose();
        }
        #endregion 

        //计算参数 更多按钮 
        private void button3_Click(object sender, EventArgs e)
        {
            MoreTags mt = new MoreTags(6);
            mt.ShowDialog();
            mt.Dispose();
        }

        //关闭按钮 
        private void btnQuit_Click(object sender, EventArgs e)
        {
            Program.mainForm.Show();
            this.Visible = false;
        }

        private void groupBox2_DoubleClick(object sender, EventArgs e)
        {
            systemGraph.ShowDialog();
        }

        private void FCZForm_Resize(object sender, EventArgs e)
        {
            int formoldwidth = this.Width;
            //左边的宽度
            this.panelParent.Width = (int)(formoldwidth * 0.41);
            this.panel5.Width = (int)(formoldwidth * 0.41);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ChartCQXZ cqxz = new ChartCQXZ(6);
            cqxz.ShowDialog();
            cqxz.Dispose();
        }

        #region 好处归热好处归电
        private void rdoHCGR_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoHCGD_CheckedChanged(object sender, EventArgs e)
        {

        }
        #endregion 

        //添加右侧板块文字
        private void button8_Click(object sender, EventArgs e)
        {

        }




    }
}
