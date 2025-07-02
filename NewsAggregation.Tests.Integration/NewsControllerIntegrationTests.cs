using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsAggregation.Server.Data;
using NewsAggregation.Server.Models.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace NewsAggregation.Tests.Integration
{
    public class NewsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public NewsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace the database context with an in-memory database for testing
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<NewsAggregationContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<NewsAggregationContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetHeadlines_ShouldReturnOkResult()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.GetAsync("/api/News/headlines");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", content.ToLower());
        }

        [Fact]
        public async Task GetHeadlinesByCategory_ShouldReturnOkResult()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.GetAsync("/api/News/category/1/headlines");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", content.ToLower());
        }

        [Fact]
        public async Task GetTodayHeadlines_ShouldReturnOkResult()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.GetAsync("/api/News/today");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", content.ToLower());
        }

        [Fact]
        public async Task SearchNews_ShouldReturnOkResult()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.GetAsync("/api/News/search?query=test");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("success", content.ToLower());
        }

        [Fact]
        public async Task GetRecommendations_ShouldReturnUnauthorized_WhenNoToken()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.GetAsync("/api/News/recommendations");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetUserPreferences_ShouldReturnUnauthorized_WhenNoToken()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.GetAsync("/api/News/user-preferences");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ReportArticle_ShouldReturnUnauthorized_WhenNoToken()
        {
            // Arrange
            await SeedTestData();
            var reportData = new { ArticleId = 1, Reason = "Test reason" };
            var json = JsonSerializer.Serialize(reportData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/News/report", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task LikeArticle_ShouldReturnUnauthorized_WhenNoToken()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.PostAsync("/api/News/1/like", null);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task SaveArticle_ShouldReturnUnauthorized_WhenNoToken()
        {
            // Arrange
            await SeedTestData();
            var saveData = new { ArticleId = 1 };
            var json = JsonSerializer.Serialize(saveData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/News/save", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetSavedArticles_ShouldReturnUnauthorized_WhenNoToken()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.GetAsync("/api/News/saved");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetPersonalizedArticlesByCategory_ShouldReturnUnauthorized_WhenNoToken()
        {
            // Arrange
            await SeedTestData();

            // Act
            var response = await _client.GetAsync("/api/News/category/1/personalized");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task SeedTestData()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NewsAggregationContext>();

            // Clear existing data
            context.NewsArticles.RemoveRange(context.NewsArticles);
            context.Categories.RemoveRange(context.Categories);
            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync();

            // Add test categories
            var category = new Category
            {
                Id = 1,
                Name = "Business",
                Description = "Business news",
                IsActive = true,
                IsHidden = false,
                CreatedAt = DateTime.UtcNow
            };
            context.Categories.Add(category);

            // Add test user
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);

            // Add test articles
            var articles = new List<NewsArticle>
            {
                new NewsArticle
                {
                    Id = 1,
                    Title = "Test Article 1",
                    Description = "This is a test article",
                    Url = "https://example.com/article1",
                    Source = "Test Source",
                    CategoryId = 1,
                    PublishedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsHidden = false
                },
                new NewsArticle
                {
                    Id = 2,
                    Title = "Test Article 2",
                    Description = "This is another test article",
                    Url = "https://example.com/article2",
                    Source = "Test Source",
                    CategoryId = 1,
                    PublishedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    IsHidden = false
                }
            };
            context.NewsArticles.AddRange(articles);

            await context.SaveChangesAsync();
        }
    }
} 