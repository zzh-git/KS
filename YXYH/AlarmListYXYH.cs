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
    public partial class AlarmListYXYH : Form
    {
        DataTable dt = null;
        string whichOne = "YXYH";
        string sql = "";
        int UnitNO = 5;
        public AlarmListYXYH(string str, int unitNO)
        {
            InitializeComponent();
            this.whichOne = str;
            this.UnitNO = unitNO;
        }

        private void AlarmList_Load(object sender, EventArgs e)
        {
            
            if(whichOne.Equals("YXYH"))
            {
                this.Text = "运行优化报警历史";
                if(UnitNO == 5)
                    sql = "select top 100 * from AlarmYXYH where UnitNO=5 order by AlarmTime desc";
                else if(UnitNO == 6)
                    sql = "select top 100 * from AlarmYXYH where UnitNO=6 order by AlarmTime desc";
            }

            try
            {
                dt = SQLHelper.ExecuteDt(sql);
                this.dataGridView1.AutoGenerateColumns = false;
                this.dataGridView1.DataSource = dt; 
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs(ex.ToString());
                MessageBox.Show("无数据");
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dt == null || dt.Rows.Count <= 0)
            {
                MessageBox.Show("无数据");
            }
            else 
            {
                try {
                    bool b = NPOIExcelHelper.DataGridViewToExcel(this.dataGridView1, "", "报警历史");
                    if(b) 
                        MessageBox.Show("导出成功"); 
                }
                catch { MessageBox.Show("导出失败"); } 
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        //查询 
        private void btnQuery_Click(object sender, EventArgs e)
        {
            sql = "select AlarmText, AlarmDesc, AlarmState, AlarmTime, AlarmEndTime, AlarmLasts from AlarmYXYH where UnitNO=" + UnitNO +
            " and (AlarmTime>='" + dateTimePickerBegin1.Value.ToString("yyyy-MM-dd") + "' and AlarmTime<='" + dateTimePickerEnd1.Value.ToString("yyyy-MM-dd") + "') order by AlarmTime desc";
            try
            {
                DataTable dt = SQLHelper.ExecuteDt(sql);
                this.dataGridView1.AutoGenerateColumns = false;
                this.dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs(ex.ToString());
            }
        }
    }
}
