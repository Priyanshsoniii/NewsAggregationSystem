using Microsoft.EntityFrameworkCore;
using NewsAggregation.Server.Data;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;

namespace NewsAggregation.Server.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NewsAggregationContext _context;

        public NotificationRepository(NewsAggregationContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications
                .Include(n => n.RelatedArticle)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IEnumerable<Notification>> GetByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Include(n => n.RelatedArticle)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Include(n => n.RelatedArticle)
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<Notification> UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
            return notification;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<UserNotificationSetting?> GetUserNotificationSettingAsync(int userId, int? categoryId)
        {
            return await _context.UserNotificationSettings
                .FirstOrDefaultAsync(uns => uns.UserId == userId && uns.CategoryId == categoryId);
        }

        public async Task<IEnumerable<UserNotificationSetting>> GetUserNotificationSettingsAsync(int userId)
        {
            return await _context.UserNotificationSettings
                .Include(uns => uns.Category)
                .Where(uns => uns.UserId == userId)
                .ToListAsync();
        }

        public async Task<UserNotificationSetting> CreateOrUpdateNotificationSettingAsync(UserNotificationSetting setting)
        {
            var existing = await _context.UserNotificationSettings
                .FirstOrDefaultAsync(uns => uns.UserId == setting.UserId && uns.CategoryId == setting.CategoryId);

            if (existing != null)
            {
                existing.IsEnabled = setting.IsEnabled;
                existing.Keywords = setting.Keywords;
                existing.EmailNotifications = setting.EmailNotifications;
                existing.UpdatedAt = DateTime.UtcNow;
                _context.UserNotificationSettings.Update(existing);
            }
            else
            {
                _context.UserNotificationSettings.Add(setting);
            }

            await _context.SaveChangesAsync();
            return existing ?? setting;
        }
    }
}