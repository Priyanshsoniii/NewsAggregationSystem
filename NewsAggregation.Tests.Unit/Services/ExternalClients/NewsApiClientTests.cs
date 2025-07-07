using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using NewsAggregation.Server.Services.ExternalClients;
using NewsAggregation.Server.Exceptions;
using Xunit;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace NewsAggregation.Tests.Unit.Services.ExternalClients
{
    public class NewsApiClientTests
    {
        private readonly Mock<ILogger<NewsApiClient>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly NewsApiClient _newsApiClient;
        private readonly TestHttpMessageHandler _testHttpHandler;

        public NewsApiClientTests()
        {
            _testHttpHandler = new TestHttpMessageHandler();
            var httpClient = new HttpClient(_testHttpHandler);
            _mockLogger = new Mock<ILogger<NewsApiClient>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _newsApiClient = new NewsApiClient(httpClient, _mockLogger.Object, _mockConfiguration.Object);
        }

        [Fact]
        public void IsConfigurationValid_WithValidApiKey_ReturnsTrue()
        {
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns("valid-api-key");
            Assert.True(_newsApiClient.IsConfigurationValid());
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void IsConfigurationValid_WithInvalidApiKey_ReturnsFalse(string apiKey)
        {
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            Assert.False(_newsApiClient.IsConfigurationValid());
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithValidConfigurationAndResponse_ReturnsArticles()
        {
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
            _testHttpHandler.SetResponse(jsonResponse);

            var result = await _newsApiClient.FetchTopHeadlinesAsync();
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
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns((string?)null);
            await Assert.ThrowsAsync<NewsApiConfigurationException>(() => _newsApiClient.FetchTopHeadlinesAsync());
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithHttpRequestException_ThrowsNewsApiException()
        {
            var apiKey = "test-api-key";
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _testHttpHandler.SetException(new HttpRequestException("Network error"));
            var exception = await Assert.ThrowsAsync<NewsApiException>(() => _newsApiClient.FetchTopHeadlinesAsync());
            Assert.Contains("HTTP request failed", exception.Message);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithJsonException_ThrowsNewsApiException()
        {
            var apiKey = "test-api-key";
            var invalidJson = "invalid json";
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _testHttpHandler.SetResponse(invalidJson);
            var exception = await Assert.ThrowsAsync<NewsApiException>(() => _newsApiClient.FetchTopHeadlinesAsync());
            Assert.Contains("Failed to parse JSON", exception.Message);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithTaskCanceledException_ThrowsNewsApiException()
        {
            var apiKey = "test-api-key";
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _testHttpHandler.SetException(new TaskCanceledException("Request cancelled"));
            var exception = await Assert.ThrowsAsync<NewsApiException>(() => _newsApiClient.FetchTopHeadlinesAsync());
            Assert.Contains("Request to NewsAPI was cancelled", exception.Message);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithEmptyArticlesArray_ReturnsEmptyList()
        {
            var apiKey = "test-api-key";
            var jsonResponse = @"{ ""articles"": [] }";
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _testHttpHandler.SetResponse(jsonResponse);
            var result = await _newsApiClient.FetchTopHeadlinesAsync();
            var resultList = result.ToList();
            Assert.Empty(resultList);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithMissingArticlesProperty_ReturnsEmptyList()
        {
            var apiKey = "test-api-key";
            var jsonResponse = @"{ ""status"": ""ok"" }";
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _testHttpHandler.SetResponse(jsonResponse);
            var result = await _newsApiClient.FetchTopHeadlinesAsync();
            var resultList = result.ToList();
            Assert.Empty(resultList);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithInvalidArticleData_HandlesGracefully()
        {
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
            _testHttpHandler.SetResponse(jsonResponse);
            var result = await _newsApiClient.FetchTopHeadlinesAsync();
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal("Valid Article", resultList[0].Title);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithNullValues_HandlesGracefully()
        {
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
            _testHttpHandler.SetResponse(jsonResponse);
            var result = await _newsApiClient.FetchTopHeadlinesAsync();
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
            _testHttpHandler.SetResponse(jsonResponse);
            var result = await _newsApiClient.FetchTopHeadlinesAsync();
            var resultList = result.ToList();
            Assert.Single(resultList);
            var article = resultList[0];
            Assert.Equal("Test Article", article.Title);
            Assert.True(article.PublishedAt > System.DateTime.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithCancellationToken_RespectsCancellation()
        {
            var apiKey = "test-api-key";
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _testHttpHandler.SetException(new TaskCanceledException());
            await Assert.ThrowsAsync<NewsApiException>(() => _newsApiClient.FetchTopHeadlinesAsync(cancellationTokenSource.Token));
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithUnexpectedException_ThrowsNewsApiException()
        {
            var apiKey = "test-api-key";
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _testHttpHandler.SetException(new System.Exception("Unexpected error"));
            var exception = await Assert.ThrowsAsync<NewsApiException>(() => _newsApiClient.FetchTopHeadlinesAsync());
            Assert.Contains("Unexpected error occurred", exception.Message);
        }

        [Fact]
        public async Task FetchTopHeadlinesAsync_WithMultipleArticles_ReturnsAllArticles()
        {
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
                        ""publishedAt"": ""2023-01-02T12:00:00Z""
                    }
                ]
            }";
            _mockConfiguration.Setup(x => x["NewsAPI:ApiKey"]).Returns(apiKey);
            _testHttpHandler.SetResponse(jsonResponse);
            var result = await _newsApiClient.FetchTopHeadlinesAsync();
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal("Article 1", resultList[0].Title);
            Assert.Equal("Article 2", resultList[1].Title);
        }

        // Test helper class for HTTP testing
        private class TestHttpMessageHandler : HttpMessageHandler
        {
            private string? _responseContent;
            private Exception? _exception;

            public void SetResponse(string content)
            {
                _responseContent = content;
                _exception = null;
            }

            public void SetException(Exception exception)
            {
                _exception = exception;
                _responseContent = null;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (_exception != null)
                {
                    throw _exception;
                }

                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseContent ?? "{}")
                };

                return Task.FromResult(response);
            }
        }
    }
} 