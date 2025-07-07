using NewsAggregation.Server.Models.Entities;
namespace NewsAggregation.Server.Services.Interfaces
{
    public interface IExternalNewsService
    {
        Task<IEnumerable<NewsArticle>> FetchLatestNewsAsync(CancellationToken cancellationToken = default);
    }
}