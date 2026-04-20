using JewelleryShopAPI.Data;
using JewelleryShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelleryShopAPI
{
    public static class EnsureAdminUser
    {
        public static async Task CreateDefaultAdminAsync(AppDbContext context)
        {
            try
            {
                // Check if admin user already exists
                var adminExists = await context.Users.AnyAsync(u => u.Mobile == "9999999999" || u.Role == "ADMIN");
                
                if (!adminExists)
                {//use those files i need this type only
                //    Console.WriteLine("Creating default admin user...");
                    
                    var adminUser = new User
                    {
                        Name = "Admin",
                        Mobile = "9999999999",
                        Email = "admin@jewellery.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        Role = "ADMIN",
                        AccountStatus = "Approved",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    context.Users.Add(adminUser);
                    await context.SaveChangesAsync();
                    
                    Console.WriteLine("✓ Default admin user created successfully!");
                    Console.WriteLine("  Mobile: 9999999999");
                    Console.WriteLine("  Email: admin@jewellery.com");
                    Console.WriteLine("  Password: admin123");
                }
                else
                {
                    Console.WriteLine("✓ Admin user already exists");
                    
                    // List all admin users for debugging
                    var admins = await context.Users
                        .Where(u => u.Role == "ADMIN")
                        .Select(u => new { u.Name, u.Mobile, u.Email, u.IsActive })
                        .ToListAsync();
                    
                    Console.WriteLine($"  Found {admins.Count} admin user(s):");
                    foreach (var admin in admins)
                    {
                        Console.WriteLine($"    - {admin.Name} | Mobile: {admin.Mobile} | Email: {admin.Email} | Active: {admin.IsActive}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error ensuring admin user: {ex.Message}");
            }
        }
    }
}
