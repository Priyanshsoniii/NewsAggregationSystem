using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Services.Interfaces
{
    public interface INewsService
    {
        Task<IEnumerable<NewsArticle>> GetHeadlinesAsync();
        Task<IEnumerable<NewsArticle>> GetHeadlinesByCategoryAsync(int categoryId);
        Task<IEnumerable<NewsArticle>> GetTodayHeadlinesAsync();
        Task<IEnumerable<NewsArticle>> GetHeadlinesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<NewsArticle>> SearchNewsAsync(string query, DateTime? startDate = null, DateTime? endDate = null);
        Task<NewsArticle?> GetArticleByIdAsync(int id);
        Task<IEnumerable<NewsArticle>> GetSavedArticlesAsync(int userId);
        Task<bool> SaveArticleAsync(int userId, int articleId);
        Task<bool> RemoveSavedArticleAsync(int userId, int articleId);
        Task<bool> LikeArticleAsync(int userId, int articleId);
        Task<bool> MarkArticleAsReadAsync(int userId, int articleId);
        Task<IEnumerable<NewsArticle>> GetRecommendedArticlesAsync(int userId, int count = 10);
        Task ImportArticlesAsync(List<NewsArticle> articles);
        Task<bool> ReportArticleAsync(int userId, int articleId, string? reason, int reportThreshold = 3);
        Task<bool> UnlikeArticleAsync(int userId, int articleId);
        Task<int> ReCategorizeAllArticlesAsync();
        Task<int> FixInvalidCategoriesAsync();
        Task<IEnumerable<NewsArticle>> GetLikedArticlesAsync(int userId);
        Task<IEnumerable<NewsArticle>> GetReadArticlesAsync(int userId);
        Task<List<string>> GetUserKeywordsAsync(int userId);
        Task<IEnumerable<NewsArticle>> GetPersonalizedArticlesByCategoryAsync(int userId, int categoryId, int count = 10);
    }
}