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
    public partial class ChartCQXZ : Form
    {
        int UnitNO = 5;
        public ChartCQXZ(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
            InitCharts();
            this.timer1.Interval = 500;
            this.timer1.Enabled = true;
            Chart1Bind();
        }


        private void InitCharts()
        {
            
            ChartTools ct2 = new ChartTools(this.chart1);
            this.chart1.BackColor = Color.Black;
            ct2.SetChartArea(Color.Transparent);
            ct2.SetAxisX(100, 900, 100, "排汽流量（t/h）", StringAlignment.Center, false, Color.LightGray, Color.LightGray, Color.LightGray, Color.LightGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");
            ct2.SetAxisY(400, 1300, 100, "主汽流量（t/h）", StringAlignment.Center, false, Color.LightGray, Color.LightGray, Color.LightGray, Color.LightGray, ChartDashStyle.Solid, AxisArrowStyle.None, "");

            this.chart1.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chart1.ChartAreas[0].AxisY.TitleFont = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

            //点 从左到右 
            ct2.SetSeries("", new double[] { 270 }, new double[] { 1170 }, SeriesChartType.Point, ChartHatchStyle.None, Color.White, 0, "550");
            ct2.SetSeries("", new double[] { 365 }, new double[] { 1170 }, SeriesChartType.Point, ChartHatchStyle.None, Color.White, 0, "450");
            ct2.SetSeries("", new double[] { 410 }, new double[] { 1170 }, SeriesChartType.Point, ChartHatchStyle.None, Color.White, 0, "400");
            ct2.SetSeries("", new double[] { 506 }, new double[] { 1170 }, SeriesChartType.Point, ChartHatchStyle.None, Color.White, 0, "300");
            ct2.SetSeries("", new double[] { 598 }, new double[] { 1170 }, SeriesChartType.Point, ChartHatchStyle.None, Color.White, 0, "200");
            ct2.SetSeries("", new double[] { 690 }, new double[] { 1170 }, SeriesChartType.Point, ChartHatchStyle.None, Color.White, 0, "100");
            ct2.SetSeries("", new double[] { 787 }, new double[] { 1170 }, SeriesChartType.Point, ChartHatchStyle.None, Color.White, 0, "0");

            //七条斜线 从左至右
            ct2.SetSeries("", new double[] { 220, 270 }, new double[] { 1080, 1170 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Yellow, 2, "");
            ct2.SetSeries("", new double[] { 247, 365 }, new double[] { 960, 1170 }, SeriesChartType.Line, ChartHatchStyle.None, Color.LightGray, 2, "");
            ct2.SetSeries("", new double[] { 267, 410 }, new double[] { 920, 1170 }, SeriesChartType.Line, ChartHatchStyle.None, Color.LightGray, 2, "");
            ct2.SetSeries("", new double[] { 312, 506 }, new double[] { 830, 1170 }, SeriesChartType.Line, ChartHatchStyle.None, Color.LightGray, 2, "");
            ct2.SetSeries("", new double[] { 355, 598 }, new double[] { 740, 1170 }, SeriesChartType.Line, ChartHatchStyle.None, Color.LightGray, 2, "");
            ct2.SetSeries("", new double[] { 395, 690 }, new double[] { 660, 1170 }, SeriesChartType.Line, ChartHatchStyle.None, Color.LightGray, 2, "");
            ct2.SetSeries("", new double[] { 430, 787 }, new double[] { 590, 1170 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Yellow, 2, "");

            //红点线
            ct2.SetSeries("", new double[] { 220, 247 }, new double[] { 1080, 960 }, SeriesChartType.Spline, ChartHatchStyle.None, Color.Red, 2, "");
            ct2.SetSeries("", new double[] { 247, 430 }, new double[] { 960, 590 }, SeriesChartType.Spline, ChartHatchStyle.None, Color.Red, 2, "");
            //蓝点线
            ct2.SetSeries("", new double[] { 330, 330 }, new double[] { 1170, 970 }, SeriesChartType.Spline, ChartHatchStyle.None, Color.Orange, 2, "");
            ct2.SetSeries("", new double[] { 330, 510 }, new double[] { 970, 720 }, SeriesChartType.Spline, ChartHatchStyle.None, Color.Orange, 2, "");


            //一条直线 
            ct2.SetSeries("", new double[] { 100, 787 }, new double[] { 1170, 1170 }, SeriesChartType.Line, ChartHatchStyle.None, Color.Yellow, 2, "");

            ct2.SetLegend(false, StringAlignment.Far, StringAlignment.Far);

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
        ///  Y轴  F1019, F2019主汽流量       X轴 F1324, F2324低压缸排汽流量
        /// </summary>
        private void Chart1Bind()
        { 
            double pointX = 0;
            double pointY = 0; 
            
            if (this.UnitNO == 5)
            {
                try
                { 
                    pointX = TagValue.GetFinishedTagValueFive("F1324");
                    if (pointX == GlobalVariables.BadValue)
                    {
                        pointX = TagValue.GetSetValueFive("1324", "SetValue");
                    }
                    pointY = TagValue.GetFinishedTagValueFive("F1019");
                    if (pointY == GlobalVariables.BadValue)
                    {
                        pointY = TagValue.GetSetValueFive("1019", "SetValue");
                    } 
                }
                catch { }
            }
            else if (this.UnitNO == 6)
            {
                try
                {
                    pointX = TagValue.GetFinishedTagValueSix("F2324");
                    if (pointX == GlobalVariables.BadValue)
                    {
                        pointX = TagValue.GetSetValueSix("2324", "SetValue");
                    }
                    pointY = TagValue.GetFinishedTagValueSix("F2019");
                    if (pointY == GlobalVariables.BadValue)
                    {
                        pointY = TagValue.GetSetValueSix("2019", "SetValue");
                    }
                }
                catch { }
            } 
            
            //闪点绑定
            this.chart1.Series["chart3Series"].Points.Clear();
            this.chart1.Series["chart3Series"].Points.AddXY(pointX, pointY);
            this.chart1.Series["chart3Series"].Label = "低压缸排汽流量：" + pointX + "\n\n      主汽流量：" + pointY;
            
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
                if (count > 19)
                {
                    Chart1Bind();
                    count = 0;
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs("抽汽工况图：" + ex.ToString());

            }
        }


    }
}
