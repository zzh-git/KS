namespace YHYXYH.YXYH
{
    partial class AlarmListYXYH
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlarmListYXYH));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.AlarmTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlarmText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.runOperate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlarmDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlarmState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlarmEndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AlarmLasts = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnQuery = new System.Windows.Forms.Button();
            this.dateTimePickerEnd1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerBegin1 = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AlarmTime,
            this.AlarmText,
            this.runOperate,
            this.AlarmDesc,
            this.AlarmState,
            this.AlarmEndTime,
            this.AlarmLasts});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(1028, 481);
            this.dataGridView1.TabIndex = 0;
            // 
            // AlarmTime
            // 
            this.AlarmTime.DataPropertyName = "AlarmTime";
            this.AlarmTime.HeaderText = "报警开始时间";
            this.AlarmTime.Name = "AlarmTime";
            this.AlarmTime.Width = 130;
            // 
            // AlarmText
            // 
            this.AlarmText.DataPropertyName = "AlarmText";
            this.AlarmText.HeaderText = "报警内容";
            this.AlarmText.Name = "AlarmText";
            this.AlarmText.Width = 260;
            // 
            // runOperate
            // 
            this.runOperate.DataPropertyName = "[Operation]";
            this.runOperate.HeaderText = "操作";
            this.runOperate.Name = "runOperate";
            // 
            // AlarmDesc
            // 
            this.AlarmDesc.DataPropertyName = "AlarmDesc";
            this.AlarmDesc.HeaderText = "拒绝原因";
            this.AlarmDesc.Name = "AlarmDesc";
            this.AlarmDesc.Width = 150;
            // 
            // AlarmState
            // 
            this.AlarmState.DataPropertyName = "AlarmState";
            this.AlarmState.HeaderText = "报警状态";
            this.AlarmState.Name = "AlarmState";
            // 
            // AlarmEndTime
            // 
            this.AlarmEndTime.DataPropertyName = "AlarmEndTime";
            this.AlarmEndTime.HeaderText = "报警结束时间";
            this.AlarmEndTime.Name = "AlarmEndTime";
            this.AlarmEndTime.Width = 130;
            // 
            // AlarmLasts
            // 
            this.AlarmLasts.DataPropertyName = "AlarmLasts";
            this.AlarmLasts.HeaderText = "报警持续时间";
            this.AlarmLasts.Name = "AlarmLasts";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(660, 9);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 1;
            this.btnExport.Text = "导出";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(777, 9);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1028, 481);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnQuery);
            this.panel2.Controls.Add(this.dateTimePickerEnd1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.dateTimePickerBegin1);
            this.panel2.Controls.Add(this.btnExport);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 481);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1028, 43);
            this.panel2.TabIndex = 4;
            // 
            // btnQuery
            // 
            this.btnQuery.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnQuery.Location = new System.Drawing.Point(464, 8);
            this.btnQuery.Margin = new System.Windows.Forms.Padding(4);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(90, 24);
            this.btnQuery.TabIndex = 18;
            this.btnQuery.Text = "查  询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // dateTimePickerEnd1
            // 
            this.dateTimePickerEnd1.Location = new System.Drawing.Point(291, 9);
            this.dateTimePickerEnd1.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerEnd1.Name = "dateTimePickerEnd1";
            this.dateTimePickerEnd1.Size = new System.Drawing.Size(138, 21);
            this.dateTimePickerEnd1.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(262, 13);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "至";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(65, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "日期：";
            // 
            // dateTimePickerBegin1
            // 
            this.dateTimePickerBegin1.Location = new System.Drawing.Point(111, 9);
            this.dateTimePickerBegin1.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerBegin1.Name = "dateTimePickerBegin1";
            this.dateTimePickerBegin1.Size = new System.Drawing.Size(141, 21);
            this.dateTimePickerBegin1.TabIndex = 14;
            // 
            // AlarmListYXYH
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 524);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "AlarmListYXYH";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "报警历史";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AlarmList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn AlarmTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn AlarmText;
        private System.Windows.Forms.DataGridViewTextBoxColumn runOperate;
        private System.Windows.Forms.DataGridViewTextBoxColumn AlarmDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn AlarmState;
        private System.Windows.Forms.DataGridViewTextBoxColumn AlarmEndTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn AlarmLasts;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerBegin1;
    }
}