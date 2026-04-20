using JewelleryShopAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace JewelleryShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly IDatabaseMigrationService _migrationService;

        public DatabaseController(IDatabaseMigrationService migrationService)
        {
            _migrationService = migrationService;
        }

        /// <summary>
        /// Apply all pending database migrations automatically
        /// </summary>
        [HttpPost("migrate")]
        public async Task<IActionResult> ApplyMigrations()
        {
            try
            {
                var result = await _migrationService.ApplyMigrationsAsync();
                
                if (result)
                {
                    return Ok(new { 
                        success = true, 
                        message = "✅ Database migrations applied successfully" 
                    });
                }
                else
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "❌ Failed to apply migrations" 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Error: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Clear all data from database (WARNING: Destructive operation)
        /// </summary>
        [Authorize]
        [HttpPost("clear")]
        public async Task<IActionResult> ClearDatabase([FromBody] ClearDatabaseRequest request)
        {
            Console.WriteLine($"🔍 Clear database request received. Confirmation: '{request?.Confirmation}'");
            
            // Safety check - require confirmation
            if (request == null || request.Confirmation != "DELETE ALL DATA")
            {
                Console.WriteLine($"⚠️ Invalid confirmation. Expected 'DELETE ALL DATA', got: '{request?.Confirmation}'");
                return BadRequest(new { 
                    success = false, 
                    message = "⚠️ Confirmation text must be 'DELETE ALL DATA'" 
                });
            }

            Console.WriteLine("✅ Confirmation verified, proceeding to clear database...");

            try
            {
                var result = await _migrationService.ClearDatabaseAsync();
                
                if (result)
                {
                    Console.WriteLine("✅ Database cleared successfully!");
                    return Ok(new { 
                        success = true, 
                        message = "🗑️ Database cleared successfully" 
                    });
                }
                else
                {
                    Console.WriteLine("❌ Database clear operation failed");
                    return BadRequest(new { 
                        success = false, 
                        message = "❌ Failed to clear database" 
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception during database clear: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Error: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// Get current migration status
        /// </summary>
        [HttpGet("migration-status")]
        public async Task<IActionResult> GetMigrationStatus()
        {
            try
            {
                var status = await _migrationService.GetMigrationStatusAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = $"Error: {ex.Message}" 
                });
            }
        }
    }

    public class ClearDatabaseRequest
    {
        [JsonPropertyName("confirmation")]
        public string Confirmation { get; set; } = string.Empty;
    }
}
