using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace YHYXYH.YXYH
{
    /// <summary>
    /// YXYHTags表数据缓存类
    /// </summary>
    class YXYHTags
    {
        static bool bIsFilled = false;//是否加载了数据
        static string sCountSql = "";//查询记录数的SQL语句
        static DateTime dateHourCheck = DateTime.Now;//保存每小时检查的时间
        static DateTime dateRead = DateTime.Now.AddDays(-1);//保存读数据的日期
        static DataView viewData = new DataView();//保存读取的数据
        static YXYHTags()
        {
            fillData();

            DataTable dtTables = SQLHelper.ExecuteDt("select * from YXYHDataTableNames");
            ArrayList arrSqls = new ArrayList();
            foreach (DataRow rowTable in dtTables.Rows)
            {
                string error;
                //判断数据表(YXYHDataXXX)里每个测点所对应的列是否都已经创建；如果没有，则创建。
                DataTable dtTagData = SQLHelper.ExecuteDt("select * from " + rowTable["TableName"].ToString() + " where 1=0",out error);
                if (error == "")
                {
                    lock (viewData)
                    {
                        string sDataType = "";
                        viewData.RowFilter = "";
                        foreach (DataRow row in viewData.Table.Rows)
                        {
                            try
                            {
                                if (dtTagData.Columns[row["ExcelCell"].ToString()] == null)
                                {
                                    sDataType = "real";
                                    if (row["DataType"].ToString().Length > 0)
                                        sDataType = row["DataType"].ToString();
                                    arrSqls.Add("alter table " + rowTable["TableName"].ToString() + " add  " + row["ExcelCell"].ToString() + " " + sDataType + ";");
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            if (arrSqls.Count > 0)
                SQLHelper.ExecuteSql(arrSqls);
        }
        // 判断视图数据是否重新加载
        /// <summary>
        /// 判断视图数据是否重新加载
        /// </summary>
        static bool isFill()
        {
            bool bReturn = false;
            //每天加载一次，一定要加载，因为不仅有增删，还有更新
            if (dateRead.Day != DateTime.Now.Day | viewData.Count == 0)
            {
                bReturn = true;
                dateRead = DateTime.Now;
            }
            //每小时检查一次记录数量的变化
            if (bReturn == false & dateHourCheck.Hour != DateTime.Now.Hour)
            {
                bReturn = true;
                dateHourCheck = DateTime.Now;
                //if (sCountSql.Length > 0)
                //{
                //    dateHourCheck = DateTime.Now;
                //    try
                //    {
                //        if (Convert.ToInt32(SQLHelper.ExecuteScalar(sCountSql).ToString()) != viewData.Table.Rows.Count)
                //        {
                //            bReturn = true;
                //            dateHourCheck = DateTime.Now;
                //        }
                //    }
                //    catch (Exception) { }
                //}
            }
            return bReturn;
        }
        // 获取经过行过滤后的新DataTable，与原来的DataView没有关系
        /// <summary>
        /// 返回经过行过滤后的新DataTable，与原来的DataView没有关系
        /// </summary>
        /// <param name="sRowFilter">RowFilter字符串</param>
        /// <returns>新DataTable</returns>
        public static DataTable getDataTableByRowFilter(string sRowFilter)
        {
            DataTable dtReturn = new DataTable();
            try
            {
                fillData();
                lock (viewData)
                {
                    viewData.RowFilter = sRowFilter;
                    dtReturn = viewData.ToTable();
                }
            }
            catch (Exception) { }
            return dtReturn;
        }
        //获取所有数据的原DataTable
        /// <summary>
        ///获取所有数据的原DataTable
        /// </summary>
        /// <returns>源DataTable</returns>
        public static DataTable getAllData()
        {
            fillData();
            lock (viewData)
            {
                //viewData.RowFilter = "";
                //return viewData.ToTable();
                return viewData.Table;
            }
        }
        // 设置类重新加载数据
        /// <summary>
        /// 设置类重新加载数据
        /// </summary>
        public static void setFill()
        {
            dateRead = dateRead.AddDays(-1);
        }

        //数据加载
        /// <summary>
        /// 数据加载
        /// </summary>
        static void fillData()
        {
            if (isFill())
            {
                try
                {
                    lock (viewData)
                    {
                        sCountSql = "select count(*) from YXYHtags";
                        string sql = "select * from YXYHtags";
                        viewData = SQLHelper.ExecuteDt(sql).DefaultView;
                    }
                    bIsFilled = true;
                    createDBDataTableColumns();
                }
                catch (Exception) { }
            }
        }
        // 判断数据库表(YXYHDataXXX)里每个测点所对应的列是否都已经创建；如果没有，则创建。
        /// <summary>
        /// 判断数据库表(YXYHDataXXX)里每个测点所对应的列是否都已经创建；如果没有，则创建。
        /// </summary>
        public static void createDBDataTableColumns()
        {
            fillData();

            //判断数据表(YXYHDataXXX)里每个测点所对应的列是否都已经创建；如果没有，则创建。
            DataTable dtTagData = SQLHelper.ExecuteDt("select * from " + ConstYXYH.CurrentTable + " where 1=0");
            ArrayList arrSqls = new ArrayList();
            lock (viewData)
            {
                string sDataType = "";
                viewData.RowFilter = "";
                foreach (DataRow row in viewData.Table.Rows)
                {
                    try
                    {
                        if (dtTagData.Columns[row["ExcelCell"].ToString()] == null)
                        {
                            sDataType = "real";
                            if (row["DataType"].ToString().Length > 0)
                                sDataType = row["DataType"].ToString();
                            arrSqls.Add("alter table " + ConstYXYH.CurrentTable + " add  " + row["ExcelCell"].ToString() + " " + sDataType + ";");
                        }
                    }
                    catch { }
                }
            }
            if (arrSqls.Count > 0)
                SQLHelper.ExecuteSql(arrSqls);
        }
        // 获取是否加载了数据
        /// <summary>
        /// 获取是否加载了数据
        /// </summary>
        public static bool isFilled
        {
            get { return bIsFilled; }
        }
    }
}
