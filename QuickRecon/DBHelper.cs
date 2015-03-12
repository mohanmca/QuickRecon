using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace QuickRecon
{
    public class DBHelper
    {
        public static DbConnection GetConnection(string appName)
        {
            ConnectionStringSettings connectionStringSettings =
                 ConfigurationManager.ConnectionStrings[appName];

            DbProviderFactory factory =
              DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);

            DbConnection connection = factory.CreateConnection();

            if (connectionStringSettings.ProviderName == "SQLite Data Provider (Entity Framework 6)")
            {
                SQLiteConnection.CreateFile(connectionStringSettings.ConnectionString);
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

    }
}