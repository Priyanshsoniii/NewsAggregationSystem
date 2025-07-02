using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Services
{
    public class ExternalServerService : IExternalServerService
    {
        private readonly IExternalServerRepository _externalServerRepository;

        public ExternalServerService(IExternalServerRepository externalServerRepository)
        {
            _externalServerRepository = externalServerRepository;
        }

        public async Task<IEnumerable<ExternalServer>> GetAllServersAsync()
        {
            return await _externalServerRepository.GetAllAsync();
        }

        public async Task<ExternalServer?> GetServerByIdAsync(int id)
        {
            return await _externalServerRepository.GetByIdAsync(id);
        }

        public async Task<ExternalServer> CreateServerAsync(ExternalServer server)
        {
            server.CreatedAt = DateTime.UtcNow;
            server.IsActive = true;
            server.CurrentHourRequests = 0;
            server.LastHourReset = DateTime.UtcNow;
            
            return await _externalServerRepository.CreateAsync(server);
        }

        public async Task<ExternalServer> UpdateServerAsync(ExternalServer server)
        {
            var existingServer = await _externalServerRepository.GetByIdAsync(server.Id);
            if (existingServer == null)
                throw new ArgumentException("Server not found");

            // Update only allowed fields
            existingServer.Name = server.Name;
            existingServer.ApiUrl = server.ApiUrl;
            existingServer.ApiKey = server.ApiKey;
            existingServer.ServerType = server.ServerType;
            existingServer.IsActive = server.IsActive;
            existingServer.RequestsPerHour = server.RequestsPerHour;

            return await _externalServerRepository.UpdateAsync(existingServer);
        }

        public async Task<bool> DeleteServerAsync(int id)
        {
            return await _externalServerRepository.DeleteAsync(id);
        }

        public async Task<bool> ToggleServerStatusAsync(int id)
        {
            var server = await _externalServerRepository.GetByIdAsync(id);
            if (server == null) return false;

            server.IsActive = !server.IsActive;
            await _externalServerRepository.UpdateAsync(server);
            return true;
        }

        public async Task UpdateServerLastAccessedAsync(int id)
        {
            await _externalServerRepository.UpdateLastAccessedAsync(id);
        }
    }
} 