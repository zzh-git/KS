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
    public partial class AlarmList : Form
    {
        DataTable dt = null;
        string whichOne = "YXYH";
        string sql = "";
        public AlarmList(string str)
        {
            InitializeComponent();
            this.whichOne = str;
        }

        private void AlarmList_Load(object sender, EventArgs e)
        {
            if(whichOne.Equals("FCZ"))
            {
                this.Text = "防颤振报警历史";
                sql = "select AlarmText, AlarmDesc, AlarmState, AlarmTime, AlarmEndTime, AlarmLasts from AlarmFCZ";
            } 
            else if(whichOne.Equals("YXYH"))
            {
                this.Text = "运行优化报警历史";
                sql = "select top 100 AlarmText, AlarmDesc, AlarmState, AlarmTime, AlarmEndTime, AlarmLasts from AlarmYXYH order by AlarmTime desc";
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
    }
}
