using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YHYXYH.YXYH;
using System.Collections;
using System.ServiceProcess;
using System.Diagnostics;
using HAOCommon;

namespace YHYXYH.Tool
{
    public partial class TagImport : Form
    {
        public TagImport()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBox1.Text = openFileDialog1.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sIDs = "";
            DataTable dtExcel;
            DataTable dtData = SQLHelper.ExecuteDt("select * from tags where 1=0");
            DataView viewYXYHtags = SQLHelper.ExecuteDt("select * from tags").DefaultView;
            if (textBox1.Text.Length > 0 & System.IO.File.Exists(textBox1.Text))
            {
                dtExcel = ExcelHelper.ImportExceltoDt(textBox1.Text, 0, 2);
                int iNo = 4;
                string sTagDesc = "";
                string sDataSourcesName = "";
                string sTag = "";
                DataRow rowNew = null;
                string sSql = "";
                ArrayList lstSql = new ArrayList();
                foreach (DataRow row in dtExcel.Rows)
                {
                    if (row[0].ToString().Length > 1)
                    {
                        viewYXYHtags.RowFilter = "[id]=" + iNo;
                        if (viewYXYHtags.Count == 0)
                        {
                            sIDs += "," + iNo;
                            rowNew = dtData.NewRow();
                            rowNew["id"] = iNo;
                            rowNew["ExcelCell"] = "F" + iNo;
                            rowNew["tagdesc"] = row[0];
                            rowNew["symbol"] = row[1];
                            rowNew["unit"] = row[2];
                            sDataSourcesName = row[3].ToString().Trim();
                            rowNew["DataSourcesName"] = sDataSourcesName;
                            if (sDataSourcesName == "测量")
                                rowNew["DataSourcesNo"] = 1;
                            if (sDataSourcesName == "计算")
                                rowNew["DataSourcesNo"] = 2;
                            if (sDataSourcesName == "取定值")
                                rowNew["DataSourcesNo"] = 3;
                            try { rowNew["SetValue"] = double.Parse(row[5].ToString().Trim()); }
                            catch { }
                            sTag = row[7].ToString().Trim();
                            if (sTag.Length > 2)
                                rowNew["tag"] = sTag;
                            dtData.Rows.Add(rowNew);
                        }
                        else
                        {
                            sSql = "update [tags] set ";
                            sTag = row[7].ToString().Trim();
                            if (sTag.Length > 2)
                                sSql += "tag='" + sTag + "',";
                            sSql += "tagdesc='" + row[0].ToString() + "',";
                            sSql += "unit='" + row[2].ToString() + "' where [id]=" + iNo;
                            lstSql.Add(sSql);
                        }
                    }
                    iNo += 1;
                }
                if (dtData.Rows.Count > 0)
                {
                    //SQLHelper.ExecuteSql("truncate table YXYHtags");
                    sIDs = sIDs.Trim(',');
                    SQLHelper.insertDataTable(dtData, "tags");
                }
                if (lstSql.Count > 0)
                {
                    SQLHelper.ExecuteSql(lstSql);
                }
                MessageBox.Show("导入完成！");
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            //DataTable dt = SQLHelper.ExecuteDt("select * from tags where (id<2000 or id>3000) and DataSourcesNo=1");
            //string sTags = "";
            //foreach (DataRow row in dt.Rows)
            //{
            //    foreach (string sTag in row["tag"].ToString().Split(','))
            //    {
            //        sTags += "CJPP:01" + sTag + "\r\n";
            //    }
            //}
            //WriteLog.WriteLogs(sTags);

            int i=0;
            double v=0;
            WinHost.WASPCN.P2VG(0.045, ref v, ref i);
            WinHost.WASPCN.P2VG97(0.045, ref v, ref i);
            WinHost.WASPCN.P2VG97(0.045, ref v, ref i);

            //bool bIsRestart = false;
            //try
            //{
            //    System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("sqlservr");
            //    foreach (Process p in processes)
            //    {
            //        if (p.PagedMemorySize64 > 100 * 1000 * 1000)
            //        {
            //            bIsRestart = true;
            //            break;
            //        }
            //    }
            //}
            //catch { }
            //try
            //{
            //    if (bIsRestart == false)
            //        return;
            //    ServiceController service = new ServiceController("MSSQL$SQL2008");
            //    if (service.Status == ServiceControllerStatus.Running)
            //    {
            //        service.Stop();
            //        service.WaitForStatus(ServiceControllerStatus.Stopped);
            //        if (service.Status == ServiceControllerStatus.Stopped)
            //            service.Start();

            //    }

            //}
            //catch (Exception)
            //{
            //}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sID = "";
            ArrayList lstSql = new ArrayList();
            //DataTable dt = SQLHelper.ExecuteDt("select * from tags");
            DataTable dt = SQLHelper.ExecuteDt("select * from TagLabelBinding");
            string sIDs = "";
            foreach (DataRow row in dt.Rows)
            {
                //if (row["id"].ToString() != row["ExcelCell"].ToString().Replace("F",""))
                //    sIDs += "," + row["id"].ToString();

                ////sID=((int)row["id"]).ToString("1000");
                ////lstSql.Add("update tags set [id]="+sID+",ExcelCell='F"+sID+"' where [id]="+row["id"].ToString());


                sID = ((int)row["TagID"]).ToString("1000");
                lstSql.Add("update TagLabelBinding set [TagID]=" + sID + " where LabelName='" + row["LabelName"].ToString() + "'");
            }
            MessageBox.Show(sIDs);
            SQLHelper.ExecuteSqlReturnState(lstSql);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string sID = "";
            ArrayList lstSql = new ArrayList();
            DataTable dt = SQLHelper.ExecuteDt("select * from tags where id<2000");
            foreach (DataRow row in dt.Rows)
            {
                sID = "2" + row["id"].ToString().Substring(1);
                //lstSql.Add("update tags set [id]=" + sID + ",ExcelCell='F" + sID + "' where [id]=" + row["id"].ToString());
                row["id"] = int.Parse(sID);
                row["ExcelCell"] = "F" + sID;
            }
            SQLHelper.insertDataTable(dt, "tags");
        }

