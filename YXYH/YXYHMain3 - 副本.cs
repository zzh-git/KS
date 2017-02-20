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

namespace YHYXYH.YXYH
{
    public partial class YXYHMain3 : Form
    {
        public YXYHMain3()
        {
            InitializeComponent();

            AddConfigedControl();
            SetBindingLabelsText();
        }
        DataTable dtControls = SQLHelper.ExecuteDt("select * from [YXYHControlConfig] order by Parent asc");
        DataTable m_dtLineData = CalTagValue.GetAllData();
        DataView viewTagLabelBinding = YXYHTagLabelBinding.getAllData().DefaultView;
        DataView viewShowList = null;
        HistoryLines m_frmHistoryLines = new HistoryLines();
        SystemGraph systemGraph = new SystemGraph();
        RunOptimizePrompt m_frmRunOptimizePrompt = new RunOptimizePrompt();
        TagLabelBinding tagLabelBinding = new TagLabelBinding();
        string sNewLabel = "";
        private void YXYHMain3_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = CalTagValue.GetAllTagValues();
            dataGridView1.Refresh();

            timerLineBind.Interval = ConstYXYH.RefIntvel; ;
            timerLineBind.Enabled = true;

            lblTemp.Tag = new LabelTag(false, 208, "环境温度");
            lblTemp.DoubleClick += new EventHandler(Label_DoubleClick);//双击事件
            lblTemp.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件


            //优化出水温度与实际出水温度
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "HH:mm";
            chart1.DataSource = m_dtLineData;
            //chart1.Series[0].LegendText = "优化出水温度";
            chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart1.Series[0].XValueMember = "ValueTime";
            chart1.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart1.Series[0].YValueMembers = "F213";
            //chart1.Series[1].LegendText = "实际出水温度";
            chart1.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart1.Series[1].XValueMember = "ValueTime";
            chart1.Series[1].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart1.Series[1].YValueMembers = "F152";

            //优化功率与实际功率，**********这里实际显示的是“热负荷”
            chart3.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "HH:mm";
            chart3.DataSource = m_dtLineData;
            //chart1.Series[0].LegendText = "优化热负荷";
            chart3.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart3.Series[0].XValueMember = "ValueTime";
            chart3.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart3.Series[0].YValueMembers = "F219";
            //chart1.Series[1].LegendText = "实际热负荷";
            chart3.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart3.Series[1].XValueMember = "ValueTime";
            chart3.Series[1].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart3.Series[1].YValueMembers = "F202";

            //优化抽汽量和实际抽汽量
            chart2.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "HH:mm";
            chart2.DataSource = m_dtLineData;
            //chart1.Series[0].LegendText = "优化抽汽量";
            chart2.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart2.Series[0].XValueMember = "ValueTime";
            chart2.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart2.Series[0].YValueMembers = "F161";
            //chart1.Series[0].LegendText = "优化抽汽量";
            chart2.Series[1].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            chart2.Series[1].XValueMember = "ValueTime";
            chart2.Series[1].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart2.Series[1].YValueMembers = "F222";

