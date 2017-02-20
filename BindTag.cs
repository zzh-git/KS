using HAOCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms; 


namespace KSPrj
{
    public partial class BindTag : Form
    {
        TagObject sTags ;
        string sTagId = "";
        static int iSelCount = 0;
        int UnitNO = 5;
        public BindTag(int unitNo)
        {
            InitializeComponent();
            this.UnitNO = unitNo;
        }
        public TagObject strTags
        {
            get { return sTags; }
            set { sTags = value; }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            sTags = null;
            this.Close();
            this.Dispose();
        }
        private void FrmShowHisSelectTag_Load(object sender, EventArgs e)
        {
            DataTable dtOrg = null;
            if(this.UnitNO == 5)
                dtOrg = GlobalVariables.dtTagsSetFive; 
            else if(this.UnitNO == 6)
                dtOrg = GlobalVariables.dtTagsSetSix; 
            if (dtOrg.Rows.Count > 0)
            {
                for (int i = 0; i < dtOrg.Rows.Count; i++)
                {
                    if (dtOrg.Rows[i]["datasourcesNo"].ToString().Trim().Equals("2"))
                    {
                        this.checkedListBox2.Items.Add(new TagObject(dtOrg.Rows[i]["tagdesc"].ToString(), dtOrg.Rows[i]["tag"].ToString(), dtOrg.Rows[i]["id"].ToString()), false);
                    }
                    else
                    {
                        this.checkedListBox3.Items.Add(new TagObject(dtOrg.Rows[i]["tagdesc"].ToString(), dtOrg.Rows[i]["tag"].ToString(), dtOrg.Rows[i]["id"].ToString()), false);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            iSelCount =this.checkedListBox2.CheckedItems.Count + this.checkedListBox3.CheckedItems.Count;
            if (iSelCount > 1)
            {
                MessageBox.Show("最多选定 1 项！");
                return;
            }
            else if (iSelCount == 0)
            {
                MessageBox.Show("至少选定 1 项！");
                return;
            }
            else
            {
                for (int i = 0; i < this.checkedListBox2.CheckedItems.Count; i++)
                {
                    //sTags = (TagObject)this.checkedListBox2.CheckedItems[i];//////////////////
                    strTags = (TagObject)this.checkedListBox2.CheckedItems[i];
                }
                for (int i = 0; i < this.checkedListBox3.CheckedItems.Count; i++)
                {
                    //sTags = (TagObject)this.checkedListBox3.CheckedItems[i];//////////////////
                    strTags = (TagObject)this.checkedListBox3.CheckedItems[i];
                }
            }
            this.Close();
            
        }

       public class TagObject
        {
            public string tagdesc;
            public string tag;
            public string id;
           public TagObject(string tagdesc,string id)
           {
               this.tagdesc = tagdesc;
               this.id = id;
           }

           public TagObject(string tagdesc,string tag,string id)
            {
                this.tagdesc = tagdesc;
                this.tag = tag;
                this.id = id;
            }
           public override string ToString()
           {
               if (string.IsNullOrEmpty(tag))
               {
                   return tagdesc;
               }else
               {
                   return tagdesc + "/" + tag; 
               }
           }
        }
    }
}
