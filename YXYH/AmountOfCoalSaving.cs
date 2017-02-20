using HAOCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace YHYXYH.YXYH
{
    /// <summary>
    /// 节煤量计算的类
    /// </summary>
    public class AmountOfCoalSaving
    {
        static Timer m_Timer = new Timer(30000);//定时器，每30秒钟计算一次节能量。
        static double m_AmountOfCoalSaving = 0;//年度供热开始本机组累计节煤量，每个年度以6月31日作为分界线
        static double m_F196 = 0;//每30秒节煤量实时值

        static AmountOfCoalSaving()
        {
            m_Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }

        public static void Start()
        {
            //取数据库里保存的最近的节煤量的值，判断如果m_AmountOfCoalSaving小于0.01表示程序刚启动，还没取节煤量的值，则其值。
            if (m_AmountOfCoalSaving < 0.001)
            {
                string sAmountOfCoalSaving = SQLHelper.ExecuteScalar("select top 1 " + GlobalVariables.FieldNameOfCoalSaving + " from yxyhData order by ValueTime desc").ToString();
                double.TryParse(sAmountOfCoalSaving, out m_AmountOfCoalSaving);
            }
            m_Timer.Enabled = true;
        }

        public static void Stop()
        {
            m_Timer.Enabled = false;
        }

        static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_AmountOfCoalSaving = m_AmountOfCoalSaving + m_F196;
        }
        /// <summary>
        /// 每30秒节煤量实时值
        /// </summary>
        public static double F196
        {
            get
            {
                //访问时如果定时器未运行，则启动
                if (m_Timer.Enabled == false)
                    Start();

                return m_F196;
            }
            set
            {
                //访问时如果定时器未运行，则启动
                if (m_Timer.Enabled == false)
                    Start();

                m_F196 = value;
            }
        }

        /// <summary>
        /// 年度供热开始本机组累计节煤量
        /// </summary>
        public static double F195
        {
            get
            {
                //访问时如果定时器未运行，则启动
                if (m_Timer.Enabled == false)
                    Start();

                return m_AmountOfCoalSaving;
            }
        }
    }
}
