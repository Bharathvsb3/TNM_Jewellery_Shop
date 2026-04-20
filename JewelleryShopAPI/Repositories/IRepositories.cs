using JewelleryShopAPI.Models;

namespace JewelleryShopAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByMobileAsync(string mobile);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task<bool> DeleteAsync(int id);
    }

    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByMobileAsync(string mobile);
        Task<IEnumerable<Customer>> GetAllAsync(bool includeInactive = false);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(int id);
    }

    public interface IEnquiryRepository
    {
        Task<Enquiry?> GetByIdAsync(int id);
        Task<IEnumerable<Enquiry>> GetAllAsync();
        Task<IEnumerable<Enquiry>> GetByStatusAsync(string status);
        Task<Enquiry> CreateAsync(Enquiry enquiry);
        Task<Enquiry> UpdateAsync(Enquiry enquiry);
    }

    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateAsync(Order order);
    }

    public interface IWorkerRepository
    {
        Task<Worker?> GetByIdAsync(int id);
        Task<Worker?> GetByMobileAsync(string mobile);
        Task<IEnumerable<Worker>> GetAllAsync(bool includeInactive = false);
        Task<IEnumerable<Worker>> GetByTypeAsync(string type);
        Task<Worker> CreateAsync(Worker worker);
        Task<Worker> UpdateAsync(Worker worker);
        Task DeleteAsync(int id);
    }

    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync(bool includeInactive = false);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }

    public interface IGoldRateRepository
    {
        Task<GoldRate?> GetLatestAsync();
        Task<GoldRate?> GetByIdAsync(int id);
        Task<IEnumerable<GoldRate>> GetAllAsync();
        Task<IEnumerable<GoldRate>> GetByDateAsync(DateTime date);
        Task<GoldRate> CreateAsync(GoldRate goldRate);
        Task<GoldRate> UpdateAsync(GoldRate goldRate);
        Task<int> GetTodayEntryCountAsync();
    }

    public interface ICraftAssignmentRepository
    {
        Task<CraftAssignment?> GetByIdAsync(int id);
        Task<CraftAssignment?> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<CraftAssignment>> GetAllAsync();
        Task<IEnumerable<CraftAssignment>> GetByWorkerIdAsync(int workerId);
        Task<IEnumerable<CraftAssignment>> GetByStatusAsync(string status);
        Task<CraftAssignment> CreateAsync(CraftAssignment assignment);
        Task<CraftAssignment> UpdateAsync(CraftAssignment assignment);
    }

    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetConversationAsync(int userId1, int userId2);
        Task<IEnumerable<Message>> GetUserMessagesAsync(int userId);
        Task<Message> CreateAsync(Message message);
        Task MarkAsReadAsync(int messageId);
    }

    public interface IShopSettingsRepository
    {
        Task<ShopSettings> GetSettingsAsync();
        Task<ShopSettings> UpdateSettingsAsync(ShopSettings settings);
    }

    public interface IOrderStatusHistoryRepository
    {
        Task<IEnumerable<OrderStatusHistory>> GetByOrderIdAsync(int orderId);
        Task<OrderStatusHistory> CreateAsync(OrderStatusHistory statusHistory);
    }

    public interface IMaterialDetailRepository
    {
        Task<MaterialDetail?> GetByIdAsync(int id);
        Task<IEnumerable<MaterialDetail>> GetByCraftAssignmentIdAsync(int craftAssignmentId);
        Task<MaterialDetail> CreateAsync(MaterialDetail materialDetail);
        Task<MaterialDetail> UpdateAsync(MaterialDetail materialDetail);
        Task DeleteAsync(int id);
    }

    public interface ICraftCompletionRepository
    {
        Task<CraftCompletion?> GetByIdAsync(int id);
        Task<CraftCompletion?> GetByOrderIdAsync(int orderId);
        Task<CraftCompletion?> GetByCraftAssignmentIdAsync(int craftAssignmentId);
        Task<IEnumerable<CraftCompletion>> GetAllAsync();
        Task<CraftCompletion> CreateAsync(CraftCompletion completion);
        Task<CraftCompletion> UpdateAsync(CraftCompletion completion);
    }
}
