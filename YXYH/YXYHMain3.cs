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
using YHYXYH.Tool;
using Microsoft.VisualBasic;
using System.Diagnostics;
using KSPrj;
using HAOCommon;
using HAOCommon.PI;
using KSPrj.YXYH;
using WinHost;

namespace YHYXYH.YXYH
{
    public partial class YXYHMain3 : Form
    {
        public YXYHMain3()
        {
            InitializeComponent();
             
            AddConfigedControl();
            SetMoveAdd();
            SetBindingLabelsText();
            SetLabelVisible();


            lblTemp.Tag = new LabelTag(false, 3208, "环境温度", "℃");
            lblTemp.DoubleClick += new EventHandler(Label_DoubleClick);//双击事件
            lblTemp.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
             

            timerLineBind.Enabled = false;

            //优化出水温度与实际出水温度
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "HH:mm";
            chart1.DataSource = m_dtLineData;
            //chart1.Series[0].LegendText = "优化出水温度";
            chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart1.Series[0].XValueMember = "ValueTime";
            chart1.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart1.Series[0].YValueMembers = "F" + chart1Series0Tag.TagID; //"F213";
            chart1.Series[0].Tag = chart1Series0Tag;
            //chart1.Series[1].LegendText = "实际出水温度";
            chart1.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart1.Series[1].XValueMember = "ValueTime";
            chart1.Series[1].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart1.Series[1].YValueMembers = "F" + chart1Series1Tag.TagID;//"F233";
            chart1.Series[1].Tag = chart1Series1Tag;
            chart1.ChartAreas[0].AxisY.Title = "出水温度(℃)";
            chart1.ChartAreas[0].AxisX.Title = "时间(m)";

            //优化功率与实际功率，**********这里实际显示的是“热负荷”
            chart3.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "HH:mm";
            chart3.DataSource = m_dtLineData;
            //chart1.Series[0].LegendText = "优化热负荷";
            chart3.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart3.Series[0].XValueMember = "ValueTime";
            chart3.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart3.Series[0].YValueMembers = "F" + chart3Series0Tag.TagID;//"F219";
            chart3.Series[0].Tag = chart3Series0Tag;
            //chart1.Series[1].LegendText = "实际热负荷";
            chart3.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart3.Series[1].XValueMember = "ValueTime";
            chart3.Series[1].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart3.Series[1].YValueMembers = "F" + chart3Series1Tag.TagID;//"F202";
            chart3.Series[1].Tag = chart3Series1Tag;
            chart3.ChartAreas[0].AxisY.Title = "热负荷(W/㎡)";
            chart3.ChartAreas[0].AxisX.Title = "时间(m)";

            //优化抽汽量和实际抽汽量
            chart2.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "HH:mm";
            chart2.DataSource = m_dtLineData;
            //chart1.Series[0].LegendText = "优化抽汽量";
            chart2.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart2.Series[0].XValueMember = "ValueTime";
            chart2.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart2.Series[0].YValueMembers = "F" + chart2Series0Tag.TagID;//"F222";
            chart2.Series[0].Tag = chart2Series0Tag;
            //chart1.Series[0].LegendText = "优化抽汽量";
            chart2.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart2.Series[1].XValueMember = "ValueTime";
            chart2.Series[1].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart2.Series[1].YValueMembers = "F" + chart2Series1Tag.TagID;//"F161";
            chart2.Series[1].Tag = chart2Series1Tag;
            chart2.ChartAreas[0].AxisY.Title = "循环水量(t/h)";
            chart2.ChartAreas[0].AxisX.Title = "时间(m)";


            timerLineBind.Interval = GlobalVariables.RefIntvel;
            timerLineBind.Enabled = true;

            try { ChangeLabels(GlobalVariables.dtOneRowDataFive); }
            catch { }
        }
        bool m_bIsFirstRun = true;
        int m_iTimerDateTimeCount = 0;
        int m_iLastUnitNo = -1;//保存上次绑定时机组的编号
        //string sUnit1HCGD_IDs = "1184,1186,1190,1262";//1号机组好处归电要显示的ID
        //string sUnit1HCGR_IDs = "1380,1381,3385,3399";//1号机组好处归热要显示的ID
        string sUnit2HCGD_IDs = "1184,1186,1190,1262";//2号机组好处归电要显示的ID
        string sUnit2HCGR_IDs = "1380,1381,3385,3399";//2号机组好处归热要显示的ID
        DateTime timeGCCollect = DateTime.Now;//系统垃圾回收时间
        DataTable dtControls = TableControlConfig.getDataTableByRowFilter("[Parent] like 'YXYHMain3.%'");
        DataTable m_dtLineData = GlobalVariables.dtChartDataFive;
        DataView viewTagLabelBinding = TableTagLabelBinding.getAllData().DefaultView;
        DataView viewShowList = null;
        HistoryLines m_frmHistoryLines = new HistoryLines(5);
        SystemGraph systemGraph = new SystemGraph();
        RunOptimizePrompt m_frmRunOptimizePrompt = new RunOptimizePrompt();
        TagLabelBinding tagLabelBinding = new TagLabelBinding(5);
        CalCSWDLL calCSWDLLobj = new CalCSWDLL();
        string sNewLabel = "";
        LabelTag chart1Series0Tag = new LabelTag(3410, "优化出水温度", "℃");//宋高工让改成了优化温度的上限值，原来是3213,edit by hlt ,2017-1-18
        LabelTag chart1Series1Tag = new LabelTag(3152, "实际出水温度", "℃"); //1233
        LabelTag chart3Series0Tag = new LabelTag(3219, "优化热负荷", "W/㎡"); //meiyou
        LabelTag chart3Series1Tag = new LabelTag(3252, "实际热负荷", "W/㎡"); //meiyou
        LabelTag chart2Series0Tag = new LabelTag(3222, "优化循环水量", "t/h"); //meiyou
        LabelTag chart2Series1Tag = new LabelTag(3505, "实际循环水量", "t/h");

