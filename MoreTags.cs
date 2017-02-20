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
using YHYXYH.YXYH;

namespace KSPrj
{
    public partial class MoreTags : Form
    {
        int UnitNO = 5;
        public MoreTags(int unitNo)
        {
            InitializeComponent();
            this.timer1.Interval = GlobalVariables.RefIntvel;
            this.timer1.Enabled = true;

            this.UnitNO = unitNo;

            this.dataGridView1.Columns[0].Width = 150;
            this.dataGridView1.Columns[1].Width = 150;
            this.dataGridView1.Columns[2].Width = 150;
            this.dataGridView1.AutoGenerateColumns = false;

            BindData();
        }


        //停止刷新 
        private void button2_Click(object sender, EventArgs e)
        {
            if (this.button2.Text == "停止刷新")
            {
                this.timer1.Enabled = false;
                this.button2.Text = "开始刷新";
            }
            else 
            {
                this.timer1.Enabled = true;
                this.button2.Text = "停止刷新";
            }
        }

        //导出 
        private void button1_Click(object sender, EventArgs e)
        {
            bool b = NPOIExcelHelper.DataGridViewToExcel(this.dataGridView1, "", "计算参数");
            if (b)
                MessageBox.Show("导出成功");
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            BindData();
        }

        DataTable dt = null; 
        private void BindData() 
        {
            
            if (dataGridView1.DataSource == null)
            {
                if (this.UnitNO == 5)
                {
                    GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = " isShow=1 ";
                    dt = GlobalVariables.dtTagsSetFive.DefaultView.ToTable(false, new string[] { "id", "tagdesc", "ExcelCell", "Unit", "TagValue", "adjustValue" });
                    GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = "";
                }
                else if (this.UnitNO == 6)
                {
                    GlobalVariables.dtTagsSetSix.DefaultView.RowFilter = "isShow=1";
                    dt = GlobalVariables.dtTagsSetSix.DefaultView.ToTable(false, new string[] { "id", "tagdesc", "ExcelCell", "Unit", "TagValue", "adjustValue" });
                    GlobalVariables.dtTagsSetSix.DefaultView.RowFilter = "";
                } 

                this.dataGridView1.DataSource = dt;
                //this.dataGridView1.Columns["ExcelCell"].Visible = false;
                //this.dataGridView1.Columns[0].HeaderText = "项目";
                //this.dataGridView1.Columns[1].HeaderText = "单位";
                //this.dataGridView1.Columns[2].HeaderText = "数值";
            }
            else
            {
                if (this.UnitNO == 5)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dataGridView1.Rows[i].Cells["TagValue"].Value = WinHost.TagValue.GetShowValueFive(int.Parse(dt.Rows[i]["id"].ToString()), "", dt.Rows[i]["adjustValue"].ToString());
                    }
                }
                else if (this.UnitNO == 6)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dataGridView1.Rows[i].Cells["TagValue"].Value = WinHost.TagValue.GetShowValueSix(int.Parse(dt.Rows[i]["id"].ToString()), "", dt.Rows[i]["adjustValue"].ToString());
                    }
                }
                
            }
            //Application.DoEvents();

        }
         

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            //dataGridView1.CurrentRow.Cells["TagDesc"].Value.ToString();
            HistoryLines m_frmHistoryLines = new HistoryLines(UnitNO);
            m_frmHistoryLines.labelTag.TagID = int.Parse(dataGridView1.CurrentRow.Cells["ID"].Value.ToString());
            m_frmHistoryLines.labelTag.TagDesc = dataGridView1.CurrentRow.Cells["TagDesc"].Value.ToString();
            m_frmHistoryLines.Show();
        }


    }
}
