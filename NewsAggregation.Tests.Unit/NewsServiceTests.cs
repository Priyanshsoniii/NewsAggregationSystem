//using Xunit;
//using Moq;
//using NewsAggregation.Server.Services;
//using NewsAggregation.Server.Services.Interfaces;
//using NewsAggregation.Server.Repository.Interfaces;
//using NewsAggregation.Server.Models.Entities;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace NewsAggregation.Tests.Unit
//{
//    public class NewsServiceTests
//    {
//        private readonly Mock<INewsRepository> _mockNewsRepository;
//        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
//        private readonly Mock<IReportRepository> _mockReportRepository;
//        private readonly Mock<INotificationService> _mockNotificationService;
//        private readonly Mock<IFilteredKeywordRepository> _mockFilteredKeywordRepository;
//        private readonly Mock<IUserArticleLikeRepository> _mockUserArticleLikeRepository;
//        private readonly Mock<IUserArticleReadRepository> _mockUserArticleReadRepository;
//        private readonly Mock<IUserService> _mockUserService;
//        private readonly NewsService _newsService;

//        public NewsServiceTests()
//        {
//            _mockNewsRepository = new Mock<INewsRepository>();
//            _mockCategoryRepository = new Mock<ICategoryRepository>();
//            _mockReportRepository = new Mock<IReportRepository>();
//            _mockNotificationService = new Mock<INotificationService>();
//            _mockFilteredKeywordRepository = new Mock<IFilteredKeywordRepository>();
//            _mockUserArticleLikeRepository = new Mock<IUserArticleLikeRepository>();
//            _mockUserArticleReadRepository = new Mock<IUserArticleReadRepository>();
//            _mockUserService = new Mock<IUserService>();

//            _newsService = new NewsService(
//                _mockNewsRepository.Object,
//                _mockCategoryRepository.Object,
//                _mockReportRepository.Object,
//                _mockNotificationService.Object,
//                _mockFilteredKeywordRepository.Object,
//                _mockUserArticleLikeRepository.Object,
//                _mockUserArticleReadRepository.Object,
//                _mockUserService.Object
//            );
//        }

//        [Fact]
//        public async Task GetHeadlinesAsync_ShouldReturnFilteredArticles()
//        {
//            // Arrange
//            var articles = new List<NewsArticle>
//            {
//                new NewsArticle { Id = 1, Title = "Test Article 1", IsHidden = false, CategoryId = 1 },
//                new NewsArticle { Id = 2, Title = "Test Article 2", IsHidden = true, CategoryId = 1 },
//                new NewsArticle { Id = 3, Title = "Test Article 3", IsHidden = false, CategoryId = 1 }
//            };

//            var categories = new List<Category>
//            {
//                new Category { Id = 1, Name = "Business", IsHidden = false }
//            };

//            var filteredKeywords = new List<FilteredKeyword>();

//            _mockNewsRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(articles);
//            _mockCategoryRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
//            _mockFilteredKeywordRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(filteredKeywords);

//            // Act
//            var result = await _newsService.GetHeadlinesAsync();

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(2, result.Count()); // Only non-hidden articles
//            Assert.DoesNotContain(result, a => a.IsHidden);
//        }

//        [Fact]
//        public async Task GetRecommendedArticlesAsync_ShouldReturnPersonalizedArticles()
//        {
//            // Arrange
//            var userId = 1;
//            var articles = new List<NewsArticle>
//            {
//                new NewsArticle { Id = 1, Title = "Liked Article", IsHidden = false, CategoryId = 1, PublishedAt = DateTime.UtcNow },
//                new NewsArticle { Id = 2, Title = "Saved Article", IsHidden = false, CategoryId = 1, PublishedAt = DateTime.UtcNow },
//                new NewsArticle { Id = 3, Title = "New Article", IsHidden = false, CategoryId = 1, PublishedAt = DateTime.UtcNow }
//            };

