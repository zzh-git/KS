using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using HAOCommon;
using WinHost;

namespace YHYXYH.YXYH
{
    /// <summary>
    /// 运行优化提示窗口
    /// </summary>
    public partial class RunOptimizePromptSix : Form
    {
        int UnitNO = 6;
        public RunOptimizePromptSix()
        {
            InitializeComponent();
            //this.UnitNO = unitNo;
            timerSound.Elapsed += new ElapsedEventHandler(timerSound_Elapsed);
            timerSound.Start();
        }

        static bool m_bIsPromptAlarm = false;//是否提示报警
        static bool m_bIsRefused = false;//是否拒绝过
        static DateTime m_timeChanged = DateTime.Now;//计算出的运行优化提示类型发生改变的时间
        static DateTime m_timeAccept = DateTime.Now.AddDays(-1);//点击“知道了”按钮的时间
        static DateTime m_timeRefuse = DateTime.Now.AddDays(-2);//点击窗口关闭按钮的时间
        static System.Timers.Timer timerSound = new System.Timers.Timer(3000);//报警的定时器
        //点击知道了的时候，保存当前的运行优化提示类型
        public static RunOptimizePromptType m_RunOptimizePromptTypeSaved = RunOptimizePromptType.Optimized;
        //当前计算出的运行优化提示类型
        public static RunOptimizePromptType m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.Optimized;
        // 获取提示文本前景色
        /// <summary>
        /// 获取提示文本前景色
        /// </summary>
        /// <returns></returns>
        public static Color GetPromptTextForeColor()
        {
            if (m_RunOptimizePromptTypeCurrent == RunOptimizePromptType.Optimized
                || m_RunOptimizePromptTypeCurrent == RunOptimizePromptType.TFGK 
                || m_RunOptimizePromptTypeCurrent == RunOptimizePromptType.CNGK)
                return Color.Lime;
            else
                return Color.Red;
        }
        // 获取提示文本
        /// <summary>
        /// 获取提示文本
        /// </summary>
        /// <returns></returns>
        public static string GetPromptText()
        {
            string sReturn = "";
            switch (m_RunOptimizePromptTypeCurrent)
            {
                case RunOptimizePromptType.Optimized:
                    sReturn = "系统优化中，一切正常。";
                    break;
                case RunOptimizePromptType.IncreaseHeadingLoad:
                    sReturn = "可以增加热负荷，提高热网出水温度！";
                    break;
                case RunOptimizePromptType.ReduceHeadingLoad:
                    sReturn = "请减少热负荷！";
                    break;
                case RunOptimizePromptType.ReduceSteamExtractionCapacity:
                    sReturn = "请减少抽汽量！";
                    break;
                case RunOptimizePromptType.IncreaseHeadingNetworkWaterCapacity:
                    sReturn = "请增大热网循环水流量！";
                    break;
                case RunOptimizePromptType.ReduceHeadingNetworkOutWaterTemperature:
                    sReturn = "请降低热网供水温度！";
                    break;
                case RunOptimizePromptType.ReduceThatSteamExtractionCapacity:
                    sReturn = "请减少临机供热抽汽流量，降低热网出水温度！";
                    break;
                case RunOptimizePromptType.ReduceThisSteamExtractionCapacity:
                    sReturn = "请减少本机供热抽汽流量，降低热网出水温度！";
                    break;
                case RunOptimizePromptType.TGBY:
                    sReturn = "请提高背压！";
                    break;
                case RunOptimizePromptType.JDBY:
                    sReturn = "请降低背压！";
                    break;
                case RunOptimizePromptType.TFGK:
                    sReturn = "调峰工况，无优化！";
                    break;
                case RunOptimizePromptType.CNGK:
                    sReturn = "抽凝工况，无优化！";
                    break;
            }
            return sReturn;
        }
        // 获取报警序号
        /// <summary>
        /// 获取报警序号
        /// </summary>
        /// <returns></returns>
        public static int GetPromptNo()
        {
            int iReturn = 1;
            iReturn = (int)m_RunOptimizePromptTypeCurrent;
            return iReturn;
        }
        // 判断提示报警标记
        /// <summary>
        /// 判断提示报警标记
        /// </summary>
        static void JustPromptAlarm(int UnitNO)
        {
            m_bIsPromptAlarm = false;
            m_bIsRefused = false;
            if (m_RunOptimizePromptTypeCurrent != m_RunOptimizePromptTypeSaved)//报警状态发生改变
            {
                //保存运行优化提示类型改变的时间和新产生的类型
                DateTime timeCurrentChanged = DateTime.Now;//保存报警改变的时间
                if (m_RunOptimizePromptTypeSaved != RunOptimizePromptType.Optimized)//上次报警结束
                {
                    string sSql = "update AlarmYXYH set AlarmState='报警结束',AlarmEndTime='"
                        + PublicFunction.DateTimeToString(timeCurrentChanged) + "',AlarmLasts='"
                        + PublicFunction.GetTimeSpanString(timeCurrentChanged - m_timeChanged)
                        + "' where AlarmTime='" + PublicFunction.DateTimeToString(m_timeChanged) + "' and UnitNO="+UnitNO;
                    try { SQLHelper.ExecuteSql(sSql); }
                    catch { WriteLog.WriteLogs("执行SQL失败：" + sSql); }
                }
                if (m_RunOptimizePromptTypeCurrent != RunOptimizePromptType.Optimized)//新报警开始
                {
                    m_bIsPromptAlarm = true;
                    string sSql = "insert into AlarmYXYH(AlarmTime,AlarmText,AlarmState,UnitNO) values('"
                        + PublicFunction.DateTimeToString(timeCurrentChanged) + "','" + GetPromptText() + "','正在报警',"+UnitNO+")";
                    try { SQLHelper.ExecuteSql(sSql); }
                    catch { WriteLog.WriteLogs("执行SQL失败：" + sSql); }
                }
                m_timeChanged = timeCurrentChanged;
                m_RunOptimizePromptTypeSaved = m_RunOptimizePromptTypeCurrent;
            }
            else if (m_RunOptimizePromptTypeCurrent != RunOptimizePromptType.Optimized)
            {
                if ((m_timeAccept - m_timeChanged).TotalSeconds > 0 || (m_timeRefuse - m_timeChanged).TotalSeconds > 0)
                {
                    if ((m_timeAccept - m_timeRefuse).TotalSeconds > 0)//点击“接受”之后的时间判断
                    {
                        if ((DateTime.Now - m_timeAccept).TotalMinutes > 30)//点击“知道了”按钮30分钟之后，如果未优化调整，则再次报警
                            m_bIsPromptAlarm = true;
                    }
                    else//点击窗口关闭按钮之后的时间判断
                    {
                        if ((DateTime.Now - m_timeRefuse).TotalHours > 8)//点击窗口关闭按钮8小时之后，如果未优化调整，则再次报警
                            m_bIsPromptAlarm = true;
                    }
                }
                else
                    m_bIsPromptAlarm = true;
            }
        }
        // 计算运行优化提示类型
        /// <summary>
        /// 计算运行优化提示类型
        /// </summary>
        public static void CalculateRunOptimizePromptType(int UnitNO)
        {
            if(GlobalVariables.YXYH_GBY_CN_JS6==false)
            {
                m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.CNGK;
                m_bIsPromptAlarm = false;
                return;
            }
            if (GlobalVariables.YXYHTF6) 
            {
                m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.TFGK;
                m_bIsPromptAlarm = false;
                return;
            }

            double dF2019 = 0;//主汽流量
            double dF2145 = 0;//发电负荷
            double dF4152 = TagValue.GetFinishedTagValueFive("F4152");//热网供水温度
            double dF4150 = TagValue.GetFinishedTagValueFive("F4150");//凝汽器循环进水温度
            double dF4410 = TagValue.GetFinishedTagValueFive("F4410");//优化出水温度上限
            double dF4411 = TagValue.GetFinishedTagValueFive("F4411");//优化出水温度下限
            double dF2110 = TagValue.GetFinishedTagValueFive("F2110");//1#机抽汽流量
            //double dF2110 = TagValue.GetFinishedTagValueFive("F2110");//2#机抽汽流量
            double dF4161 = TagValue.GetFinishedTagValueFive("F4161");//三期热网供水实时流量（至外网）
            double dF4501 = TagValue.GetFinishedTagValueFive("F4501");//二期热网供水实时流量（至外网）
            double dF4222 = TagValue.GetFinishedTagValueFive("F4222");//优化热网水量
            double dF4002 = TagValue.GetFinishedTagValueFive("F4002");//末级叶片颤振值
 
            dF2019 = TagValue.GetFinishedTagValueFive("F2019");
            dF2145 = TagValue.GetFinishedTagValueFive("F2145");
              
            if (dF2145 < 100)//发电负荷小于100相当于停机，退出不做判断
                return;

            m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.Optimized;
            if (dF2019 > 1000)//主汽量大于1000吨/小时
            {
                if (dF2145 < 260)//负荷小于26万
                {
                    if (dF4152 > dF4410 || dF4150 > 50)//供水温度高于计算温度
                    { 
                        //if (dF2110 > 5)//临机有抽气流量
                        //    m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.ReduceThatSteamExtractionCapacity;
                        if (dF2110 > 5)//本机有抽气流量
                            m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.ReduceThisSteamExtractionCapacity;
                        else if (dF4161 + dF4501 < dF4222)
                            m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.IncreaseHeadingNetworkWaterCapacity;
                        
                    }
                }
            }
            else
            {
                if (dF4161 + dF4501 < dF4222)
                    m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.IncreaseHeadingNetworkWaterCapacity;
                if (m_RunOptimizePromptTypeCurrent == RunOptimizePromptType.Optimized && (dF4152 > dF4410 || dF4150 > 50))//供水温度高，或热网回水温度高于48度
                { 
                    //if (dF2110 > 5)//临机有抽气流量
                    //    m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.ReduceThatSteamExtractionCapacity;
                    if (dF2110 > 5)//本机有抽气流量
                        m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.ReduceThisSteamExtractionCapacity;
                    else if (dF4161 + dF4501 < dF4222)
                        m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.IncreaseHeadingNetworkWaterCapacity;
                    
                }

                if (m_RunOptimizePromptTypeCurrent == RunOptimizePromptType.Optimized && (dF4152< dF4411 || dF4150 < 48))//供水温度低，或热网回水温度低于48度
                {
                    m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.IncreaseHeadingLoad;
                }
            }


            if (m_RunOptimizePromptTypeCurrent == RunOptimizePromptType.Optimized & dF2110 > 5 & dF4002 < 80)
                m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.TGBY;
            if (m_RunOptimizePromptTypeCurrent == RunOptimizePromptType.Optimized & dF2110 < 5 & dF4002 > 92)
                m_RunOptimizePromptTypeCurrent = RunOptimizePromptType.JDBY;

            //try { WriteLog.WriteLogs("yxyh:" + ((int)m_RunOptimizePromptTypeCurrent) + " ; " + GlobalVariables.dtOneRowDataFive.Rows[0]["F4322"].ToString()); }
            //catch { }
            JustPromptAlarm(UnitNO);

            if (m_bIsPromptAlarm)
                timerSound.Start();
        }

