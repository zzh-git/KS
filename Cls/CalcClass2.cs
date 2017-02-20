using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WASPCN;
using System.Data;
using CJPrj.Cls;

namespace CJPrj
{
    public class CalcClass2
    {
        /// <param name="dblhnhw">热网回水温度</param>
        /// <param name="dblnqqout">凝汽器出水温度</param>
        /// <param name="dblnqqP">凝汽器压力</param>
        /// <returns>第二条棒图子线数值</returns>

        public static double calC2()
        {
            double douRet = 0;
            try {
                #region 按供热计算 计算过程
                //大气压力
                double dqy = DQY.PreChangeValue(4);
                if (Convert.ToDouble(dqy) == ConstYXYH.ReplaceValue)
                {
                        CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F3"] = ConstYXYH.ReplaceValue;
                        return ConstYXYH.ReplaceValue;
                }
                //标注：供热凝汽器进水温度(热网回水温度)
                double dblhnhw = DQY.PreChangeValue(150); ;//热网回水温度
                if (Convert.ToDouble(dblhnhw) == ConstYXYH.ReplaceValue)
                {
                    CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F3"] = ConstYXYH.ReplaceValue;
                    return ConstYXYH.ReplaceValue;
                }
                double dblnqqout =  DQY.PreChangeValue(151); ;//凝汽器出水温度
                if (Convert.ToDouble(dblnqqout) == ConstYXYH.ReplaceValue)
                {
                    CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F3"] = ConstYXYH.ReplaceValue;
                    return ConstYXYH.ReplaceValue;
                }

                double temp_dblnqqP = DQY.PreChangeValue(126); ;//凝汽器压力
                if (Convert.ToDouble(dblhnhw) == ConstYXYH.ReplaceValue)
                {
                    CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F3"] = ConstYXYH.ReplaceValue;
                    return ConstYXYH.ReplaceValue;
                }
                double dblnqqP = dqy + Math.Abs(temp_dblnqqP);//凝汽器(绝对）压力

                douRet = BarLength(dblnqqP);


                //保存数据
                CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F3"] = douRet.ToString();
                #endregion
            }
            catch(Exception e)
            {
                WriteLog.WriteLogs("按供热计算出错"+e.Message);
                WriteLog.WriteLogs("InnerException:" + e.InnerException.Message);
                WriteLog.WriteLogs("ErrorSource:" + e.Source.ToString());
                WriteLog.WriteLogs("ErrorSource:" + e.TargetSite.Attributes.ToString());
            }
            
            return douRet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="E11">凝汽器绝对压力</param>
        /// <returns>按供热计算的棒图数据</returns>
        private static double BarLength(double E11)
        {
            double douE31 = GetE31();
            if (douE31 < 0.02)
                douE31 = 0.02;
            else if (douE31 > 0.043)
                douE31 = 0.043;

            return 100 + (E11 - douE31) / 0.0002;
        }

        private static double GetE31()
        {
            double douE29 = GetE29();
            return (douE29*78.3913 + 9.3309)/1000;
        }

        private static double GetE29()
        {
            double douE22 = DQY.PreChangeValue(157); //低压缸排气量
            if (Convert.ToDouble(douE22) == ConstYXYH.ReplaceValue)
            {
                return ConstYXYH.ReplaceValue;
            }
            // KLDPFHeat.GetD170(); // GetE22();//低压缸排气量
            return 0.001159*douE22-0.1438;
        }
    }
}
