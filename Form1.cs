using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;
using System.Diagnostics;
using HAOCommon;
using YHYXYH.YXYH;
using YHYXYH.Tool;
using KSPrj.Tool;
using KSPrj.YXYH;
using WinHost;
using System.Timers;
using HAOCommon.PI;

namespace KSPrj
{
    public partial class Form1 : Form
    {
        //public static char cFlag = '1';
        public static string CColor = "green";
        string sUnit1HCGD_IDs = "1184,1186,1190,1262";//1号机组好处归电要显示的ID
        string sUnit1HCGR_IDs = "1380,1381,3385,3399";//1号机组好处归热要显示的ID
        string sUnit2HCGD_IDs = "2184,2186,2190,2262";//2号机组好处归电要显示的ID
        string sUnit2HCGR_IDs = "2380,2381,3385,3399";//2号机组好处归热要显示的ID
        DateTime timeGCCollect = DateTime.Now;//系统垃圾回收时间
        /// <summary>
        /// FormConfig 配置表
        /// </summary>
        private DataTable formConfigDt = new DataTable();

        //当前机组的所以测点 
        public static DataTable dtCurrentUnitTag;

        private bool isminReturn = false;
        //private Panel mypanel = null;
        private Control control;
        private string rightBottId = "";
        int isShow = 0; //报警弹出框状态，0为正常未报警，1为报警弹出，2为报警确认后显示在主页
        double formoldwidth;    //窗体原始宽度   
        double formoldheight;   //窗体原始高度 
        static System.Timers.Timer timerSound = new System.Timers.Timer(100);//报警的定时器
        SystemGraph systemGraph = new SystemGraph();
        bool isFirstRun = true;
        public Form1()
        {
            InitializeComponent();
            AddConfigedControl();
            init();

            formoldwidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            formoldheight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            if (1440 != System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width || 900 != System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height)
            {
                bianliControl1(this);
            }
            //是否启用编辑 BY ZZH 
            if (GlobalVariables.IsCanAddLabel)
            {
                this.btnAdd1cTag.Visible = true;
                this.btnAdd1tTag.Visible = true;
                this.btnAdd2cTag.Visible = true;
                this.btnAdd2tTag.Visible = true;
                this.btnAdd3cTag.Visible = true;
                this.btnAdd3tTag.Visible = true;
            }
            else
            {
                this.btnAdd1cTag.Visible = false;
                this.btnAdd1tTag.Visible = false;
                this.btnAdd2cTag.Visible = false;
                this.btnAdd2tTag.Visible = false;
                this.btnAdd3cTag.Visible = false;
                this.btnAdd3tTag.Visible = false;
            }

            //Timer2没有删除 如果委托不合适 可以继续使用 BY ZZH 
            //timer2_Tick(this, new EventArgs());
            this.timer2.Interval = GlobalVariables.RefIntvel;
            this.timer2.Enabled = true;
            //try
            //{ ChangeLabels(GlobalVariables.dtOneRowData); }
            //catch { }

            //AddConfigedControl();
            SetBindingLabelsText();
            SetLabelVisible();
            InitCharts(); //初始化Chart2 Chart3 BY ZZH 
            this.tabPage1.DoubleClick += tabPage1_DoubleClick;
            this.chart2.DoubleClick += chart2_DoubleClick;
            this.chart3.DoubleClick += chart3_DoubleClick;
            Chart1Bind(); //Chart1数据绑定（仅用在初始化的时候） BY ZZH 
            Chart2Bind(); //Chart2数据绑定 BY ZZH 
            Chart3Bind(); //Chart3数据绑定 BY ZZH 

            this.tabPage1.BorderStyle = BorderStyle.None;
            this.tabPage2.BorderStyle = BorderStyle.None;
            this.tabPage3.BorderStyle = BorderStyle.None;

            if (GlobalVariables.UnitNumber == 1)
                this.rdoUnit1.Checked = true;
            else
                this.rdoUnit2.Checked = true;


            //Notice.OnMessageRecieved += OnMessageReceived;
            //upd = new UpdateControls(ChangeLabels);


            //foreach (Control ctrl1 in this.groupBox1.Controls)
            //{
            //    if (ctrl1.GetType() == typeof(Label))
            //    {
            //        ctrl1.ContextMenuStrip = contextMenuStrip_Label;
            //        ctrl1.MouseDown += control_MouseDown;
            //    }
            //} 

            timerSound.Elapsed += new ElapsedEventHandler(timerSound_Elapsed);


            //LabelCQYL = this.groupBox1.Controls.Find("Label5CYL", false)[0] as Label;
            //LabelZYGYC = this.groupBox1.Controls.Find("LabelZYGYC", false)[0] as Label;
            //LabelDYGPQWD = this.groupBox1.Controls.Find("LabelDYGPQWD", false)[0] as Label;
            //LabelZYGPQWD = this.groupBox1.Controls.Find("LabelZYGPQWD", false)[0] as Label;
            //LabelGRCQL = this.groupBox1.Controls.Find("LabelGRCQL", false)[0] as Label;

            //为固定的Label绑定双击事件 
            LabelBindDC();
        }

        /// <summary>
        /// 棒状图双击事件 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabPage1_DoubleClick(object sender, EventArgs e)
        {
            ChartPicLine cpl = new ChartPicLine();
            cpl.ShowDialog();
            cpl.Dispose();
        }

        //把相对与control控件的坐标，转换成相对于窗体的坐标。
        private Point getPointToForm(Point p)
        {
            return this.PointToClient(PointToScreen(p));
        }

