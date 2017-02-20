using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YHYXYH.YXYH;
using HAOCommon;
using WinHost;

namespace YHYXYH.Tool
{
    public partial class TagLabelBinding : Form
    {
        int UnitNO = 5;
        public TagLabelBinding(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
        }
        int iIndex = 0;
        public Control label;
        DataView m_viewTagValue = null;
        DataTable m_dtLineData = null;
        public DataView m_viewTagLabelBinding = SQLHelper.ExecuteDt("select * from TagLabelBinding").DefaultView;
        // 设置绑定的显示文本
        /// <summary>
        /// 设置绑定的显示文本
        /// </summary>
        /// <param name="sText">要设置的被绑定的项目名称</param>
        void SetBindingText(string sText)
        {
            lblIsBinded.Text = "已绑定“" + sText + "”。";
        }
        // 设置未绑定的显示文本
        /// <summary>
        /// 设置未绑定的显示文本
        /// </summary>
        void SetNoBindingText()
        {
            lblIsBinded.Text = "未绑定";
        }
        /// <summary>
        /// 设置label标签的Tag属性
        /// </summary>
        void SetLabelTag()
        {
            if (label.Tag == null)
                label.Tag = new LabelTag();
            else
                try { ((LabelTag)label.Tag).IsSetToolTipText = false; }
                catch { label.Tag = new LabelTag(); }
        }
        // 获取除所在窗体以外的所有父控件的路径
        /// <summary>
        /// 获取除所在窗体以外的所有父控件的路径
        /// </summary>
        /// <param name="control">要获取的控件</param>
        /// <returns></returns>
        public static string GetParents(Control control)
        {
            string sParent = "";
            while (control.Parent != null)
            {
                sParent = control.Parent.Name + "." + sParent;
                control = control.Parent;
            }
            sParent = sParent.Trim('.');
            //sParent = sParent.Substring(sParent.IndexOf('.') + 1);
            return sParent;
        }

