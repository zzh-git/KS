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
    public partial class ChartParabola : Form
    {
        int UnitNO = 5;
        public ChartParabola(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
        }

        private void ChartParabola_Load(object sender, EventArgs e)
        {
            InitCharts();
            Chart2Bind();
            this.timer1.Interval = 500;
            this.timer1.Enabled = true;
        }

        /// <summary>
        /// 初始化左下角Chart图表
        /// </summary>
        private void InitCharts()
        { 
            //Chart2 绘抛物线图 
            ChartTools ct2 = new ChartTools(this.chart1);
            this.chart1.BackColor = Color.Black;
            ct2.SetChartArea(Color.Transparent);
            ct2.SetAxisX(0, 1.0, 0.1, "相对容积流量", StringAlignment.Far, false, Color.LightGray, Color.LightGray, Color.LightGray, Color.LightGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            ct2.SetAxisY(0, 1.4, 0.2, "叶片相对动应力", StringAlignment.Far, false, Color.LightGray, Color.LightGray, Color.LightGray, Color.LightGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");

            this.chart1.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chart1.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

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
            //末级叶片零功率
            //ct2.SetSeries("MJYPLGL", new double[] { 0.4479, 0.4479 }, new double[] { 0, 1.4 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Blue, 2, "");
            //this.chart1.Series["MJYPLGL"].Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            //this.chart1.Series["MJYPLGL"].Points[1].Label = "末级叶片零功率";
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
            series2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            chart1.Series.Add(series2);
             
        }

        /// <summary>
        /// 抛物线图表 绑定
        /// </summary>
        private void Chart2Bind()
        {
            double xValue = 0.33;
            if(this.UnitNO == 5)
            {
                if (GlobalVariables.dtOneRowDataFive == null || GlobalVariables.dtOneRowDataFive.Rows.Count < 1)
                    return;
                xValue = TagValue.GetFinishedTagValueFive("F1329");
            } 
            else if (this.UnitNO == 6)
            {
                if (GlobalVariables.dtOneRowDataSix == null || GlobalVariables.dtOneRowDataSix.Rows.Count < 1)
                    return;
                xValue = TagValue.GetFinishedTagValueSix("F1329");
            }
            
            this.chart1.Series["chart2Series"].Points.Clear();
            this.chart1.Series["chart2Series"].Points.AddXY(xValue, 1.4);
            this.chart1.Series["chart2Series"].Points.AddXY(xValue, 0);
            this.chart1.Series["chart2Series"].Points[0].Label = xValue + "";
        }

        int count = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            count++;
            try
            {
                if (this.chart1.Series["chart2Series"].Color == Color.White)
                    this.chart1.Series["chart2Series"].Color = Color.Black;
                else
                    this.chart1.Series["chart2Series"].Color = Color.White;

                if (count > 19)
                {
                    Chart2Bind();
                    count = 0;
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs("抛物线图表："+ ex.ToString());
            }
            
        }


    }
}
