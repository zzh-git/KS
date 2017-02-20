using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms; 
using HAOCommon; 

namespace KSPrj
{
    public partial class FrmSetTagValue : Form
    {
        public FrmSetTagValue()
        {
            InitializeComponent();
        }
        private DataTable dt = new DataTable();
        private void FrmSetTagValue_Load(object sender, EventArgs e)
        {
            dt = SQLHelper.ExecuteDt("select tagdesc as 测点名,tag as 测点,unit as 单位," +
                  "up as 上限,down as 下限,SetValue as 设定值,IsSet as s,id as id " +
                  "from fczhtags where datasourcesno<>2 order by id");
            this.dataGridView1.DataSource = dt;
            string ss = "";
            for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++)
            {
                if (string.IsNullOrEmpty(this.dataGridView1.Rows[i].Cells["s"].Value.ToString()))
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = "停用"; // this.dataGridView1.Rows[i].Cells[9].Value;
                }
                ss = this.dataGridView1.Rows[i].Cells["s"].Value.ToString();
                if (ss == "1")
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = "启用"; // this.dataGridView1.Rows[i].Cells[9].Value;
                    this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else
                {
                    this.dataGridView1.Rows[i].Cells[0].Value = "停用";// this.dataGridView1.Rows[i].Cells[9].Value;
                    this.dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }
        //关闭按钮事件
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
        //单击保存按钮事件
        private void button1_Click(object sender, EventArgs e)
        {
            //保存数据，要求用户输入密码方可
            string pwd = SQLHelper.ExecuteScalar("select pws from SysPara where name='rg'");
            //string s
            FrmInputPws fip = new FrmInputPws("请输入（热工检修人员）密码");
            fip.ShowDialog();
            string strRet = fip.Text;

            string[] strRetA = strRet.Split(',');
            if (strRet.Length == 0)
                return;
            if (strRetA[1] == "ok")
            {
                if (pwd == strRetA[0])
                {
                    fip.Dispose();
                    SaveData();
                    //注意，需要区分机组，或者使用其他办法 ZZH 2016-9-17 
                    //TableTags.initDataBase(); 
                }
                else
                {
                    MessageBox.Show("密码错误！");
                    fip.Dispose();
                }
            }
        }

        private void SaveData()
        {
            string ss = "0";
            //遍历grid每行,生成update 语句 保存数据
            string strSql = "";
            for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++)
            {
                ss=this.dataGridView1.Rows[i].Cells["isset"].Value.ToString();
                if (ss == "启用")
                    ss = "1";
                else
                    ss = "0";

                strSql += "update fczhtags set SetValue=" + Convert.ToDouble(this.dataGridView1.Rows[i].Cells["setvalue"].Value)+
                    ",IsSet='" + ss + "' where tag='" + this.dataGridView1.Rows[i].Cells["tag"].Value + "' and id = '"+
                       this.dataGridView1.Rows[i].Cells["id"].Value+"';";
            }

            int iii=SQLHelper.ExecuteSql(strSql);
            MessageBox.Show("数据已保存！");
        }
    }
}
