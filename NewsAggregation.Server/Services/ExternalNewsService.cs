using System.ServiceModel.Syndication;
using System.Text.Json;
using System.Xml;
using NewsAggregation.Server.Models.Dtos.NewsApi;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Services
{
    public class ExternalNewsService : IExternalNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalNewsService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IExternalServerService _externalServerService;

        public ExternalNewsService(HttpClient httpClient, ILogger<ExternalNewsService> logger, IConfiguration configuration, IExternalServerService externalServerService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _externalServerService = externalServerService;
        }

        private async Task<IEnumerable<NewsAggregation.Server.Models.Entities.NewsArticle>> ParseRSSFeedAsync(string feedUrl, CancellationToken cancellationToken)
        {
            var articles = new List<NewsAggregation.Server.Models.Entities.NewsArticle>();
            try
            {
                using var stream = await _httpClient.GetStreamAsync(feedUrl, cancellationToken);
                using var reader = XmlReader.Create(stream);
                var feed = SyndicationFeed.Load(reader);

                foreach (var item in feed.Items)
                {
                    articles.Add(new NewsAggregation.Server.Models.Entities.NewsArticle
                    {
                        Title = item.Title?.Text ?? "No Title",
                        Description = item.Summary?.Text ?? "",
                        Url = item.Links.FirstOrDefault()?.Uri.ToString() ?? "",
                        Source = new Uri(feedUrl).Host,
                        PublishedAt = item.PublishDate.UtcDateTime != DateTime.MinValue ? item.PublishDate.UtcDateTime : DateTime.UtcNow,
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing RSS feed: {FeedUrl}", feedUrl);
            }
            return articles;
        }

        public async Task<IEnumerable<NewsAggregation.Server.Models.Entities.NewsArticle>> FetchLatestNewsAsync(CancellationToken cancellationToken = default)
        {
            var articles = new List<NewsAggregation.Server.Models.Entities.NewsArticle>();

            try
            {
                var tasks = new List<Task<IEnumerable<NewsAggregation.Server.Models.Entities.NewsArticle>>>
        {
            FetchFromNewsAPIAsync(cancellationToken),
            FetchFromRSSFeedsAsync(cancellationToken)
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

        private async Task<IEnumerable<NewsAggregation.Server.Models.Entities.NewsArticle>> FetchFromNewsAPIAsync(CancellationToken cancellationToken)
        {
            try
            {
                var apiKey = _configuration["NewsAPI:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    _logger.LogWarning("NewsAPI key not configured, skipping NewsAPI fetch");
                    await _externalServerService.UpdateServerLastAccessedAsync(1);
                    return new List<NewsAggregation.Server.Models.Entities.NewsArticle>();
                }

                var url = $"https://newsapi.org/v2/top-headlines?country=us&apiKey={apiKey}";
                var response = await _httpClient.GetStringAsync(url, cancellationToken);

                var newsApiResponse = JsonSerializer.Deserialize<NewsApiResponse>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var articles = newsApiResponse?.Articles?.Select(article => new NewsAggregation.Server.Models.Entities.NewsArticle
                {
                    Title = article.Title ?? "No Title",
                    Description = article.Description ?? "No Description",
                    Url = article.Url ?? "",
                    Source = article.Source?.Name ?? "NewsAPI",
                    PublishedAt = DateTime.TryParse(article.PublishedAt, out var date) ? date : DateTime.UtcNow,
                    ImageUrl = article.UrlToImage
                }).ToList() ?? new List<NewsAggregation.Server.Models.Entities.NewsArticle>();

                _logger.LogDebug("Fetched {Count} articles from NewsAPI", articles.Count);

                await _externalServerService.UpdateServerLastAccessedAsync(1);

                return articles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching from NewsAPI");
                return new List<NewsAggregation.Server.Models.Entities.NewsArticle>();
            }
        }

        private async Task<IEnumerable<NewsAggregation.Server.Models.Entities.NewsArticle>> FetchFromRSSFeedsAsync(CancellationToken cancellationToken)
        {
            var articles = new List<NewsAggregation.Server.Models.Entities.NewsArticle>();
            var rssFeeds = new[]
            {
               "https://rss.cnn.com/rss/edition.rss",
               "https://feeds.bbci.co.uk/news/rss.xml",
               "https://www.reuters.com/tools/rss"
           };

            foreach (var feedUrl in rssFeeds)
            {
                try
                {
                    var feedArticles = await ParseRSSFeedAsync(feedUrl, cancellationToken);
                    articles.AddRange(feedArticles);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to fetch RSS feed: {FeedUrl}", feedUrl);
                }
            }

            _logger.LogDebug("Fetched {Count} articles from RSS feeds", articles.Count);
            return articles;
        }
    }
}
