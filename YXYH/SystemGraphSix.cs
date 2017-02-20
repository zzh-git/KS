using HAOCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinHost;

namespace YHYXYH.YXYH
{
    public partial class SystemGraphSix : Form
    {
        public SystemGraphSix()
        {
            InitializeComponent(); 
            AddConfigedControl();

            SetBindingLabelsText();
            SetLabelVisible(true);
        }

        //string sUnit1HCGD_IDs = "1184,1186,1190,1262";//1号机组好处归电要显示的ID
        //string sUnit1HCGR_IDs = "1380,1381,3385,3399";//1号机组好处归热要显示的ID
        string sUnit2HCGD_IDs = "2184,2186,2190,2262";//2号机组好处归电要显示的ID
        string sUnit2HCGR_IDs = "2380,2381,3385,3399";//2号机组好处归热要显示的ID
        DataTable dtControls = SQLHelper.ExecuteDt("select * from [ControlConfig] where Parent like 'YXYHMainSix%' order by Parent asc");
        DataView viewTagLabelBinding = TableTagLabelBinding.getDataTableByRowFilter("[Parent] like 'YXYHMainSix%'").DefaultView;
        HistoryLines m_frmHistoryLines = new HistoryLines(5);
        private void SystemGraphSix_Load(object sender, EventArgs e)
        {
            timerLineBind.Interval = GlobalVariables.RefIntvel;
            timerLineBind.Enabled = true;

            //SetBindingLabelsText();
        }

        /// <summary>
        /// 加载程序运行中配置的控件
        /// </summary>
        void AddConfigedControl()
        {
            try
            {
                int i;
                Control parent = null;
                Control control = null;
                Rectangle rect;
                LabelTag labelTag = null;
                foreach (DataRow row in dtControls.Rows)
                {

                    parent = panel1;
                    try
                    {

                        if (row["type"].ToString() == "PictureBox")
                        { 
                            int controlX = int.Parse(row["ControlX"].ToString());
                            int controlY = int.Parse(row["ControlY"].ToString());
                            control = new PictureBox();
                            ((PictureBox)control).BackgroundImage = global::KSPrj.Properties.Resources.verticalGreen;
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
                            control.Tag = labelTag;

                            control.Bounds = rect;
                            control.Size = new System.Drawing.Size(12, 18); 
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
                        try { control.BackColor = Color.Transparent; }//Color.FromArgb(int.Parse(row["BackColor"].ToString()));
                        catch { }
                        labelTag = new LabelTag();
                        labelTag.ControlX = rect.X;
                        labelTag.ControlY = rect.Y;
                        labelTag.ParentHeight = int.Parse(row["ParentHeight"].ToString());
                        labelTag.ParentWidth = int.Parse(row["ParentWidth"].ToString());
                        labelTag.UnitNo = byte.Parse(row["UnitNo"].ToString());
                        control.Tag = labelTag;
                        control.DoubleClick += new EventHandler(Label_DoubleClick);//双击事件
                        parent.Controls.Add(control);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            Rectangle rect;
            LabelTag labelTag = null;
            panel1.Visible = false;
            foreach (Control control in panel1.Controls)
            {
                try
                {
                    labelTag = (LabelTag)(control.Tag);
                    rect = new Rectangle();
                    rect.X = Convert.ToInt32(1.0 * labelTag.ControlX / labelTag.ParentWidth * panel1.Width);
                    rect.Y = Convert.ToInt32(1.0 * labelTag.ControlY / labelTag.ParentHeight * panel1.Height);
                    control.Bounds = rect;

                    if (control.GetType() == typeof(PictureBox))
                    {
                        control.Size = new System.Drawing.Size((int)(22.0 / labelTag.ParentWidth * panel1.Width), (int)(28.0 / labelTag.ParentHeight * panel1.Height));
                        control.Refresh();
                    }
                }
                catch { }
            }
            panel1.Visible = true;
        }

        /// <summary>
        /// 设定绑定标签的Text（测点值+测点单位）。
        /// </summary>
        void SetBindingLabelsText()
        {
            try
            {
                int i;
                LabelTag labelTag;
                Control control, parent = panel1;
                foreach (DataRowView row in viewTagLabelBinding)
                {
                    try
                    {
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
                            }
                        }
                        catch
                        {
                            control.Tag = new LabelTag(true, int.Parse(row["TagID"].ToString()), row["TagDesc"].ToString(), row["unit"].ToString());
                            toolTip1.SetToolTip(control, row["TagDesc"].ToString());
                        }

                        //这里是否需要根据机组区分 ZZH 
                        control.Text = TagValue.GetShowValueSix(labelTag.TagID, labelTag.TagUnit, row["adjustValue"].ToString());
                        control.Refresh();
                    }
                    catch { }
                }
                lblRunOptimizePromptText.Text = RunOptimizePrompt.GetPromptText();
                lblRunOptimizePromptText.ForeColor = RunOptimizePrompt.GetPromptTextForeColor();

                lblTemp.Text = "环境温度：" + TagValue.GetShowValueSix(4208, "℃", ""); //+GlobalVariables.dtOneRowData.Rows[0]["F3208"].ToString();
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
        }
        /// <summary>
        /// 设置Label的可见性
        /// </summary>
        public void SetLabelVisible(bool bHCGDChecked)
        {
            Control control, parent = panel1;
            LabelTag labelTag = null; 
            int i;
            string sTagID = "";
            string[] sHideIDs;//要隐藏的测点
            if (bHCGDChecked == true)
                sHideIDs = sUnit2HCGR_IDs.Split(',');
            else
                sHideIDs = sUnit2HCGD_IDs.Split(',');

            dtControls = TableControlConfig.getDataTableByRowFilter("[Parent] like 'YXYHMainSix.%' and [TagID] in (" + sUnit2HCGD_IDs + "," + sUnit2HCGR_IDs + ")");
            foreach (DataRow row in dtControls.Rows)
            {
                try
                {
                    //由父控件找到本Label
                    try { control = parent.Controls.Find(row["ControlName"].ToString(), false)[0]; }
                    catch { continue; }
                    labelTag = (LabelTag)control.Tag; 

                    control.Visible = true;
                    sTagID = row["TagID"].ToString();
                    for (i = 0; i < sHideIDs.Length; i++)
                    {
                        if (sTagID == sHideIDs[i])
                        {
                            control.Visible = false;
                            break;
                        }
                    }
                     
                }
                catch (Exception ex)
                {
                    WriteLog.WriteLogs(ex.ToString());
                }
            }
            dtControls.Dispose();
            dtControls = null;
        }


        private void timerLineBind_Tick(object sender, EventArgs e)
        {
            SetBindingLabelsText();
            //KLDStateCheck();
        }

        private void SystemGraph_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
        }

        private void Label_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Label label = (Label)sender;
                m_frmHistoryLines.labelTag.TagID = ((LabelTag)label.Tag).TagID;
                m_frmHistoryLines.labelTag.TagDesc = ((LabelTag)label.Tag).TagDesc;
                m_frmHistoryLines.ShowDialog();
            }
            catch { }
        }

