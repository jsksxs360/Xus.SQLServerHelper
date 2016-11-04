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
            //SqlHelper sqlHelper = new SqlHelper(".", "TestDB", "sa", "12345678");
            //建立SqlHelper对象（不包含用户名、密码）
            //SqlHelper sqlHelper = new SqlHelper(@"(localdb)\ProjectsV12", "test");

            //通过表名获取数据表
            //DataTable stuTable = sqlHelper.GetTable("student", 50);
            //PrintTable(stuTable);
            //通过sql语句获取数据表
            //DataTable stuTable = sqlHelper.GetTable("select * from student where sex=N'男'");
            //PrintTable(stuTable);

            //按流的方式单向读取数据（使用SqlDataReader）
            //SqlDataReader sqlDataReader = sqlHelper.GetDataStream("select * from student where sex=N'男'");
            //while (sqlDataReader.Read())
            //{
            //    Console.WriteLine(sqlDataReader["sid"] + "\t" +
            //        sqlDataReader["name"] + "\t" + sqlDataReader["age"] +
            //        "\t" + sqlDataReader["sex"]);
            //}

            //执行一条SQL语句
            //sqlHelper.ExecuteSqlCommand("insert into student(sid,name,age,sex) values(102,'hong',20,N'女')");
            //DataTable stuTable = sqlHelper.GetTable("student", 50);
            //PrintTable(stuTable);

            //添加数据到指定DataSet中（添加到一张表）
            //DataSet dataSet = new DataSet();
            //sqlHelper.AddDataToDataSet(dataSet, "select * from student", "student");
            //PrintTable(dataSet.Tables["student"]);
            //添加数据到指定DataSet中（添加到多张表）
            //DataSet dataSet = new DataSet();
            //sqlHelper.AddDataToDataSet(dataSet, new List<string> { "select * from student", "select * from teacher" }, new List<string> { "student", "teacher" });
            //PrintTable(dataSet.Tables["student"]);
            //PrintTable(dataSet.Tables["teacher"]);

            //修改student表的年龄，批量提交对数据表进行的修改
            //DataTable stuTable = sqlHelper.GetTable("select * from student");
            //foreach (DataRow row in stuTable.Rows)
            //    row["age"] = int.Parse(row["age"].ToString()) - 1;
            //sqlHelper.UpdateTable(stuTable, "select * from student");

            //修改student表的年龄，批量提交对数据表进行的修改
            //DataSet dataSet = new DataSet();
            //sqlHelper.AddDataToDataSet(dataSet, "select * from student", "student");
            //foreach (DataRow row in dataSet.Tables["student"].Rows)
            //    row["age"] = int.Parse(row["age"].ToString()) + 1;
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
