using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection; 
using System.Timers;
using System.Collections; 
using System.Diagnostics;
using System.ServiceProcess;
using HAOCommon;
using HAOCache;
using HAOCommon.PI;
using WinHost;

namespace YHYXYH.YXYH
{
    // 计算测点值的类
    /// <summary>
    /// 计算测点值的类
    /// </summary>
    public class CalTagValue
    {
        static bool m_IsDeleted = false;//是否已执行过数据删除操作。
        static bool m_IsCalculating = false;//表示是否正在执行计算。
        static bool m_IsFirstRun = true;//是不是第一次运行。为防止第一次启动较慢或者起不来，第一次运行不取PI的值，使用默认数据。
        static int Range;//计算函数所用的参数，只是为了参数传递，无实际用处
        static int m_iTopValue = 60 * GlobalVariables.HourOfLinesShowing / (GlobalVariables.RefIntvel / 1000);//曲线显示的点数
        static int m_iTotalRowCount = GlobalVariables.HourOfTotalShowing * 60 / GlobalVariables.TotalTimeLong;//累计显示的点数
        static long m_LoopCount = 0;//记录定时器运行次数
        static long m_TotalCount = 0;//记录累计汇总次数
        static string m_sWillTotalTagFields = "";
        static Timer m_TimerStart = new Timer();//用于延时5秒启动计算定时器的定时器。使防颤振与运行优化从PI里取数据的开始时间相差5秒，不同时取数据
        static Timer m_Timer = new Timer();//用于循环计算测点值的定时器
        static Timer m_TimerRestartSqlServer = new Timer(5 * 60 * 1000);//判断并重启SqlServer服务的定时器 
        static DateTime m_ValueTime = DateTime.Now;//记录计算数据的时间
        static DateTime m_TotalTimeBegin = DateTime.Now;//记录统计的开始时间
        static DateTime m_TotalTimeEnd = DateTime.Now;//记录统计的结束时间
        //static DateTime m_TotalTime = DateTime.Now;//保存统计完之后的时间
        static DateTime m_TotalTimeHour = DateTime.Now;//每小时统计里，记录统计完之后的时间
        static DateTime timeGCCollect = DateTime.Now;//系统垃圾回收时间
        static DataRow m_rowTotalDataLast = null;//保存统计数据中上一次生成的数据
        static DataRow m_rowTotalDataCurrent = null;//保存统计数据中当前生成的数据
        static DataRow m_rowDataLast = null;//保存上一次生成的数据
        static DataRow m_rowDataCurrent = null;//保存当前生成的数据
        static DataView m_viewTags = null;//保存所有测点基本信息的视图
        static DataView m_viewTotalTags = TableTotalTags.getAllData().DefaultView;//保存所有统计测点基本信息的视图
        static DataView m_viewTagsCalculated = null;//保存所有测点基本信息及其计算结果的视图
        static DataTable m_dtTagDatas = SQLHelper.ExecuteDt("select * from " + GlobalVariables.CurrentTable + " where 1=0");//保存测点数据的表
        static DataTable m_dtTotalTagDatas = SQLHelper.ExecuteDt("select * from " + GlobalVariables.CurrentTotalDataTable + " where 1=0");//保存统计测点数据的表
        static DataTable m_dtTagCurDatas = m_dtTagDatas.Clone();//保存从PI或OPC里取到的当前值，保存20行用于取平均
        static DataTable m_dtWillTotalTagDatas = new DataTable();//保存要每小时进行累计的数据
        static PIRead pi = new PIRead();//从PI里读数据的类
        //static string sPIServer = "10.72.155.162";//PI服务器
        //static string sPIWriteUser = "piadmin";//PI的回写用户
        ////static string sPIWriteUser = "pidemo";//PI的回写用户
        //static string sPIWritePassword = "";//PI的回写密码
        static List<string> lstPINames = new List<string>();//保存回写的PI测点名称
        static List<string> lstPIValues = new List<string>();//保存回写的PI测点值
        static RunOptimizePrompt m_frmRunOptimizePrompt = new RunOptimizePrompt();

        // 构造函数，设置定时器事件委托与处理函数相关联；向数据表里加载数据
        /// <summary>
        /// 构造函数，设置定时器事件委托与处理函数相关联；向数据表里加载数据
        /// </summary>
        static CalTagValue()
        {
            m_Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            m_TimerStart.Elapsed += new ElapsedEventHandler(TimerStart_Elapsed);
            m_TimerRestartSqlServer.Elapsed += new ElapsedEventHandler(TimerRestartSqlServer_Elapsed);
            m_TimerRestartSqlServer.Start();

            DataRow rowNew = null;
            DataTable dt = SQLHelper.ExecuteDt("select top " + m_iTopValue + " * from " + GlobalVariables.CurrentTable + " order by valuetime desc");
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                rowNew = dt.Rows[i];
                m_dtTagDatas.Rows.Add(rowNew.ItemArray.ToArray());
            }
            if (m_dtTagDatas.Rows.Count > 0)
                m_rowDataLast = m_dtTagDatas.Rows[m_dtTagDatas.Rows.Count - 1];
            else
                WriteLog.WriteLogs("启动时获取上次停止时的值失败！m_rowDataLast");
            dt = SQLHelper.ExecuteDt("select top " + m_iTotalRowCount + " * from " + GlobalVariables.CurrentTotalDataTable + " order by valuetime desc");
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                rowNew = dt.Rows[i];
                m_dtTotalTagDatas.Rows.Add(rowNew.ItemArray.ToArray());
            }

            #region 每小时汇总的初始化计算
            //if (m_dtTotalTagDatas.Rows.Count > 0)
            //{
            //    m_rowTotalDataLast = m_dtTotalTagDatas.Rows[0];
            //    string sDateHour = m_rowTotalDataLast["DateHour"].ToString();
            //    int iYear = int.Parse(sDateHour.Substring(0, 4));
            //    int iMonth = int.Parse(sDateHour.Substring(4, 2));
            //    int iDay = int.Parse(sDateHour.Substring(6, 2));
            //    int iHour = int.Parse(sDateHour.Substring(8, 2));
            //    m_TotalTimeHour = new DateTime(iYear, iMonth, iDay, iHour + 1, 9, 0);
            //}
            #endregion

