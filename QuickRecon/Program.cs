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
            string source_file1 = ConfigurationManager.AppSettings.Get("first_source_csv_file_path");
            string source_file2 = ConfigurationManager.AppSettings.Get("second_source_csv_file_path");



            var dataTable1 = CSVReader.ReadCSVFile(source_file1, false,"A");
            var dataTable2 = CSVReader.ReadCSVFile(source_file2, false, "B");

            string databaseName = "temp100.sqlite";
            SqliteHelper.CreateDatabase(databaseName);

            var dbConnection = SqliteHelper.Connect(databaseName);

            var table1Text = SqliteHelper.GetCreateTableQuery(dbConnection, dataTable1, "A");
            var table2Text = SqliteHelper.GetCreateTableQuery(dbConnection, dataTable2, "B");

            SqliteHelper.CreateTable(dbConnection, table1Text);
            SqliteHelper.CreateTable(dbConnection, table2Text);

            SqliteHelper.InsertData(dbConnection, dataTable1);
            SqliteHelper.InsertData(dbConnection, dataTable2);

            SqliteHelper.Close(dbConnection);
        }
    }
}
