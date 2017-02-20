using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CJPrj.Cls
{
    /// <summary>
    /// 常量静态类
    /// </summary>
    class ConstYXYH
    {
        // 构造函数e
        /// <summary>
        /// 构造函数
        /// </summary>
        static ConstYXYH()
        {
            try
            {
                CurrentTable = SQLHelper.ExecuteScalar("SELECT ConfigValue FROM FCZConfig where configkey='CurrentTable'").ToString();
                YearOfDataSaved = byte.Parse(SQLHelper.ExecuteScalar("SELECT ConfigValue FROM FCZConfig where configkey='YearOfDataSaved'").ToString());
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
        }
        // 取数时出现坏点用于标示的数据
        public static double ReplaceValue = -999999;
        //取数时用于平均的次数
        public static int AvgNum = 10;

        /// <summary>
        /// 主页面刷新频率
        /// </summary>
        public static int RefIntvel = 5 * 1000;

        public static int CalIntvel = 10 * 1000;
        //数据库中，当前正在往里写数据的表名称
        /// <summary>
        /// 数据库中，当前正在往里写数据的表名称
        /// </summary>
        public static string CurrentTable = "FCZData";
        // 创建数据库表时用的表名的前缀
        /// <summary>
        /// 创建数据库表时用的表名的前缀
        /// </summary>
        public const string TABLE_NAME_PREFIX = "FCZData";
        //计算的数据在数据库中保存的年份
        /// <summary>
        /// 计算的数据在数据库中保存的年份
        /// </summary>
        public static byte YearOfDataSaved = 2;
        // 曲线显示数据的分钟数（曲线总共显示多少分钟的数据）
        /// <summary>
        /// 曲线显示数据的分钟数（曲线总共显示多少分钟的数据）
        /// </summary>
        public static byte HourOfLinesShowing = 6;

        //系统所用测点个数
        public static int TagsNum = 68;
    }
}
