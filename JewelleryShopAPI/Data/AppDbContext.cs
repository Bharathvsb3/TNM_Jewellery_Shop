using JewelleryShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelleryShopAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<GoldRate> GoldRates { get; set; }
        public DbSet<CraftAssignment> CraftAssignments { get; set; }
        public DbSet<StoneDetail> StoneDetails { get; set; }
        public DbSet<MaterialDetail> MaterialDetails { get; set; }
        public DbSet<CraftCompletion> CraftCompletions { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ShopSettings> ShopSettings { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<GoldDispatch> GoldDispatches { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User unique mobile
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Mobile)
                .IsUnique();

            // Customer unique mobile
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Mobile)
                .IsUnique();

            // Decimal precision - 3 decimal places
            modelBuilder.Entity<Product>()
                .Property(p => p.Weight)
                .HasPrecision(18, 3);

            modelBuilder.Entity<Order>()
                .Property(o => o.GrossWeight)
                .HasPrecision(18, 3);

            modelBuilder.Entity<Order>()
                .Property(o => o.Wastage)
                .HasPrecision(18, 3);

            modelBuilder.Entity<Order>()
                .Property(o => o.MakingCharges)
                .HasPrecision(18, 3);

            modelBuilder.Entity<Order>()
                .Property(o => o.EstimatedWeight)
                .HasPrecision(18, 3);

            modelBuilder.Entity<Order>()
                .Property(o => o.GoldRate)
                .HasPrecision(18, 3);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 3);

            modelBuilder.Entity<Order>()
                .Property(o => o.AdvanceAmount)
                .HasPrecision(18, 3);

            modelBuilder.Entity<Order>()
                .Property(o => o.BalanceAmount)
                .HasPrecision(18, 3);

            modelBuilder.Entity<GoldRate>()
                .Property(g => g.Gold916Rate)
                .HasPrecision(18, 3);

            modelBuilder.Entity<GoldRate>()
                .Property(g => g.GoldNormalRate)
                .HasPrecision(18, 3);

            modelBuilder.Entity<GoldRate>()
                .Property(g => g.SilverRate)
                .HasPrecision(18, 3);

            modelBuilder.Entity<CraftAssignment>()
                .Property(c => c.RawGoldGiven)
                .HasPrecision(18, 3);

            modelBuilder.Entity<CraftAssignment>()
                .Property(c => c.FinalWeight)
                .HasPrecision(18, 3);

            modelBuilder.Entity<CraftAssignment>()
                .Property(c => c.StoneWeight)
                .HasPrecision(18, 3);

            modelBuilder.Entity<CraftAssignment>()
                .Property(c => c.OtherMaterialWeight)
                .HasPrecision(18, 3);

            modelBuilder.Entity<CraftAssignment>()
                .Property(c => c.UsedGold)
                .HasPrecision(18, 3);

            modelBuilder.Entity<CraftAssignment>()
                .Property(c => c.BalanceGold)
                .HasPrecision(18, 3);

            modelBuilder.Entity<CraftAssignment>()
                .Property(c => c.ManualUsedGold)
                .HasPrecision(18, 3);

            modelBuilder.Entity<CraftAssignment>()
                .Property(c => c.ManualBalanceGold)
                .HasPrecision(18, 3);

            modelBuilder.Entity<StoneDetail>()
                .Property(s => s.Weight)
                .HasPrecision(18, 3);

            modelBuilder.Entity<Enquiry>()
                .Property(e => e.EstimatedWeight)
                .HasPrecision(18, 3);

            // Relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enquiry>()
                .HasOne(e => e.Customer)
                .WithMany(c => c.Enquiries)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CraftAssignment>()
                .HasOne(ca => ca.Order)
                .WithOne(o => o.CraftAssignment)
                .HasForeignKey<CraftAssignment>(ca => ca.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CraftAssignment>()
                .HasOne(ca => ca.Worker)
                .WithMany(w => w.CraftAssignments)
                .HasForeignKey(ca => ca.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StoneDetail>()
                .HasOne(sd => sd.CraftAssignment)
                .WithMany(ca => ca.StoneDetails)
                .HasForeignKey(sd => sd.CraftAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderStatusHistory>()
                .HasOne(osh => osh.Order)
                .WithMany()
                .HasForeignKey(osh => osh.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderStatusHistory>()
                .HasOne(osh => osh.UpdatedBy)
                .WithMany()
                .HasForeignKey(osh => osh.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed default admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin",
                    Mobile = "9999999999",
                    Email = "admin@jewellery.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "ADMIN",
                    AccountStatus = "Approved",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed default shop settings
            modelBuilder.Entity<ShopSettings>().HasData(
                new ShopSettings
                {
                    Id = 1,
                    ShopName = "T.N. Mani Gopi Jewellers",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