        //定义委托 BY ZZH 
        public delegate void UpdateControls(DataTable dt);
        UpdateControls upd = null;
        private void YXYHMain3_Load(object sender, EventArgs e)
        {
            //dataGridView1.AutoGenerateColumns = false;
            ////dataGridView1.DataSource = CalTagValue.GetAllTagValues();
            ////dataGridView1.Refresh();

            timerLineBind.Enabled = false;

            ////优化出水温度与实际出水温度
            //chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "HH:mm";
            //chart1.DataSource = m_dtLineData;
            ////chart1.Series[0].LegendText = "优化出水温度";
            //chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            //chart1.Series[0].XValueMember = "ValueTime";
            //chart1.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            //chart1.Series[0].YValueMembers = "F" + chart1Series0Tag.TagID; //"F213";
            //chart1.Series[0].Tag = chart1Series0Tag;
            ////chart1.Series[1].LegendText = "实际出水温度";
            //chart1.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            //chart1.Series[1].XValueMember = "ValueTime";
            //chart1.Series[1].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            //chart1.Series[1].YValueMembers = "F" + chart1Series1Tag.TagID;//"F233";
            //chart1.Series[1].Tag = chart1Series1Tag;

            ////优化功率与实际功率，**********这里实际显示的是“热负荷”
            //chart3.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "HH:mm";
            //chart3.DataSource = m_dtLineData;
            ////chart1.Series[0].LegendText = "优化热负荷";
            //chart3.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            //chart3.Series[0].XValueMember = "ValueTime";
            //chart3.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            //chart3.Series[0].YValueMembers = "F" + chart3Series0Tag.TagID;//"F219";
            //chart3.Series[0].Tag = chart3Series0Tag;
            ////chart1.Series[1].LegendText = "实际热负荷";
            //chart3.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            //chart3.Series[1].XValueMember = "ValueTime";
            //chart3.Series[1].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            //chart3.Series[1].YValueMembers = "F" + chart3Series1Tag.TagID;//"F202";
            //chart3.Series[1].Tag = chart3Series1Tag;

            ////优化抽汽量和实际抽汽量
            //chart2.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "HH:mm";
            //chart2.DataSource = m_dtLineData;
            ////chart1.Series[0].LegendText = "优化抽汽量";
            //chart2.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            //chart2.Series[0].XValueMember = "ValueTime";
            //chart2.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            //chart2.Series[0].YValueMembers = "F" + chart2Series0Tag.TagID;//"F222";
            //chart2.Series[0].Tag = chart2Series0Tag;
            ////chart1.Series[0].LegendText = "优化抽汽量";
            //chart2.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            //chart2.Series[1].XValueMember = "ValueTime";
            //chart2.Series[1].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            //chart2.Series[1].YValueMembers = "F" + chart2Series1Tag.TagID;//"F161";
            //chart2.Series[1].Tag = chart2Series1Tag;


            //timerLineBind_Tick(null, null);

            //注册通知事件 BY ZZH 
            NoticeFive.OnMessageRecieved += OnMessageReceived;
            upd = new UpdateControls(ChangeLabels);
        }

        //通知事件 BY ZZH 
        /// <summary>
        /// 通知事件
        /// </summary>
        /// <param name="calDataTable"></param>
        public void OnMessageReceived(DataTable calDataTable)
        {
            this.BeginInvoke(upd, calDataTable);
        }

        #region 设置 控件是否可添加，是否可移动

        //1表示按钮可用，2表示按钮不可用 
        int btnVisible = 0;
        private void SetMoveAdd()
        {
            //设置是否可添加 
            if (GlobalVariables.IsCanAddLabel)
            {
                this.btnAdd1Tag.Visible = true; 
            }
            else
            {
                this.btnAdd1Tag.Visible = false; 
            }
            //设置是否可移动 
            if (GlobalVariables.IsCanMoveLabel)
            {
                if (btnVisible != 1)
                {
                    //设置控件是否可移动 
                    SetControlMouseEvent();
                    btnVisible = 1;
                }
                else
                    btnVisible = 1;
            }
            else
            {
                if (btnVisible != 2)
                {
                    //设置控件是否可移动 
                    SetControlMouseEvent();
                    btnVisible = 2;
                }
                else
                    btnVisible = 2;
            }
        }

        /// <summary>
        /// 设置控件是否可移动 
        /// </summary>
        private void SetControlMouseEvent()
        {
            for (int i = 0; i < this.pnlSystemGraph.Controls.Count; i++)
            {
                control = this.pnlSystemGraph.Controls[i];
                if (control.Tag != null)
                {
                    if (GlobalVariables.IsCanMoveLabel)
                    {
                        control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
                        control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
                        control.ContextMenuStrip = contextMenuStrip_Label;//添加快捷菜单
                    }
                    else
                    {
                        control.MouseMove -= control_MouseMove;
                        control.MouseUp -= control_MouseUp;
                        control.ContextMenuStrip = null;
                    }
                }
            } 
        }

        #endregion 

