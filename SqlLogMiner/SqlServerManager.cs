using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlLogMiner.Entities;
using SqlLogMiner.Models;

namespace SqlLogMiner
{
    public class SqlServerManager
    {
        private SqlConnection _sqlConnection;
        public SqlServerManager()
        {
            _sqlConnection = new SqlConnection();
        }

        public void Disconnect()
        {
            _sqlConnection.Close();
        }

        public static List<string> GetSqlServerInstances()
        {
            System.Data.Sql.SqlDataSourceEnumerator instance = System.Data.Sql.SqlDataSourceEnumerator.Instance;
            System.Data.DataTable dataTable = instance.GetDataSources();
            List<string> serverInstances = new List<string>();
            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                serverInstances.Add(row.ItemArray[0]+"\\"+row.ItemArray[1]);
            }
            return serverInstances;
        }

        public static List<string> GetDatabases(string server, bool windowsAuth, string username, string password)
        {
            List<String> databases = new List<String>();
            SqlConnection sqlConn = GetTemporaryConnection(server,windowsAuth,username,password);
            sqlConn.Open();
            DataTable tblDatabases = sqlConn.GetSchema("Databases");
            sqlConn.Close();
            foreach (DataRow row in tblDatabases.Rows)
            {
                String strDatabaseName = row["database_name"].ToString();

                databases.Add(strDatabaseName);


            }
            return databases;
        }

        public static List<DataGridFilterTableModel> GetUserTables(string server, bool windowsAuth, string username, string password, string database)
        {
            List<DataGridFilterTableModel> userTables = new List<DataGridFilterTableModel>();
            SqlConnection sqlConn = GetTemporaryConnection(server, windowsAuth, username, password, database);
            sqlConn.Open();
            DataTable tblDatabases = sqlConn.GetSchema("Tables");
            sqlConn.Close();
            foreach (DataRow row in tblDatabases.Rows)
            {
                userTables.Add(new DataGridFilterTableModel
                {
                    IsChecked = true,
                    SchemaName = row[1].ToString(),
                    TableName = row["table_name"].ToString()
                });
            }

            return userTables;
        }

        public static List<DataGridFilterTableModel> GetSystemTables(string server, bool windowsAuth, string username, string password, string database)
        {
            List<DataGridFilterTableModel> userTables = new List<DataGridFilterTableModel>();
            SqlConnection sqlConn = GetTemporaryConnection(server, windowsAuth, username, password, database);
            sqlConn.Open();
            DataTable tblDatabases = sqlConn.GetSchema("Tables");
            sqlConn.Close();
            foreach (DataRow row in tblDatabases.Rows)
            {
                userTables.Add(new DataGridFilterTableModel
                {
                    IsChecked = true,
                    SchemaName = row[1].ToString(),
                    TableName = row["table_name"].ToString()
                });
            }

            return userTables;
        }

        public void ConnectWindowsAuth(string serverName, string database)
        {
            SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder
            {
                DataSource = serverName,
                IntegratedSecurity = true,
                InitialCatalog = database
            };
            _sqlConnection.ConnectionString = connection.ToString();
            _sqlConnection.Open();
        }

        public void ConnectSqlServerAuth(string serverName, string database, string userName, string password)
        {
            SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder
            {
                DataSource = serverName,
                UserID = userName,
                Password = password,
                InitialCatalog = database
            };
            _sqlConnection.ConnectionString = connection.ToString();
            _sqlConnection.Open();
        }

        private static SqlConnection GetTemporaryConnection(string server, bool windowsAuth, string username, string password, string database = null)
        {
            SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder
            {
                DataSource = server,
            };

            if (database != null)
            {
                connection.InitialCatalog = database;
            }

            if (windowsAuth)
                connection.IntegratedSecurity = true;
            else
            {
                connection.UserID = username;
                connection.Password = password;
            }

            String strConn = connection.ToString();
            return new SqlConnection(strConn);
        }

