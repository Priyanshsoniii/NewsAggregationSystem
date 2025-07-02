using Microsoft.EntityFrameworkCore;
using NewsAggregation.Server.Data;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;

namespace NewsAggregation.Server.Repository
{
    public class UserArticleLikeRepository : IUserArticleLikeRepository
    {
        private readonly NewsAggregationContext _context;

        public UserArticleLikeRepository(NewsAggregationContext context)
        {
            _context = context;
        }

        public async Task<UserArticleLike?> GetByUserAndArticleAsync(int userId, int articleId)
        {
            return await _context.UserArticleLikes.FirstOrDefaultAsync(l => l.UserId == userId && l.NewsArticleId == articleId);
        }

        public async Task<IEnumerable<UserArticleLike>> GetByUserAsync(int userId)
        {
            return await _context.UserArticleLikes.Where(l => l.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<UserArticleLike>> GetByArticleAsync(int articleId)
        {
            return await _context.UserArticleLikes.Where(l => l.NewsArticleId == articleId).ToListAsync();
        }

        public async Task<UserArticleLike> CreateAsync(UserArticleLike like)
        {
            _context.UserArticleLikes.Add(like);
            await _context.SaveChangesAsync();
            return like;
        }

        public async Task<bool> DeleteAsync(int userId, int articleId)
        {
            var like = await _context.UserArticleLikes.FirstOrDefaultAsync(l => l.UserId == userId && l.NewsArticleId == articleId);
            if (like == null) return false;
            _context.UserArticleLikes.Remove(like);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 