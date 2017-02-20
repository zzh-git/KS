using KSPrj.YXYH;
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

namespace KSPrj
{
    public partial class FrmTagCheck : Form
    {
        public FrmTagCheck()
        {
            InitializeComponent();
        }


        private DataTable dt = new DataTable();
        private void FrmTagCheck_Load(object sender, EventArgs e)
        {

            timer1.Interval = 10 * 1000;

            this.dataGridView1.AutoGenerateColumns = false;
            BindGridView();

            timer1_Tick_1(null, null); 
            timer1.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件； 
        }
        

        /// <summary>
        /// 绑定GridView
        /// </summary>
        private void BindGridView()
        { 
            try
            {  
                if (this.dataGridView1.DataSource == null)
                {
                    //if (GlobalVariables.UnitNumber == 1) 
                    //{
                    //    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "([id]<2000 or [id]>3000) and datasourcesNo=1";
                    //    dt = GlobalVariables.dtTagsSet.DefaultView.ToTable();
                    //    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "";
                    //}
                    //else if (GlobalVariables.UnitNumber == 2)
                    //{
                    //    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "[id]>2000 and datasourcesNo=1";
                    //    dt = GlobalVariables.dtTagsSet.DefaultView.ToTable();
                    //    GlobalVariables.dtTagsSet.DefaultView.RowFilter = "";
                    //} 

                    GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = "datasourcesNo=1";
                    dt = GlobalVariables.dtTagsSetFive.DefaultView.ToTable();
                    this.dataGridView1.DataSource =dt;
                }
                else
                {
                    string tagValue = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        tagValue = TagValue.GetShowValueFive(int.Parse(dt.Rows[i]["id"].ToString()), "", dt.Rows[i]["adjustValue"].ToString()); //CalTree.GetShowValueNoUnit(int.Parse(dt.Rows[i]["id"].ToString()), dt.Rows[i]["Unit"].ToString(), dt.Rows[i]["adjustValue"].ToString());
                        dataGridView1.Rows[i].Cells["TagValue"].Value = tagValue; //CalTree.GetShowValueNoUnit(int.Parse(dt.Rows[i]["id"].ToString()), dt.Rows[i]["Unit"].ToString(), dt.Rows[i]["adjustValue"].ToString());

                        if (string.IsNullOrEmpty(tagValue) || tagValue == "--" || tagValue == GlobalVariables.BadValue.ToString()) //实时值
                        {
                            this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.OrangeRed;
                            continue;
                        }
                        else
                        {
                            this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                        }
                    }
                }
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs(ex.ToString()); 
            }
        }

        //判断是否报警
        private void CheckGridData()
        {
            int count = this.dataGridView1.RowCount; 
            try
            { 
                for (int i = 0; i < count; i++)
                {
                    string tagValue = this.dataGridView1.Rows[i].Cells["TagValue"].Value + "";
                    if (string.IsNullOrEmpty(tagValue) || tagValue == "--" || tagValue == GlobalVariables.BadValue.ToString()) //实时值
                    {
                        this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.OrangeRed;
                        continue;
                    }
                    else
                    {
                        this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    }

                    if (i % 10 == 0)
                        Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs(ex.ToString()); 
            }
        }
        //按停止刷新按钮
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            this.button1.Enabled = false;
            this.button2.Enabled = true;
        }
        //按开始刷新按钮
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            this.button2.Enabled = false;
            this.button1.Enabled = true;
        }
        //单击关闭按钮
        private void button3_Click(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            this.Close();
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            BindGridView();

            //设置异常数据行格式的语句已经放在了BindGridView()方法里，少了一次For循环。
            //CheckGridData();//设置异常数据行格式
        }

        //导出
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count <= 0)
            {
                MessageBox.Show("无数据");
            }
            else
            {
                try
                {
                    bool b = NPOIExcelHelper.DataGridViewToExcel(this.dataGridView1, "", "实时测点数据");
                    //ExcelHelper.DataTableToExcel(dt, "报警历史");
                    if(b)
                        MessageBox.Show("导出成功");
                }
                catch { MessageBox.Show("导出失败"); }
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            WriteLog.WriteLogs(e.ToString());
        }
         
      }
}
