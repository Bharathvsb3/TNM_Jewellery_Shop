using Microsoft.EntityFrameworkCore;
using JewelleryShopAPI.Data;

namespace JewelleryShopAPI
{
    public class DatabaseUpdater
    {
        public static void UpdateEnquiriesTable()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=jewelleryshop.db");

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                var conn = context.Database.GetDbConnection();
                conn.Open();
                var cmd = conn.CreateCommand();

                string[] sqls = new[]
                {
                    "ALTER TABLE Enquiries ADD COLUMN MetalType TEXT NULL;",
                    "ALTER TABLE Enquiries ADD COLUMN JewelryType TEXT NULL;",
                    "ALTER TABLE Enquiries ADD COLUMN Purity TEXT NULL;",
                    "ALTER TABLE Enquiries ADD COLUMN EstimatedWeight REAL NULL;",
                    "ALTER TABLE Enquiries ADD COLUMN MakingType TEXT NULL;",
                    "ALTER TABLE Orders ADD COLUMN Gst REAL NULL;"
                };

                foreach (var sql in sqls)
                {
                    try
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"✓ Executed: {sql}");
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("duplicate column"))
                        {
                            Console.WriteLine($"○ Column already exists");
                        }
                        else
                        {
                            Console.WriteLine($"✗ Error: {ex.Message}");
                        }
                    }
                }

                conn.Close();
                Console.WriteLine("\nDatabase update completed!");
            }
        }

        public static void AddGstColumnToOrders()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=jewelleryshop.db");

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                var conn = context.Database.GetDbConnection();
                conn.Open();
                var cmd = conn.CreateCommand();

                string[] sqls = new[]
                {
                    "ALTER TABLE Orders ADD COLUMN Gst REAL NULL;"
                };

                foreach (var sql in sqls)
                {
                    try
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"✓ Executed: {sql}");
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("duplicate column"))
                        {
                            Console.WriteLine($"○ Column already exists");
                        }
                        else
                        {
                            Console.WriteLine($"✗ Error: {ex.Message}");
                        }
                    }
                }

                conn.Close();
                Console.WriteLine("\nGST column added to Orders table!");
            }
        }
        
        public static void AddDesignImageAndETAColumns()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=jewelleryshop.db");

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                var conn = context.Database.GetDbConnection();
                conn.Open();
                var cmd = conn.CreateCommand();

                string[] sqls = new[]
                {
                    "ALTER TABLE CraftAssignments ADD COLUMN DesignImage TEXT NULL;",
                    "ALTER TABLE Orders ADD COLUMN ExpectedCompletionDate TEXT NULL;"
                };

                foreach (var sql in sqls)
                {
                    try
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"✓ Executed: {sql}");
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("duplicate column"))
                        {
                            Console.WriteLine($"○ Column already exists");
                        }
                        else
                        {
                            Console.WriteLine($"✗ Error: {ex.Message}");
                        }
                    }
                }

                conn.Close();
                Console.WriteLine("\nDesignImage and ExpectedCompletionDate columns added!");
            }
        }

        public static void AddGold24KRateColumn()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=jewelleryshop.db");

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                var conn = context.Database.GetDbConnection();
                conn.Open();
                var cmd = conn.CreateCommand();

                string[] sqls = new[]
                {
                    "ALTER TABLE GoldRates ADD COLUMN Gold24KRate REAL NOT NULL DEFAULT 0;"
                };

                foreach (var sql in sqls)
                {
                    try
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"✓ Executed: {sql}");
                        Console.WriteLine("✓ Gold24KRate column added successfully!");
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("duplicate column"))
                        {
                            Console.WriteLine($"○ Gold24KRate column already exists");
                        }
                        else
                        {
                            Console.WriteLine($"✗ Error: {ex.Message}");
                        }
                    }
                }

                conn.Close();
                Console.WriteLine("\nGold24KRate migration completed!");
            }
        }
        
        public static void AddImageFieldsAndQuantity()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=jewelleryshop.db");

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                var conn = context.Database.GetDbConnection();
                conn.Open();
                var cmd = conn.CreateCommand();

                string[] sqls = new[]
                {
                    // Add DesignImage columns to Orders
                    "ALTER TABLE Orders ADD COLUMN DesignImage TEXT NULL;",
                    "ALTER TABLE Orders ADD COLUMN DesignImage2 TEXT NULL;",
                    "ALTER TABLE Orders ADD COLUMN DesignImage3 TEXT NULL;",
                    // Add DesignImage columns to Enquiries
                    "ALTER TABLE Enquiries ADD COLUMN DesignImage TEXT NULL;",
                    "ALTER TABLE Enquiries ADD COLUMN DesignImage2 TEXT NULL;",
                    "ALTER TABLE Enquiries ADD COLUMN DesignImage3 TEXT NULL;",
                    // Add Quantity column to Products
                    "ALTER TABLE Products ADD COLUMN Quantity INTEGER NULL;"
                };

                Console.WriteLine("Adding image fields and quantity column...\n");

                foreach (var sql in sqls)
                {
                    try
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                        Console.WriteLine($"[OK] Executed: {sql}");
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("duplicate column"))
                        {
                            Console.WriteLine($"[INFO] Column already exists (skipped)");
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR] {ex.Message}");
                        }
                    }
                }

                conn.Close();
                Console.WriteLine("\n[SUCCESS] Image fields and quantity migration completed!");
            }
        }
    }
}