        static void timerSound_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (m_bIsPromptAlarm)
                KSPrj.Alert.goYXYHSound();
        }

        // 获取是否提示报警的状态
        /// <summary>
        /// 获取是否提示报警的状态
        /// </summary>
        /// <returns></returns>
        public bool IsPromptAlarm()
        {
            lblPrompt.ForeColor = GetPromptTextForeColor();
            lblPrompt.Text = GetPromptText();
            lblPrompt.Refresh();
            return m_bIsPromptAlarm;
        }

        private void RunOptimizePrompt_Load(object sender, EventArgs e)
        {
            lblPrompt.ForeColor = GetPromptTextForeColor();
            lblPrompt.Text = GetPromptText();
            this.Refresh();

        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            //点击“接受”按钮
            m_timeAccept = DateTime.Now;
            timerSound.Stop();
            this.Visible = false;
            m_bIsPromptAlarm = false;
            if (m_bIsRefused == false)
                SQLHelper.ExecuteSql("update AlarmYXYH set [Operation]='接受' where AlarmTime='"
                    + PublicFunction.DateTimeToString(m_timeChanged) + "' and UnitNO=" + UnitNO);
        }

        private void RunOptimizePrompt_FormClosing(object sender, FormClosingEventArgs e)
        {
            //点击“接受”按钮
            m_timeAccept = DateTime.Now;
            timerSound.Stop();
            this.Visible = false;
            m_bIsPromptAlarm = false;
            e.Cancel = true;
        }

