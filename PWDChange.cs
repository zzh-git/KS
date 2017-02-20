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
    public partial class PWDChange : Form
    {
        public PWDChange()
        {
            InitializeComponent();
        }

        //清空 
        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
        }

        //修改 
        private void button2_Click(object sender, EventArgs e)
        {
            string which = "";
            if (this.radioButton1.Checked == true)
                which = "yx";
            if (this.radioButton2.Checked == true)
                which = "rg";

            string oldPwd = this.textBox1.Text.Trim();
            string newPwd = this.textBox2.Text.Trim();
            string reNewPwd = this.textBox3.Text.Trim();

            string pwd = "";
            try
            {
                pwd = SQLHelper.ExecuteScalar("select pws from SysPara where name='" + which + "'"); 
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
            if (oldPwd != pwd) 
            {
                MessageBox.Show("原密码输入不正确！");
                return;
            }
                

            if (string.IsNullOrEmpty(oldPwd) || string.IsNullOrEmpty(newPwd) || string.IsNullOrEmpty(reNewPwd))
                return;
            if (newPwd != reNewPwd) 
            {
                MessageBox.Show("两次输入的密码不一致！");
                return;
            }

            try
            {
                int i = SQLHelper.ExecuteSql("update SysPara set pws='" + newPwd + "' where name='" + which + "'");
                if (i > 0)
                    MessageBox.Show("密码修改成功！");
                this.Close();
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }



        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
