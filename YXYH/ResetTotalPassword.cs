using HAOCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YHYXYH.YXYH
{
    public partial class ResetTotalPassword : Form
    {
        public ResetTotalPassword()
        {
            InitializeComponent();
        }
        public string Password = "";
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Password = textBox1.Text.Trim();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ResetTotalPassword_Load(object sender, EventArgs e)
        {
            if (GlobalVariables.IsCanMoveLabel == true)
                btnResetAvgValue.Visible = true;
            else
                btnResetAvgValue.Visible = false;
        }

        private void btnResetAvgValue_Click(object sender, EventArgs e)
        {
            ////这里要修改  ZZH 2016-9-17 
            //WinHost.CalTree.ResetAvgValue();
        }

        private void ResetTotalPassword_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose(true);
        }
    }
}
