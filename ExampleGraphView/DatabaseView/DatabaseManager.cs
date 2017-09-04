using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace ExampleGraphView {
    public class DatabaseManager : IDatabaseManager {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public Database GetDatabase() {
            var database = new Database();
            using (var connection = new SqlConnection()) {
                connection.ConnectionString = GetConnectionString();
                connection.Open();
                database.Name = connection.Database;
                FetchTablesAndColumns(connection, database);
                FetchConstraints(connection, database);
            }
            return database;
        }

        private static string GetConnectionString() {
            return ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        }

        private static void FetchTablesAndColumns(SqlConnection connection, Database database) {
            var sqlColumns = "SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS";
            SqlCommand columnCommand = null;
            try {
                columnCommand = new SqlCommand(sqlColumns, connection);
                using (var reader = columnCommand.ExecuteReader()) {
                    while (reader.Read()) {
                        var tableName = reader["TABLE_NAME"].ToString();
                        var columnName = reader["COLUMN_NAME"].ToString();
                        if (!database.Tables.ContainsKey(tableName)) {
                            database.Tables[tableName] = new Table(database, tableName);
                        }
                        database.Tables[tableName].Columns[columnName] = new Column(database.Tables[tableName],
                            columnName);
                    }
                }
            } finally {
                columnCommand?.Dispose();
            }
        }

        private static void FetchConstraints(SqlConnection connection, Database database) {
            var sqlContraints = "SELECT KCU1.TABLE_NAME AS FK_TABLE_NAME, KCU1.COLUMN_NAME AS FK_COLUMN_NAME, KCU2.TABLE_NAME AS REFERENCED_TABLE_NAME, KCU2.COLUMN_NAME AS REFERENCED_COLUMN_NAME"
                                + Environment.NewLine
                                + "FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS RC"
                                + Environment.NewLine
                                + "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU1"
                                + " ON KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG"
                                + " AND KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA"
                                + " AND KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME"
                                + Environment.NewLine
                                + "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU2"
                                + " ON KCU2.CONSTRAINT_CATALOG = RC.UNIQUE_CONSTRAINT_CATALOG"
                                + " AND KCU2.CONSTRAINT_SCHEMA = RC.UNIQUE_CONSTRAINT_SCHEMA"
                                + " AND KCU2.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME"
                                + " AND KCU2.ORDINAL_POSITION = KCU1.ORDINAL_POSITION";
            SqlCommand constraintCommand = null;
            try {
                constraintCommand = new SqlCommand(sqlContraints, connection);
                using (var reader = constraintCommand.ExecuteReader()) {
                    while (reader.Read()) {
                        var fkTableName = reader["FK_TABLE_NAME"].ToString();
                        var fkColumnName = reader["FK_COLUMN_NAME"].ToString();
                        var refTableName = reader["REFERENCED_TABLE_NAME"].ToString();
                        var refColumnName = reader["REFERENCED_COLUMN_NAME"].ToString();
                        var fkColumn = database.Tables[fkTableName].Columns[fkColumnName];
                        var refColumn = database.Tables[refTableName].Columns[refColumnName];
                        database.Relations[fkColumn] = refColumn;
                    }
                }
            } finally {
                constraintCommand?.Dispose();
            }
        }
    }
}