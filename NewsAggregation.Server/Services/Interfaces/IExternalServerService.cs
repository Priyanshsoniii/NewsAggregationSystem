using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Services.Interfaces
{
    public interface IExternalServerService
    {
        Task<IEnumerable<ExternalServer>> GetAllServersAsync();
        Task<ExternalServer?> GetServerByIdAsync(int id);
        Task<ExternalServer> CreateServerAsync(ExternalServer server);
        Task<ExternalServer> UpdateServerAsync(ExternalServer server);
        Task<bool> DeleteServerAsync(int id);
        Task<bool> ToggleServerStatusAsync(int id);
        Task UpdateServerLastAccessedAsync(int id);
    }
} 