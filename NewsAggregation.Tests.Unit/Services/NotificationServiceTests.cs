using Moq;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services;
using NewsAggregation.Server.Services.Interfaces;
using Xunit;

namespace NewsAggregation.Tests.Unit.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _mockNotificationRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _mockNotificationRepository = new Mock<INotificationRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _mockUserService = new Mock<IUserService>();
            _notificationService = new NotificationService(_mockNotificationRepository.Object, _mockEmailService.Object, _mockUserService.Object);
        }

        [Fact]
        public async Task GetUserNotificationsAsync_ReturnsUserNotifications()
        {
            // Arrange
            var userId = 1;
            var notifications = new List<Notification>
            {
                new Notification { Id = 1, UserId = userId, Title = "Test 1", IsRead = false },
                new Notification { Id = 2, UserId = userId, Title = "Test 2", IsRead = true }
            };

            _mockNotificationRepository.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(notifications);

            // Act
            var result = await _notificationService.GetUserNotificationsAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, n => n.Title == "Test 1");
            Assert.Contains(resultList, n => n.Title == "Test 2");
        }

        [Fact]
        public async Task GetUnreadNotificationsAsync_ReturnsOnlyUnreadNotifications()
        {
            // Arrange
            var userId = 1;
            var notifications = new List<Notification>
            {
                new Notification { Id = 1, UserId = userId, Title = "Unread 1", IsRead = false },
                new Notification { Id = 2, UserId = userId, Title = "Read 1", IsRead = true },
                new Notification { Id = 3, UserId = userId, Title = "Unread 2", IsRead = false }
            };

            _mockNotificationRepository.Setup(x => x.GetUnreadByUserIdAsync(userId)).ReturnsAsync(notifications.Where(n => !n.IsRead));

            // Act
            var result = await _notificationService.GetUnreadNotificationsAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, n => Assert.False(n.IsRead));
            Assert.Contains(resultList, n => n.Title == "Unread 1");
            Assert.Contains(resultList, n => n.Title == "Unread 2");
        }

        [Fact]
        public async Task GetNotificationByIdAsync_WithValidId_ReturnsNotification()
        {
            // Arrange
            var notificationId = 1;
            var notification = new Notification { Id = notificationId, Title = "Test Notification" };

            _mockNotificationRepository.Setup(x => x.GetByIdAsync(notificationId)).ReturnsAsync(notification);

            // Act
            var result = await _notificationService.GetNotificationByIdAsync(notificationId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(notificationId, result.Id);
            Assert.Equal("Test Notification", result.Title);
        }

        [Fact]
        public async Task GetNotificationByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var notificationId = 999;

            _mockNotificationRepository.Setup(x => x.GetByIdAsync(notificationId)).ReturnsAsync((Notification?)null);

            // Act
            var result = await _notificationService.GetNotificationByIdAsync(notificationId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateNotificationAsync_WithValidData_ReturnsCreatedNotification()
        {
            // Arrange
            var notification = new Notification { Title = "New Notification", Message = "Test message" };
            var createdNotification = new Notification { Id = 1, Title = "New Notification", Message = "Test message" };

            _mockNotificationRepository.Setup(x => x.CreateAsync(notification)).ReturnsAsync(createdNotification);

            // Act
            var result = await _notificationService.CreateNotificationAsync(notification);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Notification", result.Title);
            _mockNotificationRepository.Verify(x => x.CreateAsync(notification), Times.Once);
        }

        [Fact]
        public async Task MarkAsReadAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var notificationId = 1;

            _mockNotificationRepository.Setup(x => x.MarkAsReadAsync(notificationId)).ReturnsAsync(true);

            // Act
            var result = await _notificationService.MarkAsReadAsync(notificationId);

            // Assert
            Assert.True(result);
            _mockNotificationRepository.Verify(x => x.MarkAsReadAsync(notificationId), Times.Once);
        }

        [Fact]
        public async Task MarkAllAsReadAsync_WithValidUserId_ReturnsTrue()
        {
            // Arrange
            var userId = 1;

            _mockNotificationRepository.Setup(x => x.MarkAllAsReadAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _notificationService.MarkAllAsReadAsync(userId);

            // Assert
            Assert.True(result);
            _mockNotificationRepository.Verify(x => x.MarkAllAsReadAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteNotificationAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var notificationId = 1;

            _mockNotificationRepository.Setup(x => x.DeleteAsync(notificationId)).ReturnsAsync(true);

            // Act
            var result = await _notificationService.DeleteNotificationAsync(notificationId);

            // Assert
            Assert.True(result);
            _mockNotificationRepository.Verify(x => x.DeleteAsync(notificationId), Times.Once);
        }

        [Fact]
        public async Task GetUserNotificationSettingsAsync_ReturnsUserSettings()
        {
            // Arrange
            var userId = 1;
            var settings = new List<UserNotificationSetting>
            {
                new UserNotificationSetting { Id = 1, UserId = userId, CategoryId = 1, IsEnabled = true },
                new UserNotificationSetting { Id = 2, UserId = userId, CategoryId = 2, IsEnabled = false }
            };

            _mockNotificationRepository.Setup(x => x.GetUserNotificationSettingsAsync(userId)).ReturnsAsync(settings);

            // Act
            var result = await _notificationService.GetUserNotificationSettingsAsync(userId);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, s => s.CategoryId == 1 && s.IsEnabled);
            Assert.Contains(resultList, s => s.CategoryId == 2 && !s.IsEnabled);
        }

        [Fact]
        public async Task ToggleNotificationSettingAsync_WithNewSetting_CreatesAndEnables()
        {
            // Arrange
            var userId = 1;
            var categoryId = 1;

            _mockNotificationRepository.Setup(x => x.GetUserNotificationSettingAsync(userId, categoryId))
                .ReturnsAsync((UserNotificationSetting?)null);
            _mockNotificationRepository.Setup(x => x.CreateOrUpdateNotificationSettingAsync(It.IsAny<UserNotificationSetting>()))
                .ReturnsAsync(new UserNotificationSetting { IsEnabled = true });

            // Act
            var result = await _notificationService.ToggleNotificationSettingAsync(userId, categoryId);

            // Assert
            Assert.True(result);
            _mockNotificationRepository.Verify(x => x.CreateOrUpdateNotificationSettingAsync(It.IsAny<UserNotificationSetting>()), Times.Once);
        }

        [Fact]
        public async Task ToggleNotificationSettingAsync_WithExistingSetting_TogglesEnabled()
        {
            // Arrange
            var userId = 1;
            var categoryId = 1;
            var existingSetting = new UserNotificationSetting { Id = 1, UserId = userId, CategoryId = categoryId, IsEnabled = true };

            _mockNotificationRepository.Setup(x => x.GetUserNotificationSettingAsync(userId, categoryId))
                .ReturnsAsync(existingSetting);
            _mockNotificationRepository.Setup(x => x.CreateOrUpdateNotificationSettingAsync(It.IsAny<UserNotificationSetting>()))
                .ReturnsAsync(new UserNotificationSetting { IsEnabled = false });

            // Act
            var result = await _notificationService.ToggleNotificationSettingAsync(userId, categoryId);

            // Assert
            Assert.False(result);
            _mockNotificationRepository.Verify(x => x.CreateOrUpdateNotificationSettingAsync(It.IsAny<UserNotificationSetting>()), Times.Once);
        }

        [Fact]
        public async Task UpdateKeywordsAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var categoryId = 1;
            var keywords = new List<string> { "technology", "AI", "machine learning" };

            _mockNotificationRepository.Setup(x => x.GetUserNotificationSettingAsync(userId, categoryId))
                .ReturnsAsync((UserNotificationSetting?)null);
            _mockNotificationRepository.Setup(x => x.CreateOrUpdateNotificationSettingAsync(It.IsAny<UserNotificationSetting>()))
                .ReturnsAsync(new UserNotificationSetting());

            // Act
            var result = await _notificationService.UpdateKeywordsAsync(userId, categoryId, keywords);

            // Assert
            Assert.True(result);
            _mockNotificationRepository.Verify(x => x.CreateOrUpdateNotificationSettingAsync(It.IsAny<UserNotificationSetting>()), Times.Once);
        }

        [Fact]
        public async Task SendEmailNotificationAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var title = "Test Notification";
            var message = "Test message";
            var user = new User { Id = userId, Email = "test@example.com" };

            _mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockNotificationRepository.Setup(x => x.CreateAsync(It.IsAny<Notification>()))
                .ReturnsAsync(new Notification());
            _mockEmailService.Setup(x => x.SendEmailAsync(user.Email, title, message))
                .ReturnsAsync(true);

            // Act
            var result = await _notificationService.SendEmailNotificationAsync(userId, title, message);

            // Assert
            Assert.True(result);
            _mockNotificationRepository.Verify(x => x.CreateAsync(It.IsAny<Notification>()), Times.Once);
            _mockEmailService.Verify(x => x.SendEmailAsync(user.Email, title, message), Times.Once);
        }

        [Fact]
        public async Task SendEmailNotificationAsync_WithInvalidUserId_ReturnsFalse()
        {
            // Arrange
            var userId = 999;
            var title = "Test Notification";
            var message = "Test message";

            _mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _notificationService.SendEmailNotificationAsync(userId, title, message);

            // Assert
            Assert.False(result);
            _mockNotificationRepository.Verify(x => x.CreateAsync(It.IsAny<Notification>()), Times.Never);
            _mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendKeywordBasedNotificationsAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var keywords = new List<string> { "technology" };
            var article = new NewsArticle { Id = 1, Title = "Tech News", Url = "https://example.com/news/1" };
            var user = new User { Id = userId, Email = "test@example.com" };

            _mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockNotificationRepository.Setup(x => x.CreateAsync(It.IsAny<Notification>()))
                .ReturnsAsync(new Notification());
            _mockEmailService.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _notificationService.SendKeywordBasedNotificationsAsync(userId, keywords, article);

            // Assert
            Assert.True(result);
            _mockNotificationRepository.Verify(x => x.CreateAsync(It.IsAny<Notification>()), Times.Once);
            _mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task SendKeywordBasedNotificationsAsync_WithInvalidUserId_ReturnsFalse()
        {
            // Arrange
            var userId = 999;
            var keywords = new List<string> { "technology" };
            var article = new NewsArticle { Id = 1, Title = "Tech News", Url = "https://example.com/news/1" };

            _mockUserService.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _notificationService.SendKeywordBasedNotificationsAsync(userId, keywords, article);

            // Assert
            Assert.False(result);
            _mockNotificationRepository.Verify(x => x.CreateAsync(It.IsAny<Notification>()), Times.Never);
            _mockEmailService.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task CreateOrUpdateNotificationSettingAsync_WithValidData_ReturnsSetting()
        {
            // Arrange
            var setting = new UserNotificationSetting { UserId = 1, CategoryId = 1, IsEnabled = true };
            var createdSetting = new UserNotificationSetting { Id = 1, UserId = 1, CategoryId = 1, IsEnabled = true };

            _mockNotificationRepository.Setup(x => x.CreateOrUpdateNotificationSettingAsync(setting))
                .ReturnsAsync(createdSetting);

            // Act
            var result = await _notificationService.CreateOrUpdateNotificationSettingAsync(setting);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.True(result.IsEnabled);
            _mockNotificationRepository.Verify(x => x.CreateOrUpdateNotificationSettingAsync(setting), Times.Once);
        }
    }
} 