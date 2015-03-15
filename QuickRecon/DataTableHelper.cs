using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QuickRecon
{
    public static class DataTableHelper
    {
        public static IEnumerable<DataRow> GetRows(this DataTable dataTable)
        {
            return
                from row in dataTable.Rows.Cast<DataRow>()
                select row;
        }

        public static IEnumerable<string> GetColumnNames(this DataColumnCollection columns)
        {
            return 
                from c in columns.Cast<DataColumn>()
                select c.ColumnName;
        }

        public static IEnumerable<string> GetColumnNames(this DataTable dataTable)
        {
            return dataTable.Columns.GetColumnNames();
        }

        public static string GetColumnNamesWithAlias(this DataTable dataTable, string aliasName)
        {
            return dataTable
                .Columns
                .GetColumnNames()
                .GetPrefixedItems(aliasName + ".")
                .GetSuffixedItems(",")
                .Aggregate((current, next) => current + next)
                .TrimEnd(',');
        }
    }
}