        /// <summary>
        /// 加载程序运行中配置的控件
        /// </summary>
        void AddConfigedControl()
        {
            try
            { 

                Control parent = null;
                Control control = null;
                Rectangle rect;
                LabelTag labelTag = null;
                foreach (DataRow row in dtControls.Rows)
                {

                    parent = TagLabelBinding.GetParentControl(this, row["Parent"].ToString().Replace("YXYHMain3.", ""));
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
                            control.Size = new System.Drawing.Size(22, 28);
                            control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
                            //if (GlobalVariables.IsCanMoveLabel)
                            //{
                            //    control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
                            //    control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
                            //    control.ContextMenuStrip = contextMenuStrip_Label;//添加快捷菜单
                            //}
                            if (parent.Controls.Contains(control))
                                parent.Controls.Remove(control);
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
                        control.DoubleClick += new EventHandler(Label_DoubleClick);//双击事件
                        control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
                        //if (GlobalVariables.IsCanMoveLabel)
                        //{
                        //    control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
                        //    control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
                        //    control.ContextMenuStrip = contextMenuStrip_Label;//添加快捷菜单
                        //}
                        if (parent.Controls.Contains(control))
                            parent.Controls.Remove(control);
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
                DataTable dtTagLabelBinding = TableTagLabelBinding.getDataTableByRowFilter("[Parent] like 'YXYHMain3.%'");
                foreach (DataRow row in dtTagLabelBinding.Rows)
                {
                    try
                    {
                        parent = TagLabelBinding.GetParentControl(this, row["Parent"].ToString());
                        //由父控件找到本Label
                        try { control = parent.Controls.Find(row["LabelName"].ToString(), false)[0]; }
                        catch { continue; }
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

                        control.Text = TagValue.GetShowValueFive(labelTag.TagID, labelTag.TagUnit, row["adjustValue"].ToString());
                        control.Refresh();
                    }
                    catch (Exception ex)
                    {
                        WriteLog.WriteLogs(row["TagID"].ToString() + "----" + ex.ToString());
                    }
                }
                lblRunOptimizePromptText.Text = RunOptimizePrompt.GetPromptText();
                lblRunOptimizePromptText.ForeColor = RunOptimizePrompt.GetPromptTextForeColor();
                dtTagLabelBinding.Dispose();
                dtTagLabelBinding = null;
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
        }
        /// <summary>
        /// 设置Label的可见性
        /// </summary>
        public void SetLabelVisible(string sRowFilter = "[Parent] like 'YXYHMain3.%'")
        {
            Control control, parent;
            LabelTag labelTag = null;
            int i = 0; 
            string sTagID = "";
            string[] sHideIDs;//要隐藏的测点 
            if (rdoHCGD.Checked == true)
                sHideIDs = sUnit2HCGR_IDs.Split(',');
            else
                sHideIDs = sUnit2HCGD_IDs.Split(','); 
            dtControls = TableControlConfig.getDataTableByRowFilter(sRowFilter);
            foreach (DataRow row in dtControls.Rows)
            {
                try
                {
                    parent = TagLabelBinding.GetParentControl(this, row["Parent"].ToString());
                    //由父控件找到本Label
                    try { control = parent.Controls.Find(row["ControlName"].ToString(), false)[0]; }
                    catch { continue; }
                    labelTag = (LabelTag)control.Tag; 
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
                catch (Exception ex)
                {
                    WriteLog.WriteLogs(ex.ToString());
                }
            }
            dtControls.Dispose();
            dtControls = null;

            systemGraph.SetLabelVisible(rdoHCGD.Checked);
        }
        void SetLabelsForeColor(Color color)
        {
            LabelTag labelTag = null;
            foreach (Control control in pnlSystemGraph.Controls)
            {
                try
                {
                    labelTag = (LabelTag)control.Tag;
                    control.ForeColor = color;
                    control.Refresh();
                }
                catch { }
            }
        }

        bool isDrag = false;
        string sSql = "";
        Point point = new Point();
        //保存鼠标按下时的坐标。这个坐标是相对于所点击的标签的坐标；在鼠标拖动时要减去这个坐标，要不然拖动时鼠标会一直处于标签的左上角
        Point pointMouseDown = new Point();
        Control control;
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
                    LabelTag labelTag = null; ;
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

        void SetChartSeriesLegendText(Chart chart)
        {
            LabelTag labelTag = null;
            foreach (Series series in chart.Series)
            {
                if (series.Enabled == true)
                {
                    labelTag = (LabelTag)series.Tag;
                    try { series.LegendText = labelTag.TagDesc + ":" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F" + labelTag.TagID].ToString()), 2) + labelTag.TagUnit; }
                    catch { WriteLog.WriteLogs(labelTag.TagDesc + labelTag.TagID); }
                }
            }
        }
        public void timerLineBind_Tick(object sender, EventArgs e)
        {
            ChangeLabels(null);
        }

        //存放通知数据的DataTable 
        DataTable dtData = new DataTable();
        /// <summary>
        /// 绑定页面数据
        /// </summary>
        /// <param name="dt"></param> 
        public void ChangeLabels(DataTable dt)
        {
            this.dtData = dt;
            try
            {
                SetMoveAdd();
                SetBindingLabelsText();
                //KLDStateCheck();
                //SetLabelsForeColor(Color.Red);
                //SetLabelsForeColor(Color.Lime);
                if (m_bIsFirstRun == false)
                    Application.DoEvents();
                lblDQY.Text = "大气压力：" + WinHost.TagValue.GetShowValueFive(3004, "", "");
                lblTemp.Text = "环境温度：" + WinHost.TagValue.GetFinishedTagValueFive("F3208").ToString("0.0") + "℃";
                m_dtLineData = GlobalVariables.dtChartDataFive;
                try
                {
                    chart1.DataSource = m_dtLineData;
                    SetChartSeriesLegendText(chart1);
                    chart1.DataBind();
                    if (m_bIsFirstRun == false)
                        Application.DoEvents();
                }
                catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
                try
                {
                    chart3.DataSource = m_dtLineData;
                    SetChartSeriesLegendText(chart3);
                    chart3.DataBind();
                    if (m_bIsFirstRun == false)
                        Application.DoEvents();
                }
                catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }

                try
                {
                    chart2.DataSource = m_dtLineData;
                    SetChartSeriesLegendText(chart2);
                    chart2.DataBind();
                    if (m_bIsFirstRun == false)
                        Application.DoEvents();
                }
                catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }

                //if (m_iLastUnitNo != GlobalVariables.UnitNumber)
                //{
                //    if (GlobalVariables.UnitNumber == 1)
                //        GlobalVariables.dtTagsSet.DefaultView.RowFilter = "([id]<2000 or [id]>3000) and isShow=1";
                //    else
                //        GlobalVariables.dtTagsSet.DefaultView.RowFilter = "[id]>2000 and isShow=1";
                //    if (viewShowList != null)
                //    {
                //        viewShowList.Table.Dispose();
                //        viewShowList = null;
                //    }
                //    viewShowList = GlobalVariables.dtTagsSet.DefaultView.ToTable().DefaultView;
                //    //GlobalVariables.dtTagsSet.DefaultView.RowFilter = "";
                //    viewShowList.Table.Columns.Add("TagValue2", typeof(string));
                //    foreach (DataRow row in viewShowList.Table.Rows)
                //        row["TagValue2"] = row["TagValue"].ToString();
                //    viewShowList.Table.Columns.Remove("TagValue");
                //    viewShowList.Table.Columns["TagValue2"].ColumnName = "TagValue";
                //    //dataGridView1.AutoGenerateColumns = false;
                //    //dataGridView1.DataSource = viewShowList;
                //    m_iLastUnitNo = GlobalVariables.UnitNumber;
                //}
                //else
                //    for (int i = 0; i < viewShowList.Count; i++)
                //    {
                //        dataGridView1.Rows[i].Cells["value"].Value = CalTree.GetShowValueNoUnit((int)viewShowList[i]["id"], viewShowList[i]["unit"].ToString(), viewShowList[i]["adjustValue"].ToString());
                //    }
                //dataGridView1.Refresh();

                //还未改造完，先不提示 2015-11-6
                if (m_bIsFirstRun == false)
                {
                    RunOptimizePrompt.CalculateRunOptimizePromptType(5);
                    if (m_frmRunOptimizePrompt.IsPromptAlarm())
                    {
                        if (bIsShow == false)
                        {
                            m_frmRunOptimizePrompt.Show();
                            bIsShow = true;
                        }
                        if (m_frmRunOptimizePrompt.Visible == false)
                            m_frmRunOptimizePrompt.Visible = true;
                        m_frmRunOptimizePrompt.Activate();
                    }
                    else
                        m_frmRunOptimizePrompt.Visible = false;
                }
                m_bIsFirstRun = false;

                if (m_bIsFirstRun == false)
                    Application.DoEvents();
                if ((DateTime.Now - timeGCCollect).TotalHours > 2.9)
                {
                    timeGCCollect = DateTime.Now;
                    GC.Collect();
                }
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
        }

        bool bIsShow = false;

        private void pnlSystemGraph_Resize(object sender, EventArgs e)
        {
            Rectangle rect;
            LabelTag labelTag = null;
            pnlSystemGraph.Visible = false;
            foreach (Control control in pnlSystemGraph.Controls)
            {
                try
                {
                    labelTag = (LabelTag)(control.Tag);
                    rect = new Rectangle();
                    rect.X = Convert.ToInt32(1.0 * labelTag.ControlX / labelTag.ParentWidth * pnlSystemGraph.Width);
                    rect.Y = Convert.ToInt32(1.0 * labelTag.ControlY / labelTag.ParentHeight * pnlSystemGraph.Height);
                    control.Bounds = rect;

                    //空冷岛阀门状态，这里必须再次写上大小 BY ZZH 
                    if (control.GetType() == typeof(PictureBox))
                    {
                        control.Size = new System.Drawing.Size((int)(22.0 / labelTag.ParentWidth * pnlSystemGraph.Width), (int)(28.0 / labelTag.ParentHeight * pnlSystemGraph.Height));
                    }
                }
                catch { }
            }
            pnlSystemGraph.Visible = true;
        }

        private void Label_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                LabelTag labelTag = (LabelTag)((Label)sender).Tag;
                if (labelTag.TagID > 0)
                {
                    HistoryLines m_frmHistoryLines = new HistoryLines(5);
                    //HistoryArea m_frmHistoryLines = new HistoryArea();
                    m_frmHistoryLines.labelTag.TagID = labelTag.TagID;
                    m_frmHistoryLines.labelTag.TagDesc = labelTag.TagDesc;
                    m_frmHistoryLines.labelTag.TagUnit = labelTag.TagUnit;
                    //m_frmHistoryLines.ShowDialog();
                    m_frmHistoryLines.Show();
                }
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
        }

        private void pnlLines_Resize(object sender, EventArgs e)
        {
            chart1.Width = pnlLines.Width / 3;
            chart2.Width = chart1.Width;
        }

        private void Chart_DoubleClick(object sender, EventArgs e)
        {
            HistoryLines historyLines = new HistoryLines(5);
            historyLines.seriesCollection = ((Chart)sender).Series;
            historyLines.ShowDialog();
            historyLines.Dispose();
        }

        private void pnlSystemGraph_DoubleClick(object sender, EventArgs e)
        {
            systemGraph.ShowDialog();
        }

        private void btnAlarmConfirm_Click(object sender, EventArgs e)
        {
            m_frmRunOptimizePrompt.Show();
            m_frmRunOptimizePrompt.Activate();
        }

        private void btnOrgTD_Click(object sender, EventArgs e)
        {
            //if (Program.frm1.PWDCheck("rg"))
            (new TagReplace(5)).Show();
        }

        private void btnHis_Click(object sender, EventArgs e)
        {
            //SelectTagDate selectTagDate = new SelectTagDate(5);
            //HistoryLines historyLines = new HistoryLines(5);
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
            //    historyLines.Show();
            //}
            //selectTagDate.Dispose();
            ////historyLines.Dispose();


            //SelectTagDate在生成对象调用时，由ShowDialog()改成了Show()；
            //因为用ShowDialog()时也会造成窗口无反应的假死现象，所以注释了上面的语句，加了下面的语句。add by hlt 2017-1-18
            SelectTagDate selectTagDate = new SelectTagDate(5,chart1);
            selectTagDate.Show();
        }

        private void btnFixedValueEdit_Click(object sender, EventArgs e)
        {
            (new FixedValueEdit(5)).Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Program.mainForm.Show();
            this.Visible = false;

            ////Close();
            ////Application.Exit();
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
            //    KSPrj.FrmInputPws fip = new KSPrj.FrmInputPws();
            //    fip.ShowDialog();
            //    string strRet = fip.Text;

            //    string[] strRetA = strRet.Split(',');
            //    if (strRet.Length == 0)
            //        return;
            //    if (strRetA[1] == "ok")
            //    {
            //        if (dtBasepwt.Rows[0]["pws"].ToString() == strRetA[0])
            //        {
            //            Program.frm1.Close();
            //            fip.Dispose();
            //            this.Close();
            //            this.Dispose();
            //            Application.Exit();
            //            WriteLog.WriteLogs("AppExit");
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

        private void button1_Click(object sender, EventArgs e)
        {
            (new GRLJSR(5)).Show();
        }

        private void contextMenuStrip_Label_del_Click(object sender, EventArgs e)
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

        private void btnAddTag_Click(object sender, EventArgs e)
        {
            //int iUnitNo = int.Parse(((Button)sender).Tag.ToString());
            control = new Label();
            control.Name = "L" + DateTime.Now.ToString("yyyyMMddHHmmssfff"); //设置label的唯一标示，用创建时间表示
            sNewLabel = control.Name;
            control.ForeColor = System.Drawing.Color.FromName("Lime");
            control.Text = DateTime.Now.ToString("New Label");
            control.AutoSize = true;
            control.Top = 100;
            control.Left = 100;
            control.BackColor = System.Drawing.Color.Black;//设置背景色透明
            control.DoubleClick += new EventHandler(Label_DoubleClick); //双击事件，选择测点。
            control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
            control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
            control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
            control.ContextMenuStrip = contextMenuStrip_Label;
            this.pnlSystemGraph.Controls.Add(control);
            LabelTag labelTag = new LabelTag();
            labelTag.ControlX = control.Left;
            labelTag.ControlY = control.Top;
            labelTag.ParentHeight = control.Parent.Height;
            labelTag.ParentWidth = control.Parent.Width;
            //labelTag.UnitNo = (byte)iUnitNo;
            control.Tag = labelTag;
            sSql = "insert into ControlConfig(ControlName,ControlX,ControlY,ForeColor,BackColor,Type,Parent,ParentHeight,ParentWidth,UnitNo) values('"
                + control.Name + "'," + control.Location.X + "," + control.Location.Y + "," + control.ForeColor.ToArgb() + ","
                + control.BackColor.ToArgb() + ",'" + control.GetType().Name + "','" + TagLabelBinding.GetParents(control) + "',"
                + control.Parent.Height + "," + control.Parent.Width + "," + 5 + ")";
            SQLHelper.ExecuteSql(sSql);
            TableControlConfig.setFill();
        }

        private void btnSSCST_Click(object sender, EventArgs e)
        {
            SSCS frm = new SSCS(5);
            frm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        //异常控制 
        private void btnGRSRJJLJ_Click(object sender, EventArgs e)
        {
            Program.yckz1.Show(); 
            this.Hide();
        }

        private void btnGRSRJTXS_Click(object sender, EventArgs e)
        {
            //CalTagValue.CalAllTagValue();
        }

        private void btnGDLJSR_Click(object sender, EventArgs e)
        {
            (new GDLJSR(5)).Show();
        }

        private void btnLJMLR_Click(object sender, EventArgs e)
        {
            (new LJMLR(5)).Show();
        }

        private void contextMenuStrip_Label_SaveLabel_Click(object sender, EventArgs e)
        {
            LabelTag labelTag = null;
            ArrayList lstSql = new ArrayList();
            Rectangle rect;
            foreach (Control control in pnlSystemGraph.Controls)
            {
                try
                {
                    labelTag = (LabelTag)(control.Tag);
                    rect = control.Bounds;
                    lstSql.Add("update ControlConfig set ControlX=" + rect.X + ",ControlY=" + rect.Y + ",ParentHeight=" + pnlSystemGraph.Height
                        + ",ParentWidth=" + pnlSystemGraph.Width + " where ControlName='" + control.Name + "'");
                }
                catch { }
            }
            SQLHelper.ExecuteSql(lstSql);
            MessageBox.Show("保存完成！");
        }

        private void btnResetTotal_Click(object sender, EventArgs e)
        {
            ResetTotalAvg reset = new ResetTotalAvg(5);
            reset.Show();

            return;

            if (MessageBox.Show("确定要复位累计值吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                ResetTotalPassword password = new ResetTotalPassword();
                password.ShowDialog();
                if (password.Password == GlobalVariables.Password)
                {
                    int i = TagLJValue.ResetTotalFive();
                    if (i > 0)
                        MessageBox.Show("复位累计值成功！");
                    else
                        MessageBox.Show("复位累计值失败！请重新复位");
                }
                else
                    MessageBox.Show("密码错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void contextMenuStrip_Label_SaveOneLabel_Click(object sender, EventArgs e)
        {
            string sSql = "insert into [ControlConfig](ControlName,ControlX,ControlY,Parent,Type,ForeColor,BackColor,ParentHeight,ParentWidth,ControlText) values(";
            string sSelect = "select * from [ControlConfig] where ControlName='" + control.Name + "'";
            DataTable dt = SQLHelper.ExecuteDt(sSelect);
            if (dt.Rows.Count > 0)
                sSql = "update ControlConfig set ControlX=" + control.Bounds.X + ",ControlY=" + control.Bounds.Y + ",ParentHeight="
                    + pnlSystemGraph.Height + ",ParentWidth=" + pnlSystemGraph.Width + " where ControlName='" + control.Name + "'";
            else
                sSql += "'" + control.Name + "'," + control.Bounds.X + "," + control.Bounds.Y + ",'" + control.Parent.Name + "','Label',"
                    + control.ForeColor.ToArgb() + "," + control.BackColor.ToArgb() + "," + control.Parent.Height + ","
                    + control.Parent.Width + ",'" + control.Text + "')";
            SQLHelper.ExecuteDt(sSql);
            MessageBox.Show("保存完成！");
        }


        private void pnlLeftButton_Resize(object sender, EventArgs e)
        {
            int iDistance = 10;
            //int iDistance = 1;
            int iHeight = (pnlLeftButton.Height - 15 * iDistance) / 14;
            btnStartStop.Top = iDistance;
            btnStartStop.Height = iHeight;
            btnChangeFCZ.Top = iDistance * 2 + iHeight;
            btnChangeFCZ.Height = iHeight;
            btnRLZCFY.Top = iDistance * 3 + iHeight * 2;
            btnRLZCFY.Height = iHeight;
            btnRealValue.Top = iDistance * 4 + iHeight * 3;
            btnRealValue.Height = iHeight;
            btnHis.Top = iDistance * 5 + iHeight * 4;
            btnHis.Height = iHeight;
            btnAlarmHistory.Top = iDistance * 6 + iHeight * 5;
            btnAlarmHistory.Height = iHeight;
            btnOrgTD.Top = iDistance * 7 + iHeight * 6;
            btnOrgTD.Height = iHeight;
            btnFixedValueEdit.Top = iDistance * 8 + iHeight * 7;
            btnFixedValueEdit.Height = iHeight;
            btnSZMX.Top = iDistance * 9 + iHeight * 8;
            btnSZMX.Height = iHeight;
            btnSSCST.Top = iDistance * 10 + iHeight * 9;
            btnSSCST.Height = iHeight;
            btnOutWaterT.Top = iDistance * 11 + iHeight * 10;
            btnOutWaterT.Height = iHeight;
            btnCalTempAndFlow.Top = iDistance * 12 + iHeight * 11;
            btnCalTempAndFlow.Height = iHeight;
            btnResetTotal.Top = iDistance * 13 + iHeight * 12;
            btnResetTotal.Height = iHeight;
            btnEditPassword.Top = iDistance * 14 + iHeight * 13;
            btnEditPassword.Height = iHeight;

            
            //btnGRLJSR.Top = iDistance * 5 + iHeight * 4;
            //btnGRLJSR.Height = iHeight;
            //btnGDLJSR.Top = iDistance * 6 + iHeight * 5;
            //btnGDLJSR.Height = iHeight;
            //btnLJMLR.Top = iDistance * 7 + iHeight * 6;
            //btnLJMLR.Height = iHeight;
            //btnLJJML.Top = iDistance * 8 + iHeight * 7;
            //btnLJJML.Height = iHeight;
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (this.btnStartStop.Text == "停  止")
            {
                if (Program.frm1.PWDCheck("rg"))
                {
                    NoticeFive.OnMessageRecieved -= OnMessageReceived;
                    //timerLineBind.Enabled = false;
                    btnStartStop.Text = "启  动";
                }
            }
            else
            {
                NoticeFive.OnMessageRecieved += OnMessageReceived;
                //timerLineBind.Enabled = true;
                btnStartStop.Text = "停  止";
            }
        }

        private void btnChangeFCZ_Click(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Minimized;
            /*this.Hide();
            //notifyIcon1.Visible = true;  //托盘图标可见

            ProcessStartInfo psi = new ProcessStartInfo(@"FczC.exe");
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();*/

            KSPrj.Program.frm1.Show();

            this.Hide();


        }

        public void timerDateTime_Tick(object sender, EventArgs e)
        {
            try
            {
                lblDateTime.Text = string.Format("{0:F}", DateTime.Now); //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//+ "  读PI已" + " 000 秒"   "yyyy-MM-dd HH:mm:ss"

                if (PIRead.IsOperatingPI & (DateTime.Now - PIRead.TimeOfBeginOperatingPI).TotalSeconds > 4)
                    lblDateTime.Text = DateTime.Now.ToLongTimeString() + "  读PI已" + Math.Round((DateTime.Now - PIRead.TimeOfBeginOperatingPI).TotalSeconds, 1) + "秒！";
                //else
                //    lblDateTime.Text = DateTime.Now.ToLongDateString() + "  " + DateTime.Now.ToLongTimeString();

                lblDateTime.Refresh();

                if (lblRunOptimizePromptText.Text != RunOptimizePrompt.GetPromptText())
                    lblRunOptimizePromptText.Text = RunOptimizePrompt.GetPromptText();
                if (RunOptimizePrompt.GetPromptTextForeColor() == Color.Red)
                {
                    if (m_iTimerDateTimeCount % 2 == 0)
                        lblRunOptimizePromptText.ForeColor = Color.DimGray;
                    else
                        lblRunOptimizePromptText.ForeColor = Color.Red;

                }
                else
                    lblRunOptimizePromptText.ForeColor = Color.Lime;

                m_iTimerDateTimeCount += 1;
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
        }

        Chart m_chartMouseDown = null;
        private void Chart_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                m_chartMouseDown = (Chart)sender;
        }
        // 创建Chart控件里Series控件的Tag数据对象
        /// <summary>
        /// 创建Chart控件里Series控件的Tag数据对象
        /// </summary>
        /// <param name="iID">测点ID</param>
        /// <returns>创建的LabelTag</returns>
        LabelTag CreateChartSeriesTag(int iID)
        {
            LabelTag labelTag = null;
            DataRow row = FTableTags.getDataTableByRowFilter("[id]=" + iID).Rows[0];
            labelTag = new LabelTag(int.Parse(row["id"].ToString()), row["tagdesc"].ToString(), row["unit"].ToString());
            return labelTag;
        }
        // 创建Chart控件里Series控件的Tag数据对象
        /// <summary>
        /// 创建Chart控件里Series控件的Tag数据对象
        /// </summary>
        /// <param name="row">测点信息行</param>
        /// <returns></returns>
        LabelTag CreateChartSeriesTag(DataRow row)
        {
            LabelTag labelTag = null;
            labelTag = new LabelTag(int.Parse(row["id"].ToString()), row["tagdesc"].ToString(), row["unit"].ToString());
            return labelTag;
        }
        //曲线选择测点快捷菜单
        private void contextMenuStrip_Chart_SelectTags_Click(object sender, EventArgs e)
        {
            SelectTag frmSelectTag = new SelectTag(5);
            frmSelectTag.ShowDialog();
            if (frmSelectTag.sExcelCells.Length > 0)
            {
                int i;
                //先隐藏所有Series
                for (i = 0; i < m_chartMouseDown.Series.Count; i++)
                {
                    m_chartMouseDown.Series[i].Enabled = false;
                }
                Series series = null;
                DataRow row = null;
                for (i = 0; i < frmSelectTag.dtCheckedTags.Rows.Count; i++)
                {
                    row = frmSelectTag.dtCheckedTags.Rows[i];
                    if (i < m_chartMouseDown.Series.Count)//获取已存在的Series，使其可见
                    {
                        series = m_chartMouseDown.Series[i];
                        series.Enabled = true;
                    }
                    else//创建Series
                    {
                        series = m_chartMouseDown.Series.Add("s" + i);
                        series.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                        series.XValueMember = "ValueTime";
                        series.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                        series.Color = Color.FromArgb(HistoryLines.Rnd(), HistoryLines.Rnd(), HistoryLines.Rnd());
                        series.ChartType = SeriesChartType.Line;
                    }
                    try { series.LegendText = row["TagDesc"].ToString() + ":" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F" + row["id"].ToString()].ToString()), 2) + row["unit"].ToString(); }
                    catch { series.LegendText = row["TagDesc"].ToString(); }
                    series.YValueMembers = "F" + row["id"].ToString();
                    series.Tag = CreateChartSeriesTag(row);
                }
                m_chartMouseDown.ChartAreas[0].AxisX.Title = "";
                m_chartMouseDown.ChartAreas[0].AxisY.Title = "";
                m_chartMouseDown.DataSource = m_dtLineData;
                m_chartMouseDown.DataBind();
            }
        }
        //恢复的默认曲线菜单
        private void contextMenuStrip_Chart_SelectTags_RecoverDefault_Click(object sender, EventArgs e)
        {
            int i;
            //先隐藏第二条以后的曲线
            for (i = 2; i < m_chartMouseDown.Series.Count; i++)
            {
                m_chartMouseDown.Series[i].Enabled = false;
            }
            Series series = null;
            switch (m_chartMouseDown.Name)
            {
                case "chart1":
                    series = m_chartMouseDown.Series[0];
                    series.Enabled = true;
                    series.LegendText = chart1Series0Tag.TagDesc + ":" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F" + chart1Series0Tag.TagID].ToString()), 2) + chart1Series0Tag.TagUnit;
                    series.YValueMembers = "F" + chart1Series0Tag.TagID;
                    series.Tag = chart1Series0Tag;

                    series = m_chartMouseDown.Series[1];
                    series.Enabled = true;
                    series.LegendText = chart1Series1Tag.TagDesc + ":" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F" + chart1Series1Tag.TagID].ToString()), 2) + chart1Series1Tag.TagUnit;
                    series.YValueMembers = "F" + chart1Series1Tag.TagID;
                    series.Tag = chart1Series1Tag;
                    chart1.ChartAreas[0].AxisY.Title = "出水温度(℃)";
                    chart1.ChartAreas[0].AxisX.Title = "时间(m)";
                    break;
                case "chart2":
                    series = m_chartMouseDown.Series[0];
                    series.Enabled = true;
                    series.LegendText = chart2Series0Tag.TagDesc + ":" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F" + chart2Series0Tag.TagID].ToString()), 2) + chart2Series0Tag.TagUnit;
                    series.YValueMembers = "F" + chart2Series0Tag.TagID;
                    series.Tag = chart2Series0Tag;

                    series = m_chartMouseDown.Series[1];
                    series.Enabled = true;
                    series.LegendText = chart2Series1Tag.TagDesc + ":" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F" + chart2Series1Tag.TagID].ToString()), 2) + chart2Series1Tag.TagUnit;
                    series.YValueMembers = "F" + chart2Series1Tag.TagID;
                    series.Tag = chart2Series1Tag;
                    chart2.ChartAreas[0].AxisY.Title = "循环水量(t/h)";
                    chart2.ChartAreas[0].AxisX.Title = "时间(m)";
                    break;
                case "chart3":
                    series = m_chartMouseDown.Series[0];
                    series.Enabled = true;
                    series.LegendText = chart3Series0Tag.TagDesc + ":" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F" + chart3Series0Tag.TagID].ToString()), 2) + chart3Series0Tag.TagUnit;
                    series.YValueMembers = "F" + chart3Series0Tag.TagID;
                    series.Tag = chart3Series0Tag;

                    series = m_chartMouseDown.Series[1];
                    series.Enabled = true;
                    series.LegendText = chart3Series1Tag.TagDesc + ":" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F" + chart3Series1Tag.TagID].ToString()), 2) + chart3Series1Tag.TagUnit;
                    series.YValueMembers = "F" + chart3Series1Tag.TagID;
                    series.Tag = chart3Series1Tag;
                    chart3.ChartAreas[0].AxisY.Title = "热负荷(W/㎡)";
                    chart3.ChartAreas[0].AxisX.Title = "时间(m)";
                    break;
            }

            m_chartMouseDown.DataSource = m_dtLineData;
            m_chartMouseDown.DataBind();

        }

        private void pnlLines_Paint(object sender, PaintEventArgs e)
        {
            pnlLines_Resize(null, null);
        }
        

        private void btnMinisize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMinisize_Click_1(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        // 热网出水温度
        /// <summary>
        /// 热网出水温度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOutWaterT_Click(object sender, EventArgs e)
        {
            (new ChartTemperature()).Show();
        }

        private void rdoHCGD_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoHCGD.Checked == true)
            { 
                SetLabelVisible("[Parent] like 'YXYHMain3.%' and [TagID] in (" + sUnit2HCGD_IDs + "," + sUnit2HCGR_IDs + ")");
            }
        }

        private void rdoHCGR_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoHCGR.Checked == true)
            { 
                SetLabelVisible("[Parent] like 'YXYHMain3.%' and [TagID] in (" + sUnit2HCGD_IDs + "," + sUnit2HCGR_IDs + ")");
            }
        }

        /// <summary>
        /// 空冷岛阀门状态
        /// 
        /// #1机组以1结尾
        /// picBoxA110/picBoxB110/picBoxC110
        /// picBoxA210/picBoxB210/picBoxC210
        /// 
        /// #2机组以2结尾
        /// picBoxA120/picBoxB120/picBoxC120
        /// picBoxA220/picBoxB220/picBoxC220
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

        //        RedGreen("picBoxA110", A1);
        //        RedGreen("picBoxB110", B1);
        //        RedGreen("picBoxC110", C1);
        //        RedGreen("picBoxA210", A2);
        //        RedGreen("picBoxB210", B2);
        //        RedGreen("picBoxC210", C2);
        //    }
        //    else
        //    {
        //        int A1 = (int)CalTree.GetFinishedTagValue("F2401");
        //        int B1 = (int)CalTree.GetFinishedTagValue("F2402");
        //        int C1 = (int)CalTree.GetFinishedTagValue("F2403");
        //        int A2 = (int)CalTree.GetFinishedTagValue("F2404");
        //        int B2 = (int)CalTree.GetFinishedTagValue("F2405");
        //        int C2 = (int)CalTree.GetFinishedTagValue("F2406");

        //        RedGreen("picBoxA120", A1);
        //        RedGreen("picBoxB120", B1);
        //        RedGreen("picBoxC120", C1);
        //        RedGreen("picBoxA220", A2);
        //        RedGreen("picBoxB220", B2);
        //        RedGreen("picBoxC220", C2);
        //    }
        //}
        private void RedGreen(string picName, int open)
        {
            PictureBox pic = null;
            try
            {
                pic = this.pnlSystemGraph.Controls.Find(picName, false)[0] as PictureBox;
                if (open == 1)
                    pic.BackgroundImage = global::KSPrj.Properties.Resources.verticalRed;
                else
                    pic.BackgroundImage = global::KSPrj.Properties.Resources.verticalGreen;
            }
            catch { pic.BackgroundImage = global::KSPrj.Properties.Resources.verticalGreen; }
            pic.Refresh();
        }

        private void btnLJJML_Click(object sender, EventArgs e)
        {
            (new LJJML(5)).Show();
        }

        private void btnAlarmHistory_Click(object sender, EventArgs e)
        {
            (new YHYXYH.YXYH.AlarmListYXYH("YXYH", 5)).Show();
        }

        private void btnCalTempAndFlow_Click(object sender, EventArgs e)
        {
            calCSWDLLobj.Show();
        }

        private void lblDateTime_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnEditPassword_Click(object sender, EventArgs e)
        {
            PWDChange pwd = new PWDChange();
            pwd.Show(); 
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

        private void label1_DoubleClick(object sender, EventArgs e)
        {
            ContactUs cu = new ContactUs();
            cu.ShowDialog();
            cu.Dispose();
        }

        private void btnSZMX_Click(object sender, EventArgs e)
        {
            (new LJSZMX(5)).Show();
        }

        private void btnRealValue_Click(object sender, EventArgs e)
        {
            MoreTags mt = new MoreTags(5);
            mt.Show(); 
        }

        private void pnlSystemGraph_Paint(object sender, PaintEventArgs e)
        {

        }


        private void GBYGK_CheckedChanged(object sender, EventArgs e)
        {
            GlobalVariables.YXYH_GBY_CN_JS = true;
        }

        private void CNGK_CheckedChanged(object sender, EventArgs e)
        {
            GlobalVariables.YXYH_GBY_CN_JS = false;
        }


    }
}
