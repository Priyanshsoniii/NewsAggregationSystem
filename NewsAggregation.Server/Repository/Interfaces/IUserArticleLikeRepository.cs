using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Repository.Interfaces
{
    public interface IUserArticleLikeRepository
    {
        Task<UserArticleLike?> GetByUserAndArticleAsync(int userId, int articleId);
        Task<IEnumerable<UserArticleLike>> GetByUserAsync(int userId);
        Task<IEnumerable<UserArticleLike>> GetByArticleAsync(int articleId);
        Task<UserArticleLike> CreateAsync(UserArticleLike like);
        Task<bool> DeleteAsync(int userId, int articleId);
    }
} 