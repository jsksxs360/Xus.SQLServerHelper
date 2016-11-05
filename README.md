# SQLServerHelper

适用于 C# 的 SQL Server 连接辅助类，提供了对数据库的常用操作，包括执行 SQL 语句、按流的方式读取数据、填充本地 DataSet、批量提交对数据表的修改等。

### 下载

[Xus.SQLServerHelper](https://github.com/jsksxs360/Xus.SQLServerHelper/releases/)

## 构造函数

SQLServerHelper 对象有三种构造方式：使用用户名密码验证的构造函数、使用 Windows 身份验证的构造函数、使用连接字符串的构造函数。

```csharp
//使用用户名、密码验证
SqlHelper(string dataSource, string dataBase, string user, string pwd, int timeout = 5)

//或者使用Windows身份验证
SqlHelper(string dataSource, string dataBase, int timeout = 5)

//传入连接字符串
SqlHelper(string connectionString)
```

- **dataSource:** 数据源
- **dataBase:** 数据库
- **user:** 用户名
- **pwd:** 密码
- **timeout:** 连接超时（秒），默认5秒

通常情况下，我们使用带有用户名和密码的验证方式连接数据库。只有当数据库开启了 Windows 身份验证，并且以该用户身份登录主机时，我们才能使用直接验证身份的连接方式。

## 执行一条SQL语句

执行 SQL 语句，是最基本的操作，SQLServerHelper 提供了 **ExecuteSqlCommand()** 函数来实现：

```csharp
int ExecuteSqlCommand(string sqlCommand, bool closeConnection = true)
```

- **sqlCommand:** 要执行的 SQL 语句
- **closeConnection:** 是否关闭连接，默认关闭
- **returns:** 执行 SQL 语句受影响的行数

**ExecuteSqlCommand()** 函数实际是对 `SqlCommand.ExecuteNonQuery()` 函数的包装。当需要批量地执行 SQL 语句时，可以将 **closeConnection** 参数设置为 false 不关闭连接，待所有语句执行完毕后再通过 **CloseConnection()** 函数关闭连接，避免了每次打开、关闭连接的时间消耗。

## 获取数据表

从数据库中获取某张数据表也是常用的操作，SQLServerHelper 提供了 **GetTable()** 函数来实现：

```csharp
//通过sql语句获取数据表
DataTable GetTable(string selectSqlCommand)
```
- **selectSqlCommand:** 获取表的select语句

```csharp
//通过表名获取数据表
DataTable GetTable(string tableName, int rows)
```


- **tableName:** 获取数据表的名称
- **rows:** 查询的数据行数

可以直接通过 Select 语句来获取数据表，也可以直接通过表名和指定返回条数的方式获取数据表。函数实际上是对 `SqlDataAdapter.Fill()` 函数的包装。

## 按流的方式单向读取数据

当需要从数据库中读取大量数据时，直接返回整张表的方式就不合适了，这时就应该使用数据流的方式来读取了，SQLServerHelper 提供了 **GetDataStream()** 函数来实现：

```csharp
SqlDataReader GetDataStream(string selectSqlCommand)
```
- **selectSqlCommand:** 获取数据的select语句
- **returns:** SqlDataReader对象

获取到 SqlDataReader 对象后，就可以通过循环的方式来一条一条的读取数据了：

```csharp
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
```

注意：在读取完所有数据后，需要使用 **CloseConnection()** 函数手动关闭连接。

## 添加数据到指定 DataSet 中

DataSet 相当于是本地的一个临时数据库，当需要频繁从同一张数据表读取数据，或者需要批量修改表中的数据时，就可以使用 **AddDataToDataSet()** 函数在内存中建立临时的本地数据库，提高数据的读取速度。

```csharp
//添加数据到指定DataSet中（添加到一张表）
void AddDataToDataSet(DataSet dataSet, string selectSqlCommands, string insertTableName)
```
- **dataSet:** 被填充的 DataSet
- **selectSqlCommands:** 获取数据的select语句
- **insertTableName:** 插入数据表的表名

```csharp
//添加数据到指定DataSet中（添加到多张表）
void AddDataToDataSet(DataSet dataSet, List<string> selectSqlCommands, List<string> insertTableNames)
```
- **dataSet:** 被填充的DataSet
- **selectSqlCommands:** 获取数据的select语句列表
- **insertTableNames:** 对应sql语句列表的插入表名列表

**AddDataToDataSet()** 函数不仅支持在本地添加一张表，也支持同时建立多张表。

## 提交对数据表进行的修改

当我们需要对一张表做大量的修改时，通过反复执行 Update 函数就不是一个好的选择了。更好的做法是，现在本地建立一张表的备份，然后直接在内存中对表中的数据进行修改，最后再向数据库批量提交修改，SQLServerHelper 提供了 **UpdateTable()** 函数来实现：

```csharp
//提交对数据表进行的修改
void UpdateTable(DataTable dataTable, string createTableSqlCommand)
```
- **dataTable:** 修改的数据表
- **createTableSqlCommand:** 创建数据表的sql语句

```csharp
//提交对数据表进行的修改（在DataSet中的数据表）
void UpdateTable(DataSet dataset, string TableName, string createTableSqlCommand)
```
- **dataset:** 修改的数据表所在的DataSet
- **TableName:** 创建被修改的数据表名
- **createTableSqlCommand:** 创建数据表的sql语句

**UpdateTable()** 函数支持提交直接对数据表的修改，也支持提交对 DataSet 中的数据表的修改。

注意：**createTableSqlCommand** 参数是为了让函数知晓数据表的字段结构，因而必须包含完整的建表 SQL 语句，最好直接使用建表时的 SQL 语句以避免不必要的错误。

## 注意

### 与数据库连接的管理问题 

绝大多函数都会自动管理与数据库连接的打开和关闭，不需要用户的干预。只有按流的方式单向读取数据的 **GetDataStream()** 函数，需要用户在读取完所有数据后调用 **CloseConnection()** 函数手动关闭连接。如果执行 SQL 语句 **ExecuteSqlCommand()** 函数以不关闭连接的方式调用，也需要用户手动关闭连接。

### 同时读写

由于每一个 SQLServerHelper 对象只维护一个与数据库的连接，所以当读取操作与写入操作同时发生时会发生错误（例如，在按流的方式 **GetDataStream()** 单向读取数据的过程中，又调用 **ExecuteSqlCommand()** 函数向数据库中写入数据），这时候必须建立多个 SQLServerHelper 对象。

### 调用示例

具体的调用方式可以参考 [测试代码](https://github.com/jsksxs360/Xus.SQLServerHelper/blob/master/test/Program.cs)
