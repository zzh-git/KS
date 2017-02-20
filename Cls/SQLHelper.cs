using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;

using System.Data.SqlClient;
using System.Configuration;
using System.Xml;

namespace CJPrj
{
    public class SQLHelper
    {


        public static string getTableName()
        {

            string sTable = "tagdata";
            string sSql = "select top 1 convert(char(10),getdate(),112) from tags";

            try
            {
                string sDate = SQLHelper.ExecuteScalar(sSql);
                sTable += sDate;

            }
            catch (Exception E)
            {
                WriteLog.WriteLogs("获取当日表名时报错 " + E.ToString());
            }

            return sTable;
        }

        public static string getTableNameByDate(DateTime dtDay)
        {

            string sTable = "tagdata";
            string sSql = "select top 1 convert(char(10),convert(datetime,'" + dtDay + "'),112) from tags";

            try
            {
                string sDate = SQLHelper.ExecuteScalar(sSql);
                sTable += sDate;
            }
            catch (Exception E)
            {
                WriteLog.WriteLogs("根据时间获取表名时报错 " + E.ToString());
            }
            return sTable;
        }
        //获取昨日数据表名
        public static string getYesTableName()
        {

            string sTable = "tagdata";
            string sSql = "select top 1 convert(char(10),DATEADD(day,-1,getdate()),112) from tags";

            try
            {
                string sDate = SQLHelper.ExecuteScalar(sSql);
                sTable += sDate;
            }
            catch (Exception E)
            {
                WriteLog.WriteLogs("获取昨日表名时报错 " + E.ToString());
            }

            return sTable;
        }

        #region 通用方法
        // 数据连接池    
        private SqlConnection con;
        /// <summary>    
        /// 返回数据库连接字符串    
        /// </summary>    
        /// <returns></returns>    
        public static String GetSqlConnection()
        {
            XmlDocument doc = new XmlDocument();
            string sPath = System.AppDomain.CurrentDomain.BaseDirectory + "\\fConfig.xml";

            doc.Load(sPath);

            XmlNode Info = doc.SelectSingleNode("Info");
            string ConStr = Info.Attributes["ConStr"].InnerText;

            //String conn = CommonFunction.En_De.DESDecrypt(ConStr, "32#4G36tg", "3Ew%");// ConfigurationManager.ConnectionStrings[_connstr].ToString();
            ////String conn = CommonFunction.En_De.DESDecrypt(ConStr, "32#4G36tg", "3Ew%");// ConfigurationManager.ConnectionStrings[_connstr].ToString();
            String conn = @"Server=.;Uid=sa;Pwd=sql;DataBase=fczData;";
            return conn;
        }
        #endregion