            TotalInit();
        }
        // 启动定时器计算
        /// <summary>
        /// 启动定时器计算
        /// </summary>
        public static void Start()
        {
            m_Timer.Interval = GlobalVariables.TimerCalculator;
            m_TimerStart.Interval = 100;

            WriteLog.WriteLogs("程序启动！");
            //
            if (m_dtTagDatas.Rows.Count > 0)
            {
                TimeSpan ts = DateTime.Now - DateTime.Parse(m_dtTagDatas.Rows[m_dtTagDatas.Rows.Count - 1]["ValueTime"].ToString());
                if (ts.TotalMinutes > GlobalVariables.HourOfLinesShowing)
                    ; //m_dtTagDatas.Rows.Clear();
            }
            //System.Threading.Thread.Sleep(5000);
            m_TimerStart.Enabled = true;
        }
        // 关闭定时器计算
        /// <summary>
        /// 关闭定时器计算
        /// </summary>
        public static void Stop()
        {
            m_Timer.Enabled = false;
        }
        /// <summary>
        /// 重启SqlServer服务的定时器方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void TimerRestartSqlServer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool bIsRestart = false;
            long lSql = 0;//保存sqlservr内存
            long lProj = System.Diagnostics.Process.GetCurrentProcess().PagedMemorySize64;//保存程序内存
            try
            {
                //保存sqlservr内存。
                System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("sqlservr");
                foreach (Process p in processes)
                {
                    if (p.PagedMemorySize64 > lSql)
                        lSql = p.PagedMemorySize64;
                }
                //判断数据库大于1G，或者数据库和程序内存之和大于3G并且数据库大于500M时，重启数据库
                if (lSql > 1000 * 1000 * 1000 || ((lSql + lProj) > 3000L * 1024 * 1024 & lSql > 500L * 1024 * 1024))
                    bIsRestart = true;
            }
            catch { }
            try
            {
                //bIsRestart为True时，重启SqlServer服务。
                if (bIsRestart == false)
                    return;
                ServiceController service = new ServiceController("MSSQLSERVER");
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    if (service.Status == ServiceControllerStatus.Stopped)
                        service.Start();
                    WriteLog.WriteLogs("重启了SqlServer服务！程序内存："+lProj+"；数据库内存："+lSql);
                    GC.Collect();
                }

            }
            catch (Exception ex)
            {
                WriteLog.WriteLogs(ex.ToString());
            }
        }
        static void TimerStart_Elapsed(object sender, ElapsedEventArgs e)
        {
            if ((DateTime.Now - CalTree.TimeOfTimerStart).TotalSeconds > 5)
            {
                m_Timer.Enabled = true;
                m_TimerStart.Enabled = false;
            }
        }
        // 定时器处理函数
        /// <summary>
        /// 定时器处理函数
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">包含事件数据的 System.Timers.ElapsedEventArgs 对象。</param>
        static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (PIRead.IsOperatingPI == true)
                    return;
                if (m_IsCalculating == false)
                {
                    DateTime d = DateTime.Now;
                    m_IsCalculating = true;
                    CalAllTagValue();

                    m_LoopCount += 1;
                    m_IsCalculating = false;
                    //System.Windows.Forms.MessageBox.Show((d - DateTime.Now).ToString());
                }
                //if (m_LoopCount % 2 == 0)
                //    YYH.ShowDialog();
                //else
                //    YYH.Hide();

                System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
                if (p.PagedMemorySize64 > 1000 * 1000 * 1000 & m_LoopCount % 78 == 0)
                {
                    WriteLog.WriteLogs("运行优化计算内存大于1G：" + p.PagedMemorySize64 + " , 程序已运行：" + (DateTime.Now - CalTree.TimeOfTimerStart).TotalHours);
                    System.GC.Collect();
                }
            }
            catch (Exception ex)
            {
                m_IsCalculating = false;
                WriteLog.WriteLogs(ex.ToString());
            }
        }
        // 获取20行实时数据的平均值
        /// <summary>
        /// 获取20行实时数据的平均值
        /// </summary>
        /// <param name="sExcelCell">Excel单元格</param>
        /// <returns></returns>
        static double GetAvgValue(string sExcelCell)
        {
            int iValueCount = 0;
            double dReturn = 0;
            double dTemp = 0;
            foreach (DataRow row in m_dtTagCurDatas.Rows)
            {
                if (double.TryParse(row[sExcelCell].ToString(), out dTemp))
                {
                    if (dTemp != GlobalVariables.BadValue)
                    {
                        iValueCount += 1;
                        dReturn += dTemp;
                    }
                }
            }
            if (iValueCount == 0)
                dReturn = GlobalVariables.BadValue;
            else
                dReturn = dReturn / iValueCount;
            return dReturn;
        }
        // 设置m_viewTags对象
        /// <summary>
        /// 设置m_viewTags对象，设置m_sWillTotalTagFields的值
        /// </summary>
        static void SetTagsView()
        {
            if (m_viewTags == null || TableTags.isFilled)
            {
                m_viewTags = TableTags.getAllData().DefaultView;

                m_viewTags.RowFilter = "isTotalize=1";
                m_sWillTotalTagFields = "ValueTime,";
                foreach (DataRowView row in m_viewTags)
                {
                    if (m_dtWillTotalTagDatas.Columns[row["ExcelCell"].ToString()] == null)
                        m_dtWillTotalTagDatas.Columns.Add(row["ExcelCell"].ToString());
                    m_sWillTotalTagFields += row["ExcelCell"].ToString() + ",";
                }
                m_sWillTotalTagFields = m_sWillTotalTagFields.Trim(',');
                m_viewTags.RowFilter = "";

            }
        }
        // 初始化数据
        /// <summary>
        /// 初始化数据
        /// </summary>
        static bool Init()
        {
            bool bReturn = true;
            SetTagsView();
            DataColumn col = m_viewTags.Table.Columns["TagValue"];
            if (col != null)
            {
                m_viewTags.Table.Columns.Remove(col);
                col.Dispose();
            }
            col = new DataColumn("TagValue", typeof(double));
            col.DefaultValue = DBNull.Value;
            m_viewTags.Table.Columns.Add(col);

            //取定值赋值
            m_viewTags.RowFilter = "DataSourcesNo=3";
            foreach (DataRowView row in m_viewTags)
            {
                row.BeginEdit();
                row["TagValue"] = row["SetValue"];
                row.EndEdit();
            }

            m_ValueTime = DateTime.Now;

            //测量值赋值
            m_viewTags.RowFilter = "DataSourcesNo=1";
            if (PIRead.IsOperatingPI == true)
            {
                WriteLog.WriteLogs("YXYH Pass");
                return false;
            }
            if (m_IsFirstRun == false)
                pi.GetPIData(m_viewTags.Table, m_dtTagCurDatas);
            if (m_dtTagCurDatas.Rows.Count > GlobalVariables.AvgNum)
                m_dtTagCurDatas.Rows.RemoveAt(0);
            foreach (DataRowView row in m_viewTags)
            {
                row.BeginEdit();
                if (m_IsFirstRun == false)
                    row["TagValue"] = GetAvgValue(row["ExcelCell"].ToString());
                else
                    row["TagValue"] = row["SetValue"];
                row.EndEdit();
            }

            //对测量值生成随机数
            //Rnd();

            return bReturn;
        }

        static double dValue = 0;
        static double dMin = 0;
        static double dMax = 0;
        static System.Random rnd = new Random();
        /// <summary>
        /// 对测量值生成随机数
        /// </summary>
        static void Rnd()
        {
            //测量值生成随机数
            m_viewTags.RowFilter = "DataSourcesNo=1";
            foreach (DataRowView row in m_viewTags)
            {
                try
                {
                    dValue = double.Parse(row["TagValue"].ToString());
                    dMin = dValue - dValue * 0.01;
                    dMax = dValue + dValue * 0.01;
                    row.BeginEdit();
                    row["TagValue"] = dMin + (dMax - dMin) * rnd.NextDouble();
                    row.EndEdit();
                }
                catch { }
            }
        }
        /// <summary>
        /// 对测量值生成随机数
        /// </summary>
        public static double Rnd(double dMin, double dMax)
        {
            return dMin + (dMax - dMin) * rnd.NextDouble();
        }
        // 检查是否要初始化
        /// <summary>
        /// 检查是否要初始化
        /// </summary>
        static void CheckInit()
        {
            if (m_viewTags == null)
                Init();
        }
        // 计算所有测点的值
        /// <summary>
        /// 计算所有测点的值
        /// </summary>
        public static void CalAllTagValue()
        {
            //DateTime timeBegin = DateTime.Now;
            //WriteLog.WriteLogs("cal Begin:"+ DateTime.Now.ToString());
            //在数据库中创建表，每一周的数据保存在一个表中
            string tableName = GetTableName();
            if (tableName != GlobalVariables.CurrentTable)
                CreateTable(tableName);
            TableTotalTags.CreateTable();

            if (Init() == false)
                return;

            m_viewTags.RowFilter = "id=59";//3抽进水温度
            if (m_viewTags.Count > 0)
                m_viewTags[0]["TagValue"] = ((double)m_viewTags[0]["TagValue"]) + 1;//+1.5

            //调用本类中每一个计算数据的函数，如CalF7()、CalF10()等等，最后得到m_viewTags中“TagValue”列里计算好的结果
            m_viewTags.RowFilter = "DataSourcesNo=2";
            MethodInfo mi = null;
            System.Type type = typeof(CalTagValue);
            foreach (DataRowView row in m_viewTags)
            {
                if (row["TagValue"] == DBNull.Value)
                {
                    mi = type.GetMethod("Cal" + row["ExcelCell"], BindingFlags.NonPublic | BindingFlags.Static);
                    if (mi != null)
                        row["TagValue"] = (double)mi.Invoke(null, null);
                }
            }

            //把计算好的数据专门用m_viewTagsCalculated保存以向外提供数据；可防止在计算数据的过程中，外部类在访问数据时访问不到
            if (m_viewTagsCalculated == null)
            {
                m_viewTags.RowFilter = "";
                m_viewTagsCalculated = m_viewTags.ToTable().DefaultView;
            }

            //把生成的数据插入到数据库，并把数据加到数据表m_dtTagDatas里。。。
            double dTemp;
            StringBuilder sbFields = new StringBuilder("ValueTime");
            StringBuilder sbValue = new StringBuilder("'" + m_ValueTime.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            m_viewTags.RowFilter = "";
            m_viewTagsCalculated = m_viewTags.ToTable().DefaultView;
            DataRow rowNewData = m_dtTagDatas.NewRow();
            DataRow rowWillTotalData = m_dtWillTotalTagDatas.NewRow();
            rowNewData["ValueTime"] = m_ValueTime;
            rowWillTotalData["ValueTime"] = m_ValueTime;
            lstPINames.Clear();
            lstPIValues.Clear();
            lock (m_viewTagsCalculated)
            {
                int i = 0;
                int iID = 0;
                m_viewTagsCalculated.RowFilter = "";
                //m_viewTagsCalculated.Sort = "id";
                foreach (DataRowView row in m_viewTags)
                {
                    try
                    {
                        if (row["TagValue"].ToString() != "非数字" & row["TagValue"].ToString() != "正无穷大" & row["TagValue"].ToString() != "负无穷大")
                        {
                            dTemp = double.Parse(row["TagValue"].ToString());
                            iID = int.Parse(row["id"].ToString());
                            //几个效率值变换成百分数
                            if (iID == 77 || iID == 86 || iID == 87 || iID == 164 || iID == 167 || iID == 178)// || iID == 185
                                dTemp = dTemp * 100;
                            dTemp = Math.Round(dTemp, 4);
                        }
                        else
                            dTemp = GlobalVariables.BadValue;
                    }
                    catch
                    { dTemp = GlobalVariables.BadValue; }
                    try
                    {
                        if (dTemp != GlobalVariables.BadValue)
                            rowNewData[row["ExcelCell"].ToString()] = dTemp;
                    }
                    catch (Exception ex)
                    {
                        //m_dtTagDatas = SQLHelper.ExecuteDt("select * from " + GlobalVariables.CurrentTable + " where 1=0");//保存测点数据的表
                        //return;
                        WriteLog.WriteLogs(ex.ToString());
                    }
                    m_viewTagsCalculated[i]["TagValue"] = dTemp;
                    if (row["isTotalize"].ToString() == "1")
                        rowWillTotalData[row["ExcelCell"].ToString()] = dTemp;
                    i += 1;

                    ////需要往数据库里写时，生成要写的字段和数据
                    //if (m_LoopCount % GlobalVariables.WriteTimer == 0)
                    //{
                    //if (row["TagValue"] != DBNull.Value)
                    //{
                    if (dTemp != GlobalVariables.BadValue)
                    {
                        sbFields.Append("," + row["ExcelCell"]);
                        sbValue.Append("," + dTemp);
                        //添加往PI里回写数据的值
                        if (row["WriteBackTag"].ToString().Length > 2)
                        {
                            lstPINames.Add(row["WriteBackTag"].ToString());
                            lstPIValues.Add(dTemp.ToString());
                        }
                    }
                    //}
                }
                if (m_IsFirstRun == false)
                    m_dtWillTotalTagDatas.Rows.Add(rowWillTotalData);
            }
            lock (m_dtTagDatas)
            {
                if (m_IsFirstRun == false)
                {
                    m_dtTagDatas.Rows.Add(rowNewData);
                    m_rowDataLast = rowNewData;
                    if (m_dtTagDatas.Rows.Count > m_iTopValue)
                        m_dtTagDatas.Rows.RemoveAt(0);
                }
            }
            ////需要往数据库里写时，插入数据
            //if (m_LoopCount % GlobalVariables.WriteTimer == 0)
            //{
            if (m_IsFirstRun == false)
            {
                SQLHelper.ExecuteSql("insert into " + GlobalVariables.CurrentTable + "(" + sbFields.ToString() + ") values(" +
                    sbValue.ToString() + ")");
            }
            //}

            #region 此方法数据加不过去
            ////把计算好的数据专门用m_viewTagsCalculated保存以向外提供数据；可防止在计算数据的过程中，外部类在访问数据时访问不到
            //m_viewTags.RowFilter = "";
            //if (m_viewTagsCalculated == null)
            //    m_viewTagsCalculated = m_viewTags.ToTable().DefaultView;
            //else
            //{
            //    DataColumn colNew = m_viewTags.Table.Columns["TagValue"];
            //    m_viewTags.Table.Columns.Remove(colNew);
            //    DataColumn colOld = m_viewTagsCalculated.Table.Columns["TagValue"];
            //    //锁定以更改数据
            //    lock (m_viewTagsCalculated)
            //    {
            //        m_viewTagsCalculated.Table.Columns.Remove(colOld);
            //        m_viewTagsCalculated.Table.Columns.Add(colNew);
            //    }
            //    colOld.Dispose();
            //}
            #endregion

            if (m_IsFirstRun == false)
                RunOptimizePrompt.CalculateRunOptimizePromptType();

            TotalizeAllData();

            AddOtherPIWriteValues();
            if (lstPINames.Count > 0)
                try
                {
                    if (m_IsFirstRun == false)
                    {
                        //if (PIRead.IsOperatingPI == true)
                        //    WriteLog.WriteLogs("要写数据，但是正在操作PI！");
                        //string sError = "";
                        //pi.UpdateValues(lstPINames, lstPIValues, m_ValueTime, PIRead.sPIWriteUser, PIRead.sPIWritePassword
                        //    , PIRead.sPIServer, out sError);
                        //if (sError.Length > 0)
                        //    WriteLog.WriteLogs(sError);
                    }
                }
                catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }

            m_IsFirstRun = false;

            DeleteOldData();

            if ((DateTime.Now - timeGCCollect).TotalHours > 1.7)
            {
                timeGCCollect = DateTime.Now;
                GC.Collect();
            }
            //WriteLog.WriteLogs("cal time:" + (DateTime.Now-timeBegin).TotalSeconds);
        }
        // 添加其他的PI回写值
        /// <summary>
        /// 添加其他的PI回写值
        /// </summary>
        static void AddOtherPIWriteValues()
        {
            //运行优化提示
            lstPINames.Add("YHTP:02_FZC_053");
            lstPIValues.Add(RunOptimizePrompt.GetPromptNo().ToString());
            //棒图长度
            try
            {
                lstPIValues.Add(Math.Round(Convert.ToDouble(CalTree.CalDataTable.Rows[0]["F2"]), 4).ToString());
                lstPINames.Add("YHTP:02_FZC_052");
            }
            catch { }
        }
        static bool bIsShow = false;
        // 获取所有测点数据，返回新创建的DataTable
        /// <summary>
        /// 获取所有测点数据，返回新创建的DataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllTagValues()
        {
            ////如果定时器未运行，则启动
            //if (m_Timer.Enabled == false)
            //    Start();

            //如果重未计算过数据，则计算
            if (m_viewTagsCalculated == null)
                CalAllTagValue();

            //锁定以返回数据
            lock (m_viewTagsCalculated)
            {
                m_viewTagsCalculated.RowFilter = "";
                return m_viewTagsCalculated.ToTable();
            }
        }
        // 获取所有纯数据，返回新创建的DataTable
        /// <summary>
        /// 获取所有纯数据，返回新创建的DataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllData()
        {
            lock (m_dtTagDatas)
            {
                m_dtTagDatas.DefaultView.RowFilter = "";
                return m_dtTagDatas.DefaultView.ToTable();
            }
        }
        // 获取测点数据；如果返回值为-999999，则表示获取值失败。
        /// <summary>
        /// 获取测点数据；如果返回值为-999999，则表示获取值失败。
        /// </summary>
        /// <param name="excelCell">数据所在Excel单元格的地址，如F7、F101等</param>
        /// <returns></returns>
        public static double GetTagValue(string excelCell)
        {
            ////如果定时器未运行，则启动
            //if (m_Timer.Enabled == false)
            //    Start();

            //如果重未计算过数据，则计算
            if (m_viewTagsCalculated == null)
                CalAllTagValue();

            double dReturn = GlobalVariables.BadValue;
            //锁定以返回数据
            lock (m_viewTagsCalculated)
            {
                m_viewTagsCalculated.RowFilter = "id=" + excelCell.Replace("F", "");
                if (m_viewTagsCalculated.Count > 0)
                    dReturn = (double)m_viewTagsCalculated[0]["TagValue"];
            }
            return dReturn;
        }

        // 通过测点ID获取测点数据和单位；如果值为-999999，则返回“坏点”。
        /// <summary>
        /// 通过测点ID获取测点数据和单位；如果值为-999999，则返回“坏点”。
        /// </summary>
        /// <param name="sID">测点ID</param>
        /// <returns></returns>
        public static string GetTagValueAndUnitByID(string sID)
        {
            //如果重未计算过数据，则计算
            if (m_viewTagsCalculated == null)
                CalAllTagValue();

            //string sReturn = "坏点";
            string sReturn = "-";
            //锁定以返回数据
            lock (m_viewTagsCalculated)
            {
                m_viewTagsCalculated.RowFilter = "id=" + sID;
                if (m_viewTagsCalculated.Count > 0)
                {
                    double dValue = (double)m_viewTagsCalculated[0]["TagValue"];
                    if (dValue != GlobalVariables.BadValue)
                    {
                        string sUnit = m_viewTagsCalculated[0]["unit"].ToString().Trim().ToLower();
                        sReturn = Math.Round(dValue, 2) + m_viewTagsCalculated[0]["unit"].ToString();// + " "
                        if (sUnit == "mpa")
                            sReturn = Math.Round(dValue, 4) + m_viewTagsCalculated[0]["unit"].ToString();
                        int iID = int.Parse(sID);
                        if (iID == 181)//发电比
                            sReturn = Math.Round(dValue, 4) + m_viewTagsCalculated[0]["unit"].ToString();
                        if (iID == 163)//高压缸效率
                            if (dValue < 75 || dValue > 87)
                                sReturn = "-";
                        if (iID == 167)//中压缸效率
                            if (dValue < 85 || dValue > 95)
                                sReturn = "-";
                        if (iID == 178)//低压缸效率
                            if (dValue < 50 || dValue > 95)
                                sReturn = "-";
                        if (iID == 77 || iID == 86 || iID == 87)
                            if (dValue < 40 || dValue > 95)
                                sReturn = "-";
                        if (iID == 186)//发电煤耗
                            if (dValue < 100 || dValue > 290)
                                sReturn = "-";

                    }
                }
            }
            return sReturn;
        }
        // 获取计算数据的时间
        /// <summary>
        /// 获取计算数据的时间
        /// </summary>
        public static DateTime ValueTime
        {
            get { return m_ValueTime; }
        }
        // 获取第一行数据的时间
        /// <summary>
        /// 获取第一行数据的时间
        /// </summary>
        public static DateTime GetTimeOfFirstData()
        {
            DateTime timeReturn = m_ValueTime;
            try { timeReturn = (DateTime)m_dtTagDatas.Rows[0]["ValueTime"]; }
            catch { }
            return timeReturn;
        }
        // 由Excel表单元格的地址获取m_viewTags里面的行
        /// <summary>
        /// 由Excel表单元格的地址获取m_viewTags里面的行
        /// </summary>
        /// <param name="sExcelCell">数据所在Excel表单元格的地址</param>
        /// <returns></returns>
        static DataRowView GetRow(string sExcelCell)
        {
            DataRowView row = null;
            //sExcelCell = sExcelCell.Replace("F", "");
            m_viewTags.RowFilter = "id=" + sExcelCell.Replace("F", "");
            //m_viewTags.RowFilter = "id='" + sExcelCell.Substring(1) + "'";
            if (m_viewTags.Count > 0)
                row = m_viewTags[0];
            return row;

        }
        // 保存计算的结果
        /// <summary>
        /// 保存计算的结果
        /// </summary>
        /// <param name="dValue">计算的结果</param>
        /// <param name="sExcelCell">数据所在Excel表单元格的地址</param>
        /// <param name="row">要保存数据的DataRowView行</param>        
        static void SaveCalValue(double dValue, string sExcelCell, DataRowView row = null)
        {
            if (row == null)
            {
                if (sExcelCell.Length > 1)
                {
                    m_viewTags.RowFilter = "id=" + sExcelCell.Replace("F", "");
                    if (m_viewTags.Count > 0)
                    {
                        m_viewTags[0].BeginEdit();
                        m_viewTags[0]["TagValue"] = dValue;
                        m_viewTags[0].EndEdit();
                    }
                }
            }
            else
            {
                row.BeginEdit();
                row["TagValue"] = dValue;
                row.EndEdit();
            }
        }
        // 获取当前时间对应的数据库表名称
        /// <summary>
        /// 获取当前时间对应的数据库表名称
        /// </summary>
        /// <returns></returns>
        static string GetTableName()
        {
            return GlobalVariables.TABLE_NAME_PREFIX + GetYearWeek(DateTime.Now);
        }
        // 获取当前时间对应的数据库表名称
        /// <summary>
        /// 获取当前时间对应的数据库表名称
        /// </summary>
        /// <returns></returns>
        static string GetTableName(DateTime dateTime)
        {
            return GlobalVariables.TABLE_NAME_PREFIX + GetYearWeek(dateTime);
        }
        // 获取由给定日期所在的年度和第几周组成的字符串
        /// <summary>
        /// 获取给定日期所在年度和第几周组成的字符串
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns></returns>
        static string GetYearWeek(DateTime date)
        {
            return date.Year.ToString() + DateControl.WeekOfYear(date).ToString();

        }
        // 创建数据库表
        /// <summary>
        /// 创建数据库表
        /// </summary>
        /// <param name="tableName">要创建的数据库的表名称</param>
        /// <returns></returns>
        static bool CreateTable(string tableName)
        {
            bool bReturn = false;
            ArrayList listSql = new ArrayList();
            //创建表，这里只创建一个字段，其余字段在YXYHTags类里添加。
            string sSql = "CREATE TABLE " + tableName + "(	[ValueTime] [datetime] NOT NULL, " +
                "CONSTRAINT PK_" + tableName + " PRIMARY KEY CLUSTERED ([ValueTime] ASC ))";
            listSql.Add(sSql);
            //设置压缩功能
            sSql = "ALTER TABLE " + tableName + " REBUILD PARTITION = ALL WITH (DATA_COMPRESSION = PAGE)";
            listSql.Add(sSql);
            //更新配置表里当前使用的表
            sSql = "update YXYHConfig SET ConfigValue='" + tableName + "' where ConfigKey='CurrentTable'";
            listSql.Add(sSql);
            //在YXYHDataTableNames表里添加创建表的记录
            sSql = "insert into YXYHDataTableNames(DataDate,TableName) values(" + tableName.Replace(GlobalVariables.TABLE_NAME_PREFIX, "")
                + ",'" + tableName + "')";
            listSql.Add(sSql);
            //执行SQL语句，并添加测点对应的字段
            try
            {
                if (SQLHelper.ExecuteSqlReturnState(listSql))
                {
                    GlobalVariables.CurrentTable = tableName;
                    TableTags.createDBDataTableColumns();
                    bReturn = true;
                }
            }
            catch { }
            return bReturn;
        }
        // 获取数据库中数据表里的所有字段
        /// <summary>
        /// 获取数据库中数据表里的所有字段
        /// </summary>
        /// <returns></returns>
        static string GetTableAllFields()
        {
            StringBuilder sbFields = new StringBuilder("ValueTime,");
            //如果重未计算过数据，则计算
            if (m_viewTagsCalculated == null)
                CalAllTagValue();

            lock (m_viewTagsCalculated)
            {
                m_viewTagsCalculated.RowFilter = "";
                foreach (DataRowView row in m_viewTagsCalculated)
                {
                    sbFields.Append(row["ExcelCell"].ToString() + ",");
                }
            }
            return sbFields.ToString().Trim(',');
        }
        // 查询数据库中的测点数据
        /// <summary>
        /// 查询数据库中的测点数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns></returns>
        public static DataTable QueryDBData(DateTime beginTime, DateTime endTime, string fields)
        {
            DataTable dt = new DataTable();
            DataTable dtTableNames = SQLHelper.ExecuteDt("SELECT * FROM YXYHDataTableNames where DataDate>=" + GetYearWeek(beginTime) +
                " and DataDate<=" + GetYearWeek(endTime));
            if (dtTableNames.Rows.Count > 0)
            {
                if (fields == "")
                    fields = GetTableAllFields();
                string sSql = "";
                foreach (DataRow row in dtTableNames.Rows)
                {
                    sSql += " union all   select " + fields + " from " + row["TableName"] + " where ValueTime>='" + beginTime +
                        "' and ValueTime<='" + endTime + "'";
                }

                //加排序速度会慢很多
                //sSql = "select * from (" + sSql.Substring(10) + ") as t order by ValueTime";
                //dt = SQLHelper.ExecuteDt(sSql);

                //不加排序也会安时间来排序，因为时间字段设置成了表的主键
                dt = SQLHelper.ExecuteDt(sSql.Substring(10));
            }
            return dt;
        }
        // 查询数据库中的累计测点数据
        /// <summary>
        /// 查询数据库中的累计测点数据
        /// </summary>
        /// <param name="beginTime">开始时间</param>l
        /// <param name="endTime">结束时间</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns></returns>
        public static DataTable QueryDBTotalData(DateTime beginTime, DateTime endTime, string fields)
        {
            //年度的判断与创建数据库的时候保持一致；供热是跨年，查询数据也跨年。
            int iBeginYear = beginTime.Year;
            if (beginTime.Month < 7)
                iBeginYear = iBeginYear - 1;
            int iEndYear = endTime.Year;
            if (endTime.Month < 7)
                iEndYear = iEndYear - 1;

            DataTable dt = new DataTable();
            DataTable dtTableNames = SQLHelper.ExecuteDt("SELECT * FROM TotalDataTableNames where DataDate>=" + iBeginYear +
                " and DataDate<=" + iEndYear);
            if (dtTableNames.Rows.Count > 0)
            {
                //if (fields == "")
                //    fields = GetTableAllFields();
                string sSql = "";
                foreach (DataRow row in dtTableNames.Rows)
                {
                    sSql += " union all   select " + fields + " from " + row["TableName"] + " where ValueTime>='" + beginTime +
                        "' and ValueTime<='" + endTime + "'";
                }

                //加排序速度会慢很多
                //sSql = "select * from (" + sSql.Substring(10) + ") as t order by ValueTime";
                //dt = SQLHelper.ExecuteDt(sSql);

                //不加排序也会安时间来排序，因为时间字段设置成了表的主键
                dt = SQLHelper.ExecuteDt(sSql.Substring(10));
            }
            return dt;
        }
        // 删除数据库中超过保存年份的老数据，防止数据库无限增大。
        /// <summary>
        /// 删除数据库中超过保存年份的数据，防止数据库无限增大。
        /// 每月1号执行一次。
        /// </summary>
        static void DeleteOldData()
        {
            if (DateTime.Now.Day == 1)
            {
                if (m_IsDeleted == false)
                {
                    try
                    {
                        DataTable dtTableNames = SQLHelper.ExecuteDt("SELECT * FROM YXYHDataTableNames where DataDate<" + GetYearWeek(DateTime.Now.AddYears(-GlobalVariables.YearOfDataSaved)));
                        if (dtTableNames.Rows.Count > 0)
                        {
                            ArrayList listSql = new ArrayList();
                            foreach (DataRow row in dtTableNames.Rows)
                            {
                                listSql.Add("drop table " + row["TableName"]);
                                listSql.Add("delete from YXYHDataTableNames where DataDate=" + row["DataDate"].ToString());
                            }
                            SQLHelper.ExecuteSql(listSql);
                        }

                        m_IsDeleted = true;
                    }
                    catch { }
                }
            }
            else
                if (m_IsDeleted == true)
                    m_IsDeleted = false;
        }
        // 复位累计值和平均值
        /// <summary>
        /// 复位累计值和平均值
        /// </summary>
        public static void ResetTotal()
        {
            if (m_rowTotalDataLast == null)
                m_rowTotalDataLast = m_dtTotalTagDatas.NewRow();

            //插入0
            StringBuilder sbFields = new StringBuilder("ValueTime,HourOfYear");
            StringBuilder sbValue = new StringBuilder("'" + DateTime.Now.ToString() + "',0");
            m_viewTotalTags.RowFilter = "";
            foreach (DataRowView row in m_viewTotalTags)
            {
                sbFields.Append(",F" + row["id"]);
                sbValue.Append(",0");
            }
            try
            {
                if (SQLHelper.ExecuteSql("SELECT * into " + GlobalVariables.CurrentTotalDataTable + "_Bak  FROM " + GlobalVariables.CurrentTotalDataTable) == 0)
                    SQLHelper.ExecuteSql("insert into " + GlobalVariables.CurrentTotalDataTable + "_Bak SELECT *   FROM " + GlobalVariables.CurrentTotalDataTable);
                SQLHelper.ExecuteSql("truncate table " + GlobalVariables.CurrentTotalDataTable);
                SQLHelper.ExecuteSql("insert into " + GlobalVariables.CurrentTotalDataTable + "(" + sbFields.ToString() + ") values(" +
                    sbValue.ToString() + ")");
                for (int i = 1; i < m_viewTotalTags.Count; i++)
                    try
                    {
                        m_rowTotalDataLast[i] = 0.0;

                    }
                    catch { }
                m_dtTotalTagDatas.Rows.Clear();

                ResetAvgValue();

                System.Windows.Forms.MessageBox.Show("复位累计值成功！");
            }
            catch (Exception ex)
            { WriteLog.WriteLogs(ex.ToString()); }
        }
        // 复位平均值
        /// <summary>
        /// 复位平均值
        /// </summary>
        public static void ResetAvgValue()
        {
            m_rowDataLast["F250"] = 0;
            m_rowDataLast["F251"] = 0;
            m_rowDataLast["F253"] = 0;
            m_rowDataLast["F258"] = 0;
            m_rowDataLast["F259"] = 0;
            m_rowDataLast["F260"] = 0;
        }

        #region 各计算数据的计算方法
        // 获取测量值数据！！！！！！
        /// <summary>
        /// 获取测量值数据
        /// </summary>
        /// <param name="sExcelCell">数据所在Excel表单元格的地址</param>
        /// <returns></returns>
        static double GetMeasureValue(string sExcelCell)
        {
            double dReturn = 0;
            double dDQY = 0.1;
            int iID = int.Parse(sExcelCell.Replace("F", ""));
            m_viewTags.RowFilter = "id=4";
            try { dDQY = (double)m_viewTags[0]["TagValue"]; }
            catch { try { dDQY = double.Parse(m_viewTags[0]["TagValue"].ToString()); } catch { } }
            m_viewTags.RowFilter = "id=" + sExcelCell.Replace("F", "");
            try
            {
                string sIsSet = m_viewTags[0]["IsSet"].ToString();
                try { dReturn = (double)m_viewTags[0]["TagValue"]; }
                catch
                {
                    try { dReturn = double.Parse(m_viewTags[0]["TagValue"].ToString()); }
                    catch { }
                }
                //if (dReturn == GlobalVariables.BadValue)
                if (sIsSet == "启用" || dReturn == GlobalVariables.BadValue)//现在暖气没供，所有为0的值也做替换，等运行起来再把替换去掉！！！！！||dReturn ==0
                    try { dReturn = (double)m_viewTags[0]["TagValue"] + dDQY; }
                    catch { dReturn = double.Parse(m_viewTags[0]["TagValue"].ToString()) + dDQY; }
                if (iID == 66 || iID == 11 || iID == 14 || iID == 17 || iID == 21 || iID == 23 || iID == 24 || iID == 35 || iID == 36 || iID == 49 || iID == 53 || iID == 54 || iID == 67)
                    try { dReturn = (double)m_viewTags[0]["TagValue"] + dDQY; }
                    catch { dReturn = double.Parse(m_viewTags[0]["TagValue"].ToString()) + dDQY; }
                if (iID == 73 || iID == 98 || iID == 99 || iID == 111 || iID == 113 || iID == 114 || iID == 125 || iID == 134 || iID == 165 || iID == 169)// 
                    try { dReturn = (double)m_viewTags[0]["TagValue"] + dDQY; }
                    catch { dReturn = double.Parse(m_viewTags[0]["TagValue"].ToString()) + dDQY; }
                if (iID == 232 || iID == 270 || iID == 275 || iID == 276 || iID == 280 || iID == 285 || iID == 286)
                    try { dReturn = (double)m_viewTags[0]["TagValue"] + dDQY; }
                    catch { dReturn = double.Parse(m_viewTags[0]["TagValue"].ToString()) + dDQY; }
                if (iID == 152)
                    dReturn += 1.3;
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }

            return dReturn;
        }
        // 获取固定值数据！！！！！！
        /// <summary>
        /// 获取固定值数据
        /// </summary>
        /// <param name="sExcelCell">数据所在Excel表单元格的地址</param>
        /// <returns></returns>
        static double GetFixedValue(string sExcelCell)
        {
            double dReturn = 0;
            m_viewTags.RowFilter = "id=" + sExcelCell.Replace("F", "");
            try { dReturn = (double)m_viewTags[0]["SetValue"]; }
            catch
            {
                try { dReturn = double.Parse(m_viewTags[0]["SetValue"].ToString()); }
                catch { }
            }
            return dReturn;
        }
        //给水焓
        /// <summary>
        /// 给水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF7()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F7");
            if (row["TagValue"] == DBNull.Value)
            {
                double F11 = GetMeasureValue("F11");
                double F6 = GetMeasureValue("F6");
                WASPCN.PT2H(F11, F6, ref dReturn, ref Range);
                SaveCalValue(dReturn, "F7", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //减温水焓
        /// <summary>
        /// 减温水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF10()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F10");
            if (row["TagValue"] == DBNull.Value)
            {
                dReturn = CalF60();
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 再热减温水温度
        /// <summary>
        /// 再热减温水温度
        /// </summary>
        /// <returns></returns>
        private static double CalF13()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F13");
            if (row["TagValue"] == DBNull.Value)
            {
                double F9 = GetMeasureValue("F9");
                dReturn = F9 - 1.5;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //再热减温水焓
        /// <summary>
        /// 再热减温水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF15()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F15");
            if (row["TagValue"] == DBNull.Value)
            {
                double F14 = GetMeasureValue("F14");
                double F13 = CalF13();
                WASPCN.PT2H(F14, F13, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //主汽焓
        /// <summary>
        /// 主汽焓
        /// </summary>
        /// <returns></returns>
        private static double CalF18()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F18");
            if (row["TagValue"] == DBNull.Value)
            {
                double F17 = GetMeasureValue("F17");
                double F16 = GetMeasureValue("F16");
                WASPCN.PT2H(F17, F16, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //再热焓
        /// <summary>
        /// 再热焓
        /// </summary>
        /// <returns></returns>
        private static double CalF22()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F22");
            if (row["TagValue"] == DBNull.Value)
            {
                double F21 = GetMeasureValue("F21");
                double F20 = GetMeasureValue("F20");
                WASPCN.PT2H(F21, F20, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //1抽压力（加热器）
        /// <summary>
        /// 1抽压力（加热器）
        /// </summary>
        /// <returns></returns>
        private static double CalF24()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F24");
            if (row["TagValue"] == DBNull.Value)
            {
                double F23 = GetMeasureValue("F23");
                dReturn = F23 * 0.97;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //1抽焓
        /// <summary>
        /// 1抽焓
        /// </summary>
        /// <returns></returns>
        private static double CalF26()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F26");
            if (row["TagValue"] == DBNull.Value)
            {
                double F24 = CalF24();
                double F25 = GetMeasureValue("F25");
                WASPCN.PT2H(F24, F25, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //1抽疏水焓
        /// <summary>
        /// 1抽疏水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF28()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F28");
            if (row["TagValue"] == DBNull.Value)
            {
                double F24 = CalF24();
                double F27 = GetMeasureValue("F27");
                WASPCN.PT2H(F24, F27, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //1抽进水焓
        /// <summary>
        /// 1抽进水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF30()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F30");
            if (row["TagValue"] == DBNull.Value)
            {
                double F11 = GetMeasureValue("F11");
                double F29 = GetMeasureValue("F29");
                WASPCN.PT2H(F11, F29, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //1抽出水焓
        /// <summary>
        /// 1抽出水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF32()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F32");
            if (row["TagValue"] == DBNull.Value)
            {
                double F11 = GetMeasureValue("F11");
                double F31 = GetMeasureValue("F31");
                WASPCN.PT2H(F11, F31, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //1抽抽汽量
        /// <summary>
        /// 1抽抽汽量
        /// </summary>
        /// <returns></returns>
        private static double CalF33()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F33");
            if (row["TagValue"] == DBNull.Value)
            {
                //F5*(F32-F30)/(F26-F28)
                double F5 = GetMeasureValue("F5");
                double F32 = CalF32();
                double F30 = CalF30();
                double F26 = CalF26();
                double F28 = CalF28();
                dReturn = F5 * (F32 - F30) / (F26 - F28);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //1抽疏水量
        /// <summary>
        /// 1抽疏水量
        /// </summary>
        /// <returns></returns>
        private static double CalF34()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F34");
            if (row["TagValue"] == DBNull.Value)
            {
                dReturn = CalF33();
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //2抽压力（加热器）
        /// <summary>
        /// 2抽压力（加热器）
        /// </summary>
        /// <returns></returns>
        private static double CalF36()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F36");
            if (row["TagValue"] == DBNull.Value)
            {
                double F35 = GetMeasureValue("F35");
                dReturn = F35 * 0.97;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //2抽焓
        /// <summary>
        /// 2抽焓
        /// </summary>
        /// <returns></returns>
        private static double CalF38()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F38");
            if (row["TagValue"] == DBNull.Value)
            {
                double F36 = CalF36();
                double F37 = GetMeasureValue("F37");
                WASPCN.PT2H(F36, F37, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //2抽疏水焓
        /// <summary>
        /// 2抽疏水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF40()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F40");
            if (row["TagValue"] == DBNull.Value)
            {
                double F36 = CalF36();
                double F39 = GetMeasureValue("F39");
                WASPCN.PT2H(F36, F39, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //2抽进水焓
        /// <summary>
        /// 2抽进水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF42()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F42");
            if (row["TagValue"] == DBNull.Value)
            {
                double F11 = GetMeasureValue("F11");
                double F41 = GetMeasureValue("F41");
                WASPCN.PT2H(F11, F41, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //2抽出水焓
        /// <summary>
        /// 2抽出水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF44()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F44");
            if (row["TagValue"] == DBNull.Value)
            {
                double F11 = GetMeasureValue("F11");
                double F43 = GetMeasureValue("F43");
                WASPCN.PT2H(F11, F43, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //2抽抽汽量
        /// <summary>
        /// 2抽抽汽量
        /// </summary>
        /// <returns></returns>
        private static double CalF45()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F45");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F5*(F44-F42)-F34*(F28-F40))/(F38-F40)
                double F5 = GetMeasureValue("F5");
                double F44 = CalF44();
                double F42 = CalF42();
                double F34 = CalF34();
                double F28 = CalF28();
                double F40 = CalF40();
                double F38 = CalF38();
                dReturn = (F5 * (F44 - F42) - F34 * (F28 - F40)) / (F38 - F40);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //2抽疏水量
        /// <summary>
        /// 2抽疏水量
        /// </summary>
        /// <returns></returns>
        private static double CalF46()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F46");
            if (row["TagValue"] == DBNull.Value)
            {
                double F45 = CalF45();
                double F34 = CalF34();
                dReturn = F45 + F34;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //再热流量
        /// <summary>
        /// 再热流量
        /// </summary>
        /// <returns></returns>
        private static double CalF48()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F48");
            if (row["TagValue"] == DBNull.Value)
            {
                //F19-F46-F47
                double F19 = GetMeasureValue("F19");
                double F46 = CalF46();
                double F47 = GetMeasureValue("F47");
                dReturn = F19 - F46 - F47;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //再热热段熵
        /// <summary>
        /// 再热热段熵
        /// </summary>
        /// <returns></returns>
        private static double CalF51()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F51");
            if (row["TagValue"] == DBNull.Value)
            {
                double F49 = GetMeasureValue("F49");
                double F50 = GetMeasureValue("F50");
                WASPCN.PT2S(F49, F50, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //再热段焓
        /// <summary>
        /// 再热段焓
        /// </summary>
        /// <returns></returns>
        private static double CalF52()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F52");
            if (row["TagValue"] == DBNull.Value)
            {
                double F49 = GetMeasureValue("F49");
                double F50 = GetMeasureValue("F50");
                WASPCN.PT2H(F49, F50, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //3抽压力
        /// <summary>
        /// 3抽压力
        /// </summary>
        /// <returns></returns>
        private static double CalF54()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F54");
            if (row["TagValue"] == DBNull.Value)
            {
                double F53 = GetMeasureValue("F53");
                dReturn = F53 * 0.97;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //3抽焓
        /// <summary>
        /// 3抽焓
        /// </summary>
        /// <returns></returns>
        private static double CalF56()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F56");
            if (row["TagValue"] == DBNull.Value)
            {
                double F54 = CalF54();
                double F55 = GetMeasureValue("F55");
                WASPCN.PT2H(F54, F55, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //3抽疏水焓
        /// <summary>
        /// 3抽疏水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF58()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F58");
            if (row["TagValue"] == DBNull.Value)
            {
                double F54 = CalF54();
                double F57 = GetMeasureValue("F57");
                WASPCN.PT2H(F54, F57, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //3抽进水焓
        /// <summary>
        /// 3抽进水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF60()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F60");
            if (row["TagValue"] == DBNull.Value)
            {
                double F11 = GetMeasureValue("F11");
                //double F59 = GetMeasureValue("F59");
                double F59 = GetMeasureValue("F59");//因测量值误差和前置泵功率影响，来修正0.80度+0.50 
                WASPCN.PT2H(F11, F59, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //3抽出水焓
        /// <summary>
        /// 3抽出水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF62()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F62");
            if (row["TagValue"] == DBNull.Value)
            {
                double F11 = GetMeasureValue("F11");
                double F61 = GetMeasureValue("F61");
                WASPCN.PT2H(F11, F61, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //3抽抽汽量
        /// <summary>
        /// 3抽抽汽量
        /// </summary>
        /// <returns></returns>
        private static double CalF64()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F64");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F5*(F62-F60)-F46*(F40-F58))/(F56-F58)-F63
                double F5 = GetMeasureValue("F5");
                double F62 = CalF62();
                double F60 = CalF60();
                double F46 = CalF46();
                double F40 = CalF40();
                double F58 = CalF58();
                double F56 = CalF56();
                double F63 = GetMeasureValue("F63");
                dReturn = (F5 * (F62 - F60) - F46 * (F40 - F58)) / (F56 - F58) - F63;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //3抽疏水量
        /// <summary>
        /// 3抽疏水量
        /// </summary>
        /// <returns></returns>
        private static double CalF65()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F65");
            if (row["TagValue"] == DBNull.Value)
            {
                //F64+F46+F63
                double F64 = CalF64();
                double F46 = CalF46();
                double F63 = GetMeasureValue("F63");
                dReturn = F64 + F46 + F63;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 除氧器压力
        /// <summary>
        /// 除氧器压力
        /// </summary>
        /// <returns></returns>
        private static double CalF67()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F67");
            if (row["TagValue"] == DBNull.Value)
            {
                double F66 = GetMeasureValue("F66");
                dReturn = F66 * 0.95;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //给水泵入口熵
        /// <summary>
        /// 给水泵入口熵
        /// </summary>
        /// <returns></returns>
        private static double CalF68()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F68");
            if (row["TagValue"] == DBNull.Value)
            {
                double F285 = GetMeasureValue("F285");//B汽泵入口压力
                double F89 = CalF89();
                //double F67 = CalF67();
                WASPCN.PH2S(F285, F89, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //给水泵出口等熵焓
        /// <summary>
        /// 给水泵出口等熵焓
        /// </summary>
        /// <returns></returns>
        private static double CalF69()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F69");
            if (row["TagValue"] == DBNull.Value)
            {
                double F11 = GetMeasureValue("F11");
                double F68 = CalF68();
                WASPCN.PS2H(F11, F68, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //给水泵等熵焓升
        /// <summary>
        /// 给水泵等熵焓升
        /// </summary>
        /// <returns></returns>
        private static double CalF70()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F70");
            if (row["TagValue"] == DBNull.Value)
            {
                double F69 = CalF69();
                double F89 = CalF89();
                dReturn = F69 - F89;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //4抽焓
        /// <summary>
        /// 4抽焓
        /// </summary>
        /// <returns></returns>
        private static double CalF72()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F72");
            if (row["TagValue"] == DBNull.Value)
            {
                double F66 = GetMeasureValue("F66");
                double F71 = GetMeasureValue("F71");
                WASPCN.PT2H(F66, F71, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小汽机进汽熵
        /// <summary>
        /// 小汽机进汽熵
        /// </summary>
        /// <returns></returns>
        private static double CalF74()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F74");
            if (row["TagValue"] == DBNull.Value)
            {
                double F73 = GetMeasureValue("F73");
                double F72 = CalF72();
                WASPCN.PH2S(F73, F72, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //给水泵焓升
        /// <summary>
        /// 给水泵焓升
        /// </summary>
        /// <returns></returns>
        private static double CalF76()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F76");
            if (row["TagValue"] == DBNull.Value)
            {
                double F60 = CalF60();
                double F89 = CalF89();
                dReturn = F60 - F89;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //给水泵效率
        /// <summary>
        /// 给水泵效率
        /// </summary>
        /// <returns></returns>
        private static double CalF77()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F77");
            if (row["TagValue"] == DBNull.Value)
            {
                double F70 = CalF70();
                double F76 = CalF76();
                dReturn = F70 / F76;
                if (dReturn > 1)
                    dReturn = 1;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小机排汽焓
        /// <summary>
        /// 小机排汽焓
        /// </summary>
        /// <returns></returns>
        private static double CalF78()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F78");
            if (row["TagValue"] == DBNull.Value)
            {
                //F72-F81*3.6/F75
                double F72 = CalF72();
                double F81 = CalF81();
                double F75 = GetMeasureValue("F75");//小机进汽量
                dReturn = F72 - F81 * 3.6 / F75;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小机排气温度
        /// <summary>
        /// 小机排气温度
        /// </summary>
        /// <returns></returns>
        private static double CalF79()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F79");
            if (row["TagValue"] == DBNull.Value)
            {
                double F83 = CalF83();
                double F78 = CalF78();
                WASPCN.PH2T(F83, F78, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小机轴功率
        /// <summary>
        /// 小机轴功率
        /// </summary>
        /// <returns></returns>
        private static double CalF81()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F81");
            if (row["TagValue"] == DBNull.Value)
            {
                //F76*F92/3.6
                double F76 = CalF76();
                double F92 = CalF92();
                dReturn = F76 * F92 / 3.6;

                //double F75=GetMeasureValue("F75");
                //double F72 = CalF72();
                //dReturn =F72-( F76 * F92 + 100 )/ 3.6/(F75*1000);

                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小机排汽等熵焓
        /// <summary>
        /// 小机排汽等熵焓
        /// </summary>
        /// <returns></returns>
        private static double CalF82()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F82");
            if (row["TagValue"] == DBNull.Value)
            {
                double F83 = CalF83();
                double F74 = CalF74();
                WASPCN.PS2H(F83, F74, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小机排气压力
        /// <summary>
        /// 小机排气压力
        /// </summary>
        /// <returns></returns>
        private static double CalF83()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F83");
            if (row["TagValue"] == DBNull.Value)
            {
                double F126 = GetMeasureValue("F126") / 1000;//除以1000把单位换算成MPa
                dReturn = 1.05 * F126;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小机等熵焓差
        /// <summary>
        /// 小机等熵焓差
        /// </summary>
        /// <returns></returns>
        private static double CalF84()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F84");
            if (row["TagValue"] == DBNull.Value)
            {
                double F72 = CalF72();
                double F82 = CalF82();
                dReturn = F72 - F82;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小机实际焓降
        /// <summary>
        /// 小机实际焓降
        /// </summary>
        /// <returns></returns>
        private static double CalF85()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F85");
            if (row["TagValue"] == DBNull.Value)
            {
                double F72 = CalF72();
                double F78 = CalF78();
                dReturn = F72 - F78;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小机总效率（含机械损失）
        /// <summary>
        /// 小机总效率（含机械损失）
        /// </summary>
        /// <returns></returns>
        private static double CalF86()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F86");
            if (row["TagValue"] == DBNull.Value)
            {
                double F85 = CalF85();
                double F84 = CalF84();
                dReturn = F85 / F84;
                if (dReturn > 1)
                    dReturn = 1;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //泵组总效率
        /// <summary>
        /// 泵组总效率
        /// </summary>
        /// <returns></returns>
        private static double CalF87()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F87");
            if (row["TagValue"] == DBNull.Value)
            {
                double F86 = CalF86();
                double F77 = CalF77();
                dReturn = F86 * F77;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //除氧器水温度
        /// <summary>
        /// 除氧器水温度
        /// </summary>
        /// <returns></returns>
        private static double CalF88()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F88");
            if (row["TagValue"] == DBNull.Value)
            {
                double F67 = CalF67();
                WASPCN.P2T(F67, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //给水泵入口水焓
        /// <summary>
        /// 给水泵入口水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF89()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F89");
            if (row["TagValue"] == DBNull.Value)
            {
                double F88 = CalF88();
                WASPCN.T2HL(F88, ref dReturn, ref Range);
                double F5 = GetMeasureValue("F5");
                dReturn = dReturn - 0.32 * F5 * 3600 / 1000000;//0.32相当于前置泵单耗（kWh/t)
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //除氧器进水焓
        /// <summary>
        /// 除氧器进水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF91()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F91");
            if (row["TagValue"] == DBNull.Value)
            {
                double F125 = GetMeasureValue("F125");
                double F90 = GetMeasureValue("F90");
                WASPCN.PT2H(F125, F90, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //除氧器出水量
        /// <summary>
        /// 除氧器出水量
        /// </summary>
        /// <returns></returns>
        private static double CalF92()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F92");
            if (row["TagValue"] == DBNull.Value)
            {
                dReturn = GetMeasureValue("F5");
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //除氧器抽汽量
        /// <summary>
        /// 除氧器抽汽量
        /// </summary>
        /// <returns></returns>
        private static double CalF93()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F93");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F128*(F89-F91)+F110*(F89-F97)-F65*(F58-F89))/(F72-F89)
                double F128 = GetMeasureValue("F128");
                double F89 = CalF89();
                double F91 = CalF91();
                double F110 = GetMeasureValue("F110");
                double F97 = CalF97();
                double F65 = CalF65();
                double F58 = CalF58();
                double F72 = CalF72();
                dReturn = (F128 * (F89 - F91) + F110 * (F89 - F97) - F65 * (F58 - F89)) / (F72 - F89);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //供热疏水回水量
        /// <summary>
        /// 供热疏水回水量
        /// </summary>
        /// <returns></returns>
        private static double CalF94()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F94");
            if (row["TagValue"] == DBNull.Value)
            {
                dReturn = GetMeasureValue("F110");
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //供热回水焓
        /// <summary>
        /// 供热回水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF97()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F97");
            if (row["TagValue"] == DBNull.Value)
            {
                double F96 = GetMeasureValue("F96");
                double F95 = GetMeasureValue("F95");
                WASPCN.PT2H(F96, F95, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //5抽加热器压力
        /// <summary>
        /// 5抽加热器压力
        /// </summary>
        /// <returns></returns>
        private static double CalF99()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F99");
            if (row["TagValue"] == DBNull.Value)
            {
                double F98 = GetMeasureValue("F98");
                dReturn = F98 * 0.95;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //5抽焓
        /// <summary>
        /// 5抽焓
        /// </summary>
        /// <returns></returns>
        private static double CalF101()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F101");
            if (row["TagValue"] == DBNull.Value)
            {
                double F99 = CalF99();
                double F100 = GetMeasureValue("F100");
                WASPCN.PT2H(F99, F100, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        /// <summary>
        /// 5抽疏水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF103()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F103");
            if (row["TagValue"] == DBNull.Value)
            {
                double F99 = CalF99();
                double F102 = GetMeasureValue("F102");
                WASPCN.PT2H(F99, F102, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //5抽进水焓
        /// <summary>
        /// 5抽进水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF105()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F105");
            if (row["TagValue"] == DBNull.Value)
            {
                double F125 = GetMeasureValue("F125");
                double F104 = GetMeasureValue("F104");
                WASPCN.PT2H(F125, F104, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //5抽出水焓
        /// <summary>
        /// 5抽出水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF107()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F107");
            if (row["TagValue"] == DBNull.Value)
            {
                double F125 = GetMeasureValue("F125");
                double F106 = GetMeasureValue("F106");
                WASPCN.PT2H(F125, F106, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //5抽抽汽量
        /// <summary>
        /// 5抽抽汽量
        /// </summary>
        /// <returns></returns>
        private static double CalF108()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F108");
            if (row["TagValue"] == DBNull.Value)
            {
                //F128*(F107-F105)/(F101-F103)
                double F128 = GetMeasureValue("F128");
                double F107 = CalF107();
                double F105 = CalF105();
                double F101 = CalF101();
                double F103 = CalF103();
                dReturn = F128 * (F107 - F105) / (F101 - F103);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //5抽疏水量
        /// <summary>
        /// 5抽疏水量
        /// </summary>
        /// <returns></returns>
        private static double CalF109()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F109");
            if (row["TagValue"] == DBNull.Value)
            {
                dReturn = CalF108();
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //调节阀节流损失
        /// <summary>
        /// 调节阀节流损失
        /// </summary>
        /// <returns></returns>
        private static double CalF112()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F112");
            if (row["TagValue"] == DBNull.Value)
            {
                double F98 = GetMeasureValue("F98");
                double F111 = GetMeasureValue("F111");
                dReturn = F98 - F111;
                if (dReturn < 0)
                    dReturn = 0;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //6抽加热器压力
        /// <summary>
        /// 6抽加热器压力
        /// </summary>
        /// <returns></returns>
        private static double CalF114()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F114");
            if (row["TagValue"] == DBNull.Value)
            {
                double F113 = GetMeasureValue("F113");
                dReturn = F113 * 0.95;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //6抽焓
        /// <summary>
        /// 6抽焓
        /// </summary>
        /// <returns></returns>
        private static double CalF116()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F116");
            if (row["TagValue"] == DBNull.Value)
            {
                double F114 = CalF114();
                double F115 = GetMeasureValue("F115");
                WASPCN.PT2H(F114, F115, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //6抽疏水焓
        /// <summary>
        /// 6抽疏水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF118()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F118");
            if (row["TagValue"] == DBNull.Value)
            {
                double F114 = CalF114();
                double F117 = GetMeasureValue("F117");
                WASPCN.PT2H(F114, F117, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //6抽进水焓
        /// <summary>
        /// 6抽进水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF120()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F120");
            if (row["TagValue"] == DBNull.Value)
            {
                double F125 = GetMeasureValue("F125");
                double F119 = GetMeasureValue("F119");
                WASPCN.PT2H(F125, F119, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //6抽出水焓
        /// <summary>
        /// 6抽出水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF122()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F122");
            if (row["TagValue"] == DBNull.Value)
            {
                double F125 = GetMeasureValue("F125");
                double F121 = GetMeasureValue("F121");
                WASPCN.PT2H(F125, F121, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //6抽抽汽量
        /// <summary>
        /// 6抽抽汽量
        /// </summary>
        /// <returns></returns>
        private static double CalF123()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F123");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F128*(F122-F120)-F109*(F103-F118))/(F116-F118)
                double F128 = GetMeasureValue("F128");
                double F122 = CalF122();
                double F120 = CalF120();
                double F109 = CalF109();
                double F103 = CalF103();
                double F118 = CalF118();
                double F116 = CalF116();
                dReturn = (F128 * (F122 - F120) - F109 * (F103 - F118)) / (F116 - F118);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //6抽疏水量
        /// <summary>
        /// 6抽疏水量
        /// </summary>
        /// <returns></returns>
        private static double CalF124()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F124");
            if (row["TagValue"] == DBNull.Value)
            {
                double F123 = CalF123();
                double F109 = CalF109();
                dReturn = F123 + F109;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //计算凝结水流量
        /// <summary>
        /// 计算凝结水流量
        /// </summary>
        /// <returns></returns>
        private static double CalF130()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F130");
            if (row["TagValue"] == DBNull.Value)
            {
                //F157+F75+F149+F124+F129
                double F157 = CalF157();
                double F75 = GetMeasureValue("F75");
                double F149 = GetMeasureValue("F149");
                double F124 = CalF124();
                double F129 = GetMeasureValue("F129");
                dReturn = F157 + F75 + F149 + F124 + F129;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //凝结水焓
        /// <summary>
        /// 凝结水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF131()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F131");
            if (row["TagValue"] == DBNull.Value)
            {
                double F126 = GetMeasureValue("F126") / 1000;
                WASPCN.P2HL(F126, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //凝结水泵出水焓
        /// <summary>
        /// 凝结水泵出水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF132()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F132");
            if (row["TagValue"] == DBNull.Value)
            {
                double F125 = GetMeasureValue("F125");
                double F127 = GetMeasureValue("F127");
                WASPCN.PT2H(F125, F127, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //补水放热焓
        /// <summary>
        /// 补水放热焓
        /// </summary>
        /// <returns></returns>
        private static double CalF133()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F133");
            if (row["TagValue"] == DBNull.Value)
            {
                dReturn = CalF135();
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //锅炉水焓
        /// <summary>
        /// 锅炉水焓
        /// </summary>
        /// <returns></returns>
        private static double CalF135()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F135");
            if (row["TagValue"] == DBNull.Value)
            {
                double F134 = GetMeasureValue("F134");
                WASPCN.P2HL(F134, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //排污热量
        /// <summary>
        /// 排污热量
        /// </summary>
        /// <returns></returns>
        private static double CalF136()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F136");
            if (row["TagValue"] == DBNull.Value)
            {
                //F149*(F133-F148)/3600
                double F149 = GetMeasureValue("F149");
                double F133 = CalF133();
                double F148 = GetMeasureValue("F148");
                dReturn = F149 * (F133 - F148) / 3600;
                if (dReturn < 0)
                    dReturn = 0;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //过热吸热量
        /// <summary>
        /// 过热吸热量
        /// </summary>
        /// <returns></returns>
        private static double CalF137()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F137");
            if (row["TagValue"] == DBNull.Value)
            {
                //((F19-F8)*(F18-F7)+F8*(F18-F10))/3600
                double F19 = GetMeasureValue("F19");
                double F8 = GetMeasureValue("F8");
                double F18 = CalF18();
                double F7 = CalF7();
                double F10 = CalF10();
                dReturn = ((F19 - F8) * (F18 - F7) + F8 * (F18 - F10)) / 3600;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //再热吸热量
        /// <summary>
        /// 再热吸热量
        /// </summary>
        /// <returns></returns>
        private static double CalF138()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F138");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F48*(F22-F38)+F12*(F22-F15))/3600
                double F48 = CalF48();
                double F22 = CalF22();
                double F38 = CalF38();
                double F12 = GetMeasureValue("F12");
                double F15 = CalF15();
                dReturn = (F48 * (F22 - F38) + F12 * (F22 - F15)) / 3600;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //总吸热量
        /// <summary>
        /// 总吸热量
        /// </summary>
        /// <returns></returns>
        private static double CalF139()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F139");
            if (row["TagValue"] == DBNull.Value)
            {
                double F138 = CalF138();
                double F137 = CalF137();
                dReturn = F138 + F137;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //供热抽汽热量
        /// <summary>
        /// 供热抽汽热量
        /// </summary>
        /// <returns></returns>
        private static double CalF140()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F140");
            if (row["TagValue"] == DBNull.Value)
            {
                //F110*(F101-F97)/3600
                double F110 = GetMeasureValue("F110");
                double F101 = CalF101();
                double F97 = CalF97();
                dReturn = F110 * (F101 - F97) / 3600;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //其他不明损失热量（含真空泵、连定排水、除氧器排汽疏水、轴加风机抽走热量等）
        /// <summary>
        /// 其他不明损失热量（含真空泵、连定排水、除氧器排汽疏水、轴加风机抽走热量等）
        /// </summary>
        /// <returns></returns>
        private static double CalF144()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F144");
            if (row["TagValue"] == DBNull.Value)
            {
                //F139-F145-F159-F141-F142-F143-F136
                double F139 = CalF139();
                double F145 = GetMeasureValue("F145");
                double F159 = CalF159();
                double F141 = GetMeasureValue("F141");
                double F142 = GetMeasureValue("F142");
                double F143 = GetMeasureValue("F143");
                double F136 = CalF136();
                dReturn = F139 - F145 - F159 - F141 - F142 - F143 - F136;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //凝汽器低压排气单位放热量
        /// <summary>
        /// 凝汽器低压排气单位放热量
        /// </summary>
        /// <returns></returns>
        private static double CalF153()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F153");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F180-F131)/3600
                double F180 = CalF180();
                double F131 = CalF131();
                dReturn = (F180 - F131) / 3600;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小机单位放热量
        /// <summary>
        /// 小机单位放热量
        /// </summary>
        /// <returns></returns>
        private static double CalF154()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F154");
            if (row["TagValue"] == DBNull.Value)
            {
                double F168 = CalF168();
                double F75 = GetMeasureValue("F75");
                dReturn = F168 / F75;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小机单位放热量
        /// <summary>
        /// 小机单位放热量
        /// </summary>
        /// <returns></returns>
        private static double CalF155()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F155");
            if (row["TagValue"] == DBNull.Value)
            {
                //F161*(F151-F150)*4.1868/3600
                double F161 = GetMeasureValue("F161");
                double F151 = GetMeasureValue("F151");
                double F150 = GetMeasureValue("F150");

                dReturn = F161 * (F151 - F150) * 4.1868 / 3600;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //低压缸排汽量1（热网平衡计算）
        /// <summary>
        /// 低压缸排汽量1（热网平衡计算）
        /// </summary>
        /// <returns></returns>
        private static double CalF157()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F157");
            if (row["TagValue"] == DBNull.Value)
            {
                //F128-F124-F75-F156
                double F128 = GetMeasureValue("F128");
                double F124 = CalF124();
                double F75 = GetMeasureValue("F75");
                double F156 = GetMeasureValue("F156");
                dReturn = F128 - F124 - F75 - F156;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //本机总放热量
        /// <summary>
        /// 本机总放热量
        /// </summary>
        /// <returns></returns>
        private static double CalF159()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F159");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F161*(F151-F150)+F161*(F152-F151))*4.1868/3600-F158*620*4.1868/3600
                double F161 = GetMeasureValue("F161");
                double F151 = GetMeasureValue("F151");
                double F150 = GetMeasureValue("F150");
                double F152 = GetMeasureValue("F152");
                double F158 = GetMeasureValue("F158");
                dReturn = (F161 * (F151 - F150) + F161 * (F152 - F151)) * 4.1868 / 3600 - F158 * 620 * 4.1868 / 3600;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //两台机组首站总放热量
        /// <summary>
        /// 两台机组首站总放热量
        /// </summary>
        /// <returns></returns>
        private static double CalF160()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F160");
            if (row["TagValue"] == DBNull.Value)
            {
                //F159+F158*620*4.1868/3600
                double F159 = CalF159();
                double F158 = GetMeasureValue("F158");
                dReturn = F159 + F158 * 620 * 4.1868 / 3600;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //主汽熵
        /// <summary>
        /// 主汽熵
        /// </summary>
        /// <returns></returns>
        private static double CalF162()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F162");
            if (row["TagValue"] == DBNull.Value)
            {
                double F17 = GetMeasureValue("F17");
                double F16 = GetMeasureValue("F16");
                WASPCN.PT2S(F17, F16, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //高压缸排汽等熵排汽焓
        /// <summary>
        /// 高压缸排汽等熵排汽焓
        /// </summary>
        /// <returns></returns>
        private static double CalF163()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F163");
            if (row["TagValue"] == DBNull.Value)
            {
                double F35 = GetMeasureValue("F35");
                double F162 = CalF162();
                WASPCN.PS2H(F35, F162, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //高压缸效率
        /// <summary>
        /// 高压缸效率
        /// </summary>
        /// <returns></returns>
        private static double CalF164()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F164");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F18-F38)/(F18-F163)
                double F18 = CalF18();
                double F38 = CalF38();
                double F163 = CalF163();
                dReturn = (F18 - F38) / (F18 - F163);
                if (dReturn > 1)
                    dReturn = 1;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //中压缸排汽等熵焓
        /// <summary>
        /// 中压缸排汽等熵焓
        /// </summary>
        /// <returns></returns>
        private static double CalF166()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F166");
            if (row["TagValue"] == DBNull.Value)
            {
                double F165 = GetMeasureValue("F165");
                double F51 = CalF51();
                WASPCN.PS2H(F165, F51, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //中压缸效率
        /// <summary>
        /// 中压缸效率
        /// </summary>
        /// <returns></returns>
        private static double CalF167()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F167");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F52-F101)/(F52-F166)
                double F52 = CalF52();
                //double F101 = CalF101();
                double F174 = CalF174();
                double F166 = CalF166();
                dReturn = (F52 - F174) / (F52 - F166);
                if (dReturn > 1)
                    dReturn = 1;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //小机排放热量
        /// <summary>
        /// 小机排放热量
        /// </summary>
        /// <returns></returns>
        private static double CalF168()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F168");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F78-F131)*F75/3600
                double F78 = CalF78();
                double F131 = CalF131();
                double F75 = GetMeasureValue("F75");
                dReturn = (F78 - F131) * F75 / 3600;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //低压缸排气量2（低压缸进汽压力推算）
        /// <summary>
        /// 低压缸排气量2（低压缸进汽压力推算）
        /// </summary>
        /// <returns></returns>
        private static double CalF170()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F170");
            if (row["TagValue"] == DBNull.Value)
            {
                //1103.26*F111+20.905
                double F4 = GetMeasureValue("F4");
                double F111 = GetMeasureValue("F111");
                dReturn = 1103.26 * (F111 - F4) + 20.905;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //低压缸进汽焓
        /// <summary>
        /// 低压缸进汽焓
        /// </summary>
        /// <returns></returns>
        private static double CalF174()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F174");
            if (row["TagValue"] == DBNull.Value)
            {
                double F169 = GetMeasureValue("F169");
                double F173 = GetMeasureValue("F173");
                WASPCN.PT2H(F169, F173, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //低压缸进汽熵
        /// <summary>
        /// 低压缸进汽熵
        /// </summary>
        /// <returns></returns>
        private static double CalF175()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F175");
            if (row["TagValue"] == DBNull.Value)
            {
                double F169 = GetMeasureValue("F169");
                double F174 = CalF174();
                WASPCN.PH2S(F169, F174, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //排汽等熵焓
        /// <summary>
        /// 排汽等熵焓
        /// </summary>
        /// <returns></returns>
        private static double CalF176()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F176");
            if (row["TagValue"] == DBNull.Value)
            {
                double F126 = GetMeasureValue("F126") / 1000;
                double F175 = CalF175();
                WASPCN.PS2H(F126, F175, ref dReturn, ref Range);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //低压缸计算效率
        /// <summary>
        /// 低压缸计算效率
        /// </summary>
        /// <returns></returns>
        private static double CalF178()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F178");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F174-F180)/(F174-F176)
                double F174 = CalF174();
                double F180 = CalF180();
                double F176 = CalF176();
                dReturn = (F174 - F180) / (F174 - F176);
                if (dReturn > 1)
                    dReturn = 1;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //低压缸排汽排放热量（供热反算）
        /// <summary>
        /// 低压缸排汽排放热量（供热反算）
        /// </summary>
        /// <returns></returns>
        private static double CalF179()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F179");
            if (row["TagValue"] == DBNull.Value)
            {
                //F155-F168-F124*(F122-F131)/3600
                //double F155 = GetMeasureValue("F155");
                double F155 = CalF155();
                double F168 = CalF168();
                double F124 = CalF124();
                double F122 = CalF122();
                double F131 = CalF131();
                dReturn = F155 - F168 - F124 * (F122 - F131) / 3600;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        static double K(double F170)
        {
            double dReturn = 0;
            dReturn = -0.0001625 * F170 + 1.08875;
            return dReturn;
        }
        //供热反算排汽焓
        /// <summary>
        /// 供热反算排汽焓
        /// </summary>
        /// <returns></returns>
        public static double CalF180()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F180");
            if (row["TagValue"] == DBNull.Value)
            {
                //F179*3600/F157+F131
                //F179*3600/F170+F131
                double F126 = GetMeasureValue("F126") / 1000;
                double F170 = CalF170();
                WASPCN.P2HG(F126, ref dReturn, ref Range);
                dReturn = dReturn * K(F170);//
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //发电比
        /// <summary>
        /// 发电比
        /// </summary>
        /// <returns></returns>
        private static double CalF181()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F181");
            if (row["TagValue"] == DBNull.Value)
            {
                double F145 = GetMeasureValue("F145");
                double f139 = CalF139();
                //dReturn = F145 / f139;
                dReturn = F145 / f139 * 100;//乘以100，变成百分比
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //理论计算可达到的机组热耗
        /// <summary>
        /// 理论计算可达到的机组热耗
        /// </summary>
        /// <returns></returns>
        private static double CalF183()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F183");
            if (row["TagValue"] == DBNull.Value)
            {
                //3600*(F145+F143+F142+F141+F136+F182)/F145
                double F145 = GetMeasureValue("F145");
                double F143 = GetMeasureValue("F143");
                double F142 = GetMeasureValue("F142");
                double F141 = GetMeasureValue("F141");
                double F136 = CalF136();
                double F182 = GetMeasureValue("F182");
                dReturn = 3600 * (F145 + F143 + F142 + F141 + F136 + F182) / F145;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //实时测量计算机组热耗2
        /// <summary>
        /// 实时测量计算机组热耗2
        /// </summary>
        /// <returns></returns>
        private static double CalF184()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F184");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F139-F159)/F145*3600
                double F139 = CalF139();
                double F159 = CalF159();
                double F145 = GetMeasureValue("F145");
                dReturn = (F139 - F159) / F145 * 3600;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //实时测量发电煤耗
        /// <summary>
        /// 实时测量发电煤耗
        /// </summary>
        /// <returns></returns>
        private static double CalF186()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F186");
            if (row["TagValue"] == DBNull.Value)
            {
                //F184/(4.1868*7*F185)
                double F184 = CalF184();
                double F185 = GetMeasureValue("F185");
                dReturn = F184 / (4.1868 * 7 * F185);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //锅炉燃煤量
        /// <summary>
        /// 锅炉燃煤量
        /// </summary>
        /// <returns></returns>
        private static double CalF187()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F187");
            if (row["TagValue"] == DBNull.Value)
            {
                //F139*3600/(4.1868*7*F185*1000)
                double F139 = CalF139();
                double F185 = GetMeasureValue("F185");
                dReturn = F139 * 3600 / (4.1868 * 7 * F185 * 1000);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //总供热量
        /// <summary>
        /// 总供热量
        /// </summary>
        /// <returns></returns>
        private static double CalF188()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F188");
            if (row["TagValue"] == DBNull.Value)
            {
                double F159 = CalF159();
                dReturn = F159 * 3600 / 1000;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //供热耗煤量
        /// <summary>
        /// 供热耗煤量
        /// </summary>
        /// <returns></returns>
        private static double CalF189()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F189");
            if (row["TagValue"] == DBNull.Value)
            {
                //F159*3600/(F185*4.1868*7*1000)
                double F159 = CalF159();
                double F185 = GetMeasureValue("F185");
                dReturn = F159 * 3600 / (F185 * 4.1868 * 7 * 1000);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //供热煤耗
        /// <summary>
        /// 供热煤耗
        /// </summary>
        /// <returns></returns>
        private static double CalF190()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F190");
            if (row["TagValue"] == DBNull.Value)
            {
                //F189*1000/(F185*F188)
                double F189 = CalF189();
                double F185 = GetMeasureValue("F185");
                double F188 = CalF188();
                dReturn = F189 * 1000 / (F185 * F188);
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //发电耗煤量
        /// <summary>
        /// 发电耗煤量
        /// </summary>
        /// <returns></returns>
        private static double CalF191()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F191");
            if (row["TagValue"] == DBNull.Value)
            {
                //F145*F186/1000
                double F145 = GetMeasureValue("F145");
                double F186 = CalF186();
                dReturn = F145 * F186 / 1000;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //发电比
        /// <summary>
        /// 发电比
        /// </summary>
        /// <returns></returns>
        private static double CalF192()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F192");
            if (row["TagValue"] == DBNull.Value)
            {
                double F145 = GetMeasureValue("F145");
                double F139 = CalF139();
                dReturn = F145 / F139;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //public double CalF193()
        //{
        //    double dReturn = 0;
        //    DataRowView row = GetRow("F193");
        //    if (row["TagValue"] == DBNull.Value)
        //    {
        //        double F145 = GetMeasureValue("F145");
        //        double F139 = CalF139();
        //        dReturn = F145 / F139;
        //        SaveCalValue(dReturn, "", row);
        //    }
        //    else
        //        dReturn = (double)row["TagValue"];
        //    return dReturn;
        //}

        //年度供热开始本机组累计节煤量
        /// <summary>
        /// 年度供热开始本机组累计节煤量
        /// </summary>
        /// <returns></returns>
        private static double CalF195()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F195");
            if (row["TagValue"] == DBNull.Value)
            {
                dReturn = AmountOfCoalSaving.F195;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }

        //每30秒节煤量实时值
        /// <summary>
        /// 每30秒节煤量实时值
        /// </summary>
        /// <returns></returns>
        private static double CalF196()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F196");
            if (row["TagValue"] == DBNull.Value)
            {
                //F197*30/3600
                double F197 = CalF197();
                dReturn = F197 * 30 / 3600;
                SaveCalValue(dReturn, "", row);
                AmountOfCoalSaving.F196 = dReturn;
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //2015年度实时节煤煤耗值
        /// <summary>
        /// 2015年度实时节煤煤耗值
        /// </summary>
        /// <returns></returns>
        private static double CalF197()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F197");
            if (row["TagValue"] == DBNull.Value)
            {
                //(F194-F186)*F145/1000
                double F194 = GetMeasureValue("F194");
                double F186 = CalF186();
                double F145 = GetMeasureValue("F145");
                dReturn = (F194 - F186) * F145 / 1000;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //实时供热负荷1(瓦/平方米)
        /// <summary>
        /// 实时供热负荷1(瓦/平方米)
        /// </summary>
        /// <returns></returns>
        private static double CalF202()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F202");
            if (row["TagValue"] == DBNull.Value)
            {
                //F160/F201*100
                double F160 = CalF160();
                double F201 = GetMeasureValue("F201");
                dReturn = F160 / F201 * 100;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 实时供热负荷2(焦耳/平方米.小时)
        /// <summary>
        /// 实时供热负荷2(焦耳/平方米.小时)
        /// </summary>
        /// <returns></returns>
        private static double CalF203()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F203");
            if (row["TagValue"] == DBNull.Value)
            {
                double F204 = CalF204();
                dReturn = F204 * 4.1868;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 实时供热负荷3(大卡/平方米)
        /// <summary>
        /// 实时供热负荷3(大卡/平方米)
        /// </summary>
        /// <returns></returns>
        private static double CalF204()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F204");
            if (row["TagValue"] == DBNull.Value)
            {
                double F202 = CalF202();
                dReturn = F202 * 0.86;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //根据环境温度判断要求的热负荷
        /// <summary>
        /// 根据环境温度判断要求的热负荷
        /// </summary>
        /// <returns></returns>
        private static double CalF209()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F209");
            if (row["TagValue"] == DBNull.Value)
            {
                double F208 = GetMeasureValue("F208");
                //dReturn = 32 - F208;///////////
                dReturn = 52 - F208;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //增减负荷提示
        /// <summary>
        /// 增减负荷提示
        /// </summary>
        /// <returns></returns>
        private static double CalF210()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F210");
            if (row["TagValue"] == DBNull.Value)
            {
                double F202 = CalF252();
                double F219 = CalF219();
                dReturn = F202 - F219;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //理论要求的出水温度
        /// <summary>
        /// 理论要求的出水温度
        /// </summary>
        /// <returns></returns>
        private static double CalF213()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F213");
            if (row["TagValue"] == DBNull.Value)
            {
                double F208 = GetMeasureValue("F208");
                //dReturn = -2 * F208 + 75;////////////
                dReturn = -2 * F208 + 90;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //要求出水温度与实际出水温度之差
        /// <summary>
        /// 要求出水温度与实际出水温度之差
        /// </summary>
        /// <returns></returns>
        private static double CalF214()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F214");
            if (row["TagValue"] == DBNull.Value)
            {
                //double F152 = GetMeasureValue("F152");
                double F233 = GetMeasureValue("F233");//实际出水温度应该是母管的F233
                double F213 = CalF213();
                dReturn = F233 - F213;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 优化功率（热负荷）
        /// <summary>
        /// 优化功率（热负荷）
        /// </summary>
        /// <returns></returns>
        private static double CalF219()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F219");
            if (row["TagValue"] == DBNull.Value)
            {
                double F208 = GetMeasureValue("F208");
                //dReturn = -3 / 280 * F208 * F208 - 159 / 140 * F208 + 263 / 7;
                dReturn = -3 / 280 * F208 * F208 - 159 / 140 * F208 + 263 / 7 - 4;//宋高工让在原来的基础上再减2到3，14-11-15
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 燃料支出费用
        /// <summary>
        /// 燃料支出费用
        /// </summary>
        /// <returns></returns>
        private static double CalF225()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F225");
            if (row["TagValue"] == DBNull.Value)
            {
                double F187 = CalF187();
                double F198 = GetFixedValue("F198");
                dReturn = F187 * F198;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 上网电费收入
        /// <summary>
        /// 上网电费收入
        /// </summary>
        /// <returns></returns>
        private static double CalF226()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F226");
            if (row["TagValue"] == DBNull.Value)
            {
                double F199 = GetFixedValue("F199");
                double F224 = GetMeasureValue("F224");//////////
                dReturn = F199 * F224 / 10 * 10000;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //对外供热收入
        /// <summary>
        /// 对外供热收入
        /// </summary>
        /// <returns></returns>
        private static double CalF227()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F227");
            if (row["TagValue"] == DBNull.Value)
            {
                double F188 = CalF188();
                double F200 = GetFixedValue("F200");//////////
                dReturn = F188 * F200;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        //粗毛利润
        /// <summary>
        /// 粗毛利润
        /// </summary>
        /// <returns></returns>
        private static double CalF228()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F228");
            if (row["TagValue"] == DBNull.Value)
            {
                double F225 = CalF225();
                double F226 = CalF226();
                double F227 = CalF227();
                dReturn = F226 + F227 - F225;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 供热期电负荷累计
        /// <summary>
        /// 供热期电负荷累计
        /// </summary>
        /// <returns></returns>
        public static double CalF250()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F250");
            if (row["TagValue"] == DBNull.Value)
            {
                double F145 = GetMeasureValue("F145");
                double F250Last = 0;
                try { F250Last = (double)m_rowDataLast["F250"]; }
                catch
                {
                    try { F250Last = double.Parse(m_rowDataLast["F250"].ToString()); }
                    catch { }
                }
                dReturn = F250Last;
                if (F145 > 150)
                    dReturn = F250Last + F145;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 供热期发电煤耗累计
        /// <summary>
        /// 供热期发电煤耗累计
        /// </summary>
        /// <returns></returns>
        private static double CalF251()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F251");
            if (row["TagValue"] == DBNull.Value)
            {
                double F186 = CalF186();
                double F251Last = 0;
                try { F251Last = (double)m_rowDataLast["F251"]; }
                catch
                {
                    try { F251Last = double.Parse(m_rowDataLast["F251"].ToString()); }
                    catch { }
                }
                dReturn = F251Last;
                if (F186 > 130 && F186 < 200)
                    dReturn = F251Last + F186;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 实时热负荷
        /// <summary>
        /// 实时热负荷
        /// </summary>
        /// <returns></returns>
        private static double CalF252()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F252");
            if (row["TagValue"] == DBNull.Value)
            {
                //热网母管回水流量*（热网出水温度-回水温度）*4.1868/3600/供热面积
                double F161 = GetMeasureValue("F161");//热网供水实时流量（至外网）
                double F233 = GetMeasureValue("F233");//公用系统热网至外网供水温度
                double F150 = GetMeasureValue("F150");//热网回水至凝汽器循环进水温度
                double F201 = GetFixedValue("F201");//供暖面积
                //(F161 * (F151 - F150) + F161 * (F152 - F151)) * 4.1868 / 3600
                dReturn = F161 * (F233 - F150) * 4.1868 / 3600 / F201 * 100;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 热负荷累计
        /// <summary>
        /// 热负荷累计
        /// </summary>
        /// <returns></returns>
        private static double CalF253()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F253");
            if (row["TagValue"] == DBNull.Value)
            {
                double F202 = CalF202();
                double F253Last = 0;
                try { F253Last = (double)m_rowDataLast["F253"]; }
                catch
                {
                    try { F253Last = double.Parse(m_rowDataLast["F253"].ToString()); }
                    catch { }
                }
                dReturn = F253Last;
                if (F202 > 20 && F202 < 45)
                    dReturn = F253Last + F202;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 供热期电负荷累计次数
        /// <summary>
        /// 供热期电负荷累计次数
        /// </summary>
        /// <returns></returns>
        private static double CalF258()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F258");
            if (row["TagValue"] == DBNull.Value)
            {
                double F145 = GetMeasureValue("F145");//发电量
                double F258Last = 0;
                try { F258Last = (double)m_rowDataLast["F258"]; }
                catch
                {
                    try { F258Last = double.Parse(m_rowDataLast["F258"].ToString()); }
                    catch { }
                }
                dReturn = F258Last;
                if (F145 > 150)
                    dReturn = F258Last + 1;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 供热期发电煤耗累计次数
        /// <summary>
        /// 供热期发电煤耗累计次数
        /// </summary>
        /// <returns></returns>
        private static double CalF259()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F259");
            if (row["TagValue"] == DBNull.Value)
            {
                double F186 = CalF186();
                double F259Last = 0;
                try { F259Last = (double)m_rowDataLast["F259"]; }
                catch
                {
                    try { F259Last = double.Parse(m_rowDataLast["F259"].ToString()); }
                    catch { }
                }
                dReturn = F259Last;
                if (F186 > 130 && F186 < 200)
                    dReturn = F259Last + 1;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 热负荷累计次数
        /// <summary>
        /// 热负荷累计次数
        /// </summary>
        /// <returns></returns>
        private static double CalF260()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F260");
            if (row["TagValue"] == DBNull.Value)
            {
                double F202 = CalF202();
                double F260Last = 0;
                try { F260Last = (double)m_rowDataLast["F260"]; }
                catch
                {
                    try { F260Last = double.Parse(m_rowDataLast["F260"].ToString()); }
                    catch { }
                }
                dReturn = F260Last;
                if (F202 > 20 && F202 < 45)
                    dReturn = F260Last + 1;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 供热期平均电负荷
        /// <summary>
        /// 供热期平均电负荷
        /// </summary>
        /// <returns></returns>
        private static double CalF261()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F261");
            if (row["TagValue"] == DBNull.Value)
            {
                double F250 = CalF250();
                double F258 = CalF258();
                dReturn = F250 / F258;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 供热期平均发电煤耗
        /// <summary>
        /// 供热期平均发电煤耗
        /// </summary>
        /// <returns></returns>
        private static double CalF262()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F262");
            if (row["TagValue"] == DBNull.Value)
            {
                double F251 = CalF251();
                double F259 = CalF259();
                dReturn = F251 / F259;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 平均热负荷
        /// <summary>
        /// 平均热负荷
        /// </summary>
        /// <returns></returns>
        private static double CalF263()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F263");
            if (row["TagValue"] == DBNull.Value)
            {
                double F253 = CalF253();
                double F260 = CalF260();
                dReturn = F253 / F260;
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }
        // 小机排汽量
        /// <summary>
        /// 小机排汽量
        /// </summary>
        /// <returns></returns>
        private static double CalF292()
        {
            double dReturn = 0;
            DataRowView row = GetRow("F292");
            if (row["TagValue"] == DBNull.Value)
            {
                dReturn = GetMeasureValue("F75");
                SaveCalValue(dReturn, "", row);
            }
            else
                dReturn = (double)row["TagValue"];
            return dReturn;
        }

        #endregion

        // 每小时总计汇总所有数据
        /// <summary>
        /// 每小时总计汇总所有数据
        /// </summary>
        void TotalizeAllDataOfHour()
        {
            if (m_TotalTimeHour.Hour < DateTime.Now.Hour)
            {
                bool bIsStopToStart = false;//程序是否是停了一段时间后，又启动。
                if (m_TotalTimeHour.Hour < DateTime.Now.Hour - 1)//程序关闭一段时间后，又启动的情况
                {
                    bIsStopToStart = true;
                }

                m_rowTotalDataCurrent = m_dtTotalTagDatas.NewRow();
                m_rowTotalDataCurrent["DateHour"] = long.Parse(m_TotalTimeHour.ToString("yyyyMMddHH"));
                m_rowTotalDataCurrent["HourOfYear"] = GetHourOfYear();
                if (m_viewTotalTags == null || TableTotalTags.isFilled)
                    m_viewTotalTags = TableTotalTags.getAllData().DefaultView;
                m_viewTotalTags.RowFilter = "DataSourcesNo=4";//“测点积分”计算
                int i = 0;
                int iTotalMinute = GlobalVariables.TotalTimeLong;
                double dCurValue = 0;//当前数据值
                double dAllSum = 0;//最后计算的所有参数的累计，用于判断是否（停机）往数据库里插入数据
                double[] dOperation2 = new double[m_viewTotalTags.Count];//保存第二个操作数
                double[] dSums = new double[m_viewTotalTags.Count];//保存数据的累计值
                double[] dValues = new double[m_viewTotalTags.Count];//保存计算好的值
                DateTime timeData;
                int[] iCount = new int[m_viewTotalTags.Count];//保存累加数的个数
                string[][] sColumns = new string[m_viewTotalTags.Count][];//保存小时内计算的列名称
                while (m_TotalTimeHour.Hour < DateTime.Now.Hour)
                {
                    //如果是程序停止一段时间后重启，则从数据库里取数据；否则，默认用定时器每几秒生成的数据
                    if (bIsStopToStart)
                    {
                        string sSql = "select " + m_sWillTotalTagFields + " " + GetTableName(m_TotalTimeHour)
                            + " where ValueTime>='" + m_TotalTimeHour.ToString("yyyy-MM-dd HH:00:00")
                            + "' and ValueTime<='" + m_TotalTimeHour.ToString("yyyy-MM-dd HH:59:59") + "'";
                        try
                        {
                            m_dtWillTotalTagDatas = SQLHelper.ExecuteDt(sSql);
                            if (m_dtWillTotalTagDatas.Rows.Count == 0)
                            {
                                m_TotalTimeHour = m_TotalTimeHour.AddHours(1);
                                continue;
                            }
                        }
                        catch
                        {
                            m_TotalTimeHour = m_TotalTimeHour.AddHours(1);
                            continue;
                        }
                    }

                    //初始化
                    iTotalMinute = GlobalVariables.TotalTimeLong;
                    for (i = 0; i < dSums.Length; i++)
                    {
                        dSums[i] = 0;
                        iCount[i] = 0;
                        dValues[i] = 0;
                        sColumns[i] = m_viewTotalTags[i]["ExcelCell"].ToString().Split('*');
                        dOperation2[i] = 0;
                        try { dOperation2[i] = GetFixedValue(sColumns[i][1]); }
                        catch { }
                    }
                    //计算
                    foreach (DataRow row in m_dtWillTotalTagDatas.Rows)
                    {
                        timeData = DateTime.Parse(row["ValueTime"].ToString());
                        if (timeData.Hour == m_TotalTimeHour.Hour)
                        {
                            if (timeData.Minute > iTotalMinute)
                            {
                                //计算、累计各参数
                                for (i = 0; i < m_viewTotalTags.Count; i++)
                                {
                                    if (dSums[i] > 0.01)
                                    {
                                        dValues[i] += dSums[i] / iCount[i] * dOperation2[i];
                                    }
                                    dSums[i] = 0;
                                    iCount[i] = 0;
                                }
                                //设置下一次计算的分钟数
                                iTotalMinute += GlobalVariables.TotalTimeLong;
                            }
                        }
                        else
                        {
                            break;
                        }
                        //累计每次的数据
                        for (i = 0; i < m_viewTotalTags.Count; i++)
                        {
                            dCurValue = double.Parse(row[sColumns[i][0]].ToString());
                            if (dCurValue > 0.01)
                            {
                                dSums[i] += dCurValue;
                                iCount[i] += 1;
                            }
                        }
                    }
                    //计算最后一个分钟段的数据值，保存
                    dAllSum = 0;
                    if (iTotalMinute > GlobalVariables.TotalTimeLong)
                    {
                        for (i = 0; i < m_viewTotalTags.Count; i++)
                        {
                            //计算最后一个分钟段的数据值
                            if (dSums[i] > 0.01)
                            {
                                dValues[i] += dSums[i] / iCount[i] * dOperation2[i];
                            }
                            //保存
                            m_rowTotalDataCurrent["F" + m_viewTotalTags[i]["id"]] = dValues[i];
                            dAllSum += dValues[i];
                        }

                    }

                    //做其他汇总计算
                    if (dAllSum > 0.1)
                    {
                        //计算
                        //调用本类中汇总计算数据的函数，如Total7()、Total10()等，最后得到m_rowTotalDataCurrent中每列里计算好的结果
                        m_viewTotalTags.RowFilter = "DataSourcesNo=5";
                        MethodInfo mi = null;
                        System.Type type = typeof(CalTagValue);
                        foreach (DataRowView row in m_viewTotalTags)
                        {
                            if (m_rowTotalDataCurrent["F" + row["id"].ToString()] == DBNull.Value)
                            {
                                mi = type.GetMethod("Total" + row["id"].ToString(), BindingFlags.NonPublic | BindingFlags.Static);
                                if (mi != null)
                                    mi.Invoke(null, null);
                            }
                        }

                        //插入
                        double dTemp;
                        string sValue = "";
                        StringBuilder sbFields = new StringBuilder("DateHour,HourOfYear");
                        StringBuilder sbValue = new StringBuilder(m_rowTotalDataCurrent["DateHour"].ToString() + ","
                            + m_rowTotalDataCurrent["HourOfYear"].ToString());
                        foreach (DataRowView row in m_viewTotalTags)
                        {
                            try
                            {
                                sValue = m_rowTotalDataCurrent["F" + row["id"].ToString()].ToString();
                                if (sValue != "非数字" & sValue != "正无穷大" & sValue != "负无穷大")
                                {
                                    dTemp = double.Parse(sValue);
                                    dTemp = Math.Round(dTemp, 4);
                                    sbFields.Append(",F" + row["id"]);
                                    sbValue.Append("," + dTemp);
                                }
                            }
                            catch
                            { dTemp = 0; }
                        }
                        SQLHelper.ExecuteSql("insert into " + GlobalVariables.CurrentTotalDataTable + "(" + sbFields.ToString() + ") values(" +
                            sbValue.ToString() + ")");
                    }

                    m_rowTotalDataLast = m_rowTotalDataCurrent;

                    if (bIsStopToStart)
                        m_TotalTimeHour = m_TotalTimeHour.AddHours(1);
                    else
                        m_TotalTimeHour = DateTime.Now;
                }
            }
        }
        /// <summary>
        /// 累计汇总初始化
        /// </summary>
        static void TotalInit()
        {
            int iTimeDiff = 100;//时间差，单位是秒，如果两行数据的时间差超过这个设定的秒数（20），就认为程序停止后又重新启动。
            DateTime timeStart = DateTime.Now.AddSeconds(-GlobalVariables.RefIntvel / 1000);
            m_TotalTimeBegin = timeStart.AddMinutes(GlobalVariables.TotalTimeLong);
            m_TotalTimeEnd = m_TotalTimeBegin.AddMinutes(GlobalVariables.TotalTimeLong);
            m_dtWillTotalTagDatas.Columns.Add("ValueTime");

            //SetTotalBeginEndTime();
            SetTagsView();

            if (m_dtTotalTagDatas.Rows.Count > 0)
            {
                m_rowTotalDataLast = m_dtTotalTagDatas.Rows[m_dtTotalTagDatas.Rows.Count - 1];
                DateTime timeCurrent = DateTime.Now;//保存当前数据时间
                DateTime timeStop = DateTime.Parse(m_rowTotalDataLast["ValueTime"].ToString()); //保存上次程序结束的时间
                DateTime timeLast = timeStop;//保存上次数据时间
                string sSql = "select " + m_sWillTotalTagFields + " from " + GlobalVariables.CurrentTable + " where ValueTime>='"
                    + timeLast + "' and ValueTime<='" + timeStart + "'";
                try
                {
                    DataTable dt = SQLHelper.ExecuteDt(sSql);
                    if (dt.Rows.Count > 1)
                    {
                        m_rowTotalDataCurrent = m_dtTotalTagDatas.NewRow();
                        m_rowTotalDataCurrent["ValueTime"] = m_TotalTimeBegin.AddMinutes(-GlobalVariables.TotalTimeLong);
                        if (m_viewTotalTags == null || TableTotalTags.isFilled)
                            m_viewTotalTags = TableTotalTags.getAllData().DefaultView;
                        m_viewTotalTags.RowFilter = "DataSourcesNo=4";//“测点积分”计算
                        int i;
                        double dCurValue = 0;//当前数据值
                        double dAllSum = 0;//最后计算的所有参数的累计，用于判断是否（停机）往数据库里插入数据
                        double dTotalMinutes = 0;//保存上次程序结束后，多长分钟没有统计
                        int[] iCount = new int[m_viewTotalTags.Count];//保存累加数的个数
                        double[] dOperation2 = new double[m_viewTotalTags.Count];//保存第二个操作数
                        double[] dSums = new double[m_viewTotalTags.Count];//保存数据的累计值
                        double[] dValues = new double[m_viewTotalTags.Count];//保存计算好的值
                        string[][] sColumns = new string[m_viewTotalTags.Count][];//保存小时内计算的列名称
                        //初始化
                        for (i = 0; i < dSums.Length; i++)
                        {
                            dSums[i] = 0;
                            iCount[i] = 0;
                            sColumns[i] = m_viewTotalTags[i]["ExcelCell"].ToString().Split('*');
                            dOperation2[i] = 0;
                            dValues[i] = 0;
                            try { dOperation2[i] = GetFixedValue(sColumns[i][1]); }
                            catch { }
                        }
                        foreach (DataRow row in dt.Rows)
                        {
                            timeCurrent = DateTime.Parse(row["ValueTime"].ToString());
                            if ((timeCurrent - timeLast).TotalSeconds > iTimeDiff)
                            {
                                m_rowTotalDataCurrent["ValueTime"] = timeLast;
                                timeCurrent = timeLast;
                                break;
                            }
                            //累计每次的数据
                            for (i = 0; i < m_viewTotalTags.Count; i++)
                            {
                                try
                                {
                                    //WriteLog.WriteLogs(sColumns[i][0] + ":" + row[sColumns[i][0]].ToString());
                                    dCurValue = double.Parse(row[sColumns[i][0]].ToString());

                                    if (dCurValue > 0.01)
                                    {
                                        dSums[i] += dCurValue;
                                        iCount[i] += 1;
                                    }
                                }
                                catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
                            }
                            timeLast = timeCurrent;
                        }
                        dTotalMinutes = (timeCurrent - timeStop).TotalMinutes;
                        m_rowTotalDataCurrent["HourOfYear"] = GetHoursOfYear(dTotalMinutes);
                        m_dtWillTotalTagDatas.DefaultView.RowFilter = "ValueTime>='" + m_TotalTimeBegin.AddMinutes(-GlobalVariables.TotalTimeLong) + "'";
                        m_dtWillTotalTagDatas = m_dtWillTotalTagDatas.DefaultView.ToTable();
                        //计算数据值，保存
                        dAllSum = 0;
                        for (i = 0; i < m_viewTotalTags.Count; i++)
                        {
                            //计算数据值
                            if (dSums[i] > 0.01)
                            {
                                dValues[i] += dSums[i] / iCount[i] * dOperation2[i] * dTotalMinutes / 60;
                                //WriteLog.WriteLogs("dValues,F" + m_viewTotalTags[i]["id"] + ":" + dValues[i]);
                            }
                            //保存
                            m_rowTotalDataCurrent["F" + m_viewTotalTags[i]["id"]] = dValues[i];
                            //WriteLog.WriteLogs("rowF" + m_viewTotalTags[i]["id"] + ":" + m_rowTotalDataCurrent["F" + m_viewTotalTags[i]["id"]]);
                            dAllSum += dValues[i];
                        }

                        //做其他汇总计算
                        if (dAllSum > 0.1)
                        {
                            //计算
                            //调用本类中汇总计算数据的函数，如Total7()、Total10()等，最后得到m_rowTotalDataCurrent中每列里计算好的结果
                            m_viewTotalTags.RowFilter = "DataSourcesNo=5";
                            MethodInfo mi = null;
                            System.Type type = typeof(CalTagValue);
                            foreach (DataRowView row in m_viewTotalTags)
                            {
                                //WriteLog.WriteLogs((m_rowTotalDataCurrent["F" + row["id"].ToString()] == DBNull.Value).ToString());
                                if (m_rowTotalDataCurrent["F" + row["id"].ToString()] == DBNull.Value)
                                {
                                    mi = type.GetMethod("Total" + row["id"].ToString(), BindingFlags.NonPublic | BindingFlags.Static);
                                    if (mi != null)
                                    {
                                        mi.Invoke(null, null);
                                        //WriteLog.WriteLogs("Total" + row["id"].ToString());
                                    }
                                }
                            }

                            //插入
                            double dTemp;
                            string sValue = "";
                            StringBuilder sbFields = new StringBuilder("ValueTime,HourOfYear");
                            StringBuilder sbValue = new StringBuilder("'" + m_rowTotalDataCurrent["ValueTime"].ToString() + "',"
                                + m_rowTotalDataCurrent["HourOfYear"].ToString());
                            m_viewTotalTags.RowFilter = "";
                            foreach (DataRowView row in m_viewTotalTags)
                            {
                                try
                                {
                                    sValue = m_rowTotalDataCurrent["F" + row["id"].ToString()].ToString();
                                    if (sValue != "非数字" & sValue != "正无穷大" & sValue != "负无穷大")
                                    {
                                        dTemp = double.Parse(sValue);
                                        dTemp = Math.Round(dTemp, 4);
                                        sbFields.Append(",F" + row["id"]);
                                        sbValue.Append("," + dTemp);
                                    }
                                }
                                catch
                                { dTemp = 0; }
                            }
                            SQLHelper.ExecuteSql("insert into " + GlobalVariables.CurrentTotalDataTable + "(" + sbFields.ToString() + ") values(" +
                                sbValue.ToString() + ")");
                            m_TotalCount += 1;
                            m_rowTotalDataLast = m_rowTotalDataCurrent;
                            //WriteLog.WriteLogs("e:"+m_TotalTimeBegin.ToString() + " ; " + m_TotalTimeEnd.ToString());
                        }

                    }
                }
                catch (Exception ex)
                {
                    WriteLog.WriteLogs(ex.ToString());
                }

            }

        }
        // 总计汇总所有数据
        /// <summary>
        /// 总计汇总所有数据
        /// </summary>
        public static void TotalizeAllData()
        {
            //SetTotalBeginEndTime();
            if (m_TotalTimeBegin <= DateTime.Now && DateTime.Now <= m_TotalTimeEnd)
            {
                m_rowTotalDataCurrent = m_dtTotalTagDatas.NewRow();
                m_rowTotalDataCurrent["ValueTime"] = m_TotalTimeBegin;
                m_rowTotalDataCurrent["HourOfYear"] = GetHoursOfYear();
                if (m_viewTotalTags == null || TableTotalTags.isFilled)
                    m_viewTotalTags = TableTotalTags.getAllData().DefaultView;
                m_viewTotalTags.RowFilter = "DataSourcesNo=4";//“测点积分”计算
                int i = 0;
                double dCurValue = 0;//当前数据值
                double dAllSum = 0;//最后计算的所有参数的累计，用于判断是否（停机）往数据库里插入数据
                double[] dOperation2 = new double[m_viewTotalTags.Count];//保存第二个操作数
                double[] dSums = new double[m_viewTotalTags.Count];//保存数据的累计值
                double[] dValues = new double[m_viewTotalTags.Count];//保存计算好的值
                DateTime timeData;
                int[] iCount = new int[m_viewTotalTags.Count];//保存累加数的个数
                string[][] sColumns = new string[m_viewTotalTags.Count][];//保存小时内计算的列名称
                //初始化
                for (i = 0; i < dSums.Length; i++)
                {
                    dSums[i] = 0;
                    iCount[i] = 0;
                    dValues[i] = 0;
                    sColumns[i] = m_viewTotalTags[i]["ExcelCell"].ToString().Split('*');
                    dOperation2[i] = 0;
                    try { dOperation2[i] = GetFixedValue(sColumns[i][1]); }
                    catch { }
                }
                m_TotalTimeBegin = m_TotalTimeBegin.AddMinutes(-GlobalVariables.TotalTimeLong);
                m_TotalTimeEnd = m_TotalTimeEnd.AddMinutes(-GlobalVariables.TotalTimeLong);
                try
                {
                    //计算
                    foreach (DataRow row in m_dtWillTotalTagDatas.Rows)
                    {
                        timeData = DateTime.Parse(row["ValueTime"].ToString());
                        if (m_TotalTimeBegin <= timeData && timeData <= m_TotalTimeEnd)
                        {
                            //累计每次的数据
                            for (i = 0; i < m_viewTotalTags.Count; i++)
                            {
                                try
                                {
                                    //WriteLog.WriteLogs(sColumns[i][0] + ":" + row[sColumns[i][0]].ToString());
                                    dCurValue = double.Parse(row[sColumns[i][0]].ToString());

                                    if (dCurValue > 0.01)
                                    {
                                        dSums[i] += dCurValue;
                                        iCount[i] += 1;
                                    }
                                }
                                catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
                            }
                        }
                    }
                    m_dtWillTotalTagDatas.DefaultView.RowFilter = "ValueTime>='" + m_TotalTimeEnd + "'";
                    m_dtWillTotalTagDatas = m_dtWillTotalTagDatas.DefaultView.ToTable();
                    //计算数据值，保存
                    dAllSum = 0;
                    for (i = 0; i < m_viewTotalTags.Count; i++)
                    {
                        //计算数据值
                        if (dSums[i] > 0.01)
                        {
                            dValues[i] += dSums[i] / iCount[i] * dOperation2[i] * GlobalVariables.TotalTimeLong / 60;
                            //WriteLog.WriteLogs("dValues,F" + m_viewTotalTags[i]["id"] + ":" + dValues[i]);
                        }
                        //保存
                        m_rowTotalDataCurrent["F" + m_viewTotalTags[i]["id"]] = dValues[i];
                        //WriteLog.WriteLogs("rowF" + m_viewTotalTags[i]["id"] + ":" + m_rowTotalDataCurrent["F" + m_viewTotalTags[i]["id"]]);
                        dAllSum += dValues[i];
                    }

                    //做其他汇总计算
                    if (dAllSum > 0.1)
                    {
                        //计算
                        //调用本类中汇总计算数据的函数，如Total7()、Total10()等，最后得到m_rowTotalDataCurrent中每列里计算好的结果
                        m_viewTotalTags.RowFilter = "DataSourcesNo=5";
                        MethodInfo mi = null;
                        System.Type type = typeof(CalTagValue);
                        foreach (DataRowView row in m_viewTotalTags)
                        {
                            //WriteLog.WriteLogs((m_rowTotalDataCurrent["F" + row["id"].ToString()] == DBNull.Value).ToString());
                            if (m_rowTotalDataCurrent["F" + row["id"].ToString()] == DBNull.Value)
                            {
                                mi = type.GetMethod("Total" + row["id"].ToString(), BindingFlags.NonPublic | BindingFlags.Static);
                                if (mi != null)
                                {
                                    mi.Invoke(null, null);
                                    //WriteLog.WriteLogs("Total" + row["id"].ToString());
                                }
                            }
                        }

                        //插入
                        double dTemp;
                        string sValue = "";
                        StringBuilder sbFields = new StringBuilder("ValueTime,HourOfYear");
                        StringBuilder sbValue = new StringBuilder("'" + m_rowTotalDataCurrent["ValueTime"].ToString() + "',"
                            + m_rowTotalDataCurrent["HourOfYear"].ToString());
                        m_viewTotalTags.RowFilter = "";
                        foreach (DataRowView row in m_viewTotalTags)
                        {
                            try
                            {
                                sValue = m_rowTotalDataCurrent["F" + row["id"].ToString()].ToString();
                                if (sValue != "非数字" & sValue != "正无穷大" & sValue != "负无穷大")
                                {
                                    dTemp = double.Parse(sValue);
                                    dTemp = Math.Round(dTemp, 4);
                                    sbFields.Append(",F" + row["id"]);
                                    sbValue.Append("," + dTemp);
                                    //添加往PI里回写数据的值
                                    if (dTemp > 0 & row["WriteBackTag"].ToString().Length > 1)
                                    {
                                        lstPINames.Add(row["WriteBackTag"].ToString());
                                        lstPIValues.Add(dTemp.ToString());
                                    }
                                }
                            }
                            catch
                            { dTemp = 0; }
                        }
                        try
                        {
                            SQLHelper.ExecuteSql("insert into " + GlobalVariables.CurrentTotalDataTable + "(" + sbFields.ToString() + ") values(" +
                                sbValue.ToString() + ")");

                            m_rowTotalDataLast = m_rowTotalDataCurrent;
                            m_dtTotalTagDatas.Rows.Add(m_rowTotalDataCurrent);
                            if (m_dtTotalTagDatas.Rows.Count > m_iTotalRowCount)
                                m_dtTotalTagDatas.Rows.RemoveAt(0);
                            m_TotalTimeBegin = m_TotalTimeBegin.AddMinutes(GlobalVariables.TotalTimeLong * 2);
                            m_TotalTimeEnd = m_TotalTimeEnd.AddMinutes(GlobalVariables.TotalTimeLong * 2);
                            SetTotalBeginEndTime();
                            m_TotalCount += 1;
                        }
                        catch (Exception ex)
                        {
                            m_TotalTimeBegin = m_TotalTimeBegin.AddMinutes(GlobalVariables.TotalTimeLong);
                            m_TotalTimeEnd = m_TotalTimeEnd.AddMinutes(GlobalVariables.TotalTimeLong);
                            WriteLog.WriteLogs(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        WriteLog.WriteLogs(ex.ToString());
                        TotalInit();
                        SetTotalBeginEndTime();
                    }
                    catch { }
                }
            }
        }
        /// <summary>
        /// 获取供热期的第几个小时
        /// </summary>
        /// <returns></returns>
        int GetHourOfYear()
        {
            int iHourOfYear = 1;
            try
            {
                iHourOfYear = (int)m_rowTotalDataLast["HourOfYear"];
                long lLastDateHour = (long)m_rowTotalDataLast["DateHour"];
                long lThisDateHour = (long)m_rowTotalDataCurrent["DateHour"];
                iHourOfYear = (int)(iHourOfYear + (lThisDateHour - lLastDateHour));
            }
            catch { }
            return iHourOfYear;
        }
        /// <summary>
        /// 获取供热期的累计小时
        /// </summary>
        /// <returns></returns>
        static double GetHoursOfYear(double dTotalMinutes = 0)
        {
            double dHoursOfYear = GlobalVariables.TotalTimeLong / 60f;
            if (dTotalMinutes > 0.0001)//有参数传入，初始化时的累计时间
            {
                dHoursOfYear = dTotalMinutes;
                try
                {
                    dHoursOfYear = double.Parse(m_rowTotalDataLast["HourOfYear"].ToString());
                    dHoursOfYear += dTotalMinutes / 60;
                }
                catch { }
            }
            else//无参数传入，中间计算的累计时间
            {
                try
                {
                    dHoursOfYear = double.Parse(m_rowTotalDataLast["HourOfYear"].ToString());
                    DateTime timeBegin = DateTime.Parse(m_rowTotalDataLast["ValueTime"].ToString());
                    DateTime timeEnd = DateTime.Parse(m_rowTotalDataCurrent["ValueTime"].ToString());
                    dHoursOfYear += (timeEnd - timeBegin).TotalHours;
                }
                catch { }
            }
            return Math.Round(dHoursOfYear, 4);
        }
        /// <summary>
        /// 设置总计的开始、结束时间
        /// </summary>
        static void SetTotalBeginEndTime()
        {
            DateTime time = DateTime.Now.AddMinutes(GlobalVariables.TotalTimeLong);
            if (time < m_TotalTimeBegin || time > m_TotalTimeEnd)
            {
                m_TotalTimeBegin = time;
                m_TotalTimeEnd = time.AddMinutes(GlobalVariables.TotalTimeLong);
            }

        }
        // 获取累计汇总数据的副本，与源数据不是同一对象
        /// <summary>
        /// 获取累计汇总数据的副本，与源数据不是同一对象
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllTotalDatas()
        {
            return m_dtTotalTagDatas.DefaultView.ToTable();
        }
        // 获取累计汇总的次数
        /// <summary>
        /// 获取累计汇总数据的行数
        /// </summary>
        /// <returns></returns>
        public static long GetTotalCount()
        {
            return m_TotalCount;
        }

        #region 各统计数据的计算方法
        //供热收入累计
        /// <summary>
        /// 供热收入累计
        /// </summary>
        private static void Total2()
        {
            double dF1 = 0;//供热收入
            try
            {
                dF1 = double.Parse(m_rowTotalDataCurrent["F1"].ToString()) / 10000;
            }
            catch { }
            double dF2 = 0;//供热收入累计
            try { dF2 = double.Parse(m_rowTotalDataLast["F2"].ToString()); }
            catch { }
            m_rowTotalDataCurrent["F2"] = dF2 + dF1;
        }
        //供热收入累计次数
        /// <summary>
        /// 供热收入累计次数
        /// </summary>
        private static void Total3()
        {
            double dF1 = 0;//供热收入
            try { dF1 = double.Parse(m_rowTotalDataCurrent["F1"].ToString()); }
            catch { }
            double dF3 = 0;//供热收入累计次数
            try { dF3 = double.Parse(m_rowTotalDataLast["F3"].ToString()); }
            catch { }
            if (dF1 > 0.01)
                dF3 += 1;
            m_rowTotalDataCurrent["F3"] = dF3;
        }
        //供热收入平均
        /// <summary>
        /// 供热收入平均
        /// </summary>
        private static void Total4()
        {
            double dF2 = 0;//供热收入累计
            try { dF2 = double.Parse(m_rowTotalDataCurrent["F2"].ToString()); }
            catch { }
            double dHourOfYear = GlobalVariables.TotalTimeLong / 60f;//供热期累计时间
            try { dHourOfYear = double.Parse(m_rowTotalDataCurrent["HourOfYear"].ToString()); }
            catch { }
            m_rowTotalDataCurrent["F4"] = dF2 / dHourOfYear * 10000;
        }
        //供热期发电收入累计
        /// <summary>
        /// 供热期发电收入累计
        /// </summary>
        private static void Total6()
        {
            double dF5 = 0;//供热期发电收入
            try
            {
                dF5 = double.Parse(m_rowTotalDataCurrent["F5"].ToString());
                m_rowTotalDataCurrent["F5"] = dF5 * 1000;//把发电收入换算成元，然后重新赋值
                dF5 = dF5 / 10;//把发电收入换算成万元，相当于计算公式：dF5 * 1000/10000
            }
            catch { }
            double dF6 = 0;//供热期发电收入累计
            try { dF6 = double.Parse(m_rowTotalDataLast["F6"].ToString()); }
            catch { }
            m_rowTotalDataCurrent["F6"] = dF6 + dF5;
        }
        //供热期发电收入累计次数
        /// <summary>
        /// 供热期发电收入累计次数
        /// </summary>
        private static void Total7()
        {
            double dF5 = 0;//供热期发电收入
            try { dF5 = double.Parse(m_rowTotalDataCurrent["F5"].ToString()); }
            catch { }
            double dF7 = 0;//供热期发电收入累计次数
            try { dF7 = double.Parse(m_rowTotalDataLast["F7"].ToString()); }
            catch { }
            if (dF5 > 0.01)
                dF7 += 1;
            m_rowTotalDataCurrent["F7"] = dF7;
        }
        //供热期发电收入平均
        /// <summary>
        /// 供热期发电收入平均
        /// </summary>
        private static void Total8()
        {
            double dF6 = 0;//供热期发电收入累计
            try { dF6 = double.Parse(m_rowTotalDataCurrent["F6"].ToString()); }
            catch { }
            double dHourOfYear = GlobalVariables.TotalTimeLong / 60d;//供热期累计时间
            try { dHourOfYear = double.Parse(m_rowTotalDataCurrent["HourOfYear"].ToString()); }
            catch { }
            m_rowTotalDataCurrent["F8"] = dF6 / dHourOfYear * 10000;
        }
        //燃料支出费用累计
        /// <summary>
        /// 燃料支出费用累计
        /// </summary>
        private static void Total10()
        {
            double dF9 = 0;//燃料支出费用
            try
            {
                dF9 = double.Parse(m_rowTotalDataCurrent["F9"].ToString());
                dF9 = dF9 / 10000;//换算成万元
                //WriteLog.WriteLogs("dF9 / 10000:" + dF9);
            }
            catch { }
            double dF10 = 0;//燃料支出费用累计
            try { dF10 = double.Parse(m_rowTotalDataLast["F10"].ToString()); }
            catch { }
            //WriteLog.WriteLogs("dF10:" + dF10 + " ; dF10 + dF9:" + (dF10 + dF9));
            m_rowTotalDataCurrent["F10"] = dF10 + dF9;
        }
        //燃料支出费用累计次数
        /// <summary>
        /// 燃料支出费用累计次数
        /// </summary>
        private static void Total11()
        {
            double dF9 = 0;//燃料支出费用
            try { dF9 = double.Parse(m_rowTotalDataCurrent["F9"].ToString()); }
            catch { }
            double dF11 = 0;//供热期发电收入累计次数
            try { dF11 = double.Parse(m_rowTotalDataLast["F11"].ToString()); }
            catch { }
            if (dF9 > 0.01)
                dF11 += 1;
            m_rowTotalDataCurrent["F11"] = dF11;
        }
        //燃料支出费用平均
        /// <summary>
        /// 燃料支出费用平均
        /// </summary>
        private static void Total12()
        {
            double dF10 = 0;//供热期发电收入累计
            try { dF10 = double.Parse(m_rowTotalDataCurrent["F10"].ToString()); }
            catch { }
            double dHourOfYear = GlobalVariables.TotalTimeLong / 60f;//供热期累计时间
            try { dHourOfYear = double.Parse(m_rowTotalDataCurrent["HourOfYear"].ToString()); }
            catch { }
            m_rowTotalDataCurrent["F12"] = dF10 / dHourOfYear * 10000;
        }
        //毛利润
        /// <summary>
        /// 毛利润
        /// </summary>
        private static void Total13()
        {
            double dF5 = 0;//供热期发电收入
            try { dF5 = double.Parse(m_rowTotalDataCurrent["F5"].ToString()); }
            catch { }
            double dF1 = 0;//供热收入
            try { dF1 = double.Parse(m_rowTotalDataCurrent["F1"].ToString()); }
            catch { }
            double dF9 = 0;//燃料支出费用
            try { dF9 = double.Parse(m_rowTotalDataCurrent["F9"].ToString()); }
            catch { }
            m_rowTotalDataCurrent["F13"] = dF5 + dF1 - dF9;
        }
        //毛利润累计
        /// <summary>
        /// 毛利润累计
        /// </summary>
        private static void Total14()
        {
            double dF13 = 0;//毛利润
            try
            {
                dF13 = double.Parse(m_rowTotalDataCurrent["F13"].ToString());
                dF13 = dF13 / 10000;//换算成万元
            }
            catch { }
            double dF14 = 0;//上次毛利润累计
            try { dF14 = double.Parse(m_rowTotalDataLast["F14"].ToString()); }
            catch { }
            m_rowTotalDataCurrent["F14"] = dF13 + dF14;
        }
        //毛利润累计次数
        /// <summary>
        /// 毛利润累计次数
        /// </summary>
        private static void Total15()
        {
            double dF13 = 0;//毛利润
            try { dF13 = double.Parse(m_rowTotalDataCurrent["F13"].ToString()); }
            catch { }
            double dF15 = 0;//上次毛利润累计次数
            try { dF15 = double.Parse(m_rowTotalDataLast["F15"].ToString()); }
            catch { }
            if (dF13 > 0.01)
                dF15 += 1;
            m_rowTotalDataCurrent["F15"] = dF15;
        }
        //毛利润平均
        /// <summary>
        /// 毛利润平均
        /// </summary>
        private static void Total16()
        {
            double dF14 = 0;//毛利润累计
            try { dF14 = double.Parse(m_rowTotalDataCurrent["F14"].ToString()); }
            catch { }
            double dHourOfYear = GlobalVariables.TotalTimeLong / 60f;//供热期累计时间
            try { dHourOfYear = double.Parse(m_rowTotalDataCurrent["HourOfYear"].ToString()); }
            catch { }
            m_rowTotalDataCurrent["F16"] = dF14 / dHourOfYear * 10000;//乘以10000是为了再换算成万元
        }
        #endregion
    }

}
