using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public NotificationService(
            INotificationRepository notificationRepository,
            IEmailService emailService,
            IUserService userService)
        {
            _notificationRepository = notificationRepository;
            _emailService = emailService;
            _userService = userService;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId)
        {
            return await _notificationRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId)
        {
            return await _notificationRepository.GetUnreadByUserIdAsync(userId);
        }

        public async Task<Notification?> GetNotificationByIdAsync(int id)
        {
            return await _notificationRepository.GetByIdAsync(id);
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            return await _notificationRepository.CreateAsync(notification);
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            return await _notificationRepository.MarkAsReadAsync(notificationId);
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            return await _notificationRepository.MarkAllAsReadAsync(userId);
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            return await _notificationRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<UserNotificationSetting>> GetUserNotificationSettingsAsync(int userId)
        {
            return await _notificationRepository.GetUserNotificationSettingsAsync(userId);
        }

        public async Task<UserNotificationSetting> CreateOrUpdateNotificationSettingAsync(UserNotificationSetting setting)
        {
            return await _notificationRepository.CreateOrUpdateNotificationSettingAsync(setting);
        }

        public async Task<bool> ToggleNotificationSettingAsync(int userId, int? categoryId)
        {
            var setting = await _notificationRepository.GetUserNotificationSettingAsync(userId, categoryId);
            
            if (setting == null)
            {
                setting = new UserNotificationSetting
                {
                    UserId = userId,
                    CategoryId = categoryId,
                    IsEnabled = true,
                    EmailNotifications = true,
                    CreatedAt = DateTime.UtcNow
                };
            }
            else
            {
                setting.IsEnabled = !setting.IsEnabled;
                setting.UpdatedAt = DateTime.UtcNow;
            }

            await _notificationRepository.CreateOrUpdateNotificationSettingAsync(setting);
            return setting.IsEnabled;
        }

        public async Task<bool> UpdateKeywordsAsync(int userId, int? categoryId, List<string> keywords)
        {
            var setting = await _notificationRepository.GetUserNotificationSettingAsync(userId, categoryId);
            
            if (setting == null)
            {
                setting = new UserNotificationSetting
                {
                    UserId = userId,
                    CategoryId = categoryId,
                    IsEnabled = true,
                    EmailNotifications = true,
                    CreatedAt = DateTime.UtcNow
                };
            }

            setting.Keywords = System.Text.Json.JsonSerializer.Serialize(keywords);
            setting.UpdatedAt = DateTime.UtcNow;

            await _notificationRepository.CreateOrUpdateNotificationSettingAsync(setting);
            return true;
        }

        public async Task<bool> SendEmailNotificationAsync(int userId, string title, string message, int? articleId = null)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null) return false;

            
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    NotificationType = articleId.HasValue ? "Article" : "General",
                    RelatedArticleId = articleId,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _notificationRepository.CreateAsync(notification);

              
                if (user.Email != null)
                {
                    await _emailService.SendEmailAsync(user.Email, title, message);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendKeywordBasedNotificationsAsync(int userId, List<string> keywords, NewsArticle article)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null) return false;

                var title = $"New Article Matching Your Keywords: {article.Title}";
                var message = $"An article matching your keywords has been published: {article.Title}. Read more at: {article.Url}";

                return await SendEmailNotificationAsync(userId, title, message, article.Id);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 