using JewelleryShopAPI.DTOs;
using JewelleryShopAPI.Models;
using JewelleryShopAPI.Repositories;

namespace JewelleryShopAPI.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDTO>> GetConversationAsync(int userId1, int userId2);
        Task<MessageDTO> SendMessageAsync(int senderId, SendMessageRequest request);
        Task MarkAsReadAsync(int messageId);
    }

    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<MessageDTO>> GetConversationAsync(int userId1, int userId2)
        {
            var messages = await _messageRepository.GetConversationAsync(userId1, userId2);
            var sender = await _userRepository.GetByIdAsync(userId1);
            var receiver = await _userRepository.GetByIdAsync(userId2);

            return messages.Select(m => new MessageDTO
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.SenderId == userId1 ? (sender?.Name ?? "") : (receiver?.Name ?? ""),
                ReceiverId = m.ReceiverId,
                ReceiverName = m.ReceiverId == userId2 ? (receiver?.Name ?? "") : (sender?.Name ?? ""),
                MessageText = m.MessageText,
                IsRead = m.IsRead,
                SentAt = m.SentAt
            });
        }

        public async Task<MessageDTO> SendMessageAsync(int senderId, SendMessageRequest request)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = request.ReceiverId,
                MessageText = request.MessageText,
                IsRead = false,
                SentAt = DateTime.UtcNow
            };

            await _messageRepository.CreateAsync(message);

            var sender = await _userRepository.GetByIdAsync(senderId);
            var receiver = await _userRepository.GetByIdAsync(request.ReceiverId);

            return new MessageDTO
            {
                Id = message.Id,
                SenderId = senderId,
                SenderName = sender?.Name ?? "",
                ReceiverId = request.ReceiverId,
                ReceiverName = receiver?.Name ?? "",
                MessageText = message.MessageText,
                IsRead = false,
                SentAt = message.SentAt
            };
        }

        public async Task MarkAsReadAsync(int messageId)
        {
            await _messageRepository.MarkAsReadAsync(messageId);
        }
    }

    public interface IReportService
    {
        Task<IEnumerable<DailySalesReport>> GetDailySalesReportAsync(DateTime startDate, DateTime endDate);
    }

    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;

        public ReportService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<DailySalesReport>> GetDailySalesReportAsync(DateTime startDate, DateTime endDate)
        {
            var orders = await _orderRepository.GetByDateRangeAsync(startDate, endDate);

            var grouped = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new DailySalesReport
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    TotalSales = g.Sum(o => o.TotalPrice),
                    TotalAdvance = g.Sum(o => o.AdvanceAmount),
                    TotalBalance = g.Sum(o => o.BalanceAmount)
                })
                .OrderBy(r => r.Date);

            return grouped;
        }
    }

    public interface IShopSettingsService
    {
        Task<ShopSettingsDTO> GetSettingsAsync();
        Task<ShopSettingsDTO> UpdateSettingsAsync(UpdateShopSettingsRequest request);
    }

    public class ShopSettingsService : IShopSettingsService
    {
        private readonly IShopSettingsRepository _settingsRepository;

        public ShopSettingsService(IShopSettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public async Task<ShopSettingsDTO> GetSettingsAsync()
        {
            var settings = await _settingsRepository.GetSettingsAsync();
            return new ShopSettingsDTO
            {
                Id = settings.Id,
                ShopName = settings.ShopName,
                ShopLogo = settings.ShopLogo
            };
        }

        public async Task<ShopSettingsDTO> UpdateSettingsAsync(UpdateShopSettingsRequest request)
        {
            var settings = await _settingsRepository.GetSettingsAsync();
            settings.ShopName = request.ShopName;
            settings.ShopLogo = request. ShopLogo;

            await _settingsRepository.UpdateSettingsAsync(settings);

            return new ShopSettingsDTO
            {
                Id = settings.Id,
                ShopName = settings.ShopName,
                ShopLogo = settings.ShopLogo
            };
        }
    }
}