        private void btnRefuse_Click(object sender, EventArgs e)
        {
            //点击“拒绝”按钮
            m_timeRefuse = DateTime.Now;
            timerSound.Stop();
            m_bIsRefused = true;
            this.Visible = false;
            m_bIsPromptAlarm = false;
            SQLHelper.ExecuteSql("update AlarmYXYH set [Operation]='拒绝',AlarmDesc=isnull(AlarmDesc,'')+'" + txtRefuseReasion.Text
                + "' where AlarmTime='" + PublicFunction.DateTimeToString(m_timeChanged) + "' and UnitNO=" + UnitNO);
        }



    }



    //// 运行优化提示类型
    ///// <summary>
    ///// 运行优化提示类型
    ///// </summary>
    //public enum RunOptimizePromptType
    //{
    //    // 运行监测中，运行已优化
    //    /// <summary>
    //    /// 运行监测中，运行已优化
    //    /// </summary>
    //    Optimized = 0,
    //    // 可以增加热负荷
    //    /// <summary>
    //    /// 可以增加热负荷
    //    /// </summary>
    //    IncreaseHeadingLoad = 1,
    //    // 减少热负荷
    //    /// <summary>
    //    /// 减少热负荷
    //    /// </summary>
    //    ReduceHeadingLoad = 2,
    //    // 减少抽汽量
    //    /// <summary>
    //    /// 减少抽汽量
    //    /// </summary>
    //    ReduceSteamExtractionCapacity = 3,
    //    // 增加热网水量
    //    /// <summary>
    //    /// 增加热网水量
    //    /// </summary>
    //    IncreaseHeadingNetworkWaterCapacity = 4,
    //    // 降低热网供水温度
    //    /// <summary>
    //    /// 降低热网供水温度
    //    /// </summary>
    //    ReduceHeadingNetworkOutWaterTemperature = 5,
    //    // 减少临机组抽汽量
    //    /// <summary>
    //    /// 减少临机组抽汽量
    //    /// </summary>
    //    ReduceThatSteamExtractionCapacity = 6,
    //    // 减少本机组抽汽量
    //    /// <summary>
    //    /// 减少本机组抽汽量
    //    /// </summary>
    //    ReduceThisSteamExtractionCapacity = 7,
    //    /// <summary>
    //    /// 调峰工况 
    //    /// </summary>
    //    TFGK = 20

    //}
}
