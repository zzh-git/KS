namespace YHYXYH.Tool
{
    partial class TagLabelBinding
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagLabelBinding));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnLogicGraph = new System.Windows.Forms.Button();
            this.btnEnd = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.btnFirst = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.dateTimePickerEnd2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerEnd1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerBegin2 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerBegin1 = new System.Windows.Forms.DateTimePicker();
            this.button1 = new System.Windows.Forms.Button();
            this.btnRemoveBinding = new System.Windows.Forms.Button();
            this.btnDind = new System.Windows.Forms.Button();
            this.lblIsBinded = new System.Windows.Forms.Label();
            this.lblText = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnQuery = new System.Windows.Forms.Button();
            this.txtQuery = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.TagDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.replaceValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StandardValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TagID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnLogicGraph);
            this.panel1.Controls.Add(this.btnEnd);
            this.panel1.Controls.Add(this.btnNext);
            this.panel1.Controls.Add(this.btnLast);
            this.panel1.Controls.Add(this.btnFirst);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.dateTimePickerEnd2);
            this.panel1.Controls.Add(this.dateTimePickerEnd1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dateTimePickerBegin2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dateTimePickerBegin1);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnRemoveBinding);
            this.panel1.Controls.Add(this.btnDind);
            this.panel1.Controls.Add(this.lblIsBinded);
            this.panel1.Controls.Add(this.lblText);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(728, 34);
            this.panel1.TabIndex = 0;
            // 
            // btnLogicGraph
            // 
            this.btnLogicGraph.Location = new System.Drawing.Point(-51, 2);
            this.btnLogicGraph.Name = "btnLogicGraph";
            this.btnLogicGraph.Size = new System.Drawing.Size(38, 23);
            this.btnLogicGraph.TabIndex = 18;
            this.btnLogicGraph.Text = "逻辑";
            this.btnLogicGraph.UseVisualStyleBackColor = true;
            this.btnLogicGraph.Visible = false;
            this.btnLogicGraph.Click += new System.EventHandler(this.btnLogicGraph_Click);
            // 
            // btnEnd
            // 
            this.btnEnd.Location = new System.Drawing.Point(1228, 2);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(28, 23);
            this.btnEnd.TabIndex = 17;
            this.btnEnd.Text = "末";
            this.btnEnd.UseVisualStyleBackColor = true;
            this.btnEnd.Visible = false;
            this.btnEnd.Click += new System.EventHandler(this.btnEnd_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(1171, 2);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(51, 23);
            this.btnNext.TabIndex = 16;
            this.btnNext.Text = "下一条";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Visible = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnLast
            // 
            this.btnLast.Location = new System.Drawing.Point(1114, 2);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(51, 23);
            this.btnLast.TabIndex = 15;
            this.btnLast.Text = "上一条";
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Visible = false;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // btnFirst
            // 
            this.btnFirst.Location = new System.Drawing.Point(1080, 2);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(28, 23);
            this.btnFirst.TabIndex = 14;
            this.btnFirst.Text = "首";
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Visible = false;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(989, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 13;
            this.button2.Text = "查  询";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dateTimePickerEnd2
            // 
            this.dateTimePickerEnd2.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerEnd2.Location = new System.Drawing.Point(909, 4);
            this.dateTimePickerEnd2.Name = "dateTimePickerEnd2";
            this.dateTimePickerEnd2.ShowUpDown = true;
            this.dateTimePickerEnd2.Size = new System.Drawing.Size(74, 21);
            this.dateTimePickerEnd2.TabIndex = 12;
            this.dateTimePickerEnd2.Visible = false;
            // 
            // dateTimePickerEnd1
            // 
            this.dateTimePickerEnd1.Location = new System.Drawing.Point(801, 4);
            this.dateTimePickerEnd1.Name = "dateTimePickerEnd1";
            this.dateTimePickerEnd1.Size = new System.Drawing.Size(102, 21);
            this.dateTimePickerEnd1.TabIndex = 11;
            this.dateTimePickerEnd1.Visible = false;
            this.dateTimePickerEnd1.ValueChanged += new System.EventHandler(this.dateTimePickerEnd1_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(778, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "至";
            this.label2.Visible = false;
            // 
            // dateTimePickerBegin2
            // 
            this.dateTimePickerBegin2.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerBegin2.Location = new System.Drawing.Point(698, 4);
            this.dateTimePickerBegin2.Name = "dateTimePickerBegin2";
            this.dateTimePickerBegin2.ShowUpDown = true;
            this.dateTimePickerBegin2.Size = new System.Drawing.Size(74, 21);
            this.dateTimePickerBegin2.TabIndex = 9;
            this.dateTimePickerBegin2.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(548, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "日期：";
            this.label1.Visible = false;
            // 
            // dateTimePickerBegin1
            // 
            this.dateTimePickerBegin1.Location = new System.Drawing.Point(589, 4);
            this.dateTimePickerBegin1.Name = "dateTimePickerBegin1";
            this.dateTimePickerBegin1.Size = new System.Drawing.Size(103, 21);
            this.dateTimePickerBegin1.TabIndex = 7;
            this.dateTimePickerBegin1.Visible = false;
            this.dateTimePickerBegin1.ValueChanged += new System.EventHandler(this.dateTimePickerBegin1_ValueChanged);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(278, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(58, 21);
            this.button1.TabIndex = 4;
            this.button1.Text = "导出";
            this.toolTip1.SetToolTip(this.button1, "请在项目文本框中输入要查询的项，结果会显示在下面的列表中。");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnRemoveBinding
            // 
            this.btnRemoveBinding.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRemoveBinding.Location = new System.Drawing.Point(481, 6);
            this.btnRemoveBinding.Name = "btnRemoveBinding";
            this.btnRemoveBinding.Size = new System.Drawing.Size(58, 21);
            this.btnRemoveBinding.TabIndex = 3;
            this.btnRemoveBinding.Text = "解绑";
            this.toolTip1.SetToolTip(this.btnRemoveBinding, "要解除绑定，请点击此“解绑”按钮。");
            this.btnRemoveBinding.UseVisualStyleBackColor = true;
            this.btnRemoveBinding.Visible = false;
            this.btnRemoveBinding.Click += new System.EventHandler(this.btnRemoveBinding_Click);
            // 
            // btnDind
            // 
            this.btnDind.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDind.Location = new System.Drawing.Point(417, 4);
            this.btnDind.Name = "btnDind";
            this.btnDind.Size = new System.Drawing.Size(58, 21);
            this.btnDind.TabIndex = 2;
            this.btnDind.Text = "绑定button1";
            this.toolTip1.SetToolTip(this.btnDind, "请点击下面列表中要绑定的行，然后点击此“绑定”按钮。");
            this.btnDind.UseVisualStyleBackColor = true;
            this.btnDind.Click += new System.EventHandler(this.btnDind_Click);
            // 
            // lblIsBinded
            // 
            this.lblIsBinded.AutoSize = true;
            this.lblIsBinded.Location = new System.Drawing.Point(86, 8);
            this.lblIsBinded.Name = "lblIsBinded";
            this.lblIsBinded.Size = new System.Drawing.Size(59, 12);
            this.lblIsBinded.TabIndex = 1;
            this.lblIsBinded.Text = "是否绑定 ";
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(15, 8);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(47, 12);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "lblText";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnQuery);
            this.panel2.Controls.Add(this.txtQuery);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 34);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(728, 38);
            this.panel2.TabIndex = 2;
            this.panel2.DoubleClick += new System.EventHandler(this.panel2_DoubleClick);
            // 
            // btnQuery
            // 
            this.btnQuery.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnQuery.Location = new System.Drawing.Point(417, 5);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(58, 21);
            this.btnQuery.TabIndex = 3;
            this.btnQuery.Text = "查询";
            this.toolTip1.SetToolTip(this.btnQuery, "请在项目文本框中输入要查询的项，结果会显示在下面的列表中。");
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // txtQuery
            // 
            this.txtQuery.Location = new System.Drawing.Point(56, 7);
            this.txtQuery.Name = "txtQuery";
            this.txtQuery.Size = new System.Drawing.Size(336, 21);
            this.txtQuery.TabIndex = 2;
            this.txtQuery.TextChanged += new System.EventHandler(this.txtQuery_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "项目";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TagDesc,
            this.unit,
            this.value,
            this.replaceValue,
            this.StandardValue,
            this.valueType,
            this.TagID,
            this.tag});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 72);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(728, 460);
            this.dataGridView1.TabIndex = 4;
            this.toolTip1.SetToolTip(this.dataGridView1, "请点击此列表中要绑定的行，然后点击“绑定”按钮。");
            // 
            // TagDesc
            // 
            this.TagDesc.DataPropertyName = "tagdesc";
            this.TagDesc.HeaderText = "项目";
            this.TagDesc.Name = "TagDesc";
            this.TagDesc.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.TagDesc.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.TagDesc.Width = 140;
            // 
            // unit
            // 
            this.unit.DataPropertyName = "unit";
            this.unit.HeaderText = "单位";
            this.unit.Name = "unit";
            this.unit.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.unit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // value
            // 
            this.value.DataPropertyName = "TagValue";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.value.DefaultCellStyle = dataGridViewCellStyle1;
            this.value.HeaderText = "数值";
            this.value.Name = "value";
            this.value.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // replaceValue
            // 
            this.replaceValue.DataPropertyName = "setvalue";
            this.replaceValue.HeaderText = "替代值";
            this.replaceValue.Name = "replaceValue";
            // 
            // StandardValue
            // 
            this.StandardValue.DataPropertyName = "StandardValue";
            this.StandardValue.HeaderText = "标准值";
            this.StandardValue.Name = "StandardValue";
            this.StandardValue.Visible = false;
            // 
            // valueType
            // 
            this.valueType.DataPropertyName = "DataSourcesName";
            this.valueType.HeaderText = "取值方式";
            this.valueType.Name = "valueType";
            this.valueType.Visible = false;
            // 
            // TagID
            // 
            this.TagID.DataPropertyName = "id";
            this.TagID.HeaderText = "测点序号";
            this.TagID.Name = "TagID";
            // 
            // tag
            // 
            this.tag.DataPropertyName = "tag";
            this.tag.HeaderText = "测点";
            this.tag.Name = "tag";
            this.tag.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.tag.Width = 120;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // TagLabelBinding
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 532);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TagLabelBinding";
            this.Text = "测点绑定";
            this.Load += new System.EventHandler(this.TagLabelBinding_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDind;
        private System.Windows.Forms.Label lblIsBinded;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.TextBox txtQuery;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnRemoveBinding;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnEnd;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd2;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePickerBegin2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerBegin1;
        private System.Windows.Forms.Button btnLogicGraph;
        private System.Windows.Forms.DataGridViewTextBoxColumn TagDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn unit;
        private System.Windows.Forms.DataGridViewTextBoxColumn value;
        private System.Windows.Forms.DataGridViewTextBoxColumn replaceValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn StandardValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueType;
        private System.Windows.Forms.DataGridViewTextBoxColumn TagID;
        private System.Windows.Forms.DataGridViewTextBoxColumn tag;
    }
}