using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlLogMiner
{
    public class SqlServerManager
    {
        public SqlServerManager()
        {
            
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

            SqlConnectionStringBuilder connection = new SqlConnectionStringBuilder
            {
                DataSource = server,
            };

            if (windowsAuth)
                connection.IntegratedSecurity = true;
            else
            {
                connection.UserID = username;
                connection.Password = password;
            }

            String strConn = connection.ToString();
            SqlConnection sqlConn = new SqlConnection(strConn);
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
    }
}