        // 内插法求低压缸排汽流量(按线性关系计算）
        /// <summary>
        /// 内插法求低压缸排汽流量(按线性关系计算）
        /// </summary>
        /// <returns></returns>
        private double CalF1324()
        {
            double dReturn = 0;
            decimal[] dYaLis = { 0.1m, 0.2m, 0.24m, 0.3m, 0.4m, 0.5m, 0.6m };//低压缸进汽压力
            decimal[] dLiuLiang = { 139.4m, 280.7m, 336.8m, 404.5m, 524.1m, 633.7m, 743.4m };//低压缸排汽流量
            decimal dF1111 = decimal.Parse(textBox1.Text.Trim());
            decimal dX1 = 0, dX2 = 0, dY1 = 0, dY2 = 0;
            if (dF1111 > dYaLis[dYaLis.Length - 1])
            {
                dX1 = dYaLis[dYaLis.Length - 2];
                dX2 = dYaLis[dYaLis.Length - 1];
                dY1 = dLiuLiang[dLiuLiang.Length - 2];
                dY2 = dLiuLiang[dLiuLiang.Length - 1];
            }
            else
            {
                for (int i = 1; i < dYaLis.Length; i++)
                {
                    if (dF1111 <= dYaLis[i])
                    {
                        dX1 = dYaLis[i - 1];
                        dX2 = dYaLis[i];
                        dY1 = dLiuLiang[i - 1];
                        dY2 = dLiuLiang[i];
                        break;
                    }
                }
            }
            dReturn = (double)(dY1 + (dF1111 - dX1) * (dY2 - dY1) / (dX2 - dX1));
            return dReturn;
        }

        private void button6_Click(object sender, EventArgs e)
        {

            string sIDs = "";
            DataTable dtExcel;
            if (textBox1.Text.Length > 0 & System.IO.File.Exists(textBox1.Text))
            {
                dtExcel = ExcelHelper.ImportExceltoDt(textBox1.Text, 0, 2);
                int iNo = 4;
                string sTagDesc = "";
                string sDataSourcesName = "";
                string sTag = "";
                DataRow rowNew = null;
                string sSql = "";
                ArrayList lstSql = new ArrayList();
                foreach (DataRow row in dtExcel.Rows)
                {
                    if (row[0].ToString().Length > 1)
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            try
                            {
                                double dValue = double.Parse(row[5].ToString().Trim());
                                sSql = "update [tags] set StandardValue=" + dValue + " where [id]=" + iNo.ToString(i + "000");
                                SQLHelper.ExecuteSql(sSql);
                            }
                            catch { }
                        }
                    }
                    iNo += 1;
                }
                //if (lstSql.Count > 0)
                //{
                //    SQLHelper.ExecuteSql(lstSql);
                //}
                MessageBox.Show("导入完成！");
            }
        }
    }
}
