using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using WinHost;
using HAOCommon;

namespace YHYXYH.YXYH
{
    public partial class SelectTagDate : Form
    {
        int UnitNO = 5;
        System.Windows.Forms.DataVisualization.Charting.Chart chartParent;
        public SelectTagDate(int unitNO, System.Windows.Forms.DataVisualization.Charting.Chart chartParent, string str = "YXYH")
        {
            InitializeComponent();
            if (str == "FCZ")
                this.Text = "选择测点";
            this.UnitNO = unitNO;
            this.chartParent = chartParent;
        }

        public bool bIsClose = false;
        DataView m_viewTagValue = null;
        public string sExcelCells = "";
        public DataTable dtCheckedTags = null;
        private void btnClose_Click(object sender, EventArgs e)
        {
            bIsClose = true;
            this.Close();
        }

        private void SelectTag_Load(object sender, EventArgs e)
        {
            dateTimePickerBegin1.Value = WinHost.TagValue.GetTimeOfFirstData();
            dateTimePickerBegin2.Value = dateTimePickerBegin1.Value;
            dateTimePickerEnd1.Value = DateTime.Now;
            dateTimePickerEnd2.Value = dateTimePickerEnd1.Value;

            if (this.UnitNO == 5)
            {
                GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = "isShow=1 or DataSourcesNo=1";
                m_viewTagValue = GlobalVariables.dtTagsSetFive.DefaultView.ToTable().DefaultView;
                GlobalVariables.dtTagsSetFive.DefaultView.RowFilter = "";
            }
            else if (this.UnitNO == 6)
            {
                GlobalVariables.dtTagsSetSix.DefaultView.RowFilter = "isShow=1 or DataSourcesNo=1";
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
            }
            //this.Close();
            //else
            //{
            //    MessageBox.Show("请选择测点！");
            //}

            //SelectTagDate在生成对象调用时，由ShowDialog()改成了Show()；
            //因为用ShowDialog()时也会造成窗口无反应的假死现象，所以加了下面的语句。add by hlt 2017-1-18
            HistoryLines historyLines = new HistoryLines(UnitNO);
            if (sExcelCells.Length > 0)
            {
                historyLines.SetBeginTime(BeginTime);
                historyLines.SetEndTime(EndTime);
                historyLines.SetExcelCells(sExcelCells);
                historyLines.SetCheckedTags(dtCheckedTags);
            }
            else
            {
                historyLines.seriesCollection = chartParent.Series;
            }
            historyLines.Show();
            //this.Hide();
            this.Close();
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
        // 选择的开始时间
        /// <summary>
        /// 选择的开始时间
        /// </summary>
        public DateTime BeginTime
        {
            get { return dateTimePickerBegin2.Value; }
        }
        // 选择的结束时间
        /// <summary>
        /// 选择的结束时间
        /// </summary>
        public DateTime EndTime
        {
            get { return dateTimePickerEnd2.Value; }
        }

    }
}
