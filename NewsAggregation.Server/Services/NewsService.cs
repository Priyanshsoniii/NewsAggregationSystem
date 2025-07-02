using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReportRepository _reportRepository;
        private readonly INotificationService _notificationService;
        private readonly IFilteredKeywordRepository _filteredKeywordRepository;
        private readonly IUserArticleLikeRepository _userArticleLikeRepository;
        private readonly IUserArticleReadRepository _userArticleReadRepository;

        public NewsService(
            INewsRepository newsRepository,
            ICategoryRepository categoryRepository,
            IReportRepository reportRepository,
            INotificationService notificationService,
            IFilteredKeywordRepository filteredKeywordRepository,
            IUserArticleLikeRepository userArticleLikeRepository,
            IUserArticleReadRepository userArticleReadRepository)
        {
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
            _reportRepository = reportRepository;
            _notificationService = notificationService;
            _filteredKeywordRepository = filteredKeywordRepository;
            _userArticleLikeRepository = userArticleLikeRepository;
            _userArticleReadRepository = userArticleReadRepository;
        }

        private async Task<IEnumerable<NewsArticle>> FilterArticlesAsync(IEnumerable<NewsArticle> articles)
        {
            var filteredKeywords = (await _filteredKeywordRepository.GetAllAsync()).Select(k => k.Keyword.ToLower()).ToList();
            return articles.Where(a =>
                !a.IsHidden &&
                (a.Category == null || !a.Category.IsHidden) &&
                !filteredKeywords.Any(k => a.Title.ToLower().Contains(k) || (a.Description != null && a.Description.ToLower().Contains(k)))
            );
        }

        public async Task<IEnumerable<NewsArticle>> GetHeadlinesAsync()
        {
            var articles = await _newsRepository.GetAllAsync();
            return await FilterArticlesAsync(articles);
        }

        public async Task<IEnumerable<NewsArticle>> GetHeadlinesByCategoryAsync(int categoryId)
        {
            var articles = await _newsRepository.GetByCategoryAsync(categoryId);
            return await FilterArticlesAsync(articles);
        }

        public async Task<IEnumerable<NewsArticle>> GetTodayHeadlinesAsync()
        {
            var articles = await _newsRepository.GetTodayHeadlinesAsync();
            return await FilterArticlesAsync(articles);
        }

        public async Task<IEnumerable<NewsArticle>> GetHeadlinesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var articles = await _newsRepository.GetByDateRangeAsync(startDate, endDate);
            return await FilterArticlesAsync(articles);
        }

        public async Task<IEnumerable<NewsArticle>> SearchNewsAsync(string query, DateTime? startDate = null, DateTime? endDate = null)
        {
            var articles = await _newsRepository.SearchAsync(query, startDate, endDate);
            return await FilterArticlesAsync(articles);
        }

        public async Task<NewsArticle?> GetArticleByIdAsync(int id)
        {
            var article = await _newsRepository.GetByIdAsync(id);
            if (article == null || article.IsHidden || (article.Category != null && article.Category.IsHidden))
                return null;
            var filteredKeywords = (await _filteredKeywordRepository.GetAllAsync()).Select(k => k.Keyword.ToLower()).ToList();
            if (filteredKeywords.Any(k => article.Title.ToLower().Contains(k) || (article.Description != null && article.Description.ToLower().Contains(k))))
                return null;
            return article;
        }

        public async Task<IEnumerable<NewsArticle>> GetSavedArticlesAsync(int userId)
        {
            var articles = await _newsRepository.GetSavedArticlesByUserAsync(userId);
            return await FilterArticlesAsync(articles);
        }

        public async Task<bool> SaveArticleAsync(int userId, int articleId)
        {
            return await _newsRepository.SaveArticleForUserAsync(userId, articleId);
        }

        public async Task<bool> RemoveSavedArticleAsync(int userId, int articleId)
        {
            return await _newsRepository.RemoveSavedArticleAsync(userId, articleId);
        }

        public async Task<bool> LikeArticleAsync(int userId, int articleId)
        {
            var article = await _newsRepository.GetByIdAsync(articleId);
            if (article == null) return false;
            var existingLike = await _userArticleLikeRepository.GetByUserAndArticleAsync(userId, articleId);
            if (existingLike != null) return false;
            await _userArticleLikeRepository.CreateAsync(new UserArticleLike { UserId = userId, NewsArticleId = articleId, LikedAt = DateTime.UtcNow });
            article.Likes++;
            await _newsRepository.UpdateAsync(article);
            return true;
        }

        public async Task<bool> UnlikeArticleAsync(int userId, int articleId)
        {
            var article = await _newsRepository.GetByIdAsync(articleId);
            if (article == null) return false;
            var deleted = await _userArticleLikeRepository.DeleteAsync(userId, articleId);
            if (!deleted) return false;
            if (article.Likes > 0) article.Likes--;
            await _newsRepository.UpdateAsync(article);
            return true;
        }

        public async Task<bool> MarkArticleAsReadAsync(int userId, int articleId)
        {
            var article = await _newsRepository.GetByIdAsync(articleId);
            if (article == null) return false;
            var existingRead = await _userArticleReadRepository.GetByUserAndArticleAsync(userId, articleId);
            if (existingRead != null) return false;
            await _userArticleReadRepository.CreateAsync(new UserArticleRead { UserId = userId, NewsArticleId = articleId, ReadAt = DateTime.UtcNow });
            return true;
        }

        public async Task<IEnumerable<NewsArticle>> GetRecommendedArticlesAsync(int userId, int count = 10)
        {
            // Simple recommendation: prioritize articles liked, saved, or matching notification keywords
            var liked = (await _userArticleLikeRepository.GetByUserAsync(userId)).Select(l => l.NewsArticleId).ToHashSet();
            var read = (await _userArticleReadRepository.GetByUserAsync(userId)).Select(r => r.NewsArticleId).ToHashSet();
            var saved = (await _newsRepository.GetSavedArticlesByUserAsync(userId)).Select(a => a.Id).ToHashSet();
            var allArticles = await _newsRepository.GetAllAsync();
            var filtered = await FilterArticlesAsync(allArticles);
            // TODO: Add notification keywords logic
            var recommended = filtered
                .Where(a => saved.Contains(a.Id) || liked.Contains(a.Id))
                .OrderByDescending(a => a.CreatedAt)
                .Take(count)
                .ToList();
            // Fill up with unread articles if not enough
            if (recommended.Count < count)
            {
                var unread = filtered.Where(a => !read.Contains(a.Id) && !recommended.Contains(a)).OrderByDescending(a => a.CreatedAt).Take(count - recommended.Count);
                recommended.AddRange(unread);
            }
            return recommended;
        }

        public async Task<bool> ReportArticleAsync(int userId, int articleId, string? reason, int reportThreshold = 3)
        {
            var article = await _newsRepository.GetByIdAsync(articleId);
            if (article == null || article.IsHidden)
                return false;

            // Prevent duplicate reports by the same user
            var existingReport = await _reportRepository.GetByUserAndArticleAsync(userId, articleId);
            if (existingReport != null)
                return false;

            var report = new Report
            {
                UserId = userId,
                NewsArticleId = articleId,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            };
            await _reportRepository.CreateAsync(report);

            // Update report count
            article.ReportCount = await _reportRepository.GetReportCountForArticleAsync(articleId);
            if (article.ReportCount >= reportThreshold)
            {
                article.IsHidden = true;
                // Optionally, notify admin here
                await _notificationService.SendEmailNotificationAsync(1, "Article Hidden Due to Reports", $"Article '{article.Title}' was automatically hidden after {article.ReportCount} reports.", article.Id);
            }
            await _newsRepository.UpdateAsync(article);

            // Notify admin of new report
            await _notificationService.SendEmailNotificationAsync(1, "Article Reported", $"Article '{article.Title}' was reported by a user. Reason: {reason}", article.Id);

            return true;
        }

        public async Task ImportArticlesAsync(List<NewsArticle> articlesFromApi)
        {
            foreach (var article in articlesFromApi)
            {
                if (await _newsRepository.ArticleExistsAsync(article.Url))
                    continue;

                if (article.CategoryId == 0)
                {
                    article.CategoryId = await GetCategoryIdForArticleAsync(article);
                }

                if (article.CreatedAt == default)
                    article.CreatedAt = DateTime.UtcNow;

                article.Likes = article.Likes;
                article.Dislikes = article.Dislikes;

                await _newsRepository.CreateAsync(article);
            }
        }

        private async Task<int> GetCategoryIdForArticleAsync(NewsArticle article)
        {
            var categories = await _categoryRepository.GetAllAsync();
            var title = article.Title?.ToLower() ?? string.Empty;
            var description = article.Description?.ToLower() ?? string.Empty;
            foreach (var category in categories)
            {
                if (!string.IsNullOrWhiteSpace(category.Keywords))
                {
                    var keywords = category.Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var keyword in keywords)
                    {
                        if (title.Contains(keyword.ToLower()) || description.Contains(keyword.ToLower()))
                        {
                            return category.Id;
                        }
                    }
                }
            }
            return categories.First().Id; // Default to first category if no match
        }
    }
}