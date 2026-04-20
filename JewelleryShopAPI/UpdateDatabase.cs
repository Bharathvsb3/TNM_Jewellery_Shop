using System;
using System.Data.SQLite;

class UpdateDatabase
{
    static void Main()
    {
        string dbPath = "jewelleryshop.db";
        string connectionString = $"Data Source={dbPath};Version=3;";
        
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            
            try
            {
                ExecuteCommand(connection, "ALTER TABLE Enquiries ADD COLUMN MetalType TEXT NULL;");
                Console.WriteLine("✓ Added MetalType column");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MetalType: {ex.Message}");
            }
            
            try
            {
                ExecuteCommand(connection, "ALTER TABLE Enquiries ADD COLUMN JewelryType TEXT NULL;");
                Console.WriteLine("✓ Added JewelryType column");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JewelryType: {ex.Message}");
            }
            
            try
            {
                ExecuteCommand(connection, "ALTER TABLE Enquiries ADD COLUMN Purity TEXT NULL;");
                Console.WriteLine("✓ Added Purity column");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Purity: {ex.Message}");
            }
            
            try
            {
                ExecuteCommand(connection, "ALTER TABLE Enquiries ADD COLUMN EstimatedWeight REAL NULL;");
                Console.WriteLine("✓ Added EstimatedWeight column");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EstimatedWeight: {ex.Message}");
            }
            
            try
            {
                ExecuteCommand(connection, "ALTER TABLE Enquiries ADD COLUMN MakingType TEXT NULL;");
                Console.WriteLine("✓ Added MakingType column");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MakingType: {ex.Message}");
            }
            
            try
            {
                ExecuteCommand(connection, "ALTER TABLE CraftAssignments ADD COLUMN DesignImage TEXT NULL;");
                Console.WriteLine("✓ Added DesignImage column");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DesignImage: {ex.Message}");
            }
            
            try
            {
                ExecuteCommand(connection, "ALTER TABLE Orders ADD COLUMN ExpectedCompletionDate TEXT NULL;");
                Console.WriteLine("✓ Added ExpectedCompletionDate column");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ExpectedCompletionDate: {ex.Message}");
            }
            
            Console.WriteLine("\nDatabase update completed!");
        }
    }
    
    static void ExecuteCommand(SQLiteConnection connection, string sql)
    {
        using (var command = new SQLiteCommand(sql, connection))
        {
            command.ExecuteNonQuery();
        }
    }
}