//            var likedArticles = new List<UserArticleLike> { new UserArticleLike { NewsArticleId = 1 } };
//            var savedArticles = new List<NewsArticle> { new NewsArticle { Id = 2 } };
//            var readArticles = new List<UserArticleRead>();
//            var categories = new List<Category> { new Category { Id = 1, IsHidden = false } };
//            var filteredKeywords = new List<FilteredKeyword>();
//            var notificationSettings = new List<UserNotificationSetting>();

//            _mockNewsRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(articles);
//            _mockNewsRepository.Setup(x => x.GetSavedArticlesByUserAsync(userId)).ReturnsAsync(savedArticles);
//            _mockUserArticleLikeRepository.Setup(x => x.GetByUserAsync(userId)).ReturnsAsync(likedArticles);
//            _mockUserArticleReadRepository.Setup(x => x.GetByUserAsync(userId)).ReturnsAsync(readArticles);
//            _mockCategoryRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
//            _mockFilteredKeywordRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(filteredKeywords);
//            _mockNotificationService.Setup(x => x.GetUserNotificationSettingsAsync(userId)).ReturnsAsync(notificationSettings);

//            // Act
//            var result = await _newsService.GetRecommendedArticlesAsync(userId, 10);

//            // Assert
//            Assert.NotNull(result);
//            Assert.True(result.Any());
//        }

//        [Fact]
//        public async Task ReportArticleAsync_ShouldCreateReportAndNotifyAdmin()
//        {
//            // Arrange
//            var userId = 1;
//            var articleId = 1;
//            var reason = "Inappropriate content";
//            var article = new NewsArticle { Id = articleId, Title = "Test Article", IsHidden = false };

//            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);
//            _mockReportRepository.Setup(x => x.GetByUserAndArticleAsync(userId, articleId)).ReturnsAsync((Report?)null);
//            _mockReportRepository.Setup(x => x.CreateAsync(It.IsAny<Report>())).ReturnsAsync(new Report());
//            _mockReportRepository.Setup(x => x.GetReportCountForArticleAsync(articleId)).ReturnsAsync(1);
//            _mockNotificationService.Setup(x => x.SendEmailNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(true);

//            // Act
//            var result = await _newsService.ReportArticleAsync(userId, articleId, reason);

//            // Assert
//            Assert.True(result);
//            _mockReportRepository.Verify(x => x.CreateAsync(It.IsAny<Report>()), Times.Once);
//            _mockNotificationService.Verify(x => x.SendEmailNotificationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Once);
//        }

//        [Fact]
//        public async Task ReportArticleAsync_ShouldReturnFalse_WhenArticleAlreadyHidden()
//        {
//            // Arrange
//            var userId = 1;
//            var articleId = 1;
//            var article = new NewsArticle { Id = articleId, Title = "Test Article", IsHidden = true };

//            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);

//            // Act
//            var result = await _newsService.ReportArticleAsync(userId, articleId, "reason");

//            // Assert
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task ReportArticleAsync_ShouldReturnFalse_WhenUserAlreadyReported()
//        {
//            // Arrange
//            var userId = 1;
//            var articleId = 1;
//            var article = new NewsArticle { Id = articleId, Title = "Test Article", IsHidden = false };
//            var existingReport = new Report { UserId = userId, NewsArticleId = articleId };

//            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);
//            _mockReportRepository.Setup(x => x.GetByUserAndArticleAsync(userId, articleId)).ReturnsAsync(existingReport);

//            // Act
//            var result = await _newsService.ReportArticleAsync(userId, articleId, "reason");

//            // Assert
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task LikeArticleAsync_ShouldIncrementLikes()
//        {
//            // Arrange
//            var userId = 1;
//            var articleId = 1;
//            var article = new NewsArticle { Id = articleId, Title = "Test Article", Likes = 5 };

