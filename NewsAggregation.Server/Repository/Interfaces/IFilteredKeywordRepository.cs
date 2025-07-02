using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Repository.Interfaces
{
    public interface IFilteredKeywordRepository
    {
        Task<FilteredKeyword?> GetByIdAsync(int id);
        Task<FilteredKeyword?> GetByKeywordAsync(string keyword);
        Task<IEnumerable<FilteredKeyword>> GetAllAsync();
        Task<FilteredKeyword> CreateAsync(FilteredKeyword keyword);
        Task<bool> DeleteAsync(int id);
    }
} 