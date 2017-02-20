using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KSPrj.YCKZ
{
    public partial class YCKZFormSix : Form
    {
        public YCKZFormSix()
        {
            InitializeComponent();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Program.mainForm.Show();
            this.Visible = false;
        }
    }
}
