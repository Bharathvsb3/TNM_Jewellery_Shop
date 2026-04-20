using Microsoft.AspNetCore.Mvc;
using JewelleryShopAPI.Repositories;
using JewelleryShopAPI.DTOs;

namespace JewelleryShopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatRepository _chatRepository;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatRepository chatRepository, ILogger<ChatController> logger)
        {
            _chatRepository = chatRepository;
            _logger = logger;
        }

        // Get chat history (custno = phone number)
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] string custno, [FromQuery] string? toUser = null)
        {
            try
            {
                if (string.IsNullOrEmpty(custno))
                    return BadRequest(new { message = "custno (phone number) is required" });

                var msgs = await _chatRepository.GetHistoryAsync(custno, toUser);
                return Ok(new { success = true, data = msgs });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat history for custno {custno}", custno);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        // Get all users with unread count + last message
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? admnuser = null)
        {
            try
            {
                var users = await _chatRepository.GetUsersAsync(admnuser);
                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat users");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        // Get unread count (custno = phone number)
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadCount([FromQuery] string custno, [FromQuery] string? fromUser = null)
        {
            try
            {
                if (string.IsNullOrEmpty(custno))
                    return BadRequest(new { message = "custno (phone number) is required" });

                var count = await _chatRepository.GetUnreadCountAsync(custno, fromUser);
                return Ok(new { success = true, count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count for custno {custno}", custno);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        // Mark all messages from a user as read
        [HttpPost("markRead")]
        public async Task<IActionResult> MarkAllRead([FromQuery] string custno, [FromQuery] string curuser)
        {
            try
            {
                if (string.IsNullOrEmpty(custno) || string.IsNullOrEmpty(curuser))
                    return BadRequest(new { message = "custno and curuser are required" });

                var result = await _chatRepository.MarkAllReadAsync(custno, curuser);
                return Ok(new { success = result, message = "Messages marked as read" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking messages as read");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        // Register device for push notifications
        [HttpPost("registerDevice")]
        public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.FCMToken))
                    return BadRequest(new { message = "UserId and FCMToken are required" });

                var result = await _chatRepository.SaveDeviceTokenAsync(
                    request.UserId,
                    request.FCMToken,
                    request.DeviceType ?? "unknown"
                );

                return Ok(new { success = result, message = "Device registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering device");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        // Send message via REST API (alternative to WebSocket)
        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] ChatSendMessageRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FromUser) || 
                    string.IsNullOrEmpty(request.ToUser) || 
                    string.IsNullOrEmpty(request.Message))
                {
                    return BadRequest(new { message = "fromUser, toUser, and message are required" });
                }

                var result = await _chatRepository.SaveMessageAsync(
                    request.FromUser,
                    request.ToUser,
                    request.Message
                );

                return Ok(new { success = result, message = "Message sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}
