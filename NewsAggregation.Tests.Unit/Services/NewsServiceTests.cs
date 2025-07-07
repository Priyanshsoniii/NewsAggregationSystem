using Moq;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services;
using NewsAggregation.Server.Services.Interfaces;
using Xunit;

namespace NewsAggregation.Tests.Unit.Services
{
    public class NewsServiceTests
    {
        private readonly Mock<INewsRepository> _mockNewsRepository;
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IReportRepository> _mockReportRepository;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<IFilteredKeywordRepository> _mockFilteredKeywordRepository;
        private readonly Mock<IUserArticleLikeRepository> _mockUserArticleLikeRepository;
        private readonly Mock<IUserArticleReadRepository> _mockUserArticleReadRepository;
        private readonly Mock<IUserService> _mockUserService;
        private readonly NewsService _newsService;

        public NewsServiceTests()
        {
            _mockNewsRepository = new Mock<INewsRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockReportRepository = new Mock<IReportRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockFilteredKeywordRepository = new Mock<IFilteredKeywordRepository>();
            _mockUserArticleLikeRepository = new Mock<IUserArticleLikeRepository>();
            _mockUserArticleReadRepository = new Mock<IUserArticleReadRepository>();
            _mockUserService = new Mock<IUserService>();

            _newsService = new NewsService(
                _mockNewsRepository.Object,
                _mockCategoryRepository.Object,
                _mockReportRepository.Object,
                _mockNotificationService.Object,
                _mockFilteredKeywordRepository.Object,
                _mockUserArticleLikeRepository.Object,
                _mockUserArticleReadRepository.Object,
                _mockUserService.Object
            );
        }

        [Fact]
        public async Task GetHeadlinesAsync_ReturnsFilteredArticles()
        {
            // Arrange
            var articles = new List<NewsArticle>
            {
                new NewsArticle { Id = 1, Title = "Test Article 1", IsHidden = false },
                new NewsArticle { Id = 2, Title = "Test Article 2", IsHidden = true },
                new NewsArticle { Id = 3, Title = "Test Article 3", IsHidden = false }
            };

            var filteredKeywords = new List<FilteredKeyword>
            {
                new FilteredKeyword { Keyword = "spam" }
            };

            _mockNewsRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(articles);
            _mockFilteredKeywordRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(filteredKeywords);

            // Act
            var result = await _newsService.GetHeadlinesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, a => a.Id == 1);
            Assert.Contains(resultList, a => a.Id == 3);
            Assert.DoesNotContain(resultList, a => a.Id == 2);
        }

