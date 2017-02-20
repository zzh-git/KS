using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HAOCommon;
using WinHost;

namespace KSPrj
{
    public partial class FrmShowHis : Form
    {
        /// <summary>
        /// 待过滤的TagID BY ZZH 
        /// </summary>
        public string stempTags = "";
        /// <summary>
        /// Tag描述/Tag ID
        /// </summary>
        public string[] stempSTags;
        public DataTable tagDt;
        /// <summary>
        /// 是否还原至主界面
        /// </summary>
        public bool ismin = false;
        public bool isfromZhu = true;
        int UnitNO = 5;
        public FCZForm fczMain;
        public FrmShowHis(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
            timer1.Interval = 10 * 1000;
        }
        private void FrmShowHis_Load(object sender, EventArgs e)
        {
            if (isfromZhu)
            {
                btnSetTag.Visible = false;
                timer1.Enabled = true;
                this.button5.Enabled = false;
            }
            else
            {
                btnSetTag.Visible = true;
                timer1.Enabled = false;
            }
            string sFiler = "F3002";//默认展示3条限制值数据
            string[] sSTags = { "按压力计算/F3002" };
            if (string.IsNullOrEmpty(stempTags))
            {
                stempTags = sFiler;
                stempSTags = sSTags;
            }
            //BindChart(stempTags, stempSTags);
            BindChartSeries(stempTags, stempSTags);
            BindChartRefresh();
        }

        private void BindChartSeries(string sFiler, string[] sArFiler)
        {
            this.chart1.Series.Clear();
            string colname;
            for (int i = 0; i < sArFiler.Length; i++)
            {
                excellCell = sArFiler[i].Split('/')[1];
                excellCellName = sArFiler[i].Split('/')[0];

                colname = sArFiler[i].Split('/')[1];
                chart1.Series.Add(colname);
                chart1.Series[i].LegendText = sArFiler[i].Split('/')[0];// dt.Rows[i]["tagdesc"].ToString();
                chart1.Series[i].XValueType = ChartValueType.DateTime;
                chart1.Series[i].XValueMember = "ValueTime";
                chart1.Series[i].YValueType = ChartValueType.Double;
                chart1.Series[i].YValueMembers = sArFiler[i].Split('/')[1];
                chart1.Series[i].ChartType = SeriesChartType.Line;// SeriesChartType.Spline;
            }
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.Format = "MM-dd HH:mm:ss";
        }


        DataTable dtData = null; //存放数据 
        string excellCell = ""; //列名 FXXXX
        string excellCellName = ""; //列名

        //实时 
        private void BindChartRefresh()
        {
            try
            {
                if (this.UnitNO == 5)
                    dtData = GlobalVariables.dtChartDataFive;
                else if (this.UnitNO == 6)
                    dtData = GlobalVariables.dtChartDataSix;
                chart1.DataSource = dtData;
                try { this.chart1.DataBind(); }
                catch { }
            }
            catch (Exception e)
            {
                WriteLog.WriteLogs("展示历史曲线报错：" + e.ToString());
            }
        }

        //历史 
        private void BindChartHistory(string sFields)
        {
            try
            {
                string sTime = PublicFunction.DateTimeToStringWithfff(DateTime.Now);
                string sSql = "insert into QueryAlarm(QueryDate,StartTime,EndTime,QueryFields,UnitNO,AlarmDesc) values('" 
                    + sTime + "','" + dateTimePickerBegin2.Value + "','" + dateTimePickerEnd2.Value + "','" + sFields + "',"
                    + UnitNO + ",'查询数据时，未按提示等待，做其他操作造成程序退出！')";
                try { SQLHelper.ExecuteSql(sSql); }
                catch { }

                dtData = TagValue.QueryDBData(dateTimePickerBegin2.Value, dateTimePickerEnd2.Value, "ValueTime," + sFields, UnitNO);

                if (dtData.Rows.Count == 0)
                {
                    sSql = "delete QueryAlarm where QueryDate='" + sTime + "'";
                    try { SQLHelper.ExecuteSql(sSql); }
                    catch { }
                    MessageBox.Show("无数据！");
                    return;
                }
                chart1.DataSource = dtData;
                this.chart1.DataBind();

                sSql = "delete QueryAlarm where QueryDate='"+sTime+"'";
                try { SQLHelper.ExecuteSql(sSql); }
                catch { }
            }
            catch (Exception e)
            {
                WriteLog.WriteLogs("展示历史曲线报错：" + e.ToString());
            }

        }