            timerLineBind_Tick(null, null);
        }
        /// <summary>
        /// 加载程序运行中配置的控件
        /// </summary>
        void AddConfigedControl()
        {
            try
            {
                //控制添加测点的按钮是否可见
                if (ConstYXYH.IsCanAddLabel)
                    btnAddTag.Visible = true;
                else
                    btnAddTag.Visible = false;

                int i;
                Control parent = null;
                Control control = null;
                Rectangle rect;
                LabelTag labelTag = null;
                foreach (DataRow row in dtControls.Rows)
                {

                    parent = panel6;
                    try
                    {
                        //这里先以Label写死；如果以后还有其他类型的控件，则在这里枚举
                        control = new Label();
                        ((Label)control).AutoSize = true;
                        control.Name = row["ControlName"].ToString();
                        control.Text = control.Name;
                        if (row["ControlText"].ToString().Length > 1)
                            control.Text = row["ControlText"].ToString();
                        rect = new Rectangle();
                        rect.X = int.Parse(row["ControlX"].ToString());
                        rect.Y = int.Parse(row["ControlY"].ToString());
                        control.Bounds = rect;
                        try { control.ForeColor = Color.FromArgb(int.Parse(row["ForeColor"].ToString())); }
                        catch { }
                        try { control.BackColor = Color.FromArgb(int.Parse(row["BackColor"].ToString())); }
                        catch { }
                        labelTag = new LabelTag();
                        labelTag.ControlX = rect.X;
                        labelTag.ControlY = rect.Y;
                        labelTag.ParentHeight = int.Parse(row["ParentHeight"].ToString());
                        labelTag.ParentWidth = int.Parse(row["ParentWidth"].ToString());
                        control.Tag = labelTag;
                        control.DoubleClick += new EventHandler(Label_DoubleClick);//双击事件
                        control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
                        if (ConstYXYH.IsCanMoveLabel)
                        {
                            control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
                            control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
                            control.ContextMenuStrip = contextMenuStrip_Label;//添加快捷菜单
                        }
                        parent.Controls.Add(control);
                    }
                    catch { }
                }

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
                parent = panel6;
                LabelTag labelTag = null;
                foreach (DataRow row in YXYHTagLabelBinding.getAllData().Rows)
                {
                    try
                    {
                        //由父控件找到本Label
                        control = parent.Controls.Find(row["LabelName"].ToString(), false)[0];
                        control.Text = CalTagValue.GetTagValueAndUnitByID(row["TagID"].ToString());
                        control.Refresh();

                        try
                        {
                            labelTag = (LabelTag)control.Tag;
                            if (labelTag.IsSetToolTipText == false)
                            {
                                toolTip1.SetToolTip(control, row["TagDesc"].ToString());
                                labelTag.IsSetToolTipText = true;
                                labelTag.TagID = int.Parse(row["TagID"].ToString());
                                labelTag.TagDesc = row["TagDesc"].ToString();
                                control.Tag = labelTag;
                            }
                            if (labelTag.TagID == 0)
                            {
                                labelTag.TagID = int.Parse(row["TagID"].ToString());
                                labelTag.TagDesc = row["TagDesc"].ToString();
                                control.Tag = labelTag;
                                WriteLog.WriteLogs("id=0");
                            }
                        }
                        catch
                        {
                            control.Tag = new LabelTag(true, int.Parse(row["TagID"].ToString()), row["TagDesc"].ToString());
                            toolTip1.SetToolTip(control, row["TagDesc"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        //WriteLog.WriteLogs(ex.ToString());
                    }
                }
                lblRunOptimizePromptText.Text = RunOptimizePrompt.GetPromptText();
                lblRunOptimizePromptText.ForeColor = RunOptimizePrompt.GetPromptTextForeColor();
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
        }
        void SetLabelsForeColor(Color color)
        {
            LabelTag labelTag = null;
            foreach (Control control in panel6.Controls)
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
                control = (Label)sender;
            if (e.Button == MouseButtons.Left)
            {
                pointMouseDown.X = e.X;
                pointMouseDown.Y = e.Y;
            }

        }
        void control_MouseMove(object sender, MouseEventArgs e)
        {
            control = (Label)sender;
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
            control = (Label)sender;
            if (e.Button == MouseButtons.Left)
            {
                if (isDrag)
                {
                    isDrag = false;
                    sSql = "update YXYHControlConfig set ControlX=" + control.Location.X + ",ControlY=" + control.Location.Y
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

        private void timerLineBind_Tick(object sender, EventArgs e)
        {
            SetBindingLabelsText();
            //SetLabelsForeColor(Color.Red);
            //SetLabelsForeColor(Color.Lime);
            lblDQY.Text = "大气压力：" + CalTagValue.GetTagValueAndUnitByID("4");
            lblTemp.Text = "环境温度：" + CalTagValue.GetTagValueAndUnitByID("208");

            m_dtLineData = CalTagValue.GetAllData();
            try
            {
                chart1.DataSource = m_dtLineData;
                chart1.Series[0].LegendText = "优化出水温度：" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F213"].ToString()), 2) + "℃";
                chart1.Series[1].LegendText = "实际出水温度：" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F152"].ToString()), 2) + "℃";
                chart1.DataBind();
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
            try
            {
                chart3.DataSource = m_dtLineData;
                chart3.Series[0].LegendText = "优化热负荷：" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F219"].ToString()), 2) + "瓦/平方米";
                chart3.Series[1].LegendText = "实际热负荷：" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F202"].ToString()), 2) + "瓦/平方米";
                chart3.DataBind();
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }

            try
            {
                chart2.DataSource = m_dtLineData;
                chart2.Series[0].LegendText = "实际循环水量：" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F161"].ToString()), 2) + "t/h";
                chart2.Series[1].LegendText = "优化循环水量：" + Math.Round(double.Parse(m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["F222"].ToString()), 2) + "t/h";
                chart2.DataBind();
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }

            viewShowList = CalTagValue.GetAllTagValues().DefaultView;
            viewShowList.RowFilter = "isShow=1 and TagValue>0";
            dataGridView1.DataSource = viewShowList;
            //dataGridView1.Refresh();
        }

        private void panel6_Resize(object sender, EventArgs e)
        {
            Rectangle rect;
            LabelTag labelTag = null;
            this.Visible = false;
            foreach (Control control in panel6.Controls)
            {
                try
                {
                    labelTag = (LabelTag)(control.Tag);
                    rect = new Rectangle();
                    rect.X = Convert.ToInt32(1.0 * labelTag.ControlX / labelTag.ParentWidth * panel6.Width);
                    rect.Y = Convert.ToInt32(1.0 * labelTag.ControlY / labelTag.ParentHeight * panel6.Height);
                    control.Bounds = rect;
                }
                catch { }
            }
            this.Visible = true;
        }

        private void Label_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                LabelTag labelTag = (LabelTag)((Label)sender).Tag;
                if (labelTag.TagID > 0)
                {
                    HistoryLines m_frmHistoryLines = new HistoryLines();
                    m_frmHistoryLines.labelTag.TagID = labelTag.TagID;
                    m_frmHistoryLines.labelTag.TagDesc = labelTag.TagDesc;
                    m_frmHistoryLines.ShowDialog();
                }
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
            chart1.Width = panel2.Width / 3;
            chart2.Width = chart1.Width;
        }

        private void Chart_DoubleClick(object sender, EventArgs e)
        {
            HistoryLines historyLines = new HistoryLines();
            historyLines.seriesCollection = ((Chart)sender).Series;
            historyLines.ShowDialog();
        }

        private void panel6_DoubleClick(object sender, EventArgs e)
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
            (new TagReplace()).ShowDialog();
        }

        private void btnHis_Click(object sender, EventArgs e)
        {
            SelectTagDate selectTagDate = new SelectTagDate();
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
        }

        private void btnFixedValueEdit_Click(object sender, EventArgs e)
        {
            (new FixedValueEdit()).ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //Close();
            //Application.Exit();
            //消息框中需要显示哪些按钮，此处显示“确定”和“取消”
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;

            //"确定要退出吗？"是对话框的显示信息，"退出系统"是对话框的标题
            //默认情况下，如MessageBox.Show("确定要退出吗？")只显示一个“确定”按钮。
            DialogResult dr = MessageBox.Show("确定要退出吗?", "退出系统", messButton);
            if (dr == DialogResult.OK) //如果点击“确定”按钮
            {
                string sSql = "select * from syspara";
                DataTable dtBasepwt = SQLHelper.ExecuteDt(sSql);
                //string s
                fczC.FrmInputPws fip = new fczC.FrmInputPws();
                fip.ShowDialog();
                string strRet = fip.Text;

                string[] strRetA = strRet.Split(',');
                if (strRet.Length == 0)
                    return;
                if (strRetA[1] == "ok")
                {
                    if (dtBasepwt.Rows[0]["pws"].ToString() == strRetA[0])
                    {
                        fip.Dispose();
                        this.Close();
                        this.Dispose();
                        Application.Exit();
                    }
                    else
                    {
                        MessageBox.Show("密码错误！");
                        fip.Dispose();
                    }
                }
            }
            else //如果点击“取消”按钮
                return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (new GRLJSR()).ShowDialog();
        }

        private void contextMenuStrip_Label_del_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除此测点吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    ArrayList listSql = new ArrayList();
                    listSql.Add("delete [YXYHControlConfig] where ControlName='" + control.Name + "'");
                    listSql.Add("delete [YXYHTagLabelBinding] where LabelName='" + control.Name + "'");
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
            this.panel6.Controls.Add(control);
            sSql = "insert into YXYHControlConfig(ControlName,ControlX,ControlY,ForeColor,BackColor,Type,Parent,ParentHeight,ParentWidth) values('"
                + control.Name + "'," + control.Location.X + "," + control.Location.Y + "," + control.ForeColor.ToArgb() + ","
                + control.BackColor.ToArgb() + ",'" + control.GetType().Name + "','" + TagLabelBinding.GetParents(control) + "',"
                + control.Parent.Height + "," + control.Parent.Width + ")";
            SQLHelper.ExecuteSql(sSql);
        }

        private void btnSSCST_Click(object sender, EventArgs e)
        {
            (new SSCS()).ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btnGRSRJJLJ_Click(object sender, EventArgs e)
        {
            (new RLZCFY()).ShowDialog();
        }

        private void btnGRSRJTXS_Click(object sender, EventArgs e)
        {
            CalTagValue.CalAllTagValue();
        }

        private void btnGDLJSR_Click(object sender, EventArgs e)
        {
            (new GDLJSR()).ShowDialog();
        }

        private void btnLJMLR_Click(object sender, EventArgs e)
        {
            (new LJMLR()).ShowDialog();
        }

        private void contextMenuStrip_Label_SaveLabel_Click(object sender, EventArgs e)
        {
            LabelTag labelTag = null;
            ArrayList lstSql = new ArrayList();
            Rectangle rect;
            foreach (Control control in panel6.Controls)
            {
                try
                {
                    labelTag = (LabelTag)(control.Tag);
                    rect = control.Bounds;
                    lstSql.Add("update YXYHControlConfig set ControlX=" + rect.X + ",ControlY=" + rect.Y + ",ParentHeight=" + panel6.Height
                        + ",ParentWidth=" + panel6.Width + " where ControlName='" + control.Name + "'");
                }
                catch { }
            }
            SQLHelper.ExecuteSql(lstSql);
            MessageBox.Show("保存完成！");
        }

        private void btnResetTotal_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要复位累计值吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                ResetTotalPassword password = new ResetTotalPassword();
                password.ShowDialog();
                if (password.Password == "fwljz")
                    CalTagValue.ResetTotal();
                else
                    MessageBox.Show("密码错误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void contextMenuStrip_Label_SaveOneLabel_Click(object sender, EventArgs e)
        {
            string sSql = "insert into [YXYHControlConfig](ControlName,ControlX,ControlY,Parent,Type,ForeColor,BackColor,ParentHeight,ParentWidth) values(";
            string sSelect = "select * from [YXYHControlConfig] where ControlName='" + control.Name + "'";
            DataTable dt = SQLHelper.ExecuteDt(sSelect);
            if (dt.Rows.Count > 0)
                sSql = "update YXYHControlConfig set ControlX=" + control.Bounds.X + ",ControlY=" + control.Bounds.Y + ",ParentHeight="
                    + panel6.Height + ",ParentWidth=" + panel6.Width + " where ControlName='" + control.Name + "'";
            else
                sSql += "'" + control.Name + "'," + control.Bounds.X + "," + control.Bounds.Y + ",'" + control.Parent.Name + "','Label',"
                    + control.ForeColor.ToArgb() + "," + control.BackColor.ToArgb() + "," + control.Parent.Height + "," 
                    + control.Parent.Width + ")";
            SQLHelper.ExecuteDt(sSql);
            MessageBox.Show("保存完成！");
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                    HistoryLines m_frmHistoryLines = new HistoryLines();
                    m_frmHistoryLines.labelTag.TagID =int.Parse( dataGridView1.CurrentRow.Cells["TagID"].Value.ToString());
                    m_frmHistoryLines.labelTag.TagDesc = dataGridView1.CurrentRow.Cells["TagDesc"].Value.ToString();
                    m_frmHistoryLines.ShowDialog();
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
        }

        private void panel3_Resize(object sender, EventArgs e)
        {
            int iDistance = 6;
            int iHeight = (panel3.Height - 13* iDistance)/12;
            btnStartStop.Top = iDistance;
            btnStartStop.Height = iHeight;
            btnSSCST.Top = iDistance * 2 + iHeight;
            btnSSCST.Height = iHeight;
            btnHis.Top = iDistance *3 + iHeight*2;
            btnHis.Height = iHeight;
            btnRLZCFY.Top = iDistance *4 + iHeight*3;
            btnRLZCFY.Height = iHeight;
            btnGRLJSR.Top = iDistance * 5 + iHeight * 4;
            btnGRLJSR.Height = iHeight;
            btnGDLJSR.Top = iDistance * 6 + iHeight *5;
            btnGDLJSR.Height = iHeight;
            btnLJMLR.Top = iDistance * 7 + iHeight *6;
            btnLJMLR.Height = iHeight;
            btnOrgTD.Top = iDistance * 8 + iHeight * 7;
            btnOrgTD.Height = iHeight;
            btnFixedValueEdit.Top = iDistance * 9 + iHeight *8;
            btnFixedValueEdit.Height = iHeight;
            btnResetTotal.Top = iDistance *10 + iHeight * 9;
            btnResetTotal.Height = iHeight;
            btnClose.Top = iDistance * 11 + iHeight *10;
            btnClose.Height = iHeight;

            btnChangeFCZ.Top = iDistance * 12 + iHeight * 11;
            btnChangeFCZ.Height = iHeight;


            
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (timerLineBind.Enabled == true)
            {
                timerLineBind.Enabled = false;
                btnStartStop.Text = "启  动";
            }
            else
            {
                timerLineBind.Enabled = true;
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

            fczC.Program.frm1.Show();

            this.Hide();


        }

     
    }
}
