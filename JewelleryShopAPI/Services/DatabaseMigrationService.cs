using JewelleryShopAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JewelleryShopAPI.Services
{
    public interface IDatabaseMigrationService
    {
        Task<bool> ApplyMigrationsAsync();
        Task<bool> ClearDatabaseAsync();
        Task<MigrationStatus> GetMigrationStatusAsync();
    }

    public class DatabaseMigrationService : IDatabaseMigrationService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public DatabaseMigrationService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> ApplyMigrationsAsync()
        {
            try
            {
                var connectionString = _context.Database.GetConnectionString();
                using var connection = new SqliteConnection(connectionString);
                await connection.OpenAsync();

                // Check and add columns to Workers table
                await AddColumnIfNotExists(connection, "Workers", "PasswordHash", "TEXT NULL");

                // Check and add columns to CraftAssignments table
                await AddColumnIfNotExists(connection, "CraftAssignments", "CraftsmanStatus", "TEXT NULL");
                await AddColumnIfNotExists(connection, "CraftAssignments", "ReturnWeight", "REAL NULL");
                await AddColumnIfNotExists(connection, "CraftAssignments", "CraftsmanRemarks", "TEXT NULL");
                await AddColumnIfNotExists(connection, "CraftAssignments", "PendingAdminApproval", "INTEGER NOT NULL DEFAULT 0");
                await AddColumnIfNotExists(connection, "CraftAssignments", "AdminConfirmed", "INTEGER NOT NULL DEFAULT 0");
                await AddColumnIfNotExists(connection, "CraftAssignments", "AdminRemarks", "TEXT NULL");
                await AddColumnIfNotExists(connection, "CraftAssignments", "UpdateTimestamp", "TEXT NULL");

                // Check and add delivery columns to Orders table
                await AddColumnIfNotExists(connection, "Orders", "ActualWeight", "REAL NULL");
                await AddColumnIfNotExists(connection, "Orders", "WastageGrams", "REAL NULL");
                await AddColumnIfNotExists(connection, "Orders", "WastagePercent", "REAL NULL");
                await AddColumnIfNotExists(connection, "Orders", "AdjustmentAmount", "REAL NULL");
                await AddColumnIfNotExists(connection, "Orders", "DeliveryNotes", "TEXT NULL");
                await AddColumnIfNotExists(connection, "Orders", "DeliveryDate", "TEXT NULL");

                // Create GoldDispatches table if not exists
                await CreateGoldDispatchesTableIfNotExists(connection);

                // Create MaterialDetails table if not exists
                await CreateMaterialDetailsTableIfNotExists(connection);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration error: {ex.Message}");
                return false;
            }
        }

        private async Task AddColumnIfNotExists(SqliteConnection connection, string tableName, string columnName, string columnDefinition)
        {
            try
            {
                // Check if column exists
                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = $"PRAGMA table_info({tableName})";
                
                bool columnExists = false;
                using (var reader = await checkCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetString(1) == columnName)
                        {
                            columnExists = true;
                            break;
                        }
                    }
                }

                if (!columnExists)
                {
                    // Add column
                    var alterCommand = connection.CreateCommand();
                    alterCommand.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnDefinition}";
                    await alterCommand.ExecuteNonQueryAsync();
                    Console.WriteLine($"✅ Added column {tableName}.{columnName}");
                }
                else
                {
                    Console.WriteLine($"⏭️ Column {tableName}.{columnName} already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error adding column {tableName}.{columnName}: {ex.Message}");
            }
        }

        private async Task CreateGoldDispatchesTableIfNotExists(SqliteConnection connection)
        {
            try
            {
                // Check if table exists
                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='GoldDispatches'";
                var exists = await checkCommand.ExecuteScalarAsync();

                if (exists == null)
                {
                    // Create table
                    var createCommand = connection.CreateCommand();
                    createCommand.CommandText = @"
                        CREATE TABLE GoldDispatches (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            PersonName TEXT NOT NULL,
                            ShopName TEXT NOT NULL,
                            MaterialType TEXT NOT NULL DEFAULT 'Gold',
                            DispatchWeight REAL NOT NULL,
                            Remarks TEXT NULL,
                            Status TEXT NOT NULL DEFAULT 'pending',
                            DispatchDate TEXT NOT NULL DEFAULT (datetime('now')),
                            ReturnWeight REAL NULL,
                            ReturnDate TEXT NULL,
                            ReturnRemarks TEXT NULL,
                            CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                            UpdatedAt TEXT NULL
                        )";
                    await createCommand.ExecuteNonQueryAsync();
                    Console.WriteLine("✅ Created GoldDispatches table");
                }
                else
                {
                    Console.WriteLine("⏭️ GoldDispatches table already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating GoldDispatches table: {ex.Message}");
            }
        }

        private async Task CreateMaterialDetailsTableIfNotExists(SqliteConnection connection)
        {
            try
            {
                // Check if table exists
                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='MaterialDetails'";
                var exists = await checkCommand.ExecuteScalarAsync();

                if (exists == null)
                {
                    // Create table
                    var createCommand = connection.CreateCommand();
                    createCommand.CommandText = @"
                        CREATE TABLE MaterialDetails (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            CraftAssignmentId INTEGER NOT NULL,
                            MaterialType TEXT NOT NULL,
                            Description TEXT NOT NULL,
                            Weight REAL NOT NULL,
                            Unit TEXT NULL,
                            EstimatedValue REAL NULL,
                            Notes TEXT NULL,
                            CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
                            FOREIGN KEY (CraftAssignmentId) REFERENCES CraftAssignments(Id) ON DELETE CASCADE
                        )";
                    await createCommand.ExecuteNonQueryAsync();
                    Console.WriteLine("✅ Created MaterialDetails table");
                }
                else
                {
                    Console.WriteLine("⏭️ MaterialDetails table already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating MaterialDetails table: {ex.Message}");
            }
        }

        public async Task<bool> ClearDatabaseAsync()
        {
            try
            {
                var connectionString = _context.Database.GetConnectionString();
                using var connection = new SqliteConnection(connectionString);
                await connection.OpenAsync();

                // Disable foreign key constraints temporarily
                var disableFKCommand = connection.CreateCommand();
                disableFKCommand.CommandText = "PRAGMA foreign_keys = OFF";
                await disableFKCommand.ExecuteNonQueryAsync();
                Console.WriteLine("🔓 Disabled foreign key constraints");

                // Get all table names
                var getTablesCommand = connection.CreateCommand();
                getTablesCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' AND name != '__EFMigrationsHistory'";
                
                var tables = new List<string>();
                using (var reader = await getTablesCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }

                Console.WriteLine($"📋 Found {tables.Count} tables to clear");

                // Delete all data from each table
                foreach (var table in tables)
                {
                    try
                    {
                        var deleteCommand = connection.CreateCommand();
                        deleteCommand.CommandText = $"DELETE FROM {table}";
                        var rowsAffected = await deleteCommand.ExecuteNonQueryAsync();
                        Console.WriteLine($"🗑️ Cleared {rowsAffected} rows from table: {table}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error clearing table {table}: {ex.Message}");
                        throw; // Re-throw to ensure transaction fails
                    }
                }

                // Reset autoincrement counters
                var resetCommand = connection.CreateCommand();
                resetCommand.CommandText = "DELETE FROM sqlite_sequence";
                await resetCommand.ExecuteNonQueryAsync();
                Console.WriteLine("🔄 Reset autoincrement counters");

                // Re-enable foreign key constraints
                var enableFKCommand = connection.CreateCommand();
                enableFKCommand.CommandText = "PRAGMA foreign_keys = ON";
                await enableFKCommand.ExecuteNonQueryAsync();
                Console.WriteLine("🔒 Re-enabled foreign key constraints");

                // Vacuum the database to reclaim space
                var vacuumCommand = connection.CreateCommand();
                vacuumCommand.CommandText = "VACUUM";
                await vacuumCommand.ExecuteNonQueryAsync();
                Console.WriteLine("🧹 Database vacuumed");

                Console.WriteLine("✅ Database cleared successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error clearing database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<MigrationStatus> GetMigrationStatusAsync()
        {
            var status = new MigrationStatus();
            
            try
            {
                var connectionString = _context.Database.GetConnectionString();
                using var connection = new SqliteConnection(connectionString);
                await connection.OpenAsync();

                // Check Workers table
                status.WorkersPasswordHash = await ColumnExists(connection, "Workers", "PasswordHash");

                // Check CraftAssignments table
                status.CraftAssignmentsCraftsmanStatus = await ColumnExists(connection, "CraftAssignments", "CraftsmanStatus");
                status.CraftAssignmentsReturnWeight = await ColumnExists(connection, "CraftAssignments", "ReturnWeight");
                status.CraftAssignmentsPendingApproval = await ColumnExists(connection, "CraftAssignments", "PendingAdminApproval");

                // Check GoldDispatches table
                var checkTableCommand = connection.CreateCommand();
                checkTableCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='GoldDispatches'";
                status.GoldDispatchesTableExists = await checkTableCommand.ExecuteScalarAsync() != null;

                status.AllMigrationsApplied = status.WorkersPasswordHash && 
                                               status.CraftAssignmentsCraftsmanStatus && 
                                               status.CraftAssignmentsReturnWeight && 
                                               status.CraftAssignmentsPendingApproval &&
                                               status.GoldDispatchesTableExists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking migration status: {ex.Message}");
            }

            return status;
        }

        private async Task<bool> ColumnExists(SqliteConnection connection, string tableName, string columnName)
        {
            var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA table_info({tableName})";
            
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (reader.GetString(1) == columnName)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class MigrationStatus
    {
        public bool WorkersPasswordHash { get; set; }
        public bool CraftAssignmentsCraftsmanStatus { get; set; }
        public bool CraftAssignmentsReturnWeight { get; set; }
        public bool CraftAssignmentsPendingApproval { get; set; }
        public bool GoldDispatchesTableExists { get; set; }
        public bool AllMigrationsApplied { get; set; }
    }
}
