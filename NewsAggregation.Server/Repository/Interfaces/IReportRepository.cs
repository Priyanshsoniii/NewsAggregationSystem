using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Repository.Interfaces
{
    public interface IReportRepository
    {
        Task<Report?> GetByIdAsync(int id);
        Task<IEnumerable<Report>> GetByArticleIdAsync(int articleId);
        Task<IEnumerable<Report>> GetByUserIdAsync(int userId);
        Task<Report?> GetByUserAndArticleAsync(int userId, int articleId);
        Task<Report> CreateAsync(Report report);
        Task<bool> DeleteAsync(int id);
        Task<int> GetReportCountForArticleAsync(int articleId);
    }
} 