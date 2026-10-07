namespace SafeParentControl.Queries
{
    using Microsoft.Data.Sqlite;
    using System;

    public class DatabaseManager
    {
        private static readonly string DbFile = "safe_control.db";
        private static readonly string ConnectionString = $"Data Source={DbFile}";

        public static void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                // Черный список (угрозы, запрещенные, предупреждения)
                string createBlacklistTable = @"
                    CREATE TABLE IF NOT EXISTS BlacklistSites (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Url TEXT NOT NULL,
                        Category TEXT NOT NULL,
                        PrivilegeLevel TEXT NOT NULL, -- 'Block', 'Warning', 'Unclassified'
                        DateAdded DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";

                // Белый список (разрешенные, безопасные, доверенные)
                string createWhitelistTable = @"
                    CREATE TABLE IF NOT EXISTS WhitelistSites (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Url TEXT NOT NULL,
                        Category TEXT NOT NULL,
                        DateAdded DATETIME DEFAULT CURRENT_TIMESTAMP
                    );";

                using (var command = new SqliteCommand(createBlacklistTable, connection))
                {
                    command.ExecuteNonQuery();
                }

                using (var command = new SqliteCommand(createWhitelistTable, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Добавление в черный список
        public static void AddToBlacklist(string url, string category, string privilegeLevel)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO BlacklistSites (Url, Category, PrivilegeLevel) VALUES (@url, @category, @privilegeLevel);";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@url", url);
                    command.Parameters.AddWithValue("@category", category);
                    command.Parameters.AddWithValue("@privilegeLevel", privilegeLevel);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Добавление в белый список
        public static void AddToWhitelist(string url, string category)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                string query = "INSERT INTO WhitelistSites (Url, Category) VALUES (@url, @category);";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@url", url);
                    command.Parameters.AddWithValue("@category", category);
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void LogThreat(string url, string category)
        {
            // Vyzyvaem nash metod dobavleniya v cherniy spisok
            AddToBlacklist(url, category, "Block");
        }
    }
}