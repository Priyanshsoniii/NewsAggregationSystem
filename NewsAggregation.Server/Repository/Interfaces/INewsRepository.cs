using NewsAggregation.Server.Data.Models.Entities;

namespace NewsAggregation.Server.Repository.Interfaces
{
    public interface INewsRepository
    {
        Task<NewsArticle?> GetByIdAsync(int id);
        Task<NewsArticle?> GetByUrlAsync(string url);
        Task<IEnumerable<NewsArticle>> GetAllAsync();
        Task<IEnumerable<NewsArticle>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<NewsArticle>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<NewsArticle>> SearchAsync(string query, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<NewsArticle>> GetTodayHeadlinesAsync();
        Task<NewsArticle> CreateAsync(NewsArticle article);
        Task<NewsArticle> UpdateAsync(NewsArticle article);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<NewsArticle>> GetSavedArticlesByUserAsync(int userId);
        Task<bool> SaveArticleForUserAsync(int userId, int articleId);
        Task<bool> RemoveSavedArticleAsync(int userId, int articleId);
        Task<bool> ArticleExistsAsync(string url);
    }
}