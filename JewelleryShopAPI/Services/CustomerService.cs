using JewelleryShopAPI.DTOs;
using JewelleryShopAPI.Models;
using JewelleryShopAPI.Repositories;
using JewelleryShopAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace JewelleryShopAPI.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync(bool includeInactive = false);
        Task<CustomerDTO?> GetCustomerByIdAsync(int id);
        Task<CustomerDTO?> CreateCustomerAsync(CreateCustomerRequest request);
        Task<CustomerDTO?> UpdateCustomerAsync(int id, UpdateCustomerRequest request);
        Task<bool> DeleteCustomerAsync(int id);
        Task<bool> UpdateProfilePhotoAsync(int id, string? photoPath);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        public CustomerService(ICustomerRepository customerRepository, IUserRepository userRepository, AppDbContext context)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync(bool includeInactive = false)
        {
            var customers = await _customerRepository.GetAllAsync(includeInactive);
            var customerDTOs = new List<CustomerDTO>();
            
            foreach (var customer in customers)
            {
                customerDTOs.Add(await MapToDTOAsync(customer));
            }
            
            return customerDTOs;
        }

        public async Task<CustomerDTO?> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return customer != null ? await MapToDTOAsync(customer) : null;
        }

        public async Task<CustomerDTO?> CreateCustomerAsync(CreateCustomerRequest request)
        {
            // Check duplicate mobile
            var existing = await _customerRepository.GetByMobileAsync(request.Mobile);
            if (existing != null)
                return null;

            var customer = new Customer
            {
                Name = request.Name,
                Mobile = request.Mobile,
                Notes = request.Notes,
                DefaultWastage = request.DefaultWastage,
                DefaultGst = request.DefaultGst,
                IsActive = true
            };

            await _customerRepository.CreateAsync(customer);
            
            // Update associated user account if exists (for email/address)
            var user = await _userRepository.GetByMobileAsync(request.Mobile);
            if (user != null && user.Role == "CUSTOMER")
            {
                user.Email = request.Email;
                user.Address = request.Address;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
            
            return await MapToDTOAsync(customer);
        }

        public async Task<CustomerDTO?> UpdateCustomerAsync(int id, UpdateCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return null;

            // Check duplicate mobile (except current)
            var existing = await _customerRepository.GetByMobileAsync(request.Mobile);
            if (existing != null && existing.Id != id)
                return null;

            customer.Name = request.Name;
            customer.Mobile = request.Mobile;
            customer.Notes = request.Notes;
            customer.DefaultWastage = request.DefaultWastage;
            customer.DefaultGst = request.DefaultGst;
            customer.IsActive = request.IsActive;

            await _customerRepository.UpdateAsync(customer);
            
            // Update associated user account (email, address, password)
            var user = await _userRepository.GetByMobileAsync(request.Mobile);
            if (user != null && user.Role == "CUSTOMER")
            {
                // Update email and address
                user.Email = request.Email;
                user.Address = request.Address;
                user.UpdatedAt = DateTime.UtcNow;
                
                // Update password if provided
                if (!string.IsNullOrWhiteSpace(request.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }
                
                await _userRepository.UpdateAsync(user);
            }
            
            return await MapToDTOAsync(customer);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return false;

            // Check if customer has any orders - prevent deletion if they do
            var hasOrders = await _context.Orders.AnyAsync(o => o.CustomerId == id);
            if (hasOrders)
            {
                throw new InvalidOperationException("Cannot delete customer with existing orders. Please archive or reassign orders first.");
            }

            // Check if customer has any enquiries - prevent deletion if they do
            var hasEnquiries = await _context.Enquiries.AnyAsync(e => e.CustomerId == id);
            if (hasEnquiries)
            {
                throw new InvalidOperationException("Cannot delete customer with existing enquiries. Please close or reassign enquiries first.");
            }

            // Also delete the associated user account if exists
            var user = await _userRepository.GetByMobileAsync(customer.Mobile);
            if (user != null && user.Role == "CUSTOMER")
            {
                await _userRepository.DeleteAsync(user.Id);
            }

            // Delete the customer record
            await _customerRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> UpdateProfilePhotoAsync(int id, string? photoPath)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return false;

            customer.ProfilePhoto = photoPath;
            customer.UpdatedAt = DateTime.UtcNow;
            await _customerRepository.UpdateAsync(customer);
            return true;
        }

        private async Task<CustomerDTO> MapToDTOAsync(Customer customer)
        {
            // Fetch associated user to get email and address
            var user = await _userRepository.GetByMobileAsync(customer.Mobile);
            
            return new CustomerDTO
            {
                Id = customer.Id,
                Name = customer.Name,
                Mobile = customer.Mobile,
                Email = user?.Email,
                Address = user?.Address,
                Notes = customer.Notes,
                ProfilePhoto = customer.ProfilePhoto,
                DefaultWastage = customer.DefaultWastage,
                DefaultGst = customer.DefaultGst,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt
            };
        }
    }
}
