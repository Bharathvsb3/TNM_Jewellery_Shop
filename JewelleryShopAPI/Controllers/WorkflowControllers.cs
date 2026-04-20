using JewelleryShopAPI.DTOs;
using JewelleryShopAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JewelleryShopAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CraftWorkflowController : ControllerBase
    {
        private readonly ICraftWorkflowService _craftService;

        public CraftWorkflowController(ICraftWorkflowService craftService)
        {
            _craftService = craftService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var assignments = await _craftService.GetAllAssignmentsAsync();
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to retrieve craft assignments", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var assignment = await _craftService.GetAssignmentByIdAsync(id);
            if (assignment == null)
                return NotFound();

            return Ok(assignment);
        }

        [HttpGet("worker/{workerId}")]
        public async Task<IActionResult> GetByWorker(int workerId)
        {
            var assignments = await _craftService.GetWorkerAssignmentsAsync(workerId);
            return Ok(assignments);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> Assign([FromBody] AssignCraftRequest request)
        {
            try
            {
                var assignment = await _craftService.AssignCraftAsync(request);
                if (assignment == null)
                    return BadRequest(new { 
                        message = "Cannot assign order. Order may already be assigned to a craftsman who is currently working on it, or the order does not exist.",
                        orderId = request.OrderId 
                    });

                return Ok(assignment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while assigning the order", error = ex.Message });
            }
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartWork(int id, [FromBody] StartWorkRequest request)
        {
            var assignment = await _craftService.StartWorkAsync(id, request);
            if (assignment == null)
                return NotFound();

            return Ok(assignment);
        }

        [HttpPost("{id}/complete-work")]
        public async Task<IActionResult> CompleteWork(int id, [FromBody] CompleteWorkRequest request)
        {
            var completion = await _craftService.CompleteWorkAsync(id, request);
            if (completion == null)
                return NotFound();

            return Ok(completion);
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> Complete(int id, [FromBody] CompleteCraftRequest request)
        {
            var assignment = await _craftService.CompleteCraftAsync(id, request);
            if (assignment == null)
                return NotFound();

            return Ok(assignment);
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmCompletion(int id)
        {
            var assignment = await _craftService.ConfirmCompletionAsync(id);
            if (assignment == null)
                return NotFound();

            return Ok(assignment);
        }

        [HttpPut("{id}/update-completion")]
        public async Task<IActionResult> UpdateCompletion(int id, [FromBody] CompleteCraftRequest request)
        {
            var assignment = await _craftService.UpdateCompletionAsync(id, request);
            if (assignment == null)
                return NotFound();

            return Ok(assignment);
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("conversation/{otherUserId}")]
        public async Task<IActionResult> GetConversation(int otherUserId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var messages = await _messageService.GetConversationAsync(userId, otherUserId);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] DTOs.SendMessageRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var message = await _messageService.SendMessageAsync(userId, request);
            return Ok(message);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _messageService.MarkAsReadAsync(id);
            return Ok();
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("daily-sales")]
        public async Task<IActionResult> GetDailySales([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var report = await _reportService.GetDailySalesReportAsync(startDate, endDate);
            return Ok(report);
        }
    }

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShopSettingsController : ControllerBase
    {
        private readonly IShopSettingsService _settingsService;

        public ShopSettingsController(IShopSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _settingsService.GetSettingsAsync();
            return Ok(settings);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateShopSettingsRequest request)
        {
            var settings = await _settingsService.UpdateSettingsAsync(request);
            return Ok(settings);
        }
    }
}
