//using Xunit;
//using Moq;
//using NewsAggregation.Server.Services;
//using NewsAggregation.Server.Services.Interfaces;
//using NewsAggregation.Server.Repository.Interfaces;
//using NewsAggregation.Server.Models.Entities;
//using NewsAggregation.Server.Models.Dtos;
//using System.Threading.Tasks;

//namespace NewsAggregation.Tests.Unit
//{
//    public class AuthServiceTests
//    {
//        private readonly Mock<IUserRepository> _mockUserRepository;
//        private readonly Mock<IJwtHelper> _mockJwtHelper;
//        private readonly AuthService _authService;

//        public AuthServiceTests()
//        {
//            _mockUserRepository = new Mock<IUserRepository>();
//            _mockJwtHelper = new Mock<IJwtHelper>();
//            _authService = new AuthService(_mockUserRepository.Object, _mockJwtHelper.Object);
//        }

//        [Fact]
//        public async Task LoginAsync_ShouldReturnToken_WhenValidCredentials()
//        {
//            // Arrange
//            var loginDto = new LoginDto { Username = "testuser", Password = "password123" };
//            var user = new User 
//            { 
//                Id = 1, 
//                Username = "testuser", 
//                Email = "test@example.com",
//                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
//                Role = "User",
//                IsActive = true
//            };
//            var expectedToken = "jwt_token_here";

//            _mockUserRepository.Setup(x => x.GetByUsernameAsync("testuser")).ReturnsAsync(user);
//            _mockJwtHelper.Setup(x => x.GenerateToken(user)).Returns(expectedToken);

//            // Act
//            var result = await _authService.LoginAsync(loginDto);

//            // Assert
//            Assert.NotNull(result);
//            Assert.True(result.Success);
//            Assert.Equal(expectedToken, result.Token);
//            Assert.Equal(user.Username, result.Username);
//            Assert.Equal(user.Role, result.Role);
//        }

//        [Fact]
//        public async Task LoginAsync_ShouldReturnFailure_WhenUserNotFound()
//        {
//            // Arrange
//            var loginDto = new LoginDto { Username = "nonexistent", Password = "password123" };

//            _mockUserRepository.Setup(x => x.GetByUsernameAsync("nonexistent")).ReturnsAsync((User?)null);

//            // Act
//            var result = await _authService.LoginAsync(loginDto);

//            // Assert
//            Assert.NotNull(result);
//            Assert.False(result.Success);
//            Assert.Equal("Invalid username or password", result.Message);
//        }

//        [Fact]
//        public async Task LoginAsync_ShouldReturnFailure_WhenInvalidPassword()
//        {
//            // Arrange
//            var loginDto = new LoginDto { Username = "testuser", Password = "wrongpassword" };
//            var user = new User 
//            { 
//                Id = 1, 
//                Username = "testuser", 
//                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
//                IsActive = true
//            };

//            _mockUserRepository.Setup(x => x.GetByUsernameAsync("testuser")).ReturnsAsync(user);

//            // Act
//            var result = await _authService.LoginAsync(loginDto);

//            // Assert
//            Assert.NotNull(result);
//            Assert.False(result.Success);
//            Assert.Equal("Invalid username or password", result.Message);
//        }

//        [Fact]
//        public async Task LoginAsync_ShouldReturnFailure_WhenUserInactive()
//        {
//            // Arrange
//            var loginDto = new LoginDto { Username = "testuser", Password = "password123" };
//            var user = new User 
//            { 
//                Id = 1, 
//                Username = "testuser", 
//                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
//                IsActive = false
//            };

//            _mockUserRepository.Setup(x => x.GetByUsernameAsync("testuser")).ReturnsAsync(user);

//            // Act
//            var result = await _authService.LoginAsync(loginDto);

//            // Assert
//            Assert.NotNull(result);
//            Assert.False(result.Success);
//            Assert.Equal("Account is deactivated", result.Message);
//        }

//        [Fact]
//        public async Task RegisterAsync_ShouldReturnSuccess_WhenValidData()
//        {
//            // Arrange
//            var registerDto = new RegisterDto 
//            { 
//                Username = "newuser", 
//                Email = "new@example.com", 
//                Password = "password123" 
//            };

