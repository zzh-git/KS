using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CJPrj.Common;

namespace CJPrj.Cls
{
    /// <summary>
    /// 
    /// </summary>
    class TagInit
    {
        static bool bIsFilled = false;//是否加载了数据
        static string sCountSql = "";//查询记录数的SQL语句
        static DateTime dateHourCheck = DateTime.Now;//保存每小时检查的时间
        static DateTime dateRead = DateTime.Now.AddDays(-1);//保存读数据的日期
        static DataView viewData = new DataView();//保存读取的数据
        static TagInit()
        {
            fillData();

            DataTable dtTables = SQLHelper.ExecuteDt("select * from FCZDataTableNames");
            ArrayList arrSqls = new ArrayList();
            foreach (DataRow rowTable in dtTables.Rows)
            {
                try
                {
                    string error;
                    //判断数据表(YXYHDataXXX)里每个测点所对应的列是否都已经创建；如果没有，则创建。
                    DataTable dtTagData = SQLHelper.ExecuteDt("select * from " + rowTable["TableName"].ToString() + " where 1=0", out error);
                    if (error.Length == 0)
                    {
                        lock (viewData)
                        {
                            viewData.RowFilter = "";
                            foreach (DataRow row in viewData.Table.Rows)
                            {
                                try
                                {
                                    if (dtTagData.Columns[row["ExcelCell"].ToString()] == null)
                                        arrSqls.Add("alter table " + rowTable["TableName"].ToString() + " add  " + row["ExcelCell"].ToString() + " real;");
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch { }
            }
            if (arrSqls.Count > 0)
                SQLHelper.ExecuteSql(arrSqls);
        }
        // 判断视图数据是否重新加载
        /// <summary>
        /// 判断视图数据是否重新加载
        /// </summary>
       public static bool isFill()
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
                //            dateRead = DateTime.Now;
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
        //获取所有数据，直接把源DataTable返回
        /// <summary>
        ///获取所有数据，直接把源DataTable返回
        /// </summary>
        /// <returns>源DataTable</returns>
        public static DataTable getAllData()
        {
            fillData();
            lock (viewData)
            {
                //viewData.RowFilter = "";
                return viewData.Table;
            }
        }
        // 设置类重新加载数据
        /// <summary>
        /// 设置类重新加载数据
        /// </summary>
        public static void setFill()
        {
            dateRead.AddDays(-1);
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
                        sCountSql = "select count(*) from FCZHtags";
                        string sql = "select *,'' as currentValue from FCZHtags";
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
                viewData.RowFilter = "";
                foreach (DataRow row in viewData.Table.Rows)
                {
                    try
                    {
                        if (dtTagData.Columns[row["ExcelCell"].ToString()] == null)
                            arrSqls.Add("alter table " + ConstYXYH.CurrentTable + " add  " + row["ExcelCell"].ToString() + " real;");
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
        public static void initDataBase()
        {
            //在数据库中创建表，每一周的数据保存在一个表中
            string tableName = ConstYXYH.TABLE_NAME_PREFIX + DateControl.GetYearWeek(DateTime.Now);
            if (tableName != ConstYXYH.CurrentTable)
                CreateTable(tableName);
        }

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
            //设置压缩功能 sql server 2005没有压缩，如果使用2008及以上版本，可以使用。 edit by zhangchuanyun 2014-7-17
            sSql = "ALTER TABLE " + tableName + " REBUILD PARTITION = ALL WITH (DATA_COMPRESSION = PAGE)";
            listSql.Add(sSql);
            //更新配置表里当前使用的表
            sSql = "update FCZConfig SET ConfigValue='" + tableName + "' where ConfigKey='CurrentTable'";
            listSql.Add(sSql);
            //在YXYHDataTableNames表里添加创建表的记录
            sSql = "insert into FCZDataTableNames(DataDate,TableName) values(" + tableName.Replace(ConstYXYH.TABLE_NAME_PREFIX, "")
                + ",'" + tableName + "')";
            listSql.Add(sSql);
            //执行SQL语句，并添加测点对应的字段
            if (SQLHelper.ExecuteSqlReturnState(listSql))
            {
                ConstYXYH.CurrentTable = tableName;
                createDBDataTableColumns();
            }
            return bReturn;
        }
    }
}
