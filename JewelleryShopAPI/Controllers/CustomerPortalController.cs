using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JewelleryShopAPI.DTOs;
using JewelleryShopAPI.Services;
using System.Security.Claims;

namespace JewelleryShopAPI.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerPortalController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IGoldRateService _goldRateService;
        private readonly IOrderService _orderService;

        public CustomerPortalController(
            IProductService productService,
            IGoldRateService goldRateService,
            IOrderService orderService)
        {
            _productService = productService;
            _goldRateService = goldRateService;
            _orderService = orderService;
        }

        // GET: api/customer/products
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/customer/gold-rates/today
        [HttpGet("gold-rates/today")]
        public async Task<IActionResult> GetTodayGoldRates()
        {
            var rates = await _goldRateService.GetTodayRatesAsync();
            return Ok(rates);
        }

        // POST: api/customer/orders
        [Authorize(Roles = "CUSTOMER")]
        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CustomerOrderRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (userId == 0)
                return Unauthorized();

            var order = await _orderService.CreateCustomerOrderAsync(userId, request);
            
            if (order == null)
                return BadRequest("Failed to create order");

            return Ok(order);
        }

        // GET: api/customer/orders
        [Authorize(Roles = "CUSTOMER")]
        [HttpGet("orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (userId == 0)
                return Unauthorized();

            var orders = await _orderService.GetCustomerOrdersAsync(userId);
            return Ok(orders);
        }

        // GET: api/customer/orders/{id}/tracking
        [Authorize(Roles = "CUSTOMER")]
        [HttpGet("orders/{id}/tracking")]
        public async Task<IActionResult> GetOrderTracking(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (userId == 0)
                return Unauthorized();

            var tracking = await _orderService.GetOrderTrackingAsync(id, userId);
            
            if (tracking == null)
                return NotFound();

            return Ok(tracking);
        }
    }
}
