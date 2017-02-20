using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YHYXYH.YXYH;
using KSPrj.YXYH;
using HAOCommon;
using HAOCommon.PI;
using HAOCommon.Security;

namespace KSPrj
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            pnlLeft.Width = this.Width / 2;
        }

        private void btnYXYH_Click(object sender, EventArgs e)
        {
            Program.yxyh.Show();
            this.Hide();
        }

        private void btnSSXY_Click(object sender, EventArgs e)
        {
            SSCS frm = new SSCS(5);
            frm.Show();
        }

        private void btnRLLJZC_Click(object sender, EventArgs e)
        {
            (new RLZCFY(5)).Show();
        }

        private void btnGRLJSR_Click(object sender, EventArgs e)
        {
            (new GRLJSR(5)).Show();
        }

        private void btnGDLJSR_Click(object sender, EventArgs e)
        {
            (new GDLJSR(5)).Show();
        }

        private void btnLJMLR_Click(object sender, EventArgs e)
        {
            (new LJMLR(5)).Show();
        }

        private void btnLJJBML_Click(object sender, EventArgs e)
        {
            (new LJJML(5)).Show();
        }

        private void btnRWCSWD_Click(object sender, EventArgs e)
        {
            (new ChartTemperature()).Show();
        }


        int minite = 0;
        bool b = true;
        private void timerDateTime_Tick(object sender, EventArgs e)
        {
            try
            {
                timerDateTime.Enabled = false;

                if (b)
                {
                    int days = ReadTime.GetDaysExpired();
                    if (days <= 0)
                    {
                        MessageBox.Show("软件授权已到期，请联系开发商重新授权！");
                        System.Environment.Exit(0);
                    }
                    else
                    {
                        if (days > 30)
                        {
                            this.label5.Visible = false;
                        }
                        else
                        {
                            this.label5.Visible = true;
                            this.label5.Text = "距授权到期：" + days + " 天";
                        }
                    }
                    b = false;
                }
                else
                {
                    minite++;
                    if (minite > 1200)
                    {
                        //每10分钟判断一次
                        minite = 0;
                        b = true;
                    }
                }

                lblDateTime.Text = string.Format("{0:F}", DateTime.Now);
                //if (PIRead.IsOperatingPI & (DateTime.Now - PIRead.TimeOfBeginOperatingPI).TotalSeconds > 4)
                //    lblDateTime.Text = DateTime.Now.ToLongTimeString() + "  读PI已" + Math.Round((DateTime.Now - PIRead.TimeOfBeginOperatingPI).TotalSeconds, 1) + "秒！";
                CheckAlarm();
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
            finally
            {
                timerDateTime.Enabled = true;
            } 
        }

        //抽汽限制图 
        private void button2_Click(object sender, EventArgs e)
        {
            ChartCQXZ cqxz = new ChartCQXZ(5);
            cqxz.Show();
        }

        //背压限制图 
        private void button1_Click(object sender, EventArgs e)
        {
            ChartBrokenline ct3 = new ChartBrokenline(5);
            ct3.Show();
        }

        //相对动应力限制图 
        private void button3_Click(object sender, EventArgs e)
        {
            ChartParabola ct2 = new ChartParabola(5);
            ct2.Show();
        }

        //排汽压力限制 
        private void button5_Click(object sender, EventArgs e)
        {
            ChartPicLine cpl = new ChartPicLine(5);
            cpl.Show();
        }

        //安全监测主窗体 
        private void button4_Click(object sender, EventArgs e)
        {
            Program.frm1.Show();
            this.Hide();
        }

        //窗体关闭 
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //消息框中需要显示哪些按钮，此处显示“确定”和“取消”
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            
            //"确定要退出吗？"是对话框的显示信息，"退出系统"是对话框的标题
            //默认情况下，如MessageBox.Show("确定要退出吗？")只显示一个“确定”按钮。
            DialogResult dr = MessageBox.Show("确定要退出吗?", "退出系统", messButton);
            if (dr == DialogResult.OK) //如果点击“确定”按钮
            {
                string pwd = SQLHelper.ExecuteScalar("select pws from SysPara where name='yx'"); 
                //string s
                FrmInputPws fip = new FrmInputPws("请输入(运行人员)密码");
                fip.ShowDialog();
                string strRet = fip.Text;

                string[] strRetA = strRet.Split(',');
                if (strRet.Length == 0) 
                {
                    e.Cancel = true;
                    return;
                }
                    
                if (strRetA[1] == "ok")
                {
                    if (pwd == strRetA[0])
                    {
                        try
                        { 
                            Program.service5.Disconnect();
                        }
                        catch  
                        { }
                        try { Program.service6.Disconnect(); }
                        catch { }
                        try
                        {
                            Program.yckz1.Disconnect();
                        }
                        catch
                        { }
                        //增加下面的二行语句，add by hlt 2017-1-14
                        WinHost.TagLJValue.SaveMinMaxValuesFive();
                        WinHost.TagLJValue.SaveMinMaxValuesSix();
                        fip.Dispose();

                        WriteLog.WriteLogs("程序关闭。");

                        System.Environment.Exit(0);
                        //this.Close();
                        //this.Dispose();
                        //Application.Exit();
                    }
                    else
                    {
                        MessageBox.Show("密码错误！");
                        fip.Dispose();
                        e.Cancel = true;
                    }
                }
            }
            else //如果点击“取消”按钮
            {
                e.Cancel = true;
                return;
            } 
        }

        /// <summary>
        /// 检查报警
        /// 如果安全监测有报警，相应的按钮也要闪烁 
        /// </summary>
        private void CheckAlarm() 
        {
            if (Program.frm1.picChart)
            {
                if (this.button5.ForeColor == Color.Black)
                    this.button5.ForeColor = Program.frm1.picColor;
                else
                    this.button5.ForeColor = Color.Black;
            }
            else
                this.button5.ForeColor = Color.Black;

            if (Program.frm1.brokenChart) 
            {
                if (this.button1.ForeColor == Color.Black)
                    this.button1.ForeColor = Program.frm1.brokenColor;
                else
                    this.button1.ForeColor = Color.Black;
            }

            if (Program.frm1.parabolaChart)
            {
                if (this.button3.ForeColor == Color.Black)
                    this.button3.ForeColor = Program.frm1.parabolaColor;
                else
                    this.button3.ForeColor = Color.Black;
            }

            //if (Program.frm1.CQXZboolAlarm)
            //{
            //    if (this.button2.ForeColor == Color.Black)
            //        this.button2.ForeColor = Program.frm1.CQXZColor;
            //    else
            //        this.button2.ForeColor = Color.Black;
            //}

        }

        private void label4_DoubleClick(object sender, EventArgs e)
        {
            ContactUs cu = new ContactUs();
            cu.ShowDialog();
            cu.Dispose();
        }

        private void btnSZMX_Click(object sender, EventArgs e)
        {
            (new LJSZMX(5)).Show();
        }

        private void btnEditPassword_Click(object sender, EventArgs e)
        {
            PWDChange pwd = new PWDChange();
            pwd.ShowDialog();
            pwd.Dispose();
        }

        

        //#5异常控制 
        private void button7_Click(object sender, EventArgs e)
        {
            Program.yckz1.Show();
            this.Hide();
        }

        //#6安全监测 
        private void button8_Click(object sender, EventArgs e)
        {
            //Program.frm2.Show();
            //this.Hide();
        }

        //#6优化运行 
        private void button6_Click(object sender, EventArgs e)
        {
            Program.yxyh2.Show();
            this.Hide();
        }

        //#6异常控制 
        private void button9_Click(object sender, EventArgs e)
        {
            //Program.yckz2.Show();
            //this.Hide();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
 

       

    }
}
