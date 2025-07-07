using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Xml;
using NewsAggregation.Server.Models.Dtos.NewsApi;
using NewsAggregation.Server.Services.Interfaces;
using NewsAggregation.Server.Exceptions;

namespace NewsAggregation.Server.Services
{
    public class ExternalNewsService : IExternalNewsService
    {
        private readonly ILogger<ExternalNewsService> _logger;
        private readonly ITheNewsApiClient _theNewsApiClient;
        private readonly INewsApiClient _newsApiClient;
        private readonly IBbcRssClient _bbcRssClient;

        public ExternalNewsService(
            ILogger<ExternalNewsService> logger,
            ITheNewsApiClient theNewsApiClient,
            INewsApiClient newsApiClient,
            IBbcRssClient bbcRssClient)
        {
            _logger = logger;
            _theNewsApiClient = theNewsApiClient;
            _newsApiClient = newsApiClient;
            _bbcRssClient = bbcRssClient;
        }

        public async Task<IEnumerable<NewsAggregation.Server.Models.Entities.NewsArticle>> FetchLatestNewsAsync(CancellationToken cancellationToken = default)
        {
            var articles = new List<NewsAggregation.Server.Models.Entities.NewsArticle>();
            try
            {
                var tasks = new List<Task<IEnumerable<NewsAggregation.Server.Models.Entities.NewsArticle>>>
                {
                    FetchFromNewsApiAsync(cancellationToken),
                    FetchFromTheNewsApiAsync(cancellationToken),
                    FetchFromBbcRssAsync(cancellationToken)
                };
                var results = await Task.WhenAll(tasks);
                foreach (var result in results)
                {
                    articles.AddRange(result);
                }
                var uniqueArticles = articles
                    .GroupBy(a => a.Title.ToLowerInvariant())
                    .Select(g => g.First())
                    .OrderByDescending(a => a.PublishedAt)
                    .ToList();
                _logger.LogDebug("Fetched {Count} unique articles from external sources", uniqueArticles.Count);
                return uniqueArticles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching news from external sources");
                return new List<NewsAggregation.Server.Models.Entities.NewsArticle>();
            }
        }

        private async Task<IEnumerable<NewsAggregation.Server.Models.Entities.NewsArticle>> FetchFromNewsApiAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _newsApiClient.FetchTopHeadlinesAsync(cancellationToken);
            }
            catch (NewsApiConfigurationException ex)
            {
                _logger.LogWarning(ex, "NewsAPI configuration error: {Message}", ex.Message);
                return Enumerable.Empty<NewsAggregation.Server.Models.Entities.NewsArticle>();
            }
            catch (NewsApiException ex)
            {
                _logger.LogError(ex, "NewsAPI error: {Message}", ex.Message);
                return Enumerable.Empty<NewsAggregation.Server.Models.Entities.NewsArticle>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error with NewsAPI: {Message}", ex.Message);
                return Enumerable.Empty<NewsAggregation.Server.Models.Entities.NewsArticle>();
            }
        }

        private async Task<IEnumerable<NewsAggregation.Server.Models.Entities.NewsArticle>> FetchFromTheNewsApiAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _theNewsApiClient.FetchTopNewsAsync(cancellationToken);
            }
            catch (TheNewsApiConfigurationException ex)
            {
                _logger.LogWarning(ex, "The News API configuration error: {Message}", ex.Message);
                return Enumerable.Empty<NewsAggregation.Server.Models.Entities.NewsArticle>();
            }
            catch (TheNewsApiException ex)
            {
                _logger.LogError(ex, "The News API error: {Message}", ex.Message);
                return Enumerable.Empty<NewsAggregation.Server.Models.Entities.NewsArticle>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error with The News API: {Message}", ex.Message);
                return Enumerable.Empty<NewsAggregation.Server.Models.Entities.NewsArticle>();
            }
        }

        private async Task<IEnumerable<NewsAggregation.Server.Models.Entities.NewsArticle>> FetchFromBbcRssAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _bbcRssClient.FetchRssArticlesAsync(cancellationToken);
            }
            catch (BbcRssConfigurationException ex)
            {
                _logger.LogWarning(ex, "BBC RSS configuration error: {Message}", ex.Message);
                return Enumerable.Empty<NewsAggregation.Server.Models.Entities.NewsArticle>();
            }
            catch (BbcRssException ex)
            {
                _logger.LogError(ex, "BBC RSS error: {Message}", ex.Message);
                return Enumerable.Empty<NewsAggregation.Server.Models.Entities.NewsArticle>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error with BBC RSS: {Message}", ex.Message);
                return Enumerable.Empty<NewsAggregation.Server.Models.Entities.NewsArticle>();
            }
        }
    }
}
