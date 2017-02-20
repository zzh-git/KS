using HAOCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YHYXYH.YXYH
{
    public partial class SSCSSix : Form
    {
        //需要注意区分 ZZH 2016-9-17
        int UnitNO = 6;
        public SSCSSix(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
        }
        double m_dChart1Width = 892;//Chart1的宽度
        int[] m_iX1 = { 80, 156 };//第1个柱子的开始、结束的X坐标值。
        int[] m_iX2 = { 172, 248 };//第2个柱子的开始、结束的X坐标值。
        int[] m_iX3 = { 266, 341 };//第3个柱子的开始、结束的X坐标值。
        int[] m_iX4 = { 359, 435 };//第4个柱子的开始、结束的X坐标值。
        int[] m_iX5 = { 453, 528 };//第5个柱子的开始、结束的X坐标值。
        int[] m_iX6 = { 546, 622 };//第6个柱子的开始、结束的X坐标值。
        int[] m_iX7 = { 640, 715 };//第7个柱子的开始、结束的X坐标值。
        int[] m_iX8 = { 734, 810 };//第8个柱子的开始、结束的X坐标值。
        private void SSCS_Load(object sender, EventArgs e)
        {
            timerChartBind.Interval = GlobalVariables.RefIntvel;
            timerChartBind.Enabled = true;
            timerChartBind_Tick(null, null);
            //lblRemark.Text = "注：1、发电比即发电负荷与汽轮机吸收总热量之比，是本机组优化运行的一个重要指标，越高越好，设计最大值是39.5%；\n"
            //    + "    2、收入毛利润是经济收入的重要指标，越高越好，设计最大值是94000元/小时。双击每个柱状图可以查看历史数据。";
        }
        /// <summary>
        /// 把实际的Y坐标值换算成0到100的坐标值
        /// </summary>
        /// <param name="dMin">下限</param>
        /// <param name="dMax">上限</param>
        /// <param name="dCurrent">当前值</param>
        /// <returns></returns>
        double GetY(double dMin, double dMax, double dCurrent)
        {
            double dResult = 100 / (dMax - dMin) * (dCurrent - dMin);
            if (dResult < 1)
                dResult = 1;
            if (dResult > 100)
                dResult = 100;
            return dResult;
        }
        private void timerChartBind_Tick(object sender, EventArgs e)
        {
            double dGLRML = 0;//锅炉燃煤量（标煤）
            //if (GlobalVariables.UnitNumber == 1)
            //    dGLRML = WinHost.CalTree.GetFinishedTagValue("F1187");
            //else
            //    dGLRML = WinHost.CalTree.GetFinishedTagValue("F2187");
            dGLRML = WinHost.TagValue.GetFinishedTagValueSix("F4371");
            chart1.Series[0].Points[0].Label = "锅炉燃煤量(标煤)\n\n" + Math.Round(dGLRML, 2) + "吨/小时";
            chart1.Series[0].Points[0].YValues[0] = GetY(30, 120, dGLRML);
            double dRLZCFY = WinHost.TagValue.GetFinishedTagValueSix("F4225");//然料支出费用
            chart1.Series[0].Points[1].Label = "然料支出费用\n\n" + Math.Round(dRLZCFY) + "元/小时";
            chart1.Series[0].Points[1].YValues[0] = GetY(10000, 120000, dRLZCFY);
            double dSWDL = WinHost.TagValue.GetFinishedTagValueSix("F4224");//上网电量
            chart1.Series[0].Points[2].Label = "上网电量\n\n" + Math.Round(dSWDL * 1000) + "千瓦/小时";
            chart1.Series[0].Points[2].YValues[0] = GetY(100000, 300000, dSWDL * 1000);
            double dSWDFSR = WinHost.TagValue.GetFinishedTagValueSix("F4226");//上网电费收入
            chart1.Series[0].Points[3].Label = "上网电费收入\n\n" + Math.Round(dSWDFSR) + "元/小时";
            chart1.Series[0].Points[3].YValues[0] = GetY(10000, 120000, dSWDFSR);
            double dDWGRL = 0;//对外供热量
            //if (GlobalVariables.UnitNumber == 1)
            //    dDWGRL = WinHost.CalTree.GetFinishedTagValue("F1188");
            //else
            //    dDWGRL = WinHost.CalTree.GetFinishedTagValue("F2188");
            dDWGRL = WinHost.TagValue.GetFinishedTagValueSix("F4457");
            chart1.Series[0].Points[4].Label = "本机对外供热量\n\n" + Math.Round(dDWGRL, 1) + "吉焦/小时";
            chart1.Series[0].Points[4].YValues[0] = GetY(600, 2500, dDWGRL);
            double dDWGRSR = WinHost.TagValue.GetFinishedTagValueSix("F4227");//对外供热收入
            chart1.Series[0].Points[5].Label = "对外供热收入\n\n" + Math.Round(dDWGRSR) + "元/小时";
            chart1.Series[0].Points[5].YValues[0] = GetY(10000, 120000, dDWGRSR);
            double dCMLR = WinHost.TagValue.GetFinishedTagValueSix("F4228");//收入毛利润
            chart1.Series[0].Points[6].Label = "收入毛利润\n\n" + Math.Round(dCMLR) + "元/小时";
            chart1.Series[0].Points[6].YValues[0] = GetY(10000, 120000, dCMLR);
            double dFDB = 0;//发电比
            //if (GlobalVariables.UnitNumber == 1)
            //    dFDB = WinHost.CalTree.GetFinishedTagValue("F1181");
            //else
            //    dFDB = WinHost.CalTree.GetFinishedTagValue("F2181");
            dFDB = WinHost.TagValue.GetFinishedTagValueSix("F4372");
            //dFDB = dFDB * 100;
            chart1.Series[0].Points[7].Label = "发电比\n\n" + Math.Round(dFDB, 3) + "%";
            chart1.Series[0].Points[7].YValues[0] = GetY(20, 40, dFDB);
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            //Text = chart1.Tag.ToString() + " X:" + e.X;
        }

        int m_iMouseUpX = 0;
        private void chart1_MouseUp(object sender, MouseEventArgs e)
        {
            m_iMouseUpX = e.X;
        }

        private void chart1_Resize(object sender, EventArgs e)
        {
            //Text = "1:" + (int)(chart1.Width / m_dChart1Width * m_iX1[0]) + "," + (int)(chart1.Width / m_dChart1Width * m_iX1[1])
            //    + " 2:" + (int)(chart1.Width / m_dChart1Width * m_iX2[0]) + "," + (int)(chart1.Width / m_dChart1Width * m_iX2[1])
            //    + " 3:" + (int)(chart1.Width / m_dChart1Width * m_iX3[0]) + "," + (int)(chart1.Width / m_dChart1Width * m_iX3[1])
            //    + " 4:" + (int)(chart1.Width / m_dChart1Width * m_iX4[0]) + "," + (int)(chart1.Width / m_dChart1Width * m_iX4[1])
            //    + " 5:" + (int)(chart1.Width / m_dChart1Width * m_iX5[0]) + "," + (int)(chart1.Width / m_dChart1Width * m_iX5[1])
            //    + " 6:" + (int)(chart1.Width / m_dChart1Width * m_iX6[0]) + "," + (int)(chart1.Width / m_dChart1Width * m_iX6[1])
            //    + " 7:" + (int)(chart1.Width / m_dChart1Width * m_iX7[0]) + "," + (int)(chart1.Width / m_dChart1Width * m_iX7[1])
            //    + " 8:" + (int)(chart1.Width / m_dChart1Width * m_iX8[0]) + "," + (int)(chart1.Width / m_dChart1Width * m_iX8[1]);
            //chart1.Tag = Text;
        }

        private void chart1_Paint(object sender, PaintEventArgs e)
        {
            chart1_Resize(null, null);
        }


        private void chart1_DoubleClick(object sender, EventArgs e)
        {
             
            HistoryArea historyArea = new HistoryArea(UnitNO);
            //第1个柱子
            if (m_iMouseUpX >= (int)(chart1.Width / m_dChart1Width * m_iX1[0]) && m_iMouseUpX <= (int)(chart1.Width / m_dChart1Width * m_iX1[1]))
            {
                //if (GlobalVariables.UnitNumber == 1)
                //    historyArea.labelTag.TagID = 1187;
                //else
                //    historyArea.labelTag.TagID = 2187;
                historyArea.labelTag.TagID = 4371;
                historyArea.labelTag.TagDesc = "锅炉燃煤量(标煤)";
                historyArea.labelTag.TagUnit = "吨/小时";
                historyArea.colorSeries = chart1.Series[0].Points[0].Color;
                historyArea.Show();
            }
            //第2个柱子
            if (m_iMouseUpX >= (int)(chart1.Width / m_dChart1Width * m_iX2[0]) && m_iMouseUpX <= (int)(chart1.Width / m_dChart1Width * m_iX2[1]))
            {
                //if (GlobalVariables.UnitNumber == 1)
                //    historyArea.labelTag.TagID = 1225;
                //else
                //    historyArea.labelTag.TagID = 2225;
                historyArea.iTotalID = 10;
                historyArea.sTotalUnit = "万元";
                historyArea.labelTag.TagID = 4225;
                historyArea.labelTag.TagDesc = "然料支出费用";
                historyArea.labelTag.TagUnit = "元/小时";
                historyArea.colorSeries = chart1.Series[0].Points[1].Color;
                historyArea.Show();
            }
            //第3个柱子
            if (m_iMouseUpX >= (int)(chart1.Width / m_dChart1Width * m_iX3[0]) && m_iMouseUpX <= (int)(chart1.Width / m_dChart1Width * m_iX3[1]))
            {
                historyArea.labelTag.TagID = 4224;
                historyArea.labelTag.TagDesc = "上网电量";
                historyArea.labelTag.TagUnit = "兆瓦/小时";
                historyArea.colorSeries = chart1.Series[0].Points[2].Color;
                historyArea.Show();
            }
            //第4个柱子
            if (m_iMouseUpX >= (int)(chart1.Width / m_dChart1Width * m_iX4[0]) && m_iMouseUpX <= (int)(chart1.Width / m_dChart1Width * m_iX4[1]))
            {
                historyArea.iTotalID = 6;
                historyArea.sTotalUnit = "万元";
                historyArea.labelTag.TagID = 4226;
                historyArea.labelTag.TagDesc = "上网电费收入";
                historyArea.labelTag.TagUnit = "元/小时";
                historyArea.colorSeries = chart1.Series[0].Points[3].Color;
                historyArea.Show();
            }
            //第5个柱子
            if (m_iMouseUpX >= (int)(chart1.Width / m_dChart1Width * m_iX5[0]) && m_iMouseUpX <= (int)(chart1.Width / m_dChart1Width * m_iX5[1]))
            {
                historyArea.labelTag.TagID = 4457;
                historyArea.labelTag.TagDesc = "本机组对外供热量";
                historyArea.labelTag.TagUnit = "吉焦/小时";
                historyArea.colorSeries = chart1.Series[0].Points[4].Color;
                historyArea.Show();
            }
            //第6个柱子
            if (m_iMouseUpX >= (int)(chart1.Width / m_dChart1Width * m_iX6[0]) && m_iMouseUpX <= (int)(chart1.Width / m_dChart1Width * m_iX6[1]))
            {
                historyArea.iTotalID = 2;
                historyArea.sTotalUnit = "万元";
                historyArea.labelTag.TagID = 4227;
                historyArea.labelTag.TagDesc = "对外供热收入";
                historyArea.labelTag.TagUnit = "元/小时";
                historyArea.colorSeries = chart1.Series[0].Points[5].Color;
                historyArea.Show();
            }
            //第7个柱子
            if (m_iMouseUpX >= (int)(chart1.Width / m_dChart1Width * m_iX7[0]) && m_iMouseUpX <= (int)(chart1.Width / m_dChart1Width * m_iX7[1]))
            {
                historyArea.iTotalID = 14;
                historyArea.sTotalUnit = "万元";
                historyArea.labelTag.TagID = 4228;
                historyArea.labelTag.TagDesc = "收入毛利润";
                historyArea.labelTag.TagUnit = "元/小时";
                historyArea.colorSeries = chart1.Series[0].Points[6].Color;
                historyArea.Show();
            }
            //第8个柱子
            if (m_iMouseUpX >= (int)(chart1.Width / m_dChart1Width * m_iX8[0]) && m_iMouseUpX <= (int)(chart1.Width / m_dChart1Width * m_iX8[1]))
            {
                //if (GlobalVariables.UnitNumber == 1)
                //    historyArea.labelTag.TagID = 1181;
                //else
                //    historyArea.labelTag.TagID = 2181;
                historyArea.labelTag.TagID = 4372;
                historyArea.labelTag.TagDesc = "发电比";
                historyArea.labelTag.TagUnit = "%";
                historyArea.colorSeries = chart1.Series[0].Points[7].Color;
                historyArea.Show();
            }
            historyArea.Refresh();
        }

        private void SSCS_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose(true);
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

    }
}
