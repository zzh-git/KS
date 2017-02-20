using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonFunction;
using System.Data;
using YHYXYH.YXYH;

namespace YHYXYH.PI
{
    /// <summary>
    /// 读PI的测点数据
    /// </summary>
    class PIRead
    {
        static bool bIsOperating = false;// 是否正在操作PI，读或者写。
        static DateTime timeBeginOperating = DateTime.Now;//开始操作的时间
        static DateTime timeClosePI = DateTime.Now;//PI被关闭的时间
        public static string sPIServer = "10.72.155.162";//PI服务器
        public static string sPIWriteUser = "piadmin";//PI的回写用户
        public static string sPIWritePassword = "";//PI的回写密码
        umPISDK pisdk = new umPISDK();
        //umPISDK pisdkWrite = new umPISDK();
        ///// <summary>
        /////  读PI的测点数据
        ///// </summary>
        ///// <param name="dtTags">测点DataTable</param>
        ///// <param name="dtData">数据DataTable</param>
        //public void GetPIData(DataTable dtTags, DataTable dtData)
        //{
        //    pisdk.PIOpen();
        //    int i, iCount;
        //    double dSum, dTemp, dResult, dRate;
        //    string sTag;
        //    string[] sTags;
        //    CalType calType;
        //    DataRow rowNew = dtData.NewRow();
        //    rowNew["ValueTime"] = DateTime.Now;
        //    foreach (DataRow row in dtTags.Rows)
        //    {
        //        dResult = ConstYXYH.BadValue;
        //        //rowNew["F" + row["id"].ToString()] = ConstYXYH.BadValue;
        //        if (row["DataSourcesNo"].ToString() == "1" & row["tag"].ToString().Trim().Length > 2)
        //        {
        //            sTag = row["tag"].ToString().Trim().Replace("，", ",");
        //            sTags = sTag.Split(',');
        //            if (sTags.Length == 1)//单个测点取值
        //            {
        //                try
        //                {
        //                    dResult = pisdk.PIgetCurValue(row["tag"].ToString());
        //                    //rowNew["F" + row["id"].ToString()] = pisdk.PIgetCurValue(row["tag"].ToString());
        //                }
        //                catch (Exception ex) { YXYH.WriteLog.WriteLogs(ex.Message); }
        //            }
        //            else//以“,”号分割的多个测点取值
        //            {
        //                iCount = 0;
        //                dSum = 0;
        //                dTemp = 0;
        //                for (i = 0; i < sTags.Length; i++)
        //                {
        //                    try
        //                    {
        //                        dTemp = pisdk.PIgetCurValue(sTags[i]);
        //                        if (dTemp != ConstYXYH.BadValue)
        //                        {
        //                            dSum += dTemp;
        //                            iCount += 1;
        //                        }
        //                    }
        //                    catch (Exception ex) 
        //                    {
        //                        dTemp = ConstYXYH.BadValue;
        //                        YXYH.WriteLog.WriteLogs(ex.Message); 
        //                    }
        //                }
        //                #region 多测点的计算方式
        //                if (iCount > 0)
        //                {
        //                    dResult = dSum;
        //                    calType = CalType.Add;
        //                    try
        //                    {
        //                        calType = (CalType)(int.Parse(row["CalType"].ToString()));
        //                    }
        //                    catch { }
        //                    switch (calType)
        //                    {
        //                        //case CalType.Add:
        //                        //    ;
        //                        //    break;
        //                        case CalType.Avg:
        //                            dResult = dSum / iCount;
        //                            break;
        //                    }
        //                }
        //                #endregion
        //            }
        //            #region 单位换算的换算倍率（系数）计算
        //            if (dResult != ConstYXYH.BadValue)
        //            {
        //                dRate = 1;
        //                try { dRate = double.Parse(row["Rate"].ToString()); }
        //                catch { }
        //                dResult = Math.Round(dResult * dRate, 4);
        //            }
        //            #endregion
        //            rowNew["F" + row["id"].ToString()] = dResult;
        //        }
        //    }
        //    dtData.Rows.Add(rowNew);
        //}

