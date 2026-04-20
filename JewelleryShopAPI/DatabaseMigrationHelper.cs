using Microsoft.Data.Sqlite;
using System;

namespace JewelleryShopAPI
{
    public static class DatabaseMigrationHelper
    {
        /// <summary>
        /// Automatically checks and applies all pending database migrations on startup
        /// </summary>
        public static void EnsureAllMigrationsApplied(string connectionString)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                Console.WriteLine("==========================================");
                Console.WriteLine("Starting Automatic Database Migrations...");
                Console.WriteLine("==========================================\n");

                // Migrate Enquiries table
                EnsureEnquiryColumnsExist(connection);

                // Migrate Products table
                EnsureProductColumnsExist(connection);

                // Migrate Chat tables
                EnsureChatTablesExist(connection);

                // Migrate Enquiry-Order Linking
                EnsureEnquiryOrderLinkingExists(connection);

                // Migrate Customer Defaults
                EnsureCustomerDefaultsExist(connection);

                Console.WriteLine("==========================================");
                Console.WriteLine("All Database Migrations Completed!");
                Console.WriteLine("==========================================\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during automatic database migration: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public static void EnsureEnquiryColumnsExist(string connectionString)
        {
            try
            {
                using var connection = new SqliteConnection(connectionString);
                connection.Open();
                EnsureEnquiryColumnsExist(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Enquiries migration: {ex.Message}");
                throw;
            }
        }

        private static void EnsureEnquiryColumnsExist(SqliteConnection connection)
        {
            Console.WriteLine("Checking Enquiries table structure...");

            var columnsToAdd = new[]
            {
                new { Name = "Notes", Type = "TEXT" },
                new { Name = "ExpectedDate", Type = "TEXT" },
                new { Name = "FollowupDate", Type = "TEXT" }
            };

            foreach (var column in columnsToAdd)
            {
                if (!ColumnExists(connection, "Enquiries", column.Name))
                {
                    Console.WriteLine($"  → Adding column: Enquiries.{column.Name}");
                    AddColumn(connection, "Enquiries", column.Name, column.Type);
                    Console.WriteLine($"  ✓ Column {column.Name} added successfully");
                }
                else
                {
                    Console.WriteLine($"  ✓ Column {column.Name} already exists");
                }
            }

            Console.WriteLine();
        }

        private static void EnsureProductColumnsExist(SqliteConnection connection)
        {
            Console.WriteLine("Checking Products table structure...");

            var columnsToAdd = new[]
            {
                new { Name = "JewelryType", Type = "TEXT" },
                new { Name = "Purity", Type = "INTEGER" },
                new { Name = "Wastage", Type = "REAL" },
                new { Name = "MakingCharges", Type = "REAL" },
                new { Name = "Gst", Type = "REAL" },
                new { Name = "StoneDetails", Type = "TEXT" },
                new { Name = "Description", Type = "TEXT" }
            };

            foreach (var column in columnsToAdd)
            {
                if (!ColumnExists(connection, "Products", column.Name))
                {
                    Console.WriteLine($"  → Adding column: Products.{column.Name}");
                    AddColumn(connection, "Products", column.Name, column.Type);
                    Console.WriteLine($"  ✓ Column {column.Name} added successfully");
                }
                else
                {
                    Console.WriteLine($"  ✓ Column {column.Name} already exists");
                }
            }

            Console.WriteLine();
        }

        private static void EnsureChatTablesExist(SqliteConnection connection)
        {
            Console.WriteLine("Checking Chat tables...");

            // Check if Chatusr table exists
            if (!TableExists(connection, "Chatusr"))
            {
                Console.WriteLine("  → Creating table: Chatusr");
                CreateChatUsrTable(connection);
                Console.WriteLine("  ✓ Table Chatusr created successfully");
            }
            else
            {
                Console.WriteLine("  ✓ Table Chatusr already exists");
            }

            // Check if Chathst table exists
            if (!TableExists(connection, "Chathst"))
            {
                Console.WriteLine("  → Creating table: Chathst");
                CreateChatHstTable(connection);
                Console.WriteLine("  ✓ Table Chathst created successfully");
            }
            else
            {
                Console.WriteLine("  ✓ Table Chathst already exists");
            }

            Console.WriteLine();
        }

        private static bool TableExists(SqliteConnection connection, string tableName)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
            cmd.Parameters.AddWithValue("@tableName", tableName);
            
            using var reader = cmd.ExecuteReader();
            return reader.Read();
        }

        private static void CreateChatUsrTable(SqliteConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE Chatusr (
                    id_col INTEGER PRIMARY KEY AUTOINCREMENT,
                    userid TEXT NOT NULL,
                    fcmtoken TEXT NOT NULL,
                    devicetype TEXT,
                    status INTEGER DEFAULT 1,
                    adduser TEXT,
                    adddate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    addtime TEXT
                );
                CREATE INDEX idx_chatusr_userid ON Chatusr(userid);
            ";
            cmd.ExecuteNonQuery();
        }

        private static void CreateChatHstTable(SqliteConnection connection)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE Chathst (
                    id_col INTEGER PRIMARY KEY AUTOINCREMENT,
                    adddate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    addtime TEXT,
                    adduser TEXT,
                    fromusr TEXT NOT NULL,
                    tousr TEXT NOT NULL,
                    msgtxt TEXT NOT NULL,
                    isread INTEGER DEFAULT 0
                );
                CREATE INDEX idx_chathst_fromusr ON Chathst(fromusr);
                CREATE INDEX idx_chathst_tousr ON Chathst(tousr);
                CREATE INDEX idx_chathst_isread ON Chathst(isread);
                CREATE INDEX idx_chathst_adddate ON Chathst(adddate DESC);
            ";
            cmd.ExecuteNonQuery();
        }