        int m_iTimerDateTimeCount = 0;
        private void timerDateTime_Tick(object sender, EventArgs e)
        {

            if (lblRunOptimizePromptText.Text != RunOptimizePrompt.GetPromptText())
                lblRunOptimizePromptText.Text = RunOptimizePrompt.GetPromptText();
            if (RunOptimizePrompt.GetPromptTextForeColor() == Color.Red)
            {
                if (m_iTimerDateTimeCount % 2 == 0)
                    lblRunOptimizePromptText.ForeColor = Color.DimGray;
                else
                    lblRunOptimizePromptText.ForeColor = Color.Red;

            }
            else
                lblRunOptimizePromptText.ForeColor = Color.Lime;


            m_iTimerDateTimeCount += 1;
        }

        //空冷岛阀门状态检查 
        //private void KLDStateCheck()
        //{
        //    if (GlobalVariables.UnitNumber == 1)
        //    {
        //        int A1 = (int)CalTree.GetFinishedTagValue("F1401");
        //        int B1 = (int)CalTree.GetFinishedTagValue("F1402");
        //        int C1 = (int)CalTree.GetFinishedTagValue("F1403");
        //        int A2 = (int)CalTree.GetFinishedTagValue("F1404");
        //        int B2 = (int)CalTree.GetFinishedTagValue("F1405");
        //        int C2 = (int)CalTree.GetFinishedTagValue("F1406");

        //        RedGreen("picBoxA110", A1);
        //        RedGreen("picBoxB110", B1);
        //        RedGreen("picBoxC110", C1);
        //        RedGreen("picBoxA210", A2);
        //        RedGreen("picBoxB210", B2);
        //        RedGreen("picBoxC210", C2);
        //    }
        //    else
        //    {
        //        int A1 = (int)CalTree.GetFinishedTagValue("F2401");
        //        int B1 = (int)CalTree.GetFinishedTagValue("F2402");
        //        int C1 = (int)CalTree.GetFinishedTagValue("F2403");
        //        int A2 = (int)CalTree.GetFinishedTagValue("F2404");
        //        int B2 = (int)CalTree.GetFinishedTagValue("F2405");
        //        int C2 = (int)CalTree.GetFinishedTagValue("F2406");

        //        RedGreen("picBoxA120", A1);
        //        RedGreen("picBoxB120", B1);
        //        RedGreen("picBoxC120", C1);
        //        RedGreen("picBoxA220", A2);
        //        RedGreen("picBoxB220", B2);
        //        RedGreen("picBoxC220", C2);
        //    }
        //}
        private void RedGreen(string picName, int open)
        {
            PictureBox pic = null;
            try
            {
                pic = this.panel1.Controls.Find(picName, false)[0] as PictureBox;
                if (open == 1)
                    pic.BackgroundImage = global::KSPrj.Properties.Resources.verticalRed;
                else
                    pic.BackgroundImage = global::KSPrj.Properties.Resources.verticalGreen;
            }
            catch { pic.BackgroundImage = global::KSPrj.Properties.Resources.verticalGreen; }
        }

    }
}