        #region 公用方法
        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns>是否存在</returns>
        public static bool ColumnExists(string tableName, string columnName)
        {
            string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
            object res = ExecuteScalar(sql);
            if (res == null)
            {
                return false;
            }
            return Convert.ToInt32(res) > 0;
        }
        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = ExecuteScalar(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        public static int GetMaxID(string FieldName, string TableName, string parentid)
        {
            if (parentid.Length < 1)
            {
                parentid = "1";
            }
            string strsql = "select max(" + FieldName + ")+1 from " + TableName + " where FFUNPARENTID = '" + parentid + "'";
            object obj = ExecuteScalar(strsql);
            if (obj == null)
            {
                return int.Parse(parentid + "0001");
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        public static bool Exists(string strSql)
        {
            object obj = ExecuteScalar(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString()); //也可能=0
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static bool TabExists(string TableName)
        {
            string strsql = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
            //string strsql = "SELECT count(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')";
            object obj = ExecuteScalar(strsql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region 执行sql字符串
        /// <summary>    
        /// 执行不带参数的SQL语句    
        /// </summary>    
        /// <param name="Sqlstr"></param>    
        /// <returns></returns>    
        public static int ExecuteSql(String Sqlstr)
        {
            int i = 0;
            try
            {
                String ConnStr = GetSqlConnection();
                using (SqlConnection conn = new SqlConnection(ConnStr))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = Sqlstr;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                    i = 1;
                }
            }
            catch (Exception e)
            {
                WriteLog.WriteLogs("执行sql语句出错" + e.Message);
                WriteLog.WriteLogs("InnerException:" + e.InnerException.Message);
                WriteLog.WriteLogs("ErrorSource:" + e.Source.ToString());
                WriteLog.WriteLogs("ErrorSource:" + e.TargetSite.Attributes.ToString());
            }
            return i;
        }
        public static void insertDataTable(DataTable dt, string tablename)
        {
            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(ConnStr, SqlBulkCopyOptions.UseInternalTransaction);
                sqlbulkcopy.DestinationTableName = tablename;//数据库中的表名
                sqlbulkcopy.WriteToServer(dt);
                sqlbulkcopy.Close();
                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>    
        /// 执行多条SQL语句，实现数据库事务。    
        /// </summary>    
        /// <param name="SQLStringList">多条SQL语句</param>         
        public static void ExecuteSql(ArrayList SQLStringList)
        {
            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();

                }
                catch (Exception E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                    //    ITNB.Base.Error.showError(E.Message.ToString());    
                }
                conn.Close();
                conn.Dispose();
            }
        }

        /// <summary>    
        /// 执行多条SQL语句，实现数据库事务。    
        /// </summary>    
        /// <param name="SQLStringList">多条SQL语句</param>         
        public static bool ExecuteSqlReturnState(ArrayList SQLStringList)
        {
            bool bReturn = false;
            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    bReturn = true;
                }
                catch (Exception E)
                {
                    tx.Rollback();
                    throw new Exception(E.Message);
                    //    ITNB.Base.Error.showError(E.Message.ToString());    
                }
                conn.Close();
                conn.Dispose();
            }
            return bReturn;
        }

        /// <summary>    
        /// 执行带参数的SQL语句    
        /// </summary>    
        /// <param name="Sqlstr">SQL语句</param>    
        /// <param name="param">参数对象数组</param>    
        /// <returns></returns>    
        public static int ExecuteSql(String Sqlstr, SqlParameter[] param)
        {
            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = Sqlstr;
                cmd.Parameters.AddRange(param);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                return 1;
            }
        }
        /// <summary>    
        /// 返回DataReader    
        /// </summary>    
        /// <param name="Sqlstr"></param>    
        /// <returns></returns>    
        public static SqlDataReader ExecuteReader(String Sqlstr)
        {
            String ConnStr = GetSqlConnection();
            SqlConnection conn = new SqlConnection(ConnStr);//返回DataReader时,是不可以用using()的    
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = Sqlstr;
                conn.Open();
                return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);//关闭关联的Connection    
            }
            catch //(Exception ex)    
            {
                return null;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }
        }
        /// <summary>    
        /// 执行SQL语句并返回数据表    
        /// </summary>    
        /// <param name="Sqlstr">SQL语句</param>    
        /// <returns></returns>    
        public static DataTable ExecuteDt(String Sqlstr)
        {
            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(Sqlstr, conn);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    da.Fill(dt);

                }
                catch (Exception E)
                {
                    WriteLog.WriteLogs("获取数据表时报错: " + E.ToString());
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();

                }
                return dt;
            }
        }
        /// <summary>    
        /// 执行SQL语句并返回数据表    
        /// </summary>    
        /// <param name="Sqlstr">SQL语句</param>    
        /// <returns></returns>    
        public static DataTable ExecuteDt(String Sqlstr ,out string error)
        {
            error = "";
            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(Sqlstr, conn);
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    da.Fill(dt);

                }
                catch (Exception E)
                {
                    error = E.Message;
                    //WriteLog.WriteLogs("获取数据表时报错: " + E.ToString());
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();

                }
                return dt;
            }
        }

        /// <summary>    
        /// 执行SQL语句并返回数据表    
        /// </summary>    
        /// <param name="Sqlstr">SQL语句</param>    
        /// <returns></returns>    
        public static int GetSingle(String Sqlstr)
        {
            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                int i = 0;
                SqlDataAdapter da = new SqlDataAdapter(Sqlstr, conn);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                i = dt.Rows.Count;
                conn.Close();
                conn.Dispose();
                return i;
            }
        }

        /// <summary>    
        /// 执行SQL语句并返回DataSet    
        /// </summary>    
        /// <param name="Sqlstr">SQL语句</param>    
        /// <returns></returns>    
        public static DataSet ExecuteDs(String Sqlstr)
        {
            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlDataAdapter da = new SqlDataAdapter(Sqlstr, conn);
                DataSet ds = new DataSet();
                conn.Open();
                da.Fill(ds);
                conn.Close();
                conn.Dispose();
                return ds;
            }
        }
        /// <summary>
        ///获取单个记录 
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <param name="con"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string ExecuteScalar(string sqlstr)
        {
            string temp = "";
            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sqlstr;
                //int i = 0;
                try
                {
                    conn.Open();
                    temp = cmd.ExecuteScalar().ToString();
                }
                catch (Exception e)
                {
                    WriteLog.WriteLogs("获取单值时报错:" + e.ToString());
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
                return temp;
            }
        }

        ///获取结果记录 
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <param name="con"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static int ExecuteResultCount(string sqlstr)
        {
            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = sqlstr;
                int i = 0;
                try
                {
                    conn.Open();
                    i = cmd.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
                return i;
            }
        }

        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="query">select查询语句</param>
        /// <param name="pageSize">每页显示数据数</param>
        /// <param name="currentPageIndex">当前页码</param>
        /// <param name="dt">存储数据的DataTable实例</param>
        public static DataTable FillDataTable(string query, int pageSize, int currentPageIndex)
        {
            DataTable dt = new DataTable();
            //读取数据的开始索引
            long startIndex = (currentPageIndex - 1) * pageSize;
            //读取数据的结束索引
            long endIndex = currentPageIndex * pageSize - 1;
            //DataReader读取的当前数据行的索引
            long readToIndex = -1;
            String ConnStr = GetSqlConnection();
            using (SqlConnection conn = new SqlConnection(ConnStr))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                //数据源中的列数
                int cols = dr.VisibleFieldCount;
                //构造DataTable结构
                for (int i = 0; i < cols; i++)
                {
                    dt.Columns.Add(new DataColumn(dr.GetName(i), dr.GetFieldType(i)));
                }
                //读取数据，将数据一行一行添加到DataTable
                while (dr.Read())
                {
                    readToIndex++;
                    //当DataReader指针在开始索引和结束索引闭区间时才读取数据构造DataRow
                    //并添加到DataTable
                    if (readToIndex >= startIndex && readToIndex <= endIndex)
                    {
                        DataRow row = dt.NewRow();
                        for (int i = 0; i < cols; i++)
                        {
                            row[i] = dr[i];
                        }
                        dt.Rows.Add(row);
                    }
                }
                dr.Close();
            }
            return dt;
        }
        #endregion
        #region 操作存储过程
        /// <summary>    
        /// 运行存储过程(已重载)    
        /// </summary>    
        /// <param name="procName">存储过程的名字</param>    
        /// <returns>存储过程的返回值</returns>    
        public int RunProc(string procName)
        {
            SqlCommand cmd = CreateCommand(procName, null);
            cmd.ExecuteNonQuery();
            this.Close();
            return (int)cmd.Parameters["ReturnValue"].Value;
        }
        /// <summary>    
        /// 运行存储过程(已重载)    
        /// </summary>    
        /// <param name="procName">存储过程的名字</param>    
        /// <param name="prams">存储过程的输入参数列表</param>    
        /// <returns>存储过程的返回值</returns>    
        public int RunProc(string procName, SqlParameter[] prams)
        {
            SqlCommand cmd = CreateCommand(procName, prams);
            cmd.ExecuteNonQuery();
            this.Close();
            return (int)cmd.Parameters[0].Value;
        }
        /// <summary>    
        /// 运行存储过程(已重载)    
        /// </summary>    
        /// <param name="procName">存储过程的名字</param>    
        /// <param name="dataReader">结果集</param>    
        public void RunProc(string procName, out SqlDataReader dataReader)
        {
            SqlCommand cmd = CreateCommand(procName, null);
            dataReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
        /// <summary>    
        /// 运行存储过程(已重载)    
        /// </summary>    
        /// <param name="procName">存储过程的名字</param>    
        /// <param name="prams">存储过程的输入参数列表</param>    
        /// <param name="dataReader">结果集</param>    
        public void RunProc(string procName, SqlParameter[] prams, out SqlDataReader dataReader)
        {
            SqlCommand cmd = CreateCommand(procName, prams);
            dataReader = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
        /// <summary>    
        /// 创建Command对象用于访问存储过程    
        /// </summary>    
        /// <param name="procName">存储过程的名字</param>    
        /// <param name="prams">存储过程的输入参数列表</param>    
        /// <returns>Command对象</returns>    
        private SqlCommand CreateCommand(string procName, SqlParameter[] prams)
        {
            // 确定连接是打开的    
            Open();
            //command = new SqlCommand( sprocName, new SqlConnection( ConfigManager.DALConnectionString ) );    
            SqlCommand cmd = new SqlCommand(procName, con);
            cmd.CommandType = CommandType.StoredProcedure;
            // 添加存储过程的输入参数列表    
            if (prams != null)
            {
                foreach (SqlParameter parameter in prams)
                    cmd.Parameters.Add(parameter);
            }
            // 返回Command对象    
            return cmd;
        }
        /// <summary>    
        /// 创建输入参数    
        /// </summary>    
        /// <param name="ParamName">参数名</param>    
        /// <param name="DbType">参数类型</param>    
        /// <param name="Size">参数大小</param>    
        /// <param name="Value">参数值</param>    
        /// <returns>新参数对象</returns>    
        public SqlParameter MakeInParam(string ParamName, SqlDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }
        /// <summary>    
        /// 创建输出参数    
        /// </summary>    
        /// <param name="ParamName">参数名</param>    
        /// <param name="DbType">参数类型</param>    
        /// <param name="Size">参数大小</param>    
        /// <returns>新参数对象</returns>    
        public SqlParameter MakeOutParam(string ParamName, SqlDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }
        /// <summary>    
        /// 创建存储过程参数    
        /// </summary>    
        /// <param name="ParamName">参数名</param>    
        /// <param name="DbType">参数类型</param>    
        /// <param name="Size">参数大小</param>    
        /// <param name="Direction">参数的方向(输入/输出)</param>    
        /// <param name="Value">参数值</param>    
        /// <returns>新参数对象</returns>    
        public SqlParameter MakeParam(string ParamName, SqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            SqlParameter param;
            if (Size > 0)
            {
                param = new SqlParameter(ParamName, DbType, Size);
            }
            else
            {
                param = new SqlParameter(ParamName, DbType);
            }
            param.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null))
            {
                param.Value = Value;
            }
            return param;
        }
        #endregion
        #region 数据库连接和关闭
        /// <summary>    
        /// 打开连接池    
        /// </summary>    
        private void Open()
        {
            // 打开连接池    
            if (con == null)
            {
                //这里不仅需要using System.Configuration;还要在引用目录里添加    
                con = new SqlConnection(GetSqlConnection());
                con.Open();
            }
        }
        /// <summary>    
        /// 关闭连接池    
        /// </summary>    
        public void Close()
        {
            if (con != null)
                con.Close();
        }
        /// <summary>    
        /// 释放连接池    
        /// </summary>    
        public void Dispose()
        {
            // 确定连接已关闭    
            if (con != null)
            {
                con.Dispose();
                con = null;
            }
        }
        #endregion
    }
}