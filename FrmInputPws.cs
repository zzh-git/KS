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
    public partial class FrmInputPws : Form
    {
        public FrmInputPws(string mes)
        {
            InitializeComponent(); 
            this.label1.Text = mes;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Text = this.textBox1.Text+",ok";
            this.Close();
            //this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Text = "";
            this.Close();
            this.Dispose();
        }

    }
}