//            _mockUserRepository.Setup(x => x.GetByUsernameAsync("newuser")).ReturnsAsync((User?)null);
//            _mockUserRepository.Setup(x => x.GetByEmailAsync("new@example.com")).ReturnsAsync((User?)null);
//            _mockUserRepository.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(new User { Id = 1 });

//            // Act
//            var result = await _authService.RegisterAsync(registerDto);

//            // Assert
//            Assert.NotNull(result);
//            Assert.True(result.Success);
//            Assert.Equal("User registered successfully", result.Message);
//            _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
//        }

//        [Fact]
//        public async Task RegisterAsync_ShouldReturnFailure_WhenUsernameExists()
//        {
//            // Arrange
//            var registerDto = new RegisterDto 
//            { 
//                Username = "existinguser", 
//                Email = "new@example.com", 
//                Password = "password123" 
//            };
//            var existingUser = new User { Username = "existinguser" };

//            _mockUserRepository.Setup(x => x.GetByUsernameAsync("existinguser")).ReturnsAsync(existingUser);

//            // Act
//            var result = await _authService.RegisterAsync(registerDto);

//            // Assert
//            Assert.NotNull(result);
//            Assert.False(result.Success);
//            Assert.Equal("Username already exists", result.Message);
//        }

//        [Fact]
//        public async Task RegisterAsync_ShouldReturnFailure_WhenEmailExists()
//        {
//            // Arrange
//            var registerDto = new RegisterDto 
//            { 
//                Username = "newuser", 
//                Email = "existing@example.com", 
//                Password = "password123" 
//            };
//            var existingUser = new User { Email = "existing@example.com" };

//            _mockUserRepository.Setup(x => x.GetByUsernameAsync("newuser")).ReturnsAsync((User?)null);
//            _mockUserRepository.Setup(x => x.GetByEmailAsync("existing@example.com")).ReturnsAsync(existingUser);

//            // Act
//            var result = await _authService.RegisterAsync(registerDto);

//            // Assert
//            Assert.NotNull(result);
//            Assert.False(result.Success);
//            Assert.Equal("Email already exists", result.Message);
//        }

//        [Fact]
//        public async Task RegisterAsync_ShouldReturnFailure_WhenInvalidEmail()
//        {
//            // Arrange
//            var registerDto = new RegisterDto 
//            { 
//                Username = "newuser", 
//                Email = "invalid-email", 
//                Password = "password123" 
//            };

//            // Act
//            var result = await _authService.RegisterAsync(registerDto);

//            // Assert
//            Assert.NotNull(result);
//            Assert.False(result.Success);
//            Assert.Equal("Invalid email format", result.Message);
//        }

//        [Fact]
//        public async Task RegisterAsync_ShouldReturnFailure_WhenWeakPassword()
//        {
//            // Arrange
//            var registerDto = new RegisterDto 
//            { 
//                Username = "newuser", 
//                Email = "new@example.com", 
//                Password = "123" 
//            };

//            // Act
//            var result = await _authService.RegisterAsync(registerDto);

//            // Assert
//            Assert.NotNull(result);
//            Assert.False(result.Success);
//            Assert.Equal("Password must be at least 6 characters long", result.Message);
//        }

//        [Fact]
//        public async Task ValidateTokenAsync_ShouldReturnUser_WhenValidToken()
//        {
//            // Arrange
//            var token = "valid_token";
//            var userId = 1;
//            var user = new User { Id = userId, Username = "testuser", Role = "User" };

//            _mockJwtHelper.Setup(x => x.ValidateToken(token)).Returns(userId);
//            _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

//            // Act
//            var result = await _authService.ValidateTokenAsync(token);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(user, result);
//        }

//        [Fact]
//        public async Task ValidateTokenAsync_ShouldReturnNull_WhenInvalidToken()
//        {
//            // Arrange
//            var token = "invalid_token";

//            _mockJwtHelper.Setup(x => x.ValidateToken(token)).Returns((int?)null);

//            // Act
//            var result = await _authService.ValidateTokenAsync(token);

//            // Assert
//            Assert.Null(result);
//        }
//    }
//} 