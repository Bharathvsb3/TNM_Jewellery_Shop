namespace JewelleryShopAPI.DTOs
{
    public class LoginRequest
    {
        public string EmailOrMobile { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "CUSTOMER"; // Default to CUSTOMER for portal
        public bool AutoApprove { get; set; } = false; // Set to true for auto-approval, false for manual admin approval
    }

    public class AuthResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class CraftsmanLoginRequest
    {
        public string Mobile { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class CraftsmanAuthResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string WorkerType { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public class UnifiedLoginRequest
    {
        public string EmailOrMobile { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UnifiedAuthResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? UserType { get; set; } // "admin", "customer", "craftsman"
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RedirectUrl { get; set; } = string.Empty;
    }
}
