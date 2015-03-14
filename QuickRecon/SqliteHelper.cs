using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.Linq;
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
       

        public static void ExecuteQueryText(SQLiteConnection connection, string queryText)
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

                createRow(dataTable.Rows[i], newRow);

                dataSet.Tables[0].Rows.Add(newRow);
            }
            new SQLiteCommandBuilder(dataAdapter);
            dataAdapter.Update(dataSet);
        }

        private static void createRow(DataRow memoryRow, DataRow sqlRow)
        {
            //memoryRow.ItemArray.Select((o, i) => sqlRow.ItemArray[i] = o);
            memoryRow.ItemArray.Select((o, i) => sqlRow[i] = o).AsParallel().ToArray();
        }
        public static string GetQueryTextCommonRecords(DataTable dataTable)
        {
            StringBuilder query = new StringBuilder();
            query.Append("insert into result ");
            query.Append("select ");
            query.Append(GetColumnNames(dataTable, "s"));
            query.Append("\"Available in both\"");
            query.Append(" from source s join target t on s.");
            query.Append("date");
            query.Append(" = ");
            query.Append("t.");
            query.Append("date");
            query.Append(" and ");
            query.Append("s.");
            query.Append("symbol");
            query.Append(" = ");
            query.Append("t.");
            query.Append("symbol");
            return query.ToString();
        }

        public static string GetQueryTextLeftRecords(DataTable dataTable)
        {
            StringBuilder query = new StringBuilder();
            query.Append("insert into result ");
            query.Append("select ");
            query.Append(GetColumnNames(dataTable, "s"));
            query.Append("\"Available in source\"");
            query.Append(" from source s left join target t on s.");
            query.Append("date");
            query.Append(" = ");
            query.Append("t.");
            query.Append("date");
            query.Append(" and ");
            query.Append("s.");
            query.Append("symbol");
            query.Append(" = ");
            query.Append("t.");
            query.Append("symbol");
            query.Append(" where t.date is null and t.symbol is null");
            return query.ToString();
        }

        public static String GetQueryTextRightRecords(DataTable dataTable)
        {
            StringBuilder query = new StringBuilder();
            query.Append("insert into result ");
            query.Append("select ");
            query.Append(GetColumnNames(dataTable, "t"));
            query.Append("\"Available in target\"");
            query.Append(" from  target t  left join source s on s.");
            query.Append("date");
            query.Append(" = ");
            query.Append("t.");
            query.Append("date");
            query.Append(" and ");
            query.Append("s.");
            query.Append("symbol");
            query.Append(" = ");
            query.Append("t.");
            query.Append("symbol");
            query.Append(" where s.date is null and s.symbol is null");
            return query.ToString();
        }

        public static string GetColumnNames(DataTable dataTable, string aliasName)
        {
            StringBuilder columnNames = new StringBuilder();

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                columnNames.Append(aliasName+ "." + dataTable.Columns[i].ColumnName + ",");
            }
            return columnNames.ToString();
        }
    }
}
