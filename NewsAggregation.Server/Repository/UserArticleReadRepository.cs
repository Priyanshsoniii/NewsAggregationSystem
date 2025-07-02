using Microsoft.EntityFrameworkCore;
using NewsAggregation.Server.Data;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;

namespace NewsAggregation.Server.Repository
{
    public class UserArticleReadRepository : IUserArticleReadRepository
    {
        private readonly NewsAggregationContext _context;

        public UserArticleReadRepository(NewsAggregationContext context)
        {
            _context = context;
        }

        public async Task<UserArticleRead?> GetByUserAndArticleAsync(int userId, int articleId)
        {
            return await _context.UserArticleReads.FirstOrDefaultAsync(r => r.UserId == userId && r.NewsArticleId == articleId);
        }

        public async Task<IEnumerable<UserArticleRead>> GetByUserAsync(int userId)
        {
            return await _context.UserArticleReads.Where(r => r.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<UserArticleRead>> GetByArticleAsync(int articleId)
        {
            return await _context.UserArticleReads.Where(r => r.NewsArticleId == articleId).ToListAsync();
        }

        public async Task<UserArticleRead> CreateAsync(UserArticleRead read)
        {
            _context.UserArticleReads.Add(read);
            await _context.SaveChangesAsync();
            return read;
        }
    }
} 