using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Services.Interfaces
{
    public interface INewsApiClient
    {
        Task<IEnumerable<NewsArticle>> FetchTopHeadlinesAsync(CancellationToken cancellationToken = default);
        bool IsConfigurationValid();
    }
} 