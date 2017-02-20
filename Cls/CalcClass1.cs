using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CJPrj.Cls;

namespace CJPrj
{
    //按压力计算
    public class CalcClass1
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MidInP">低压缸进汽压力</param>
        /// <param name="LowOutP">低压缸排汽压力</param>
        /// <returns>返回值为kPa</returns> 
        public static double calC1()
        {
            double decRet = 0;
            try
            {
                #region 计算过程
                //低压缸进汽压力 表压
                double dou_jqyl = Convert.ToDouble(CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F111"].ToString());
                double dqy = Convert.ToDouble(CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F4"].ToString());
                if (Convert.ToDouble(dou_jqyl) == ConstYXYH.ReplaceValue)
                {
                    DataRow[] dtrarr = CJPrj.Cls.CalTree.dtTagsSet.Select("ExcelCell='F111'");
                    if (string.IsNullOrEmpty(dtrarr[0]["setvalue"].ToString()))
                    {
                        CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F2"] = ConstYXYH.ReplaceValue;
                        return ConstYXYH.ReplaceValue;
                    }
                    dou_jqyl= Convert.ToDouble(dtrarr[0]["setvalue"]);
                }
                if (Convert.ToDouble(dqy) == ConstYXYH.ReplaceValue)
                {
                    DataRow[] dtrarr = CJPrj.Cls.CalTree.dtTagsSet.Select("ExcelCell='F4'");
                    if (string.IsNullOrEmpty(dtrarr[0]["setvalue"].ToString()))
                    {
                        CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F2"] = ConstYXYH.ReplaceValue;
                        return ConstYXYH.ReplaceValue;
                    }
                    dqy = Convert.ToDouble(dtrarr[0]["setvalue"]);
                }
                dou_jqyl = dou_jqyl + dqy;//低压缸进汽绝对压力

                double dou_pqyl =Math.Round( Convert.ToDouble(CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F126"].ToString()) ,5);//低压缸排汽压力f126*100     59  

                //string tempStr1 = CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F126"].ToString();
                //string tempStr2 = CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F4"].ToString();

                if (Convert.ToDouble(dou_pqyl) == ConstYXYH.ReplaceValue)
                {
                    DataRow[] dtrarr = CJPrj.Cls.CalTree.dtTagsSet.Select("ExcelCell='F126'");
                    if (string.IsNullOrEmpty(dtrarr[0]["setvalue"].ToString()))
                    {
                        return 7;
                    }
                    dou_pqyl = Convert.ToDouble(dtrarr[0]["setvalue"]);
                }


                double K = 0;
                if (dou_jqyl < 0.1361)//低压缸进气压力低于0.17Mpa时
                {
                    K = 20;//低压缸排汽压力限制值为20KP
                    decRet = 100 + (dou_pqyl - K) / 0.2;
                }
                else if (dou_jqyl > 0.43)//低压缸进气压力高于0.43Mpa时
                {
                    K = 58;//低压缸排汽压力限制值为58KP
                    decRet = 100 + (dou_pqyl - K) / 0.2;
                }
                else//低压缸进气压力在0.17和0.43Mpa之间时
                {
                    //限制值为：（146.154*低压缸进汽压力的绝对值-4.846）
                    K = dou_jqyl * 146.154-4.846;
                    decRet = 100 + (dou_pqyl - K) / 0.2;
                }
                //保存数据
                CJPrj.Cls.CalTree.CalDataTable.Rows[0]["F2"] = decRet.ToString();
                #endregion
            }
            catch (Exception e)
            {
                WriteLog.WriteLogs("按压力计算出错:"+e.Message);
                WriteLog.WriteLogs("InnerException:" + e.InnerException.Message);
                WriteLog.WriteLogs("ErrorSource:" + e.Source.ToString());
                WriteLog.WriteLogs("ErrorSource:" + e.TargetSite.Attributes.ToString());
            }
            finally {
            
            }
            return decRet;
        }
    }
}
