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
        Task ImportArticlesAsync(List<NewsArticle> articles);
    }
}