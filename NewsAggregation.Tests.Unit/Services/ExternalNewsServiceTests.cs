using Moq;
using Microsoft.Extensions.Logging;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Services;
using NewsAggregation.Server.Services.Interfaces;
using NewsAggregation.Server.Exceptions;
using Xunit;

namespace NewsAggregation.Tests.Unit.Services
{
    public class ExternalNewsServiceTests
    {
        private readonly Mock<ILogger<ExternalNewsService>> _mockLogger;
        private readonly Mock<ITheNewsApiClient> _mockTheNewsApiClient;
        private readonly Mock<INewsApiClient> _mockNewsApiClient;
        private readonly Mock<IBbcRssClient> _mockBbcRssClient;
        private readonly ExternalNewsService _externalNewsService;

        public ExternalNewsServiceTests()
        {
            _mockLogger = new Mock<ILogger<ExternalNewsService>>();
            _mockTheNewsApiClient = new Mock<ITheNewsApiClient>();
            _mockNewsApiClient = new Mock<INewsApiClient>();
            _mockBbcRssClient = new Mock<IBbcRssClient>();
            _externalNewsService = new ExternalNewsService(_mockLogger.Object, _mockTheNewsApiClient.Object, _mockNewsApiClient.Object, _mockBbcRssClient.Object);
        }

        [Fact]
        public async Task FetchLatestNewsAsync_WithAllSourcesWorking_ReturnsCombinedArticles()
        {
            // Arrange
            var newsApiArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 1, Title = "News API Article 1", PublishedAt = DateTime.UtcNow.AddHours(-1) },
                new NewsArticle { Id = 2, Title = "News API Article 2", PublishedAt = DateTime.UtcNow.AddHours(-2) }
            };

