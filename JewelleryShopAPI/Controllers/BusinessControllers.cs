using JewelleryShopAPI.DTOs;
using JewelleryShopAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JewelleryShopAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var order = await _orderService.CreateOrderAsync(request);
            if (order == null)
                return BadRequest(new { message = "Gold rate not found or invalid data" });

            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderRequest request)
        {
            var order = await _orderService.UpdateOrderAsync(id, request);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveOrder(int id, [FromBody] ApproveOrderRequest request)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (userId == 0)
                return Unauthorized();

            var result = await _orderService.ApproveOrderAsync(id, userId, request);
            
            if (!result)
                return NotFound();

            return Ok(new { message = request.IsApproved ? "Order approved successfully" : "Order rejected" });
        }

        [HttpPost("outstanding-report")]
        public async Task<IActionResult> GetOutstandingReport([FromBody] CustomerOutstandingRequestDTO request)
        {
            var report = await _orderService.GetCustomerOutstandingReportAsync(request);
            return Ok(report);
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WorkersController : ControllerBase
    {
        private readonly IWorkerService _workerService;

        public WorkersController(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var workers = await _workerService.GetAllWorkersAsync(includeInactive);
            return Ok(workers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var worker = await _workerService.GetWorkerByIdAsync(id);
            if (worker == null)
                return NotFound();

            return Ok(worker);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWorkerRequest request)
        {
            var worker = await _workerService.CreateWorkerAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = worker.Id }, worker);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkerRequest request)
        {
            var worker = await _workerService.UpdateWorkerAsync(id, request);
            if (worker == null)
                return NotFound();

            return Ok(worker);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var worker = await _workerService.GetWorkerByIdAsync(id);
            if (worker == null)
                return NotFound();

            var updateRequest = new UpdateWorkerRequest
            {
                Name = worker.Name,
                Mobile = worker.Mobile,
                WorkerType = worker.WorkerType,
                IsActive = request.IsActive
            };

            var updated = await _workerService.UpdateWorkerAsync(id, updateRequest);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _workerService.DeleteWorkerAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var products = await _productService.GetAllProductsAsync(includeInactive);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var product = await _productService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDTO request)
        {
            var product = await _productService.UpdateProductAsync(id, request);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            product.IsActive = request.IsActive;
            var updated = await _productService.UpdateProductAsync(id, product);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }

    public class UpdateStatusRequest
    {
        public bool IsActive { get; set; }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GoldRatesController : ControllerBase
    {
        private readonly IGoldRateService _goldRateService;

        public GoldRatesController(IGoldRateService goldRateService)
        {
            _goldRateService = goldRateService;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            var rate = await _goldRateService.GetLatestRateAsync();
            if (rate == null)
                return NotFound(new { message = "No gold rate found" });

            return Ok(rate);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rates = await _goldRateService.GetAllRatesAsync();
            return Ok(rates);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rate = await _goldRateService.GetRateByIdAsync(id);
            if (rate == null)
                return NotFound(new { message = "Gold rate not found" });

            return Ok(rate);
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetToday()
        {
            var rates = await _goldRateService.GetTodayRatesAsync();
            return Ok(rates);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGoldRateRequest request)
        {
            var rate = await _goldRateService.CreateRateAsync(request);
            if (rate == null)
                return BadRequest(new { message = "Invalid time slot or maximum entries reached for today" });

            return Ok(rate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateGoldRateRequest request)
        {
            var rate = await _goldRateService.UpdateRateAsync(id, request);
            if (rate == null)
                return NotFound(new { message = "Gold rate not found or invalid time slot" });

            return Ok(rate);
        }
    }
}
