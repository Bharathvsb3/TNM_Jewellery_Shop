namespace JewelleryShopAPI.DTOs
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public decimal DefaultWastage { get; set; }
        public decimal DefaultGst { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCustomerRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public decimal DefaultWastage { get; set; } = 0;
        public decimal DefaultGst { get; set; } = 0;
    }

    public class UpdateCustomerRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public decimal DefaultWastage { get; set; } = 0;
        public decimal DefaultGst { get; set; } = 0;
        public bool IsActive { get; set; }
        public string? Password { get; set; }  // Optional: If provided, update user password
    }
}