        [Fact]
        public async Task GetHeadlinesByCategoryAsync_ReturnsFilteredArticlesForCategory()
        {
            // Arrange
            var categoryId = 1;
            var articles = new List<NewsArticle>
            {
                new NewsArticle { Id = 1, Title = "Test Article 1", CategoryId = categoryId, IsHidden = false },
                new NewsArticle { Id = 2, Title = "Test Article 2", CategoryId = categoryId, IsHidden = false }
            };

            var filteredKeywords = new List<FilteredKeyword>();

            _mockNewsRepository.Setup(x => x.GetByCategoryAsync(categoryId)).ReturnsAsync(articles);
            _mockFilteredKeywordRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(filteredKeywords);

            // Act
            var result = await _newsService.GetHeadlinesByCategoryAsync(categoryId);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, a => Assert.Equal(categoryId, a.CategoryId));
        }

        [Fact]
        public async Task GetArticleByIdAsync_WithValidArticle_ReturnsArticle()
        {
            // Arrange
            var articleId = 1;
            var article = new NewsArticle
            {
                Id = articleId,
                Title = "Test Article",
                IsHidden = false,
                Description = "Test description"
            };

            var filteredKeywords = new List<FilteredKeyword>();

            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);
            _mockFilteredKeywordRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(filteredKeywords);

            // Act
            var result = await _newsService.GetArticleByIdAsync(articleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(articleId, result.Id);
        }

        [Fact]
        public async Task GetArticleByIdAsync_WithHiddenArticle_ReturnsNull()
        {
            // Arrange
            var articleId = 1;
            var article = new NewsArticle
            {
                Id = articleId,
                Title = "Test Article",
                IsHidden = true
            };

            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);

            // Act
            var result = await _newsService.GetArticleByIdAsync(articleId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetArticleByIdAsync_WithFilteredKeyword_ReturnsNull()
        {
            // Arrange
            var articleId = 1;
            var article = new NewsArticle
            {
                Id = articleId,
                Title = "Test Article with spam content",
                IsHidden = false,
                Description = "Test description"
            };

            var filteredKeywords = new List<FilteredKeyword>
            {
                new FilteredKeyword { Keyword = "spam" }
            };

            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);
            _mockFilteredKeywordRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(filteredKeywords);

            // Act
            var result = await _newsService.GetArticleByIdAsync(articleId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveArticleAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var articleId = 1;

            _mockNewsRepository.Setup(x => x.SaveArticleForUserAsync(userId, articleId))
                .ReturnsAsync(true);

            // Act
            var result = await _newsService.SaveArticleAsync(userId, articleId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveSavedArticleAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var articleId = 1;

            _mockNewsRepository.Setup(x => x.RemoveSavedArticleAsync(userId, articleId))
                .ReturnsAsync(true);

            // Act
            var result = await _newsService.RemoveSavedArticleAsync(userId, articleId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task LikeArticleAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var articleId = 1;
            var article = new NewsArticle { Id = articleId, Likes = 5 };

            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);
            _mockUserArticleLikeRepository.Setup(x => x.GetByUserAndArticleAsync(userId, articleId))
                .ReturnsAsync((UserArticleLike?)null);
            _mockUserArticleLikeRepository.Setup(x => x.CreateAsync(It.IsAny<UserArticleLike>()))
                .ReturnsAsync(new UserArticleLike());
            _mockNewsRepository.Setup(x => x.UpdateAsync(It.IsAny<NewsArticle>()))
                .ReturnsAsync(new NewsArticle());

            // Act
            var result = await _newsService.LikeArticleAsync(userId, articleId);

            // Assert
            Assert.True(result);
            _mockUserArticleLikeRepository.Verify(x => x.CreateAsync(It.IsAny<UserArticleLike>()), Times.Once);
            _mockNewsRepository.Verify(x => x.UpdateAsync(It.IsAny<NewsArticle>()), Times.Once);
        }

        [Fact]
        public async Task LikeArticleAsync_WithExistingLike_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var articleId = 1;
            var existingLike = new UserArticleLike { UserId = userId, NewsArticleId = articleId };

            _mockUserArticleLikeRepository.Setup(x => x.GetByUserAndArticleAsync(userId, articleId))
                .ReturnsAsync(existingLike);

            // Act
            var result = await _newsService.LikeArticleAsync(userId, articleId);

            // Assert
            Assert.False(result);
            _mockUserArticleLikeRepository.Verify(x => x.CreateAsync(It.IsAny<UserArticleLike>()), Times.Never);
        }

        [Fact]
        public async Task UnlikeArticleAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var articleId = 1;
            var article = new NewsArticle { Id = articleId, Likes = 5 };

            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);
            _mockUserArticleLikeRepository.Setup(x => x.DeleteAsync(userId, articleId))
                .ReturnsAsync(true);
            _mockNewsRepository.Setup(x => x.UpdateAsync(It.IsAny<NewsArticle>()))
                .ReturnsAsync(new NewsArticle());

            // Act
            var result = await _newsService.UnlikeArticleAsync(userId, articleId);

            // Assert
            Assert.True(result);
            _mockUserArticleLikeRepository.Verify(x => x.DeleteAsync(userId, articleId), Times.Once);
            _mockNewsRepository.Verify(x => x.UpdateAsync(It.IsAny<NewsArticle>()), Times.Once);
        }

        [Fact]
        public async Task MarkArticleAsReadAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var articleId = 1;
            var article = new NewsArticle { Id = articleId };

            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);
            _mockUserArticleReadRepository.Setup(x => x.GetByUserAndArticleAsync(userId, articleId))
                .ReturnsAsync((UserArticleRead?)null);
            _mockUserArticleReadRepository.Setup(x => x.CreateAsync(It.IsAny<UserArticleRead>()))
                .ReturnsAsync(new UserArticleRead());

            // Act
            var result = await _newsService.MarkArticleAsReadAsync(userId, articleId);

            // Assert
            Assert.True(result);
            _mockUserArticleReadRepository.Verify(x => x.CreateAsync(It.IsAny<UserArticleRead>()), Times.Once);
        }

        [Fact]
        public async Task MarkArticleAsReadAsync_WithExistingRead_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var articleId = 1;
            var existingRead = new UserArticleRead { UserId = userId, NewsArticleId = articleId };

            _mockUserArticleReadRepository.Setup(x => x.GetByUserAndArticleAsync(userId, articleId))
                .ReturnsAsync(existingRead);

            // Act
            var result = await _newsService.MarkArticleAsReadAsync(userId, articleId);

            // Assert
            Assert.False(result);
            _mockUserArticleReadRepository.Verify(x => x.CreateAsync(It.IsAny<UserArticleRead>()), Times.Never);
        }

        [Fact]
        public async Task GetRecommendedArticlesAsync_ReturnsScoredArticles()
        {
            // Arrange
            var userId = 1;
            var articles = new List<NewsArticle>
            {
                new NewsArticle 
                { 
                    Id = 1, 
                    Title = "Recent Article", 
                    Description = "Test description",
                    PublishedAt = DateTime.UtcNow.AddHours(-12),
                    Likes = 10,
                    Dislikes = 2
                },
                new NewsArticle 
                { 
                    Id = 2, 
                    Title = "Old Article", 
                    Description = "Test description",
                    PublishedAt = DateTime.UtcNow.AddDays(-5),
                    Likes = 5,
                    Dislikes = 1
                }
            };

            var likedArticles = new List<UserArticleLike>();
            var readArticles = new List<UserArticleRead>();
            var savedArticles = new List<NewsArticle>();
            var filteredKeywords = new List<FilteredKeyword>();
            var notificationSettings = new List<UserNotificationSetting>();

            _mockUserArticleLikeRepository.Setup(x => x.GetByUserAsync(userId))
                .ReturnsAsync(likedArticles);
            _mockUserArticleReadRepository.Setup(x => x.GetByUserAsync(userId))
                .ReturnsAsync(readArticles);
            _mockNewsRepository.Setup(x => x.GetSavedArticlesByUserAsync(userId))
                .ReturnsAsync(savedArticles);
            _mockNewsRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(articles);
            _mockFilteredKeywordRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(filteredKeywords);
            _mockNotificationService.Setup(x => x.GetUserNotificationSettingsAsync(userId))
                .ReturnsAsync(notificationSettings);

            // Act
            var result = await _newsService.GetRecommendedArticlesAsync(userId, 10);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            // Recent article should be recommended first due to recency bonus
            Assert.Equal(1, resultList[0].Id);
        }

        [Fact]
        public async Task SearchNewsAsync_ReturnsFilteredResults()
        {
            // Arrange
            var query = "test";
            var articles = new List<NewsArticle>
            {
                new NewsArticle { Id = 1, Title = "Test Article", IsHidden = false },
                new NewsArticle { Id = 2, Title = "Another Article", IsHidden = false }
            };

            var filteredKeywords = new List<FilteredKeyword>();

            _mockNewsRepository.Setup(x => x.SearchAsync(query, null, null))
                .ReturnsAsync(articles);
            _mockFilteredKeywordRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(filteredKeywords);

            // Act
            var result = await _newsService.SearchNewsAsync(query);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
        }

        [Fact]
        public async Task ReportArticleAsync_WithValidData_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var articleId = 1;
            var reason = "Inappropriate content";
            var article = new NewsArticle { Id = articleId, Title = "Test Article" };

            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);
            _mockReportRepository.Setup(x => x.CreateAsync(It.IsAny<Report>()))
                .ReturnsAsync(new Report());
            _mockReportRepository.Setup(x => x.GetByArticleIdAsync(articleId))
                .ReturnsAsync(new List<Report> { new Report() });

            // Act
            var result = await _newsService.ReportArticleAsync(userId, articleId, reason);

            // Assert
            Assert.True(result);
            _mockReportRepository.Verify(x => x.CreateAsync(It.IsAny<Report>()), Times.Once);
        }

        [Fact]
        public async Task ReportArticleAsync_WhenThresholdReached_HidesArticle()
        {
            // Arrange
            var userId = 1;
            var articleId = 1;
            var reason = "Inappropriate content";
            var article = new NewsArticle { Id = articleId, Title = "Test Article", IsHidden = false };

            var existingReports = new List<Report>
            {
                new Report { Id = 1, NewsArticleId = articleId },
                new Report { Id = 2, NewsArticleId = articleId }
            };

            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);
            _mockReportRepository.Setup(x => x.CreateAsync(It.IsAny<Report>()))
                .ReturnsAsync(new Report());
            _mockReportRepository.Setup(x => x.GetByArticleIdAsync(articleId))
                .ReturnsAsync(existingReports);
            _mockNewsRepository.Setup(x => x.UpdateAsync(It.IsAny<NewsArticle>()))
                .ReturnsAsync(new NewsArticle());

            // Act
            var result = await _newsService.ReportArticleAsync(userId, articleId, reason, 3);

            // Assert
            Assert.True(result);
            _mockNewsRepository.Verify(x => x.UpdateAsync(It.Is<NewsArticle>(a => a.IsHidden)), Times.Once);
        }
    }
} 