        //  读PI的测点数据
        /// <summary>
        ///  读PI的测点数据
        /// </summary>
        /// <param name="dtTags">测点DataTable</param>
        /// <param name="dtData">数据DataTable</param>
        public void GetPIData(DataTable dtTags, DataTable dtData)
        {
            bIsOperating = true;

            int i, iCount;
            double dSum, dTemp, dResult, dRate;
            string sTag;
            string[] sTags;
            CalType calType;
            DataRow rowNew = dtData.NewRow();
            rowNew["ValueTime"] = DateTime.Now;
            List<string> lstTags = new List<string>();
            foreach (DataRow row in dtTags.Rows)
            {
                if (row["DataSourcesNo"].ToString() == "1" & row["tag"].ToString().Trim().Length > 2)
                    foreach (string tag in row["tag"].ToString().Trim().Replace("，", ",").Split(','))
                        lstTags.Add(tag);
            }
            timeBeginOperating = DateTime.Now;

            string sError = pisdk.PIOpen();
            if (sError.Length > 1)
                WriteLog.WriteLogs(sError);
            string[] sValues = pisdk.getPICurValues(lstTags);

            bIsOperating = false;
            WriteLog.WriteLogs((DateTime.Now - timeBeginOperating).TotalSeconds.ToString());

            if ((DateTime.Now - timeBeginOperating).TotalSeconds > 10 && (DateTime.Now - timeClosePI).TotalHours > 2)
            {
                pisdk.DisConnection(sPIServer, out sError);
                timeClosePI = DateTime.Now;
                WriteLog.WriteLogs("读PI后关闭了连接！" + sError);
            }
            int iIndex = -1;
            foreach (DataRow row in dtTags.Rows)
            {
                dResult = ConstYXYH.BadValue;
                if (row["DataSourcesNo"].ToString() == "1" & row["tag"].ToString().Trim().Length > 2)
                {
                    sTag = row["tag"].ToString().Trim().Replace("，", ",");
                    sTags = sTag.Split(',');
                    if (sTags.Length == 1)//单个测点取值
                    {
                        iIndex += 1;
                        try
                        {
                            dResult = double.Parse(sValues[iIndex]);
                        }
                        catch (Exception ex) { YXYH.WriteLog.WriteLogs(ex.Message); }
                    }
                    else//以“,”号分割的多个测点取值
                    {
                        iCount = 0;
                        dSum = 0;
                        dTemp = 0;
                        for (i = 0; i < sTags.Length; i++)
                        {
                            iIndex += 1;
                            try
                            {
                                dTemp = double.Parse(sValues[iIndex]);
                                if (dTemp != ConstYXYH.BadValue)
                                {
                                    dSum += dTemp;
                                    iCount += 1;
                                }
                            }
                            catch (Exception ex)
                            {
                                dTemp = ConstYXYH.BadValue;
                                YXYH.WriteLog.WriteLogs(ex.Message);
                            }
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
                    if (dResult != ConstYXYH.BadValue)
                    {
                        dRate = 1;
                        try { dRate = double.Parse(row["Rate"].ToString()); }
                        catch { }
                        dResult = Math.Round(dResult * dRate, 4);
                    }
                    #endregion
                    rowNew["F" + row["id"].ToString()] = dResult;
                }
            }
            dtData.Rows.Add(rowNew);
        }
        public bool UpdateValues(List<string> tagnames, List<string> tagvalues, DateTime datetimes, string piuser, string pwd,
                                 string piserver, out string error)
        {
            bIsOperating = true;
            timeBeginOperating = DateTime.Now;

            bool bReturn = pisdk.UpdateValues(tagnames, tagvalues, datetimes, piuser, pwd, piserver, out error);
            bIsOperating = false;
            WriteLog.WriteLogs("Write:" + (DateTime.Now - timeBeginOperating).TotalSeconds.ToString() + error);
            if ((DateTime.Now - timeBeginOperating).TotalSeconds > 10 && (DateTime.Now - timeClosePI).TotalHours > 2)
            {
                string sError;
                pisdk.DisConnection(piserver, out sError);
                timeClosePI = DateTime.Now;
                WriteLog.WriteLogs("写PI后关闭了连接！" + sError);
            }
            return bReturn;
        }
        // 获取是否正在操作PI的状态，读或者写。
        /// <summary>
        /// 获取是否正在操作PI的状态，读或者写。
        /// </summary>
        public static bool IsOperatingPI
        {
            get { return bIsOperating; }
        }
        // 获取开始操作PI的时间
        /// <summary>
        /// 获取开始操作PI的时间
        /// </summary>
        public static DateTime TimeOfBeginOperatingPI
        {
            get { return timeBeginOperating; }
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
