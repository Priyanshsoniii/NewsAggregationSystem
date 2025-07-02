using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Repository.Interfaces
{
    public interface IUserArticleReadRepository
    {
        Task<UserArticleRead?> GetByUserAndArticleAsync(int userId, int articleId);
        Task<IEnumerable<UserArticleRead>> GetByUserAsync(int userId);
        Task<IEnumerable<UserArticleRead>> GetByArticleAsync(int articleId);
        Task<UserArticleRead> CreateAsync(UserArticleRead read);
    }
} 