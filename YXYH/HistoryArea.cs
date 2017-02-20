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
    public partial class HistoryArea : Form
    {
        int UnitNO = 5;
        public HistoryArea(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
        }
        public int iTotalID = 0;//累计值ID。
        public string sTotalUnit = "";//累计值单位
        string sTitle = "历史区域";
        string sExcelCells = "";
        public LabelTag labelTag = new LabelTag();
        public Color colorSeries = Color.Lime;
        public SeriesCollection seriesCollection = null;//双击的主窗体曲线的SeriesCollection
        DataTable m_dtLineData = null;
        DataTable m_dtCheckedTags = null;
        private void HistoryLines_Load(object sender, EventArgs e)
        {
            timerChartBind.Interval = GlobalVariables.RefIntvel;
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "yyyy-MM-dd HH:mm";
            if (sExcelCells.Length > 0)
            {
                ShowLines();
                this.Text = sTitle + "--历史查询";
            }
            else
            {
                if (this.UnitNO == 5)
                    m_dtLineData = GlobalVariables.dtChartDataFive;
                else if (this.UnitNO == 6)
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
                    //chart1.Series[0].LegendText = labelTag.TagDesc+"（"+labelTag.TagUnit+"）";
                    chart1.Series[0].Color = colorSeries;
                }
                else//双曲线
                {
                    Series seriesNew = null;
                    chart1.Series.Clear();
                    foreach (Series series in seriesCollection)
                    {
                        seriesNew = chart1.Series.Add(series.YValueMembers);
                        //seriesNew.LegendText = series.LegendText;
                        seriesNew.XValueType = series.XValueType;
                        seriesNew.XValueMember = series.XValueMember;
                        seriesNew.YValueType = series.YValueType;
                        seriesNew.YValueMembers = series.YValueMembers;
                        sExcelCells += series.YValueMembers + ",";
                        seriesNew.Color = series.Color;
                        seriesNew.ChartType = SeriesChartType.Area;
                    }
                    sExcelCells = sExcelCells.Trim(',');
                }

                chart1.DataSource = m_dtLineData;
                chart1.DataBind();
                chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].BorderWidth = 5;
                chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].BorderColor = Color.White;
                chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].Label = labelTag.TagDesc + "实时值:" + chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].YValues[0].ToString("0.##") + labelTag.TagUnit;
                //这里面注释掉的都需要放开，并把没有的方法加上 ZZH 2016-9-17 
                //if (iTotalID > 0)
                //    chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].Label += "\n" + labelTag.TagDesc + "累计值：" + CalTree.GetTotalValue(iTotalID).ToString("0.##") + sTotalUnit;
                chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].LabelForeColor = Color.White;

                //MinMaxValue mmv = CalTree.GetMinMaxValue("F" + labelTag.TagID);
                try
                {
                    //这里的方法需要拿出来 ZZH 2016-9-17
                    //chart1.ChartAreas[0].AxisX.Title = "供热期累计小时数：" + CalTree.GetTotalHours().ToString("0.00")
                    //+ "                累计开始时间：" + mmv.BeginDateString + "\n" + labelTag.TagDesc + "最小值："
                    //+ mmv.MinValue.ToString("0.00") + "        " + labelTag.TagDesc + "最小值时间：" + mmv.MinDateString
                    //+ "\n" + labelTag.TagDesc + "最大值：" + mmv.MaxValue.ToString("0.00") + "        " + labelTag.TagDesc + "最大值时间："
                    //+ mmv.MaxDateString + "\n" + labelTag.TagDesc + "平均值：" + mmv.AvgValue.ToString("0.00") + labelTag.TagUnit;
                }
                catch { }
                chart1.ChartAreas[0].AxisY.Title = labelTag.TagDesc;
                timerChartBind.Enabled = true;
                this.Text = sTitle + "--实时显示";
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
                lblQueryWait.Visible = false;
                lblQueryWait.Refresh();
                timer2.Enabled = false;
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
                Series series = null;
                chart1.Series.Clear();
                foreach (DataRow row in m_dtCheckedTags.Rows)
                {
                    series = chart1.Series.Add(row["ExcelCell"].ToString());
                    //series.LegendText = row["TagDesc"].ToString() + "（" + row["unit"].ToString() + "）";
                    series.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                    series.XValueMember = "ValueTime";
                    series.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                    series.YValueMembers = "F" + row["id"].ToString();
                    series.Color = Color.FromArgb(Rnd(), Rnd(), Rnd());
                    series.ChartType = SeriesChartType.Area;
                }
                m_dtLineData = TagValue.QueryDBData(dateTimePickerBegin2.Value, dateTimePickerEnd2.Value, "ValueTime," + sExcelCells, UnitNO);
                chart1.DataSource = m_dtLineData;
                chart1.DataBind();
                //chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].BorderWidth = 5;
                //chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].BorderColor = Color.White;
                //chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].Label = labelTag.TagDesc + "实时值:" + chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].YValues[0].ToString("0.##") + labelTag.TagUnit;
                //if (iTotalID > 0)
                //    chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].Label += "\n" + labelTag.TagDesc + "累计值：" + CalTree.GetTotalValue(iTotalID).ToString("0.##") + sTotalUnit;
                //chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].LabelForeColor = Color.White;
                //MinMaxValue mmv = CalTree.GetMinMaxValue("F" + labelTag.TagID);
                //chart1.ChartAreas[0].AxisX.Title = "供热期累计小时数：" + CalTree.GetTotalHours().ToString("0.0000")
                //    + "                累计开始时间：" + mmv.BeginDateString + "\n" + labelTag.TagDesc + "最小值："
                //    + Math.Round(mmv.MinValue / 10000, 4).ToString("0.0000") + "        " + labelTag.TagDesc + "最小值时间：" + mmv.MinDateString
                //    + "\n" + labelTag.TagDesc + "最大值：" + Math.Round(mmv.MaxValue / 10000, 4).ToString("0.0000") + "        " + labelTag.TagDesc + "最大值时间："
                //    + mmv.MaxDateString + "\n" + labelTag.TagDesc + "平均值：" + mmv.AvgValue + labelTag.TagUnit;
            }
        }

        private void timerChartBind_Tick(object sender, EventArgs e)
        {
            if (this.UnitNO == 5)
                m_dtLineData = GlobalVariables.dtChartDataFive;
            else if (this.UnitNO == 6)
                m_dtLineData = GlobalVariables.dtChartDataSix;
            //m_dtLineData = GlobalVariables.dtChartDatas;
            chart1.DataSource = m_dtLineData;
            try { chart1.DataBind(); }
            catch { }
            chart1.Series[0].Points[chart1.Series[0].Points.Count - 1].BorderWidth = 5;
            chart1.Series[0].Points[chart1.Series[0].Points.Count - 1].BorderColor = Color.White;
            chart1.Series[0].Points[chart1.Series[0].Points.Count - 1].BorderDashStyle = ChartDashStyle.Dot;
            chart1.Series[0].Points[chart1.Series[0].Points.Count - 1].Label = labelTag.TagDesc + "实时值:" + chart1.Series[0].Points[m_dtLineData.Rows.Count - 1].YValues[0].ToString("0.##") + labelTag.TagUnit;
            //if (iTotalID > 0)
            //    chart1.Series[0].Points[chart1.Series[0].Points.Count - 1].Label += "\n" + labelTag.TagDesc + "累计值：" + CalTree.GetTotalValue(iTotalID).ToString("0.##") + sTotalUnit;
            chart1.Series[0].Points[chart1.Series[0].Points.Count - 1].LabelForeColor = Color.White;
            // MinMaxValue mmv = CalTree.GetMinMaxValue("F" + labelTag.TagID);
            try
            {
                //这里的方法需要拿出来 ZZH 2016-9-17
                //chart1.ChartAreas[0].AxisX.Title = "供热期累计小时数：" + CalTree.GetTotalHours().ToString("0.00")
                //+ "                累计开始时间：" + mmv.BeginDateString + "\n" + labelTag.TagDesc + "最小值："
                //+ mmv.MinValue.ToString("0.00") + "        " + labelTag.TagDesc + "最小值时间：" + mmv.MinDateString
                //+ "\n" + labelTag.TagDesc + "最大值：" + mmv.MaxValue.ToString("0.00") + "        " + labelTag.TagDesc + "最大值时间："
                //+ mmv.MaxDateString + "\n" + labelTag.TagDesc + "平均值：" + mmv.AvgValue + labelTag.TagUnit;
            }
            catch { }
        }

        private void btnRealTime_Click(object sender, EventArgs e)
        {
            timerChartBind_Tick(null, null);
            timerChartBind.Enabled = true;
            this.Text = sTitle + "--实时显示";
        }

        private void HistoryArea_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose(true);
        }

        //导出 
        private void button1_Click(object sender, EventArgs e)
        {
            try
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
                        MessageBox.Show("保存成功");
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
                        MessageBox.Show("保存成功");
                }


            }
            catch
            { MessageBox.Show("保存失败"); }
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

    }
}
