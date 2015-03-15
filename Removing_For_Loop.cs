Removing For Loop exercise

 //string sql = "create table highscores (name varchar(20), score int)";
public static string GetCreateTableQuery(DataTable dataTable, string tableName)
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

Questions:
-----------

1. What is?

input: data table with sef of columns, table name
output: create table, query text

declarative: 
init
end
