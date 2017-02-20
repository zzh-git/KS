using HAOCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinHost;

namespace YHYXYH.YXYH
{
    public partial class LJJML : Form
    {
        ////这里注释掉的都要看一遍 ZZH 2016-9-17 
        int UnitNO = 5;
        string F18, F19, F3197;
        public LJJML(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
            if (UnitNO == 5)
            {
                F18 = "F18";
                F19 = "F19";
                F3197 = "F3197";
            }
            else if (UnitNO == 6)
            {
                F18 = "F118";
                F19 = "F119";
                F3197 = "F4197";
            }
        }

        long lTotalCount = -1;
        double dHourOfYear = 0;
        double dXMin = 0;
        DataTable dtChart = null;
        private void LJMLR_Load(object sender, EventArgs e)
        {
            chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart1.Series[0].XValueMember = "HourOfYear";
            chart1.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            //chart1.Series[0].YValueMembers = "F18";
            chart1.Series[0].YValueMembers = F18;
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "0.##";
            timerChartBind.Interval = 1000;
            timerChartBind.Enabled = true;
            dtChart = TagLJValue.GetTotalDataWithNewTable(UnitNO);
            if (dtChart.Rows.Count > 0)
            {
                dateTimePickerBegin1.Value = DateTime.Parse(dtChart.Rows[0]["ValueTime"].ToString());
                dateTimePickerBegin2.Value = dateTimePickerBegin1.Value;
            }

            timerChartBind_Tick(null, null);

        }

        private void timerChartBind_Tick(object sender, EventArgs e)
        {
            DataRow row;
            if (lTotalCount < TagLJValue.GetTotalCount(UnitNO))
            {
                lTotalCount = TagLJValue.GetTotalCount(UnitNO);
                dtChart.Dispose();
                dtChart = null;
                dtChart = TagLJValue.GetTotalDataWithNewTable(UnitNO);
                if (dtChart.Rows.Count > 0)
                {
                    dHourOfYear = double.Parse(dtChart.Rows[dtChart.Rows.Count - 1]["HourOfYear"].ToString());
                    if (rdoTotalHours.Checked == true)
                    {
                        chart1.Series[0].XValueMember = "HourOfYear";
                        chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                        dXMin = double.Parse(dtChart.Rows[0]["HourOfYear"].ToString());
                        chart1.ChartAreas[0].AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
                        chart1.ChartAreas[0].AxisX.MajorGrid.Interval = (dHourOfYear - dXMin) / 10;
                        chart1.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
                        chart1.ChartAreas[0].AxisX.Interval = (dHourOfYear - dXMin) / 10;
                        chart1.ChartAreas[0].AxisX.Title = "供热期累计小时数";
                        chart1.ChartAreas[0].AxisX.LabelStyle.Format = "0";
                    }
                    else
                    {
                        DateTime timeBegin = (DateTime)dtChart.Rows[0]["ValueTime"];
                        DateTime timeEnd = (DateTime)dtChart.Rows[dtChart.Rows.Count - 1]["ValueTime"];
                        chart1.Series[0].XValueMember = "ValueTime";
                        chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                        chart1.ChartAreas[0].AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Hours;
                        chart1.ChartAreas[0].AxisX.MajorGrid.Interval = (timeEnd - timeBegin).TotalHours / 6;
                        chart1.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Hours;
                        chart1.ChartAreas[0].AxisX.Interval = (timeEnd - timeBegin).TotalHours / 6;
                        chart1.ChartAreas[0].AxisX.Title = "时间";
                        chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd HH:mm";
                    }
                    chart1.ChartAreas[0].AxisY.Title = "累计节标煤量：" + Math.Round(double.Parse(dtChart.Rows[dtChart.Rows.Count - 1][F18].ToString()), 2) + "吨";
                    row = dtChart.NewRow();
                    row["ValueTime"] = DateTime.Parse(dtChart.Rows[dtChart.Rows.Count - 1]["ValueTime"].ToString()).AddSeconds(1);
                    row["HourOfYear"] = dHourOfYear + 0.0001;
                    dtChart.Rows.Add(row);
                }
            }
            row = dtChart.NewRow();
            row["ValueTime"] = DateTime.Now;
            dHourOfYear += timerChartBind.Interval / 1000 / 3600d;
            row["HourOfYear"] = dHourOfYear;
            MinMaxValue mmv = TagLJValue.GetMinMaxValue(F3197, UnitNO);
            if (UnitNO == 5)
                chart1.ChartAreas[0].AxisX.Title = "供热期累计小时数：" + Math.Round(dHourOfYear, 4).ToString("0.0000")
                + "                累计开始时间：" + mmv.BeginDateString + "\n节标煤量最小值："
                + mmv.MinValue.ToString("0.0000") + "        节标煤量最小值时间：" + mmv.MinDateString
                + "\n节标煤量最大值：" + mmv.MaxValue.ToString("0.0000") + "        节标煤量最大值时间："
                + mmv.MaxDateString + "\n节标煤量平均值：" + TagLJValue.GetTotalDataAvg(F18, UnitNO) + "吨/小时        累计节省费用："
                + Math.Round(double.Parse(GlobalVariables.dtChartTotalDatasFive.Rows[GlobalVariables.dtChartTotalDatasFive.Rows.Count - 1][F19].ToString()), 2) + "万元";
            else if (UnitNO == 6)
                chart1.ChartAreas[0].AxisX.Title = "供热期累计小时数：" + Math.Round(dHourOfYear, 4).ToString("0.0000")
                + "                累计开始时间：" + mmv.BeginDateString + "\n节标煤量最小值："
                + mmv.MinValue.ToString("0.0000") + "        节标煤量最小值时间：" + mmv.MinDateString
                + "\n节标煤量最大值：" + mmv.MaxValue.ToString("0.0000") + "        节标煤量最大值时间："
                + mmv.MaxDateString + "\n节标煤量平均值：" + TagLJValue.GetTotalDataAvg(F18, UnitNO) + "吨/小时        累计节省费用："
                + Math.Round(double.Parse(GlobalVariables.dtChartTotalDatasSix.Rows[GlobalVariables.dtChartTotalDatasSix.Rows.Count - 1][F19].ToString()), 2) + "万元";
            chart1.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Near;
            dtChart.Rows.Add(row);
            chart1.DataSource = dtChart;
            chart1.DataBind();

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

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                lblQueryWait.Visible = true;
                lblQueryWait.Refresh();
                string sTime = PublicFunction.DateTimeToStringWithfff(DateTime.Now);
                string sSql = "insert into QueryAlarm(QueryDate,StartTime,EndTime,QueryFields,UnitNO,AlarmDesc) values('"
                    + sTime + "','" + dateTimePickerBegin2.Value + "','" + dateTimePickerEnd2.Value + "','" + F18 + "',"
                    + UnitNO + ",'查询数据时，未按提示等待，做其他操作造成程序退出！')";
                try { SQLHelper.ExecuteSql(sSql); }
                catch { }

                dtChart = TagLJValue.QueryDBTotalData(dateTimePickerBegin2.Value, dateTimePickerEnd2.Value, "[ValueTime],[HourOfYear]," + F18, UnitNO);
                if (dtChart.Rows.Count > 0)
                {
                    if (rdoTotalHours.Checked == true)
                    {
                        chart1.Series[0].XValueMember = "HourOfYear";
                        chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                        dXMin = double.Parse(dtChart.Rows[0]["HourOfYear"].ToString());
                        dHourOfYear = double.Parse(dtChart.Rows[dtChart.Rows.Count - 1]["HourOfYear"].ToString());
                        chart1.ChartAreas[0].AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
                        chart1.ChartAreas[0].AxisX.MajorGrid.Interval = (dHourOfYear - dXMin) / 10;
                        chart1.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
                        chart1.ChartAreas[0].AxisX.Interval = (dHourOfYear - dXMin) / 10;
                        chart1.ChartAreas[0].AxisX.Title = "供热期累计小时数";
                        chart1.ChartAreas[0].AxisX.LabelStyle.Format = "0";
                    }
                    else
                    {
                        DateTime timeBegin = (DateTime)dtChart.Rows[0]["ValueTime"];
                        DateTime timeEnd = (DateTime)dtChart.Rows[dtChart.Rows.Count - 1]["ValueTime"];
                        chart1.Series[0].XValueMember = "ValueTime";
                        chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                        chart1.ChartAreas[0].AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Hours;
                        chart1.ChartAreas[0].AxisX.MajorGrid.Interval = (timeEnd - timeBegin).TotalHours / 6;
                        chart1.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Hours;
                        chart1.ChartAreas[0].AxisX.Interval = (timeEnd - timeBegin).TotalHours / 6;
                        chart1.ChartAreas[0].AxisX.Title = "日期时间";
                        chart1.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd HH:mm";
                    }
                    chart1.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Far;
                    chart1.ChartAreas[0].AxisY.Title = "累计节标煤量：" + Math.Round(double.Parse(dtChart.Rows[dtChart.Rows.Count - 1][F18].ToString()), 2) + "（吨）";
                    chart1.DataSource = dtChart;
                    chart1.DataBind();
                    timerChartBind.Enabled = false;
                }
                else
                    MessageBox.Show("没有查询到数据！");

                sSql = "delete QueryAlarm where QueryDate='" + sTime + "'";
                try { SQLHelper.ExecuteSql(sSql); }
                catch { }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs(ex.ToString());
                MessageBox.Show("错误：" + ex.Message);
            }
            finally
            {
                lblQueryWait.Visible = false;
                lblQueryWait.Refresh();
            }
        }