        private void init()
        {
            this.Size = new Size(1440, 900); //设置mainform的大小

            //locationid为3的 是Chart1绑定的数据 X轴是时间 Y轴是凝结水量 BY ZZH 
            //else //if (dtr["locationId"].ToString().Trim().Equals("3"))
            //{

            //初始化Chart1 BY ZZH 
            if (GlobalVariables.UnitNumber == 1)
                chart1.Series.Add("F1128");
            else
                chart1.Series.Add("F2128");
            chart1.Series[0].Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            chart1.Series[0].LegendText = "凝结水量";
            chart1.Series[0].ChartType = SeriesChartType.Line; //图表类型为曲线
            chart1.Series[0].XValueType = ChartValueType.DateTime;
            chart1.DoubleClick += chart1_DoubleClick;

            for (int ii = 0; ii < chart1.Series.Count; ii++)
            {
                chart1.Series[ii].XValueMember = "valuetime";
                chart1.Series[ii].YValueMembers = chart1.Series[ii].Name;
            }

            //chart1.Series.Add("F" + dtr["labelTagId"].ToString());
            ////Legend先注释掉 BY ZZH 
            ////chart1.Series[i].LegendText = taginfoHs1[dtr["labelTagId"].ToString()].ToString();
            //chart1.Series[i].ChartType = SeriesChartType.Line; //图表类型为曲线
            //this.chart1.Series[i].XValueType = ChartValueType.DateTime;
            //i++;
            //} 

            //}

            ////初始化dtBind BY ZZH 
            //string chartSqlStr = "select * from " + GlobalVariables.CurrentTable + " where 1=2";
            //dtBind = SQLHelper.ExecuteDt(chartSqlStr);

            initControlData();
        }
        //#endregion
        #region form窗体缩放所用的方法
        void initControlData()
        {
            //7个大块控件单独处理
            bianliControl(this);
        }
        void bianliControl(Control ctrl)
        {
            double scalewh;     //控件宽高比
            this.BackgroundImageLayout = ImageLayout.Stretch;
            //scalewh = (double)ctrl.Width / (double)ctrl.Height;
            if (ctrl.Tag == null)
            {
                ctrl.Tag = new LabelTag(ctrl.Left + " " + ctrl.Top + " " + ctrl.Width + " " + ctrl.Height + " ");     //将控件的Left,Top,Width,宽高比放入控件的Tag内
            }
            else
            {
                try
                {
                    ctrl.Tag = LabelTag.addReSize((LabelTag)ctrl.Tag, ctrl.Left + " " + ctrl.Top + " " + ctrl.Width + " " + ctrl.Height + " "); //将控件的Left,Top,Width,宽高比放入控件的Tag内
                }
                catch { }
            }

            if (ctrl.Controls.Count > 0)
            {
                foreach (Control ctrl1 in ctrl.Controls)
                {
                    bianliControl(ctrl1);
                }

            }
        }
        void bianliControl1(Control ctrl)
        {
            double scalex;  //水平伸缩比   
            double scaley;  //垂直伸缩比   
            long i;
            int temppos;
            string temptag = "";
            double[] pos = new double[4];   //pos数组保存当前控件的left,top,width,height    
            scalex = (double)formoldwidth / 1440;//this.Width / formoldwidth; //formoldwidth;
            scaley = (double)formoldheight / 900;//this.Height / formoldheight; //formoldheight;
            if (ctrl.Name == "Form1")
            {
                ctrl.Left = 0;
                ctrl.Top = 0;
                ctrl.Width = (int)formoldwidth;
                ctrl.Height = (int)formoldheight;
            }
            else
            {
                try
                {
                    temptag = ((LabelTag)ctrl.Tag).ReSizeStr.ToString();
                }
                catch { return; }
                for (i = 0; i <= 3; i++)
                {
                    temppos = temptag.IndexOf(" ");
                    if (temppos > 0)
                    {
                        pos[i] = Convert.ToDouble(temptag.Substring(0, temppos));   //从Tag中取出参数   
                        temptag = temptag.Substring(temppos + 1);
                    }
                    else
                        pos[i] = 0;
                }
                ctrl.Left = (int)(pos[0] * scalex);
                ctrl.Top = (int)(pos[1] * scaley);
                try
                {
                    if (ctrl.Width > 2)
                    {
                        ctrl.Width = (int)(pos[2] * scalex);
                    }
                }
                catch (Exception)
                {
                    ctrl.Width = this.Width;
                }
                ctrl.Width = (int)(pos[2] * scalex);
                try
                {
                    ctrl.Height = (int)(pos[3] * scaley); //(int)((double)ctrl.Width / pos[3]);   //高度由宽高比算出   
                    if (ctrl.Height < 2)
                        ctrl.Height = 2;
                }
                catch (Exception)
                {
                    ctrl.Height = this.Height; //(int)((double)ctrl.Width / pos[3]);   //高度由宽高比算出   
                }
            }
            //if (ctrl.Name == "Form1") {
            //    ctrl.Left = 0;
            //    ctrl.Top = 0;
            //    ctrl.Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            //    ctrl.Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            //}
            if (ctrl.Controls.Count > 0 && (ctrl.Name != "groupBox2" && ctrl.Name != "groupBox1"))
            {
                foreach (Control ctrl1 in ctrl.Controls)
                {
                    bianliControl1(ctrl1);
                }
            }

        }
        #endregion
        /// <summary>
        /// Label双击显示曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Label_DoubleClick(object sender, MouseEventArgs e)
        {
            FrmShowHis myfrmshouHis = new FrmShowHis();
            try
            {
                Label label = (Label)sender;
                string labelId = ((LabelTag)label.Tag).TagID.ToString();
                myfrmshouHis.isfromZhu = true;
                myfrmshouHis.stempTags = "F" + labelId;
                rightBottId = "F" + labelId;
                //如果调整值带有F3004，需要加上（绝对） 
                string str = CalTree.GetValueByIdName(labelId, "adjustValue");
                if (str != null && str.Contains("F3004"))
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
        //退出按钮的单击事件
        private void btnQuit_Click(object sender, EventArgs e)
        {
            Program.mainForm.Show();
            this.Hide();


            ////消息框中需要显示哪些按钮，此处显示“确定”和“取消”
            //MessageBoxButtons messButton = MessageBoxButtons.OKCancel;

            ////"确定要退出吗？"是对话框的显示信息，"退出系统"是对话框的标题
            ////默认情况下，如MessageBox.Show("确定要退出吗？")只显示一个“确定”按钮。
            //DialogResult dr = MessageBox.Show("确定要退出吗?", "退出系统", messButton);
            //if (dr == DialogResult.OK) //如果点击“确定”按钮
            //{
            //    string sSql = "select * from syspara";
            //    DataTable dtBasepwt = SQLHelper.ExecuteDt(sSql);
            //    //string s
            //    FrmInputPws fip = new FrmInputPws();
            //    fip.ShowDialog();
            //    string strRet = fip.Text;

            //    string[] strRetA = strRet.Split(',');
            //    if (strRet.Length == 0)
            //        return;
            //    if (strRetA[1] == "ok")
            //    {
            //        if (dtBasepwt.Rows[0]["pws"].ToString() == strRetA[0])
            //        {
            //            fip.Dispose();
            //            this.Close();
            //            this.Dispose();
            //            Application.Exit();
            //        }
            //        else
            //        {
            //            MessageBox.Show("密码错误！");
            //            fip.Dispose();
            //        }
            //    }
            //}
            //else //如果点击“取消”按钮
            //    return;
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
                if (GlobalVariables.UnitNumber == 1)
                {
                    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "([id]<2000 or [id]>3000) and datasourcesNo=1";
                    dtCurrentUnitTag = GlobalVariables.dtTagsSet.DefaultView.ToTable();
                    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "";
                }
                else
                {
                    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "[id]>2000 and datasourcesNo=1";
                    dtCurrentUnitTag = GlobalVariables.dtTagsSet.DefaultView.ToTable();
                    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "";
                }
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

        //取数计算棒图
        double douBar1Value = 50;
        //public bool Freshisfinished = true;

        private void timer2_Tick(object sender, EventArgs e)
        {
            // Timer2，先不使用，使用下面的委托方法，这样不仅能实时得到通知，而且省去了一个Timer控件
            // 如果委托不行，可以再使用Timer2
            // BY ZZH
            ChangeLabels(null);
        }


        private void ChangeLabels(DataTable dt)
        {
            try
            {

                #region 绑定chart数据

                try
                {
                    dataBindChart(); //绑定chart 数据
                    Application.DoEvents();
                    //dataBindPositionChart(); //Chart2绑定 ZZH 
                    Chart2Bind();
                    Chart3Bind();
                    Application.DoEvents();
                    //空冷岛阀门状态 
                    KLDStateCheck();
                }
                catch (Exception e1)
                {
                    WriteLog.WriteLogs("绑定chart 数据报错" + e1.ToString());
                }

                #endregion

                #region 给每个label赋值

                SetBindingLabelsText();
                Application.DoEvents();

                #endregion

                #region 报警检查

                try
                {
                    //除了三个重要报警外，其他报警先屏蔽掉 BY ZZH 
                    ////空冷岛报警检查 
                    //KLDCheck();
                    ////低压缸排气温度报警检查 
                    //DYGPQWDCheck();

                    //double fuhe = 0;
                    //double cql11 = 0;
                    //if (GlobalVariables.UnitNumber == 1)
                    //{
                    //    cql11 = CalTree.GetFinishedTagValue("F1110");
                    //    fuhe = CalTree.GetFinishedTagValue("F1145");
                    //}
                    //else
                    //{
                    //    cql11 = CalTree.GetFinishedTagValue("F2110");
                    //    fuhe = CalTree.GetFinishedTagValue("F2145");
                    //}
                    ////负荷小于100  就不做报警判断了
                    //if (fuhe != GlobalVariables.BadValue && fuhe > 100)
                    //{
                    //    //抽汽压力报警检查
                    //    CQYLCheck();

                    //    //中压缸压差报警检查
                    //    ZYGCYCheck();

                    //    //中压缸排气温度报警检查 
                    //    ZYGPQWDCheck();

                    //    //抽汽限制报警
                    //    if (cql11 > 5)
                    //        GRCQLCheck();
                    //    else 
                    //    {
                    //        CQXZboolAlarm = false;
                    //        if (GRCQLAlarm.Visible == true)
                    //            GRCQLAlarm.Visible = false;
                    //    } 
                    //}
                    //else
                    //{
                    //    //如果不需要报警，就隐藏弹出框，取消文本闪烁 
                    //    if (CQYLboolAlarm) 
                    //    {
                    //        CQYLboolAlarm = false;
                    //        if (CQYLAlarm.Visible == true)
                    //            CQYLAlarm.Visible = false;
                    //    } 
                    //    if (ZYGYCboolAlarm) 
                    //    {
                    //        ZYGYCboolAlarm = false;
                    //        if (ZYGCYAlarm.Visible == true)
                    //            ZYGCYAlarm.Visible = false;
                    //    } 
                    //    if (ZYGPQWDboolAlarm) 
                    //    {
                    //        ZYGPQWDboolAlarm = false;
                    //        if (ZYGPQWDAlarm.Visible == true)
                    //            ZYGPQWDAlarm.Visible = false;
                    //    } 
                    //    if (CQXZboolAlarm) 
                    //    {
                    //        CQXZboolAlarm = false;
                    //        if (GRCQLAlarm.Visible == true)
                    //            GRCQLAlarm.Visible = false;
                    //    } 
                    //}
                }
                catch (Exception ex)
                { WriteLog.WriteLogs(ex.ToString()); }
                #endregion

                #region 检查实时测点是否有不正确的
                try
                {
                    //原始数据若有不合法，开始报警
                    if (!CheckTags(GlobalVariables.dtOneRowData))
                        btnTagBool = true;
                    else
                    {
                        btnTagBool = false;
                        this.btnTags.ForeColor = Color.White;
                    }
                }
                catch (Exception ex)
                {
                    WriteLog.WriteLogs(ex.ToString());
                }
                #endregion

                #region 获取、计算棒图长度

                double tdouBar1Value = CalTree.GetFinishedTagValue("F3002");// Convert.ToDouble(CalDataTable.Rows[0]["F3002"]);
                if (tdouBar1Value != GlobalVariables.ReplaceValue)
                {
                    douBar1Value = tdouBar1Value;
                }

                douBar1Value = System.Math.Round(douBar1Value, 2);
                this.labelSub1.Text = douBar1Value.ToString();
                this.label7.Text = douBar1Value.ToString();
                if (douBar1Value < 145)
                {
                    if (douBar1Value < 10)
                    {
                        this.labelSub1.Location = new Point(68 + Convert.ToInt32(10 * 4), Convert.ToInt32(110 * this.Height / 900));   //new Point(68 + Convert.ToInt32(douBar1Value * 4), 120);
                    }
                    else
                    {
                        this.labelSub1.Location = new Point(Convert.ToInt32((68 + Convert.ToInt32(douBar1Value * 4)) * this.Width / 1440), Convert.ToInt32(110 * this.Height / 900));
                    }
                }
                else
                {
                    this.labelSub1.Location = new Point(Convert.ToInt32(670 * this.Width / 1440), Convert.ToInt32(132 * this.Height / 900));
                }
                #endregion
                #region //设置棒图长度

                //绑定棒状图上的Label
                PicChartLabelBind();

                CColor = "green";
                //第一条棒图
                if (douBar1Value <= 100)
                {
                    this.labelSub1.ForeColor = Color.Green;

                    this.picGreen1.Visible = true;
                    this.picGreen1.BackColor = Color.Green;
                    if (douBar1Value < 10)//如果低于10，则按照10的长度显示
                    {
                        this.picGreen1.Width = Convert.ToInt32(10 * 4 * this.Width / 1440);
                    }
                    else
                    {
                        this.picGreen1.Width = Convert.ToInt32(douBar1Value * 4 * this.Width / 1440);
                    }

                    this.picRed1.Visible = false;
                    this.picYellow1.Visible = false;
                    this.picOrange1.Visible = false;
                }
                else if ((douBar1Value > 100) && (douBar1Value <= 112.5))
                {
                    this.labelSub1.ForeColor = Color.Yellow;

                    this.picGreen1.Visible = true;
                    this.picGreen1.BackColor = Color.Yellow;
                    this.picGreen1.Width = (int)400 * this.Width / 1440; // Convert.ToInt32(douData * 10) - 1000;

                    this.picYellow1.Visible = true;
                    this.picYellow1.BackColor = Color.Yellow;
                    this.picYellow1.Width = (int)(Convert.ToInt32(douBar1Value * 4) - 400) * this.Width / 1440;

                    this.picOrange1.Visible = false;
                    this.picRed1.Visible = false;
                }
                else if ((douBar1Value > 112.5) && (douBar1Value <= 125))
                {
                    this.labelSub1.ForeColor = Color.Orange;

                    this.picGreen1.Visible = true;
                    this.picGreen1.BackColor = Color.Orange;
                    this.picGreen1.Width = (int)400 * this.Width / 1440; // Convert.ToInt32(douData * 10) - 1000;

                    this.picYellow1.Visible = true;
                    this.picYellow1.BackColor = Color.Orange;
                    this.picYellow1.Width = (int)50 * this.Width / 1440;

                    this.picOrange1.Visible = true;
                    this.picOrange1.BackColor = Color.Orange;
                    this.picOrange1.Width = (int)(Convert.ToInt32(douBar1Value * 4) - 450) * this.Width / 1440;

                    this.picRed1.Visible = false;
                }
                else if (douBar1Value > 125)
                {
                    this.labelSub1.ForeColor = Color.Red;

                    this.picGreen1.Visible = true;
                    this.picGreen1.BackColor = Color.Red;
                    this.picGreen1.Width = Convert.ToInt32(400 * this.Width / 1440); // Convert.ToInt32(douData * 10) - 1000;

                    this.picYellow1.Visible = true; // Convert.ToInt32(douData * 10) - 1000;
                    this.picYellow1.BackColor = Color.Red;
                    this.picYellow1.Width = Convert.ToInt32(50 * this.Width / 1440);

                    this.picOrange1.Visible = true;
                    this.picOrange1.BackColor = Color.Red;
                    this.picOrange1.Width = Convert.ToInt32(50 * this.Width / 1440);

                    this.picRed1.Visible = true;
                    int ii = Convert.ToInt32((douBar1Value * 4 - 500) * this.Width / 1440);
                    if (ii > 25 * 4)
                        this.picRed1.Width = Convert.ToInt32(25 * 4 * this.Width / 1440);
                    else
                        this.picRed1.Width = Convert.ToInt32(ii * this.Width / 1440);
                }

                Application.DoEvents();
                #region 棒状图报警检查

                //报警检查
                PicChartChecking(douBar1Value);


                #endregion

                #endregion
                //Freshisfinished = true;

                Application.DoEvents();
                if ((DateTime.Now - timeGCCollect).TotalHours > 2.3)
                {
                    timeGCCollect = DateTime.Now;
                    GC.Collect();
                }
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
        }

        //查看历史按钮单击事件
        private void btnHis1_Click(object sender, EventArgs e)
        {
            SelectTagDate selectTagDate = new SelectTagDate("FCZ");
            HistoryLines historyLines = new HistoryLines();
            selectTagDate.ShowDialog();
            if (selectTagDate.bIsClose == false)
            {
                if (selectTagDate.sExcelCells.Length > 0)
                {
                    historyLines.SetBeginTime(selectTagDate.BeginTime);
                    historyLines.SetEndTime(selectTagDate.EndTime);
                    historyLines.SetExcelCells(selectTagDate.sExcelCells);
                    historyLines.SetCheckedTags(selectTagDate.dtCheckedTags);
                }
                else
                {
                    historyLines.seriesCollection = (chart1).Series;
                }
                historyLines.ShowDialog();
            }
            selectTagDate.Dispose();
            historyLines.Dispose();

            //try
            //{
            //    FrmShowHis frs = new FrmShowHis();
            //    frs.isfromZhu = false;
            //    frs.ShowDialog();
            //    frs.Dispose();
            //}
            //catch (Exception E)
            //{
            //    WriteLog.WriteLogs("展示历史曲线出错：" + E.ToString());
            //    //string s = e.ToString();
            //}

        }

        //确定报警按钮事件
        //报警历史按钮
        private void btnAlarmConfirm_Click(object sender, EventArgs e)
        {
            ////this.picAlert.Visible = false;
            //this.timer1.Enabled = false;
            //this.btnAlarmConfirm.Enabled = false;
            KSPrj.Tool.AlarmList aList = new KSPrj.Tool.AlarmList("FCZ");
            aList.Show();
        }



        //定义委托 BY ZZH 
        public delegate void UpdateControls(DataTable dt);
        UpdateControls upd = null;
        private void Form1_Load(object sender, EventArgs e)
        {
            //主页面定时刷新
            //this.timer2.Interval = KSPrj.Cls.ConstYXYH.RefIntvel;
            this.timer2.Enabled = false;

            Notice.OnMessageRecieved += OnMessageReceived;
            upd = new UpdateControls(ChangeLabels);
            isFirstRun = false;

        }

        public void OnMessageReceived(DataTable calDataTable, DataTable dtYXYH)
        {
            this.BeginInvoke(upd, calDataTable);
        }



        //测点报警按钮事件
        private void btnTags_Click(object sender, EventArgs e)
        {
            //ChartTemperature ct = new ChartTemperature();
            //ct.ShowDialog();
            //ct.Dispose();

            FrmTagCheck frm1 = new FrmTagCheck();
            frm1.Show();
        }
        //屏蔽alt+f4
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4 && e.Modifiers == Keys.Alt)
                e.Handled = true;
        }
        //打开测点替代值设置
        private void btnOrgTD_Click(object sender, EventArgs e)
        {
            //if (PWDCheck("rg"))
            //{
            //这里用运行优化的坏点替代 BY ZZH 
            TagReplace tr = new TagReplace();
            tr.Show();
            //} 
        }
        //运行和停止运行按钮
        private void button2_Click(object sender, EventArgs e)
        {
            if (PWDCheck("rg"))
            {
                if (this.button2.Text.Replace(" ", "").Trim() == "运行")
                {
                    Notice.OnMessageRecieved += OnMessageReceived;
                    //this.timer2.Enabled = true;
                    this.button2.Text = "停  止";
                }
                else
                {
                    Notice.OnMessageRecieved -= OnMessageReceived;
                    //this.timer2.Enabled = false;
                    this.button2.Text = "运  行";
                }
            }
        }
        //曲线
        /// <summary>
        /// Chart1绑定
        /// </summary>
        private void dataBindChart()
        {
            if (isminReturn == true)
            {
                //dtBind = null;
                //dtBind = FrmShowHis.QueryDBData(DateTime.Now.AddMinutes(-10), DateTime.Now, rightBottId);
                //string iiii = rightBottId;
            }

            try
            {
                string ss = chart1.Series[0].Name;
                if (GlobalVariables.UnitNumber == 1)
                {
                    if (ss == "F2128")
                    {
                        ss = "F1128";
                        chart1.Series[0].YValueMembers = ss;
                        chart1.Series[0].Name = ss;
                    }
                }
                else
                {
                    if (ss == "F1128")
                    {
                        ss = "F2128";
                        chart1.Series[0].YValueMembers = ss;
                        chart1.Series[0].Name = ss;
                    }
                }
            }
            catch { }


            chart1.DataSource = GlobalVariables.dtChartDatas;
            chart1.DataBind();
            //chart1.Refresh();
        }



        //时间刷新函数，每秒一次
        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                this.label59.Text = string.Format("{0:F}", DateTime.Now);// System.DateTime.Now.ToLongTimeString();//+ "OPC取数用时："  + " 000 秒"
                if (PIRead.IsOperatingPI && (DateTime.Now - PIRead.TimeOfBeginOperatingPI).TotalSeconds > 4)
                    this.label59.Text = DateTime.Now.ToLongDateString() + "  读PI已" + (DateTime.Now - PIRead.TimeOfBeginOperatingPI).TotalSeconds.ToString("0.0") + "秒！";
                //报警检查 
                AlarmHint();
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
        }

        private void chart1_GetToolTipText(object sender,
                                           System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs e)
        {
            if (e.HitTestResult.ChartElementType == ChartElementType.DataPoint)
            {
                int i = e.HitTestResult.PointIndex;
                DataPoint dp = e.HitTestResult.Series.Points[i];
                //分别显示x轴和y轴的数值，其中{1:F3},表示显示的是float类型，精确到小数点后3位。
                e.Text = string.Format("时间:{0};数值:{1:F3} ", dp.AxisLabel.ToString(), dp.YValues[0]);
                //e.Text = string.Format("{0:F3}", dp.YValues[0]);
            }
        }

        //重置变量
        //private void reset()
        //{
        //    mouseDownPoint = Point.Empty;
        //    rect = Rectangle.Empty;
        //    isDrag = false;
        //}

        #region 添加测点按钮

        private void btnAddtTag_Click(object sender, EventArgs e)
        {
            int iUnitNo = int.Parse(((Button)sender).Tag.ToString());
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

            //control = new Label();
            //control.Name = "L" + DateTime.Now.ToString("yyyyMMddHHmmssfff"); //设置label的唯一标示，用创建时间表示
            //control.Text = DateTime.Now.ToString("New Label");
            //control.Location = getPointToForm(new Point(20, 30));
            //control.BackColor = System.Drawing.Color.Transparent;//设置背景色透明
            //control.MouseDoubleClick += new MouseEventHandler(myLabel_Click); //双击事件，选择测点。
            //control.MouseDown += new MouseEventHandler(control_MouseDown); //移动时的鼠标按下事件
            //control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
            //control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
            //control.AutoSize = true;///////////////////////
            //SQLHelper.ExecuteSql("insert into formConfig (labelguid,labelX,labelY,parentcontrol,labelText,locationId,locationname) values ('" + control.Name + "'," + control.Location.X + "," + control.Location.Y + ",'" +
            //    "groupBox2" + "','" + control.Text + "',1,'左下系统图')");
            //this.groupBox2.Controls.Add(control);
            //control.BringToFront();
        }

        //左上添加测点 BY ZZH 
        private void btnAddcTag_Click(object sender, EventArgs e)
        {
            int iUnitNo = int.Parse(((Button)sender).Tag.ToString());
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
            this.groupBox1.Controls.Add(control);
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

            //control = new Label();
            //control.Name = "L" + DateTime.Now.ToString("yyyyMMddHHmmssfff"); //设置label的唯一标示，用创建时间表示
            //control.Text = DateTime.Now.ToString("New Label");
            //control.Location = getPointToForm(new Point(110, 90));
            //control.BackColor = System.Drawing.Color.Transparent;//设置背景色透明
            //control.MouseDoubleClick += new MouseEventHandler(myLabel_Click); //双击事件，选择测点。
            //control.MouseDown += new MouseEventHandler(control_MouseDown); //移动时的鼠标按下事件
            //control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
            //control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
            //SQLHelper.ExecuteSql("insert into formConfig (labelguid,labelX,labelY,parentcontrol,labelText,locationId,locationname) values ('" + control.Name + "'," + control.Location.X + "," + control.Location.Y + ",'" +
            //    "groupBox1" + "','" + control.Text + "',2,'右下系统图')");
            //this.groupBox1.Controls.Add(control);
            //control.BringToFront();
        }

        #endregion

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)  //判断是否最小化
            {
                this.ShowInTaskbar = true;  //不显示在系统任务栏
                notifyIcon1.Visible = true;  //托盘图标可见
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = true;  //显示在系统任务栏
                this.WindowState = FormWindowState.Normal;  //还原窗体
                notifyIcon1.Visible = false;  //托盘图标隐藏
            }
        }
        //最小化事件
        private void button6_Click(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Minimized;
            /*this.Hide();
            notifyIcon1.Visible = true;  //托盘图标可见

            ProcessStartInfo psi = new ProcessStartInfo(@"YHYXYH.exe"); 
            Process p = new Process(); 
            p.StartInfo = psi; 
            p.Start();  */
            Program.yxyh.Show();

            this.Hide();

        }

