using Moq;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services;
using Xunit;

namespace NewsAggregation.Tests.Unit.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                Role = "User",
                IsActive = true
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("testuser", result.Username);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var userId = 999;

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByEmailAsync_WithValidEmail_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = email,
                Role = "User",
                IsActive = true
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull()
        {
            // Arrange
            var email = "nonexistent@example.com";

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetByEmailAsync(email);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "user1@example.com", IsActive = true },
                new User { Id = 2, Username = "user2", Email = "user2@example.com", IsActive = true },
                new User { Id = 3, Username = "user3", Email = "user3@example.com", IsActive = false }
            };

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.Contains(resultList, u => u.Username == "user1");
            Assert.Contains(resultList, u => u.Username == "user2");
            Assert.Contains(resultList, u => u.Username == "user3");
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ReturnsUpdatedUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "updateduser",
                Email = "updated@example.com",
                Role = "User",
                IsActive = true
            };

            var updatedUser = new User
            {
                Id = 1,
                Username = "updateduser",
                Email = "updated@example.com",
                Role = "User",
                IsActive = true
            };

            _mockUserRepository.Setup(x => x.UpdateAsync(user)).ReturnsAsync(updatedUser);

            // Act
            var result = await _userService.UpdateAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("updateduser", result.Username);
            Assert.Equal("updated@example.com", result.Email);
            _mockUserRepository.Verify(x => x.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var userId = 1;

            _mockUserRepository.Setup(x => x.DeleteAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            Assert.True(result);
            _mockUserRepository.Verify(x => x.DeleteAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var userId = 999;

            _mockUserRepository.Setup(x => x.DeleteAsync(userId)).ReturnsAsync(false);

            // Act
            var result = await _userService.DeleteAsync(userId);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(x => x.DeleteAsync(userId), Times.Once);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingEmail_ReturnsTrue()
        {
            // Arrange
            var email = "existing@example.com";
            var username = "existinguser";

            _mockUserRepository.Setup(x => x.ExistsAsync(email, username)).ReturnsAsync(true);

            // Act
            var result = await _userService.ExistsAsync(email, username);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingEmail_ReturnsFalse()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var username = "nonexistentuser";

            _mockUserRepository.Setup(x => x.ExistsAsync(email, username)).ReturnsAsync(false);

            // Act
            var result = await _userService.ExistsAsync(email, username);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetActiveUsersAsync_ReturnsOnlyActiveUsers()
        {
            // Arrange
            var allUsers = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "user1@example.com", IsActive = true },
                new User { Id = 2, Username = "user2", Email = "user2@example.com", IsActive = true },
                new User { Id = 3, Username = "user3", Email = "user3@example.com", IsActive = false },
                new User { Id = 4, Username = "user4", Email = "user4@example.com", IsActive = true }
            };

            _mockUserRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(allUsers);

            // Act
            var result = await _userService.GetActiveUsersAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.All(resultList, u => Assert.True(u.IsActive));
            Assert.Contains(resultList, u => u.Username == "user1");
            Assert.Contains(resultList, u => u.Username == "user2");
            Assert.Contains(resultList, u => u.Username == "user4");
            Assert.DoesNotContain(resultList, u => u.Username == "user3");
        }

        [Fact]
        public async Task GetByIdAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var userId = 1;

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.GetByIdAsync(userId));
        }

        [Fact]
        public async Task UpdateAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser" };

            _mockUserRepository.Setup(x => x.UpdateAsync(user))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.UpdateAsync(user));
        }

        [Fact]
        public async Task GetAllAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockUserRepository.Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.GetAllAsync());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("invalid-email")]
        public async Task GetByEmailAsync_WithInvalidEmail_ReturnsNull(string email)
        {
            // Arrange
            _mockUserRepository.Setup(x => x.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetByEmailAsync(email);

            // Assert
            Assert.Null(result);
        }
    }
} 