using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Token, User? User)> LoginAsync(string username, string password);
        Task<(bool Success, string Message, User? User)> RegisterAsync(string username, string email, string password);
    }

}