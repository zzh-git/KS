using HAOCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WinHost;

namespace YHYXYH.YXYH
{
    public partial class HistoryLines : Form
    {
        int UnitNO = 5;
        public HistoryLines(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
        }
        string sTitle = "历史曲线";
        string sExcelCells = "";
        public LabelTag labelTag = new LabelTag();
        public SeriesCollection seriesCollection = null;//双击的主窗体曲线的SeriesCollection
        DataTable m_dtLineData = null;
        DataTable m_dtCheckedTags = null;
        private void HistoryLines_Load(object sender, EventArgs e)
        {
            timerChartBind.Interval = GlobalVariables.RefIntvel;
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "yyyy-MM-dd HH:mm:ss";
            if (sExcelCells.Length > 0)
            {
                ShowLines();
                this.Text = sTitle + "--历史查询";
            }
            else
            {
                //注意区分五六号机 ZZH 
                if (UnitNO == 5)
                    m_dtLineData = GlobalVariables.dtChartDataFive;
                else if (UnitNO == 6)
                    m_dtLineData = GlobalVariables.dtChartDataSix;
                dateTimePickerBegin1.Value = (DateTime)m_dtLineData.Rows[0]["ValueTime"];
                dateTimePickerBegin2.Value = dateTimePickerBegin1.Value;
                dateTimePickerEnd1.Value = (DateTime)m_dtLineData.Rows[m_dtLineData.Rows.Count - 1]["ValueTime"];
                dateTimePickerEnd2.Value = dateTimePickerEnd1.Value;

                if (seriesCollection == null)//单个测点曲线
                {
                    chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                    chart1.Series[0].XValueMember = "ValueTime";
                    chart1.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                    chart1.Series[0].YValueMembers = "F" + labelTag.TagID.ToString();
                    //这里也要区分五六号机 ZZH 
                    if (TagValue.GetStrSetValueFive(labelTag.TagID.ToString(), "adjustValue").Contains("F3004"))
                        chart1.Series[0].LegendText = labelTag.TagDesc + "(绝对)" + "(" + labelTag.TagUnit + ")";
                    else
                        chart1.Series[0].LegendText = labelTag.TagDesc + "(" + labelTag.TagUnit + ")";
                }
                else//双曲线
                {
                    Series seriesNew = null;
                    chart1.Series.Clear();
                    foreach (Series series in seriesCollection)
                    {
                        seriesNew = chart1.Series.Add(series.YValueMembers);
                        seriesNew.LegendText = series.LegendText;
                        seriesNew.XValueType = series.XValueType;
                        seriesNew.XValueMember = series.XValueMember;
                        seriesNew.YValueType = series.YValueType;
                        seriesNew.YValueMembers = series.YValueMembers;
                        sExcelCells += series.YValueMembers + ",";
                        seriesNew.Color = series.Color;
                        seriesNew.ChartType = SeriesChartType.Line;
                    }
                    sExcelCells = sExcelCells.Trim(',');
                }

                chart1.DataSource = m_dtLineData;
                chart1.DataBind();
                timerChartBind.Enabled = true;
                this.Text = sTitle + "--实时曲线";
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                string sFields = "F" + labelTag.TagID.ToString();

                lblQueryWait.Visible = true;
                lblQueryWait.Refresh();
                timer2.Enabled = true;
                string sTime = PublicFunction.DateTimeToStringWithfff(DateTime.Now);
                string sSql = "insert into QueryAlarm(QueryDate,StartTime,EndTime,QueryFields,UnitNO,AlarmDesc) values('"
                    + sTime + "','" + dateTimePickerBegin2.Value + "','" + dateTimePickerEnd2.Value + "','" + sFields + "',"
                    + UnitNO + ",'查询数据时，未按提示等待，做其他操作造成程序退出！')";
                try { SQLHelper.ExecuteSql(sSql); }
                catch { }

                if (sExcelCells.Length > 0)
                    sFields = sExcelCells;
                m_dtLineData = TagValue.QueryDBData(dateTimePickerBegin2.Value, dateTimePickerEnd2.Value, "ValueTime," + sFields, UnitNO);
                chart1.DataSource = m_dtLineData;
                chart1.DataBind();
                this.Text = sTitle + "--历史查询";
                timerChartBind.Enabled = false;
                
                sSql = "delete QueryAlarm where QueryDate='" + sTime + "'";
                try { SQLHelper.ExecuteSql(sSql); }
                catch { }
            }
            finally
            {
                timer2.Enabled = false;
                lblQueryWait.Visible = false;
                lblQueryWait.Refresh();
            }
        }

        private void dateTimePickerBegin1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerBegin2.Value = DateTime.Parse(dateTimePickerBegin1.Value.ToString("yyyy-MM-dd") + " "
                + dateTimePickerBegin2.Value.ToString("HH:mm:ss"));
        }

