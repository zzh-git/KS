using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using HAOCommon;

namespace YHYXYH.YXYH
{
    public partial class TagReplace : Form
    {
        int UnitNO = 5;
        public TagReplace(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
        }

        DataView m_viewTagValue = null;

        private void TagReplace_Load(object sender, EventArgs e)
        {
            if (this.UnitNO == 5)
            {
                GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = "DataSourcesNo=1";
                m_viewTagValue = GlobalVariables.dtTagsSetFive.DefaultView.ToTable().DefaultView;
                GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = "";
            }
            else if (this.UnitNO == 6)
            {
                GlobalVariables.dtTagsSetSix.DefaultView.RowFilter = "DataSourcesNo=1";
                m_viewTagValue = GlobalVariables.dtTagsSetSix.DefaultView.ToTable().DefaultView;
                GlobalVariables.dtTagsSetSix.DefaultView.RowFilter = "";
            }

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = m_viewTagValue;
            //dataGridView1.Refresh();

            //dataGridView1.Refresh();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string sSql = "";
            ArrayList lstSql = new ArrayList();
            DataTable dtChanges = m_viewTagValue.Table.GetChanges();
            if (dtChanges != null)
            {
                string pwd = SQLHelper.ExecuteScalar("select pws from SysPara where name='rg'");
                //string s
                KSPrj.FrmInputPws fip = new KSPrj.FrmInputPws("请输入(热工检修人员)密码");
                fip.ShowDialog();
                string strRet = fip.Text;

                string[] strRetA = strRet.Split(',');
                if (strRet.Length == 0)
                    return;
                if (strRetA[1] == "ok")
                {
                    if (pwd == strRetA[0])
                    {
                        foreach (DataRow row in dtChanges.Rows)
                        {
                            sSql = "update tags set IsSet='" + row["IsSet"] + "', tag='" + row["tag"] + "', unit='"
                                + row["unit"] + "', SetValue=" + PublicFunction.nullOrBadToSqlNULL(row["SetValue"]) + " where [id]=" + row["id"].ToString();
                            lstSql.Add(sSql);
                        }

                        try
                        {
                            SQLHelper.ExecuteSql(lstSql);
                            m_viewTagValue.Table.AcceptChanges();
                            if (this.UnitNO == 5)
                                FTableTags.setFill();
                            else
                                STableTags.setFill();
                            MessageBox.Show("保存成功！");
                            this.Close();
                        }
                        catch
                        {
                            m_viewTagValue.Table.RejectChanges();
                            MessageBox.Show("保存失败！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("密码错误！");
                        fip.Dispose();
                    }
                }
            }
            else
            {
                MessageBox.Show("数据未改变！");
                this.Close();
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 1; i <= dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i - 1].Cells["No"].Value = i;
                if (dataGridView1.Rows[i - 1].Cells["value"].Value.ToString() == GlobalVariables.BadValue.ToString())
                    dataGridView1.Rows[i - 1].DefaultCellStyle.BackColor = Color.Red;
            }
        }

        private void TagReplace_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose(true);
        }

        private void button1_Click(object sender, EventArgs e)
        { 
            bool b = NPOIExcelHelper.DataGridViewToExcel(this.dataGridView1, "", "坏点替代"); 
            if (b)
                MessageBox.Show("导出成功");
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                HistoryLines m_frmHistoryLines = new HistoryLines(UnitNO);
                m_frmHistoryLines.labelTag.TagID = int.Parse(dataGridView1.CurrentRow.Cells["TagID"].Value.ToString());
                m_frmHistoryLines.labelTag.TagDesc = dataGridView1.CurrentRow.Cells["TagDesc"].Value.ToString();
                m_frmHistoryLines.Show();
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
        }

    }
}