        private static bool ColumnExists(SqliteConnection connection, string tableName, string columnName)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"PRAGMA table_info({tableName})";
            
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var name = reader.GetString(1); // Column name is at index 1
                if (name.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            
            return false;
        }

        private static void AddColumn(SqliteConnection connection, string tableName, string columnName, string columnType)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType}";
            cmd.ExecuteNonQuery();
        }

        private static void EnsureEnquiryOrderLinkingExists(SqliteConnection connection)
        {
            Console.WriteLine("Checking Enquiry-Order linking columns...");

            // Add ConvertedToOrderId to Enquiries table
            if (!ColumnExists(connection, "Enquiries", "ConvertedToOrderId"))
            {
                Console.WriteLine("  → Adding column: Enquiries.ConvertedToOrderId");
                AddColumn(connection, "Enquiries", "ConvertedToOrderId", "INTEGER");
                Console.WriteLine("  ✓ Column ConvertedToOrderId added successfully");
            }
            else
            {
                Console.WriteLine("  ✓ Column ConvertedToOrderId already exists");
            }

            // Add EnquiryId to Orders table
            if (!ColumnExists(connection, "Orders", "EnquiryId"))
            {
                Console.WriteLine("  → Adding column: Orders.EnquiryId");
                AddColumn(connection, "Orders", "EnquiryId", "INTEGER");
                Console.WriteLine("  ✓ Column EnquiryId added successfully");
            }
            else
            {
                Console.WriteLine("  ✓ Column EnquiryId already exists");
            }

            Console.WriteLine();
        }

        private static void EnsureCustomerDefaultsExist(SqliteConnection connection)
        {
            Console.WriteLine("Checking Customer default value columns...");

            var columnsToAdd = new[]
            {
                new { Name = "DefaultWastage", Type = "REAL DEFAULT 0" },
                new { Name = "DefaultGst", Type = "REAL DEFAULT 0" }
            };

            foreach (var column in columnsToAdd)
            {
                if (!ColumnExists(connection, "Customers", column.Name))
                {
                    Console.WriteLine($"  → Adding column: Customers.{column.Name}");
                    AddColumn(connection, "Customers", column.Name, column.Type);
                    Console.WriteLine($"  ✓ Column {column.Name} added successfully");
                }
                else
                {
                    Console.WriteLine($"  ✓ Column {column.Name} already exists");
                }
            }

            Console.WriteLine();
        }
    }
}
