namespace YHYXYH.YXYH
{
    partial class SystemGraph
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SystemGraph));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTemp = new System.Windows.Forms.Label();
            this.lblRunOptimizePromptText = new System.Windows.Forms.Label();
            this.timerLineBind = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timerDateTime = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.BackgroundImage = global::KSPrj.Properties.Resources._5系统图;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.lblTemp);
            this.panel1.Controls.Add(this.lblRunOptimizePromptText);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Font = new System.Drawing.Font("宋体", 12F);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(887, 583);
            this.panel1.TabIndex = 0;
            this.panel1.Resize += new System.EventHandler(this.panel1_Resize);
            // 
            // lblTemp
            // 
            this.lblTemp.AutoSize = true;
            this.lblTemp.BackColor = System.Drawing.Color.Transparent;
            this.lblTemp.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblTemp.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.lblTemp.ForeColor = System.Drawing.Color.Lime;
            this.lblTemp.Location = new System.Drawing.Point(713, 0);
            this.lblTemp.Name = "lblTemp";
            this.lblTemp.Size = new System.Drawing.Size(174, 24);
            this.lblTemp.TabIndex = 35;
            this.lblTemp.Text = "环境温度:-1℃";
            // 
            // lblRunOptimizePromptText
            // 
            this.lblRunOptimizePromptText.AutoSize = true;
            this.lblRunOptimizePromptText.BackColor = System.Drawing.Color.Transparent;
            this.lblRunOptimizePromptText.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.lblRunOptimizePromptText.ForeColor = System.Drawing.Color.Lime;
            this.lblRunOptimizePromptText.Location = new System.Drawing.Point(349, 2);
            this.lblRunOptimizePromptText.Name = "lblRunOptimizePromptText";
            this.lblRunOptimizePromptText.Size = new System.Drawing.Size(310, 24);
            this.lblRunOptimizePromptText.TabIndex = 33;
            this.lblRunOptimizePromptText.Text = "运行监测中，运行已优化。\r\n";
            // 
            // timerLineBind
            // 
            this.timerLineBind.Tick += new System.EventHandler(this.timerLineBind_Tick);
            // 
            // timerDateTime
            // 
            this.timerDateTime.Enabled = true;
            this.timerDateTime.Interval = 500;
            this.timerDateTime.Tick += new System.EventHandler(this.timerDateTime_Tick);
            // 
            // SystemGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 583);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "SystemGraph";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "系统图";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SystemGraph_FormClosing);
            this.Load += new System.EventHandler(this.SystemGraph_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer timerLineBind;
        private System.Windows.Forms.Label lblRunOptimizePromptText;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lblTemp;
        private System.Windows.Forms.Timer timerDateTime;

    }
}