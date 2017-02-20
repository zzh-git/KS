using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using CJPrj.PI;
using YHYXYH.PI;

namespace CJPrj.Cls
{
    /*  * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    *   文 件 名:CalTree
    *   功能描述:遍历解析公式的类,公式中包含以下类型公式：1、直接从hashtable中取到的数；2、焓熵公式计算的数；3、简单的加减乘除和平方开方
    *   创 建 人:张传昀
    *   日    期:2014-5-20
    *   * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
    public class CalTree
    {
        static bool m_IsFirstRun = true;//是不是第一次运行。为防止第一次启动较慢或者起不来，第一次运行不取PI的值，使用默认数据。
        DateTime timeGCCollect = DateTime.Now;//系统垃圾回收时间
        DateTime time1GMemoryCollect = DateTime.Now;//程序内在到1G时的垃圾回收时间
        System.Timers.Timer t = new System.Timers.Timer(ConstYXYH.CalIntvel);   //实例化Timer类，设置间隔时间为20秒；  
        public static DateTime TimeOfTimerStart = DateTime.Now;//计算定时器启动的时间
        public static DataTable CalDataTable = new DataTable();
        private Regex getArgRegex = new Regex(@"^GET[\s\S]*"); //匹配GetArg()的正则表达式
        private Regex mathRegex = new Regex(@"^MATH[\s\S]*"); //匹配Math()的正则表达式
        private Hashtable FormulaHs = new Hashtable(); //存放;公式的Hashtable
        private Hashtable ValueHs = new Hashtable(); //存放结果的Hashtable
        //private Hashtable badValueHs = new Hashtable(); //存放死循环结果的Hashtable
        private ExtendPressMath extendMathStr = new ExtendPressMath();
        public static DataTable dtTagsData = new DataTable(); //存放测点实时数据
        public static DataTable dtTagsSet = new DataTable(); //存放测点
        //private PI.FPIRead piClass = new FPIRead();
        private YHYXYH.PI.PIRead piClass = new YHYXYH.PI.PIRead();
        private object[] ArgObj = new object[1];
        // 根据传入的原点代码，直接获取到实时数据,传入的sourtag是数据表中的列名
        private double GetValueForOne(string sourTag)
        {
            double returnvalue = 0.0;
            try
            {
                returnvalue = Convert.ToDouble(CalDataTable.Rows[0][sourTag.ToString()]);
            }
            catch (Exception)
            {

                returnvalue = 0.0;
            }
            return returnvalue;
        }
        // 根据传入的焓熵公式进行计算
        private double GetValueForTwo(string formulaStr)
        {
            //获取到需要出入的数据，结果是[39.23,3)]最后的数字是说明取返回数组的第几个数据
            string[] tempArgsArr = formulaStr.Substring(formulaStr.IndexOf('(') + 1).Split(',');
            ArgObj[0] = tempArgsArr; //因为嫌麻烦，数组最后的一个参数也一块传过去，只是在被调用的同名方法比如P2L中没有使用例子中的参数3)
            // MethodInfo getMethodFromFunction = StaticArgClass.type.GetMethod(functionName, new Type[]{typeof(string[])});
            MethodInfo getMethodFromFunction = typeof(WASPCNTrancate).GetMethod(formulaStr.Split('(')[0].ToString(), new Type[] { typeof(string[]) });
            double[] aaa = (double[])getMethodFromFunction.Invoke(StaticArgClass.typeObj, ArgObj);
            int reutrnIndex = Convert.ToInt16(tempArgsArr[tempArgsArr.Length - 1].Split(')')[0]);
            return Math.Round(aaa[reutrnIndex - 1], 4);
        }
        //对math公式进行计算
        private double GetValueForThree(string formulaStr)
        {
            string temp = formulaStr.Trim().Substring(5);
            temp = temp.Substring(0, temp.Length - 1);
            string returnStr = extendMathStr.runExpress(temp);
            //("(34+23)/34+sqrt(9)");
            double returnValue = 0.0;
            try
            {
                if (returnStr.ToString().Contains("无穷大") || returnStr.ToString().Contains("非数字"))
                {
                    returnValue = ConstYXYH.ReplaceValue;
                }
                else
                {
                    returnValue = Convert.ToDouble(returnStr);
                }

            }
            catch (Exception)
            {
                returnValue = 0.0;
            }
            return Math.Round(returnValue, 4);
        }
        //对传入的包含20行数据的datatable做平均处理，并将-9999的错误数据排除
        private void PreCalData(DataTable preDataTable)
        {
            if (CalDataTable == null)
            {
                CalDataTable = preDataTable.Clone();
            }
            if (CalDataTable.Rows.Count < 1)
            {
                CalDataTable = null;
                CalDataTable = preDataTable.Clone();
                DataRow dtr = CalDataTable.NewRow();
                CalDataTable.Rows.Add(dtr);
            }
            //CalDataTable=null;
            //CalDataTable = preDataTable.Clone();

            CalDataTable.Rows[0]["valuetime"] = DateTime.Now;
            for (int i = 0; i < dtTagsSet.Rows.Count; i++)
            {
                int num = 0;
                double sum = 0.0;
                bool iscontinue = true;
                //如果替代值启用
                if (dtTagsSet.Rows[i]["isset"].ToString().Equals("1"))
                {
                    double tempSetValue = 0.0;
                    //CalDataTable.Rows[0][dtTagsSet.Rows[i]["ExcelCell"].ToString()] = ConstYXYH.ReplaceValue.ToString();
                    try
                    {
                        tempSetValue = Convert.ToDouble(dtTagsSet.Rows[i]["SetValue"]);
                    }
                    catch (Exception e)
                    {
                        tempSetValue = 0.0;
                    }
                    //tempSetValue = Convert.ToDouble(dtTagsSet.Rows[i]["SetValue"]);
                    //Random rand = new Random();
                    //double xishu = Math.Round((rand.NextDouble()-0.5)*5,2);//正负0.5*5=2.5
                    //tempSetValue += tempSetValue*xishu/100;
                    CalDataTable.Rows[0][dtTagsSet.Rows[i]["ExcelCell"].ToString()] = tempSetValue;
                    dtTagsSet.Rows[i]["currentValue"] = tempSetValue;
                    if (!ValueHs.ContainsKey(CalDataTable.Columns[dtTagsSet.Rows[i]["ExcelCell"].ToString()].ColumnName.ToUpper()))
                    {
                        ValueHs.Add(CalDataTable.Columns[dtTagsSet.Rows[i]["ExcelCell"].ToString()].ColumnName.ToUpper(), tempSetValue);
                    }
                    continue;
                }

                for (int j = 0; j < dtTagsData.Rows.Count; j++)
                {
                    if (string.IsNullOrEmpty(dtTagsData.Rows[j][dtTagsSet.Rows[i]["ExcelCell"].ToString()].ToString()))
                    {
                        iscontinue = false;
                        break;//如果该列为空，说明为计算点，跳过
                    }
                    if (!dtTagsData.Rows[j][dtTagsSet.Rows[i]["ExcelCell"].ToString()].ToString().Equals(ConstYXYH.ReplaceValue.ToString()))
                    {
                        sum += Convert.ToDouble(preDataTable.Rows[j][dtTagsSet.Rows[i]["ExcelCell"].ToString()].ToString());
                        num += 1;
                    }
                }
                if (iscontinue)
                {
                    try
                    {
                        if (num > 0)
                        {
                            CalDataTable.Rows[0][dtTagsSet.Rows[i]["ExcelCell"].ToString()] = Math.Round(sum / num, 4);
                        }
                        else
                        {
                            CalDataTable.Rows[0][dtTagsSet.Rows[i]["ExcelCell"].ToString()] = ConstYXYH.ReplaceValue.ToString();
                        }

                    }
                    catch (Exception e)
                    {

                    }
                    dtTagsSet.Rows[i]["currentValue"] = CalDataTable.Rows[0][dtTagsSet.Rows[i]["ExcelCell"].ToString()];
                    if (!ValueHs.ContainsKey(CalDataTable.Columns[dtTagsSet.Rows[i]["ExcelCell"].ToString()].ColumnName.ToUpper()))
                    {
                        ValueHs.Add(CalDataTable.Columns[dtTagsSet.Rows[i]["ExcelCell"].ToString()].ColumnName.ToUpper()
                            , CalDataTable.Rows[0][dtTagsSet.Rows[i]["ExcelCell"].ToString()].ToString());
                    }
                }
            }

            #region
            //for (int i = 1; i < CalDataTable.Columns.Count; i++)
            //{
            //    int num = 0;
            //    double sum = 0.0;
            //    bool iscontinue = true;
            //    for(int j=0;j<dtTagsData.Rows.Count;j++)
            //    {
            //        if(string.IsNullOrEmpty(dtTagsData.Rows[j][i].ToString()))
            //        {
            //            iscontinue = false;
            //            break;//如果该列为空，说明为计算点，跳过
            //        }
            //        if(!dtTagsData.Rows[j][i].ToString().Equals(ConstYXYH.ReplaceValue.ToString()))
            //        {
            //            sum += Convert.ToDouble(preDataTable.Rows[j][i].ToString());
            //            num+=1;
            //        }
            //    }
            //    if(iscontinue)
            //    {
            //        try
            //        {
            //            if(num>0)
            //            {
            //                dtr[i] = Math.Round(sum / num, 4);
            //            }else
            //            {
            //                dtr[i] = ConstYXYH.ReplaceValue.ToString();
            //            }

            //        }catch(Exception e)
            //        {
            //            dtr[i] = ConstYXYH.ReplaceValue.ToString();
            //        }
            //        if(!ValueHs.ContainsKey(CalDataTable.Columns[i].ColumnName.ToUpper()))
            //        {
            //            ValueHs.Add(CalDataTable.Columns[i].ColumnName.ToUpper(),dtr[i].ToString());
            //        }
            //    }
            //}
            #endregion
            //CalDataTable.Rows.Add(dtr);
        }
        //计算的对外方法，调用此方法，将会进行公式计算
        public void CalTreeMain()
        {
            try
            {
                #region 初始化sql server
                TagInit.initDataBase();
                dtTagsSet = TagInit.getAllData();
                //dtTagsSet.Columns.Add("currentValue");
                dtTagsData = SQLHelper.ExecuteDt("select * from " + ConstYXYH.CurrentTable + " where 0=1");
                #endregion
                #region 读取公式进行计算
                //StreamReader objReader = new StreamReader("formulate.txt", Encoding.Default);
                //string sLine = "";
                //while (sLine != null)
                //{
                //    sLine = objReader.ReadLine();
                //    if (sLine != null && !sLine.Equals(""))
                //    {
                //        string lineStr = sLine.Split('%')[0].ToString();
                //        string[] formulaHsArr = lineStr.Split('=');
                //        string formulaHaKey = formulaHsArr[0].ToString().Trim().ToUpper();
                //        if (!FormulaHs.ContainsKey(formulaHaKey))
                //        {
                //            string formulaHaValue =  Regex.Replace(formulaHsArr[1].ToString().Trim().ToUpper(), @"\s", "");
                //            FormulaHs.Add(formulaHaKey, formulaHaValue);
                //        }
                //    }
                //}
                //objReader.Close();
                for (int i = 0; i < Formulate.formulateArr.Length; i++)
                {
                    string lineStr = Formulate.formulateArr[i].Split('%')[0].ToString(); //去掉公示后面%后的注释
                    string[] formulaHsArr = lineStr.Split('='); //分开公式的结果和表达式
                    string formulaHaKey = formulaHsArr[0].ToString().Trim().ToUpper();
                    if (!FormulaHs.ContainsKey(formulaHaKey))
                    {
                        string formulaHaValue = Regex.Replace(formulaHsArr[1].ToString().Trim().ToUpper(), @"\s", "");
                        FormulaHs.Add(formulaHaKey, formulaHaValue);
                    }
                }
                #endregion
                //WriteLog.WriteLogs("-------");
                timer1_Tick(null, null);
                //WriteLog.WriteLogs("11111111111111111");
                //定时器要一直运行，它的定义不应该放在方法的内部，否则，运行一段时间可能就被垃圾回收而停止了；已放在了类的开头，改成了类变量，edit by hlt 2014-12-6
                //System.Timers.Timer t = new System.Timers.Timer(ConstYXYH.CalIntvel);   //实例化Timer类，设置间隔时间为20秒；   
                t.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Tick); //到达时间的时候执行事件；   
                t.AutoReset = true;   //设置是执行一次（false）还是一直执行(true)；
                t.Enabled = true;     //是否执行System.Timers.Timer.Elapsed事件； 
                TimeOfTimerStart = DateTime.Now;
            }
            catch (Exception ex) { WriteLog.WriteLogs(ex.ToString()); }
        }
        //遍历计算公式并且保存
        private void CalStep()
        {
            //创建表的方法应该每次计算时都调用，而不应该只在程序启动和关闭的时候才调用！add by hlt 2014-12-8
            try { TagInit.initDataBase(); }
            catch { }

            //遍历存放公式的Hashtable
            foreach (DictionaryEntry de in FormulaHs)
            {
                GetValueByFormule(de.Key.ToString().Trim().ToUpper());
            }
            #region 计算完成后，将数据存入数据库中，其中包括原始点和计算点，注意存入时，将时间保持一致
            for (int i = 1; i < CalDataTable.Columns.Count; i++)
            {
                //if (string.IsNullOrEmpty(CalDataTable.Rows[0][i].ToString())) //如果该列是空
                //{
                if (ValueHs.ContainsKey(CalDataTable.Columns[i].ColumnName.ToUpper()))
                {
                    CalDataTable.Rows[0][i] = ValueHs[CalDataTable.Columns[i].ColumnName.ToUpper()].ToString();
                }
                //}
            }
            CalcClass1.calC1();//按压力计算
            if (m_IsFirstRun == false)
                SQLHelper.insertDataTable(CalDataTable, ConstYXYH.CurrentTable);
            #endregion
        }
        //递归方法，进行具体计算方法的选择。选择调用 GetValueForOne，GetValueForTwo，GetValueForThree
        private void GetValueByFormule(string ArgStr)//,string parentStr)
        {
            if (ArgStr == "F2370001")
                ArgStr = ArgStr;
            if (ArgStr.Equals("F159"))
            {
                for (int i = 0; i < 2; i++)
                {
                    int ii = 0;
                }
            }
            if (!ValueHs.ContainsKey(ArgStr))
            {
                string FormulaStr = FormulaHs[ArgStr].ToString().ToUpper(); //取出公式
                //如果是直接取数，直接根据点名去hashtable中取数
                #region
                if (getArgRegex.Match(FormulaStr).Success)
                {
                    double tempvalue = GetValueForOne(ArgStr);
                    ValueHs.Add(ArgStr, tempvalue.ToString());
                }
                #endregion
                else
                {
                    //解析参数。
                    #region
                    string[] formulaStrArr = FormulaStr.Split('$');
                    string tempformulaStr = "";
                    #region
                    for (int i = 0; i < formulaStrArr.Length; i++)
                    {
                        if (i % 2 == 0)
                        {
                            tempformulaStr += formulaStrArr[i].ToString();
                        }
                        else
                        {
                            string tempArgStr = formulaStrArr[i].ToString();
                            //判断用到的参数是否已经计算
                            if (!ValueHs.ContainsKey(tempArgStr))
                            {
                                #region
                                //    //-------------------------------------------------------
                                //    if(badValueHs.ContainsKey(ArgStr))
                                //    {
                                //        if (badValueHs[ArgStr].ToString().Contains(tempArgStr+","))
                                //        {
                                //            WriteLog.WriteLogs("出现死循环");
                                //            break;
                                //        }
                                //        string tempStr = badValueHs[tempArgStr].ToString() + tempArgStr + ",";
                                //        badValueHs.Remove(ArgStr);
                                //        badValueHs.Add(ArgStr,tempStr);
                                //    }else
                                //    {
                                //        badValueHs.Add(ArgStr, tempArgStr + ",");
                                //    }
                                //-------------------------------------------------------
                                #endregion
                                GetValueByFormule(tempArgStr); //递归调用
                                //tempformulaStr += ValueHs[tempArgStr].ToString();
                            }
                            //如果该参数已经计算，看是否是-9999
                            if (Convert.ToDouble(ValueHs[tempArgStr].ToString()) == ConstYXYH.ReplaceValue)
                            {
                                DataRow[] dtrarr = dtTagsSet.Select("ExcelCell='" + tempArgStr + "'");
                                //ValueHs.Remove(tempArgStr);
                                //ValueHs.Add(tempArgStr,dtrarr[0]["setvalue"]);
                                if (string.IsNullOrEmpty(dtrarr[0]["setvalue"].ToString()))
                                {
                                    int a = 0;
                                    ValueHs.Add(ArgStr, ConstYXYH.ReplaceValue);
                                    return;
                                }
                                tempformulaStr += dtrarr[0]["setvalue"];
                            }
                            else
                            {
                                tempformulaStr += ValueHs[tempArgStr].ToString();
                            }
                        }
                        //tempformulaStr += ValueHs[tempArgStr].ToString();
                    }
                    #endregion
                    if (mathRegex.Match(FormulaStr).Success) //如果是数学计算
                    {
                        double tempvalue = GetValueForThree(tempformulaStr.Replace('[', '(').Replace(']', ')'));
                        ValueHs.Add(ArgStr, tempvalue);
                        //if(badValueHs.ContainsKey(ArgStr))
                        //{badValueHs.Remove(ArgStr);}
                    }
                    else
                    {
                        double tempvalue = GetValueForTwo(tempformulaStr);
                        ValueHs.Add(ArgStr, tempvalue);
                        //if (badValueHs.ContainsKey(ArgStr))
                        //{ badValueHs.Remove(ArgStr); }
                    }
                    #endregion
                }
            }
        }
        //按timer1间隔设置，读取并存储测点数据到数据库
        private bool isfinish = true;

        private void timer1_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (isfinish)
                {
                    isfinish = false;
                    if (DateTime.Now.Minute < 1 && DateTime.Now.Second < 30)
                    {
                        dtTagsData.Dispose();
                        dtTagsData = null;
                        dtTagsData = SQLHelper.ExecuteDt("select * from " + ConstYXYH.CurrentTable + " where 0=1");
                        //GC.Collect();
                    }
                    if (PIRead.IsOperatingPI == true)
                    {
                        WriteLog.WriteLogs("FCZ pass");
                        isfinish = true;
                        return;
                    }
                    dtTagsSet = TagInit.getAllData();
                    if (m_IsFirstRun == false)
                        piClass.GetPIData(dtTagsSet, dtTagsData); //后面的new datatable()替换成从pi中传入的datatable
                    else
                    {
                        DataRow rowNew = dtTagsData.NewRow();
                        rowNew["ValueTime"] = DateTime.Now;
                        foreach (DataRow row in dtTagsSet.Rows)
                        {
                            if (row["DataSourcesNo"].ToString() == "1" && row["tag"].ToString().Trim().Length > 1)
                                rowNew[row["ExcelCell"].ToString()] = row["SetValue"];
                        }
                        dtTagsData.Rows.Add(rowNew);
                    }

                    #region
                    if (dtTagsData.Rows.Count > ConstYXYH.AvgNum)
                    {
                        dtTagsData.Rows.RemoveAt(0);
                    }
                    //已经生成20行datatable
                    ValueHs.Clear();
                    //badValueHs = new Hashtable(); //存放死循环结果的Hashtable
                    PreCalData(dtTagsData);//预处理取数的20行数据
                    CalStep();//遍历计算并存入sql server
                    isfinish = true;
                    #endregion
                    if (m_IsFirstRun == true)
                        dtTagsData.Rows.RemoveAt(0);
                    m_IsFirstRun = false;
                }
                if ((DateTime.Now - timeGCCollect).TotalHours > 1)
                {
                    timeGCCollect = DateTime.Now;
                    GC.Collect();
                }
                System.Diagnostics.Process p = System.Diagnostics.Process.GetCurrentProcess();
                if (p.PagedMemorySize64 > 1000 * 1000 * 1000 & (DateTime.Now - time1GMemoryCollect).TotalMinutes > 10)
                {
                    time1GMemoryCollect = DateTime.Now;
                    WriteLog.WriteLogs("防颤振计算内存大于1G：" + p.PagedMemorySize64 + " , 程序已运行：" + (DateTime.Now - CalTree.TimeOfTimerStart).TotalHours);
                    System.GC.Collect();
                }
            }
            catch (Exception ex)
            {
                isfinish = true;
                WriteLog.WriteLogs(ex.ToString());
            }
        }
    }
}