        private void dateTimePickerEnd1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerEnd2.Value = DateTime.Parse(dateTimePickerEnd1.Value.ToString("yyyy-MM-dd") + " "
                + dateTimePickerEnd2.Value.ToString("HH:mm:ss"));
        }

        static System.Random rnd = new Random();
        /// <summary>
        /// 生成随机0到255的RGB值
        /// </summary>
        public static int Rnd()
        {
            double dMin = 0;
            double dMax = 255;
            return Convert.ToInt32(dMin + (dMax - dMin) * rnd.NextDouble()); ;
        }

        //选择测点 
        private void btnSelectTags_Click(object sender, EventArgs e)
        {
            SelectTag frmSelectTag = new SelectTag(UnitNO);
            frmSelectTag.ShowDialog();
            if (frmSelectTag.sExcelCells.Length > 0)
            {
                this.m_dtCheckedTags = frmSelectTag.dtCheckedTags;
                this.sExcelCells = frmSelectTag.sExcelCells;
                ShowLines();

                this.Text = sTitle + "--历史查询";
                timerChartBind.Enabled = false;
            }
        }
        public void SetBeginTime(DateTime BeginTime)
        {
            dateTimePickerBegin1.Value = BeginTime;
            dateTimePickerBegin2.Value = BeginTime;
        }
        public void SetEndTime(DateTime EndTime)
        {
            dateTimePickerEnd1.Value = EndTime;
            dateTimePickerEnd2.Value = EndTime;
        }
        public void SetCheckedTags(DataTable CheckedTags)
        {
            m_dtCheckedTags = CheckedTags;
        }
        public void SetExcelCells(string ExcelCells)
        {
            sExcelCells = ExcelCells;
        }
        public void ShowLines()
        {
            if (sExcelCells.Length > 0)
            {
                mutiLineDesc = "";
                Series series = null;
                chart1.Series.Clear();
                foreach (DataRow row in m_dtCheckedTags.Rows)
                {
                    mutiLineDesc += row["TagDesc"] + ","; //用于导出的时候

                    series = chart1.Series.Add(row["ExcelCell"].ToString());
                    if (row["adjustValue"].ToString().Contains("F3004"))
                        series.LegendText = row["TagDesc"].ToString() + "(绝对)" + "（" + row["unit"].ToString() + "）";
                    else
                        series.LegendText = row["TagDesc"].ToString() + "（" + row["unit"].ToString() + "）";
                    series.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                    series.XValueMember = "ValueTime";
                    series.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                    series.YValueMembers = "F" + row["id"].ToString();
                    series.Color = Color.FromArgb(Rnd(), Rnd(), Rnd());
                    series.ChartType = SeriesChartType.Line;
                }
                m_dtLineData = TagValue.QueryDBData(dateTimePickerBegin2.Value, dateTimePickerEnd2.Value, "ValueTime," + sExcelCells, UnitNO);
                chart1.DataSource = m_dtLineData;
                chart1.DataBind();
            }
        }

        private void timerChartBind_Tick(object sender, EventArgs e)
        {
            //这里注意区分五六号机 ZZH 
            if (UnitNO == 5)
                m_dtLineData = GlobalVariables.dtChartDataFive;
            else if (UnitNO == 6)
                m_dtLineData = GlobalVariables.dtChartDataSix;
            chart1.DataSource = m_dtLineData;
            try { chart1.DataBind(); }
            catch { }
        }

        private void btnRealTime_Click(object sender, EventArgs e)
        {
            timerChartBind_Tick(null, null);
            timerChartBind.Enabled = true;
            this.Text = sTitle + "--实时曲线";
        }

        private void HistoryLines_FormClosed(object sender, FormClosedEventArgs e)
        {
            try { this.Dispose(true); }
            catch { }
        }

        string mutiLineDesc = "";
        //导出 
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //历史曲线 
                if (sExcelCells.Length > 0 && seriesCollection == null)
                {
                    if (m_dtLineData != null && m_dtLineData.Rows.Count > 0)
                    {
                        string[] desc = mutiLineDesc.Trim(',').Split(',');
                        string[] strs = ("ValueTime," + sExcelCells).Split(',');
                        DataTable dt = m_dtLineData.DefaultView.ToTable(false, strs);
                        dt.Columns[0].ColumnName = "时间";
                        int i = 1;
                        foreach (string str in desc)
                        {
                            dt.Columns[i].ColumnName = desc[i - 1];
                            i++;
                        }
                        bool b = NPOIExcelHelper.DataTableToExcel(dt, "", "历史曲线");
                        if (b)
                            MessageBox.Show("导出成功");
                    }
                }
                else
                {
                    //单个测点曲线
                    if (seriesCollection == null)
                    {
                        string excellCell = "F" + labelTag.TagID.ToString();
                        string excellName = labelTag.TagDesc;
                        DataTable dt = m_dtLineData.DefaultView.ToTable(false, new string[] { "ValueTime", excellCell });
                        dt.Columns[0].ColumnName = "时间";
                        dt.Columns[1].ColumnName = excellName;
                        bool b = NPOIExcelHelper.DataTableToExcel(dt, "", excellName);
                        if (b)
                            MessageBox.Show("导出成功");
                    }
                    //双曲线
                    else
                    {
                        string excellCell = "";
                        string[] excellName = new string[seriesCollection.Count];
                        int i = 0;
                        foreach (Series series in seriesCollection)
                        {
                            excellName[i] = series.LegendText;
                            excellCell += series.YValueMembers + ",";
                            i++;
                        }
                        excellCell = excellCell.Trim(',');
                        string[] strs = ("ValueTime," + excellCell).Split(',');
                        DataTable dt = m_dtLineData.DefaultView.ToTable(false, strs);
                        dt.Columns[0].ColumnName = "时间";
                        int count = 1;
                        foreach (string str in excellName)
                        {
                            dt.Columns[count].ColumnName = excellName[count - 1];
                            count++;
                        }
                        bool b = NPOIExcelHelper.DataTableToExcel(dt, "", "曲线数据");
                        if (b)
                            MessageBox.Show("导出成功");
                    }
                }

            }
            catch
            { MessageBox.Show("导出失败"); }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            try
            {
                lblQueryWait.Refresh();
                Application.DoEvents();
            }
            catch { }
        }

        private void lblQueryWait_Click(object sender, EventArgs e)
        {

        }

    }
}