        //private Opaquelayer myOpaquelayer = new Opaquelayer();

        #region 图表双击事件

        private void chart1_DoubleClick(object sender, EventArgs e)
        {
            FrmShowHis myfrmshouHis = new FrmShowHis();
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

        private void chart2_DoubleClick(object sender, EventArgs e)
        {
            ChartParabola ct2 = new ChartParabola();
            ct2.ShowDialog();
            ct2.Dispose();
        }

        private void chart3_DoubleClick(object sender, EventArgs e)
        {
            ChartBrokenline ct3 = new ChartBrokenline();
            ct3.ShowDialog();
            ct3.Dispose();
        }

        #endregion

        private void btnMinisize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        #region 左下角图表部分 BY ZZH
        // 图1 
        private void button7_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedIndex = 0;
            this.button7.ForeColor = Color.Red;
            this.button7.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button8.ForeColor = Color.Lime;
            this.button8.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.ForeColor = Color.Lime;
            this.button4.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        // 图2 
        private void button8_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedIndex = 1;
            this.button8.ForeColor = Color.Red;
            this.button8.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button7.ForeColor = Color.Lime;
            this.button7.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button4.ForeColor = Color.Lime;
            this.button4.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }
        // 图3 
        private void button4_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedIndex = 2;
            this.button4.ForeColor = Color.Red;
            this.button4.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button7.ForeColor = Color.Lime;
            this.button7.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button8.ForeColor = Color.Lime;
            this.button8.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        /// <summary>
        /// 初始化左下角Chart图表
        /// </summary>
        private void InitCharts()
        {
            this.tabPage1.BackColor = Color.Black;
            this.tabPage2.BackColor = Color.Black;
            this.tabPage3.BackColor = Color.Black;
            this.tabControl1.Appearance = TabAppearance.FlatButtons;

            Font fontStyle = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

            //隐藏选项卡标题
            this.tabControl1.Region = new Region(new RectangleF(this.tabPage1.Left, this.tabPage1.Top, this.tabPage1.Width, this.tabPage1.Height));

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
            ct2.SetSeries("", new double[] { 0.4479, 1.0 }, new double[] { 1.4, 1.4 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Green, 0, "");

            //五条竖线 从左至右
            ct2.SetSeries("", new double[] { 0.05, 0.05 }, new double[] { 0, 1.4 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct2.SetSeries("", new double[] { 0.1, 0.1 }, new double[] { 0, 1.4 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct2.SetSeries("", new double[] { 0.27, 0.27 }, new double[] { 0, 1.4 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct2.SetSeries("", new double[] { 0.3, 0.3 }, new double[] { 0, 1.4 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            //末级叶片零功率
            ct2.SetSeries("mjyplgl", new double[] { 0.4479, 0.4479 }, new double[] { 0, 1.4 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            this.chart2.Series["mjyplgl"].Points[1].Label = "末级叶片零功率";

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
            //series2.LegendText = "123123";
            //for (int i = 0; i < chart2.Legends.Count; i++ ) 
            //{
            //    chart2.Legends[i].Enabled = false;
            //}
            //chart2.Legends[chart2.Legends.Count-1].Enabled = true;



            //Chart3 绘折线图 
            ChartTools ct3 = new ChartTools(this.chart3);
            this.chart3.BackColor = Color.Black;
            ct3.SetChartArea(Color.Transparent);

            ct3.SetAxisX(0, 0.7, 0.1, "低压缸进汽压力（绝对）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            ct3.SetAxisY(0, 70, 10, "低压缸排汽压力（绝对）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            ct3.SetAxisY2(24.5, 94.5, 10, "真空（KPa）", StringAlignment.Near, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Dash, AxisArrowStyle.None, true, "");

            this.chart3.ChartAreas[0].AxisX.TitleFont = fontStyle;
            this.chart3.ChartAreas[0].AxisY.TitleFont = fontStyle;
            this.chart3.ChartAreas[0].AxisY2.TitleFont = fontStyle;

            //ct3.SetAxisX2(120, 190, 5, "低压缸排气流量（绝对）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.Triangle, "");

            //阴影
            ct3.SetSeries("", new double[] { 0, 0.7 }, new double[] { 70, 70 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Red, 0, "");
            ct3.SetSeries("", new double[] { 0, 0.1361, 0.4295, 0.7 }, new double[] { 25, 25, 48, 48 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.DarkOrange, 0, "");
            ct3.SetSeries("", new double[] { 0, 0.1361, 0.4295, 0.7 }, new double[] { 20, 20, 43, 43 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Green, 0, "");

            //两条直线 红色 粉色     跳机点：#VALY" + "KP
            ct3.SetSeries("tjd", new double[] { 0, 0.7 }, new double[] { 65, 65 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct3.SetSeries("", new double[] { 0, 0.7 }, new double[] { 33, 33 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Yellow, 2, "");
            //this.chart3.Series["tjd"].Points[0].Label = "#VALY" + "KP                 ";

            //两条红色折线 
            ct3.SetSeries("", new double[] { 0, 0.1361, 0.4295, 0.7 }, new double[] { 20, 20, 43, 43 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct3.SetSeries("", new double[] { 0, 0.1361, 0.4295, 0.7 }, new double[] { 25, 25, 48, 48 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");

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
        }

        /// <summary>
        /// Chart1数据绑定/凝结水量（仅用在初始化的时候绑定） BY ZZH 
        /// </summary>
        private void Chart1Bind()
        {
            if (GlobalVariables.dtChartDatas.Rows.Count > 0)
            {
                this.chart1.DataSource = GlobalVariables.dtChartDatas;
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
            if (GlobalVariables.UnitNumber == 1)
            {
                this.label36.Text = CalTree.GetShowValue(1323, "m³/s", "");
                this.label38.Text = CalTree.GetShowValue(1320, "m/s", "");
                this.label40.Text = CalTree.GetShowValue(1329, "", ""); 
            }
            else
            {
                this.label36.Text = CalTree.GetShowValue(2323, "m³/s", "");
                this.label38.Text = CalTree.GetShowValue(2320, "m³/s", "");
                this.label40.Text = CalTree.GetShowValue(2329, "", ""); 
            }
        }

        /// <summary>
        /// 抛物线图表 绑定
        /// </summary>
        private void Chart2Bind()
        {
            //if (CalDataTable == null || CalDataTable.Rows.Count < 1)
            //    return;
            double xValue = 0.33;
            if (GlobalVariables.UnitNumber == 1)
            {
                xValue = CalTree.GetFinishedTagValue("F1329"); // double.Parse(CalDataTable.Rows[0]["F1329"].ToString()); //F1329
                if (xValue == GlobalVariables.BadValue)
                    xValue = CalTree.GetSetValue("1329", "SetValue");
            }
            else
            {
                xValue = CalTree.GetFinishedTagValue("F2329"); //double.Parse(CalDataTable.Rows[0]["F2329"].ToString()); //F2329
                if (xValue == GlobalVariables.BadValue)
                    xValue = CalTree.GetSetValue("2329", "SetValue");
            }


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
            double pointX = 0.55;
            double pointY = 25;
            string dygpqll = "--"; //低压缸排气流量1324/2324
            double dqy = 0.1; //大气压 
            string dygjqyl = "--"; //低压缸进汽压力1111/2111 
            //string zk = "--"; //真空 
            if (GlobalVariables.UnitNumber == 1)
            {
                try
                {
                    //这里添加#1机组的XY坐标  
                    pointX = CalTree.GetFinishedTagValue("F1169");// double.Parse(CalDataTable.Rows[0]["F1169"].ToString()); 
                    if (pointX == GlobalVariables.BadValue)
                    {
                        pointX = CalTree.GetSetValue("1169", "SetValue");
                    }
                    dygpqll = CalTree.GetShowValue(1324, "t/h", ""); // CalDataTable.Rows[0]["F1324"].ToString();
                    dygjqyl = CalTree.GetShowValue(1111, "MPa", "");
                }
                catch { }
            }
            else
            {
                try
                {
                    //这里添加#2机组的XY坐标
                    pointX = CalTree.GetFinishedTagValue("F2169");//double.Parse(CalDataTable.Rows[0]["F2169"].ToString()); 
                    if (pointX == GlobalVariables.BadValue)
                    {
                        pointX = CalTree.GetSetValue("1169", "SetValue");
                    }
                    dygpqll = CalTree.GetShowValue(2324, "t/h", ""); //CalDataTable.Rows[0]["F2324"].ToString(); 
                    dygjqyl = CalTree.GetShowValue(2111, "MPa", "");
                }
                catch { }
            }
            try
            {
                pointY = CalTree.GetFinishedTagValue("F3126") * 1000;//double.Parse(CalDataTable.Rows[0]["F3126"].ToString()) * 1000;
                dqy = CalTree.GetFinishedTagValue("F3004") * 1000;//double.Parse(CalDataTable.Rows[0]["F3004"].ToString()) * 1000;
            }
            catch { }

            //判断XY轴的值

            if (pointY == GlobalVariables.BadValue * 1000)
            {
                pointY = CalTree.GetSetValue("3126", "SetValue") * 1000;
            }
            if (dqy == GlobalVariables.BadValue * 1000)
            {
                dqy = CalTree.GetSetValue("3004", "SetValue") * 1000;
            }

            //绑定低压缸排气流量和相对容积流量
            this.label27.Text = dygpqll;
            this.label28.Text = pointY + " KPa";
            this.label3.Text = dygjqyl;
            //真空 F3004*1000 - F3126*1000
            this.label43.Text = Math.Round(dqy - pointY, 2) + " KPa";
            try
            {
                //Y2轴 真空绑定
                double max = dqy - 0;
                double min = dqy - 70;
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

        #region 控件加载与绑定

        string sNewLabel = "";
        DataTable dtControls = TableControlConfig.getDataTableByRowFilter("[Parent] like 'Form1%'");
        TagLabelBinding tagLabelBinding = new TagLabelBinding();

        /// <summary>
        /// 加载程序运行中配置的控件
        /// </summary>
        void AddConfigedControl()
        {
            try
            {
                int i;
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
                            ((PictureBox)control).BackgroundImage = global::KSPrj.Properties.Resources.开关绿;
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
        /// <summary>
        /// 设定绑定标签的Text（测点值+测点单位）。
        /// </summary>
        void SetBindingLabelsText()
        {
            try
            {
                Control control, parent;
                LabelTag labelTag = null;
                DataTable dtTagLabelBinding = TableTagLabelBinding.getDataTableByRowFilter("[Parent] like 'Form1%'");
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
                            control.Text = CalTree.GetShowValue(int.Parse(row["TagID"].ToString()), labelTag.TagUnit, row["adjustValue"].ToString());
                        }
                        catch { }
                        control.Refresh();

                        //string tagValue = CalDataTable.Rows[0]["F" + row["TagID"]].ToString();
                        //if(string.IsNullOrEmpty(labelTag.TagUnit))
                        //    control.Text = tagValue;
                        //else 
                        //{
                        //if (string.IsNullOrEmpty(tagValue))
                        //    control.Text = "--";
                        //else 
                        //{
                        //    double dTagValue = double.Parse(tagValue);
                        //防颤振页面数据过滤（裕华） BY ZZH 

                        ////屏蔽掉主页上显示负数的数据 
                        //if (dTagValue < 0)
                        //{
                        //    control.Text = "--";
                        //}
                        //else
                        //{
                        //    //主页面上以MPad为单位的测点要求4位有效数字
                        //    if ("MPA".Equals(labelTag.TagUnit.ToUpper()))
                        //    {
                        //        dTagValue = Math.Round(dTagValue, 4);
                        //    }
                        //    int rowIntID = int.Parse(row["TagID"].ToString());
                        //    if (rowIntID == 1164)//高压缸效率限制范围40-90
                        //    {
                        //        if (dTagValue > 90 || dTagValue < 40)
                        //        {
                        //            control.Text = "--";
                        //        }
                        //    }
                        //    else if (rowIntID == 1167)//中压缸效率限制范围70-95
                        //    {
                        //        if (dTagValue > 95 || dTagValue < 70)
                        //        {
                        //            control.Text = "--";
                        //        }
                        //    }
                        //    else if (rowIntID == 1178)//低压缸效率限制范围30-95
                        //    {
                        //        if (dTagValue > 95 || dTagValue < 30)
                        //        {
                        //            control.Text = "--";
                        //        }
                        //    }
                        //    else if (rowIntID == 1077 || rowIntID == 1087)
                        //    {
                        //        if (dTagValue > 95 || dTagValue < 40)
                        //        {
                        //            control.Text = "--";
                        //        }
                        //    }
                        //    else if (rowIntID == 1186)//发电煤耗
                        //    {
                        //        if (dTagValue < 100 || dTagValue > 290) 
                        //        {
                        //            control.Text = "--";
                        //        } 
                        //    } 
                        //}
                        //control.Text = tagValue + " " + labelTag.TagUnit;
                        //}

                        //}


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
        /// <summary>
        /// 设置Label的可见性
        /// </summary>
        public void SetLabelVisible(string sRowFilter = "[Parent] like 'Form1%'")
        {
            Control control, parent;
            LabelTag labelTag = null;
            int i = 0;
            int iHideUnitNo = 2;//要隐藏的机组编号
            string sTagID = "";
            string[] sHideIDs;//要隐藏的测点
            if (rdoHCGD.Checked == true)
                sHideIDs = sUnit1HCGR_IDs.Split(',');
            else
                sHideIDs = sUnit1HCGD_IDs.Split(',');
            if (GlobalVariables.UnitNumber == 2)
            {
                iHideUnitNo = 1;
                if (rdoHCGD.Checked == true)
                    sHideIDs = sUnit2HCGR_IDs.Split(',');
                else
                    sHideIDs = sUnit2HCGD_IDs.Split(',');
            }
            dtControls = TableControlConfig.getDataTableByRowFilter(sRowFilter);
            foreach (DataRow row in dtControls.Rows)
            {
                try
                {
                    parent = TagLabelBinding.GetParentControl(this, row["Parent"].ToString());
                    //由父控件找到本Label
                    control = parent.Controls.Find(row["ControlName"].ToString(), false)[0];
                    labelTag = (LabelTag)control.Tag;
                    if (labelTag.UnitNo == iHideUnitNo)
                        control.Visible = false;
                    else
                    {
                        control.Visible = true;
                        sTagID = row["TagID"].ToString();
                        for (i = 0; i < sHideIDs.Length; i++)
                        {
                            if (sTagID == sHideIDs[i])
                            {
                                control.Visible = false;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLog.WriteLogs(ex.ToString());
                }
            }
            dtControls.Dispose();
            dtControls = null;

            systemGraph.SetLabelVisible(rdoHCGD.Checked);
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

            if (e.Button == MouseButtons.Left)
            {
                if (isDrag)
                {
                    isDrag = false;
                    sSql = "update ControlConfig set ControlX=" + control.Location.X + ",ControlY=" + control.Location.Y
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


        private void contextMenuStrip_Label_del_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除此测点吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
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

        private void contextMenuStrip_Label_DataBinding_Click(object sender, EventArgs e)
        {
            tagLabelBinding.label = (Label)this.control;
            tagLabelBinding.ShowDialog();
        }

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

        #endregion

        #region 报警

        AlarmReport chart1Alarm = new AlarmReport("排汽压力限制报警");
        int chart1AlarmIsShow = 0;
        AlarmReport chart2Alarm = new AlarmReport("相对动应力显示报警");
        int chart2AlarmIsShow = 0;
        AlarmReport chart3Alarm = new AlarmReport("背压限制报警");
        int chart3AlarmIsShow = 0;
        AlarmReport KLDAlarm = new AlarmReport("空冷岛防冻阀检查报警");
        int KLDAlarmIsShow = 0;

        AlarmReport CQYLAlarm = new AlarmReport("抽汽压力报警");
        int cqylAlarmIsShow = 0;
        AlarmReport ZYGCYAlarm = new AlarmReport("中压缸压差报警");
        int zygcyAlarmIsShow = 0;
        AlarmReport ZYGPQWDAlarm = new AlarmReport("中压缸排气温度报警");
        int zygpqwdAlarmIsShow = 0;
        AlarmReport DYGPQWDAlarm = new AlarmReport("低压缸排汽温度报警");
        int dygpqwdAlarmIsShow = 0;
        AlarmReport GRCQLAlarm = new AlarmReport("抽汽限制报警");
        int grcqlAlarmIsShow = 0;

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
        /// 空冷岛防冻阀检查报警
        /// 环境温度 -10 -- 0    最冷点低于3度 开启第一列
        /// 环境温度 -20 -- -10  最冷点低于3度 开启第二列
        /// 环境温度 -20以下     最冷点低于3度 开启第三列
        /// 阀门状态 0关/1开
        /// </summary>
        private void KLDCheck()
        {
            if (isFirstRun)
                return;

            double tempreture = CalTree.GetFinishedTagValue("F3208");
            double zld = 0;
            double zld2 = 0;
            double zld3 = 0;
            if (GlobalVariables.UnitNumber == 1)
            {
                zld = CalTree.GetFinishedTagValue("F1400");
                zld2 = CalTree.GetFinishedTagValue("F1412");
                zld3 = CalTree.GetFinishedTagValue("F1456");
            }
            else
            {
                zld = CalTree.GetFinishedTagValue("F2400");
                zld2 = CalTree.GetFinishedTagValue("F2412");
                zld3 = CalTree.GetFinishedTagValue("F2456");
            }


            if (tempreture == GlobalVariables.BadValue || zld == GlobalVariables.BadValue || zld2 == GlobalVariables.BadValue || zld3 == GlobalVariables.BadValue)
                return;

            if ((tempreture >= -10 && tempreture <= 0) && (zld < 3 || zld2 < 3 || zld3 < 3))
            {
                int isOpen1 = 1;
                int isOpen2 = 1;
                if (GlobalVariables.UnitNumber == 1)
                {
                    isOpen1 = (int)CalTree.GetFinishedTagValue("F1401");
                    isOpen2 = (int)CalTree.GetFinishedTagValue("F1404");
                }
                else
                {
                    isOpen1 = (int)CalTree.GetFinishedTagValue("F2401");
                    isOpen2 = (int)CalTree.GetFinishedTagValue("F2404");
                }
                string str = "";
                if (isOpen1 == 0)
                    str = "A1";
                if (isOpen2 == 0)
                    str += "A2";
                if (string.IsNullOrEmpty(str))
                {
                    //阀门已经打开
                    KLDboolAlarm = false;
                }
                else
                {
                    //阀门未打开，报警
                    if (KLDAlarmIsShow == 0)
                    {
                        KLDAlarm.SetText("空冷岛防冻阀非常冷，请开启防冻阀第一列", "", AlarmType.AlarmWhite, 500, true, 1);
                        KLDAlarm.Show();
                        KLDAlarmIsShow = 1;
                    }
                    else
                    {
                        //if (KLDAlarm.Visible == false)
                        KLDAlarm.SetText("空冷岛防冻阀非常冷，请开启防冻阀第一列", "", AlarmType.AlarmWhite, 500, true, 1);
                        //else
                        if (KLDAlarm.Visible == true)
                            KLDAlarm.Activate();
                    }
                    this.label5.Text = "空冷岛防冻阀非常冷，请开启防冻阀第一列";
                    KLDboolAlarm = true;
                }
            }
            else if ((tempreture >= -20 && tempreture < -10) && (zld < 3 || zld2 < 3 || zld3 < 3))
            {
                int isOpen1 = 1;
                int isOpen2 = 1;
                if (GlobalVariables.UnitNumber == 1)
                {
                    isOpen1 = (int)CalTree.GetFinishedTagValue("F1402");
                    isOpen2 = (int)CalTree.GetFinishedTagValue("F1405");
                }
                else
                {
                    isOpen1 = (int)CalTree.GetFinishedTagValue("F2402");
                    isOpen2 = (int)CalTree.GetFinishedTagValue("F2405");
                }
                string str = "";
                if (isOpen1 == 0)
                    str = "A1";
                if (isOpen2 == 0)
                    str += "A2";
                if (string.IsNullOrEmpty(str))
                {
                    //阀门已经打开
                    KLDboolAlarm = false;
                }
                else
                {
                    //阀门未打开，报警
                    if (KLDAlarmIsShow == 0)
                    {
                        KLDAlarm.SetText("空冷岛防冻阀非常冷，请开启防冻阀第二列", "", AlarmType.AlarmWhite, 500, true, 2);
                        KLDAlarm.Show();
                        KLDAlarmIsShow = 1;
                    }
                    else
                    {
                        //if (KLDAlarm.Visible == false)
                        KLDAlarm.SetText("空冷岛防冻阀非常冷，请开启防冻阀第二列", "", AlarmType.AlarmWhite, 500, true, 2);
                        //else
                        if (KLDAlarm.Visible == true)
                            KLDAlarm.Activate();
                    }
                    this.label5.Text = "空冷岛防冻阀非常冷，请开启防冻阀第二列";
                    KLDboolAlarm = true;
                }
            }
            else if (tempreture < -20 && (zld < 3 || zld2 < 3 || zld3 < 3))
            {
                int isOpen1 = 1;
                int isOpen2 = 1;
                if (GlobalVariables.UnitNumber == 1)
                {
                    isOpen1 = (int)CalTree.GetFinishedTagValue("F1403");
                    isOpen2 = (int)CalTree.GetFinishedTagValue("F1406");
                }
                else
                {
                    isOpen1 = (int)CalTree.GetFinishedTagValue("F2403");
                    isOpen2 = (int)CalTree.GetFinishedTagValue("F2406");
                }
                string str = "";
                if (isOpen1 == 0)
                    str = "A1";
                if (isOpen2 == 0)
                    str += "A2";
                if (string.IsNullOrEmpty(str))
                {
                    //阀门已经打开
                    KLDboolAlarm = false;
                }
                else
                {
                    //阀门未打开，报警
                    if (KLDAlarmIsShow == 0)
                    {
                        KLDAlarm.SetText("空冷岛防冻阀非常冷，请开启防冻阀第三列", "", AlarmType.AlarmWhite, 500, true, 3);
                        KLDAlarm.Show();
                        KLDAlarmIsShow = 1;
                    }
                    else
                    {
                        //if (KLDAlarm.Visible == false)
                        KLDAlarm.SetText("空冷岛防冻阀非常冷，请开启防冻阀第三列", "", AlarmType.AlarmWhite, 500, true, 3);
                        //else
                        if (KLDAlarm.Visible == true)
                            KLDAlarm.Activate();
                    }
                    this.label5.Text = "空冷岛防冻阀非常冷，请开启防冻阀第三列";
                    KLDboolAlarm = true;
                }
            }
            else
            {
                KLDAlarm.SetText("", "", AlarmType.AlarmSafe, 500, true, 0);
                KLDboolAlarm = false;
            }
        }

        /// <summary>
        /// 抽汽压力报警
        /// F1098/F2098  5抽压力
        /// 低于0.45报警（绝对）0.43二次报警
        /// 高于0.67（绝对）MPa,报警
        /// 全部白色报警，在页面上显示黄（0.45）  橙（0.67）  红（0.43）
        /// </summary>
        private void CQYLCheck()
        {
            double press = 0;

            if (GlobalVariables.UnitNumber == 1)
                press = CalTree.GetFinishedTagValue("F1098");
            else
                press = CalTree.GetFinishedTagValue("F2098");

            if (press == GlobalVariables.BadValue)
                return;

            if (press < 0.45 && press >= 0.43)
            {
                if (cqylAlarmIsShow == 0)
                {
                    CQYLAlarm.SetText("抽汽压力报警", "抽汽压力（绝对）小于0.45MPa并且大于0.43MPa", AlarmType.AlarmWhite, 500, true, 1);
                    CQYLAlarm.Show();
                    cqylAlarmIsShow = 1;
                }
                else
                {
                    CQYLAlarm.SetText("抽汽压力报警", "抽汽压力（绝对）小于0.45MPa并且大于0.43MPa", AlarmType.AlarmWhite, 500, true, 1);
                    if (CQYLAlarm.Visible == true)
                        CQYLAlarm.Activate();
                }
                CQYLboolAlarm = true;
                CQYLColor = Color.Yellow;
            }
            else if (press < 0.43)
            {
                if (cqylAlarmIsShow == 0)
                {
                    CQYLAlarm.SetText("抽汽压力报警", "抽汽压力（绝对）小于0.43MPa", AlarmType.AlarmWhite, 500, true, 2);
                    CQYLAlarm.Show();
                    cqylAlarmIsShow = 1;
                }
                else
                {
                    CQYLAlarm.SetText("抽汽压力报警", "抽汽压力（绝对）小于0.43MPa", AlarmType.AlarmWhite, 500, true, 2);
                    if (CQYLAlarm.Visible == true)
                        CQYLAlarm.Activate();
                }
                CQYLboolAlarm = true;
                CQYLColor = Color.Red;
            }
            else if (press > 0.67)
            {
                if (cqylAlarmIsShow == 0)
                {
                    CQYLAlarm.SetText("抽汽压力报警", "抽汽压力（绝对）大于0.67MPa", AlarmType.AlarmWhite, 500, true, 3);
                    CQYLAlarm.Show();
                    cqylAlarmIsShow = 1;
                }
                else
                {
                    CQYLAlarm.SetText("抽汽压力报警", "抽汽压力（绝对）大于0.67MPa", AlarmType.AlarmWhite, 500, true, 3);
                    if (CQYLAlarm.Visible == true)
                        CQYLAlarm.Activate();
                }
                CQYLboolAlarm = true;
                CQYLColor = Color.DarkOrange;
            }
            else
            {
                CQYLAlarm.SetText("", "", AlarmType.AlarmSafe, 500, true, 0);
                CQYLboolAlarm = false;
            }
        }

        /// <summary>
        /// 中压缸压差报警
        /// F1021, F2021(热再热压力)-F1165, F2165(中压缸排气压力)
        /// 前后压差不得大于3.2MPa
        /// 请减少抽气量 
        /// F1409,F2409 中压缸压差值
        /// </summary>
        private void ZYGCYCheck()
        {
            double cy = 0;
            if (GlobalVariables.UnitNumber == 1)
            {
                cy = CalTree.GetFinishedTagValue("F1409");
                if (cy == GlobalVariables.BadValue)
                    return;
            }
            else
            {
                cy = CalTree.GetFinishedTagValue("F2409");
                if (cy == GlobalVariables.BadValue)
                    return;
            }

            if (cy > 3.2)
            {
                if (zygcyAlarmIsShow == 0)
                {
                    ZYGCYAlarm.SetText("中压缸压差报警", "前后压差不得大于3.2MPa，请减少抽气量", AlarmType.AlarmWhite, 500, true, 1);
                    ZYGCYAlarm.Show();
                    zygcyAlarmIsShow = 1;
                }
                else
                {
                    ZYGCYAlarm.SetText("中压缸压差报警", "前后压差不得大于3.2MPa，请减少抽气量", AlarmType.AlarmWhite, 500, true, 1);
                    if (ZYGCYAlarm.Visible == true)
                        ZYGCYAlarm.Activate();
                }
                ZYGYCboolAlarm = true;
            }
            else
            {
                ZYGCYAlarm.SetText("", "", AlarmType.AlarmSafe, 500, true, 0);
                ZYGYCboolAlarm = false;
            }
        }

        /// <summary>
        /// 中压缸排气温度报警
        /// 温度限制不得大于388度
        /// 提示：降低再热温度
        /// F1407,F2407
        /// </summary>
        private void ZYGPQWDCheck()
        {
            double tempreture = 0;
            if (GlobalVariables.UnitNumber == 1)
            {
                tempreture = CalTree.GetFinishedTagValue("F1407");
                if (tempreture == GlobalVariables.BadValue)
                    return;
            }
            else
            {
                tempreture = CalTree.GetFinishedTagValue("F2407");
                if (tempreture == GlobalVariables.BadValue)
                    return;
            }

            if (tempreture > 388)
            {
                if (zygpqwdAlarmIsShow == 0)
                {
                    ZYGPQWDAlarm.SetText("中压缸排气温度报警", "中压缸排气温度大于388度，请降低再热温度", AlarmType.AlarmWhite, 500, true, 1);
                    ZYGPQWDAlarm.Show();
                    zygpqwdAlarmIsShow = 1;
                }
                else
                {
                    ZYGPQWDAlarm.SetText("中压缸排气温度报警", "中压缸排气温度大于388度，请降低再热温度", AlarmType.AlarmWhite, 500, true, 1);
                    if (ZYGPQWDAlarm.Visible == true)
                        ZYGPQWDAlarm.Activate();
                }
                ZYGPQWDboolAlarm = true;
            }
            else
            {
                ZYGPQWDAlarm.SetText("", "", AlarmType.AlarmSafe, 500, true, 0);
                ZYGPQWDboolAlarm = false;
            }


        }

        /// <summary>
        /// 低压缸排气温度报警
        /// 93度报警，92度喷水。（93度报警喷水）
        /// F1218, F2218
        /// </summary>
        private void DYGPQWDCheck()
        {
            double tempreture = 0;
            if (GlobalVariables.UnitNumber == 1)
            {
                tempreture = CalTree.GetFinishedTagValue("F1218");
                if (tempreture == GlobalVariables.BadValue)
                    return;
            }
            else
            {
                tempreture = CalTree.GetFinishedTagValue("F2218");
                if (tempreture == GlobalVariables.BadValue)
                    return;
            }

            if (tempreture >= 93)
            {
                if (dygpqwdAlarmIsShow == 0)
                {
                    DYGPQWDAlarm.SetText("低压缸排气温度报警", "低压缸排气温度过高，请及时喷水", AlarmType.AlarmWhite, 500, true, 1);
                    DYGPQWDAlarm.Show();
                    dygpqwdAlarmIsShow = 1;
                }
                else
                {
                    DYGPQWDAlarm.SetText("低压缸排气温度报警", "低压缸排气温度过高，请及时喷水", AlarmType.AlarmWhite, 500, true, 1);
                    if (DYGPQWDAlarm.Visible == true)
                        DYGPQWDAlarm.Activate();
                }
                DYGPQWDboolAlarm = true;
            }
            else
            {
                DYGPQWDAlarm.SetText("", "", AlarmType.AlarmSafe, 500, true, 0);
                DYGPQWDboolAlarm = false;
            }

        }

        /// <summary>
        /// 供热抽汽量报警
        ///  F1110, F2110 5抽供热抽汽量
        ///  超过370吨一次报警，最高限制550吨，二次报警420吨。
        /// </summary> 
        //private void GRCQLCheck2() 
        //{
        //    double cql = 0;

        //    if (GlobalVariables.UnitNumber == 1)
        //        cql = CalTree.GetFinishedTagValue("F1110");
        //    else
        //        cql = CalTree.GetFinishedTagValue("F2110");

        //    if (cql == GlobalVariables.BadValue)
        //        return;

        //    if(cql > 370 && cql < 420)
        //    {
        //        //一次报警
        //        if (grcqlAlarmIsShow == 0)
        //        {
        //            GRCQLAlarm.SetText("供热抽汽量报警", "供热抽汽量超过370吨，请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
        //            GRCQLAlarm.Show();
        //            grcqlAlarmIsShow = 1;
        //        }
        //        else
        //        {
        //            GRCQLAlarm.SetText("供热抽汽量报警", "供热抽汽量超过370吨，请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
        //            if (GRCQLAlarm.Visible == true)
        //                GRCQLAlarm.Activate();
        //        }
        //    }
        //    else if (cql >= 420 && cql < 550)
        //    {
        //        //二次报警
        //        if (grcqlAlarmIsShow == 0)
        //        {
        //            GRCQLAlarm.SetText("供热抽汽量报警", "供热抽汽量超过420吨，请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
        //            GRCQLAlarm.Show();
        //            grcqlAlarmIsShow = 1;
        //        }
        //        else
        //        {
        //            GRCQLAlarm.SetText("供热抽汽量报警", "供热抽汽量超过420吨，请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
        //            if (GRCQLAlarm.Visible == true)
        //                GRCQLAlarm.Activate();
        //        }
        //    }
        //    else if (cql >= 550)
        //    {
        //        if (grcqlAlarmIsShow == 0)
        //        {
        //            GRCQLAlarm.SetText("供热抽汽量报警", "供热抽汽量超过550吨，请减少抽汽量", AlarmType.AlarmWhite, 500, true, 3);
        //            GRCQLAlarm.Show();
        //            grcqlAlarmIsShow = 1;
        //        }
        //        else
        //        {
        //            GRCQLAlarm.SetText("供热抽汽量报警", "供热抽汽量超过550吨，请减少抽汽量", AlarmType.AlarmWhite, 500, true, 3);
        //            if (GRCQLAlarm.Visible == true)
        //                GRCQLAlarm.Activate();
        //        }
        //    }
        //}

        /// <summary>
        /// 抽汽限制报警
        /// 分六个阶段判断报警 
        /// </summary>
        private void GRCQLCheck()
        {
            double pointX = 0;
            double pointY = 0;

            if (GlobalVariables.UnitNumber == 1)
            {
                try
                {
                    pointX = CalTree.GetFinishedTagValue("F1324");
                    pointY = CalTree.GetFinishedTagValue("F1019");
                    if (pointX == GlobalVariables.BadValue || pointY == GlobalVariables.BadValue)
                        return;
                }
                catch { }
            }
            else
            {
                try
                {
                    pointX = CalTree.GetFinishedTagValue("F2324");
                    pointY = CalTree.GetFinishedTagValue("F2019");
                    if (pointX == GlobalVariables.BadValue || pointY == GlobalVariables.BadValue)
                        return;
                }
                catch { }
            }

            //分六个阶段进行报警判断，依次从右到左 
            if (pointX >= 430 && pointX < 510)
            {
                //判断橙色报警
                //上橙色线斜率（y=-1.39x+1428.8）
                //下黄线斜率（y=1.625x+108.75）
                //只判断两者之间橙色报警
                double up = -1.39 * pointX + 1428.8;
                double down = 1.625 * pointX + 108.75;
                if (pointY > down && pointY < up)
                {
                    //橙色报警
                    if (grcqlAlarmIsShow == 0)
                    {
                        GRCQLAlarm.SetText("抽汽限制橙色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
                        GRCQLAlarm.Show();
                        grcqlAlarmIsShow = 1;
                    }
                    else
                    {
                        GRCQLAlarm.SetText("抽汽限制橙色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
                        if (GRCQLAlarm.Visible == true)
                            GRCQLAlarm.Activate();
                    }
                    CQXZboolAlarm = true;
                    CQXZColor = Color.DarkOrange;
                }
                else
                    CQXZboolAlarm = false;
            }
            else if (pointX >= 330 && pointX < 430)
            {
                //橙色与红色报警
                //上橙色线斜率（y=-1.39x+1428.8）
                //下红线斜率（y=-2.022x+1459.4）
                //两者之间橙色报警，红线以下红色报警 
                double up = -1.39 * pointX + 1428.8;
                double down = -2.022 * pointX + 1459.4;
                if (pointY >= down && pointY < up)
                {
                    //橙色报警
                    if (grcqlAlarmIsShow == 0)
                    {
                        GRCQLAlarm.SetText("抽汽限制橙色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
                        GRCQLAlarm.Show();
                        grcqlAlarmIsShow = 1;
                    }
                    else
                    {
                        GRCQLAlarm.SetText("抽汽限制橙色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
                        if (GRCQLAlarm.Visible == true)
                            GRCQLAlarm.Activate();
                    }
                    CQXZboolAlarm = true;
                    CQXZColor = Color.DarkOrange;
                }
                else if (pointY < down)
                {
                    //红色报警
                    if (grcqlAlarmIsShow == 0)
                    {
                        GRCQLAlarm.SetText("抽汽限制红色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
                        GRCQLAlarm.Show();
                        grcqlAlarmIsShow = 1;
                    }
                    else
                    {
                        GRCQLAlarm.SetText("抽汽限制红色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
                        if (GRCQLAlarm.Visible == true)
                            GRCQLAlarm.Activate();
                    }
                    CQXZboolAlarm = true;
                    CQXZColor = Color.Red;
                }
                else
                    CQXZboolAlarm = false;
            }
            else if (pointX >= 270 && pointX < 330)
            {
                //橙色与红色报警
                //上黄线Y值是1170
                //下红线斜率是（y=-2.022x+1459.4）
                //两者之间橙色报警，低于红线红色报警
                double down = -2.022 * pointX + 1459.4;
                if (pointY >= down && pointY <= 1170)
                {
                    //橙色报警
                    if (grcqlAlarmIsShow == 0)
                    {
                        GRCQLAlarm.SetText("抽汽限制橙色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
                        GRCQLAlarm.Show();
                        grcqlAlarmIsShow = 1;
                    }
                    else
                    {
                        GRCQLAlarm.SetText("抽汽限制橙色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
                        if (GRCQLAlarm.Visible == true)
                            GRCQLAlarm.Activate();
                    }
                    CQXZboolAlarm = true;
                    CQXZColor = Color.DarkOrange;
                }
                else if (pointY < down)
                {
                    //红色报警 
                    if (grcqlAlarmIsShow == 0)
                    {
                        GRCQLAlarm.SetText("抽汽限制红色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
                        GRCQLAlarm.Show();
                        grcqlAlarmIsShow = 1;
                    }
                    else
                    {
                        GRCQLAlarm.SetText("抽汽限制红色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
                        if (GRCQLAlarm.Visible == true)
                            GRCQLAlarm.Activate();
                    }
                    CQXZboolAlarm = true;
                    CQXZColor = Color.Red;
                }
                else
                    CQXZboolAlarm = false;
            }
            else if (pointX >= 247 && pointX < 270)
            {
                //橙色与红色报警
                //上黄线斜率（y=1.8x+684）
                //下红线斜率（y=-2.022x+1459.4）
                //两者之间橙色报警，大于黄线或小于红线 红色报警
                double up = 1.8 * pointX + 684;
                double down = -2.022 * pointX + 1459.4;
                if (pointY >= down && pointY <= up)
                {
                    //橙色报警
                    if (grcqlAlarmIsShow == 0)
                    {
                        GRCQLAlarm.SetText("抽汽限制橙色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
                        GRCQLAlarm.Show();
                        grcqlAlarmIsShow = 1;
                    }
                    else
                    {
                        GRCQLAlarm.SetText("抽汽限制橙色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
                        if (GRCQLAlarm.Visible == true)
                            GRCQLAlarm.Activate();
                    }
                    CQXZboolAlarm = true;
                    CQXZColor = Color.DarkOrange;
                }
                else if (pointY < down || pointY > up)
                {
                    //红色报警
                    if (grcqlAlarmIsShow == 0)
                    {
                        GRCQLAlarm.SetText("抽汽限制红色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
                        GRCQLAlarm.Show();
                        grcqlAlarmIsShow = 1;
                    }
                    else
                    {
                        GRCQLAlarm.SetText("抽汽限制红色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
                        if (GRCQLAlarm.Visible == true)
                            GRCQLAlarm.Activate();
                    }
                    CQXZboolAlarm = true;
                    CQXZColor = Color.Red;
                }
                else
                    CQXZboolAlarm = false;
            }
            else if (pointX >= 220 && pointX < 247)
            {
                //橙色与红色报警
                //上黄线斜率（y=1.8x+684）
                //下红线斜率（y=-4.4x+2048）
                //两者之间橙色报警，大于或小于红色报警
                double up = 1.8 * pointX + 684;
                double down = -4.4 * pointX + 2048;
                if (pointY >= down && pointY <= up)
                {
                    //橙色报警
                    if (grcqlAlarmIsShow == 0)
                    {
                        GRCQLAlarm.SetText("抽汽限制橙色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
                        GRCQLAlarm.Show();
                        grcqlAlarmIsShow = 1;
                    }
                    else
                    {
                        GRCQLAlarm.SetText("抽汽限制橙色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 1);
                        if (GRCQLAlarm.Visible == true)
                            GRCQLAlarm.Activate();
                    }
                    CQXZboolAlarm = true;
                    CQXZColor = Color.DarkOrange;
                }
                else if (pointY < down || pointY > up)
                {
                    //红色报警
                    if (grcqlAlarmIsShow == 0)
                    {
                        GRCQLAlarm.SetText("抽汽限制红色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
                        GRCQLAlarm.Show();
                        grcqlAlarmIsShow = 1;
                    }
                    else
                    {
                        GRCQLAlarm.SetText("抽汽限制红色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
                        if (GRCQLAlarm.Visible == true)
                            GRCQLAlarm.Activate();
                    }
                    CQXZboolAlarm = true;
                    CQXZColor = Color.Red;
                }
                else
                    CQXZboolAlarm = false;
            }
            else if (pointX < 220)
            {
                //红色报警
                if (grcqlAlarmIsShow == 0)
                {
                    GRCQLAlarm.SetText("抽汽限制红色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
                    GRCQLAlarm.Show();
                    grcqlAlarmIsShow = 1;
                }
                else
                {
                    GRCQLAlarm.SetText("抽汽限制红色报警", "请减少抽汽量", AlarmType.AlarmWhite, 500, true, 2);
                    if (GRCQLAlarm.Visible == true)
                        GRCQLAlarm.Activate();
                }
                CQXZboolAlarm = true;
                CQXZColor = Color.Red;
            }
            else
            {
                GRCQLAlarm.SetText("", "", AlarmType.AlarmSafe, 500, true, 0);
                CQXZboolAlarm = false;
            }
        }

        #endregion

        #region 机组切换
        /// <summary>
        /// 1#机组单选按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoUnit1_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoUnit1.Checked == true)
            {
                if (GlobalVariables.UnitNumber == 1)
                    return;
                if (PWDCheck("yx"))
                {
                    GlobalVariables.UnitNumber = 1;
                    SetLabelVisible();
                    try
                    {
                        Program.yxyh.SetLabelVisible();
                    }
                    catch { }
                    try
                    {
                        SQLHelper.ExecuteSql("update Config set ConfigValue=1 where ConfigKey='CurrentUnitNO'");
                    }
                    catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }

                    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "([id]<2000 or [id]>3000) and datasourcesNo=1";
                    dtCurrentUnitTag = GlobalVariables.dtTagsSet.DefaultView.ToTable();
                    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "";
                }
                else
                {
                    rdoUnit1.Checked = false;
                    rdoUnit2.Checked = true;
                }
                //为固定的Label绑定双击事件 
                LabelBindDC();
            }
        }

        /// <summary>
        /// 2#机组单选按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoUnit2_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoUnit2.Checked == true)
            {
                if (GlobalVariables.UnitNumber == 2)
                    return;
                if (PWDCheck("yx"))
                {
                    GlobalVariables.UnitNumber = 2;
                    SetLabelVisible();
                    try
                    {
                        Program.yxyh.SetLabelVisible();
                    }
                    catch { }
                    try
                    {
                        SQLHelper.ExecuteSql("update Config set ConfigValue=2 where ConfigKey='CurrentUnitNO'");
                    }
                    catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }

                    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "[id]>2000 and datasourcesNo=1";
                    dtCurrentUnitTag = GlobalVariables.dtTagsSet.DefaultView.ToTable();
                    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "";
                }
                else
                {
                    rdoUnit2.Checked = false;
                    rdoUnit1.Checked = true;
                }
                //为固定的Label绑定双击事件 
                LabelBindDC();
            }
        }

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

        #endregion


        //窗体大小改变触发事件
        private void Control_Resize(object sender, EventArgs e)
        {
            Rectangle rect;
            LabelTag labelTag = null;
            //this.Visible = false;
            Control parentControl = sender as Control;
            parentControl.Visible = false;
            foreach (Control control in parentControl.Controls)
            {
                try
                {
                    labelTag = (LabelTag)(control.Tag);
                    rect = new Rectangle();
                    rect.X = Convert.ToInt32(1.0 * labelTag.ControlX / labelTag.ParentWidth * parentControl.Width);
                    rect.Y = Convert.ToInt32(1.0 * labelTag.ControlY / labelTag.ParentHeight * parentControl.Height);
                    control.Bounds = rect;

                    //空冷岛阀门状态，这里必须再次写上大小 BY ZZH 
                    if (control.GetType() == typeof(PictureBox))
                    {
                        control.Size = new System.Drawing.Size((int)(12.0 / labelTag.ParentWidth * parentControl.Width), (int)(18.0 / labelTag.ParentHeight * parentControl.Height));
                    }
                }
                catch { }
            }
            parentControl.Visible = true;
        }

        private void groupBox2_DoubleClick(object sender, EventArgs e)
        {
            systemGraph.ShowDialog();
        }

        //更多按钮 
        private void button3_Click(object sender, EventArgs e)
        {
            MoreTags mt = new MoreTags();
            mt.ShowDialog();
            mt.Dispose();
        }

        #region 页面上所有 Lebel 闪烁报警

        bool btnTagBool = false;

        public bool picChart = false; //棒状图是否报警
        public bool brokenChart = false; //折线图是否报警
        public bool parabolaChart = false; //抛物线是否报警
        bool KLDboolAlarm = false; //空冷岛报警 
        bool CQYLboolAlarm = false; //抽汽压力报警
        bool ZYGYCboolAlarm = false; //中压缸压差报警
        bool ZYGPQWDboolAlarm = false; //中压缸排气温度报警 
        bool DYGPQWDboolAlarm = false; //低压缸排气温度报警 
        public bool CQXZboolAlarm = false; //抽汽限制报警
        public Color picColor;
        public Color brokenColor;
        public Color parabolaColor;
        Color CQYLColor;
        public Color CQXZColor;

        Label LabelCQYL;
        Label LabelZYGYC;
        Label LabelZYGPQWD;
        Label LabelDYGPQWD;
        //Label LabelGRCQL;
        /// <summary>
        /// 报警提示（闪烁）
        /// </summary>
        private void AlarmHint()
        {
            if (btnTagBool)
            {
                if (this.btnTags.ForeColor == Color.Red)
                {
                    this.btnTags.ForeColor = Color.White;
                }
                else
                { this.btnTags.ForeColor = Color.Red; }
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

            //抛物线图
            if (this.chart2.Series["chart2Series"].Color != Color.White)
            {
                this.chart2.Series["chart2Series"].Color = Color.White;
            }
            else
            {
                this.chart2.Series["chart2Series"].Color = Color.Black;
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
                if (this.tabControl1.SelectedIndex != 0)
                    this.button7.ForeColor = Color.Lime;
                else
                    this.button7.ForeColor = Color.Red;
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
                if (this.tabControl1.SelectedIndex != 2)
                    this.button4.ForeColor = Color.Lime;
                else
                    this.button4.ForeColor = Color.Red;
            }

            //抛物线
            if (parabolaChart)
            {
                if (this.button8.ForeColor == Color.DarkBlue)
                    this.button8.ForeColor = parabolaColor;
                else
                    this.button8.ForeColor = Color.DarkBlue;
            }
            else
            {
                if (this.tabControl1.SelectedIndex != 1)
                    this.button8.ForeColor = Color.Lime;
                else
                    this.button8.ForeColor = Color.Red;
            }

            //空冷岛
            if (KLDboolAlarm)
            {
                this.label5.Visible = true;
                if (this.label5.ForeColor == Color.DarkBlue)
                    this.label5.ForeColor = Color.DarkOrange;
                else
                    this.label5.ForeColor = Color.DarkBlue;
            }
            else
                this.label5.Visible = false;

            ////抽汽压力 5抽压力
            //if (CQYLboolAlarm)
            //{
            //    if (LabelCQYL.ForeColor == Color.DodgerBlue)
            //        LabelCQYL.ForeColor = CQYLColor;
            //    else
            //        LabelCQYL.ForeColor = Color.DodgerBlue;
            //}
            //else
            //    LabelCQYL.ForeColor = Color.DodgerBlue;

            ////中压缸压差报警
            //if (ZYGYCboolAlarm)
            //{
            //    if (LabelZYGYC.ForeColor == Color.DodgerBlue)
            //        LabelZYGYC.ForeColor = Color.DarkOrange;
            //    else
            //        LabelZYGYC.ForeColor = Color.DodgerBlue;
            //}
            //else
            //    LabelZYGYC.ForeColor = Color.DodgerBlue;

            ////中压缸排气温度报警 
            //if (ZYGPQWDboolAlarm)
            //{
            //    if (LabelZYGPQWD.ForeColor == Color.DodgerBlue)
            //        LabelZYGPQWD.ForeColor = Color.DarkOrange;
            //    else
            //        LabelZYGPQWD.ForeColor = Color.DodgerBlue;
            //}
            //else
            //    LabelZYGPQWD.ForeColor = Color.DodgerBlue;

            ////低压缸排气温度报警 
            //if (DYGPQWDboolAlarm)
            //{
            //    if (LabelDYGPQWD.ForeColor == Color.DodgerBlue)
            //        LabelDYGPQWD.ForeColor = Color.DarkOrange;
            //    else
            //        LabelDYGPQWD.ForeColor = Color.DodgerBlue;
            //}
            //else
            //    LabelDYGPQWD.ForeColor = Color.DodgerBlue;

            //抽汽限制 
            if (CQXZboolAlarm)
            {
                if (this.button1.ForeColor == Color.White)
                    this.button1.ForeColor = CQXZColor;
                else
                    this.button1.ForeColor = Color.White;
            }
            else
                this.button1.ForeColor = Color.White;
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
        private void KLDStateCheck()
        {
            if (GlobalVariables.UnitNumber == 1)
            {
                int A1 = (int)CalTree.GetFinishedTagValue("F1401");
                int B1 = (int)CalTree.GetFinishedTagValue("F1402");
                int C1 = (int)CalTree.GetFinishedTagValue("F1403");
                int A2 = (int)CalTree.GetFinishedTagValue("F1404");
                int B2 = (int)CalTree.GetFinishedTagValue("F1405");
                int C2 = (int)CalTree.GetFinishedTagValue("F1406");

                RedGreen("picBoxA11", A1);
                RedGreen("picBoxB11", B1);
                RedGreen("picBoxC11", C1);
                RedGreen("picBoxA21", A2);
                RedGreen("picBoxB21", B2);
                RedGreen("picBoxC21", C2);
            }
            else
            {
                int A1 = (int)CalTree.GetFinishedTagValue("F2401");
                int B1 = (int)CalTree.GetFinishedTagValue("F2402");
                int C1 = (int)CalTree.GetFinishedTagValue("F2403");
                int A2 = (int)CalTree.GetFinishedTagValue("F2404");
                int B2 = (int)CalTree.GetFinishedTagValue("F2405");
                int C2 = (int)CalTree.GetFinishedTagValue("F2406");

                RedGreen("picBoxA12", A1);
                RedGreen("picBoxB12", B1);
                RedGreen("picBoxC12", C1);
                RedGreen("picBoxA22", A2);
                RedGreen("picBoxB22", B2);
                RedGreen("picBoxC22", C2);
            }
        }
        private void RedGreen(string picName, int open)
        {
            PictureBox pic = null;
            try
            {
                pic = this.groupBox2.Controls.Find(picName, false)[0] as PictureBox;
                if (open == 1)
                    pic.BackgroundImage = global::KSPrj.Properties.Resources.开关;
                else
                    pic.BackgroundImage = global::KSPrj.Properties.Resources.开关绿;
            }
            catch { pic.BackgroundImage = global::KSPrj.Properties.Resources.开关绿; }
        }

        #endregion

        #region 好处归热/好处归电，切换按钮
        private void rdoHCGD_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoHCGD.Checked == true)
            {
                if (GlobalVariables.UnitNumber == 1)
                    SetLabelVisible("[Parent] like 'Form1%' and [TagID] in (" + sUnit1HCGD_IDs + "," + sUnit1HCGR_IDs + ")");
                else
                    SetLabelVisible("[Parent] like 'Form1%' and [TagID] in (" + sUnit2HCGD_IDs + "," + sUnit2HCGR_IDs + ")");
            }
        }

        private void rdoHCGR_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoHCGR.Checked == true)
            {
                if (GlobalVariables.UnitNumber == 1)
                    SetLabelVisible("[Parent] like 'Form1%' and [TagID] in (" + sUnit1HCGD_IDs + "," + sUnit1HCGR_IDs + ")");
                else
                    SetLabelVisible("[Parent] like 'Form1%' and [TagID] in (" + sUnit2HCGD_IDs + "," + sUnit2HCGR_IDs + ")");
            }
        }

        #endregion

        //抽汽限制图 
        private void button1_Click(object sender, EventArgs e)
        {
            ChartCQXZ cqxz = new ChartCQXZ();
            cqxz.Show();
        }

        private void btnAlarmTest_Click(object sender, EventArgs e)
        {
            //PWDChange pwd = new PWDChange();
            //pwd.ShowDialog();
            timerSound.Enabled = true;
        }

        static void timerSound_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerSound.Enabled = false;
            KSPrj.Alert.goSoundTest();
        }

        private void contextMenuStrip_Label_SetBigFont_Click(object sender, EventArgs e)
        {
            SQLHelper.ExecuteSql("update ControlConfig set FontSize=16 where ControlName='" + control.Name + "'");
            control.Font = new System.Drawing.Font("宋体", 16, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        }

        private void contextMenuStrip_Label_SetSmallFont_Click(object sender, EventArgs e)
        {
            SQLHelper.ExecuteSql("update ControlConfig set FontSize=NULL where ControlName='" + control.Name + "'");
            control.Font = Font;
        }

        private void label62_DoubleClick(object sender, System.EventArgs e)
        {
            ContactUs cu = new ContactUs();
            cu.ShowDialog();
            cu.Dispose();
        }

        private void label62_DoubleClick_1(object sender, EventArgs e)
        {
            ContactUs cu = new ContactUs();
            cu.ShowDialog();
            cu.Dispose();
        }

        private void LabelBindDC() 
        {
            if (GlobalVariables.UnitNumber == 1)
            {
                LabelBindDoubleClick(this.label36, 1323, "低压缸单侧排汽容积流量");
                LabelBindDoubleClick(this.label38, 1320, "低压缸排汽流速");
                LabelBindDoubleClick(this.label40, 1329, "相对容积流量比");
                LabelBindDoubleClick(this.label27, 1324, "当前低压缸排汽流量");
                LabelBindDoubleClick(this.label3, 1111, "当前低压缸进汽压力（MPa）");
            }
            else 
            {
                LabelBindDoubleClick(this.label36, 2323, "低压缸单侧排汽容积流量");
                LabelBindDoubleClick(this.label38, 2320, "低压缸排汽流速");
                LabelBindDoubleClick(this.label40, 2329, "相对容积流量比");
                LabelBindDoubleClick(this.label27, 2324, "当前低压缸排汽流量");
                LabelBindDoubleClick(this.label3, 2111, "当前低压缸进汽压力（MPa）");
            }
            LabelBindDoubleClick(this.label28, 3126, "当前低压缸排汽压力（MPa）");
            LabelBindDoubleClick(this.label7, 3002, "排汽压力限制");
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

    }
}
