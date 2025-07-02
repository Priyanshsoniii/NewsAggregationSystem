using Microsoft.EntityFrameworkCore;
using NewsAggregation.Server.Data;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;

namespace NewsAggregation.Server.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly NewsAggregationContext _context;

        public ReportRepository(NewsAggregationContext context)
        {
            _context = context;
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports.FindAsync(id);
        }

        public async Task<IEnumerable<Report>> GetByArticleIdAsync(int articleId)
        {
            return await _context.Reports.Where(r => r.NewsArticleId == articleId).ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetByUserIdAsync(int userId)
        {
            return await _context.Reports.Where(r => r.UserId == userId).ToListAsync();
        }

        public async Task<Report?> GetByUserAndArticleAsync(int userId, int articleId)
        {
            return await _context.Reports.FirstOrDefaultAsync(r => r.UserId == userId && r.NewsArticleId == articleId);
        }

        public async Task<Report> CreateAsync(Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return false;
            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetReportCountForArticleAsync(int articleId)
        {
            return await _context.Reports.CountAsync(r => r.NewsArticleId == articleId);
        }
    }
} 