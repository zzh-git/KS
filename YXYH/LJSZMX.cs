using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinHost;
using HAOCommon;
using System.Windows.Forms.DataVisualization.Charting;
using YHYXYH.YXYH;

namespace KSPrj.YXYH
{
    public partial class LJSZMX : Form
    {
        //这里注释掉的都要看一遍 ZZH 2016-9-17 
        int UnitNO = 5;
        string F10, F6, F2, F14, F18, F19, F3225;
        string[] sYValueMembers = new string[6]; //{ F10, F6, F2, F14, F18, F19 };
        public LJSZMX(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
            if (UnitNO == 5)
            {
                F10 = "F10";
                F6 = "F6";
                F2 = "F2";
                F14 = "F14";
                F18 = "F18";
                F19 = "F19";
                F3225 = "F3225";
                sYValueMembers[0] = "F10";
                sYValueMembers[1] = "F6";
                sYValueMembers[2] = "F2";
                sYValueMembers[3] = "F14";
                sYValueMembers[4] = "F18";
                sYValueMembers[5] = "F19";
                sFields = "F2,F6,F10,F14,F18,F19";
            }
            else if (UnitNO == 6)
            {
                F10 = "F110";
                F6 = "F106";
                F2 = "F102";
                F14 = "F114";
                F18 = "F118";
                F19 = "F119";
                F3225 = "F4225";
                sYValueMembers[0] = "F110";
                sYValueMembers[1] = "F106";
                sYValueMembers[2] = "F102";
                sYValueMembers[3] = "F114";
                sYValueMembers[4] = "F118";
                sYValueMembers[5] = "F119";
                sFields = "F102,F106,F110,F114,F118,F119";
            }
        }

        long lTotalCount = -1;
        double dHourOfYear = 0;
        double dXMin = 0;
        byte bHourOfYearGridCount = 6;
        byte bDateTimeGridCount = 3;
        string[] sChartNames = { "chartRLLJZC", "chartGDLJSR", "chartGRLJSR", "chartLJMLR", "chartLJJML", "chartLJJSFY" };

        string sFields = "";
        string[] sYValueLabels = { "燃料累计支出：{0:0.00}万元", "供电累计收入：{0:0.00}万元", "供热累计收入：{0:0.00}万元", "累计毛利润：{0:0.00}万元", "累计节标煤量：{0:0.00}吨", "累计节省费用：{0:0.00}万元" };
        DataTable dtChart = null;
        Chart chartTemp = null;

        private void pnlData_Resize(object sender, EventArgs e)
        {
            pnlDataUp.Height = pnlData.Height / 2;
        }

        private void pnlDataUp_Resize(object sender, EventArgs e)
        {
            chartRLLJZC.Width = pnlDataUp.Width / 3;
            chartGRLJSR.Width = chartRLLJZC.Width;
        }

