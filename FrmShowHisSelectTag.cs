using HAOCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;  

namespace KSPrj
{
    public partial class FrmShowHisSelectTag : Form
    {
       public ArrayList sTags = new ArrayList();
        //string sTags = "";
        //static string sTags1 = "";
        //static string sTags2 = "";
        //static string sTags3 = "";
        static int iSelCount = 0;
        public ArrayList strTags
        {
            get { return sTags; }
            set { sTags = value; }
        }
        int UnitNO = 5;
        public FrmShowHisSelectTag(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            sTags.Clear();
            this.Close();
            this.Dispose();
        }



        private void FrmShowHisSelectTag_Load(object sender, EventArgs e)
        {
            //DataTable dtOrg = GlobalVariables.dtTagsSet;
            DataTable dtOrg = null;
            if (this.UnitNO == 5)
                dtOrg = GlobalVariables.dtTagsSetFive;
            else if (this.UnitNO == 6)
                dtOrg = GlobalVariables.dtTagsSetSix; 
            if (dtOrg.Rows.Count > 0)
            {
                for (int i = 0; i < dtOrg.Rows.Count; i++)
                {
                    if (dtOrg.Rows[i]["datasourcesNo"].ToString().Trim().Equals("0"))
                    {
                        this.checkedListBox1.Items.Add(new BindTag.TagObject(dtOrg.Rows[i]["tagdesc"].ToString(), dtOrg.Rows[i]["tag"].ToString(), dtOrg.Rows[i]["id"].ToString()), false);
                    }
                    else if (dtOrg.Rows[i]["datasourcesNo"].ToString().Trim().Equals("2"))
                    {
                        this.checkedListBox2.Items.Add(new BindTag.TagObject(dtOrg.Rows[i]["tagdesc"].ToString(), dtOrg.Rows[i]["tag"].ToString(), dtOrg.Rows[i]["id"].ToString()), false);
                    }
                    else
                    {
                        this.checkedListBox3.Items.Add(new BindTag.TagObject(dtOrg.Rows[i]["tagdesc"].ToString(), dtOrg.Rows[i]["tag"].ToString(), dtOrg.Rows[i]["id"].ToString()), false);
                    }
                }
            }
            ////添加计算参数
            //this.checkedListBox2.Items.Add("低压缸进汽压力(绝对)/LowGInPa", false);
            //this.checkedListBox2.Items.Add("凝结水量/NJSL", false);
            //this.checkedListBox2.Items.Add("热网回水流量/HotCycleWater", false);
            //this.checkedListBox2.Items.Add("机组抽汽量/JZCQL", false);
            //this.checkedListBox2.Items.Add("热网回水温度/HotCycleWD", false);
            //this.checkedListBox2.Items.Add("凝汽器出水温度/NQQOutWaterWD", false);
            //this.checkedListBox2.Items.Add("凝汽器吸收热量/NQQXSHot", false);
            //this.checkedListBox2.Items.Add("机组对外供热量/JZOutGRHot", false);
            //this.checkedListBox2.Items.Add("空冷岛排放热量/KLIsLandOutHot", false);
            //this.checkedListBox2.Items.Add("发电煤耗/RealMH", false);
            //this.checkedListBox2.Items.Add("机组热耗/JZHotWast", false);
            //this.checkedListBox2.Items.Add("低压缸排气压力（绝对）/LowGOutPa", false);
            //this.checkedListBox2.Items.Add("低压缸排汽流量/LowGOutLL", false);
            //this.checkedListBox2.Items.Add("空冷岛抽汽量/LKInSteamMou", false);

            ////添加原始测点
            //string sSql = "select tag,tagdesc from tagInfo order by tag";
            //DataTable dtOrg = SQLHelper.ExecuteDt(sSql);
            //if (dtOrg.Rows.Count > 0)
            //{
            //    for (int i = 0; i < dtOrg.Rows.Count; i++)
            //    {
            //        this.checkedListBox3.Items.Add(dtOrg.Rows[i]["tagdesc"].ToString() + "/" + dtOrg.Rows[i]["tag"].ToString(), false);
            //    }
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            iSelCount = this.checkedListBox1.CheckedItems.Count + this.checkedListBox2.CheckedItems.Count + this.checkedListBox3.CheckedItems.Count;
            
            BindTag.TagObject sTemp = null;
            //string []arrTemp
            if (iSelCount > 5)
            {
                MessageBox.Show("最多选定 5 项！");
                return;
            }
            else if (iSelCount == 0)
            {
                MessageBox.Show("至少选定 1 项！");
                return;
            }
            else
            {
                for (int i = 0; i < this.checkedListBox1.CheckedItems.Count; i++)
                {
                    sTemp = (BindTag.TagObject)this.checkedListBox1.CheckedItems[i];
                    sTags.Add(sTemp);
                }

                for (int i = 0; i < this.checkedListBox2.CheckedItems.Count; i++)
                {
                    sTemp = (BindTag.TagObject)this.checkedListBox2.CheckedItems[i];
                    sTags.Add(sTemp);
                }

                for (int i = 0; i < this.checkedListBox3.CheckedItems.Count; i++)
                {
                    sTemp = (BindTag.TagObject) this.checkedListBox3.CheckedItems[i];
                    sTags.Add(sTemp);
                }
            }
            this.Close();
            
        }
    }
}
