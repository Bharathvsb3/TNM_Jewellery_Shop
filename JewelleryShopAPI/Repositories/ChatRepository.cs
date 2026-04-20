using JewelleryShopAPI.Data;
using JewelleryShopAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelleryShopAPI.Repositories
{
    public interface IChatRepository
    {
        Task<List<ChatHistory>> GetHistoryAsync(string userId, string? toUser = null);
        Task<List<UserChatInfo>> GetUsersAsync(string? adminUser = null);
        Task<int> GetUnreadCountAsync(string userId, string? fromUser = null);
        Task<bool> MarkAllReadAsync(string userId, string fromUser);
        Task<bool> SaveMessageAsync(string fromUser, string toUser, string messageText);
        Task<bool> SaveDeviceTokenAsync(string userId, string fcmToken, string deviceType);
        Task<List<ChatUser>> GetFcmTokensAsync(string userId);
    }

    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;

        public ChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatHistory>> GetHistoryAsync(string userId, string? toUser = null)
        {
            var query = _context.Set<ChatHistory>().AsQueryable();

            if (!string.IsNullOrEmpty(toUser))
            {
                // Get conversation between two specific users
                query = query.Where(m =>
                    (m.FromUser == userId && m.ToUser == toUser) ||
                    (m.FromUser == toUser && m.ToUser == userId)
                );
            }
            else
            {
                // Get all messages for this user
                query = query.Where(m => m.FromUser == userId || m.ToUser == userId);
            }

            return await query
                .OrderBy(m => m.AddDate)
                .ThenBy(m => m.Id)
                .ToListAsync();
        }

        public async Task<List<UserChatInfo>> GetUsersAsync(string? adminUser = null)
        {
            // Get all unique users who have sent or received messages
            var fromUsers = await _context.Set<ChatHistory>()
                .Select(m => m.FromUser)
                .Distinct()
                .ToListAsync();

            var toUsers = await _context.Set<ChatHistory>()
                .Select(m => m.ToUser)
                .Distinct()
                .ToListAsync();

            var allUsers = fromUsers.Union(toUsers).Distinct().ToList();

            var userInfoList = new List<UserChatInfo>();

            foreach (var user in allUsers)
            {
                if (user == adminUser)
                    continue; // Skip admin user themselves

                // Get unread count for this user
                var unreadCount = await GetUnreadCountAsync(adminUser ?? "", user);

                // Get last message
                var lastMessage = await _context.Set<ChatHistory>()
                    .Where(m =>
                        (m.FromUser == user && m.ToUser == adminUser) ||
                        (m.FromUser == adminUser && m.ToUser == user)
                    )
                    .OrderByDescending(m => m.AddDate)
                    .ThenByDescending(m => m.Id)
                    .FirstOrDefaultAsync();

                // Try to get user name from Users table
                var userEntity = await _context.Users
                    .Where(u => u.Mobile == user)
                    .FirstOrDefaultAsync();

                userInfoList.Add(new UserChatInfo
                {
                    UserId = user,
                    UserName = userEntity?.Name ?? user,
                    UnreadCount = unreadCount,
                    LastMessage = lastMessage?.MessageText ?? "",
                    LastMessageTime = lastMessage?.AddDate
                });
            }

            return userInfoList.OrderByDescending(u => u.LastMessageTime).ToList();
        }

        public async Task<int> GetUnreadCountAsync(string userId, string? fromUser = null)
        {
            var query = _context.Set<ChatHistory>()
                .Where(m => m.ToUser == userId && m.IsRead == 0);

            if (!string.IsNullOrEmpty(fromUser))
            {
                query = query.Where(m => m.FromUser == fromUser);
            }

            return await query.CountAsync();
        }

        public async Task<bool> MarkAllReadAsync(string userId, string fromUser)
        {
            var messages = await _context.Set<ChatHistory>()
                .Where(m => m.ToUser == userId && m.FromUser == fromUser && m.IsRead == 0)
                .ToListAsync();

            foreach (var msg in messages)
            {
                msg.IsRead = 1;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveMessageAsync(string fromUser, string toUser, string messageText)
        {
            var message = new ChatHistory
            {
                FromUser = fromUser,
                ToUser = toUser,
                MessageText = messageText,
                IsRead = 0,
                AddDate = DateTime.UtcNow,
                AddTime = DateTime.Now.ToString("HH:mm:ss"),
                AddUser = fromUser
            };

            _context.Set<ChatHistory>().Add(message);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SaveDeviceTokenAsync(string userId, string fcmToken, string deviceType)
        {
            // Check if token already exists for this user and device
            var existing = await _context.Set<ChatUser>()
                .FirstOrDefaultAsync(u => u.UserId == userId && u.FcmToken == fcmToken);

            if (existing != null)
            {
                // Update existing token
                existing.DeviceType = deviceType;
                existing.Status = 1;
                existing.AddDate = DateTime.UtcNow;
                existing.AddTime = DateTime.Now.ToString("HH:mm:ss");
            }
            else
            {
                // Add new token
                var chatUser = new ChatUser
                {
                    UserId = userId,
                    FcmToken = fcmToken,
                    DeviceType = deviceType,
                    Status = 1,
                    AddUser = userId,
                    AddDate = DateTime.UtcNow,
                    AddTime = DateTime.Now.ToString("HH:mm:ss")
                };

                _context.Set<ChatUser>().Add(chatUser);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ChatUser>> GetFcmTokensAsync(string userId)
        {
            return await _context.Set<ChatUser>()
                .Where(u => u.UserId == userId && u.Status == 1)
                .ToListAsync();
        }
    }

    // DTO for user chat info
    public class UserChatInfo
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int UnreadCount { get; set; }
        public string LastMessage { get; set; } = string.Empty;
        public DateTime? LastMessageTime { get; set; }
    }
}
