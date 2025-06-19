using Microsoft.EntityFrameworkCore;
using NewsAggregation.Server.Data;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;

namespace NewsAggregation.Server.Repository
{
    public class ExternalServerRepository : IExternalServerRepository
    {
        private readonly NewsAggregationContext _context;

        public ExternalServerRepository(NewsAggregationContext context)
        {
            _context = context;
        }

        public async Task<ExternalServer?> GetByIdAsync(int id)
        {
            return await _context.ExternalServers.FindAsync(id);
        }

        public async Task<IEnumerable<ExternalServer>> GetAllAsync()
        {
            return await _context.ExternalServers.ToListAsync();
        }

        public async Task<IEnumerable<ExternalServer>> GetActiveServersAsync()
        {
            return await _context.ExternalServers
                .Where(es => es.IsActive)
                .ToListAsync();
        }

        public async Task<ExternalServer> CreateAsync(ExternalServer server)
        {
            _context.ExternalServers.Add(server);
            await _context.SaveChangesAsync();
            return server;
        }

        public async Task<ExternalServer> UpdateAsync(ExternalServer server)
        {
            _context.ExternalServers.Update(server);
            await _context.SaveChangesAsync();
            return server;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var server = await _context.ExternalServers.FindAsync(id);
            if (server == null) return false;

            server.IsActive = false; // Soft delete
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateLastAccessedAsync(int serverId)
        {
            var server = await _context.ExternalServers.FindAsync(serverId);
            if (server != null)
            {
                server.LastAccessed = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}