            var theNewsApiArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 3, Title = "The News API Article 1", PublishedAt = DateTime.UtcNow.AddHours(-3) },
                new NewsArticle { Id = 4, Title = "The News API Article 2", PublishedAt = DateTime.UtcNow.AddHours(-4) }
            };

            var bbcRssArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 5, Title = "BBC RSS Article 1", PublishedAt = DateTime.UtcNow.AddHours(-5) },
                new NewsArticle { Id = 6, Title = "BBC RSS Article 2", PublishedAt = DateTime.UtcNow.AddHours(-6) }
            };

            _mockNewsApiClient.Setup(x => x.FetchTopHeadlinesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsApiArticles);
            _mockTheNewsApiClient.Setup(x => x.FetchTopNewsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(theNewsApiArticles);
            _mockBbcRssClient.Setup(x => x.FetchRssArticlesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(bbcRssArticles);

            // Act
            var result = await _externalNewsService.FetchLatestNewsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(6, resultList.Count);
            Assert.Contains(resultList, a => a.Title == "News API Article 1");
            Assert.Contains(resultList, a => a.Title == "The News API Article 1");
            Assert.Contains(resultList, a => a.Title == "BBC RSS Article 1");
        }

        [Fact]
        public async Task FetchLatestNewsAsync_WithDuplicateTitles_ReturnsDeduplicatedArticles()
        {
            // Arrange
            var newsApiArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 1, Title = "Same Title", PublishedAt = DateTime.UtcNow.AddHours(-1) },
                new NewsArticle { Id = 2, Title = "Unique Article 1", PublishedAt = DateTime.UtcNow.AddHours(-2) }
            };

            var theNewsApiArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 3, Title = "Same Title", PublishedAt = DateTime.UtcNow.AddHours(-3) },
                new NewsArticle { Id = 4, Title = "Unique Article 2", PublishedAt = DateTime.UtcNow.AddHours(-4) }
            };

            var bbcRssArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 5, Title = "Same Title", PublishedAt = DateTime.UtcNow.AddHours(-5) },
                new NewsArticle { Id = 6, Title = "Unique Article 3", PublishedAt = DateTime.UtcNow.AddHours(-6) }
            };

            _mockNewsApiClient.Setup(x => x.FetchTopHeadlinesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsApiArticles);
            _mockTheNewsApiClient.Setup(x => x.FetchTopNewsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(theNewsApiArticles);
            _mockBbcRssClient.Setup(x => x.FetchRssArticlesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(bbcRssArticles);

            // Act
            var result = await _externalNewsService.FetchLatestNewsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(4, resultList.Count); // 3 unique articles + 1 duplicate removed
            Assert.Single(resultList.Where(a => a.Title == "Same Title"));
            Assert.Contains(resultList, a => a.Title == "Unique Article 1");
            Assert.Contains(resultList, a => a.Title == "Unique Article 2");
            Assert.Contains(resultList, a => a.Title == "Unique Article 3");
        }

        [Fact]
        public async Task FetchLatestNewsAsync_WithNewsApiException_ContinuesWithOtherSources()
        {
            // Arrange
            var theNewsApiArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 1, Title = "The News API Article", PublishedAt = DateTime.UtcNow.AddHours(-1) }
            };

            var bbcRssArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 2, Title = "BBC RSS Article", PublishedAt = DateTime.UtcNow.AddHours(-2) }
            };

            _mockNewsApiClient.Setup(x => x.FetchTopHeadlinesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NewsApiException("API Error"));
            _mockTheNewsApiClient.Setup(x => x.FetchTopNewsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(theNewsApiArticles);
            _mockBbcRssClient.Setup(x => x.FetchRssArticlesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(bbcRssArticles);

            // Act
            var result = await _externalNewsService.FetchLatestNewsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, a => a.Title == "The News API Article");
            Assert.Contains(resultList, a => a.Title == "BBC RSS Article");
        }

        [Fact]
        public async Task FetchLatestNewsAsync_WithTheNewsApiException_ContinuesWithOtherSources()
        {
            // Arrange
            var newsApiArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 1, Title = "News API Article", PublishedAt = DateTime.UtcNow.AddHours(-1) }
            };

            var bbcRssArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 2, Title = "BBC RSS Article", PublishedAt = DateTime.UtcNow.AddHours(-2) }
            };

            _mockNewsApiClient.Setup(x => x.FetchTopHeadlinesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsApiArticles);
            _mockTheNewsApiClient.Setup(x => x.FetchTopNewsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TheNewsApiException("API Error"));
            _mockBbcRssClient.Setup(x => x.FetchRssArticlesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(bbcRssArticles);

            // Act
            var result = await _externalNewsService.FetchLatestNewsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, a => a.Title == "News API Article");
            Assert.Contains(resultList, a => a.Title == "BBC RSS Article");
        }

        [Fact]
        public async Task FetchLatestNewsAsync_WithBbcRssException_ContinuesWithOtherSources()
        {
            // Arrange
            var newsApiArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 1, Title = "News API Article", PublishedAt = DateTime.UtcNow.AddHours(-1) }
            };

            var theNewsApiArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 2, Title = "The News API Article", PublishedAt = DateTime.UtcNow.AddHours(-2) }
            };

            _mockNewsApiClient.Setup(x => x.FetchTopHeadlinesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsApiArticles);
            _mockTheNewsApiClient.Setup(x => x.FetchTopNewsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(theNewsApiArticles);
            _mockBbcRssClient.Setup(x => x.FetchRssArticlesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BbcRssException("RSS Error"));

            // Act
            var result = await _externalNewsService.FetchLatestNewsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, a => a.Title == "News API Article");
            Assert.Contains(resultList, a => a.Title == "The News API Article");
        }

        [Fact]
        public async Task FetchLatestNewsAsync_WithAllSourcesFailing_ReturnsEmptyList()
        {
            // Arrange
            _mockNewsApiClient.Setup(x => x.FetchTopHeadlinesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NewsApiException("API Error"));
            _mockTheNewsApiClient.Setup(x => x.FetchTopNewsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TheNewsApiException("API Error"));
            _mockBbcRssClient.Setup(x => x.FetchRssArticlesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BbcRssException("RSS Error"));

            // Act
            var result = await _externalNewsService.FetchLatestNewsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Empty(resultList);
        }

        [Fact]
        public async Task FetchLatestNewsAsync_WithConfigurationExceptions_ContinuesWithOtherSources()
        {
            // Arrange
            var newsApiArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 1, Title = "News API Article", PublishedAt = DateTime.UtcNow.AddHours(-1) }
            };

            _mockNewsApiClient.Setup(x => x.FetchTopHeadlinesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NewsApiConfigurationException("Config Error"));
            _mockTheNewsApiClient.Setup(x => x.FetchTopNewsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TheNewsApiConfigurationException("Config Error"));
            _mockBbcRssClient.Setup(x => x.FetchRssArticlesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsApiArticles);

            // Act
            var result = await _externalNewsService.FetchLatestNewsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Contains(resultList, a => a.Title == "News API Article");
        }

        [Fact]
        public async Task FetchLatestNewsAsync_WithUnexpectedExceptions_ContinuesWithOtherSources()
        {
            // Arrange
            var newsApiArticles = new List<NewsArticle>
            {
                new NewsArticle { Id = 1, Title = "News API Article", PublishedAt = DateTime.UtcNow.AddHours(-1) }
            };

            _mockNewsApiClient.Setup(x => x.FetchTopHeadlinesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected Error"));
            _mockTheNewsApiClient.Setup(x => x.FetchTopNewsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected Error"));
            _mockBbcRssClient.Setup(x => x.FetchRssArticlesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(newsApiArticles);

            // Act
            var result = await _externalNewsService.FetchLatestNewsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Contains(resultList, a => a.Title == "News API Article");
        }

        [Fact]
        public async Task FetchLatestNewsAsync_WithCancellationToken_RespectsCancellation()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act
            var result = await _externalNewsService.FetchLatestNewsAsync(cancellationTokenSource.Token);

            // Assert
            var resultList = result.ToList();
            Assert.Empty(resultList);
        }

        [Fact]
        public async Task FetchLatestNewsAsync_WithEmptyResults_ReturnsEmptyList()
        {
            // Arrange
            _mockNewsApiClient.Setup(x => x.FetchTopHeadlinesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<NewsArticle>());
            _mockTheNewsApiClient.Setup(x => x.FetchTopNewsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<NewsArticle>());
            _mockBbcRssClient.Setup(x => x.FetchRssArticlesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<NewsArticle>());

            // Act
            var result = await _externalNewsService.FetchLatestNewsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Empty(resultList);
        }

        [Fact]
        public async Task FetchLatestNewsAsync_WithNullResults_HandlesGracefully()
        {
            // Arrange
            _mockNewsApiClient.Setup(x => x.FetchTopHeadlinesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<NewsArticle>)null!);
            _mockTheNewsApiClient.Setup(x => x.FetchTopNewsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<NewsArticle>)null!);
            _mockBbcRssClient.Setup(x => x.FetchRssArticlesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<NewsArticle>)null!);

            // Act
            var result = await _externalNewsService.FetchLatestNewsAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Empty(resultList);
        }
    }
} 