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
    public partial class ValuesConfig : Form
    {

        public ValuesConfig()
        {
            InitializeComponent();
           
        }
        DataTable m_dt_config = null;
        DataView m_configValue = null;
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 1; i <= dataGridView1.Rows.Count; i++)
                dataGridView1.Rows[i - 1].Cells["id"].Value = i;
        }
        private void ValuesConfig_Load(object sender, EventArgs e)
        {

            //GlobalVariables.dtConfig.DefaultView.RowFilter = "ConfigValue=1";
            m_dt_config = TableConfig.getDataTableByRowFilter("IsShow=1");
            m_configValue = m_dt_config.DefaultView;
           
               // GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = "";

                m_configValue.Table.AcceptChanges();

                m_configValue.RowFilter = "IsShow=1";
                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.DataSource = m_configValue;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string sSql = "";
            ArrayList lstSql = new ArrayList();
            DataTable dtChanges = m_configValue.Table.GetChanges();
            if (dtChanges != null)
            {
                if (Program.frm1.PWDCheck("rg"))
                {
                    foreach (DataRow row in dtChanges.Rows)
                    {
                        sSql = "update Config set ConfigValue=" + "'"+row["ConfigValue"].ToString()+"'" + " where [Description]=" + "'" + row["Description"].ToString() + "'";  
                        lstSql.Add(sSql);
                    }
                    try
                    {
                        SQLHelper.ExecuteSql(lstSql);
                        m_configValue.Table.AcceptChanges();
                        TableConfig.setFill();
                        MessageBox.Show("保存成功！");
                        ChangeState();
                        this.Close();
                    }
                    catch
                    {
                        m_configValue.Table.RejectChanges();
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
        private void ValuesConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose(true);
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ChangeState()
        {
            try
            {
                string strIsAdd = TableConfig.getData("IsCanAddLabel"); 
                if ("true".Equals(strIsAdd) || "是".Equals(strIsAdd))
                    GlobalVariables.IsCanAddLabel = true;
                else
                    GlobalVariables.IsCanAddLabel = false;
            }
            catch
            {
                //MessageBox.Show("初始化 是否启用编辑 失败");
            }

            try
            {
                string strIsMove = TableConfig.getData("IsCanMoveLabel"); 
                if ("true".Equals(strIsMove) || "是".Equals(strIsMove))
                    GlobalVariables.IsCanMoveLabel = true;
                else
                    GlobalVariables.IsCanMoveLabel = false;

                GlobalVariables.dMJYPCZZ_Set = double.Parse(TableConfig.getData("MJYPCZZ_Set"));   

            }
            catch
            {
                //MessageBox.Show("初始化 是否可以移动Label 失败");
            }
        }

    }
}
