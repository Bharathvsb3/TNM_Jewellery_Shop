using JewelleryShopAPI.DTOs;
using JewelleryShopAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JewelleryShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("unified-login")]
        public async Task<IActionResult> UnifiedLogin([FromBody] UnifiedLoginRequest request)
        {
            var response = await _authService.UnifiedLoginAsync(request);
            if (response == null || !response.Success)
                return Unauthorized(new { message = response?.Message ?? "Invalid credentials" });

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            if (response == null)
                return BadRequest(new { message = "Mobile number already exists" });

            return Ok(response);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _authService.ChangePasswordAsync(userId, request);

            if (!result)
                return BadRequest(new { message = "Current password is incorrect" });

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpPost("craftsman-login")]
        public async Task<IActionResult> CraftsmanLogin([FromBody] CraftsmanLoginRequest request)
        {
            var response = await _authService.CraftsmanLoginAsync(request);
            if (response == null)
                return Unauthorized("Invalid mobile number or password");

            return Ok(response);
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IWebHostEnvironment _environment;

        public CustomersController(ICustomerService customerService, IWebHostEnvironment environment)
        {
            _customerService = customerService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var customers = await _customerService.GetAllCustomersAsync(includeInactive);
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            var customer = await _customerService.CreateCustomerAsync(request);
            if (customer == null)
                return BadRequest(new { message = "Mobile number already exists" });

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerRequest request)
        {
            // First check if customer exists
            var existingCustomer = await _customerService.GetCustomerByIdAsync(id);
            if (existingCustomer == null)
                return NotFound();
            
            var customer = await _customerService.UpdateCustomerAsync(id, request);
            if (customer == null)
            {
                // UpdateCustomerAsync returns null when mobile number already exists for another customer
                return BadRequest(new { message = "Mobile number already exists" });
            }

            return Ok(customer);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                if (!result)
                    return NotFound(new { message = "Customer not found or could not be deleted" });

                return Ok(new { message = "Customer deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the customer.", details = ex.Message });
            }
        }

        [HttpPost("{id}/photo")]
        public async Task<IActionResult> UploadPhoto(int id, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                return BadRequest(new { message = "No photo provided" });

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
            if (!allowedTypes.Contains(photo.ContentType.ToLower()))
                return BadRequest(new { message = "Invalid file type. Only JPEG, PNG, WebP, and GIF are allowed." });

            // Validate file size (max 5MB)
            if (photo.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "File size exceeds 5MB limit." });

            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();

            // Create uploads directory
            var uploadsDir = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "profiles");
            Directory.CreateDirectory(uploadsDir);

            // Generate unique filename
            var extension = Path.GetExtension(photo.FileName).ToLower();
            var fileName = $"customer_{id}_{DateTime.UtcNow.Ticks}{extension}";
            var filePath = Path.Combine(uploadsDir, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            // Update customer profile photo path
            var photoUrl = $"/uploads/profiles/{fileName}";
            await _customerService.UpdateProfilePhotoAsync(id, photoUrl);

            return Ok(new { profilePhoto = photoUrl });
        }

        [HttpDelete("{id}/photo")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound();

            if (!string.IsNullOrEmpty(customer.ProfilePhoto))
            {
                // Delete file from disk
                var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", customer.ProfilePhoto.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                await _customerService.UpdateProfilePhotoAsync(id, null);
            }

            return Ok(new { message = "Profile photo removed" });
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EnquiriesController : ControllerBase
    {
        private readonly IEnquiryService _enquiryService;

        public EnquiriesController(IEnquiryService enquiryService)
        {
            _enquiryService = enquiryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enquiries = await _enquiryService.GetAllEnquiriesAsync();
            return Ok(enquiries);
        }

        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var enquiries = await _enquiryService.GetEnquiriesByStatusAsync(status);
            return Ok(enquiries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var enquiry = await _enquiryService.GetEnquiryByIdAsync(id);
            if (enquiry == null)
                return NotFound();

            return Ok(enquiry);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEnquiryRequest request)
        {
            var enquiry = await _enquiryService.CreateEnquiryAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = enquiry.Id }, enquiry);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEnquiryRequest request)
        {
            var enquiry = await _enquiryService.UpdateEnquiryAsync(id, request);
            if (enquiry == null)
                return NotFound();

            return Ok(enquiry);
        }
    }
}
