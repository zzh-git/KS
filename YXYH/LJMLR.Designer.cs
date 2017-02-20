namespace YHYXYH.YXYH
{
    partial class LJMLR
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(321D, 433D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(4342D, 234D);
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint3 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(5543D, 434D);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LJMLR));
            this.timerChartBind = new System.Windows.Forms.Timer(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rdoDataTime = new System.Windows.Forms.RadioButton();
            this.rdoTotalHours = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btnRealTime = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.dateTimePickerEnd2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerEnd1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dateTimePickerBegin2 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePickerBegin1 = new System.Windows.Forms.DateTimePicker();
            this.panel4 = new System.Windows.Forms.Panel();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.lblQueryWait = new System.Windows.Forms.Label();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // timerChartBind
            // 
            this.timerChartBind.Tick += new System.EventHandler(this.timerChartBind_Tick);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1361, 65);
            this.panel3.TabIndex = 66;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.DimGray;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("宋体", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Navy;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(1359, 63);
            this.label4.TabIndex = 0;
            this.label4.Text = "累计毛利润";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DimGray;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnRealTime);
            this.panel1.Controls.Add(this.btnQuery);
            this.panel1.Controls.Add(this.dateTimePickerEnd2);
            this.panel1.Controls.Add(this.dateTimePickerEnd1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dateTimePickerBegin2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.dateTimePickerBegin1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 65);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1361, 58);
            this.panel1.TabIndex = 67;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.rdoDataTime);
            this.panel2.Controls.Add(this.rdoTotalHours);
            this.panel2.Location = new System.Drawing.Point(1092, 8);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(245, 43);
            this.panel2.TabIndex = 24;
            this.panel2.Visible = false;
            // 
            // rdoDataTime
            // 
            this.rdoDataTime.AutoSize = true;
            this.rdoDataTime.Checked = true;
            this.rdoDataTime.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdoDataTime.Location = new System.Drawing.Point(139, 9);
            this.rdoDataTime.Margin = new System.Windows.Forms.Padding(4);
            this.rdoDataTime.Name = "rdoDataTime";
            this.rdoDataTime.Size = new System.Drawing.Size(94, 20);
            this.rdoDataTime.TabIndex = 15;
            this.rdoDataTime.TabStop = true;
            this.rdoDataTime.Text = "日期时间";
            this.rdoDataTime.UseVisualStyleBackColor = true;
            this.rdoDataTime.Visible = false;
            this.rdoDataTime.CheckedChanged += new System.EventHandler(this.rdoDataTime_CheckedChanged);
            // 
            // rdoTotalHours
            // 
            this.rdoTotalHours.AutoSize = true;
            this.rdoTotalHours.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rdoTotalHours.Location = new System.Drawing.Point(13, 11);
            this.rdoTotalHours.Margin = new System.Windows.Forms.Padding(4);
            this.rdoTotalHours.Name = "rdoTotalHours";
            this.rdoTotalHours.Size = new System.Drawing.Size(111, 20);
            this.rdoTotalHours.TabIndex = 14;
            this.rdoTotalHours.Text = "累计小时数";
            this.rdoTotalHours.UseVisualStyleBackColor = true;
            this.rdoTotalHours.Visible = false;
            this.rdoTotalHours.CheckedChanged += new System.EventHandler(this.rdoTotalHours_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(973, 21);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 16);
            this.label3.TabIndex = 23;
            this.label3.Text = "X轴显示方式：";
            this.label3.Visible = false;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(801, 13);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 31);
            this.button1.TabIndex = 18;
            this.button1.Text = "导  出";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnRealTime
            // 
            this.btnRealTime.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRealTime.Location = new System.Drawing.Point(693, 13);
            this.btnRealTime.Margin = new System.Windows.Forms.Padding(4);
            this.btnRealTime.Name = "btnRealTime";
            this.btnRealTime.Size = new System.Drawing.Size(100, 31);
            this.btnRealTime.TabIndex = 17;
            this.btnRealTime.Text = "实时数据";
            this.btnRealTime.UseVisualStyleBackColor = true;
            this.btnRealTime.Click += new System.EventHandler(this.btnRealTime_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnQuery.Location = new System.Drawing.Point(585, 13);
            this.btnQuery.Margin = new System.Windows.Forms.Padding(4);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(100, 31);
            this.btnQuery.TabIndex = 16;
            this.btnQuery.Text = "查  询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // dateTimePickerEnd2
            // 
            this.dateTimePickerEnd2.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerEnd2.Location = new System.Drawing.Point(486, 15);
            this.dateTimePickerEnd2.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerEnd2.Name = "dateTimePickerEnd2";
            this.dateTimePickerEnd2.ShowUpDown = true;
            this.dateTimePickerEnd2.Size = new System.Drawing.Size(91, 26);
            this.dateTimePickerEnd2.TabIndex = 15;
            // 
            // dateTimePickerEnd1
            // 
            this.dateTimePickerEnd1.Location = new System.Drawing.Point(340, 15);
            this.dateTimePickerEnd1.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerEnd1.Name = "dateTimePickerEnd1";
            this.dateTimePickerEnd1.Size = new System.Drawing.Size(138, 26);
            this.dateTimePickerEnd1.TabIndex = 14;
            this.dateTimePickerEnd1.ValueChanged += new System.EventHandler(this.dateTimePickerBegin1_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(308, 23);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 16);
            this.label2.TabIndex = 13;
            this.label2.Text = "至";
            // 
            // dateTimePickerBegin2
            // 
            this.dateTimePickerBegin2.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePickerBegin2.Location = new System.Drawing.Point(209, 15);
            this.dateTimePickerBegin2.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerBegin2.Name = "dateTimePickerBegin2";
            this.dateTimePickerBegin2.ShowUpDown = true;
            this.dateTimePickerBegin2.Size = new System.Drawing.Size(91, 26);
            this.dateTimePickerBegin2.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(1, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "日期：";
            // 
            // dateTimePickerBegin1
            // 
            this.dateTimePickerBegin1.Location = new System.Drawing.Point(61, 15);
            this.dateTimePickerBegin1.Margin = new System.Windows.Forms.Padding(4);
            this.dateTimePickerBegin1.Name = "dateTimePickerBegin1";
            this.dateTimePickerBegin1.Size = new System.Drawing.Size(140, 26);
            this.dateTimePickerBegin1.TabIndex = 10;
            this.dateTimePickerBegin1.ValueChanged += new System.EventHandler(this.dateTimePickerBegin1_ValueChanged);
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.lblQueryWait);
            this.panel4.Controls.Add(this.chart1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 123);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1361, 627);
            this.panel4.TabIndex = 69;
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.Color.DimGray;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.IsStartedFromZero = false;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            chartArea1.AxisX.MajorGrid.Interval = 0D;
            chartArea1.AxisX.MajorGrid.IntervalOffset = 0D;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisX.Title = "供热期累计小时数";
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.IsStartedFromZero = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            chartArea1.AxisY.LabelStyle.Format = "0.####";
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisY.Title = "累计毛利润（万元）";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            chartArea1.AxisY2.IsStartedFromZero = false;
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Margin = new System.Windows.Forms.Padding(4);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Fuchsia;
            series1.Legend = "Legend1";
            series1.Name = "供热累计收入（万元）";
            series1.Points.Add(dataPoint1);
            series1.Points.Add(dataPoint2);
            series1.Points.Add(dataPoint3);
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(1359, 625);
            this.chart1.TabIndex = 69;
            this.chart1.Text = "chart3";
            // 
            // lblQueryWait
            // 
            this.lblQueryWait.AutoSize = true;
            this.lblQueryWait.BackColor = System.Drawing.Color.Black;
            this.lblQueryWait.Font = new System.Drawing.Font("宋体", 42F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblQueryWait.ForeColor = System.Drawing.Color.Red;
            this.lblQueryWait.Location = new System.Drawing.Point(252, 213);
            this.lblQueryWait.Name = "lblQueryWait";
            this.lblQueryWait.Size = new System.Drawing.Size(864, 56);
            this.lblQueryWait.TabIndex = 74;
            this.lblQueryWait.Text = "正在查询数据，请等待，勿操作！";
            this.lblQueryWait.Visible = false;
            // 
            // LJMLR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1361, 750);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.Name = "LJMLR";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "累计毛利润";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LJMLR_FormClosed);
            this.Load += new System.EventHandler(this.LJMLR_Load);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerChartBind;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rdoDataTime;
        private System.Windows.Forms.RadioButton rdoTotalHours;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnRealTime;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd2;
        private System.Windows.Forms.DateTimePicker dateTimePickerEnd1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dateTimePickerBegin2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePickerBegin1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Label lblQueryWait;
    }
}