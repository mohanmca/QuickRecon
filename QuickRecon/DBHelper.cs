﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;


namespace QuickRecon
{
    public static class DbHelper
    {
        public static DbConnection GetConnection(string appName)
        {
            ConnectionStringSettings connectionStringSettings =
                 ConfigurationManager.ConnectionStrings[appName];

            DbProviderFactory factory =
              DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);

            DbConnection connection = factory.CreateConnection();

            if (connectionStringSettings.ProviderName == "System.Data.SQLite")
            {
                SQLiteConnection.CreateFile(ConfigurationManager.AppSettings.Get("DATABASE_FILE_NAME"));
            }
            connection.ConnectionString = connectionStringSettings.ConnectionString;

            connection.Open();

            return connection;
        }
        public static void Close(DbConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public static void ExecuteQueryText(DbConnection connection, string queryText)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = queryText;
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }

        public static string GetCreateTableQuery(DataTable dataTable, string tableName)
        {
            return dataTable
                    .GetColumnNames()
                    .Select(name => name + " varchar(20),")
                    .Aggregate("create table " + tableName + " (",
                    (current, next) => current + next)
                    .TrimEnd(',') + ')';
        }

        public static void InsertData(string appName, DbConnection connection, DataTable dataTable)
        {

            ConnectionStringSettings connectionStringSettings =
               ConfigurationManager.ConnectionStrings[appName];

            DbProviderFactory factory =
              DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);

            string tableName = dataTable.TableName;

            var sqlQuery = "select * from " + tableName + " where 0 = 1";
            //var dataAdapter = new SQLiteDataAdapter(sqlQuery, connection);
            var dataAdapter = factory.CreateDataAdapter();
            DbCommand cmdSelect = factory.CreateCommand();
            cmdSelect.Connection = connection;
            cmdSelect.CommandText = sqlQuery;
            dataAdapter.SelectCommand = cmdSelect;
            var dataSet = new DataSet();
            dataAdapter.Fill(dataSet);

          
            Array.ForEach(dataTable.GetRows().ToArray(),
                row =>
                {
                    var newRow = dataSet.Tables[0].NewRow();
                    CreateRow(row, newRow);
                    dataSet.Tables[0].Rows.Add(newRow);
                });
            
            DbCommandBuilder cmdBuilder = factory.CreateCommandBuilder();
            cmdBuilder.DataAdapter = dataAdapter;
            dataAdapter.Update(dataSet);
        }
        private static void CreateRow(DataRow memoryRow, DataRow sqlRow)
        {
            memoryRow.ItemArray.Select((o, i) => sqlRow[i] = o).AsParallel().ToArray();
        }
        public static string GetQueryTextCommonRecords(DataTable dataTable)
        {
            StringBuilder query = new StringBuilder();
            query.Append("insert into result ");
            query.Append("select ");
            query.Append(dataTable.GetColumnNamesWithAlias("s"));
            query.Append(",");
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
            query.Append(dataTable.GetColumnNamesWithAlias("s"));
            query.Append(",");
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
            query.Append(dataTable.GetColumnNamesWithAlias("t"));
            query.Append(",");
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
    }
}