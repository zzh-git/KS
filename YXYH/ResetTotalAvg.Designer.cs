namespace YHYXYH.YXYH
{
    partial class ResetTotalAvg
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResetTotalAvg));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlTotal = new System.Windows.Forms.Panel();
            this.pnlTotalFill = new System.Windows.Forms.Panel();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.pnlTotalTop = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlAvg = new System.Windows.Forms.Panel();
            this.pnlAvgFill = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.pnlAvgTop = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.TagID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TagDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AvgValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resetData = new System.Windows.Forms.DataGridViewButtonColumn();
            this.BeginDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TTagDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TotalValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TReset = new System.Windows.Forms.DataGridViewButtonColumn();
            this.TBeginDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.pnlTotal.SuspendLayout();
            this.pnlTotalFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.pnlTotalTop.SuspendLayout();
            this.pnlAvg.SuspendLayout();
            this.pnlAvgFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.pnlAvgTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlTotal);
            this.panel1.Controls.Add(this.pnlAvg);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(890, 750);
            this.panel1.TabIndex = 3;
            // 
            // pnlTotal
            // 
            this.pnlTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTotal.Controls.Add(this.pnlTotalFill);
            this.pnlTotal.Controls.Add(this.pnlTotalTop);
            this.pnlTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTotal.Location = new System.Drawing.Point(0, 372);
            this.pnlTotal.Name = "pnlTotal";
            this.pnlTotal.Size = new System.Drawing.Size(890, 378);
            this.pnlTotal.TabIndex = 2;
            // 
            // pnlTotalFill
            // 
            this.pnlTotalFill.Controls.Add(this.dataGridView2);
            this.pnlTotalFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTotalFill.Location = new System.Drawing.Point(0, 45);
            this.pnlTotalFill.Name = "pnlTotalFill";
            this.pnlTotalFill.Size = new System.Drawing.Size(888, 331);
            this.pnlTotalFill.TabIndex = 1;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightCyan;
            this.dataGridView2.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Tid,
            this.TTagDesc,
            this.TotalValue,
            this.TUnit,
            this.TReset,
            this.TBeginDate});
            this.dataGridView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView2.Location = new System.Drawing.Point(0, 0);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersWidth = 30;
            this.dataGridView2.RowTemplate.Height = 23;
            this.dataGridView2.Size = new System.Drawing.Size(888, 331);
            this.dataGridView2.TabIndex = 1;
            this.dataGridView2.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_CellContentClick);
            // 
            // pnlTotalTop
            // 
            this.pnlTotalTop.Controls.Add(this.label1);
            this.pnlTotalTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTotalTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTotalTop.Name = "pnlTotalTop";
            this.pnlTotalTop.Size = new System.Drawing.Size(888, 45);
            this.pnlTotalTop.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(888, 45);
            this.label1.TabIndex = 2;
            this.label1.Text = "累计值数据";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlAvg
            // 
            this.pnlAvg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAvg.Controls.Add(this.pnlAvgFill);
            this.pnlAvg.Controls.Add(this.pnlAvgTop);
            this.pnlAvg.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAvg.Location = new System.Drawing.Point(0, 0);
            this.pnlAvg.Name = "pnlAvg";
            this.pnlAvg.Size = new System.Drawing.Size(890, 372);
            this.pnlAvg.TabIndex = 1;
            // 
            // pnlAvgFill
            // 
            this.pnlAvgFill.Controls.Add(this.dataGridView1);
            this.pnlAvgFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAvgFill.Location = new System.Drawing.Point(0, 42);
            this.pnlAvgFill.Name = "pnlAvgFill";
            this.pnlAvgFill.Size = new System.Drawing.Size(888, 328);
            this.pnlAvgFill.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.LightCyan;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TagID,
            this.TagDesc,
            this.AvgValue,
            this.unit,
            this.resetData,
            this.BeginDate});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 30;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(888, 328);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // pnlAvgTop
            // 
            this.pnlAvgTop.Controls.Add(this.label4);
            this.pnlAvgTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAvgTop.Location = new System.Drawing.Point(0, 0);
            this.pnlAvgTop.Name = "pnlAvgTop";
            this.pnlAvgTop.Size = new System.Drawing.Size(888, 42);
            this.pnlAvgTop.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(888, 42);
            this.label4.TabIndex = 1;
            this.label4.Text = "平均值数据";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TagID
            // 
            this.TagID.DataPropertyName = "TagID";
            this.TagID.HeaderText = "TagID";
            this.TagID.Name = "TagID";
            this.TagID.ReadOnly = true;
            this.TagID.Visible = false;
            // 
            // TagDesc
            // 
            this.TagDesc.DataPropertyName = "TagDesc";
            dataGridViewCellStyle9.Format = "g";
            dataGridViewCellStyle9.NullValue = null;
            this.TagDesc.DefaultCellStyle = dataGridViewCellStyle9;
            this.TagDesc.HeaderText = "名称";
            this.TagDesc.Name = "TagDesc";
            this.TagDesc.ReadOnly = true;
            this.TagDesc.Width = 260;
            // 
            // AvgValue
            // 
            this.AvgValue.DataPropertyName = "AvgValue";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.AvgValue.DefaultCellStyle = dataGridViewCellStyle10;
            this.AvgValue.HeaderText = "数值";
            this.AvgValue.Name = "AvgValue";
            this.AvgValue.ReadOnly = true;
            this.AvgValue.Width = 160;
            // 
            // unit
            // 
            this.unit.DataPropertyName = "unit";
            this.unit.HeaderText = "单位";
            this.unit.Name = "unit";
            this.unit.ReadOnly = true;
            // 
            // resetData
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.resetData.DefaultCellStyle = dataGridViewCellStyle11;
            this.resetData.HeaderText = "复  位";
            this.resetData.Name = "resetData";
            this.resetData.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.resetData.Text = "复  位";
            this.resetData.UseColumnTextForButtonValue = true;
            this.resetData.Width = 80;
            // 
            // BeginDate
            // 
            this.BeginDate.DataPropertyName = "BeginDate";
            dataGridViewCellStyle12.Format = "yyyy-MM-dd HH:mm:ss";
            dataGridViewCellStyle12.NullValue = null;
            this.BeginDate.DefaultCellStyle = dataGridViewCellStyle12;
            this.BeginDate.HeaderText = "复位时间";
            this.BeginDate.Name = "BeginDate";
            this.BeginDate.ReadOnly = true;
            this.BeginDate.Width = 200;
            // 
            // Tid
            // 
            this.Tid.DataPropertyName = "id";
            this.Tid.HeaderText = "Tid";
            this.Tid.Name = "Tid";
            this.Tid.ReadOnly = true;
            this.Tid.Visible = false;
            // 
            // TTagDesc
            // 
            this.TTagDesc.DataPropertyName = "TagDesc";
            dataGridViewCellStyle3.Format = "g";
            dataGridViewCellStyle3.NullValue = null;
            this.TTagDesc.DefaultCellStyle = dataGridViewCellStyle3;
            this.TTagDesc.HeaderText = "名称";
            this.TTagDesc.Name = "TTagDesc";
            this.TTagDesc.ReadOnly = true;
            this.TTagDesc.Width = 260;
            // 
            // TotalValue
            // 
            this.TotalValue.DataPropertyName = "TotalValue";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.Format = "N2";
            dataGridViewCellStyle4.NullValue = null;
            this.TotalValue.DefaultCellStyle = dataGridViewCellStyle4;
            this.TotalValue.HeaderText = "数值";
            this.TotalValue.Name = "TotalValue";
            this.TotalValue.ReadOnly = true;
            this.TotalValue.Width = 160;
            // 
            // TUnit
            // 
            this.TUnit.DataPropertyName = "Unit";
            this.TUnit.HeaderText = "单位";
            this.TUnit.Name = "TUnit";
            this.TUnit.ReadOnly = true;
            // 
            // TReset
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TReset.DefaultCellStyle = dataGridViewCellStyle5;
            this.TReset.HeaderText = "复  位";
            this.TReset.Name = "TReset";
            this.TReset.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TReset.Text = "复  位";
            this.TReset.UseColumnTextForButtonValue = true;
            this.TReset.Width = 80;
            // 
            // TBeginDate
            // 
            this.TBeginDate.DataPropertyName = "BeginDate";
            dataGridViewCellStyle6.Format = "yyyy-MM-dd HH:mm:ss";
            dataGridViewCellStyle6.NullValue = null;
            this.TBeginDate.DefaultCellStyle = dataGridViewCellStyle6;
            this.TBeginDate.HeaderText = "复位时间";
            this.TBeginDate.Name = "TBeginDate";
            this.TBeginDate.ReadOnly = true;
            this.TBeginDate.Width = 200;
            // 
            // ResetTotalAvg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 750);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "ResetTotalAvg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "复位平均累计值";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.AlarmList_Load);
            this.Resize += new System.EventHandler(this.ResetTotalAvg_Resize);
            this.panel1.ResumeLayout(false);
            this.pnlTotal.ResumeLayout(false);
            this.pnlTotalFill.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.pnlTotalTop.ResumeLayout(false);
            this.pnlAvg.ResumeLayout(false);
            this.pnlAvgFill.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.pnlAvgTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlTotal;
        private System.Windows.Forms.Panel pnlTotalFill;
        private System.Windows.Forms.Panel pnlTotalTop;
        private System.Windows.Forms.Panel pnlAvg;
        private System.Windows.Forms.Panel pnlAvgFill;
        private System.Windows.Forms.Panel pnlAvgTop;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn TagID;
        private System.Windows.Forms.DataGridViewTextBoxColumn TagDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn AvgValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn unit;
        private System.Windows.Forms.DataGridViewButtonColumn resetData;
        private System.Windows.Forms.DataGridViewTextBoxColumn BeginDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tid;
        private System.Windows.Forms.DataGridViewTextBoxColumn TTagDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn TUnit;
        private System.Windows.Forms.DataGridViewButtonColumn TReset;
        private System.Windows.Forms.DataGridViewTextBoxColumn TBeginDate;
    }
}