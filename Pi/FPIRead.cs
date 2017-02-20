using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonFunction;
using System.Data;
using CJPrj;
using CJPrj.Cls;

namespace CJPrj.PI
{
    /// <summary>
    /// 读PI的测点数据
    /// </summary>
    class FPIRead
    {
        umPISDK pisdk = new umPISDK();
        /// <summary>
        ///  读PI的测点数据
        /// </summary>
        /// <param name="dtTags">测点DataTable</param>
        /// <param name="dtData">数据DataTable</param>
        public void GetPIData(DataTable dtTags, DataTable dtData)
        {
            pisdk.PIOpen();
            int i, iCount;
            double dSum, dTemp, dResult, dRate;
            string sTag;
            string[] sTags;
            CalType calType;
            DataRow rowNew = dtData.NewRow();
            rowNew["ValueTime"] = DateTime.Now;
            #region
            foreach (DataRow row in dtTags.Rows)
            {
                dResult = ConstYXYH.ReplaceValue;
                if (row["DataSourcesNo"].ToString() == "1")
                {
                    if (row["tag"].ToString().Trim().Length <= 2)
                    {
                        dResult = -999999;
                    }else
                    {
                        sTag = row["tag"].ToString().Trim().Replace("，", ",");
                        if (sTag.Equals("YHTP:P216LBQ70CP102"))
                        {
                            int aa = 0;
                        }
                        sTags = sTag.Split(',');
                        if (sTags.Length == 1)//单个测点取值
                        {
                            try
                            {
                                dResult = pisdk.PIgetCurValue(row["tag"].ToString());
                                //rowNew["F" + row["id"].ToString()] = pisdk.PIgetCurValue(row["tag"].ToString());
                            }
                            catch (Exception ex) { WriteLog.WriteLogs(ex.Message); }
                        }
                        else//以“,”号分割的多个测点取值
                        {
                            iCount = 0;
                            dSum = 0;
                            dTemp = 0;
                            for (i = 0; i < sTags.Length; i++)
                            {
                                try
                                {
                                    dTemp = pisdk.PIgetCurValue(sTags[i]);
                                    if (dTemp != -999999)
                                    {
                                        dSum += dTemp;
                                        iCount += 1;
                                    }
                                }
                                catch (Exception ex) { WriteLog.WriteLogs(ex.Message); }
                            }
                            #region 多测点的计算方式
                            if (iCount > 0)
                            {
                                dResult = dSum;
                                calType = CalType.Add;
                                try
                                {
                                    calType = (CalType)(int.Parse(row["CalType"].ToString()));
                                }
                                catch { }
                                switch (calType)
                                {
                                    //case CalType.Add:
                                    //    ;
                                    //    break;
                                    case CalType.Avg:
                                        dResult = dSum / iCount;
                                        break;
                                }
                            }
                            #endregion
                        }
                        #region 单位换算的换算倍率（系数）计算
                        if (dResult != ConstYXYH.ReplaceValue)
                        {
                            dRate = 1;
                            try { dRate = double.Parse(row["Rate"].ToString()); }
                            catch { }
                            dResult = Math.Round(dResult * dRate, 4);
                        }
                        #endregion
                    }
                    rowNew["F" + row["id"].ToString()] = dResult;
                }
            }
            #endregion
            dtData.Rows.Add(rowNew);
        }
    }

    // 计算类型
    /// <summary>
    /// 计算类型
    /// </summary>
    public enum CalType
    {
        // 相加
        /// <summary>
        /// 相加
        /// </summary>
        Add = 1,
        // 取平均
        /// <summary>
        /// 取平均
        /// </summary>
        Avg = 2
    }
}
