using HAOCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KSPrj
{
    public partial class ChartPicLine : Form
    {
        int UnitNO = 5;
        public ChartPicLine(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
            this.timer1.Interval = 10 * 1000;
            this.timer1.Enabled = true;
            PicChartLabelBind();
        }

        private void ChartPicLine_Load(object sender, EventArgs e)
        {
            PicChartLabelBind();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            PicChartLabelBind();
        }

        private void PicChartBind(double douBar1Value) 
        {
            if (douBar1Value <= 100)
            {
                this.labelSub1.ForeColor = Color.Green;
                        
                this.picGreen1.Visible = true;
                this.picGreen1.BackColor = Color.Green;
                if (douBar1Value < 10)//如果低于10，则按照10的长度显示
                {
                    this.picGreen1.Width = Convert.ToInt32(10 * 8);
                }
                else
                {
                    this.picGreen1.Width = Convert.ToInt32(douBar1Value * 8);
                }

                this.picRed1.Visible = false;
                this.picYellow1.Visible = false; 
            }
            //else if ((douBar1Value > 100) && (douBar1Value <= 112.5))
            //{
            //    this.labelSub1.ForeColor = Color.Yellow;

            //    this.picGreen1.Visible = true;
            //    this.picGreen1.BackColor = Color.Yellow;
            //    this.picGreen1.Width = 800; // Convert.ToInt32(douData * 10) - 1000;

            //    this.picYellow1.Visible = true;
            //    this.picYellow1.BackColor = Color.Yellow;
            //    this.picYellow1.Width = Convert.ToInt32(douBar1Value * 8) - 800;

            //    this.picOrange1.Visible = false;
            //    this.picRed1.Visible = false;
            //}
            else if ((douBar1Value > 100) && (douBar1Value <= 125))
            {
                this.labelSub1.ForeColor = Color.Yellow;

                this.picGreen1.Visible = true;
                this.picGreen1.BackColor = Color.Yellow; 
                this.picGreen1.Width = 800; // Convert.ToInt32(douData * 10) - 1000;

                this.picYellow1.Visible = true;
                this.picYellow1.BackColor = Color.Yellow;
                this.picYellow1.Width = Convert.ToInt32(douBar1Value * 8) - 800; ;

                //this.picOrange1.Visible = true;
                //this.picOrange1.BackColor = Color.Orange;
                //this.picOrange1.Width = Convert.ToInt32(douBar1Value * 8) - 900;
                        
                this.picRed1.Visible = false;
            }
            else if (douBar1Value > 125)
            { 
                this.labelSub1.ForeColor = Color.Red;

                this.picGreen1.Visible = true;
                this.picGreen1.BackColor = Color.Red;
                this.picGreen1.Width = 800; // Convert.ToInt32(douData * 10) - 1000;

                this.picYellow1.Visible = true; // Convert.ToInt32(douData * 10) - 1000;
                this.picYellow1.BackColor = Color.Red;
                this.picYellow1.Width = 200;

                //this.picOrange1.Visible = true;
                //this.picOrange1.BackColor = Color.Red;
                //this.picOrange1.Width = 100;

                this.picRed1.Visible = true;
                int ii = Convert.ToInt32(douBar1Value * 8) - 1000;
                if (ii > 25 * 8)
                    this.picRed1.Width = Convert.ToInt32(25 * 8);
                else
                    this.picRed1.Width = Convert.ToInt32(ii);
            } 

            Application.DoEvents();
        }

        private void PicChartLabelBind()
        {
            DataRow CalDataTable = null;
            if(this.UnitNO == 5)
            {
                if (GlobalVariables.dtOneRowDataFive == null || GlobalVariables.dtOneRowDataFive.Rows.Count <= 0)
                    return;
                else
                {
                    CalDataTable = GlobalVariables.dtOneRowDataFive.Rows[0];
                }
            }
            else if (this.UnitNO == 6)
            {
                if (GlobalVariables.dtOneRowDataSix == null || GlobalVariables.dtOneRowDataSix.Rows.Count <= 0)
                    return;
                else
                {
                    CalDataTable = GlobalVariables.dtOneRowDataSix.Rows[0];
                }
            } 

            double F3002 = 0;
            try { F3002 = Convert.ToDouble(CalDataTable["F3002"].ToString()); }
            catch { F3002 = 0; }
            this.labelSub1.Text = F3002.ToString();
            PicChartBind(F3002); //绑定棒状图 

            string num1 = "--"; //低压缸排气容积流量
            string num2 = "--"; //流速
            string num3 = "--"; //相对容积流量
            if (this.UnitNO == 5)
            {
                try { num1 = CalDataTable["F1323"] + "m³/s"; }
                catch { num1 = "--"; }

                try { num2 = CalDataTable["F1320"] + "m/s"; }
                catch { num2 = "--"; }

                try { num3 = CalDataTable["F1329"].ToString(); }
                catch { num3 = "--"; }
            }
            else if (this.UnitNO == 6)
            {
                try { num1 = CalDataTable["F2323"] + "m³/s"; }
                catch { num1 = "--"; }

                try { num2 = CalDataTable["F2320"] + "m/s"; }
                catch { num2 = "--"; }

                try { num3 = CalDataTable["F2329"].ToString(); }
                catch { num3 = "--"; }
            }
            
            this.label36.Text = num1;
            this.label38.Text = num2;
            this.label40.Text = num3;
        }



    }
}
