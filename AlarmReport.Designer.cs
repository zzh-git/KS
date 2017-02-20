namespace KSPrj
{
    partial class AlarmReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlarmReport));
            this.promteText = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.noticeLabel = new System.Windows.Forms.Label();
            this.UnitName = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // promteText
            // 
            this.promteText.BackColor = System.Drawing.Color.Black;
            this.promteText.Dock = System.Windows.Forms.DockStyle.Top;
            this.promteText.Font = new System.Drawing.Font("宋体", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.promteText.ForeColor = System.Drawing.Color.Lime;
            this.promteText.Location = new System.Drawing.Point(0, 0);
            this.promteText.Name = "promteText";
            this.promteText.Size = new System.Drawing.Size(684, 129);
            this.promteText.TabIndex = 4;
            this.promteText.Text = "请及时处理";
            this.promteText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOK
            // 
            this.btnOK.BackgroundImage = global::KSPrj.Properties.Resources.btnbg;
            this.btnOK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOK.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Location = new System.Drawing.Point(259, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(128, 59);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "确认";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Black;
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 338);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(684, 74);
            this.panel1.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::KSPrj.Properties.Resources.btnbg;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(476, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 59);
            this.button1.TabIndex = 6;
            this.button1.Text = "隐藏";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // noticeLabel
            // 
            this.noticeLabel.BackColor = System.Drawing.Color.Black;
            this.noticeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noticeLabel.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.noticeLabel.ForeColor = System.Drawing.Color.Lime;
            this.noticeLabel.Location = new System.Drawing.Point(0, 129);
            this.noticeLabel.Name = "noticeLabel";
            this.noticeLabel.Size = new System.Drawing.Size(684, 148);
            this.noticeLabel.TabIndex = 7;
            this.noticeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UnitName
            // 
            this.UnitName.Dock = System.Windows.Forms.DockStyle.Top;
            this.UnitName.Font = new System.Drawing.Font("宋体", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UnitName.ForeColor = System.Drawing.Color.White;
            this.UnitName.Location = new System.Drawing.Point(0, 0);
            this.UnitName.Name = "UnitName";
            this.UnitName.Size = new System.Drawing.Size(684, 61);
            this.UnitName.TabIndex = 8;
            this.UnitName.Text = "#5机组报警";
            this.UnitName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.noticeLabel);
            this.panel2.Controls.Add(this.promteText);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 61);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(684, 277);
            this.panel2.TabIndex = 9;
            // 
            // AlarmReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(684, 412);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.UnitName);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AlarmReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "安全监测报警";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label promteText;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label noticeLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label UnitName;
        private System.Windows.Forms.Panel panel2;
    }
}