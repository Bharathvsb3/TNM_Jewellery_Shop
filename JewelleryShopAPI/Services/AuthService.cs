using JewelleryShopAPI.DTOs;
using JewelleryShopAPI.Models;
using JewelleryShopAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JewelleryShopAPI.Services
{
    public interface IAuthService
    {
        Task<UnifiedAuthResponse?> UnifiedLoginAsync(UnifiedLoginRequest request);
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<CraftsmanAuthResponse?> CraftsmanLoginAsync(CraftsmanLoginRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWorkerRepository _workerRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _configuration;
        private readonly IDatabaseMigrationService _migrationService;

        public AuthService(
            IUserRepository userRepository, 
            IWorkerRepository workerRepository,
            ICustomerRepository customerRepository,
            IConfiguration configuration, 
            IDatabaseMigrationService migrationService)
        {
            _userRepository = userRepository;
            _workerRepository = workerRepository;
            _customerRepository = customerRepository;
            _configuration = configuration;
            _migrationService = migrationService;
        }

        public async Task<UnifiedAuthResponse?> UnifiedLoginAsync(UnifiedLoginRequest request)
        {
            // Auto-apply database migrations
            try
            {
                await _migrationService.ApplyMigrationsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration error during login: {ex.Message}");
            }

            string identifier = request.EmailOrMobile.Trim();
            Console.WriteLine($"[LOGIN] Attempting login for: {identifier}");

            // 1. Try Admin/User login (Users table)
            User? user = null;
            if (identifier.Contains("@"))
            {
                Console.WriteLine($"[LOGIN] Searching by email: {identifier}");
                user = await _userRepository.GetByEmailAsync(identifier);
            }
            else
            {
                Console.WriteLine($"[LOGIN] Searching by mobile: {identifier}");
                user = await _userRepository.GetByMobileAsync(identifier);
            }

            if (user != null)
            {
                Console.WriteLine($"[LOGIN] User found: {user.Name} | Role: {user.Role} | Active: {user.IsActive}");
                
                if (!user.IsActive)
                {
                    Console.WriteLine($"[LOGIN] User is inactive");
                    return new UnifiedAuthResponse
                    {
                        Success = false,
                        Message = "Your account has been deactivated. Please contact the administrator."
                    };
                }
                
                // Verify password
                bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                Console.WriteLine($"[LOGIN] Password verification: {(passwordValid ? "SUCCESS" : "FAILED")}");
                
                if (passwordValid)
                {
                    // Check approval status for customers
                    if (user.Role == "CUSTOMER" && user.AccountStatus != "Approved")
                    {
                        Console.WriteLine($"[LOGIN] Customer not approved. Status: {user.AccountStatus}");
                        return new UnifiedAuthResponse
                        {
                            Success = false,
                            Message = "Your account is pending approval. Please contact the administrator."
                        };
                    }

                    var token = GenerateJwtToken(user);
                    Console.WriteLine($"[LOGIN] Login successful for {user.Role} user: {user.Name}");
                    
                    return new UnifiedAuthResponse
                    {
                        Success = true,
                        UserType = user.Role == "CUSTOMER" ? "customer" : "admin",
                        Id = user.Id,
                        Name = user.Name,
                        Mobile = user.Mobile,
                        Email = user.Email,
                        Role = user.Role,
                        Token = token,
                        RedirectUrl = user.Role == "CUSTOMER" ? "customer.html" : "dashboard.html"
                    };
                }
            }
            else
            {
                Console.WriteLine($"[LOGIN] No user found in Users table");
            }

            // 2. Try Craftsman login (Workers table)
            Console.WriteLine($"[LOGIN] Checking Workers table for craftsman...");
            var worker = await _workerRepository.GetByMobileAsync(identifier);
            if (worker != null)
            {
                Console.WriteLine($"[LOGIN] Worker found: {worker.Name} | Type: {worker.WorkerType} | Active: {worker.IsActive} | Has Password: {!string.IsNullOrEmpty(worker.PasswordHash)}");
                
                if (worker.WorkerType == "External" && !string.IsNullOrEmpty(worker.PasswordHash))
                {
                    if (worker.IsActive)
                    {
                        bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, worker.PasswordHash);
                        Console.WriteLine($"[LOGIN] Craftsman password verification: {(passwordValid ? "SUCCESS" : "FAILED")}");
                        
                        if (passwordValid)
                        {
                            var token = GenerateCraftsmanJwtToken(worker);
                            Console.WriteLine($"[LOGIN] Login successful for craftsman: {worker.Name}");
                            
                            return new UnifiedAuthResponse
                            {
                                Success = true,
                                UserType = "craftsman",
                                Id = worker.Id,
                                Name = worker.Name,
                                Mobile = worker.Mobile,
                                Role = "CRAFTSMAN",
                                Token = token,
                                RedirectUrl = "craftsman-index.html"
                            };
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[LOGIN] Craftsman account is inactive");
                        return new UnifiedAuthResponse
                        {
                            Success = false,
                            Message = "Your account has been deactivated. Please contact the administrator."
                        };
                    }
                }
            }
            else
            {
                Console.WriteLine($"[LOGIN] No worker found in Workers table");
            }

            // No match found
            Console.WriteLine($"[LOGIN] Login failed - invalid credentials for: {identifier}");
            return new UnifiedAuthResponse
            {
                Success = false,
                Message = "Invalid credentials. Please check your email/mobile and password."
            };
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            // Auto-apply database migrations on every login
            try
            {
                await _migrationService.ApplyMigrationsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration error during login: {ex.Message}");
                // Continue with login even if migration fails
            }

            User? user = null;

            // Check if input is email or mobile
            if (request.EmailOrMobile.Contains("@"))
            {
                user = await _userRepository.GetByEmailAsync(request.EmailOrMobile);
            }
            else
            {
                user = await _userRepository.GetByMobileAsync(request.EmailOrMobile);
            }

            if (user == null || !user.IsActive)
                return null;

            // Check account approval status for customers
            if (user.Role == "CUSTOMER" && user.AccountStatus != "Approved")
                return null; // Account not approved

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Id = user.Id,
                Name = user.Name,
                Mobile = user.Mobile,
                Email = user.Email,
                Address = user.Address,
                Role = user.Role,
                AccountStatus = user.AccountStatus,
                Token = token
            };
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            // Check if mobile already exists
            var existingUser = await _userRepository.GetByMobileAsync(request.Mobile);
            if (existingUser != null)
            {
                // If user exists but it's a CUSTOMER registration, check if Customer record exists
                if (request.Role == "CUSTOMER")
                {
                    var existingCustomer = await _customerRepository.GetByMobileAsync(request.Mobile);
                    if (existingCustomer == null)
                    {
                        // User exists but Customer record doesn't - create it
                        var customer = new Customer
                        {
                            Name = existingUser.Name,
                            Mobile = existingUser.Mobile,
                            Notes = null,
                            IsActive = true
                        };
                        
                        await _customerRepository.CreateAsync(customer);
                    }
                }
                return null; // Still return null as user already exists
            }

            // Determine account status based on role and auto-approve flag from request
            string accountStatus = "Approved"; // Default for ADMIN
            if (request.Role == "CUSTOMER")
            {
                // Use AutoApprove flag from request, default to false (manual approval)
                accountStatus = request.AutoApprove ? "Approved" : "Pending";
            }

            var user = new User
            {
                Name = request.Name,
                Mobile = request.Mobile,
                Email = request.Email,
                Address = request.Address,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                AccountStatus = accountStatus,
                IsActive = true
            };

            await _userRepository.CreateAsync(user);
            
            // Create Customer record if role is CUSTOMER
            if (request.Role == "CUSTOMER")
            {
                var customer = new Customer
                {
                    Name = request.Name,
                    Mobile = request.Mobile,
                    Notes = null,
                    IsActive = true
                };
                
                await _customerRepository.CreateAsync(customer);
            }

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Id = user.Id,
                Name = user.Name,
                Mobile = user.Mobile,
                Email = user.Email,
                Address = user.Address,
                Role = user.Role,
                AccountStatus = user.AccountStatus,
                Token = token
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<CraftsmanAuthResponse?> CraftsmanLoginAsync(CraftsmanLoginRequest request)
        {
            var worker = await _workerRepository.GetByMobileAsync(request.Mobile);
            
            if (worker == null || worker.WorkerType != "External" || string.IsNullOrEmpty(worker.PasswordHash))
                return null;

            if (!worker.IsActive)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, worker.PasswordHash))
                return null;

            var token = GenerateCraftsmanJwtToken(worker);

            return new CraftsmanAuthResponse
            {
                Id = worker.Id,
                Name = worker.Name,
                Mobile = worker.Mobile,
                WorkerType = worker.WorkerType,
                Token = token
            };
        }

        private string GenerateCraftsmanJwtToken(Worker worker)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"] ?? "YourSuperSecretKeyForJewelleryShopManagement2026!@#$";
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, worker.Id.ToString()),
                new Claim(ClaimTypes.Name, worker.Name),
                new Claim(ClaimTypes.MobilePhone, worker.Mobile),
                new Claim(ClaimTypes.Role, "CRAFTSMAN")
            };

            var expiryDays = int.Parse(_configuration["JwtSettings:ExpiryInDays"] ?? "30");

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expiryDays),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"] ?? "YourSuperSecretKeyForJewelleryShopManagement2026!@#$";
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.MobilePhone, user.Mobile),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var expiryDays = int.Parse(_configuration["JwtSettings:ExpiryInDays"] ?? "30");

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expiryDays),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
