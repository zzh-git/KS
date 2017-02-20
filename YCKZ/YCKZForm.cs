using HAOCommon;
using HAOCommon.OPCKZ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinHost;
using YHYXYH.Tool;
using YHYXYH.YXYH;

namespace KSPrj.YCKZ
{
    public partial class YCKZForm : Form
    {
        Control control = null;
        TagLabelBinding tagLabelBinding = new TagLabelBinding(5);
        double panelScalex = 1;
        System.Timers.Timer timer;
        OPCReadKZ opc = null;
        Hashtable ControlWriteBackHs = new Hashtable();
        bool isStart = false;

        Label L_UP_1SJ, L_UP_1ZL, L_UP_2SJ, L_UP_2ZL, L_UP_3SJ, L_UP_3ZL, L_UP_4SJ, L_UP_4ZL, L_UP_5SJ, L_UP_5ZL, L_UP_6SJ, L_UP_6ZL, L_UP_7SJ, L_UP_7ZL, L_UP_8SJ, L_UP_8ZL,
              L_DOWN_1SJ, L_DOWN_1ZL, L_DOWN_2SJ, L_DOWN_2ZL, L_DOWN_3SJ, L_DOWN_3ZL, L_DOWN_4SJ, L_DOWN_4ZL, L_DOWN_5SJ, L_DOWN_5ZL, L_DOWN_6SJ, L_DOWN_6ZL, L_DOWN_7SJ, L_DOWN_7ZL, L_DOWN_8SJ, L_DOWN_8ZL, L_DOWN_9SJ, L_DOWN_9ZL;

        public YCKZForm()
        {
            InitializeComponent();
            AddConfigedControl();
            SetMoveAdd();
            FindControl();

            try { ChangeLabels(GlobalVariables.dtOneRowDataFive); }
            catch { }
        }

        private void YCKZForm_Load(object sender, EventArgs e)
        {
            NoticeFive.OnMessageRecieved += OnMessageReceived;
            upd = new UpdateControls(ChangeLabels);

            if (GlobalVariables.OPCPI == "OPC")
                opc = new OPCReadKZ(51);

            timer = new System.Timers.Timer(100);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Tick);
            timer.Enabled = false;
            isStart = true;
        }

        public void Disconnect()
        {
            opc.Disconnect();
        }

        #region 委托接收并更新页面数据
        //定义委托 BY ZZH 
        public delegate void UpdateControls(DataTable dt);
        UpdateControls upd = null;

        public void OnMessageReceived(DataTable calDataTable)
        {
            this.BeginInvoke(upd, calDataTable);
        }

        private void ChangeLabels(DataTable dt)
        {
            SetBindingLabelsText();
            SetMoveAdd();
            SetBindFixedLabel();
            BindRightDown();
            BindPicValue();
        }

