using Moq;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services;
using Xunit;

namespace NewsAggregation.Tests.Unit.Services
{
    public class ExternalServerServiceTests
    {
        private readonly Mock<IExternalServerRepository> _mockExternalServerRepository;
        private readonly ExternalServerService _externalServerService;

        public ExternalServerServiceTests()
        {
            _mockExternalServerRepository = new Mock<IExternalServerRepository>();
            _externalServerService = new ExternalServerService(_mockExternalServerRepository.Object);
        }

        [Fact]
        public async Task GetAllServersAsync_ReturnsAllServers()
        {
            // Arrange
            var servers = new List<ExternalServer>
            {
                new ExternalServer { Id = 1, Name = "News API", ServerType = "NewsAPI", IsActive = true },
                new ExternalServer { Id = 2, Name = "The News API", ServerType = "TheNewsAPI", IsActive = true },
                new ExternalServer { Id = 3, Name = "BBC RSS", ServerType = "BBCRSS", IsActive = false }
            };

            _mockExternalServerRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(servers);

            // Act
            var result = await _externalServerService.GetAllServersAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.Contains(resultList, s => s.Name == "News API");
            Assert.Contains(resultList, s => s.Name == "The News API");
            Assert.Contains(resultList, s => s.Name == "BBC RSS");
        }

        [Fact]
        public async Task GetServerByIdAsync_WithValidId_ReturnsServer()
        {
            // Arrange
            var serverId = 1;
            var server = new ExternalServer { Id = serverId, Name = "News API", ServerType = "NewsAPI", IsActive = true };

            _mockExternalServerRepository.Setup(x => x.GetByIdAsync(serverId)).ReturnsAsync(server);

            // Act
            var result = await _externalServerService.GetServerByIdAsync(serverId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(serverId, result.Id);
            Assert.Equal("News API", result.Name);
            Assert.Equal("NewsAPI", result.ServerType);
        }

        [Fact]
        public async Task GetServerByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var serverId = 999;

            _mockExternalServerRepository.Setup(x => x.GetByIdAsync(serverId)).ReturnsAsync((ExternalServer?)null);

            // Act
            var result = await _externalServerService.GetServerByIdAsync(serverId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateServerAsync_WithValidData_ReturnsCreatedServer()
        {
            // Arrange
            var server = new ExternalServer
            {
                Name = "New Server",
                ApiUrl = "https://api.example.com",
                ApiKey = "test-key",
                ServerType = "TestAPI",
                RequestsPerHour = 1000
            };

            var createdServer = new ExternalServer
            {
                Id = 1,
                Name = "New Server",
                ApiUrl = "https://api.example.com",
                ApiKey = "test-key",
                ServerType = "TestAPI",
                RequestsPerHour = 1000,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _mockExternalServerRepository.Setup(x => x.CreateAsync(It.IsAny<ExternalServer>())).ReturnsAsync(createdServer);

            // Act
            var result = await _externalServerService.CreateServerAsync(server);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Server", result.Name);
            Assert.True(result.IsActive);
            Assert.Equal(0, result.CurrentHourRequests);
            _mockExternalServerRepository.Verify(x => x.CreateAsync(It.Is<ExternalServer>(s => 
                s.IsActive && s.CurrentHourRequests == 0 && s.CreatedAt > DateTime.UtcNow.AddMinutes(-1))), Times.Once);
        }

        [Fact]
        public async Task UpdateServerAsync_WithValidData_ReturnsUpdatedServer()
        {
            // Arrange
            var serverId = 1;
            var existingServer = new ExternalServer
            {
                Id = serverId,
                Name = "Old Name",
                ApiUrl = "https://old-api.example.com",
                ApiKey = "old-key",
                ServerType = "OldAPI",
                IsActive = true,
                RequestsPerHour = 500
            };

            var updatedServer = new ExternalServer
            {
                Id = serverId,
                Name = "Updated Name",
                ApiUrl = "https://new-api.example.com",
                ApiKey = "new-key",
                ServerType = "NewAPI",
                IsActive = false,
                RequestsPerHour = 1000
            };

            var resultServer = new ExternalServer
            {
                Id = serverId,
                Name = "Updated Name",
                ApiUrl = "https://new-api.example.com",
                ApiKey = "new-key",
                ServerType = "NewAPI",
                IsActive = false,
                RequestsPerHour = 1000
            };

            _mockExternalServerRepository.Setup(x => x.GetByIdAsync(serverId)).ReturnsAsync(existingServer);
            _mockExternalServerRepository.Setup(x => x.UpdateAsync(It.IsAny<ExternalServer>())).ReturnsAsync(resultServer);

            // Act
            var result = await _externalServerService.UpdateServerAsync(updatedServer);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(serverId, result.Id);
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal("https://new-api.example.com", result.ApiUrl);
            Assert.Equal("new-key", result.ApiKey);
            Assert.Equal("NewAPI", result.ServerType);
            Assert.False(result.IsActive);
            Assert.Equal(1000, result.RequestsPerHour);
            _mockExternalServerRepository.Verify(x => x.UpdateAsync(It.Is<ExternalServer>(s => 
                s.Name == "Updated Name" && s.ApiUrl == "https://new-api.example.com")), Times.Once);
        }

        [Fact]
        public async Task UpdateServerAsync_WithNonExistentServer_ThrowsArgumentException()
        {
            // Arrange
            var serverId = 999;
            var server = new ExternalServer { Id = serverId, Name = "Non-existent Server" };

            _mockExternalServerRepository.Setup(x => x.GetByIdAsync(serverId)).ReturnsAsync((ExternalServer?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _externalServerService.UpdateServerAsync(server));
            _mockExternalServerRepository.Verify(x => x.UpdateAsync(It.IsAny<ExternalServer>()), Times.Never);
        }

        [Fact]
        public async Task DeleteServerAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var serverId = 1;

            _mockExternalServerRepository.Setup(x => x.DeleteAsync(serverId)).ReturnsAsync(true);

            // Act
            var result = await _externalServerService.DeleteServerAsync(serverId);

            // Assert
            Assert.True(result);
            _mockExternalServerRepository.Verify(x => x.DeleteAsync(serverId), Times.Once);
        }

        [Fact]
        public async Task DeleteServerAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var serverId = 999;

            _mockExternalServerRepository.Setup(x => x.DeleteAsync(serverId)).ReturnsAsync(false);

            // Act
            var result = await _externalServerService.DeleteServerAsync(serverId);

            // Assert
            Assert.False(result);
            _mockExternalServerRepository.Verify(x => x.DeleteAsync(serverId), Times.Once);
        }

        [Fact]
        public async Task ToggleServerStatusAsync_WithActiveServer_DeactivatesServer()
        {
            // Arrange
            var serverId = 1;
            var server = new ExternalServer { Id = serverId, Name = "Test Server", IsActive = true };

            _mockExternalServerRepository.Setup(x => x.GetByIdAsync(serverId)).ReturnsAsync(server);
            _mockExternalServerRepository.Setup(x => x.UpdateAsync(It.IsAny<ExternalServer>())).ReturnsAsync(server);

            // Act
            var result = await _externalServerService.ToggleServerStatusAsync(serverId);

            // Assert
            Assert.True(result);
            _mockExternalServerRepository.Verify(x => x.UpdateAsync(It.Is<ExternalServer>(s => !s.IsActive)), Times.Once);
        }

        [Fact]
        public async Task ToggleServerStatusAsync_WithInactiveServer_ActivatesServer()
        {
            // Arrange
            var serverId = 1;
            var server = new ExternalServer { Id = serverId, Name = "Test Server", IsActive = false };

            _mockExternalServerRepository.Setup(x => x.GetByIdAsync(serverId)).ReturnsAsync(server);
            _mockExternalServerRepository.Setup(x => x.UpdateAsync(It.IsAny<ExternalServer>())).ReturnsAsync(server);

            // Act
            var result = await _externalServerService.ToggleServerStatusAsync(serverId);

            // Assert
            Assert.True(result);
            _mockExternalServerRepository.Verify(x => x.UpdateAsync(It.Is<ExternalServer>(s => s.IsActive)), Times.Once);
        }

        [Fact]
        public async Task ToggleServerStatusAsync_WithNonExistentServer_ReturnsFalse()
        {
            // Arrange
            var serverId = 999;

            _mockExternalServerRepository.Setup(x => x.GetByIdAsync(serverId)).ReturnsAsync((ExternalServer?)null);

            // Act
            var result = await _externalServerService.ToggleServerStatusAsync(serverId);

            // Assert
            Assert.False(result);
            _mockExternalServerRepository.Verify(x => x.UpdateAsync(It.IsAny<ExternalServer>()), Times.Never);
        }

        [Fact]
        public async Task UpdateServerLastAccessedAsync_WithValidId_UpdatesServer()
        {
            // Arrange
            var serverId = 1;

            _mockExternalServerRepository.Setup(x => x.UpdateLastAccessedAsync(serverId)).Returns(Task.CompletedTask);

            // Act
            await _externalServerService.UpdateServerLastAccessedAsync(serverId);

            // Assert
            _mockExternalServerRepository.Verify(x => x.UpdateLastAccessedAsync(serverId), Times.Once);
        }

        [Fact]
        public async Task GetAllServersAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockExternalServerRepository.Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _externalServerService.GetAllServersAsync());
        }

