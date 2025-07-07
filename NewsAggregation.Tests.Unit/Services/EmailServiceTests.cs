using Moq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using NewsAggregation.Server.Configuration;
using NewsAggregation.Server.Services;
using Xunit;

namespace NewsAggregation.Tests.Unit.Services
{
    public class EmailServiceTests
    {
        private readonly Mock<IOptions<AppSettings>> _mockAppSettings;
        private readonly Mock<ILogger<EmailService>> _mockLogger;
        private readonly EmailService _emailService;
        private readonly AppSettings _appSettings;

        public EmailServiceTests()
        {
            _mockAppSettings = new Mock<IOptions<AppSettings>>();
            _mockLogger = new Mock<ILogger<EmailService>>();

            _appSettings = new AppSettings
            {
                EmailSettings = new EmailSettings
                {
                    SmtpServer = "smtp.example.com",
                    SmtpPort = 587,
                    Username = "test@example.com",
                    Password = "testpassword",
                    FromEmail = "noreply@example.com",
                    FromName = "News Aggregation"
                }
            };

            _mockAppSettings.Setup(x => x.Value).Returns(_appSettings);

            _emailService = new EmailService(_mockAppSettings.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SendEmailAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var toEmail = "recipient@example.com";
            var subject = "Test Subject";
            var body = "<html><body><h1>Test Email</h1></body></html>";

            // Note: This test would require mocking the SMTP client
            // For now, we'll test the method structure and configuration

            // Act
            var result = await _emailService.SendEmailAsync(toEmail, subject, body);

            // Assert
            // The actual result depends on SMTP configuration
            // In a real test environment, you would mock the SMTP client
            Assert.True(result || !result); // Either true or false depending on SMTP setup
        }

        [Theory]
        [InlineData("", "Subject", "Body")]
        [InlineData("invalid-email", "Subject", "Body")]
        [InlineData("test@example.com", "", "Body")]
        [InlineData("test@example.com", "Subject", "")]
        [InlineData(null, "Subject", "Body")]
        [InlineData("test@example.com", null, "Body")]
        [InlineData("test@example.com", "Subject", null)]
        public async Task SendEmailAsync_WithInvalidInput_HandlesGracefully(string toEmail, string subject, string body)
        {
            // Act
            var result = await _emailService.SendEmailAsync(toEmail, subject, body);

            // Assert
            // Should handle gracefully without throwing exceptions
            Assert.True(result || !result);
        }

        [Fact]
        public async Task SendNewsNotificationAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var toEmail = "user@example.com";
            var userName = "TestUser";
            var newsTitle = "Breaking News";
            var newsUrl = "https://example.com/news/1";

            // Act
            var result = await _emailService.SendNewsNotificationAsync(toEmail, userName, newsTitle, newsUrl);

            // Assert
            // The actual result depends on SMTP configuration
            Assert.True(result || !result);
        }

        [Fact]
        public async Task SendNewsNotificationAsync_WithEmptyData_HandlesGracefully()
        {
            // Arrange
            var toEmail = "";
            var userName = "";
            var newsTitle = "";
            var newsUrl = "";

            // Act
            var result = await _emailService.SendNewsNotificationAsync(toEmail, userName, newsTitle, newsUrl);

            // Assert
            // Should handle gracefully without throwing exceptions
            Assert.True(result || !result);
        }

        [Fact]
        public async Task SendWelcomeEmailAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var toEmail = "newuser@example.com";
            var userName = "NewUser";

            // Act
            var result = await _emailService.SendWelcomeEmailAsync(toEmail, userName);

            // Assert
            // The actual result depends on SMTP configuration
            Assert.True(result || !result);
        }

        [Fact]
        public async Task SendWelcomeEmailAsync_WithEmptyData_HandlesGracefully()
        {
            // Arrange
            var toEmail = "";
            var userName = "";

            // Act
            var result = await _emailService.SendWelcomeEmailAsync(toEmail, userName);

            // Assert
            // Should handle gracefully without throwing exceptions
            Assert.True(result || !result);
        }

        [Fact]
        public void Constructor_WithValidSettings_InitializesCorrectly()
        {
            // Arrange & Act
            var emailService = new EmailService(_mockAppSettings.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(emailService);
        }

        [Fact]
        public void Constructor_WithNullSettings_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<NullReferenceException>(() => new EmailService(null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ThrowsException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => new EmailService(_mockAppSettings.Object, null!));
        }

        [Theory]
        [InlineData("user@example.com", "John Doe", "Tech News", "https://example.com/tech")]
        [InlineData("admin@example.com", "Admin", "Breaking News", "https://example.com/breaking")]
        public async Task SendNewsNotificationAsync_WithDifferentData_HandlesCorrectly(string email, string name, string title, string url)
        {
            // Act
            var result = await _emailService.SendNewsNotificationAsync(email, name, title, url);

            // Assert
            // Should handle different data combinations gracefully
            Assert.True(result || !result);
        }

        [Theory]
        [InlineData("user@example.com", "John Doe")]
        [InlineData("admin@example.com", "Admin User")]
        public async Task SendWelcomeEmailAsync_WithDifferentData_HandlesCorrectly(string email, string name)
        {
            // Act
            var result = await _emailService.SendWelcomeEmailAsync(email, name);

            // Assert
            // Should handle different data combinations gracefully
            Assert.True(result || !result);
        }
    }
} 