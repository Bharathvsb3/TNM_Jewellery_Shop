using JewelleryShopAPI.Data;
using JewelleryShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelleryShopAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByMobileAsync(string mobile)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Mobile == mobile);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.Where(u => u.IsActive).ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<Customer?> GetByMobileAsync(string mobile)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Mobile == mobile);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Customers.AsQueryable();
            if (!includeInactive)
            {
                query = query.Where(c => c.IsActive);
            }
            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            customer.UpdatedAt = DateTime.UtcNow;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class EnquiryRepository : IEnquiryRepository
    {
        private readonly AppDbContext _context;

        public EnquiryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Enquiry?> GetByIdAsync(int id)
        {
            return await _context.Enquiries
                .Include(e => e.Customer)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Enquiry>> GetAllAsync()
        {
            return await _context.Enquiries
                .Include(e => e.Customer)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enquiry>> GetByStatusAsync(string status)
        {
            return await _context.Enquiries
                .Include(e => e.Customer)
                .Where(e => e.Status == status)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<Enquiry> CreateAsync(Enquiry enquiry)
        {
            _context.Enquiries.Add(enquiry);
            await _context.SaveChangesAsync();
            return enquiry;
        }

        public async Task<Enquiry> UpdateAsync(Enquiry enquiry)
        {
            enquiry.UpdatedAt = DateTime.UtcNow;
            _context.Enquiries.Update(enquiry);
            await _context.SaveChangesAsync();
            return enquiry;
        }
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            order.UpdatedAt = DateTime.UtcNow;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }
    }

    public class WorkerRepository : IWorkerRepository
    {
        private readonly AppDbContext _context;

        public WorkerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Worker?> GetByIdAsync(int id)
        {
            return await _context.Workers.FindAsync(id);
        }

        public async Task<IEnumerable<Worker>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Workers.AsQueryable();
            if (!includeInactive)
            {
                query = query.Where(w => w.IsActive);
            }
            return await query.OrderBy(w => w.Name).ToListAsync();
        }

        public async Task<IEnumerable<Worker>> GetByTypeAsync(string type)
        {
            return await _context.Workers
                .Where(w => w.WorkerType == type && w.IsActive)
                .OrderBy(w => w.Name)
                .ToListAsync();
        }

        public async Task<Worker> CreateAsync(Worker worker)
        {
            _context.Workers.Add(worker);
            await _context.SaveChangesAsync();
            return worker;
        }

        public async Task<Worker> UpdateAsync(Worker worker)
        {
            worker.UpdatedAt = DateTime.UtcNow;
            _context.Workers.Update(worker);
            await _context.SaveChangesAsync();
            return worker;
        }

        public async Task<Worker?> GetByMobileAsync(string mobile)
        {
            return await _context.Workers
                .FirstOrDefaultAsync(w => w.Mobile == mobile);
        }

        public async Task DeleteAsync(int id)
        {
            var worker = await _context.Workers.FindAsync(id);
            if (worker != null)
            {
                _context.Workers.Remove(worker);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Products.AsQueryable();
            if (!includeInactive)
            {
                query = query.Where(p => p.IsActive);
            }
            return await query.OrderBy(p => p.Category).ThenBy(p => p.Name).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Where(p => p.Category == category && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class GoldRateRepository : IGoldRateRepository
    {
        private readonly AppDbContext _context;

        public GoldRateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GoldRate?> GetLatestAsync()
        {
            return await _context.GoldRates
                .OrderByDescending(g => g.EntryDate)
                .ThenByDescending(g => g.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<GoldRate?> GetByIdAsync(int id)
        {
            return await _context.GoldRates.FindAsync(id);
        }

        public async Task<IEnumerable<GoldRate>> GetAllAsync()
        {
            return await _context.GoldRates
                .OrderByDescending(g => g.EntryDate)
                .ThenByDescending(g => g.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<GoldRate>> GetByDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);
            return await _context.GoldRates
                .Where(g => g.EntryDate >= startDate && g.EntryDate < endDate)
                .OrderBy(g => g.TimeSlot)
                .ToListAsync();
        }

        public async Task<GoldRate> CreateAsync(GoldRate goldRate)
        {
            _context.GoldRates.Add(goldRate);
            await _context.SaveChangesAsync();
            return goldRate;
        }

        public async Task<GoldRate> UpdateAsync(GoldRate goldRate)
        {
            _context.GoldRates.Update(goldRate);
            await _context.SaveChangesAsync();
            return goldRate;
        }

        public async Task<int> GetTodayEntryCountAsync()
        {
            // Use India Standard Time (UTC+5:30)
            var istNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            var today = istNow.Date;
            var tomorrow = today.AddDays(1);
            return await _context.GoldRates
                .CountAsync(g => g.EntryDate >= today && g.EntryDate < tomorrow);
        }
    }

    public class CraftAssignmentRepository : ICraftAssignmentRepository
    {
        private readonly AppDbContext _context;

        public CraftAssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CraftAssignment?> GetByIdAsync(int id)
        {
            return await _context.CraftAssignments
                .Include(ca => ca.Order)
                    .ThenInclude(o => o.Customer)
                .Include(ca => ca.Worker)
                .Include(ca => ca.StoneDetails)
                .Include(ca => ca.MaterialDetails)
                .FirstOrDefaultAsync(ca => ca.Id == id);
        }

        public async Task<CraftAssignment?> GetByOrderIdAsync(int orderId)
        {
            return await _context.CraftAssignments
                .Include(ca => ca.Order)
                    .ThenInclude(o => o.Customer)
                .Include(ca => ca.Worker)
                .Include(ca => ca.StoneDetails)
                .Include(ca => ca.MaterialDetails)
                .FirstOrDefaultAsync(ca => ca.OrderId == orderId);
        }

        public async Task<IEnumerable<CraftAssignment>> GetAllAsync()
        {
            return await _context.CraftAssignments
                .Include(ca => ca.Order)
                    .ThenInclude(o => o.Customer)
                .Include(ca => ca.Worker)
                .Include(ca => ca.StoneDetails)
                .Include(ca => ca.MaterialDetails)
                .OrderByDescending(ca => ca.AssignedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<CraftAssignment>> GetByWorkerIdAsync(int workerId)
        {
            return await _context.CraftAssignments
                .Include(ca => ca.Order)
                    .ThenInclude(o => o.Customer)
                .Include(ca => ca.Worker)
                .Include(ca => ca.StoneDetails)
                .Include(ca => ca.MaterialDetails)
                .Where(ca => ca.WorkerId == workerId)
                .OrderByDescending(ca => ca.AssignedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<CraftAssignment>> GetByStatusAsync(string status)
        {
            return await _context.CraftAssignments
                .Include(ca => ca.Order)
                    .ThenInclude(o => o.Customer)
                .Include(ca => ca.Worker)
                .Include(ca => ca.StoneDetails)
                .Where(ca => ca.Status == status)
                .OrderByDescending(ca => ca.AssignedDate)
                .ToListAsync();
        }

        public async Task<CraftAssignment> CreateAsync(CraftAssignment assignment)
        {
            _context.CraftAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        public async Task<CraftAssignment> UpdateAsync(CraftAssignment assignment)
        {
            assignment.UpdatedAt = DateTime.UtcNow;
            _context.CraftAssignments.Update(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }
    }

    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(int userId1, int userId2)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                           (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetUserMessagesAsync(int userId)
        {
            return await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<Message> CreateAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task MarkAsReadAsync(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }

    public class ShopSettingsRepository : IShopSettingsRepository
    {
        private readonly AppDbContext _context;

        public ShopSettingsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShopSettings> GetSettingsAsync()
        {
            var settings = await _context.ShopSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new ShopSettings { ShopName = "T.N. Mani Gopi Jewellers" };
                _context.ShopSettings.Add(settings);
                await _context.SaveChangesAsync();
            }
            return settings;
        }

        public async Task<ShopSettings> UpdateSettingsAsync(ShopSettings settings)
        {
            settings.UpdatedAt = DateTime.UtcNow;
            _context.ShopSettings.Update(settings);
            await _context.SaveChangesAsync();
            return settings;
        }
    }

    public class OrderStatusHistoryRepository : IOrderStatusHistoryRepository
    {
        private readonly AppDbContext _context;

        public OrderStatusHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderStatusHistory>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderStatusHistories
                .Where(osh => osh.OrderId == orderId)
                .Include(osh => osh.UpdatedBy)
                .OrderBy(osh => osh.CreatedAt)
                .ToListAsync();
        }

        public async Task<OrderStatusHistory> CreateAsync(OrderStatusHistory statusHistory)
        {
            statusHistory.CreatedAt = DateTime.UtcNow;
            _context.OrderStatusHistories.Add(statusHistory);
            await _context.SaveChangesAsync();
            return statusHistory;
        }
    }

    public class MaterialDetailRepository : IMaterialDetailRepository
    {
        private readonly AppDbContext _context;

        public MaterialDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MaterialDetail?> GetByIdAsync(int id)
        {
            return await _context.MaterialDetails
                .Include(md => md.CraftAssignment)
                .FirstOrDefaultAsync(md => md.Id == id);
        }

        public async Task<IEnumerable<MaterialDetail>> GetByCraftAssignmentIdAsync(int craftAssignmentId)
        {
            return await _context.MaterialDetails
                .Where(md => md.CraftAssignmentId == craftAssignmentId)
                .ToListAsync();
        }

        public async Task<MaterialDetail> CreateAsync(MaterialDetail materialDetail)
        {
            materialDetail.CreatedAt = DateTime.UtcNow;
            _context.MaterialDetails.Add(materialDetail);
            await _context.SaveChangesAsync();
            return materialDetail;
        }

        public async Task<MaterialDetail> UpdateAsync(MaterialDetail materialDetail)
        {
            _context.MaterialDetails.Update(materialDetail);
            await _context.SaveChangesAsync();
            return materialDetail;
        }

        public async Task DeleteAsync(int id)
        {
            var materialDetail = await _context.MaterialDetails.FindAsync(id);
            if (materialDetail != null)
            {
                _context.MaterialDetails.Remove(materialDetail);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class CraftCompletionRepository : ICraftCompletionRepository
    {
        private readonly AppDbContext _context;

        public CraftCompletionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CraftCompletion?> GetByIdAsync(int id)
        {
            return await _context.CraftCompletions
                .Include(cc => cc.Order)
                .Include(cc => cc.CraftAssignment)
                .FirstOrDefaultAsync(cc => cc.Id == id);
        }

        public async Task<CraftCompletion?> GetByOrderIdAsync(int orderId)
        {
            return await _context.CraftCompletions
                .Include(cc => cc.CraftAssignment)
                .FirstOrDefaultAsync(cc => cc.OrderId == orderId);
        }

        public async Task<CraftCompletion?> GetByCraftAssignmentIdAsync(int craftAssignmentId)
        {
            return await _context.CraftCompletions
                .Include(cc => cc.Order)
                .FirstOrDefaultAsync(cc => cc.CraftAssignmentId == craftAssignmentId);
        }

        public async Task<IEnumerable<CraftCompletion>> GetAllAsync()
        {
            return await _context.CraftCompletions
                .Include(cc => cc.Order)
                .Include(cc => cc.CraftAssignment)
                .ToListAsync();
        }

        public async Task<CraftCompletion> CreateAsync(CraftCompletion completion)
        {
            completion.CompletedAt = DateTime.UtcNow;
            _context.CraftCompletions.Add(completion);
            await _context.SaveChangesAsync();
            return completion;
        }

        public async Task<CraftCompletion> UpdateAsync(CraftCompletion completion)
        {
            _context.CraftCompletions.Update(completion);
            await _context.SaveChangesAsync();
            return completion;
        }
    }
}