        static string sLastParentPath = "";
        static Control parent = null;
        // 获取父控件
        /// <summary>
        /// 获取父控件
        /// </summary>
        /// <param name="control">要获取的控件</param>
        /// <returns></returns>
        public static Control GetParentControl(Form form, string sParentPath)
        {
            if (sParentPath != sLastParentPath)
            {
                string[] sParents = sParentPath.Replace(form.Name + ".", "").Split('.');
                Control control = form;
                for (int i = 0; i < sParents.Length; i++)
                {
                    try { control = control.Controls.Find(sParents[i], false)[0]; }
                    catch { }
                }
                parent = control;
                sLastParentPath = sParentPath;
            }
            return parent;
        }
        private void TagLabelBinding_Load(object sender, EventArgs e)
        {
            m_viewTagLabelBinding.AllowDelete = true;
            m_viewTagLabelBinding.AllowNew = true;
            m_viewTagLabelBinding.AllowEdit = true;

            if (this.UnitNO == 5)
            {
                //GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = "";
                GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = "DataSourcesNo=1 or isShow=1";
                m_viewTagValue = GlobalVariables.dtTagsSetFive.DefaultView.ToTable().DefaultView;
            }
            else if (this.UnitNO == 6)
            {
                //GlobalVariables.dtTagsSetSix.DefaultView.RowFilter = "";
                GlobalVariables.dtTagsSetSix.DefaultView.RowFilter = "DataSourcesNo=1 or isShow=1";
                m_viewTagValue = GlobalVariables.dtTagsSetSix.DefaultView.ToTable().DefaultView;
            }
            //else if(this.UnitNO == 51)
            //    m_viewTagValue = GlobalVariables.dtTagsSetFiveKZ.DefaultView.ToTable().DefaultView;
            //else if (this.UnitNO == 61)
            //    m_viewTagValue = GlobalVariables.dtTagsSetSixKZ.DefaultView.ToTable().DefaultView;
            //m_viewTagValue = GlobalVariables.dtTagsSet.DefaultView.ToTable().DefaultView;
            //GlobalVariables.dtTagsSet.DefaultView.RowFilter = "";
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = m_viewTagValue;
            dataGridView1.Refresh();
            m_viewTagValue.Table.AcceptChanges();

            if (label.GetType() != typeof(PictureBox))
                lblText.Text = label.Text;

            m_viewTagLabelBinding.RowFilter = "LabelName like '%" + label.Name + "%'";
            if (m_viewTagLabelBinding.Count > 0)
                SetBindingText(m_viewTagLabelBinding[0]["TagDesc"].ToString());
            else
                SetNoBindingText();

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

        private void btnDind_Click(object sender, EventArgs e)
        {
            string sTagID = dataGridView1.CurrentRow.Cells["TagID"].Value.ToString();
            string sTagDesc = dataGridView1.CurrentRow.Cells["TagDesc"].Value.ToString();

            m_viewTagLabelBinding.RowFilter = "LabelName='" + label.Name + "'";
            if (m_viewTagLabelBinding.Count > 0)//更新绑定
            {
                if (sTagID != m_viewTagLabelBinding[0]["TagID"].ToString())
                {
                    m_viewTagLabelBinding[0].BeginEdit();
                    m_viewTagLabelBinding[0]["TagID"] = sTagID;
                    m_viewTagLabelBinding[0]["TagDesc"] = sTagDesc;
                    try
                    {
                        SQLHelper.ExecuteSql("update TagLabelBinding set TagID=" + sTagID + ",TagDesc='" + sTagDesc
                        + "' where LabelName='" + label.Name + "'");
                        m_viewTagLabelBinding[0].EndEdit();
                        SetBindingText(sTagDesc);
                        SetLabelTag();
                        TableTagLabelBinding.setFill();
                        MessageBox.Show("绑定成功！");
                        Close();
                    }
                    catch (Exception ex)
                    {
                        m_viewTagLabelBinding[0].CancelEdit();
                        MessageBox.Show("绑定失败！");
                        WriteLog.WriteLogs(ex.ToString());
                    }
                }
            }
            else//建立绑定
            {
                //string sParent = GetParents(label);
                string sParent = GetParents(label);
                DataRowView rowNew = m_viewTagLabelBinding.AddNew();
                rowNew.BeginEdit();
                rowNew["LabelName"] = label.Name;
                rowNew["Parent"] = sParent;
                rowNew["TagID"] = sTagID;
                rowNew["TagDesc"] = sTagDesc;
                string sSql = "insert into TagLabelBinding(LabelName,Parent,TagID,TagDesc) ";
                sSql += "values('" + label.Name + "','" + sParent + "'," + sTagID + ",'" + sTagDesc + "')";
                try
                {
                    SQLHelper.ExecuteSql(sSql);
                    rowNew.EndEdit();
                    SetBindingText(sTagDesc);
                    SetLabelTag();
                    TableTagLabelBinding.setFill();
                    MessageBox.Show("绑定成功！");
                    Close();
                }
                catch (Exception ex)
                {
                    rowNew.CancelEdit();
                    MessageBox.Show("绑定失败！");
                    WriteLog.WriteLogs(ex.ToString());
                }
            }
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            txtQuery_TextChanged(null, null);
        }

        private void btnRemoveBinding_Click(object sender, EventArgs e)
        {
            m_viewTagLabelBinding.RowFilter = "LabelName='" + label.Name + "'";
            if (m_viewTagLabelBinding.Count > 0)
            {
                try
                {
                    SQLHelper.ExecuteSql("delete FROM [TagLabelBinding] where LabelName='" + label.Name + "'");
                    m_viewTagLabelBinding[0].Delete();
                    TableTagLabelBinding.setFill();
                    SetNoBindingText();
                }
                catch (Exception ex)
                {
                    WriteLog.WriteLogs(ex.ToString());
                    MessageBox.Show("解绑失败！");
                }
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Right)
            //    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
        }

        private void panel2_DoubleClick(object sender, EventArgs e)
        {

            MessageBox.Show(m_viewTagValue.Table.GetChanges().Rows.Count.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName.Length > 0)
            {
                string sFileName = openFileDialog1.FileName;
                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                app.Visible = true;
                Microsoft.Office.Interop.Excel.Workbooks wbks = app.Workbooks;
                Microsoft.Office.Interop.Excel._Workbook _wbk = wbks.Open(openFileDialog1.FileName);
                Microsoft.Office.Interop.Excel.Sheets shs = _wbk.Sheets;
                Microsoft.Office.Interop.Excel._Worksheet _wsh = (Microsoft.Office.Interop.Excel._Worksheet)shs[1];
                foreach (DataRowView row in m_viewTagValue)
                {
                    if (row["DataSourcesNo"].ToString() == "1")
                        _wsh.Cells[int.Parse(row["id"].ToString()), 6] = row["TagValue"].ToString();
                }
                string sExt = sFileName.Substring(sFileName.LastIndexOf('.'));
                sFileName = sFileName.Substring(0, sFileName.LastIndexOf('.'));
                sFileName = sFileName + "_2" + sExt;
                _wbk.SaveAs(sFileName);
                MessageBox.Show("完成！");
            }
        }

        private void dateTimePickerBegin1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerBegin2.Value = DateTime.Parse(dateTimePickerBegin1.Value.ToString("yyyy-MM-dd") + " "
                + dateTimePickerBegin2.Value.ToString("HH:mm:ss"));
        }

        private void dateTimePickerEnd1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePickerEnd2.Value = DateTime.Parse(dateTimePickerEnd1.Value.ToString("yyyy-MM-dd") + " "
                + dateTimePickerEnd2.Value.ToString("HH:mm:ss"));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            iIndex = 0;
            m_dtLineData = TagValue.QueryDBData(dateTimePickerBegin2.Value, dateTimePickerEnd2.Value, "*", UnitNO);
            foreach (DataRowView row in m_viewTagValue)
            {
                row["TagValue"] = m_dtLineData.Rows[iIndex]["F" + row["id"].ToString()];
            }
        }

        void DataBinding()
        {
            m_viewTagValue.RowFilter = "";
            foreach (DataRowView row in m_viewTagValue)
            {
                row["TagValue"] = m_dtLineData.Rows[iIndex]["F" + row["id"].ToString()];
            }
            dataGridView1.Refresh();
        }
        private void btnFirst_Click(object sender, EventArgs e)
        {
            iIndex = 0;
            DataBinding();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            if (iIndex > 0)
            {
                iIndex -= 1;
                DataBinding();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (iIndex <m_dtLineData.Rows.Count-1)
            {
                iIndex += 1;
                DataBinding();
            }
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            iIndex = m_dtLineData.Rows.Count;
            DataBinding();
        }

        private void btnLogicGraph_Click(object sender, EventArgs e)
        {
            LogicGraph lg = new LogicGraph(UnitNO);
            lg.viewTagSet = m_viewTagValue;
            lg.ShowDialog();
        }

    }
}
