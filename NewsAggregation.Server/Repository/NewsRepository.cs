using Microsoft.EntityFrameworkCore;
using NewsAggregation.Server.Data;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;

namespace NewsAggregation.Server.Repository
{
    public class NewsRepository : INewsRepository
    {
        private readonly NewsAggregationContext _context;

        public NewsRepository(NewsAggregationContext context)
        {
            _context = context;
        }

        public async Task<NewsArticle?> GetByIdAsync(int id)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<NewsArticle?> GetByUrlAsync(string url)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .FirstOrDefaultAsync(n => n.Url == url);
        }

        public async Task<IEnumerable<NewsArticle>> GetAllAsync()
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetByCategoryAsync(int categoryId)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Where(n => n.CategoryId == categoryId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Where(n => n.CreatedAt >= startDate && n.CreatedAt <= endDate)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> SearchAsync(string query, DateTime? startDate = null, DateTime? endDate = null)
        {
            var articlesQuery = _context.NewsArticles
                .Include(n => n.Category)
                .Where(n => n.Title.Contains(query) ||
                           (n.Description != null && n.Description.Contains(query)));

            if (startDate.HasValue)
                articlesQuery = articlesQuery.Where(n => n.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                articlesQuery = articlesQuery.Where(n => n.CreatedAt <= endDate.Value);

            return await articlesQuery
                .OrderByDescending(n => n.Likes - n.Dislikes)
                .ThenByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetTodayHeadlinesAsync()
        {
            var today = DateTime.Today;

            return await _context.NewsArticles
                .Include(n => n.Category)
                .Where(n => n.CreatedAt >= today)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<NewsArticle> CreateAsync(NewsArticle article)
        {
            _context.NewsArticles.Add(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<NewsArticle> UpdateAsync(NewsArticle article)
        {
            _context.NewsArticles.Update(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var article = await _context.NewsArticles.FindAsync(id);
            if (article == null) return false;

            _context.NewsArticles.Remove(article);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NewsArticle>> GetSavedArticlesByUserAsync(int userId)
        {
            return await _context.SavedArticles
                .Include(sa => sa.NewsArticle)
                    .ThenInclude(na => na.Category)
                .Where(sa => sa.UserId == userId)
                .Select(sa => sa.NewsArticle)
                .OrderByDescending(na => na.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> SaveArticleForUserAsync(int userId, int articleId)
        {
            var existingSave = await _context.SavedArticles
                .FirstOrDefaultAsync(sa => sa.UserId == userId && sa.NewsArticleId == articleId);

            if (existingSave != null) return false;

            var savedArticle = new SavedArticle
            {
                UserId = userId,
                NewsArticleId = articleId
            };

            _context.SavedArticles.Add(savedArticle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveSavedArticleAsync(int userId, int articleId)
        {
            var savedArticle = await _context.SavedArticles
                .FirstOrDefaultAsync(sa => sa.UserId == userId && sa.NewsArticleId == articleId);

            if (savedArticle == null) return false;

            _context.SavedArticles.Remove(savedArticle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ArticleExistsAsync(string url)
        {
            return await _context.NewsArticles.AnyAsync(n => n.Url == url);
        }
    }
}