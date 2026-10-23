using JewelleryShopAPI.Data;
using JewelleryShopAPI.Repositories;
using JewelleryShopAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Get connection string once
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=jewellery_shop.db";

// Automatic Database Migrations - Runs on every startup
try
{
    Console.WriteLine("\n🔄 Starting automatic database migrations...\n");
    JewelleryShopAPI.DatabaseMigrationHelper.EnsureAllMigrationsApplied(connectionString);
    Console.WriteLine("✅ All database migrations completed successfully!\n");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Database migration error: {ex.Message}");
    Console.WriteLine("The API will continue, but some features may not work properly.");
}

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (connectionString?.Contains("Server=") == true)
    {
        options.UseSqlServer(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString ?? "Data Source=jewelleryshop.db");
    }
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyForJewelleryShopManagement2026!@#$";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IEnquiryRepository, EnquiryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IGoldRateRepository, GoldRateRepository>();
builder.Services.AddScoped<ICraftAssignmentRepository, CraftAssignmentRepository>();
builder.Services.AddScoped<IMaterialDetailRepository, MaterialDetailRepository>();
builder.Services.AddScoped<ICraftCompletionRepository, CraftCompletionRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IShopSettingsRepository, ShopSettingsRepository>();
builder.Services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IEnquiryService, EnquiryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IWorkerService, WorkerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IGoldRateService, GoldRateService>();
builder.Services.AddScoped<ICraftWorkflowService, CraftWorkflowService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IShopSettingsService, ShopSettingsService>();
builder.Services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();

// Chat and Notification Services
// Note: ChatRepository already registered above with interface
// builder.Services.AddScoped<FirebaseNotificationService>();
// builder.Services.AddSingleton<AutoReplyService>();

var app = builder.Build();

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // Log the Swagger URL
    app.Logger.LogInformation("Swagger UI is available at: /swagger");
//}

// Uncomment this line if you want to force HTTPS redirection
// app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseStaticFiles(); // Serve uploaded profile photos from wwwroot

app.UseAuthentication();
app.UseAuthorization();

// WebSocket Support for Chat (uncomment when implementing real-time chat)
// app.UseWebSockets();
// app.UseMiddleware<ChatWebSocketMiddleware>();

app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
    
    // Add Gold24KRate column migration
    Console.WriteLine("==============================================");
    Console.WriteLine("  Running Gold24KRate Migration...");
    Console.WriteLine("==============================================");
    try
    {
        JewelleryShopAPI.DatabaseUpdater.AddGold24KRateColumn();
        Console.WriteLine("✓ Gold24KRate column migration completed!");
        app.Logger.LogInformation("Gold24KRate column migration completed");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Migration warning: {ex.Message}");
        app.Logger.LogWarning(ex, "Gold24KRate column migration - column may already exist");
    }
    Console.WriteLine("==============================================");
    Console.WriteLine();
    
    // Run database migrations on startup
    try
    {
        var migrationService = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>();
        await migrationService.ApplyMigrationsAsync();
        app.Logger.LogInformation("Database migrations completed successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error running database migrations on startup");
    }
    
    // Ensure default admin user exists
    Console.WriteLine("==============================================");
    Console.WriteLine("  Checking Admin User...");
    Console.WriteLine("==============================================");
    try
    {
        await JewelleryShopAPI.EnsureAdminUser.CreateDefaultAdminAsync(dbContext);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error ensuring admin user: {ex.Message}");
        app.Logger.LogError(ex, "Error ensuring admin user exists");
    }
    Console.WriteLine("==============================================");
    Console.WriteLine();
}

// Log application URLs
app.Logger.LogInformation("Application started successfully!");
app.Logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
app.Logger.LogInformation("Navigate to: https://localhost:5001/swagger or http://localhost:5000/swagger");

app.Run();
