using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NewsAggregation.Server.Services.ExternalClients;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Exceptions;
using Xunit;

namespace NewsAggregation.Tests.Unit.Services.ExternalClients
{
    public class NewsApiClientTests
    {
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly Mock<ILogger<NewsApiClient>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly NewsApiClient _newsApiClient;

        public NewsApiClientTests()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _mockLogger = new Mock<ILogger<NewsApiClient>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _newsApiClient = new NewsApiClient(_mockHttpClient.Object, _mockLogger.Object, _mockConfiguration.Object);
        }

        [Fact]
        public void IsConfigurationValid_WithValidApiKey_ReturnsTrue()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns("valid-api-key");

            // Act
            var result = _newsApiClient.IsConfigurationValid();

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void IsConfigurationValid_WithInvalidApiKey_ReturnsFalse(string apiKey)
        {
            // Arrange
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);

            // Act
            var result = _newsApiClient.IsConfigurationValid();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithValidConfigurationAndResponse_ReturnsArticles()
        {
            // Arrange
            var apiKey = "test-api-key";
            var jsonResponse = @"{
                ""articles"": [
                    {
                        ""title"": ""Test Article"",
                        ""description"": ""Test Description"",
                        ""url"": ""https://example.com/article"",
                        ""source"": { ""name"": ""Test Source"" },
                        ""publishedAt"": ""2023-01-01T12:00:00Z"",
                        ""urlToImage"": ""https://example.com/image.jpg""
                    }
                ]
            }";

            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jsonResponse);

            // Act
            var result = await _newsApiClient.FetchTopHeadlinesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            var article = resultList[0];
            Assert.Equal("Test Article", article.Title);
            Assert.Equal("Test Description", article.Description);
            Assert.Equal("https://example.com/article", article.Url);
            Assert.Equal("Test Source", article.Source);
            Assert.Equal("https://example.com/image.jpg", article.ImageUrl);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithInvalidConfiguration_ThrowsConfigurationException()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns((string?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NewsApiConfigurationException>(() => _newsApiClient.FetchTopHeadlinesAsync());
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithHttpRequestException_ThrowsNewsApiException()
        {
            // Arrange
            var apiKey = "test-api-key";
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NewsApiException>(() => _newsApiClient.FetchTopHeadlinesAsync());
            Assert.Contains("HTTP request failed", exception.Message);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithJsonException_ThrowsNewsApiException()
        {
            // Arrange
            var apiKey = "test-api-key";
            var invalidJson = "invalid json";
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalidJson);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NewsApiException>(() => _newsApiClient.FetchTopHeadlinesAsync());
            Assert.Contains("Failed to parse JSON", exception.Message);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithTaskCanceledException_ThrowsNewsApiException()
        {
            // Arrange
            var apiKey = "test-api-key";
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException("Request cancelled"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NewsApiException>(() => _newsApiClient.FetchTopHeadlinesAsync());
            Assert.Contains("Request to NewsAPI was cancelled", exception.Message);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithEmptyArticlesArray_ReturnsEmptyList()
        {
            // Arrange
            var apiKey = "test-api-key";
            var jsonResponse = @"{ ""articles"": [] }";

            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jsonResponse);

            // Act
            var result = await _newsApiClient.FetchTopHeadlinesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Empty(resultList);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithMissingArticlesProperty_ReturnsEmptyList()
        {
            // Arrange
            var apiKey = "test-api-key";
            var jsonResponse = @"{ ""status"": ""ok"" }";

            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jsonResponse);

            // Act
            var result = await _newsApiClient.FetchTopHeadlinesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Empty(resultList);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithInvalidArticleData_HandlesGracefully()
        {
            // Arrange
            var apiKey = "test-api-key";
            var jsonResponse = @"{
                ""articles"": [
                    {
                        ""title"": ""Valid Article"",
                        ""description"": ""Valid Description"",
                        ""url"": ""https://example.com/valid"",
                        ""source"": { ""name"": ""Valid Source"" },
                        ""publishedAt"": ""2023-01-01T12:00:00Z""
                    },
                    {
                        ""invalid"": ""data""
                    }
                ]
            }";

            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jsonResponse);

            // Act
            var result = await _newsApiClient.FetchTopHeadlinesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Valid Article", resultList[0].Title);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithNullValues_HandlesGracefully()
        {
            // Arrange
            var apiKey = "test-api-key";
            var jsonResponse = @"{
                ""articles"": [
                    {
                        ""title"": null,
                        ""description"": null,
                        ""url"": null,
                        ""source"": { ""name"": null },
                        ""publishedAt"": null,
                        ""urlToImage"": null
                    }
                ]
            }";

            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jsonResponse);

            // Act
            var result = await _newsApiClient.FetchTopHeadlinesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            var article = resultList[0];
            Assert.Equal("No Title", article.Title);
            Assert.Equal("No Description", article.Description);
            Assert.Equal(string.Empty, article.Url);
            Assert.Equal("NewsAPI", article.Source);
            Assert.Null(article.ImageUrl);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithInvalidDate_DefaultsToUtcNow()
        {
            // Arrange
            var apiKey = "test-api-key";
            var jsonResponse = @"{
                ""articles"": [
                    {
                        ""title"": ""Test Article"",
                        ""description"": ""Test Description"",
                        ""url"": ""https://example.com/article"",
                        ""source"": { ""name"": ""Test Source"" },
                        ""publishedAt"": ""invalid-date"",
                        ""urlToImage"": ""https://example.com/image.jpg""
                    }
                ]
            }";

            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jsonResponse);

            // Act
            var result = await _newsApiClient.FetchTopHeadlinesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            var article = resultList[0];
            Assert.Equal("Test Article", article.Title);
            Assert.True(article.PublishedAt > DateTime.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithCancellationToken_RespectsCancellation()
        {
            // Arrange
            var apiKey = "test-api-key";
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TaskCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<NewsApiException>(() => _newsApiClient.FetchTopHeadlinesAsync(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithUnexpectedException_ThrowsNewsApiException()
        {
            // Arrange
            var apiKey = "test-api-key";
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NewsApiException>(() => _newsApiClient.FetchTopHeadlinesAsync());
            Assert.Contains("Unexpected error occurred", exception.Message);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithMultipleArticles_ReturnsAllArticles()
        {
            // Arrange
            var apiKey = "test-api-key";
            var jsonResponse = @"{
                ""articles"": [
                    {
                        ""title"": ""Article 1"",
                        ""description"": ""Description 1"",
                        ""url"": ""https://example.com/article1"",
                        ""source"": { ""name"": ""Source 1"" },
                        ""publishedAt"": ""2023-01-01T12:00:00Z""
                    },
                    {
                        ""title"": ""Article 2"",
                        ""description"": ""Description 2"",
                        ""url"": ""https://example.com/article2"",
                        ""source"": { ""name"": ""Source 2"" },
                        ""publishedAt"": ""2023-01-01T13:00:00Z""
                    }
                ]
            }";

            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _mockHttpClient.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(jsonResponse);

            // Act
            var result = await _newsApiClient.FetchTopHeadlinesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, a => a.Title == "Article 1");
            Assert.Contains(resultList, a => a.Title == "Article 2");
        }
    }
} 