        private void pnlDataDown_Resize(object sender, EventArgs e)
        {
            chartLJMLR.Width = pnlDataDown.Width / 3;
            chartLJJSFY.Width = chartLJMLR.Width;
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

        private void LJSZMX_Load(object sender, EventArgs e)
        {

            for (int i = 0; i < sChartNames.Length; i++)
            {
                chartTemp = (Chart)pnlData.Controls.Find(sChartNames[i], true)[0];

                chartTemp.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                chartTemp.Series[0].XValueMember = "HourOfYear";
                chartTemp.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                chartTemp.Series[0].YValueMembers = sYValueMembers[i];
                chartTemp.ChartAreas[0].AxisY.LabelStyle.Format = "0.00";
                chartTemp.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Far;
            }

            timerChartBind.Interval = 1000;
            timerChartBind.Enabled = true;
            dtChart = TagLJValue.GetTotalDataWithNewTable(UnitNO); //这些注释都要改过来 ZZH 
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
                        dXMin = double.Parse(dtChart.Rows[0]["HourOfYear"].ToString());

                        for (int i = 0; i < sChartNames.Length; i++)
                        {
                            chartTemp = (Chart)pnlData.Controls.Find(sChartNames[i], true)[0];
                            chartTemp.Series[0].XValueMember = "HourOfYear";
                            chartTemp.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                            chartTemp.ChartAreas[0].AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
                            chartTemp.ChartAreas[0].AxisX.MajorGrid.Interval = (int)(dHourOfYear - dXMin) / bHourOfYearGridCount;
                            chartTemp.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
                            chartTemp.ChartAreas[0].AxisX.Interval = (int)(dHourOfYear - dXMin) / bHourOfYearGridCount;
                            chartTemp.ChartAreas[0].AxisX.Title = "供热期累计小时数(h)";
                            chartTemp.ChartAreas[0].AxisX.LabelStyle.Format = "0";
                            //chartTemp.ChartAreas[0].AxisY.Title = string.Format(sYValueLabels[i], Math.Round(double.Parse(dtChart.Rows[dtChart.Rows.Count - 1][sYValueMembers[i]].ToString()), 2));
                            chartTemp.Series[0].LegendText = string.Format(sYValueLabels[i], Math.Round(double.Parse(dtChart.Rows[dtChart.Rows.Count - 1][sYValueMembers[i]].ToString()), 2));
                        }

                    }
                    else
                    {
                        DateTime timeBegin = (DateTime)dtChart.Rows[0]["ValueTime"];
                        DateTime timeEnd = (DateTime)dtChart.Rows[dtChart.Rows.Count - 1]["ValueTime"];

                        for (int i = 0; i < sChartNames.Length; i++)
                        {
                            chartTemp = (Chart)pnlData.Controls.Find(sChartNames[i], true)[0];
                            chartTemp.Series[0].XValueMember = "ValueTime";
                            chartTemp.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                            chartTemp.ChartAreas[0].AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Hours;
                            chartTemp.ChartAreas[0].AxisX.MajorGrid.Interval = (int)(timeEnd - timeBegin).TotalHours / bDateTimeGridCount;
                            chartTemp.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Hours;
                            chartTemp.ChartAreas[0].AxisX.Interval = (int)(timeEnd - timeBegin).TotalHours / bDateTimeGridCount;
                            chartTemp.ChartAreas[0].AxisX.Title = "日期时间(t)";
                            chartTemp.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd HH:mm";
                            //chartTemp.ChartAreas[0].AxisY.Title = string.Format(sYValueLabels[i], Math.Round(double.Parse(dtChart.Rows[dtChart.Rows.Count - 1][sYValueMembers[i]].ToString()), 2));
                            chartTemp.Series[0].LegendText = string.Format(sYValueLabels[i], Math.Round(double.Parse(dtChart.Rows[dtChart.Rows.Count - 1][sYValueMembers[i]].ToString()), 2));
                        }
                    }
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
            MinMaxValue mmv = TagLJValue.GetMinMaxValue(F3225, UnitNO);
            //chartRLLJZC.ChartAreas[0].AxisX.Title = "供热期累计小时数：" + Math.Round(dHourOfYear, 4).ToString("0.0000")
            //    + "                累计开始时间：" + mmv.BeginDateString + "\n燃料支出最小值："
            //    + Math.Round(mmv.MinValue / 10000, 4).ToString("0.0000") + "        燃料支出最小值时间：" + mmv.MinDateString
            //    + "\n燃料支出最大值：" + Math.Round(mmv.MaxValue / 10000, 4).ToString("0.0000") + "        燃料支出最大值时间："
            //    + mmv.MaxDateString + "\n燃料支出平均值：" + TagLJValue.GetTotalDataAvg(F10, UnitNO) + "万元/小时";
            lblMsg.Text = "供热期累计小时数：" + Math.Round(dHourOfYear, 4).ToString("0.0000") + "        累计开始时间：" + mmv.BeginDateString + "        双击每个图可以放大显示。";
            dtChart.Rows.Add(row);

            foreach (string sChartName in sChartNames)
            {
                chartTemp = (Chart)pnlData.Controls.Find(sChartName, true)[0];
                chartTemp.DataSource = dtChart;
                chartTemp.DataBind();
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {

                lblQueryWait.Visible = true;
                lblQueryWait.Refresh();
                string sTime = PublicFunction.DateTimeToStringWithfff(DateTime.Now);
                string sSql = "insert into QueryAlarm(QueryDate,StartTime,EndTime,QueryFields,UnitNO,AlarmDesc) values('"
                    + sTime + "','" + dateTimePickerBegin2.Value + "','" + dateTimePickerEnd2.Value + "','" + sFields + "',"
                    + UnitNO + ",'查询数据时，未按提示等待，做其他操作造成程序退出！')";
                try { SQLHelper.ExecuteSql(sSql); }
                catch { }

                dtChart = TagLJValue.QueryDBTotalData(dateTimePickerBegin2.Value, dateTimePickerEnd2.Value, "[ValueTime],[HourOfYear]," + sFields, UnitNO);
                if (dtChart.Rows.Count > 0)
                {
                    if (rdoTotalHours.Checked == true)
                    {
                        dXMin = double.Parse(dtChart.Rows[0]["HourOfYear"].ToString());
                        dHourOfYear = double.Parse(dtChart.Rows[dtChart.Rows.Count - 1]["HourOfYear"].ToString());

                        for (int i = 0; i < sChartNames.Length; i++)
                        {
                            chartTemp = (Chart)pnlData.Controls.Find(sChartNames[i], true)[0];
                            chartTemp.Series[0].XValueMember = "HourOfYear";
                            chartTemp.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
                            chartTemp.ChartAreas[0].AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
                            chartTemp.ChartAreas[0].AxisX.MajorGrid.Interval = (int)(dHourOfYear - dXMin) / bHourOfYearGridCount;
                            chartTemp.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
                            chartTemp.ChartAreas[0].AxisX.Interval = (int)(dHourOfYear - dXMin) / bHourOfYearGridCount;
                            chartTemp.ChartAreas[0].AxisX.Title = "供热期累计小时数(h)";
                            chartTemp.ChartAreas[0].AxisX.LabelStyle.Format = "0";
                            //chartTemp.ChartAreas[0].AxisY.Title = string.Format(sYValueLabels[i], Math.Round(double.Parse(dtChart.Rows[dtChart.Rows.Count - 1][sYValueMembers[i]].ToString()), 2));
                            chartTemp.Series[0].LegendText = string.Format(sYValueLabels[i], Math.Round(double.Parse(dtChart.Rows[dtChart.Rows.Count - 1][sYValueMembers[i]].ToString()), 2));
                        }
                    }
                    else
                    {
                        DateTime timeBegin = (DateTime)dtChart.Rows[0]["ValueTime"];
                        DateTime timeEnd = (DateTime)dtChart.Rows[dtChart.Rows.Count - 1]["ValueTime"];

                        for (int i = 0; i < sChartNames.Length; i++)
                        {
                            chartTemp = (Chart)pnlData.Controls.Find(sChartNames[i], true)[0];
                            chartTemp.Series[0].XValueMember = "HourOfYear";
                            chartTemp.Series[0].XValueMember = "ValueTime";
                            chartTemp.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
                            chartTemp.ChartAreas[0].AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Hours;
                            chartTemp.ChartAreas[0].AxisX.MajorGrid.Interval = (int)(timeEnd - timeBegin).TotalHours / bDateTimeGridCount;
                            chartTemp.ChartAreas[0].AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Hours;
                            chartTemp.ChartAreas[0].AxisX.Interval = (int)(timeEnd - timeBegin).TotalHours / bDateTimeGridCount;
                            chartTemp.ChartAreas[0].AxisX.Title = "日期时间(t)";
                            chartTemp.ChartAreas[0].AxisX.LabelStyle.Format = "yyyy-MM-dd HH:mm";
                            //chartTemp.ChartAreas[0].AxisY.Title = string.Format(sYValueLabels[i], Math.Round(double.Parse(dtChart.Rows[dtChart.Rows.Count - 1][sYValueMembers[i]].ToString()), 2));
                            chartTemp.Series[0].LegendText = string.Format(sYValueLabels[i], Math.Round(double.Parse(dtChart.Rows[dtChart.Rows.Count - 1][sYValueMembers[i]].ToString()), 2));
                        }
                    }
                    chartRLLJZC.ChartAreas[0].AxisX.TitleAlignment = StringAlignment.Far;
                    chartRLLJZC.ChartAreas[0].AxisY.Title = "燃料累计支出：" + Math.Round(double.Parse(dtChart.Rows[dtChart.Rows.Count - 1][F10].ToString()), 2) + "（万元）";
                    chartRLLJZC.DataSource = dtChart;
                    chartRLLJZC.DataBind();
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

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable dt = dtChart.DefaultView.ToTable(false, "ValueTime", F10, F6, F2, F14, F18, F19, "HourOfYear");
            string title = lblMsg.Text.Substring(0, lblMsg.Text.IndexOf("双击") - 8); ;
            dt.Columns[0].ColumnName = "时间";
            dt.Columns[1].ColumnName = "燃料累计支出（万元）";
            dt.Columns[2].ColumnName = "供电累计收入（万元）";
            dt.Columns[3].ColumnName = "供热累计收入（万元）";
            dt.Columns[4].ColumnName = "累计毛利润（万元）";
            dt.Columns[5].ColumnName = "累计节标煤量（吨）";
            dt.Columns[6].ColumnName = "累计节省费用（万元）";
            dt.Columns[7].ColumnName = "累计小时数";
            bool b = NPOIExcelHelper.DataTableToExcel(dt, title, "收支明细");
            if (b)
                MessageBox.Show("导出成功");
        }

        //累计小时数 
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

        //日期时间 
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

        private void chartLJJSFY_DoubleClick(object sender, EventArgs e)
        {
            (new LJJSFY(UnitNO)).Show();
        }

        private void chartLJJML_DoubleClick(object sender, EventArgs e)
        {
            (new LJJML(UnitNO)).Show();
        }


        private void chartLJMLR_DoubleClick(object sender, EventArgs e)
        {
            (new LJMLR(UnitNO)).Show();
        }

        private void chartRLLJZC_DoubleClick(object sender, EventArgs e)
        {
            (new RLZCFY(UnitNO)).Show();
        }

        private void chartGDLJSR_DoubleClick(object sender, EventArgs e)
        {
            (new GDLJSR(UnitNO)).Show();
        }

        private void chartGRLJSR_DoubleClick(object sender, EventArgs e)
        {
            (new GRLJSR(UnitNO)).Show();
        }
    }
}
