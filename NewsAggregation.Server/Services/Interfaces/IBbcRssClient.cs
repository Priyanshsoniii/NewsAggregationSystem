using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Services.Interfaces
{
    public interface IBbcRssClient
    {
        Task<IEnumerable<NewsArticle>> FetchRssArticlesAsync(CancellationToken cancellationToken = default);
        bool IsConfigurationValid();
    }
} 