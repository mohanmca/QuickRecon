using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
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
            string appName = ConfigurationManager.AppSettings.Get("APPLICATION_CONN_STRING");

            string sourceDataTableName = "source";
            string targetDataTableName = "target";
            string resultDataTableName = "result";

            var sourceDataTable = CSVReader.ReadCSVFile(source, true, sourceDataTableName);
            var targetDataTable = CSVReader.ReadCSVFile(target, true, targetDataTableName);

            DbConnection connection = DbHelper.GetConnection(appName);



            var sourceTableQueryText = DbHelper.GetCreateTableQuery(sourceDataTable, sourceDataTableName);
            var targetTableQueryText = DbHelper.GetCreateTableQuery(targetDataTable, targetDataTableName);

            DbHelper.ExecuteQueryText(connection, sourceTableQueryText);
            DbHelper.ExecuteQueryText(connection, targetTableQueryText);

            DbHelper.InsertData(appName, connection, sourceDataTable);
            DbHelper.InsertData(appName, connection, targetDataTable);

            var resultTableQueryText = DbHelper.GetCreateTableQuery(sourceDataTable, resultDataTableName);
            DbHelper.ExecuteQueryText(connection, resultTableQueryText);
            DbHelper.ExecuteQueryText(connection, "ALTER TABLE " + resultDataTableName + " ADD COLUMN IsAvailable varchar(20);");

            var insertCommonDataQueryText = DbHelper.GetQueryTextCommonRecords(sourceDataTable);
            DbHelper.ExecuteQueryText(connection, insertCommonDataQueryText);

            var insertSourceDataOnlyQueryText = DbHelper.GetQueryTextLeftRecords(sourceDataTable);
            DbHelper.ExecuteQueryText(connection, insertSourceDataOnlyQueryText);

            var insertTargetDataOnlyQueryText = DbHelper.GetQueryTextRightRecords(targetDataTable);
            DbHelper.ExecuteQueryText(connection, insertTargetDataOnlyQueryText);

            DbHelper.Close(connection);
        }
    }
}
//both
//first
//second