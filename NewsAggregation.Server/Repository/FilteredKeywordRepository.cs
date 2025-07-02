using Microsoft.EntityFrameworkCore;
using NewsAggregation.Server.Data;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;

namespace NewsAggregation.Server.Repository
{
    public class FilteredKeywordRepository : IFilteredKeywordRepository
    {
        private readonly NewsAggregationContext _context;

        public FilteredKeywordRepository(NewsAggregationContext context)
        {
            _context = context;
        }

        public async Task<FilteredKeyword?> GetByIdAsync(int id)
        {
            return await _context.FilteredKeywords.FindAsync(id);
        }

        public async Task<FilteredKeyword?> GetByKeywordAsync(string keyword)
        {
            return await _context.FilteredKeywords.FirstOrDefaultAsync(k => k.Keyword.ToLower() == keyword.ToLower());
        }

        public async Task<IEnumerable<FilteredKeyword>> GetAllAsync()
        {
            return await _context.FilteredKeywords.ToListAsync();
        }

        public async Task<FilteredKeyword> CreateAsync(FilteredKeyword keyword)
        {
            _context.FilteredKeywords.Add(keyword);
            await _context.SaveChangesAsync();
            return keyword;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var keyword = await _context.FilteredKeywords.FindAsync(id);
            if (keyword == null) return false;
            _context.FilteredKeywords.Remove(keyword);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 