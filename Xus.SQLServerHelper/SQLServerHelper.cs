using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace Xus.SQLServerHelper
{
	/// <summary>
	/// SQL 连接助手类
	/// </summary>
    public class SqlHelper
    {
		/// <summary>
		/// Sql连接对象
		/// </summary>
		/// <value>Sql连接对象</value>
        public SqlConnection SqlCnt { get; set; } //Sql连接对象

        /// <summary>
        /// 构造函数
        /// （使用用户名、密码验证）
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="dataBase">数据库</param>
        /// <param name="user">用户名</param>
        /// <param name="pwd">密码</param>
        /// <param name="timeout">连接超时（秒），默认5秒</param>
        public SqlHelper(string dataSource, string dataBase, string user, string pwd, int timeout = 5)
        {
            string connectionString = "Data Source=" + dataSource + ";Initial Catalog=" + dataBase + ";User ID=" + user + ";Password=" + pwd + ";Connection Timeout=" + timeout + ";";
            SqlCnt = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 构造函数
        /// （使用Windows身份验证）
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="dataBase">数据库</param>
        /// <param name="timeout">连接超时（秒），默认5秒</param>
        public SqlHelper(string dataSource, string dataBase, int timeout = 5)
        {
            string connectionString = "Data Source=" + dataSource + ";Initial Catalog=" + dataBase + ";Integrated Security=True;Connection Timeout=" + timeout + ";";
            SqlCnt = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 构造函数
        /// （传入连接字符串）
        /// </summary>
        /// <param name="connectionString"></param>
        public SqlHelper(string connectionString)
        {
            SqlCnt = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 打开连接
        /// </summary>
        private void OpenConnection()
        {
            if (SqlCnt.State == ConnectionState.Closed) //连接关闭
            {
                try
                {
                    SqlCnt.Open();
                }
				catch (Exception e)
                {
                    throw new Exception("服务器连接失败:" + e);
                }
            }
            else if (SqlCnt.State == ConnectionState.Broken) //连接中断
            {
                try
                {
                    CloseConnection();
                    SqlCnt.Open();
                }
                catch (Exception e)
                {
                    throw new Exception("服务器连接失败:" + e);
                }
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnection()
        {
            try
            {
                SqlCnt.Close();
            }
            catch (Exception e)
            {
                throw new Exception("关闭数据库连接失败:" + e);
            }
        }

        /// <summary>
        /// 执行一条SQL语句
        /// </summary>
        /// <param name="sqlCommand">要执行的SQL语句</param>
        /// <param name="closeConnection">是否关闭连接，默认关闭</param>
        /// <returns>执行SQL语句受影响的行数</returns>
        public int ExecuteSqlCommand(string sqlCommand, bool closeConnection = true)
        {
            if (sqlCommand == null || sqlCommand == string.Empty)
                throw new Exception("要执行的SQL语句不能为空");
            OpenConnection();
            SqlCommand sqlCmd = new SqlCommand(sqlCommand, SqlCnt);
            try
            {
                int changeRows = sqlCmd.ExecuteNonQuery(); //执行SQL语句
                if (closeConnection) //关闭连接
                    CloseConnection();
                return changeRows;
            }
            catch (Exception e)
            {
                throw new Exception("SQL语句存在错误:" + e);
            }
        }

        /// <summary>
        /// 通过sql语句获取数据表
        /// </summary>
        /// <param name="selectSqlCommand">获取表的select语句</param>
        /// <returns>获取到的数据表</returns>
        public DataTable GetTable(string selectSqlCommand)
        {
            if (selectSqlCommand == null || selectSqlCommand == string.Empty)
                throw new Exception("要执行的select语句不能为空");
            OpenConnection();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectSqlCommand, SqlCnt);
            DataTable dataTable = new DataTable();
            try
            {
                sqlDataAdapter.Fill(dataTable); //通过SqlDataAdapter填充DataTable对象
            }
            catch (Exception e)
            {
                throw new Exception("select语句有错或者数据表不存在:" + e);
            }
            finally
            {
                CloseConnection();
            }
            return dataTable;
        }

        /// <summary>
        /// 通过表名获取数据表
        /// </summary>
        /// <param name="tableName">获取数据表的名称</param>
        /// <param name="rows">查询的数据行数</param>
        /// <returns>获取到的数据表</returns>
        public DataTable GetTable(string tableName, int rows)
        {
            if (tableName == null || tableName == string.Empty)
                throw new Exception("要获取的数据表名称不能为空");
            OpenConnection();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("select top " + rows + " * from " + tableName, SqlCnt);
            DataTable dataTable = new DataTable();
            try
            {
                sqlDataAdapter.Fill(dataTable); //通过SqlDataAdapter填充DataTable对象
                CloseConnection();
                return dataTable;
            }
            catch (Exception e)
            {
                throw new Exception("数据表不存在:" + e);
            }
        }

        /// <summary>
        /// 按流的方式单向读取数据
        /// （使用SqlDataReader）
        /// </summary>
        /// <param name="selectSqlCommand">获取数据的select语句</param>
        /// <returns>SqlDataReader对象</returns>
        public SqlDataReader GetDataStream(string selectSqlCommand)
        {
            if (selectSqlCommand == null || selectSqlCommand == string.Empty)
                throw new Exception("要执行的select语句不能为空");
            OpenConnection();
            SqlCommand sqlCmd = new SqlCommand(selectSqlCommand, SqlCnt);
            try
            {
                SqlDataReader reader = sqlCmd.ExecuteReader(); //建立SqlDataReader对象
                return reader;
            }
            catch (Exception e)
            {
                throw new Exception("select语句存在错误或者数据表不存在:" + e);
            }
        }

        /// <summary>
        /// 添加数据到指定DataSet中
        /// （添加到一张表）
        /// </summary>
        /// <param name="dataSet">被填充的DataSet</param>
        /// <param name="selectSqlCommands">获取数据的select语句</param>
        /// <param name="insertTableName">插入数据表的表名</param>
        public void AddDataToDataSet(DataSet dataSet, string selectSqlCommands, string insertTableName)
        {
            if (dataSet == null)
                throw new Exception("要填充数据的DataSet不能为null");
            if (selectSqlCommands == null || selectSqlCommands == string.Empty)
                throw new Exception("获取数据的select语句不能为空");
            if (insertTableName == null || insertTableName == string.Empty)
                throw new Exception("插入的表名不能为空");
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectSqlCommands, SqlCnt);
            try
            {
                sqlDataAdapter.Fill(dataSet, insertTableName); //通过SqlDataAdapter向DataSet中填充数据
            }
            catch (Exception e)
            {
                throw new Exception("select语句存在错误:" + e);
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// 添加数据到指定DataSet中
        /// （添加到多张表）
        /// </summary>
        /// <param name="dataSet">被填充的DataSet</param>
        /// <param name="selectSqlCommands">获取数据的select语句列表</param>
        /// <param name="insertTableNames">对应sql语句列表的插入表名列表</param>
        public void AddDataToDataSet(DataSet dataSet, List<string> selectSqlCommands, List<string> insertTableNames)
        {
            if (dataSet == null)
                throw new Exception("要填充数据的DataSet不能为null");
            if (selectSqlCommands == null || selectSqlCommands.Count == 0)
                throw new Exception("获取数据的select语句列表不能为空");
            if (insertTableNames == null || insertTableNames.Count == 0)
                throw new Exception("插入表名列表不能为空");
            if (selectSqlCommands.Count != insertTableNames.Count)
                throw new Exception("select语句列表与插入表名列表长度不一致");
            //拼接select语句列表，获取最终执行的select语句
            string selectCommand = string.Empty;
            foreach (string cmd in selectSqlCommands)
                if (cmd.Last() == ';')
                    selectCommand += cmd;
                else
                    selectCommand += (cmd + ";");
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(selectCommand, SqlCnt);
            //通过插入表名列表，指定数据插入的数据表名称
            sqlDataAdapter.TableMappings.Add("Table", insertTableNames.ElementAt(0));
            for (int i = 1; i < insertTableNames.Count; i++)
                sqlDataAdapter.TableMappings.Add("Table" + i, insertTableNames.ElementAt(i));
            try
            {
                sqlDataAdapter.Fill(dataSet); //通过SqlDataAdapter向DataSet中填充数据
            }
            catch (Exception e)
            {
                throw new Exception("select语句列表中存在错误的sql语句:" + e);
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// 提交对数据表进行的修改
        /// </summary>
        /// <param name="dataTable">修改的数据表</param>
        /// <param name="createTableSqlCommand">创建数据表的sql语句</param>
        public void UpdateTable(DataTable dataTable, string createTableSqlCommand)
        {
            if (dataTable == null)
                throw new Exception("修改的数据表不能为空");
            if (createTableSqlCommand == null || createTableSqlCommand == string.Empty)
                throw new Exception("创建数据表的sql语句不能为空");
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(createTableSqlCommand, SqlCnt);
            //为SqlDataAdapter赋予SqlCommandBuilder功能
            SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
            try
            {
                sqlDataAdapter.Update(dataTable); //批量提交表中的所有修改
            }
            catch (Exception e)
            {
                throw new Exception("向数据库批量提交修改失败:" + e);
            }
        }

        /// <summary>
        /// 提交对数据表进行的修改
        /// （在DataSet中的数据表）
        /// </summary>
        /// <param name="dataset">修改的数据表所在的DataSet</param>
        /// <param name="createTableSqlCommand">创建数据表的sql语句</param>
        /// <param name="TableName">被修改的数据表名</param>
        public void UpdateTable(DataSet dataset, string TableName, string createTableSqlCommand)
        {
            if (dataset == null)
                throw new Exception("修改过的DataSet不能为null");
            if (TableName == null || TableName == string.Empty)
                throw new Exception("数据表名不能为空");
            if (createTableSqlCommand == null || createTableSqlCommand == string.Empty)
                throw new Exception("创建数据表的select语句不能为空");
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(createTableSqlCommand, SqlCnt);
            //为SqlDataAdapter赋予SqlCommandBuilder功能
            SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
            try
            {
                sqlDataAdapter.Update(dataset, TableName); //批量提交表中的所有修改
            }
			catch (Exception e)
            {
                throw new Exception("向数据库批量提交修改失败:" + e);
            }
        }
    }
}