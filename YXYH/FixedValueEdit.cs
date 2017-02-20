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
using KSPrj;

namespace YHYXYH.YXYH
{
    public partial class FixedValueEdit : Form
    {
        int UnitNO = 5;
        public FixedValueEdit(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
        }

        DataView m_viewTagValue = null;
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 1; i <= dataGridView1.Rows.Count; i++)
                dataGridView1.Rows[i - 1].Cells["No"].Value = i;
        }

        private void FixedValueEdit_Load(object sender, EventArgs e)
        {
            if (this.UnitNO == 5)
            {
                GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = "DataSourcesNo=3";
                m_viewTagValue = GlobalVariables.dtTagsSetFive.DefaultView.ToTable().DefaultView;
                GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = "";
            }
            else if (this.UnitNO == 6) 
            {
                GlobalVariables.dtTagsSetSix.DefaultView.RowFilter = "DataSourcesNo=3";
                m_viewTagValue = GlobalVariables.dtTagsSetSix.DefaultView.ToTable().DefaultView;
                GlobalVariables.dtTagsSetSix.DefaultView.RowFilter = "";
            }
            
            m_viewTagValue.Table.AcceptChanges();
            m_viewTagValue.RowFilter = "DataSourcesNo=3";
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = m_viewTagValue;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            string sSql = "";
            ArrayList lstSql = new ArrayList();
            DataTable dtChanges = m_viewTagValue.Table.GetChanges();
            if (dtChanges != null)
            {

                if (Program.frm1.PWDCheck("rg"))
                {
                    foreach (DataRow row in dtChanges.Rows)
                    {
                        sSql = "update tags set unit='"
                            + row["unit"] + "', SetValue=" + row["SetValue"].ToString() + "where [id]=" + row["id"].ToString();
                        lstSql.Add(sSql);
                    }

                    try
                    {
                        SQLHelper.ExecuteSql(lstSql);
                        m_viewTagValue.Table.AcceptChanges();
                        if(this.UnitNO == 5)
                            FTableTags.setFill();
                        else if(this.UnitNO == 6)
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
                    MessageBox.Show("密码错误！");
            }
            else
            {
                MessageBox.Show("数据未改变！");
                this.Close();
            }
        }
    }
}
