using NewsAggregation.Server.Data.Models.Entities;

namespace NewsAggregation.Server.Repository.Interfaces
{
    public interface IExternalServerRepository
    {
        Task<ExternalServer?> GetByIdAsync(int id);
        Task<IEnumerable<ExternalServer>> GetAllAsync();
        Task<IEnumerable<ExternalServer>> GetActiveServersAsync();
        Task<ExternalServer> CreateAsync(ExternalServer server);
        Task<ExternalServer> UpdateAsync(ExternalServer server);
        Task<bool> DeleteAsync(int id);
        Task UpdateLastAccessedAsync(int serverId);
    }
}