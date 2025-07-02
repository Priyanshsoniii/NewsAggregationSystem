using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId);
        Task<Notification?> GetNotificationByIdAsync(int id);
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int id);
        
        // Notification Settings
        Task<IEnumerable<UserNotificationSetting>> GetUserNotificationSettingsAsync(int userId);
        Task<UserNotificationSetting> CreateOrUpdateNotificationSettingAsync(UserNotificationSetting setting);
        Task<bool> ToggleNotificationSettingAsync(int userId, int? categoryId);
        Task<bool> UpdateKeywordsAsync(int userId, int? categoryId, List<string> keywords);
        
        // Email Notifications
        Task<bool> SendEmailNotificationAsync(int userId, string title, string message, int? articleId = null);
        Task<bool> SendKeywordBasedNotificationsAsync(int userId, List<string> keywords, NewsArticle article);
    }
} 