//            _mockNewsRepository.Setup(x => x.GetByIdAsync(articleId)).ReturnsAsync(article);
//            _mockUserArticleLikeRepository.Setup(x => x.GetByUserAndArticleAsync(userId, articleId)).ReturnsAsync((UserArticleLike?)null);
//            _mockUserArticleLikeRepository.Setup(x => x.CreateAsync(It.IsAny<UserArticleLike>())).ReturnsAsync(new UserArticleLike());
//            _mockNewsRepository.Setup(x => x.UpdateAsync(It.IsAny<NewsArticle>())).Returns(Task.CompletedTask);

//            // Act
//            var result = await _newsService.LikeArticleAsync(userId, articleId);

//            // Assert
//            Assert.True(result);
//            _mockNewsRepository.Verify(x => x.UpdateAsync(It.Is<NewsArticle>(a => a.Likes == 6)), Times.Once);
//        }

//        [Fact]
//        public async Task SaveArticleAsync_ShouldCallRepository()
//        {
//            // Arrange
//            var userId = 1;
//            var articleId = 1;

//            _mockNewsRepository.Setup(x => x.SaveArticleForUserAsync(userId, articleId)).ReturnsAsync(true);

//            // Act
//            var result = await _newsService.SaveArticleAsync(userId, articleId);

//            // Assert
//            Assert.True(result);
//            _mockNewsRepository.Verify(x => x.SaveArticleForUserAsync(userId, articleId), Times.Once);
//        }

//        [Fact]
//        public async Task GetUserKeywordsAsync_ShouldReturnUserKeywords()
//        {
//            // Arrange
//            var userId = 1;
//            var notificationSettings = new List<UserNotificationSetting>
//            {
//                new UserNotificationSetting { Keywords = "[\"tech\", \"AI\"]", IsEnabled = true },
//                new UserNotificationSetting { Keywords = "[\"business\", \"finance\"]", IsEnabled = true }
//            };

//            _mockNotificationService.Setup(x => x.GetUserNotificationSettingsAsync(userId)).ReturnsAsync(notificationSettings);

//            // Act
//            var result = await _newsService.GetUserKeywordsAsync(userId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Contains("tech", result);
//            Assert.Contains("AI", result);
//            Assert.Contains("business", result);
//            Assert.Contains("finance", result);
//        }

//        [Fact]
//        public async Task GetPersonalizedArticlesByCategoryAsync_ShouldReturnScoredArticles()
//        {
//            // Arrange
//            var userId = 1;
//            var categoryId = 1;
//            var articles = new List<NewsArticle>
//            {
//                new NewsArticle { Id = 1, Title = "Article 1", CategoryId = categoryId, IsHidden = false, PublishedAt = DateTime.UtcNow },
//                new NewsArticle { Id = 2, Title = "Article 2", CategoryId = categoryId, IsHidden = false, PublishedAt = DateTime.UtcNow }
//            };

//            var categories = new List<Category> { new Category { Id = categoryId, IsHidden = false } };
//            var filteredKeywords = new List<FilteredKeyword>();
//            var notificationSettings = new List<UserNotificationSetting>();

//            _mockNewsRepository.Setup(x => x.GetByCategoryAsync(categoryId)).ReturnsAsync(articles);
//            _mockCategoryRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
//            _mockFilteredKeywordRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(filteredKeywords);
//            _mockUserArticleLikeRepository.Setup(x => x.GetByUserAsync(userId)).ReturnsAsync(new List<UserArticleLike>());
//            _mockUserArticleReadRepository.Setup(x => x.GetByUserAsync(userId)).ReturnsAsync(new List<UserArticleRead>());
//            _mockNewsRepository.Setup(x => x.GetSavedArticlesByUserAsync(userId)).ReturnsAsync(new List<NewsArticle>());
//            _mockNotificationService.Setup(x => x.GetUserNotificationSettingsAsync(userId)).ReturnsAsync(notificationSettings);

//            // Act
//            var result = await _newsService.GetPersonalizedArticlesByCategoryAsync(userId, categoryId, 10);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal(2, result.Count());
//        }
//    }
//} 