        [Fact]
        public async Task CreateServerAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var server = new ExternalServer { Name = "Test Server" };

            _mockExternalServerRepository.Setup(x => x.CreateAsync(It.IsAny<ExternalServer>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _externalServerService.CreateServerAsync(server));
        }

        [Fact]
        public async Task UpdateServerAsync_WhenRepositoryThrowsException_PropagatesException()
        {
            // Arrange
            var serverId = 1;
            var server = new ExternalServer { Id = serverId, Name = "Test Server" };
            var existingServer = new ExternalServer { Id = serverId, Name = "Existing Server" };

            _mockExternalServerRepository.Setup(x => x.GetByIdAsync(serverId)).ReturnsAsync(existingServer);
            _mockExternalServerRepository.Setup(x => x.UpdateAsync(It.IsAny<ExternalServer>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _externalServerService.UpdateServerAsync(server));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateServerAsync_WithInvalidName_HandlesGracefully(string name)
        {
            // Arrange
            var server = new ExternalServer { Name = name };

            _mockExternalServerRepository.Setup(x => x.CreateAsync(It.IsAny<ExternalServer>()))
                .ReturnsAsync(new ExternalServer { Id = 1, Name = name });

            // Act
            var result = await _externalServerService.CreateServerAsync(server);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
        }
    }
} 