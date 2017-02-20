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

namespace KSPrj.YXYH
{
    public partial class CalCSWDLLSix : Form
    {
        //这个类里面注释掉的都要放开并改正 ZZH 2016-9-17 
        public CalCSWDLLSix()
        {
            InitializeComponent();
        }
        double dF3430 = 0;//热网回水温度
        private void CalCSWDLL_Load(object sender, EventArgs e)
        {
            try
            {
                txtInputMaxFlow.Text = "";//CalTree.GetFixedValue("F3222").ToString("0"); 
            }
            catch { }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void btnCal_Click(object sender, EventArgs e)
        {
            double dInputTemperature = 0;
            double dInputFlow = 0;
            double dInputMaxFlow = 0;
            if (txtInputTemperature.Text.Trim() == "" | double.TryParse(txtInputTemperature.Text.Trim(), out dInputTemperature) == false)
            {
                MessageBox.Show("请输入热网要求的出水温度！");
                txtInputTemperature.Focus();
                return;
            }
            if (txtInputFlow.Text.Trim() == "" | double.TryParse(txtInputFlow.Text.Trim(), out dInputFlow) == false)
            {
                MessageBox.Show("请输入热网要求的出水流量！");
                txtInputFlow.Focus();
            }
            if (txtInputMaxFlow.Text.Trim() == "" | double.TryParse(txtInputMaxFlow.Text.Trim(), out dInputMaxFlow) == false)
            {
                MessageBox.Show("请输入能够达到的最大流量！");
                txtInputMaxFlow.Focus();
            }
            try
            {
                dF3430 = 0;// CalTree.GetFinishedTagValue("F3430");//热网回水温度
                double dF3213 = 0;// CalTree.GetFinishedTagValue("F3213");//理论要求的出水温度

                double dHeat = (dInputTemperature - dF3430) * dInputFlow;
                double dOutputFlow = dHeat / (dF3213 - dF3430);
                if (dOutputFlow > dInputMaxFlow)
                {
                    dOutputFlow = dInputMaxFlow;
                    dF3213 = dF3430 + dHeat / dInputMaxFlow;
                }
                txtOutputTemperature.Text = dF3213.ToString("0.0");
                txtOutputFlow.Text = dOutputFlow.ToString("0.0");
                double dF3222 = 0;// CalTree.GetFixedValue("F3222");
                if ((int)dF3222 != (int)dInputMaxFlow)
                {
                    SQLHelper.ExecuteSql("update [tags] set SetValue=" + dInputMaxFlow + " where [id]=3222");
                    //TableTags.setFill();
                }
                string sSql = "insert into CalTempAndFlow(CalTime,InputTemperature,InputFlow,InputMaxFlow,OutputTemperature,OutputFlow) values('"
                    + PublicFunction.DateTimeToString(DateTime.Now) + "'," + dInputTemperature + "," + dInputFlow + "," + dInputMaxFlow + "," 
                    + dF3213.ToString("0.0") + "," + dOutputFlow + ")";
                SQLHelper.ExecuteSql(sSql);
            }
            catch (Exception ex)
            {
                MessageBox.Show("计算失败！");
                WriteLog.WriteLogs("计算热网温度和流量失败！" + ex.ToString());
            }
        }
    }
}
