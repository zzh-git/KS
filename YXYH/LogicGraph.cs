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
    public partial class LogicGraph : Form
    {
        public LogicGraph(int UnitNo=5)
        {
            InitializeComponent();
            iUnitNo = UnitNo;
        }
        int iUnitNo = 5;
        private Hashtable formularTempHs = null;
        public DataView viewTagSet = null;
        private void LogicGraph_Load(object sender, EventArgs e)
        {
            if (iUnitNo == 5)
                formularTempHs = GlobalVariables.FormulaHs1;
            else
                formularTempHs = GlobalVariables.FormulaHs2;
            foreach (DataRow row in viewTagSet.Table.Rows)
            {
                try
                {
                    if (row["DataSourcesNo"].ToString() == "2")
                    {
                        TreeNode node = new TreeNode();
                        node.Text = row["id"] + "：" + row["tagdesc"] + "：RV:" + row["tagvalue"] + ", SV:" + row["standardvalue"];
                        node.Text += ", ST:" + row["setvalue"] + ", F=" + formularTempHs["F" + row["id"]];
                        treeView1.Nodes.Add(node);
                        addNode(node, row["id"].ToString());
                    }
                }
                catch (Exception ex) { WriteLog.WriteLogs(row["id"].ToString() + row["tagdesc"] + ", F=" + formularTempHs["F" + row["id"]] + "\r\n" + ex.ToString()); }
            }

        }
        void addNode(TreeNode node, String sID)
        {
            string sF = formularTempHs["F" + sID].ToString();
            int iIndex = 0;
            while (sF.IndexOf('F', iIndex) >= 0)
            {
                iIndex = sF.IndexOf('F', iIndex);
                string sNewID = sF.Substring(iIndex + 1, 4);
                try
                {
                    viewTagSet.RowFilter = "[id]=" + sNewID;
                    if (viewTagSet.Count > 0)
                    {
                        TreeNode nodeNew = new TreeNode();
                        nodeNew.Text = viewTagSet[0]["id"] + "：" + viewTagSet[0]["tagdesc"] + "：RV:" + viewTagSet[0]["tagvalue"]
                            + ", SV:" + viewTagSet[0]["standardvalue"] + ", ST:" + viewTagSet[0]["setvalue"] + ", F="
                            + formularTempHs["F" + sNewID];
                        node.Nodes.Add(nodeNew);
                        addNode(nodeNew, sNewID);
                    }
                }
                catch { }
                iIndex += 1;
            }
        }
    }
}