        ////选择测点按钮
        //private void btnSetTag_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        FrmShowHisSelectTag fshs1 = new FrmShowHisSelectTag();
        //        fshs1.TopMost = true;    //窗体前端显示设为true
        //        fshs1.ShowDialog();
        //        ArrayList sTags = new ArrayList();
        //        sTags = fshs1.sTags;//返回待查询的数据
        //        stempTags = "";
        //        //若点击了取消按钮
        //        if (sTags.Count == 0)
        //            return;
        //        stempSTags = new string[sTags.Count];
        //        for (int ii = 0; ii < sTags.Count; ii++)
        //        {
        //            stempTags += "F" + ((BindTag.TagObject)sTags[ii]).id + ",";
        //            stempSTags[ii] = ((BindTag.TagObject)sTags[ii]).tagdesc + "/F" + ((BindTag.TagObject)sTags[ii]).id;
        //        }

        //        stempTags = stempTags.Substring(0, stempTags.Length - 1);
        //        fshs1.Dispose();
        //        //DateTime DtB = this.dtpBtime.Value;
        //        //DateTime DtE = this.dtpEtime.Value;

        //        //if (DtB >= DtE)//若不是同一天
        //        //{
        //        //    MessageBox.Show("开始时间必须小于结束时间！");
        //        //    return;
        //        //}
        //        //BindChart(stempTags, stempSTags);
        //        BindChartSeries(stempTags, stempSTags);
        //        BindChartHistory(stempTags);
        //    }
        //    catch (Exception ex)
        //    {
        //        int i = 0;
        //        throw;
        //    }


        //}

        private void chart1_GetToolTipText(object sender, System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs e)
        {
            if (e.HitTestResult.ChartElementType == System.Windows.Forms.DataVisualization.Charting.ChartElementType.DataPoint)
            {
                int i = e.HitTestResult.PointIndex;
                System.Windows.Forms.DataVisualization.Charting.DataPoint dp = e.HitTestResult.Series.Points[i];
                Object obj = dp.XValue;
                e.Text = string.Format("{0:F3}", dp.YValues[0]);
            }
        }
        /// <summary>
        /// 查询数据库中的测点数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns></returns>
        //public static DataTable QueryDBData(DateTime beginTime, DateTime endTime, string fields)
        //{
        //    DataTable dt = new DataTable();
        //    DataTable dtTableNames = SQLHelper.ExecuteDt("SELECT * FROM DataTableNames where DataDate>=" + DateControl.GetYearWeek(beginTime) +
        //        " and DataDate<=" + DateControl.GetYearWeek(endTime));
        //    if (dtTableNames.Rows.Count > 0)
        //    {
        //        if (fields == "")
        //            fields = "F1,F2,F3";
        //        string sSql = "";
        //        foreach (DataRow row in dtTableNames.Rows)
        //        {
        //            sSql += " union all   select ValueTime," + fields + " from " + row["TableName"] + " where ValueTime>='" + beginTime.ToString("yyyy-MM-dd HH:mm:ss") +
        //                "' and ValueTime<='" + endTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        //        }

        //        //加排序速度会慢很多
        //        //sSql = "select * from (" + sSql.Substring(10) + ") as t order by ValueTime";
        //        //dt = SQLHelper.ExecuteDt(sSql);

        //        //不加排序也会安时间来排序，因为时间字段设置成了表的主键
        //        dt = SQLHelper.ExecuteDt(sSql.Substring(10));
        //    }
        //    return dt;
        //}


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



        private void timer1_Tick(object sender, EventArgs e)
        {
            BindChartRefresh();
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                lblQueryWait.Visible = true;
                lblQueryWait.Refresh();
                timer2.Enabled = true;

                this.button5.Enabled = true;
                this.timer1.Enabled = false;
                BindChartHistory(stempTags);
            }
            finally
            {
                lblQueryWait.Visible = false;
                lblQueryWait.Refresh();
                timer2.Enabled = false;
            }
        }

        /// <summary>
        /// 还原至主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            ismin = true;

            fczMain.minLine();

            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// 实时曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            this.button5.Enabled = false;
            BindChartRefresh();
            this.timer1.Enabled = true;
            return;
        }

        //导出 
        private void button1_Click(object sender, EventArgs e)
        {
            if (dtData != null && dtData.Rows.Count > 0)
            {
                try
                {
                    DataTable dt = dtData.DefaultView.ToTable(false, new string[] { "ValueTime", excellCell });
                    dt.Columns[0].ColumnName = "时间";
                    dt.Columns[1].ColumnName = excellCellName;
                    bool b = NPOIExcelHelper.DataTableToExcel(dt, "", excellCellName);
                    if (b)
                        MessageBox.Show("保存成功");
                }
                catch
                { MessageBox.Show("保存失败"); }
            }

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
