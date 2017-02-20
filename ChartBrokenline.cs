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

namespace KSPrj
{
    public partial class ChartBrokenline : Form
    {
        int UnitNO = 5;
        public ChartBrokenline(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
        }

        private void ChartBrokenline_Load(object sender, EventArgs e)
        {
            InitCharts();
            Chart1Bind();
            this.timer1.Interval = 500;
            this.timer1.Enabled = true;
        }

        /// <summary>
        /// 初始化左下角Chart图表
        /// </summary>
        private void InitCharts()
        {  

            //Chart3 绘折线图 
            ChartTools ct3 = new ChartTools(this.chart1);
            this.chart1.BackColor = Color.Black;
            ct3.SetChartArea(Color.Transparent);

            ct3.SetAxisX(0, 800, 100, "低压缸排汽流量（t/h）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            ct3.SetAxisY(0, 70, 10, "背压（绝对kPa）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            ct3.SetAxisY2(24.5, 94.5, 10, "真空（kPa）", StringAlignment.Near, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Dash, AxisArrowStyle.None, true, "");

            this.chart1.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chart1.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chart1.ChartAreas[0].AxisY2.TitleFont = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            //ct3.SetAxisX2(120, 190, 5, "低压缸排气流量（绝对）", StringAlignment.Far, false, Color.DarkGray, Color.DarkGray, Color.DarkGray, Color.DarkGray, ChartDashStyle.Solid, AxisArrowStyle.Triangle, "");

            //阴影
            ct3.SetSeries("", new double[] { 0, 800 }, new double[] { 70, 70 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Red, 0, "");
            ct3.SetSeries("", new double[] { 0, 191, 556.5, 800 }, new double[] { 25, 25, 48, 48 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.DarkOrange, 0, "");
            ct3.SetSeries("", new double[] { 0, 191, 556.5, 800 }, new double[] { 20, 20, 43, 43 }, SeriesChartType.Area, ChartHatchStyle.Percent50, Color.Green, 0, "");

            //两条直线 红色 粉色  

            ct3.SetSeries("TJD", new double[] { 0, 800 }, new double[] { 65, 65 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct3.SetSeries("", new double[] { 0, 800 }, new double[] { 33, 33 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Yellow, 2, "");
            this.chart1.Series["TJD"].Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chart1.Series["TJD"].Points[0].Label = "跳机点：#VALY" + "KP                 ";
            
            //两条红色折线 
            ct3.SetSeries("", new double[] { 0, 191, 556.5, 800 }, new double[] { 20, 20, 43, 43 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            ct3.SetSeries("", new double[] { 0, 191, 556.5, 800 }, new double[] { 25, 25, 48, 48 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");

            ct3.SetLegend(false, StringAlignment.Far, StringAlignment.Far);

            //Chart3 数据绑定Series 
            Series series3 = new Series();
            series3.Name = "chart3Series";
            series3.LabelForeColor = Color.White;
            series3.ChartType = SeriesChartType.Point;
            series3.MarkerSize = 16;
            series3.MarkerStyle = MarkerStyle.Circle;
            series3.Label = "#VALX{N2} #VAL{N2}";
            series3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            series3.Color = Color.White;
            chart1.Series.Add(series3);
        }

        /// <summary>
        /// 折线图表 绑定
        /// </summary>
        private void Chart1Bind()
        {
            //if (GlobalVariables.dtChartDatas == null || GlobalVariables.dtChartDatas.Rows.Count < 1)
            //    return;
            double pointX = 550;
            double pointY = 25;
            string dygpqll = ""; //低压缸排气流量1324/2324
            double dqy = 0.1; //大气压 
            //string dygjqyl = "--"; //低压缸进汽压力

            //int rowCount = GlobalVariables.dtChartDatas.Rows.Count - 1;
            //if (GlobalVariables.UnitNumber == 1)
            //{
            //    try
            //    {
            //        //这里添加#1机组的XY坐标  
            //        pointX = double.Parse(GlobalVariables.dtChartDatas.Rows[rowCount]["F1169"].ToString());
            //        //pointY = double.Parse(CalDataTable.Rows[0]["F3126"].ToString()) * 1000;
            //        dygpqll = GlobalVariables.dtChartDatas.Rows[rowCount]["F1324"].ToString();
            //        dygjqyl = CalTree.GetShowValue(1111, "MPa", "F3004");
            //    }
            //    catch { }
            //}
            //else
            //{
            //    try
            //    {
            //        //这里添加#2机组的XY坐标
            //        pointX = double.Parse(GlobalVariables.dtChartDatas.Rows[rowCount]["F2169"].ToString());
            //        //pointY = double.Parse(CalDataTable.Rows[0]["F3126"].ToString()) * 1000;
            //        dygpqll = GlobalVariables.dtChartDatas.Rows[rowCount]["F2324"].ToString();
            //        dygjqyl = CalTree.GetShowValue(2111, "MPa", "F3004");
            //    }
            //    catch { }
            //}
            //try
            //{
            //    pointY = double.Parse(GlobalVariables.dtChartDatas.Rows[rowCount]["F3126"].ToString()) * 1000;
            //    dqy = double.Parse(GlobalVariables.dtChartDatas.Rows[rowCount]["F3004"].ToString()) * 1000;
            //}
            //catch { }

            if (this.UnitNO == 5)
            {
                try
                {
                    //这里添加#1机组的XY坐标  
                    // X坐标修改成低压缸排汽流量 ZZH 20160830
                    pointX = TagValue.GetFinishedTagValueFive("F1324");// double.Parse(CalDataTable.Rows[0]["F1169"].ToString()); 
                    if (pointX == GlobalVariables.BadValue)
                    {
                        pointX = TagValue.GetSetValueFive("1324", "SetValue");
                    }
                    dygpqll = TagValue.GetShowValueFive(1324, "t/h", ""); // CalDataTable.Rows[0]["F1324"].ToString();
                    //dygjqyl = TagValue.GetShowValueFive(1111, "MPa", "");
                    //这里，如果不配置公共，需要修改 ZZH 2016-9-17
                    pointY = TagValue.GetFinishedTagValueFive("F3126") * 1000;//double.Parse(CalDataTable.Rows[0]["F3126"].ToString()) * 1000;
                    dqy = TagValue.GetFinishedTagValueFive("F3004") * 1000;//double.Parse(CalDataTable.Rows[0]["F3004"].ToString()) * 1000;
                    if (pointY == GlobalVariables.BadValue * 1000)
                    {
                        pointY = TagValue.GetSetValueFive("3126", "SetValue") * 1000;
                    }
                    if (dqy == GlobalVariables.BadValue * 1000)
                    {
                        dqy = TagValue.GetSetValueFive("3004", "SetValue") * 1000;
                    }
                }
                catch { }
            }
            else if (this.UnitNO == 6)
            {
                try
                {
                    //这里添加#2机组的XY坐标
                    pointX = TagValue.GetFinishedTagValueSix("F2324");//double.Parse(CalDataTable.Rows[0]["F2169"].ToString()); 
                    if (pointX == GlobalVariables.BadValue)
                    {
                        pointX = TagValue.GetSetValueSix("2324", "SetValue");
                    }
                    dygpqll = TagValue.GetShowValueSix(2324, "t/h", ""); //CalDataTable.Rows[0]["F2324"].ToString(); 
                    //dygjqyl = TagValue.GetShowValueSix(2111, "MPa", "");
                    //这里，如果不配置公共，需要修改 ZZH 2016-9-17
                    pointY = TagValue.GetFinishedTagValueSix("F4126") * 1000;//double.Parse(CalDataTable.Rows[0]["F3126"].ToString()) * 1000;
                    dqy = TagValue.GetFinishedTagValueSix("F4004") * 1000;//double.Parse(CalDataTable.Rows[0]["F3004"].ToString()) * 1000;
                    if (pointY == GlobalVariables.BadValue * 1000)
                    {
                        pointY = TagValue.GetSetValueSix("4126", "SetValue") * 1000;
                    }
                    if (dqy == GlobalVariables.BadValue * 1000)
                    {
                        dqy = TagValue.GetSetValueSix("4004", "SetValue") * 1000;
                    }
                }
                catch { }
            }  
             
            

            //绑定低压缸排气流量和相对容积流量
            this.label27.Text = dygpqll;
            this.label28.Text = pointY + " KPa";
            //this.label1.Text = dygjqyl; //低压缸进气压力 
            //真空 F3004*1000 - F3126*1000
            this.label43.Text = Math.Round(dqy - pointY, 2) + " KPa";
            try
            {
                //Y2轴 真空绑定
                double max = dqy - 0;
                double min = dqy - 70;
                this.chart1.ChartAreas[0].AxisY2.Minimum = min;
                this.chart1.ChartAreas[0].AxisY2.Maximum = max;
            }
            catch { }


            //闪点绑定
            this.chart1.Series["chart3Series"].Points.Clear();
            this.chart1.Series["chart3Series"].Points.AddXY(pointX, pointY);
            this.chart1.Series["chart3Series"].Label = "低压缸排气流量：" + dygpqll + "\r\n真        空：" + (dqy - pointY) + "KPa";
            //this.chart1.Series["chart3Series"].Label = "低压缸排气流量：" + dygpqll + "\r\n低压缸进汽压力：" + dygjqyl + "\r\n真        空：" + (dqy - pointY) + "KPa";

        }

        int count = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.chart1.Series["chart3Series"].Color != Color.Blue)
            {
                this.chart1.Series["chart3Series"].Color = Color.Blue;
            }
            else
            {
                this.chart1.Series["chart3Series"].Color = Color.Red;
            } 

            count++;
            
            try
            {
                if (count > 19) { 
                    Chart1Bind();
                    count = 0;
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs("折线图表："+ex.ToString());
                
            }
        }

    }
}
 