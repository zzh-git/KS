using HAOCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KSPrj
{
    public partial class AlarmReport : Form
    {
        
        string AlarmName = "";
        System.Timers.Timer timer = new System.Timers.Timer();
        int UnitNO = 5;

        //抛物线图表提示
        public string NoticeLabel_PWXYellow = "此时末级叶片输出功率为零，排汽温度开始提高，并升温速度较快，注意运行排汽温度，及时喷水";
        public string NoticeLabel_PWXOrange = "运行工况已经接近颤振区域了，建议增大热网循环水量、降低出水温度、降低回水温度";
        public string NoticeLabel_PWXRed = "末级叶片已经进入颤振区域，运行时间不得大于900秒";
        //棒状图提示
        public string NoticeLabel_PICYellow = "运行工况已经接近颤振区域了，建议增大热网循环水量、降低出水温度、降低回水温度";
        public string NoticeLabel_PICOrange = "末级叶片动应力上升迅速，请增加间冷塔循环水量，增大百叶窗开度";
        public string NoticeLabel_PICRed = "末级叶片已经进入颤振区域，运行时间不得大于900秒";
        //折线图提示
        public string NoticeLabel_ZXOrange = "背压进入限制区域，建议增大热网循环水量、降低出水温度，或增加间冷塔循环水量、增大百叶窗开度";
        public string NoticeLabel_ZXRed = "末级叶片已经进入颤振区域，运行时间不得大于900秒";

        public AlarmReport(string AlarmName, int UnitNO)
        {
            InitializeComponent();
            this.button1.Visible = false;
            this.timer1.Enabled = false;
            this.FormClosing += AlarmReport_FormClosed;
            this.AlarmName = AlarmName;
            this.UnitNO = UnitNO;
            if (UnitNO == 5)
                this.UnitName.Text = "#5机组报警";
            else if(UnitNO == 6)
                this.UnitName.Text = "#6机组报警";
            timer.Interval = 500;
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = false;
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        { 
            timer.Enabled = false;
            Alert.gosound();
        }

        private void AlarmReport_FormClosed(object sender, FormClosingEventArgs e)
        {
            if (currentAlarm == AlarmType.AlarmYellow)
                SetMinituesYellow = 15;
            else if (currentAlarm == AlarmType.AlarmOrange)
                SetMinituesOrange = 15;
            else if (currentAlarm == AlarmType.AlarmWhite)
                SetMinituesWhite = 15;

            this.timer1.Enabled = false;
            this.Visible = false;
            ConfirmTime = DateTime.Now;
            e.Cancel = true;
        }

        

        /// <summary>
        /// 设置要报警的内容/颜色/闪烁时间间隔
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="NoticeText"></param>
        /// <param name="type"></param>
        /// <param name="timeInterval"></param>
        /// <param name="isShowHide">是否显示隐藏按钮，隐藏按钮仅作用于黄色和橙色报警</param>
        /// <param name="flag">仅用于判断空冷岛阀门状态</param>
        public void SetText(string txt, string NoticeText, AlarmType type, int timeInterval, bool isShowHide, int flag) 
        {

            this.AlarmMessage = txt;
            this.flags = flag;
            if (SetAlarmType(type))
            {
                if (currentAlarm == AlarmType.AlarmRed)
                {
                    if (isAlarming)
                        timer.Enabled = true;
                    else { 
                        this.button1.Visible = false;
                        currentColor = Color.Red;
                        //Alert.gosound();
                        timer.Enabled = true;
                    }
                }
                else if (currentAlarm == AlarmType.AlarmYellow)
                {
                    if (isAlarming)
                    {
                        //下面这个判断临时添加（20151030）
                        //抛物线黄色报警提示但不响声音 
                        if (txt == "相对动应力显示图表黄色警报")
                            timer.Enabled = false;
                        else
                            timer.Enabled = true;
                        //return;
                    }
                    else
                    {
                        this.button1.Visible = isShowHide;
                        currentColor = Color.Yellow;
                        //Alert.gosound();
                        //timer_Elapsed(null, null);
                        //timer.Interval = 10;
                        if (txt == "相对动应力显示图表黄色警报")
                            timer.Enabled = false;
                        else
                            timer.Enabled = true;
                    }
                }
                else if (currentAlarm == AlarmType.AlarmOrange)
                {
                    if (isAlarming)
                    {
                        timer.Enabled = true;
                        //return;
                    }
                    else
                    {
                        this.button1.Visible = isShowHide;
                        currentColor = Color.DarkOrange;
                        //Alert.gosound();
                        timer.Enabled = true;
                    }
                }
                else if (currentAlarm == AlarmType.AlarmWhite)
                {
                    if (isAlarming)
                    {
                        timer.Enabled = true;
                        //return;
                    }
                    else
                    {
                        this.button1.Visible = isShowHide;
                        currentColor = Color.Tomato;
                        //Alert.gosound();
                        timer.Enabled = true;
                    }
                }
                
                this.promteText.Text = txt + alarmMes;
                this.promteText.ForeColor = currentColor;
                this.noticeLabel.Text = NoticeText;
                this.timer1.Interval = timeInterval;
                this.timer1.Enabled = true;
                this.Visible = true;
            }
            else
                this.Visible = false;
            
            
        }

        /// <summary>
        /// 报警显示内容
        /// </summary>
        string AlarmMessage = "";
        /// <summary>
        /// 当前字体颜色
        /// </summary>
        Color currentColor;
        /// <summary>
        /// 当前警报类型
        /// </summary>
        AlarmType currentAlarm = AlarmType.AlarmSafe;
        /// <summary>
        /// 附加报警内容（仅用于多次红色报警）
        /// </summary>
        string alarmMes = "";
        /// <summary>
        /// 第一次报警时间（数据库中主键）
        /// </summary>
        DateTime AlarmTime = DateTime.Now;
        /// <summary>
        /// 确认时间（用于判断上次确认时间是否大于指定时间）
        /// </summary>
        DateTime ConfirmTime = DateTime.Now;
        /// <summary>
        /// 如果下次还是相同警告，则延迟多少分钟再显示
        /// </summary> 
        int SetMinituesYellow = 0;
        int SetMinituesOrange = 0;
        int SetMinituesWhite = 0;
        int flags = 0;
        int currentFlag = 0;

        /// <summary>
        /// 是否正在报警（如果正在报警，那么就不用再次声音提示了）
        /// </summary>
        bool isAlarming = false;

        public bool SetAlarmType(AlarmType type) 
        {
            if(type == AlarmType.AlarmSafe)
            {
                if(currentAlarm == type)
                { 
                    return false;
                }
                else if(currentAlarm == AlarmType.AlarmYellow)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    SQLHelper.ExecuteSql("update AlarmFCZ set AlarmEndTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',AlarmState='已结束'" + ",AlarmLasts='" + PublicFunction.GetTimeSpanString(DateTime.Now - AlarmTime) + "' where AlarmTime='" +  AlarmTime.ToString("yyyy-MM-dd HH:mm:ss")+"'");
                    return false;
                }
                else if(currentAlarm == AlarmType.AlarmOrange)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    SQLHelper.ExecuteSql("update AlarmFCZ set AlarmEndTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',AlarmState='已结束'" + ",AlarmLasts='" + PublicFunction.GetTimeSpanString(DateTime.Now - AlarmTime) + "' where AlarmTime='" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    return false;
                }
                else if(currentAlarm == AlarmType.AlarmRed)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    SQLHelper.ExecuteSql("update AlarmFCZ set AlarmEndTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',AlarmState='已结束'" + ",AlarmLasts='" + PublicFunction.GetTimeSpanString(DateTime.Now - AlarmTime) + "' where AlarmTime='" +  AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    return false;
                }
                else if (currentAlarm == AlarmType.AlarmWhite)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    SQLHelper.ExecuteSql("update AlarmFCZ set AlarmEndTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',AlarmState='已结束'" + ",AlarmLasts='" + PublicFunction.GetTimeSpanString(DateTime.Now - AlarmTime) + "' where AlarmTime='" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    return false;
                }
                isAlarming = false;
            }
            else if(type == AlarmType.AlarmYellow)
            {
                //如果上一次是安全
                if(currentAlarm == AlarmType.AlarmSafe)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    AlarmTime = DateTime.Now;
                    SQLHelper.ExecuteSql("insert into AlarmFCZ (AlarmTime,AlarmText,AlarmState,AlarmDesc,UnitNO) values ('" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + AlarmMessage +"','正在报警','" + AlarmName + "',"+UnitNO+")");
                    isAlarming = false;
                }
                //如果一直是黄色报警，30分钟以内不再弹框
                else if (currentAlarm == type)
                {
                    if ((DateTime.Now - ConfirmTime).TotalMinutes < SetMinituesYellow)
                    {
                        return false;
                    }
                    else
                        isAlarming = true;
                }
                //如果上一次是橙色报警
                else if(currentAlarm == AlarmType.AlarmOrange)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    //结束橙色报警
                    SQLHelper.ExecuteSql("update AlarmFCZ set AlarmEndTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',AlarmState='已结束'" + ",AlarmLasts='" + PublicFunction.GetTimeSpanString(DateTime.Now - AlarmTime) + "' where AlarmTime='" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    //开始黄色报警
                    AlarmTime = DateTime.Now;
                    SQLHelper.ExecuteSql("insert into AlarmFCZ (AlarmTime,AlarmText,AlarmState,AlarmDesc,UnitNO) values ('" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + AlarmMessage + "','正在报警','" + AlarmName + "',"+UnitNO+")");
                    isAlarming = false;
                }
                //如果上一次是红色报警
                else if(currentAlarm == AlarmType.AlarmRed)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    //结束红色报警
                    SQLHelper.ExecuteSql("update AlarmFCZ set AlarmEndTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',AlarmState='已结束'" + ",AlarmLasts='" + PublicFunction.GetTimeSpanString(DateTime.Now - AlarmTime) + "' where AlarmTime='" +  AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    //开始黄色报警
                    AlarmTime = DateTime.Now;
                    SQLHelper.ExecuteSql("insert into AlarmFCZ (AlarmTime,AlarmText,AlarmState,AlarmDesc,UnitNO) values ('" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + AlarmMessage + "','正在报警','" + AlarmName + "',"+UnitNO+")");
                    isAlarming = false;
                }
                
            }
            else if(type == AlarmType.AlarmOrange)
            {
                if(currentAlarm == AlarmType.AlarmSafe)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    AlarmTime = DateTime.Now;
                    SQLHelper.ExecuteSql("insert into AlarmFCZ (AlarmTime,AlarmText,AlarmState,AlarmDesc,UnitNO) values ('" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + AlarmMessage + "','正在报警','" + AlarmName + "',"+UnitNO+")");
                    isAlarming = false;
                }
                else if (currentAlarm == type)
                {
                    if ((DateTime.Now - ConfirmTime).TotalMinutes < SetMinituesOrange)
                    {
                        return false;
                    }
                    else
                        isAlarming = true;
                }
                else if (currentAlarm == AlarmType.AlarmYellow)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    //结束橙色报警
                    SQLHelper.ExecuteSql("update AlarmFCZ set AlarmEndTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',AlarmState='已结束'" + ",AlarmLasts='" + PublicFunction.GetTimeSpanString(DateTime.Now - AlarmTime) + "' where AlarmTime='" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    //开始黄色报警
                    AlarmTime = DateTime.Now;
                    SQLHelper.ExecuteSql("insert into AlarmFCZ (AlarmTime,AlarmText,AlarmState,AlarmDesc,UnitNO) values ('" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + AlarmMessage + "','正在报警','" + AlarmName + "',"+UnitNO+")");
                    isAlarming = false;
                }
                else if(currentAlarm == AlarmType.AlarmRed)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    //结束红色报警
                    SQLHelper.ExecuteSql("update AlarmFCZ set AlarmEndTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',AlarmState='已结束'" + ",AlarmLasts='" + PublicFunction.GetTimeSpanString(DateTime.Now - AlarmTime) + "' where AlarmTime='" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    //开始黄色报警
                    AlarmTime = DateTime.Now;
                    SQLHelper.ExecuteSql("insert into AlarmFCZ (AlarmTime,AlarmText,AlarmState,AlarmDesc,UnitNO) values ('" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + AlarmMessage + "','正在报警','" + AlarmName + "',"+UnitNO+")");
                    isAlarming = false;
                }
            }
            else if(type == AlarmType.AlarmRed)
            {
                //如果上次也是一级报警
                if (currentAlarm == type)
                {
                    double second = 900d - (DateTime.Now - AlarmTime).TotalSeconds;
                    //红色报警时间超过900秒
                    if (second < 0)
                    {
                        alarmMes = ",已超过900秒, 立即打闸停机！";
                    }
                    else
                    {
                        alarmMes = ",还剩下" + (int)second + "秒";
                    }
                    isAlarming = true;
                }
                //如果上一次是安全，那么就把这次的报警保存数据库，并且记录当前ID
                else if(currentAlarm == AlarmType.AlarmSafe)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    AlarmTime = DateTime.Now;
                    SQLHelper.ExecuteSql("insert into AlarmFCZ (AlarmTime,AlarmText,AlarmState,AlarmDesc,UnitNO) values ('" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + AlarmMessage + "','正在报警','" + AlarmName + "',"+UnitNO+")");
                    isAlarming = false;
                }
                //如果上一次是黄色报警，则结束黄色报警，开始红色报警
                else if(currentAlarm == AlarmType.AlarmYellow)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    //结束黄色报警
                    SQLHelper.ExecuteSql("update AlarmFCZ set AlarmEndTime='"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',AlarmState='已结束'"+",AlarmLasts='"+PublicFunction.GetTimeSpanString(DateTime.Now - AlarmTime) + "' where AlarmTime='"+ AlarmTime.ToString("yyyy-MM-dd HH:mm:ss")+"'");
                    //开始红色报警
                    AlarmTime = DateTime.Now;
                    SQLHelper.ExecuteSql("insert into AlarmFCZ (AlarmTime,AlarmText,AlarmState,AlarmDesc,UnitNO) values ('" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + AlarmMessage + "','正在报警','" + AlarmName + "',"+UnitNO+")");
                    isAlarming = false;
                }
                //如果上一次是橙色，则结束橙色，开始红色
                else if(currentAlarm == AlarmType.AlarmOrange)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    //结束黄色报警
                    SQLHelper.ExecuteSql("update AlarmFCZ set AlarmEndTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',AlarmState='已结束'" + ",AlarmLasts='" + PublicFunction.GetTimeSpanString(DateTime.Now - AlarmTime) + "' where AlarmTime='" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    //开始红色报警
                    AlarmTime = DateTime.Now;
                    SQLHelper.ExecuteSql("insert into AlarmFCZ (AlarmTime,AlarmText,AlarmState,AlarmDesc,UnitNO) values ('" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + AlarmMessage + "','正在报警','" + AlarmName + "',"+UnitNO+")");
                    isAlarming = false;
                }
            }
            else if (type == AlarmType.AlarmWhite)
            {
                if (currentAlarm == AlarmType.AlarmSafe)
                {
                    alarmMes = "";
                    currentAlarm = type;
                    //开始报警
                    AlarmTime = DateTime.Now;
                    SQLHelper.ExecuteSql("insert into AlarmFCZ (AlarmTime,AlarmText,AlarmState,AlarmDesc,UnitNO) values ('" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + AlarmMessage + "','正在报警','" + AlarmName + "',"+UnitNO+")");
                    isAlarming = false;
                }
                else 
                {
                    if (currentFlag == flags)
                    {
                        //和上次同样的报警
                        if ((DateTime.Now - ConfirmTime).TotalMinutes < SetMinituesWhite)
                        {
                            return false;
                        }
                        else
                            isAlarming = true;
                    }
                    else 
                    {
                        //报警
                        alarmMes = "";
                        currentAlarm = type;
                        //结束上次报警
                        SQLHelper.ExecuteSql("update AlarmFCZ set AlarmEndTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',AlarmState='已结束'" + ",AlarmLasts='" + PublicFunction.GetTimeSpanString(DateTime.Now - AlarmTime) + "' where AlarmTime='" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                        //开始报警
                        AlarmTime = DateTime.Now;
                        SQLHelper.ExecuteSql("insert into AlarmFCZ (AlarmTime,AlarmText,AlarmState,AlarmDesc,UnitNO) values ('" + AlarmTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + AlarmMessage + "','正在报警','" + AlarmName + "',"+UnitNO+")");
                        isAlarming = false;
                    }
                    
                }
                currentFlag = flags;
            }
            return true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.promteText.ForeColor == Color.DarkBlue)
            {
                this.promteText.ForeColor = currentColor;
            }
            else 
            {
                this.promteText.ForeColor = Color.DarkBlue;
            }
        }

        //关闭按钮
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (currentAlarm == AlarmType.AlarmYellow)
                SetMinituesYellow = 15;
            else if (currentAlarm == AlarmType.AlarmOrange)
                SetMinituesOrange = 15;
            else if (currentAlarm == AlarmType.AlarmWhite)
                SetMinituesWhite = 15;

            this.timer1.Enabled = false;
            ConfirmTime = DateTime.Now;
            this.Visible = false;
        }

        //确认 
        private void button1_Click(object sender, EventArgs e)
        {
            if (currentAlarm == AlarmType.AlarmYellow)
                SetMinituesYellow = 1000000;
            else if (currentAlarm == AlarmType.AlarmOrange)
                SetMinituesOrange = 1000000;
            else if (currentAlarm == AlarmType.AlarmWhite)
                SetMinituesWhite = 1000000;

            this.timer1.Enabled = false;
            ConfirmTime = DateTime.Now;
            this.Visible = false;
        }
    }

    public enum AlarmType 
    {
        /// <summary>
        /// 一级 红色报警
        /// </summary>
        AlarmRed = 1,
        /// <summary>
        /// 二级 橙色报警
        /// </summary>
        AlarmOrange = 2,
        /// <summary>
        /// 三级 黄色报警
        /// </summary>
        AlarmYellow = 3,
        /// <summary>
        /// 没有报警
        /// </summary>
        AlarmSafe = 5,
        /// <summary>
        /// 常用报警（用于空冷岛阀门）
        /// </summary>
        AlarmWhite = 6
    }

}