        public List<TransactionLogRow> GetTransactionLog(DateTime from, DateTime to, string[] operations, string[] schemaObject)
        {
            List<TransactionLogRow> transactionLog = new List<TransactionLogRow>();
            string query = ConstructTransactionLogQuery(from, to, operations, schemaObject);

            SqlCommand command = new SqlCommand(query, _sqlConnection);

            DataTable dataTable = new DataTable();
            dataTable.Load(command.ExecuteReader());

            foreach (DataRow row in dataTable.Rows)
            {
                string[] schemaAndObject = row["AllocUnitName"].ToString().Split('.');
                transactionLog.Add(new TransactionLogRow
                {
                    TransactionId = row["Transaction Id"].ToString(),
                    Operation = row["Operation"].ToString(),
                    User = row["UserName"].ToString(),
                    LSN = row["Current LSN"].ToString(),
                    BeginTime = DateTime.ParseExact(row["Begin Time"].ToString(), "yyyy/MM/dd HH:mm:ss:fff", CultureInfo.InvariantCulture),
                    Schema = schemaAndObject[0],
                    Object = schemaAndObject[1],
                    RowLogContents0 = (byte[])row["RowLog Contents 0"],
                    RowLogContents1 = (byte[])row["RowLog Contents 1"]
                });
                
            }

            return transactionLog;
        }

        private string ConstructTransactionLogQuery(DateTime from, DateTime to, string[] operations, string[] schemaObjects)
        {
            string query = "select log1.[Operation], log1.[AllocUnitName],SUSER_SNAME(log2.[Transaction SID]) as UserName , log2.[Begin Time], log1.[Transaction ID], log1.[Current LSN], log1.[RowLog Contents 0], log1.[RowLog Contents 1] from fn_dblog(NULL,NULL) as log1 Inner Join ("
            + "select [Transaction ID],[Transaction SID], [Begin Time] from fn_dblog(NULL,NULL) where [Begin Time] >= '" + from.ToString("yyyy/MM/dd HH:mm:ss") + "' AND [Begin Time] <= '" + to.ToString("yyyy-MMdd HH:mm:ss") + "' "
            + ") as log2 on log1.[Transaction ID] = log2.[Transaction ID] where ";
            if (operations.Count() != 0)
                query += " (";
            foreach (var operation in operations)
            {
                query += "log1.[Operation] = '" + operation;
                if (operations.Last() == operation)
                    query += "') and (";
                else
                    query += "' or ";
            }

            foreach (var schemaObject in schemaObjects)
            {
                query += "log1.[AllocUnitName] = '" + schemaObject;
                if (schemaObjects.Last() == schemaObject)
                    query += "')";
                else
                    query += "' or ";
                
            }

            return query ;
        }

        public TableSchema GetTableSchema(string database, string table)
        {
            var columns = _sqlConnection.GetSchema("Columns", new[] { database, null, table});
            TableSchema tableSchema = new TableSchema();
            tableSchema.TableName = table;
            foreach (DataRow column in columns.Rows)
            {
                if (column.ItemArray[7].ToString() == "decimal" || column.ItemArray[7].ToString() == "numeric")
                    tableSchema.Columns.Add(new Column { Type = column.ItemArray[7] + "(" + column.ItemArray[10] + "," + column.ItemArray[12] + ")", ColumnName = column.ItemArray[3].ToString() });
                else if (column.ItemArray[7].ToString() == "binary")
                    tableSchema.Columns.Add(new Column { Type = column.ItemArray[7] + "(" + column.ItemArray[8] + ")", ColumnName = column.ItemArray[3].ToString() });
                else if (column.ItemArray[7].ToString() == "char")
                    tableSchema.Columns.Add(new Column { Type = column.ItemArray[7] + "(" + column.ItemArray[8] + ")", ColumnName = column.ItemArray[3].ToString() });
                else
                    tableSchema.Columns.Add(new Column { Type = column.ItemArray[7].ToString(), ColumnName = column.ItemArray[3].ToString() });

            }

            return tableSchema;
        }
    }
}
