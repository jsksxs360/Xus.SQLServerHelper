using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Xus.SQLServerHelper;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            //建立SqlHelper对象（包含用户名、密码）
            //SqlHelper sqlHelper = new SqlHelper("127.0.0.1", "TestDB", "sa", "12345678");
            //建立SqlHelper对象（不包含用户名、密码）
            SqlHelper sqlHelper = new SqlHelper("127.0.0.1", "TestDB");

            //通过表名获取数据表
            DataTable stuTable = sqlHelper.GetTable("student", 50);
            PrintTable(stuTable);
            //通过sql语句获取数据表
            DataTable stuTable2 = sqlHelper.GetTable("select * from student where sex=N'男'");
            PrintTable(stuTable2);

            //按流的方式单向读取数据（使用SqlDataReader）
            SqlDataReader sqlDataReader = sqlHelper.GetDataStream("select * from student where sex=N'男'");
            while (sqlDataReader.Read())
            {
				//获取指定字段的值
				string id = sqlDataReader["sid"].ToString();
				string name = sqlDataReader["name"].ToString();
				string sex = sqlDataReader["sex"].ToString();
				string score = sqlDataReader["score"].ToString();
				Console.WriteLine(id + "\t" + name + "\t" + sex + "\t" + score);
            }
			sqlHelper.CloseConnection();

            //执行一条SQL语句
            sqlHelper.ExecuteSqlCommand("insert into student(sid,name,sex,score) values(102,'hong',N'女',78.5)");
            DataTable stuTable3 = sqlHelper.GetTable("student", 50);
            PrintTable(stuTable3);

            //添加数据到指定DataSet中（添加到一张表）
            DataSet dataSet = new DataSet();
            sqlHelper.AddDataToDataSet(dataSet, "select * from student", "student");
            PrintTable(dataSet.Tables["student"]);
            //添加数据到指定DataSet中（添加到多张表）
            //DataSet dataSet = new DataSet();
            //sqlHelper.AddDataToDataSet(dataSet, new List<string> { "select * from student", "select * from teacher" }, new List<string> { "student", "teacher" });
            //PrintTable(dataSet.Tables["student"]);
            //PrintTable(dataSet.Tables["teacher"]);

            //修改student表的分数，批量提交对数据表进行的修改
            DataTable tempTable = sqlHelper.GetTable("select * from student");
            foreach (DataRow row in tempTable.Rows)
				row["score"] = double.Parse(row["score"].ToString()) - 1;
            sqlHelper.UpdateTable(tempTable, "select * from student");

            //修改student表的分数，批量提交对数据表进行的修改
            //DataSet dataSet = new DataSet();
            //sqlHelper.AddDataToDataSet(dataSet, "select * from student", "student");
            //foreach (DataRow row in dataSet.Tables["student"].Rows)
            //    row["score"] = int.Parse(row["score"].ToString()) + 1;
            //sqlHelper.UpdateTable(dataSet, "student", "select * from student");
        }

        /// <summary>
        /// 打印数据表
        /// </summary>
        /// <param name="table">要打印的DataTable表</param>
        public static void PrintTable(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    Console.Write(row[column] + "\t");
                }
                Console.WriteLine();
            }
        }
    }
}
