using HAOCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinHost;
using KSPrj;

namespace YHYXYH.YXYH
{
    public partial class ResetTotalAvg : Form
    {
        DataTable dtAvg = null;
        DataTable dtTotal = null;
        string sql = "";
        int UnitNO = 5;
        public ResetTotalAvg(int unitNO)
        {
            InitializeComponent();
            this.UnitNO = unitNO;
        }

        private void AlarmList_Load(object sender, EventArgs e)
        {
            try
            {
                MinMaxValue mmv = null;
                if (UnitNO == 5)
                {
                    dtAvg = TableMinMaxValue.getDatasFiveWithValueToData();
                    foreach (DataRow row in dtAvg.Rows)
                    {
                        mmv = (MinMaxValue)TagLJValue.MinMaxValueHsFive["F" + row["TagID"].ToString()];
                        row["AvgValue"] = mmv.AvgValue * mmv.AvgToDataRate;
                    }
                    dtTotal = FTableTotalTags.getDataTableByRowFilter("IsResetShow=1");
                    DataTable dtTemp = GlobalVariables.dtOneRowTotalDataFive;
                    foreach (DataRow row in dtTotal.Rows)
                    {
                        try { row["TotalValue"] = dtTemp.Rows[0]["F" + row["id"].ToString()]; }
                        catch { }
                    }
                }
                else if (UnitNO == 6)
                {
                    dtAvg = TableMinMaxValue.getDatasSixWithValueToData();
                    foreach (DataRow row in dtAvg.Rows)
                    {
                        mmv = (MinMaxValue)TagLJValue.MinMaxValueHsSix["F" + row["TagID"].ToString()];
                        row["AvgValue"] = mmv.AvgValue * mmv.AvgToDataRate;
                    }
                    dtTotal = STableTotalTags.getDataTableByRowFilter("IsResetShow=1");
                    DataTable dtTemp = GlobalVariables.dtOneRowTotalDataSix;
                    foreach (DataRow row in dtTotal.Rows)
                    {
                        try { row["TotalValue"] = dtTemp.Rows[0]["F" + row["id"].ToString()]; }
                        catch { }
                    }
                }

                dataGridView1.AutoGenerateColumns = false;
                this.dataGridView1.DataSource = dtAvg;
                dataGridView2.AutoGenerateColumns = false;
                this.dataGridView2.DataSource = dtTotal;
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs(ex.ToString());
                MessageBox.Show("无数据");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            if (grid.Columns[e.ColumnIndex].Name == "resetData")
            {
                DateTime ResetTime = DateTime.Now;
                FrmInputPws fip = new FrmInputPws("请输入(运行人员)密码");
                fip.ShowDialog();
                string strRet = fip.Text;
                fip.Dispose();
                string[] strRetA = strRet.Split(',');
                if (strRet.Length == 0)
                    return;

                if (strRetA[1] == "ok")
                {
                    if (TableSysPara.getData("yx") == strRetA[0])
                    {
                        int TagID = (int)grid.Rows[e.RowIndex].Cells["TagID"].Value;
                        bool b = false;
                        if (UnitNO == 5)
                            b = TagLJValue.ResetAvgFive(TagID, ResetTime);
                        else
                            b = TagLJValue.ResetAvgSix(TagID, ResetTime);
                        if (b == true)
                        {
                            grid.Rows[e.RowIndex].Cells["AvgValue"].Value = 0;
                            grid.Rows[e.RowIndex].Cells["BeginDate"].Value = ResetTime;
                        }
                        else
                            MessageBox.Show("数据复位失败！");
                    }
                    else
                    {
                        MessageBox.Show("密码错误，数据未复位！");
                    }
                }
            }
        }

        private void ResetTotalAvg_Resize(object sender, EventArgs e)
        {
            pnlAvg.Height = this.Height / 2;
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            if (grid.Columns[e.ColumnIndex].Name == "TReset")
            {
                DateTime ResetTime = DateTime.Now;
                FrmInputPws fip = new FrmInputPws("请输入(运行人员)密码");
                fip.ShowDialog();
                string strRet = fip.Text;
                fip.Dispose();
                string[] strRetA = strRet.Split(',');
                if (strRet.Length == 0)
                    return;

                if (strRetA[1] == "ok")
                {
                    if (TableSysPara.getData("yx") == strRetA[0])
                    {
                        int TagID = (int)grid.Rows[e.RowIndex].Cells["Tid"].Value;
                        bool b = false;
                        if (UnitNO == 5)
                            b = Program.service5.ResetTotalValue(TagID, ResetTime);
                        else
                            b = Program.service6.ResetTotalValue(TagID, ResetTime);

                        if (b == true)
                        {
                            grid.Rows[e.RowIndex].Cells["TotalValue"].Value = 0;
                            grid.Rows[e.RowIndex].Cells["TBeginDate"].Value = ResetTime;
                        }
                        else
                            MessageBox.Show("数据复位失败！");
                    }
                    else
                    {
                        MessageBox.Show("密码错误，数据未复位！");
                    }
                }
            }

        }

    }
}
