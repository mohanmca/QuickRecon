using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.Text;

namespace QuickRecon
{
    public static class SqliteHelper
    {
        public static void CreateDatabase(string databaseName)
        {
            SQLiteConnection.CreateFile(databaseName);
        }

        public static SQLiteConnection Connect(string databaseName)
        {
            SQLiteConnectionStringBuilder connString = new SQLiteConnectionStringBuilder
            {
                Version = 3,
                DataSource = databaseName
            };

            SQLiteConnection connection =
                new SQLiteConnection
                {
                    ConnectionString = connString.ToString()
                };

            connection.Open();

            return connection;
        }

        public static void Close(IDbConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        //string sql = "create table highscores (name varchar(20), score int)";
        public static string GetCreateTableQuery(SQLiteConnection connection, DataTable dataTable, string tableName)
        {
            StringBuilder createTableSql = new StringBuilder();
            createTableSql.Append("create table " + tableName + " (");
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                createTableSql.Append(dataTable.Columns[i].ColumnName + " varchar(20)");
                if (i != dataTable.Columns.Count - 1)
                {
                    createTableSql.Append(",");
                }
            }
            createTableSql.Append(")");

            return createTableSql.ToString();
        }

        public static void CreateTable(SQLiteConnection connection, string queryText)
        {
            SQLiteCommand command = new SQLiteCommand(queryText, connection);
            command.ExecuteNonQuery();
        }
        public static void InsertData(SQLiteConnection connection, DataTable dataTable)
        {
            string tableName = dataTable.TableName;

            var sqlQuery = "select * from " + tableName + " where 0 = 1";
            var dataAdapter = new SQLiteDataAdapter(sqlQuery, connection);
            var dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

            //dataSet.Tables[0].TableName = tableName;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var newRow = dataSet.Tables[0].NewRow();

                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    newRow[j] = dataTable.Rows[i][j];
                }

                dataSet.Tables[0].Rows.Add(newRow);
            }
            
            new SQLiteCommandBuilder(dataAdapter);
            dataAdapter.Update(dataSet);
        }


    }
}