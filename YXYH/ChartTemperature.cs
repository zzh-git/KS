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

namespace KSPrj.YXYH
{
    public partial class ChartTemperature : Form
    {
        public ChartTemperature()
        {
            InitializeComponent();
            this.timer1.Interval = 500;
            this.timer1.Enabled = true;
            InitValues();
            InitChart();
            BindData();
        }

        //热网出水温度最低和最高值（X轴最大最小值）
        int min = 35;
        int max = 95;
        //K 和 B 的值
        double k = -0.8889;
        double greenB1 = 59.454;
        double redB1 = 58.1165;
        double greenB2 = 54.1025;
        private void InitValues() 
        {
            //这里也要区分五六号机 ZZH 
            DataTable dt = FTableTags.getDataTableByRowFilter("[id] in (3393, 3394, 3395, 3396)");
            k = double.Parse(dt.Rows[0]["SetValue"].ToString()); 
            greenB1 = double.Parse(dt.Rows[1]["SetValue"].ToString());
            greenB2 = double.Parse(dt.Rows[3]["SetValue"].ToString());
            redB1 = double.Parse(dt.Rows[2]["SetValue"].ToString());
        }
        private double GetValues(double k, double b, double x) 
        {
            return k * x + b;
        }


        //初始化图表 
        private void InitChart() 
        {
            ChartTools ct = new ChartTools(this.chart1);
            this.chart1.ChartAreas[0].BackColor = Color.Black;
            //X Y 轴
            ct.SetAxisX(35, 95, 5, "热网出水温度（摄氏度）", StringAlignment.Far, false, Color.Gray, Color.Gray, Color.Gray, Color.Gray, ChartDashStyle.Dash, AxisArrowStyle.Lines, "");
            ct.SetAxisY(-35, 35, 5, "环境温度（摄氏度）", StringAlignment.Far, false, Color.Gray, Color.Gray, Color.Gray, Color.Gray, ChartDashStyle.Dash, AxisArrowStyle.Lines, "");
            //两条绿线 (上绿线 y = -0.8889x + 59.4545)
            // (下绿线 y = -0.8889x + 54.1025)
            ct.SetSeries("", new double[] { min, max }, new double[] { GetValues(k, greenB1, min), GetValues(k, greenB1, max) }, SeriesChartType.Line, ChartHatchStyle.None, Color.Green, 2, "");
            ct.SetSeries("", new double[] { min, max }, new double[] { GetValues(k, greenB2, min), GetValues(k, greenB2, max) }, SeriesChartType.Line, ChartHatchStyle.None, Color.Green, 2, "");
            //红线 (y = -0.8889x + 58.1165)
            ct.SetSeries("", new double[] { min, max }, new double[] { GetValues(k, redB1, min), GetValues(k, redB1, max) }, SeriesChartType.Line, ChartHatchStyle.None, Color.Red, 2, "");

            chart1.ChartAreas[0].AxisX.TitleFont = Font;
            chart1.ChartAreas[0].AxisX.LabelStyle.Font = Font;
            chart1.ChartAreas[0].AxisY.TitleFont = Font;
            chart1.ChartAreas[0].AxisY.LabelStyle.Font = Font;
            Series series3 = new Series();
            series3.Name = "chart3Series";
            series3.LabelForeColor = Color.White;
            series3.ChartType = SeriesChartType.Point;
            series3.MarkerSize = 16;
            series3.MarkerStyle = MarkerStyle.Circle;
            series3.Label = "#VALX #VAL";
            series3.Font = Font;
            series3.LabelForeColor = Color.Silver;
            //series3.BorderWidth = 2;
            series3.Color = Color.White;
            chart1.Series.Add(series3);
            

            ct.SetLegend(false, StringAlignment.Far, StringAlignment.Far);
        }

        private void BindData() 
        {
            double xValue = 0; //3507 
            double yValue = 0; //3208 
            int rowCount = GlobalVariables.dtChartDataFive.Rows.Count - 1;
            //这里注意区分五六号机 ZZH 
            try
            {
                xValue = double.Parse(GlobalVariables.dtChartDataFive.Rows[rowCount]["F3507"].ToString());
                yValue = double.Parse(GlobalVariables.dtChartDataFive.Rows[rowCount]["F3208"].ToString());
            }
            catch  { } 

            this.chart1.Series["chart3Series"].Points.Clear();
            this.chart1.Series["chart3Series"].Points.AddXY(xValue, yValue);
            this.chart1.Series["chart3Series"].Label = "热网出水温度：" + xValue.ToString("0.0") + "℃\n环境温度：" + yValue.ToString("0.0")+"℃";
            
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
                    BindData();
                    count = 0;
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs("温度图表：" + ex.ToString()); 
            }
        }

    }
}