        private void btnRealTime_Click(object sender, EventArgs e)
        {
            lTotalCount = TagLJValue.GetTotalCount(UnitNO) - 1;
            timerChartBind_Tick(null, null);
            timerChartBind.Enabled = true;
        }

        private void LJMLR_FormClosed(object sender, FormClosedEventArgs e)
        {
            dtChart.Dispose();
            dtChart = null;
            this.Dispose(true);
        }

        //导出 
        private void button1_Click(object sender, EventArgs e)
        {
            DataTable dt = dtChart.DefaultView.ToTable(false, "ValueTime", F18, "HourOfYear");
            string title = chart1.ChartAreas[0].AxisX.Title;
            dt.Columns[0].ColumnName = "时间";
            dt.Columns[1].ColumnName = "累计节标煤量";
            dt.Columns[2].ColumnName = "小时数";
            bool b = NPOIExcelHelper.DataTableToExcel(dt, title, "累计节标煤量");
            if (b)
                MessageBox.Show("导出成功");
        }

        private void rdoTotalHours_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoTotalHours.Checked)
            {
                if (timerChartBind.Enabled == true)
                {
                    lTotalCount = TagLJValue.GetTotalCount(UnitNO) - 1;
                    timerChartBind_Tick(null, null);
                }
                else
                    btnQuery_Click(null, null);
            }
        }

        private void rdoDataTime_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDataTime.Checked)
            {
                if (timerChartBind.Enabled == true)
                {
                    lTotalCount = TagLJValue.GetTotalCount(UnitNO) - 1;
                    timerChartBind_Tick(null, null);
                }
                else
                    btnQuery_Click(null, null);
            }
        }
    }
}
