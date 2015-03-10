using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickRecon
{
    class Program
    {
        static void Main(string[] args)
        {
            string source = ConfigurationManager.AppSettings.Get("SOURCE_PATH");
            string target = ConfigurationManager.AppSettings.Get("TARGET_PATH");
            string databaseName = ConfigurationManager.AppSettings.Get("DATABASE_FILE_NAME");
            string sourceDataTableName = "source";
            string targetDataTableName = "target";
            string resultDataTableName = "result";

            var sourceDataTable = CSVReader.ReadCSVFile(source, true, sourceDataTableName);
            var targetDataTable = CSVReader.ReadCSVFile(target, true, targetDataTableName);

            
            SqliteHelper.CreateDatabase(databaseName);

            var dbConnection = SqliteHelper.Connect(databaseName);

            var sourceTableQueryText = SqliteHelper.GetCreateTableQuery(sourceDataTable, sourceDataTableName);
            var targetTableQueryText = SqliteHelper.GetCreateTableQuery(targetDataTable, targetDataTableName);

            SqliteHelper.ExecuteQueryText(dbConnection, sourceTableQueryText);
            SqliteHelper.ExecuteQueryText(dbConnection, targetTableQueryText);

            SqliteHelper.InsertData(dbConnection, sourceDataTable);
            SqliteHelper.InsertData(dbConnection, targetDataTable);

            var resultTableQueryText = SqliteHelper.GetCreateTableQuery(sourceDataTable, resultDataTableName);
            SqliteHelper.ExecuteQueryText(dbConnection, resultTableQueryText);
            SqliteHelper.ExecuteQueryText(dbConnection, "ALTER TABLE " + resultDataTableName + " ADD COLUMN IsAvailable varchar(20);");

            var insertCommonDataQueryText = SqliteHelper.GetQueryTextCommonRecords(sourceDataTable);
            SqliteHelper.ExecuteQueryText(dbConnection, insertCommonDataQueryText);

            var insertSourceDataOnlyQueryText = SqliteHelper.GetQueryTextLeftRecords(sourceDataTable);
            SqliteHelper.ExecuteQueryText(dbConnection, insertSourceDataOnlyQueryText);

            var insertTargetDataOnlyQueryText = SqliteHelper.GetQueryTextRightRecords(targetDataTable);
            SqliteHelper.ExecuteQueryText(dbConnection, insertTargetDataOnlyQueryText);

            SqliteHelper.Close(dbConnection);
        }
    }
}
//both
//first
//second