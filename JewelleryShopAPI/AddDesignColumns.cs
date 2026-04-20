using System;
using System.Data.SQLite;

class AddDesignColumns
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
                ExecuteCommand(connection, "ALTER TABLE CraftAssignments ADD COLUMN DesignImage TEXT NULL;");
                Console.WriteLine("✓ Added DesignImage column to CraftAssignments");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DesignImage: {ex.Message}");
            }
            
            try
            {
                ExecuteCommand(connection, "ALTER TABLE Orders ADD COLUMN ExpectedCompletionDate TEXT NULL;");
                Console.WriteLine("✓ Added ExpectedCompletionDate column to Orders");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ExpectedCompletionDate: {ex.Message}");
            }
            
            Console.WriteLine("\nDatabase update completed!");
            Console.WriteLine("Please restart the API for changes to take effect.");
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
