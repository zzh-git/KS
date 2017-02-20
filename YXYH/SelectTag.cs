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
    public partial class SelectTag : Form
    {
        int UnitNO = 5;
        public SelectTag(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
        }

        DataView m_viewTagValue = null;
        public string sExcelCells = "";
        public DataTable dtCheckedTags = null;
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SelectTag_Load(object sender, EventArgs e)
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

            m_viewTagValue.Table.Columns.Add("CheckRow", typeof(int));
            m_viewTagValue.Table.AcceptChanges();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = m_viewTagValue;
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 1; i <= dataGridView1.Rows.Count; i++)
                dataGridView1.Rows[i - 1].Cells["No"].Value = i;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                dtCheckedTags = m_viewTagValue.Table.GetChanges();
                List<DataRow> lstDeleteRows = new List<DataRow>();
                if (dtCheckedTags != null)
                {
                    foreach (DataRow row in dtCheckedTags.Rows)
                    {
                        if (row["CheckRow"].ToString() == "1")
                            sExcelCells += row["ExcelCell"] + ",";
                        else
                            lstDeleteRows.Add(row);
                    }
                    sExcelCells = sExcelCells.Trim(',');
                    if (lstDeleteRows.Count > 0)
                        foreach (DataRow row in lstDeleteRows)
                        {
                            dtCheckedTags.Rows.Remove(row);
                        }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("请选择测点！");
                }
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
        }

        private void txtQuery_TextChanged(object sender, EventArgs e)
        {
            try
            {
                m_viewTagValue.RowFilter = "tagdesc like '%" + txtQuery.Text.Trim() + "%'";
                dataGridView1.Refresh();
            }
            catch { }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            txtQuery_TextChanged(null, null);
        }
    }
}