        /// <summary>
        /// 上半部分-赋值给Label、图片 
        /// </summary>
        void SetBindingLabelsText()
        {
            try
            {
                Control control, parent;
                LabelTag labelTag = null;
                DataTable dtTagLabelBinding = TableTagLabelBinding.getDataTableByRowFilter("[Parent] like 'YCKZForm.%'");
                foreach (DataRow row in dtTagLabelBinding.Rows)
                {
                    try
                    {
                        parent = TagLabelBinding.GetParentControl(this, row["Parent"].ToString());
                        //由父控件找到本Label
                        control = parent.Controls.Find(row["LabelName"].ToString(), false)[0];
                        labelTag = (LabelTag)control.Tag;

                        try
                        {

                            if (labelTag.IsSetToolTipText == false)
                            {
                                toolTip1.SetToolTip(control, row["TagDesc"].ToString());
                                labelTag.IsSetToolTipText = true;
                                labelTag.TagID = int.Parse(row["TagID"].ToString());
                                labelTag.TagDesc = row["TagDesc"].ToString();
                                labelTag.TagUnit = row["unit"].ToString();
                                control.Tag = labelTag;
                            }
                        }
                        catch
                        {
                            control.Tag = new LabelTag(true, int.Parse(row["TagID"].ToString()), row["TagDesc"].ToString(), row["unit"].ToString());
                            toolTip1.SetToolTip(control, row["TagDesc"].ToString());
                        }

                        try
                        {
                            if (control.GetType() == typeof(PictureBox))
                            {
                                double dv = TagValue.GetFinishedTagValueFive("F" + row["TagID"]);
                                if (row["LabelName"].ToString().Contains("Point"))
                                {
                                    if (dv > 0)
                                        control.BackgroundImage = global::KSPrj.Properties.Resources.red;
                                    else
                                        control.BackgroundImage = global::KSPrj.Properties.Resources.green;
                                }
                                else if (row["LabelName"].ToString().Contains("Horizontal"))
                                {
                                    if (dv > 0)
                                        control.BackgroundImage = global::KSPrj.Properties.Resources.horizontalRed;
                                    else
                                        control.BackgroundImage = global::KSPrj.Properties.Resources.horizontalGreen;
                                }
                                else if (row["LabelName"].ToString().Contains("Vertical"))
                                {
                                    if (dv > 0)
                                        control.BackgroundImage = global::KSPrj.Properties.Resources.verticalRed;
                                    else
                                        control.BackgroundImage = global::KSPrj.Properties.Resources.verticalGreen;
                                }
                            }
                            else
                            {

                                if ((control.Name.StartsWith("L_UP_") && control.Name.EndsWith("ZL")) || (control.Name.StartsWith("L_DOWN_") && control.Name.EndsWith("ZL")))
                                {
                                    if (GlobalVariables.dtOneRowDataFive.Rows[0]["F5001"].ToString() == "1" && GlobalVariables.dtOneRowDataFive.Rows[0]["F5002"].ToString() == "0")
                                        ;
                                    else
                                    {
                                        control.Text = TagValue.GetShowValueFive(int.Parse(row["TagID"].ToString()), labelTag.TagUnit, row["adjustValue"].ToString());
                                    }
                                }
                                else
                                    control.Text = TagValue.GetShowValueFive(int.Parse(row["TagID"].ToString()), labelTag.TagUnit, row["adjustValue"].ToString());

                                //Application.DoEvents(); 
                            }
                        }
                        catch (Exception ee) { WriteLog.WriteLogs(ee.ToString()); }
                        control.Refresh();


                    }
                    catch (Exception ex)
                    {
                        WriteLog.WriteLogs(ex.ToString());
                    }
                }
                //lblRunOptimizePromptText.Text = RunOptimizePrompt.GetPromptText();
                //lblRunOptimizePromptText.ForeColor = RunOptimizePrompt.GetPromptTextForeColor();
                dtTagLabelBinding.Dispose();
                dtTagLabelBinding = null;
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
        }

        /// <summary>
        /// 绑定页面上固定的Label值 
        /// </summary>
        void SetBindFixedLabel()
        {
            try
            {
                double d1 = TagValue.GetFinishedTagValueFive("F5003");
                double d2 = TagValue.GetFinishedTagValueFive("F5004");
                double d3 = TagValue.GetFinishedTagValueFive("F5005");
                double d4 = TagValue.GetFinishedTagValueFive("F5006");
                double d5 = TagValue.GetFinishedTagValueFive("F5007");
                double d6 = TagValue.GetFinishedTagValueFive("F5008");

                if (d1 > 0)
                    this.topshanquLabel.Text = "#5-1扇区";
                else if (d3 > 0)
                    this.topshanquLabel.Text = "#5-3扇区";
                else if (d5 > 0)
                    this.topshanquLabel.Text = "#5-5扇区";
                else
                    this.topshanquLabel.Text = "";

                if (d2 > 0)
                    this.botshanquLabel.Text = "#5-2扇区";
                else if (d4 > 0)
                    this.botshanquLabel.Text = "#5-4扇区";
                else if (d6 > 0)
                    this.botshanquLabel.Text = "#5-6扇区";
                else
                    this.botshanquLabel.Text = "";
            }
            catch { }
        }

        /// <summary>
        /// 左下角棒状图部分-赋值 
        /// </summary>
        void BindPicValue()
        {
            //label20 间冷塔实际释放热量 
            //label24 凝汽器背压（绝对） 
            //label27 热网循环水量 
            //label30 冷却水总流量 
            //label3 末级叶片颤振值 
            //label8 末级叶片颤振值的设定值
            double label20Value = 0;
            double label24Value = 0;
            double label27Value = 0;
            double label30Value = 0;
            double label3Value = 0;
            double label8Value = 0;

            //label20 间冷塔实际释放热量 
            int length1 = 0;
            try
            {
                label20Value = Math.Round(TagValue.GetFinishedTagValueFive("F5252"), 2);
                length1 = Convert.ToInt32(800d / 500d * label20Value);
            }
            catch (Exception ex) { label20Value = 0; WriteLog.WriteLogs(ex.ToString()); }
            if (length1 > 50)
            {
                if (length1 > 800)
                    length1 = 800;
                this.label20.Width = length1;
            }
            else
                this.label20.Width = 50;
            this.label20.Text = label20Value + "";

            //label24 凝汽器背压（绝对） 
            int length2 = 0;
            try
            {
                label24Value = TagValue.GetFinishedTagValueFive("F3126") * 1000;
                length2 = Convert.ToInt32(800d / 50d * label24Value);
            }
            catch (Exception ex) { label24Value = 0; WriteLog.WriteLogs(ex.ToString()); }
            if (length2 > 50)
            {
                if (length2 > 800)
                    length2 = 800;
                this.label24.Width = length2;
            }
            else
                this.label24.Width = 50;
            this.label24.Text = label24Value + "";

            //label27 热网循环水量 
            int length3 = 0;
            try
            {
                label27Value = Math.Round(TagValue.GetFinishedTagValueFive("F3505"), 2);
                length3 = Convert.ToInt32(800d / 16000d * label27Value);
            }
            catch (Exception ex) { label27Value = 0; WriteLog.WriteLogs(ex.ToString()); }
            if (length3 > 50)
            {
                if (length3 > 800)
                    length3 = 800;
                this.label27.Width = length3;
            }
            else
                this.label27.Width = 50;
            this.label27.Text = label27Value + "";

            //label30 冷却水总流量 
            int length4 = 0;
            try
            {
                label30Value = Math.Round(TagValue.GetFinishedTagValueFive("F5279"), 2);
                length4 = Convert.ToInt32(800d / 16000d * label30Value);
            }
            catch (Exception ex) { label30Value = 0; WriteLog.WriteLogs(ex.ToString()); }
            if (length4 > 50)
            {
                if (length4 > 800)
                    length4 = 800;
                this.label30.Width = length4;
            }
            else
                this.label30.Width = 50;
            this.label30.Text = label30Value + "";

            //label3 末级叶片颤振值 
            int length5 = 0;
            try
            {
                label3Value = Math.Round(TagValue.GetFinishedTagValueFive("F3002"), 2);
                if (label3Value < 0)
                    label3Value = 0;
                else if (label3Value > 150)
                    label3Value = 150;
                length5 = Convert.ToInt32(800d / 150d * label3Value);
            }
            catch (Exception ex) { label3Value = 0; WriteLog.WriteLogs(ex.ToString()); }
            if (length5 > 50)
            {
                if (length5 > 800)
                    length5 = 800;
                this.label3.Width = length5;
            }
            else
                this.label3.Width = 50;
            this.label3.Text = label3Value + "";

            //label8 末级叶片颤振值的设定值
            int length6 = 0;
            try
            {
                //label8Value = GlobalVariables.dMJYPCZZ_Set;
                label8Value = Math.Round(TagValue.GetFinishedTagValueFive("F5280"), 2);
                length6 = Convert.ToInt32(800d / 150d * label8Value);
            }
            catch (Exception ex) { label8Value = 0; WriteLog.WriteLogs(ex.ToString()); }
            if (length6 > 50)
            {
                if (length6 > 800)
                    length6 = 800;
                this.label8.Width = length6;
            }
            else
                this.label8.Width = 50;
            this.label8.Text = label8Value + "";

        }

        /// <summary>
        /// 右下角变频泵绑定 
        /// </summary>
        void BindRightDown()
        {
            try
            {
                //实际频率 
                double d1 = TagValue.GetFinishedTagValueFive("F5169");
                if (d1 >= 0)
                    this.labelSJPL.Text = d1 + "";
                else
                    this.labelSJPL.Text = "--";
                
                //指令频率  
                if (GlobalVariables.dtOneRowDataFive.Rows[0]["F5001"].ToString() == "1" && GlobalVariables.dtOneRowDataFive.Rows[0]["F5002"].ToString() == "0")
                    ;
                else
                {
                    double d2 = TagValue.GetFinishedTagValueFive("F5010");
                    if (d2 >= 0)
                        this.labelZLPL.Text = d2 + "";
                    else
                        this.labelZLPL.Text = "--";
                }
                //double d2 = TagValue.GetFinishedTagValueFive("F5010");
                //if (d2 >= 0)
                //    this.labelZLPL.Text = d2 + "";
                //else
                //    this.labelZLPL.Text = "--";

                //高背压/抽凝 运行状态 
                double dv1 = TagValue.GetFinishedTagValueFive("F5001");
                if (dv1 > 0)
                {
                    this.picBoxGBY.BackgroundImage = global::KSPrj.Properties.Resources.red;
                    this.picBoxCN.BackgroundImage = global::KSPrj.Properties.Resources.green;
                }
                else
                {
                    this.picBoxGBY.BackgroundImage = global::KSPrj.Properties.Resources.green;
                    this.picBoxCN.BackgroundImage = global::KSPrj.Properties.Resources.red;
                }

                //高背压 自动/手动 
                double dv3 = TagValue.GetFinishedTagValueFive("F5002");
                if (dv3 > 0)
                {
                    this.picBoxGBYZD.BackgroundImage = global::KSPrj.Properties.Resources.red;
                    this.picBoxGBYSD.BackgroundImage = global::KSPrj.Properties.Resources.green;
                    ChangeBtnState(false);
                }
                else
                {
                    this.picBoxGBYZD.BackgroundImage = global::KSPrj.Properties.Resources.green;
                    this.picBoxGBYSD.BackgroundImage = global::KSPrj.Properties.Resources.red;
                    ChangeBtnState(true);
                }
                if (dv1 > 0 && dv3 < 0.5)
                    ChangeBtnState(true);
                else
                    ChangeBtnState(false);
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
        }

        //1表示按钮可用，2表示按钮不可用 
        int btnEnable = 0;
        private void ChangeBtnState(bool enable)
        {
            if (enable)
            {
                if (btnEnable != 1)
                {
                    SetBtnEnable(enable);
                    btnEnable = 1;
                }
                else
                    btnEnable = 1;
            }
            else
            {
                if (btnEnable != 2)
                {
                    SetBtnEnable(enable);
                    btnEnable = 2;
                }
                else
                    btnEnable = 2;
            }
        }
        void SetBtnEnable(bool b)
        {
            this.button8.Enabled = b;
            this.button9.Enabled = b;
            this.button10.Enabled = b;
            this.button11.Enabled = b;
            this.button12.Enabled = b;
            this.button13.Enabled = b;
            this.button14.Enabled = b;
            this.button15.Enabled = b;
            this.button16.Enabled = b;
            this.button17.Enabled = b;
            this.button18.Enabled = b;
            this.button19.Enabled = b;
            this.button20.Enabled = b;
            this.button21.Enabled = b;
            this.button22.Enabled = b;
            this.button23.Enabled = b;
            this.button24.Enabled = b;
            this.button25.Enabled = b;
            this.button26.Enabled = b;
            this.button27.Enabled = b;
            this.button28.Enabled = b;
            this.button29.Enabled = b;
            this.button30.Enabled = b;
            this.button31.Enabled = b;
            this.button32.Enabled = b;
            this.button33.Enabled = b;
            this.button34.Enabled = b;
            this.button35.Enabled = b;
            this.button36.Enabled = b;
            this.button37.Enabled = b;
            this.button38.Enabled = b;
            this.button39.Enabled = b;
            this.button40.Enabled = b;
            this.button41.Enabled = b;
            this.button42.Enabled = b;
            this.button43.Enabled = b;
            this.button44.Enabled = b;
            this.button45.Enabled = b;
        }

        #endregion

        #region 设置 控件是否可添加，是否可移动

        //1表示按钮可用，2表示按钮不可用 
        int btnVisible = 0;
        private void SetMoveAdd()
        {
            //设置是否可添加 
            if (GlobalVariables.IsCanAddLabel)
            {
                this.button1.Visible = true;
                this.button2.Visible = true;
                this.button3.Visible = true;
                this.button7.Visible = true;
            }
            else
            {
                this.button1.Visible = false;
                this.button2.Visible = false;
                this.button3.Visible = false;
                this.button7.Visible = false;
            }
            //设置是否可移动 
            if (GlobalVariables.IsCanMoveLabel)
            {
                if (btnVisible != 1)
                {
                    //设置控件是否可移动 
                    SetControlMouseEvent();
                    btnVisible = 1;
                }
                else
                    btnVisible = 1;
            }
            else
            {
                if (btnVisible != 2)
                {
                    //设置控件是否可移动 
                    SetControlMouseEvent();
                    btnVisible = 2;
                }
                else
                    btnVisible = 2;
            }
        }

        /// <summary>
        /// 设置控件是否可移动 
        /// </summary>
        private void SetControlMouseEvent()
        {
            for (int i = 0; i < this.pnlCenterTop.Controls.Count; i++)
            {
                control = this.pnlCenterTop.Controls[i];
                if (control.Tag != null)
                {
                    if (GlobalVariables.IsCanMoveLabel)
                    {
                        control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
                        control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
                        control.ContextMenuStrip = cmenu;//添加快捷菜单
                    }
                    else
                    {
                        control.MouseMove -= control_MouseMove;
                        control.MouseUp -= control_MouseUp;
                        control.ContextMenuStrip = null;
                    }
                }
            }
        }

        #endregion

        private void btnQuit_Click(object sender, EventArgs e)
        {
            Program.mainForm.Show();
            this.Visible = false;
        }

        private void pnlCenter_Resize(object sender, EventArgs e)
        {
            //pnlCenterTop.Height = (pnlCenter.Height * 60 / 100);
        }

        private void pnlCenterDownLeft_Resize(object sender, EventArgs e)
        {

        }

        private void pnlCenterTop_MouseDown(object sender, MouseEventArgs e)
        {
            cmenuSaveLocationSize.Visible = true;
            //cmenu2.Visible = false;
            control = (Control)sender;
        }

        private void pnlCenterDownLeft_MouseDown(object sender, MouseEventArgs e)
        {
            cmenuSaveLocationSize.Visible = true;
            //cmenu2.Visible = false;
            control = (Control)sender;
        }


        #region Resize事件
        //控件的Resize事件 
        private void Control_Resize(object sender, EventArgs e)
        {
            Rectangle rect;
            LabelTag labelTag = null;
            //this.Visible = false;
            Control parentControl = sender as Control;
            parentControl.Visible = false;
            for (int i = 0; i < parentControl.Controls.Count; i++)
            {
                Control control = parentControl.Controls[i];
                if (control.Tag == null)
                    continue;
                //}
                //foreach (Control control in parentControl.Controls)
                //{
                try
                {
                    labelTag = (LabelTag)(control.Tag);
                    rect = new Rectangle();
                    rect.X = Convert.ToInt32(1.0 * labelTag.ControlX / labelTag.ParentWidth * parentControl.Width);
                    rect.Y = Convert.ToInt32(1.0 * labelTag.ControlY / labelTag.ParentHeight * parentControl.Height);
                    if (control.GetType() == typeof(PictureBox))
                    {
                        //if (parentControl.Name == "panel10")
                        //{
                        //图片统一不缩放，可以在加载的时候设定好大小 ZZH 
                        //panelScalex = Convert.ToDouble(1.0 * parentControl.Width / labelTag.ParentWidth);
                        rect.Width = labelTag.Width; // Convert.ToInt32(1.0 * labelTag.Width / labelTag.ParentWidth * parentControl.Width);
                        rect.Height = labelTag.Height; // Convert.ToInt32(1.0 * labelTag.Height / labelTag.ParentHeight * parentControl.Height);
                        //}
                        //else if (parentControl.Name == "groupBox2")
                        //{
                        //    // 注意这里，如果需要缩放图片的大小，可以将空冷岛图片保存到controlconfig_pic里面，然后把这段删掉 ZZH
                        //    rect.Width = 12;
                        //    rect.Height = 18;
                        //}
                    }
                    control.Bounds = rect;
                }
                catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
            }
            parentControl.Visible = true;
        }

        #endregion


        DataTable dtControls = TableControlConfig.getDataTableByRowFilter("[Parent] like 'YCKZForm.%'");
        /// <summary>
        /// 初始化界面，添加控件 
        /// </summary>
        void AddConfigedControl()
        {
            try
            {
                //int i;
                Control parent = null;
                Control control = null;
                Rectangle rect;
                LabelTag labelTag = null;
                foreach (DataRow row in dtControls.Rows)
                {
                    try
                    {
                        if (row["ControlName"].ToString().Contains("L_DOWN_") || row["ControlName"].ToString().Contains("L_UP_"))
                        {
                            if (row["ControlText"].ToString().Length > 5)
                                if (!ControlWriteBackHs.ContainsKey(row["ControlName"].ToString()))
                                    ControlWriteBackHs.Add(row["ControlName"].ToString(), row["ControlText"].ToString());
                        }
                    }
                    catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }

                    parent = TagLabelBinding.GetParentControl(this, row["Parent"].ToString());
                    try
                    {
                        if (row["type"].ToString() == "PictureBox")
                        {
                            int controlX = int.Parse(row["ControlX"].ToString());
                            int controlY = int.Parse(row["ControlY"].ToString());
                            control = new PictureBox();

                            ((PictureBox)control).BackgroundImageLayout = ImageLayout.Stretch;
                            control.Name = row["ControlName"].ToString();
                            rect = new Rectangle();
                            rect.X = controlX;
                            rect.Y = controlY;
                            labelTag = new LabelTag();
                            labelTag.ParentHeight = int.Parse(row["ParentHeight"].ToString());
                            labelTag.ParentWidth = int.Parse(row["ParentWidth"].ToString());
                            labelTag.ControlX = controlX;
                            labelTag.ControlY = controlY;
                            labelTag.UnitNo = byte.Parse(row["UnitNo"].ToString());

                            if ("圆点".Equals(row["ControlText"].ToString()))
                            {
                                ((PictureBox)control).BackgroundImage = global::KSPrj.Properties.Resources.green;
                                control.Size = new System.Drawing.Size(30, 30);
                            }
                            else if ("横向".Equals(row["ControlText"].ToString()))
                            {
                                ((PictureBox)control).BackgroundImage = global::KSPrj.Properties.Resources.horizontalGreen;
                                control.Size = new System.Drawing.Size(36, 24);
                            }
                            else if ("纵向".Equals(row["ControlText"].ToString()))
                            {
                                ((PictureBox)control).BackgroundImage = global::KSPrj.Properties.Resources.verticalGreen;
                                control.Size = new System.Drawing.Size(24, 36);
                            }

                            labelTag.Width = control.Width;
                            labelTag.Height = control.Height;
                            control.Tag = labelTag;
                            control.Bounds = rect;

                            control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
                            //if (GlobalVariables.IsCanMoveLabel)
                            //{
                            //    control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
                            //    control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
                            //    control.ContextMenuStrip = cmenu;//添加快捷菜单
                            //}
                            parent.Controls.Add(control);
                            control.Refresh();
                            continue;
                        }

                        //这里先以Label写死；如果以后还有其他类型的控件，则在这里枚举
                        control = new Label();
                        ((Label)control).AutoSize = true;
                        control.Name = row["ControlName"].ToString();
                        control.Text = control.Name;
                        if (row["ControlText"].ToString().Length > 1)
                            control.Text = row["ControlText"].ToString();
                        if (row["FontSize"].ToString().Length > 1)
                            control.Font = new System.Drawing.Font("宋体", float.Parse(row["FontSize"].ToString()), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                        rect = new Rectangle();
                        rect.X = int.Parse(row["ControlX"].ToString());
                        rect.Y = int.Parse(row["ControlY"].ToString());
                        control.Bounds = rect;
                        try { control.ForeColor = Color.FromArgb(int.Parse(row["ForeColor"].ToString())); }
                        catch { }
                        try { control.BackColor = Color.FromArgb(int.Parse(row["BackColor"].ToString())); }
                        catch { }
                        control.BackColor = Color.Transparent;
                        labelTag = new LabelTag();
                        labelTag.ControlX = rect.X;
                        labelTag.ControlY = rect.Y;
                        labelTag.ParentHeight = int.Parse(row["ParentHeight"].ToString());
                        labelTag.ParentWidth = int.Parse(row["ParentWidth"].ToString());
                        labelTag.UnitNo = byte.Parse(row["UnitNo"].ToString());
                        control.Tag = labelTag;
                        //control.MouseDoubleClick += new MouseEventHandler(Label_DoubleClick);//双击事件  改动 BY ZZH 
                        control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
                        //if (GlobalVariables.IsCanMoveLabel)
                        //{
                        //    control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
                        //    control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
                        //    control.ContextMenuStrip = cmenu;//添加快捷菜单
                        //}
                        parent.Controls.Add(control);
                    }
                    catch { }
                }
                dtControls.Dispose();
                dtControls = null;
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }

        }

        /// <summary>
        /// 找到指定的控件 
        /// </summary>
        void FindControl()
        {
            try
            {

                L_UP_1SJ = this.pnlCenterTop.Controls.Find("L_UP_1SJ", false)[0] as Label;
                L_UP_1ZL = this.pnlCenterTop.Controls.Find("L_UP_1ZL", false)[0] as Label;
                L_UP_2SJ = this.pnlCenterTop.Controls.Find("L_UP_2SJ", false)[0] as Label;
                L_UP_2ZL = this.pnlCenterTop.Controls.Find("L_UP_2ZL", false)[0] as Label;
                L_UP_3SJ = this.pnlCenterTop.Controls.Find("L_UP_3SJ", false)[0] as Label;
                L_UP_3ZL = this.pnlCenterTop.Controls.Find("L_UP_3ZL", false)[0] as Label;
                L_UP_4SJ = this.pnlCenterTop.Controls.Find("L_UP_4SJ", false)[0] as Label;
                L_UP_4ZL = this.pnlCenterTop.Controls.Find("L_UP_4ZL", false)[0] as Label;
                L_UP_5SJ = this.pnlCenterTop.Controls.Find("L_UP_5SJ", false)[0] as Label;
                L_UP_5ZL = this.pnlCenterTop.Controls.Find("L_UP_5ZL", false)[0] as Label;
                L_UP_6SJ = this.pnlCenterTop.Controls.Find("L_UP_6SJ", false)[0] as Label;
                L_UP_6ZL = this.pnlCenterTop.Controls.Find("L_UP_6ZL", false)[0] as Label;
                L_UP_7SJ = this.pnlCenterTop.Controls.Find("L_UP_7SJ", false)[0] as Label;
                L_UP_7ZL = this.pnlCenterTop.Controls.Find("L_UP_7ZL", false)[0] as Label;
                L_UP_8SJ = this.pnlCenterTop.Controls.Find("L_UP_8SJ", false)[0] as Label;
                L_UP_8ZL = this.pnlCenterTop.Controls.Find("L_UP_8ZL", false)[0] as Label;
                L_DOWN_1SJ = this.pnlCenterTop.Controls.Find("L_DOWN_1SJ", false)[0] as Label;
                L_DOWN_1ZL = this.pnlCenterTop.Controls.Find("L_DOWN_1ZL", false)[0] as Label;
                L_DOWN_2SJ = this.pnlCenterTop.Controls.Find("L_DOWN_2SJ", false)[0] as Label;
                L_DOWN_2ZL = this.pnlCenterTop.Controls.Find("L_DOWN_2ZL", false)[0] as Label;
                L_DOWN_3SJ = this.pnlCenterTop.Controls.Find("L_DOWN_3SJ", false)[0] as Label;
                L_DOWN_3ZL = this.pnlCenterTop.Controls.Find("L_DOWN_3ZL", false)[0] as Label;
                L_DOWN_4SJ = this.pnlCenterTop.Controls.Find("L_DOWN_4SJ", false)[0] as Label;
                L_DOWN_4ZL = this.pnlCenterTop.Controls.Find("L_DOWN_4ZL", false)[0] as Label;
                L_DOWN_5SJ = this.pnlCenterTop.Controls.Find("L_DOWN_5SJ", false)[0] as Label;
                L_DOWN_5ZL = this.pnlCenterTop.Controls.Find("L_DOWN_5ZL", false)[0] as Label;
                L_DOWN_6SJ = this.pnlCenterTop.Controls.Find("L_DOWN_6SJ", false)[0] as Label;
                L_DOWN_6ZL = this.pnlCenterTop.Controls.Find("L_DOWN_6ZL", false)[0] as Label;
                L_DOWN_7SJ = this.pnlCenterTop.Controls.Find("L_DOWN_7SJ", false)[0] as Label;
                L_DOWN_7ZL = this.pnlCenterTop.Controls.Find("L_DOWN_7ZL", false)[0] as Label;
                L_DOWN_8SJ = this.pnlCenterTop.Controls.Find("L_DOWN_8SJ", false)[0] as Label;
                L_DOWN_8ZL = this.pnlCenterTop.Controls.Find("L_DOWN_8ZL", false)[0] as Label;
                L_DOWN_9SJ = this.pnlCenterTop.Controls.Find("L_DOWN_9SJ", false)[0] as Label;
                L_DOWN_9ZL = this.pnlCenterTop.Controls.Find("L_DOWN_9ZL", false)[0] as Label;


                L_UP_1SJ.Text = "20";
                L_UP_1ZL.Text = "20";
                L_UP_2SJ.Text = "20";
                L_UP_2ZL.Text = "20";
                L_UP_3SJ.Text = "20";
                L_UP_3ZL.Text = "20";
                L_UP_4SJ.Text = "20";
                L_UP_4ZL.Text = "20";
                L_UP_5SJ.Text = "20";
                L_UP_5ZL.Text = "20";
                L_UP_6SJ.Text = "20";
                L_UP_6ZL.Text = "20";
                L_UP_7SJ.Text = "20";
                L_UP_7ZL.Text = "20";
                L_UP_8SJ.Text = "20";
                L_UP_8ZL.Text = "20";
                L_DOWN_1SJ.Text = "20";
                L_DOWN_1ZL.Text = "20";
                L_DOWN_2SJ.Text = "20";
                L_DOWN_2ZL.Text = "20";
                L_DOWN_3SJ.Text = "20";
                L_DOWN_3ZL.Text = "20";
                L_DOWN_4SJ.Text = "20";
                L_DOWN_4ZL.Text = "20";
                L_DOWN_5SJ.Text = "20";
                L_DOWN_5ZL.Text = "20";
                L_DOWN_6SJ.Text = "20";
                L_DOWN_6ZL.Text = "20";
                L_DOWN_7SJ.Text = "20";
                L_DOWN_7ZL.Text = "20";
                L_DOWN_8SJ.Text = "20";
                L_DOWN_8ZL.Text = "20";
                L_DOWN_9SJ.Text = "20";
                L_DOWN_9ZL.Text = "20";


            }
            catch (Exception e) { }
        }

        #region pnlCenterTop 功能按钮
        //添加圆点到 pnlCenterTop 
        private void button1_Click(object sender, EventArgs e)
        {
            //int iUnitNo = 2; // int.Parse(((Button)sender).Tag.ToString());
            //control = new Label();
            control = new PictureBox();
            control.Name = "Point" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            control.Width = 20;
            control.Height = 20;
            control.BackgroundImage = global::KSPrj.Properties.Resources.green;
            control.BackgroundImageLayout = ImageLayout.Stretch;
            control.Top = 100;
            control.Left = 100;
            control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
            control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
            control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
            control.ContextMenuStrip = cmenu;
            this.pnlCenterTop.Controls.Add(control);
            LabelTag labelTag = new LabelTag();
            labelTag.ControlX = control.Left;
            labelTag.ControlY = control.Top;
            labelTag.ParentHeight = control.Parent.Height;
            labelTag.ParentWidth = control.Parent.Width;
            labelTag.UnitNo = 5;
            control.Tag = labelTag;
            sSql = "insert into ControlConfig(ControlName,ControlX,ControlY,ForeColor,BackColor,Type,Parent,ParentHeight,ParentWidth,ControlText,UnitNo) values('"
                + control.Name + "'," + control.Location.X + "," + control.Location.Y + "," + control.ForeColor.ToArgb() + ","
                + control.BackColor.ToArgb() + ",'" + control.GetType().Name + "','" + TagLabelBinding.GetParents(control) + "',"
                + control.Parent.Height + "," + control.Parent.Width + ",'圆点'," + 5 + ")";
            SQLHelper.ExecuteSql(sSql);
            TableControlConfig.setFill();
        }

        //添加横向开关到 pnlCenterTop 
        private void button2_Click(object sender, EventArgs e)
        {
            control = new PictureBox();
            control.Name = "Horizontal" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            control.Width = 18;
            control.Height = 12;
            control.BackgroundImage = global::KSPrj.Properties.Resources.horizontalGreen;
            control.BackgroundImageLayout = ImageLayout.Stretch;
            control.Top = 120;
            control.Left = 120;
            control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
            control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
            control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
            control.ContextMenuStrip = cmenu;
            this.pnlCenterTop.Controls.Add(control);
            LabelTag labelTag = new LabelTag();
            labelTag.ControlX = control.Left;
            labelTag.ControlY = control.Top;
            labelTag.ParentHeight = control.Parent.Height;
            labelTag.ParentWidth = control.Parent.Width;
            labelTag.UnitNo = 5;
            control.Tag = labelTag;
            sSql = "insert into ControlConfig(ControlName,ControlX,ControlY,ForeColor,BackColor,Type,Parent,ParentHeight,ParentWidth,ControlText,UnitNo) values('"
                + control.Name + "'," + control.Location.X + "," + control.Location.Y + "," + control.ForeColor.ToArgb() + ","
                + control.BackColor.ToArgb() + ",'" + control.GetType().Name + "','" + TagLabelBinding.GetParents(control) + "',"
                + control.Parent.Height + "," + control.Parent.Width + ",'横向'," + 5 + ")";
            SQLHelper.ExecuteSql(sSql);
            TableControlConfig.setFill();
        }

        //添加纵向开关到 pnlCenterTop 
        private void button3_Click(object sender, EventArgs e)
        {
            control = new PictureBox();
            control.Name = "Vertical" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
            control.Width = 12;
            control.Height = 18;
            control.BackgroundImage = global::KSPrj.Properties.Resources.verticalGreen;
            control.BackgroundImageLayout = ImageLayout.Stretch;
            control.Top = 130;
            control.Left = 130;
            control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
            control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
            control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
            control.ContextMenuStrip = cmenu;
            this.pnlCenterTop.Controls.Add(control);
            LabelTag labelTag = new LabelTag();
            labelTag.ControlX = control.Left;
            labelTag.ControlY = control.Top;
            labelTag.ParentHeight = control.Parent.Height;
            labelTag.ParentWidth = control.Parent.Width;
            labelTag.UnitNo = 5;
            control.Tag = labelTag;
            sSql = "insert into ControlConfig(ControlName,ControlX,ControlY,ForeColor,BackColor,Type,Parent,ParentHeight,ParentWidth,ControlText,UnitNo) values('"
                + control.Name + "'," + control.Location.X + "," + control.Location.Y + "," + control.ForeColor.ToArgb() + ","
                + control.BackColor.ToArgb() + ",'" + control.GetType().Name + "','" + TagLabelBinding.GetParents(control) + "',"
                + control.Parent.Height + "," + control.Parent.Width + ",'纵向'," + 5 + ")";
            SQLHelper.ExecuteSql(sSql);
            TableControlConfig.setFill();
        }

        //添加测点 
        private void button7_Click(object sender, EventArgs e)
        {
            int iUnitNo = 5;
            control = new Label();
            control.Name = "L" + DateTime.Now.ToString("yyyyMMddHHmmssfff"); //设置label的唯一标示，用创建时间表示
            //sNewLabel = control.Name;
            control.ForeColor = System.Drawing.Color.FromName("Lime");
            control.Text = DateTime.Now.ToString("New Label");
            control.AutoSize = true;
            control.Top = 100;
            control.Left = 100;
            control.BackColor = System.Drawing.Color.Black;//设置背景色透明
            //control.MouseDoubleClick += new MouseEventHandler(Label_DoubleClick); //双击事件，选择测点。
            control.MouseDown += new MouseEventHandler(control_MouseDown);//鼠标按下事件
            control.MouseMove += new MouseEventHandler(control_MouseMove);//移动时的鼠标移动事件
            control.MouseUp += new MouseEventHandler(control_MouseUp);//移动时的鼠标抬起事件
            control.ContextMenuStrip = cmenu;
            this.pnlCenterTop.Controls.Add(control);
            LabelTag labelTag = new LabelTag();
            labelTag.ControlX = control.Left;
            labelTag.ControlY = control.Top;
            labelTag.ParentHeight = control.Parent.Height;
            labelTag.ParentWidth = control.Parent.Width;
            labelTag.UnitNo = (byte)iUnitNo;
            control.Tag = labelTag;
            sSql = "insert into ControlConfig(ControlName,ControlX,ControlY,ForeColor,BackColor,Type,Parent,ParentHeight,ParentWidth,UnitNo) values('"
                + control.Name + "'," + control.Location.X + "," + control.Location.Y + "," + control.ForeColor.ToArgb() + ","
                + control.BackColor.ToArgb() + ",'" + control.GetType().Name + "','" + TagLabelBinding.GetParents(control) + "',"
                + control.Parent.Height + "," + control.Parent.Width + "," + iUnitNo + ")";
            SQLHelper.ExecuteSql(sSql);
            TableControlConfig.setFill();
        }

        #endregion

        #region 鼠标按下 拖动 抬起 事件

        bool isDrag = false;
        string sSql = "";
        Point point = new Point();
        //保存鼠标按下时的坐标。这个坐标是相对于所点击的标签的坐标；在鼠标拖动时要减去这个坐标，要不然拖动时鼠标会一直处于标签的左上角
        Point pointMouseDown = new Point();

        void control_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if ((sender as Control).GetType() == typeof(PictureBox))
                {
                    control = (PictureBox)sender;
                }
                else if ((sender as Control).GetType() == typeof(Label))
                {
                    control = (Label)sender;
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                pointMouseDown.X = e.X;
                pointMouseDown.Y = e.Y;
            }

        }
        void control_MouseMove(object sender, MouseEventArgs e)
        {
            if ((sender as Control).GetType() == typeof(PictureBox))
            {
                control = (PictureBox)sender;
            }
            else if ((sender as Control).GetType() == typeof(Label))
            {
                control = (Label)sender;
            }
            if (e.Button == MouseButtons.Left)
            {
                isDrag = true;
                point.X = control.Location.X + e.X - pointMouseDown.X;
                point.Y = control.Location.Y + e.Y - pointMouseDown.Y;
                control.Location = point;
                control.Refresh();
            }
        }
        void control_MouseUp(object sender, MouseEventArgs e)
        {
            if ((sender as Control).GetType() == typeof(PictureBox))
            {
                control = (PictureBox)sender;
            }
            else if ((sender as Control).GetType() == typeof(Label))
            {
                control = (Label)sender;
            }
            string tabelName = " ControlConfig ";
            //图片或者Label都保存到 ControlConfig 表里面
            //if (control.Parent.Name == "panel10")
            //    tabelName = " ControlConfig_PIC ";
            if (e.Button == MouseButtons.Left)
            {
                if (isDrag)
                {
                    isDrag = false;
                    sSql = "update " + tabelName + " set ControlX=" + control.Location.X + ",ControlY=" + control.Location.Y
                        + ",ParentHeight=" + control.Parent.Height + ",ParentWidth=" + control.Parent.Width
                        + " where ControlName='" + control.Name + "'";
                    SQLHelper.ExecuteSql(sSql);
                    control.Location = point;
                    control.Refresh();
                    LabelTag labelTag = null;
                    try
                    {
                        labelTag = (LabelTag)control.Tag;
                        if (labelTag == null)
                            labelTag = new LabelTag();
                    }
                    catch
                    { labelTag = new LabelTag(); }

                    labelTag.ControlX = point.X;
                    labelTag.ControlY = point.Y;
                    labelTag.ParentHeight = control.Parent.Height;
                    labelTag.ParentWidth = control.Parent.Width;
                    control.Tag = labelTag;
                }
            }
        }

        #endregion

        #region 右键菜单

        private void cmenuSaveLocationSize_Click(object sender, EventArgs e)
        {
            if (control.Name == "pnlCenterDownLeft")
            {

            }
            else if (control.Name == "pnlCenterTop")
            {
            }
        }


        //删除 
        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除此测点吗？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    ArrayList listSql = new ArrayList();
                    listSql.Add("delete [ControlConfig] where ControlName='" + control.Name + "'");
                    listSql.Add("delete [TagLabelBinding] where LabelName='" + control.Name + "'");
                    SQLHelper.ExecuteSql(listSql);
                    tagLabelBinding.m_viewTagLabelBinding.RowFilter = "LabelName='" + control.Name + "'";
                    if (tagLabelBinding.m_viewTagLabelBinding.Count > 0)
                        tagLabelBinding.m_viewTagLabelBinding[0].Delete();
                    control.Visible = false;
                    control.Dispose();
                }
                catch (Exception ex)
                { WriteLog.WriteLogs(ex.ToString()); }
            }
        }

        //绑定测点 
        private void 绑定测点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tagLabelBinding.label = this.control;
            tagLabelBinding.ShowDialog();
        }

        #endregion

        #region 顶部菜单栏按钮

        //停止按钮 
        private void button4_Click(object sender, EventArgs e)
        {
            if (PWDCheck("rg"))
            {
                if (this.button4.Text.Replace(" ", "").Trim() == "运行")
                {
                    NoticeFive.OnMessageRecieved += OnMessageReceived;
                    this.button4.Text = "停  止";
                }
                else
                {
                    NoticeFive.OnMessageRecieved -= OnMessageReceived;
                    this.button4.Text = "运  行";
                }
            }
        }

        //切换到运行优化 
        private void button5_Click(object sender, EventArgs e)
        {
            Program.frm1.Show();
            this.Hide();
        }
        //切换到防颤振 
        private void button6_Click(object sender, EventArgs e)
        {
            Program.yxyh.Show();
            this.Hide();
        }

        //系统设置 
        private void button46_Click(object sender, EventArgs e)
        {
            (new ValuesConfig()).Show();
        }

        public bool PWDCheck(string name)
        {
            string pwd = SQLHelper.ExecuteScalar("select pws from SysPara where name='" + name + "'");
            FrmInputPws fip = new FrmInputPws(name == "rg" ? "请输入(热工检修人员)密码" : "请输入(运行人员)密码");
            fip.ShowDialog();
            string strRet = fip.Text;

            string[] strRetA = strRet.Split(',');
            if (strRet.Length == 0)
                return false;
            if (strRetA[1] == "ok")
            {
                if (pwd == strRetA[0])
                {
                    fip.Dispose();
                    return true;
                }
                else
                {
                    MessageBox.Show("密码错误！");
                    fip.Dispose();
                    return false;
                }
            }
            return false;
        }

        #endregion

        #region 控制按钮 上

        string writeBack_UP1 = "",
            writeBack_UP2 = "",
            writeBack_UP3 = "",
            writeBack_UP4 = "",
            writeBack_UP5 = "",
            writeBack_UP6 = "",
            writeBack_UP7 = "",
            writeBack_UP8 = "";
        //上--第一列--减 
        private void button8_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP1))
                writeBack_UP1 = ControlWriteBackHs["L_UP_1ZL"].ToString();
            double dValue = MinusValue(L_UP_1ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_1ZL.Text = dValue + "";
                SendCommand(writeBack_UP1, dValue);
            }
        }
        //上--第一列--加 
        private void button9_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP1))
                writeBack_UP1 = ControlWriteBackHs["L_UP_1ZL"].ToString();
            double dValue = AddValue(L_UP_1ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_1ZL.Text = dValue + "";
                SendCommand(writeBack_UP1, dValue);
            }
        }

        //上--第二列--减 
        private void button11_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP2))
                writeBack_UP2 = ControlWriteBackHs["L_UP_2ZL"].ToString();
            double dValue = MinusValue(L_UP_2ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_2ZL.Text = dValue + "";
                SendCommand(writeBack_UP2, dValue);
            }
        }
        //上--第二列--加 
        private void button10_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP2))
                writeBack_UP2 = ControlWriteBackHs["L_UP_2ZL"].ToString();
            double dValue = AddValue(L_UP_2ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_2ZL.Text = dValue + "";
                SendCommand(writeBack_UP2, dValue);
            }
        }

        //上--第三列--减 
        private void button13_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP3))
                writeBack_UP3 = ControlWriteBackHs["L_UP_3ZL"].ToString();
            double dValue = MinusValue(L_UP_3ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_3ZL.Text = dValue + "";
                SendCommand(writeBack_UP3, dValue);
            }
        }
        //上--第三列--加 
        private void button12_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP3))
                writeBack_UP3 = ControlWriteBackHs["L_UP_3ZL"].ToString();
            double dValue = AddValue(L_UP_3ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_3ZL.Text = dValue + "";
                SendCommand(writeBack_UP3, dValue);
            }
        }

        //上--第四列--减 
        private void button15_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP4))
                writeBack_UP4 = ControlWriteBackHs["L_UP_4ZL"].ToString();
            double dValue = MinusValue(L_UP_4ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_4ZL.Text = dValue + "";
                SendCommand(writeBack_UP4, dValue);
            }
        }
        //上--第四列--加 
        private void button14_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP4))
                writeBack_UP4 = ControlWriteBackHs["L_UP_4ZL"].ToString();
            double dValue = AddValue(L_UP_4ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_4ZL.Text = dValue + "";
                SendCommand(writeBack_UP4, dValue);
            }
        }

        //上--第五列--减 
        private void button17_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP5))
                writeBack_UP5 = ControlWriteBackHs["L_UP_5ZL"].ToString();
            double dValue = MinusValue(L_UP_5ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_5ZL.Text = dValue + "";
                SendCommand(writeBack_UP5, dValue);
            }
        }
        //上--第五列--加 
        private void button16_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP5))
                writeBack_UP5 = ControlWriteBackHs["L_UP_5ZL"].ToString();
            double dValue = AddValue(L_UP_5ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_5ZL.Text = dValue + "";
                SendCommand(writeBack_UP5, dValue);
            }
        }

        //上--第六列--减 
        private void button19_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP6))
                writeBack_UP6 = ControlWriteBackHs["L_UP_6ZL"].ToString();
            double dValue = MinusValue(L_UP_6ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_6ZL.Text = dValue + "";
                SendCommand(writeBack_UP6, dValue);
            }
        }
        //上--第六列--加 
        private void button18_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP6))
                writeBack_UP6 = ControlWriteBackHs["L_UP_6ZL"].ToString();
            double dValue = AddValue(L_UP_6ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_6ZL.Text = dValue + "";
                SendCommand(writeBack_UP6, dValue);
            }
        }

        //上--第七列--减 
        private void button21_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP7))
                writeBack_UP7 = ControlWriteBackHs["L_UP_7ZL"].ToString();
            double dValue = MinusValue(L_UP_7ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_7ZL.Text = dValue + "";
                SendCommand(writeBack_UP7, dValue);
            }
        }
        //上--第七列--加 
        private void button20_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP7))
                writeBack_UP7 = ControlWriteBackHs["L_UP_7ZL"].ToString();
            double dValue = AddValue(L_UP_7ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_7ZL.Text = dValue + "";
                SendCommand(writeBack_UP7, dValue);
            }
        }

        //上--第八列--减 
        private void button23_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP8))
                writeBack_UP8 = ControlWriteBackHs["L_UP_8ZL"].ToString();
            double dValue = MinusValue(L_UP_8ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_8ZL.Text = dValue + "";
                SendCommand(writeBack_UP8, dValue);
            }
        }
        //上--第八列--加 
        private void button22_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_UP8))
                writeBack_UP8 = ControlWriteBackHs["L_UP_8ZL"].ToString();
            double dValue = AddValue(L_UP_8ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_UP_8ZL.Text = dValue + "";
                SendCommand(writeBack_UP8, dValue);
            }
        }

        #endregion

        #region 控制按钮 下

        string writeBack_DOWN1 = "",
            writeBack_DOWN2 = "",
            writeBack_DOWN3 = "",
            writeBack_DOWN4 = "",
            writeBack_DOWN5 = "",
            writeBack_DOWN6 = "",
            writeBack_DOWN7 = "",
            writeBack_DOWN8 = "",
            writeBack_DOWN9 = "";

        //下--第一列--减 
        private void button39_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN1))
                writeBack_DOWN1 = ControlWriteBackHs["L_DOWN_1ZL"].ToString();
            double dValue = MinusValue(L_DOWN_1ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_1ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN1, dValue);
            }
        }
        //下--第一列--加 
        private void button38_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN1))
                writeBack_DOWN1 = ControlWriteBackHs["L_DOWN_1ZL"].ToString();
            double dValue = AddValue(L_DOWN_1ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_1ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN1, dValue);
            }
        }

        //下--第二列--减 
        private void button37_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN2))
                writeBack_DOWN2 = ControlWriteBackHs["L_DOWN_2ZL"].ToString();
            double dValue = MinusValue(L_DOWN_2ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_2ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN2, dValue);
            }
        }
        //下--第二列--加 
        private void button36_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN2))
                writeBack_DOWN2 = ControlWriteBackHs["L_DOWN_2ZL"].ToString();
            double dValue = AddValue(L_DOWN_2ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_2ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN2, dValue);
            }
        }

        //下--第三列--减 
        private void button35_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN3))
                writeBack_DOWN3 = ControlWriteBackHs["L_DOWN_3ZL"].ToString();
            double dValue = MinusValue(L_DOWN_3ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_3ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN3, dValue);
            }
        }
        //下--第三列--加 
        private void button34_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN3))
                writeBack_DOWN3 = ControlWriteBackHs["L_DOWN_3ZL"].ToString();
            double dValue = AddValue(L_DOWN_3ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_3ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN3, dValue);
            }
        }

        //下--第四列--减 
        private void button33_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN4))
                writeBack_DOWN4 = ControlWriteBackHs["L_DOWN_4ZL"].ToString();
            double dValue = MinusValue(L_DOWN_4ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_4ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN4, dValue);
            }
        }
        //下--第四列--加 
        private void button32_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN4))
                writeBack_DOWN4 = ControlWriteBackHs["L_DOWN_4ZL"].ToString();
            double dValue = AddValue(L_DOWN_4ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_4ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN4, dValue);
            }
        }

        //下--第五列--减 
        private void button31_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN5))
                writeBack_DOWN5 = ControlWriteBackHs["L_DOWN_5ZL"].ToString();
            double dValue = MinusValue(L_DOWN_5ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_5ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN5, dValue);
            }
        }
        //下--第五列--加 
        private void button30_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN5))
                writeBack_DOWN5 = ControlWriteBackHs["L_DOWN_5ZL"].ToString();
            double dValue = AddValue(L_DOWN_5ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_5ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN5, dValue);
            }
        }

        //下--第六列--减 
        private void button29_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN6))
                writeBack_DOWN6 = ControlWriteBackHs["L_DOWN_6ZL"].ToString();
            double dValue = MinusValue(L_DOWN_6ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_6ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN6, dValue);
            }
        }
        //下--第六列--加 
        private void button28_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN6))
                writeBack_DOWN6 = ControlWriteBackHs["L_DOWN_6ZL"].ToString();
            double dValue = AddValue(L_DOWN_6ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_6ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN6, dValue);
            }
        }

        //下--第七列--减 
        private void button27_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN7))
                writeBack_DOWN7 = ControlWriteBackHs["L_DOWN_7ZL"].ToString();
            double dValue = MinusValue(L_DOWN_7ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_7ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN7, dValue);
            }
        }
        //下--第七列--加 
        private void button26_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN7))
                writeBack_DOWN7 = ControlWriteBackHs["L_DOWN_7ZL"].ToString();
            double dValue = AddValue(L_DOWN_7ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_7ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN7, dValue);
            }
        }

        //下--第八列--减 
        private void button25_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN8))
                writeBack_DOWN8 = ControlWriteBackHs["L_DOWN_8ZL"].ToString();
            double dValue = MinusValue(L_DOWN_8ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_8ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN8, dValue);
            }
        }
        //下--第八列--加 
        private void button24_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN8))
                writeBack_DOWN8 = ControlWriteBackHs["L_DOWN_8ZL"].ToString();
            double dValue = AddValue(L_DOWN_8ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_8ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN8, dValue);
            }
        }

        //下--第九列--减 
        private void button41_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN9))
                writeBack_DOWN9 = ControlWriteBackHs["L_DOWN_9ZL"].ToString();
            double dValue = MinusValue(L_DOWN_9ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_9ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN9, dValue);
            }
        }
        //下--第九列--加 
        private void button40_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(writeBack_DOWN9))
                writeBack_DOWN9 = ControlWriteBackHs["L_DOWN_9ZL"].ToString();
            double dValue = AddValue(L_DOWN_9ZL.Text);
            if (dValue == -999)
                return;
            else
            {
                this.L_DOWN_9ZL.Text = dValue + "";
                SendCommand(writeBack_DOWN9, dValue);
            }
        }

        #endregion

        #region 变频循环水泵调节按钮


        //变频循环水泵 频率调节 - 
        private void button43_Click(object sender, EventArgs e)
        {
            string dValue = this.labelZLPL.Text;
            if (string.IsNullOrEmpty(dValue))
                return;
            if (dValue.Contains("HZ") || dValue.Contains("Hz"))
                dValue = dValue.Replace("HZ", "").Replace("Hz", "");
            try
            {
                double dv = double.Parse(dValue);
                dv = dv - 0.5;
                if (dv < 0)
                    dv = 0;
                SendCommand("5BYJATAG090.optarget", dv);
                this.labelZLPL.Text = dv + "";
            }
            catch (Exception ee)
            {
                WriteLog.WriteLogs(ee.ToString());
                return;
            }
        }
        //变频循环水泵 频率调节 -- 
        private void button42_Click(object sender, EventArgs e)
        {
            string dValue = this.labelZLPL.Text;
            if (string.IsNullOrEmpty(dValue))
                return;
            if (dValue.Contains("HZ") || dValue.Contains("Hz"))
                dValue = dValue.Replace("HZ", "").Replace("Hz", "");
            try
            {
                double dv = double.Parse(dValue);
                dv = dv - 2;
                if (dv < 0)
                    dv = 0;
                SendCommand("5BYJATAG090.optarget", dv);
                this.labelZLPL.Text = dv + "";
            }
            catch (Exception ee)
            {
                WriteLog.WriteLogs(ee.ToString());
                return;
            }
        }

        //变频循环水泵 频率调节 + 
        private void button45_Click(object sender, EventArgs e)
        {
            string dValue = this.labelZLPL.Text;
            if (string.IsNullOrEmpty(dValue))
                return;
            if (dValue.Contains("HZ") || dValue.Contains("Hz"))
                dValue = dValue.Replace("HZ", "").Replace("Hz", "");
            try
            {
                double dv = double.Parse(dValue);
                dv = dv + 0.5;
                if (dv > 50)
                    dv = 50;
                SendCommand("5BYJATAG090.optarget", dv);
                this.labelZLPL.Text = dv + "";
            }
            catch (Exception ee)
            {
                WriteLog.WriteLogs(ee.ToString());
                return;
            }
        }
        //变频循环水泵 频率调节 ++ 
        private void button44_Click(object sender, EventArgs e)
        {
            string dValue = this.labelZLPL.Text;
            if (string.IsNullOrEmpty(dValue))
                return;
            if (dValue.Contains("HZ") || dValue.Contains("Hz"))
                dValue = dValue.Replace("HZ", "").Replace("Hz", "");
            try
            {
                double dv = double.Parse(dValue);
                dv = dv + 2;
                if (dv > 50)
                    dv = 50;
                SendCommand("5BYJATAG090.optarget", dv);
                this.labelZLPL.Text = dv + "";
            }
            catch (Exception ee)
            {
                WriteLog.WriteLogs(ee.ToString());
                return;
            }
        }

        #endregion

        #region 扇区开度调节公用 加/减 方法
        /// <summary>
        /// 扇区开度调节 减方法 
        /// </summary>
        /// <param name="backTag"></param>
        /// <param name="dValue"></param>
        /// <returns></returns>
        private double MinusValue(string dValue)
        {
            if (string.IsNullOrEmpty(dValue))
                return -999;
            if (dValue.Contains("%"))
                dValue = dValue.Replace("%", "");

            try
            {
                double dv = double.Parse(dValue);
                dv = dv - 4.25;
                if (dv < 0)
                    dv = 0;
                return dv;
            }
            catch (Exception ee)
            {
                WriteLog.WriteLogs(ee.ToString());
                return -999;
            }
        }

        /// <summary>
        /// 扇区开度调节 加方法 
        /// </summary>
        /// <param name="backTag"></param>
        /// <param name="dValue"></param>
        /// <returns></returns>
        private double AddValue(string dValue)
        {
            if (string.IsNullOrEmpty(dValue))
                return -999;
            if (dValue.Contains("%"))
                dValue = dValue.Replace("%", "");

            try
            {
                double dv = double.Parse(dValue);
                dv = dv + 4.25;
                if (dv > 60)
                    dv = 60;
                return dv;
            }
            catch (Exception ee)
            {
                WriteLog.WriteLogs(ee.ToString());
                return -999;
            }
        }
        #endregion

        #region 发送指令公用方法

        int secondTime = 0;
        Hashtable writeBackHs = new Hashtable();
        private object locker = new object();
        private void SendCommand(string backTag, object dValue)
        {
            try
            {
                //线程锁 防止 secondTime 自增的时候并发冲突 ZZH 
                lock (locker)
                {
                    secondTime = 0;
                }

                //HashTable锁，有读写一定要锁SyncRoot，而不是obj ZZH 
                lock (writeBackHs.SyncRoot)
                {
                    if (writeBackHs.ContainsKey(backTag))
                    {
                        writeBackHs.Remove(backTag);
                        writeBackHs.Add(backTag, dValue);
                    }
                    else
                        writeBackHs.Add(backTag, dValue);
                }

                if (timer.Enabled == false)
                    timer.Enabled = true;
            }
            catch (Exception ee)
            {
                WriteLog.WriteLogs(ee.ToString());
            }
        }

        private void timer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (locker)
            {
                secondTime += 1;
            }
            if (secondTime > 1)
            {
                //进来以后把 secondTime 清零，把 timer 停止
                secondTime = 0;
                this.timer.Enabled = false;

                //发送指令 
                //发送之后把Hashtable清空，把timer关了 
                lock (writeBackHs.SyncRoot)
                {
                    try
                    {
                        if (writeBackHs.Count <= 0)
                            return;
                        if (GlobalVariables.IsWriteBackMT)
                        {
                            int count = opc.WriteHash(writeBackHs);
                            //回写成功了多少条记录  
                        }
                        //测试用 
                        string strs = "";
                        foreach (DictionaryEntry d in writeBackHs)
                            strs += "测点：" + d.Key + " 值：" + d.Value + "\r\n";
                        strs += "一共回写 " + writeBackHs.Count + " 个命令：" + "，发送成功 ";
                        WriteLog.WriteLogs(strs);//+ count + " 个");
                        writeBackHs.Clear();
                    }
                    catch (Exception ee)
                    {
                        writeBackHs.Clear();
                        WriteLog.WriteLogs(ee.ToString());
                    }
                    finally
                    {
                        this.timer.Enabled = false;
                    }
                }
            }
        }
        #endregion


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            GlobalVariables.YXYHTF = false;
            string sSql = "update [Config] set ConfigValue='1' where ConfigKey='IsTF5'";
            try { SQLHelper.ExecuteSql(sSql); }
            catch { }
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            GlobalVariables.YXYHTF = true;
        }










    }
}
