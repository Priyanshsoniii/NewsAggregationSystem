using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Repository.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(int id);
        Task<IEnumerable<Notification>> GetByUserIdAsync(int userId);
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(int userId);
        Task<Notification> CreateAsync(Notification notification);
        Task<Notification> UpdateAsync(Notification notification);
        Task<bool> DeleteAsync(int id);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<UserNotificationSetting?> GetUserNotificationSettingAsync(int userId, int? categoryId);
        Task<IEnumerable<UserNotificationSetting>> GetUserNotificationSettingsAsync(int userId);
        Task<UserNotificationSetting> CreateOrUpdateNotificationSettingAsync(UserNotificationSetting setting);
    }
}