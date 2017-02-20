using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YHYXYH.YXYH
{
    /// <summary>
    /// 常量静态类
    /// </summary>
    public class ConstYXYH
    {
        //年度供热开始本机组累计节煤量在数据库中保存的字段的名称，用于程序重新启动时取上次的节煤量。
        /// <summary>
        /// 年度供热开始本机组累计节煤量在数据库中保存的字段的名称，用于程序重新启动时取上次的节煤量。
        /// </summary>
        public static string FieldNameOfCoalSaving = "F195";
        //取数、计算 刷新频率 单位：ms
        /// <summary>
        /// 取数、计算、保存数据 刷新频率 单位：ms
        /// </summary>
        public static int RefIntvel = 5 * 1000;
        //计算数据频率 单位：ms
        /// <summary>
        /// 计算数据频率 单位：ms
        /// </summary>
        public static int CalIntvel = 10 * 1000;
        // 写计数，表示RefIntvel运行（计算）多少次往数据库里写一次数
        /// <summary>
        /// 写计数，表示RefIntvel运行（计算）多少次往数据库里写一次数
        /// </summary>
        public static int WriteTimer = 2;
        //数据库中，当前正在往里写数据的表名称
        /// <summary>
        /// 数据库中，当前正在往里写数据的表名称
        /// </summary>
        public static string CurrentTable = "YXYHData";
        // 创建数据库表时用的表名的前缀
        /// <summary>
        /// 创建数据库表时用的表名的前缀
        /// </summary>
        public const string TABLE_NAME_PREFIX = "YXYHData";
        //数据库中，当前正在往里写数据的表名称
        /// <summary>
        /// 数据库中，当前正在往里写数据的表名称
        /// </summary>
        public static string CurrentYXYHTotalDataTable = "YXYHTotalData2014";
        // 创建数据库汇总表时用的表名的前缀
        /// <summary>
        /// 创建数据库汇总表时用的表名的前缀
        /// </summary>
        public const string TOTAL_TABLE_NAME_PREFIX = "TotalData";
        //计算的数据在数据库中保存的年份
        /// <summary>
        /// 计算的数据在数据库中保存的年份
        /// </summary>
        public static byte YearOfDataSaved = 3;
        // 曲线显示数据的分钟数（曲线总共显示多少分钟的数据）
        /// <summary>
        /// 曲线显示数据的分钟数（曲线总共显示多少分钟的数据）
        /// </summary>
        public static byte HourOfLinesShowing = 6;
        // 累计显示数据的小时数
        /// <summary>
        /// 累计显示数据的小时数
        /// </summary>
        public static byte HourOfTotalShowing = 1;
        // 取平均的实时数的个数
        /// <summary>
        /// 取平均的实时数的个数
        /// </summary>
        public static byte AvgCount = 10;
        // 用-999999表示测点坏值
        /// <summary>
        /// 用-999999表示测点坏值
        /// </summary>
        public static double BadValue = -999999d;
        // 累计时间长度，即多长时间累计一次，单位：分钟
        /// <summary>
        /// 累计时间长度，即多长时间累计一次，单位：分钟
        /// </summary>
        public static int TotalTimeLong = 5;
        // 是否可以移动标签的位置
        /// <summary>
        /// 是否可以移动标签的位置
        /// </summary>
        public static bool IsCanMoveLabel = true;
        // 是否可以添加标签
        /// <summary>
        /// 是否可以添加标签
        /// </summary>
        public static bool IsCanAddLabel = true;

        // 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        static ConstYXYH()
        {
            try
            {
                //CurrentTable = SQLHelper.ExecuteScalar("SELECT ConfigValue FROM YXYHConfig where configkey='CurrentTable'").ToString();
                //YearOfDataSaved =byte.Parse( SQLHelper.ExecuteScalar("SELECT ConfigValue FROM YXYHConfig where configkey='YearOfDataSaved'").ToString());
                IsCanMoveLabel = bool.Parse(SQLHelper.ExecuteScalar("SELECT ConfigValue FROM YXYHConfig where configkey='IsCanMoveLabel'").ToString());
                IsCanAddLabel = bool.Parse(SQLHelper.ExecuteScalar("SELECT ConfigValue FROM YXYHConfig where configkey='IsCanAddLabel'").ToString());
                CurrentYXYHTotalDataTable = SQLHelper.ExecuteScalar("SELECT ConfigValue FROM YXYHConfig where configkey='CurrentTotalTable'").ToString();
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
        }
    }
}
