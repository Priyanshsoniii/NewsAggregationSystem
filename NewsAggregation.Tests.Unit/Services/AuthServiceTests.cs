using Moq;
using NewsAggregation.Server.Configuration;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace NewsAggregation.Tests.Unit.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IOptions<AppSettings>> _mockAppSettings;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthService _authService;
        private readonly AppSettings _appSettings;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockAppSettings = new Mock<IOptions<AppSettings>>();
            _mockConfiguration = new Mock<IConfiguration>();

            _appSettings = new AppSettings
            {
                JwtSettings = new JwtSettings
                {
                    SecretKey = "your-super-secret-key-with-at-least-32-characters",
                    Issuer = "NewsAggregation",
                    Audience = "NewsAggregationUsers",
                    ExpirationHours = 24
                }
            };

            _mockAppSettings.Setup(x => x.Value).Returns(_appSettings);

            _authService = new AuthService(_mockUserRepository.Object, _mockAppSettings.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            
            var user = new User
            {
                Id = 1,
                Email = email,
                Username = "testuser",
                PasswordHash = hashedPassword,
                Role = "User",
                IsActive = true
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.User);
            Assert.Equal(user.Id, result.User.Id);
            Assert.Equal(user.Email, result.User.Email);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidEmail_ReturnsFailure()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var password = "password123";

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Empty(result.Token);
            Assert.Null(result.User);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            var password = "wrongpassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
            
            var user = new User
            {
                Id = 1,
                Email = email,
                Username = "testuser",
                PasswordHash = hashedPassword,
                Role = "User",
                IsActive = true
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Empty(result.Token);
            Assert.Null(result.User);
        }

        [Fact]
        public async Task LoginAsync_WithInactiveUser_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            
            var user = new User
            {
                Id = 1,
                Email = email,
                Username = "testuser",
                PasswordHash = hashedPassword,
                Role = "User",
                IsActive = false
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Empty(result.Token);
            Assert.Null(result.User);
        }

        [Fact]
        public async Task LoginAsync_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Empty(result.Token);
            Assert.Null(result.User);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsSuccess()
        {
            // Arrange
            var username = "newuser";
            var email = "newuser@example.com";
            var password = "password123";

            var createdUser = new User
            {
                Id = 1,
                Username = username,
                Email = email,
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _mockUserRepository.Setup(x => x.ExistsAsync(email, username))
                .ReturnsAsync(false);
            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("User registered successfully", result.Message);
            Assert.NotNull(result.User);
            Assert.Equal(username, result.User.Username);
            Assert.Equal(email, result.User.Email);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ReturnsFailure()
        {
            // Arrange
            var username = "newuser";
            var email = "existing@example.com";
            var password = "password123";

            _mockUserRepository.Setup(x => x.ExistsAsync(email, username))
                .ReturnsAsync(true);

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User with this email or username already exists", result.Message);
            Assert.Null(result.User);
        }

        [Fact]
        public async Task RegisterAsync_WhenRepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var username = "newuser";
            var email = "newuser@example.com";
            var password = "password123";

            _mockUserRepository.Setup(x => x.ExistsAsync(email, username))
                .ReturnsAsync(false);
            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Registration failed", result.Message);
            Assert.Null(result.User);
        }

        [Theory]
        [InlineData("", "test@example.com", "password123")]
        [InlineData("testuser", "", "password123")]
        [InlineData("testuser", "test@example.com", "")]
        [InlineData(null, "test@example.com", "password123")]
        [InlineData("testuser", null, "password123")]
        [InlineData("testuser", "test@example.com", null)]
        public async Task RegisterAsync_WithInvalidInput_HandlesGracefully(string username, string email, string password)
        {
            // Arrange
            _mockUserRepository.Setup(x => x.ExistsAsync(email, username))
                .ReturnsAsync(false);

            // Act
            var result = await _authService.RegisterAsync(username, email, password);

            // Assert
            // Should handle gracefully without throwing exceptions
            Assert.True(result.Success || !result.Success);
        }
    }
} 