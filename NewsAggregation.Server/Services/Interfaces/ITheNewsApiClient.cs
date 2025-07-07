using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Services.Interfaces
{
    public interface ITheNewsApiClient
    {
        Task<IEnumerable<NewsArticle>> FetchTopNewsAsync(CancellationToken cancellationToken = default);

        bool IsConfigurationValid();
    }
} 