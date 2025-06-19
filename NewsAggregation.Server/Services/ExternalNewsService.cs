//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Configuration;
//using System.Text.Json;

//using NewsAggregation.Server.Services.Interfaces;

//namespace NewsAggregation.Server.Services
//{
//    public class ExternalNewsService : IExternalNewsService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly ILogger<ExternalNewsService> _logger;
//        private readonly IConfiguration _configuration;

//        public ExternalNewsService(HttpClient httpClient, ILogger<ExternalNewsService> logger, IConfiguration configuration)
//        {
//            _httpClient = httpClient;
//            _logger = logger;
//            _configuration = configuration;
//        }

//        public async Task<IEnumerable<NewsArticle>> FetchLatestNewsAsync(CancellationToken cancellationToken = default)
//        {
//            var articles = new List<NewsArticle>();

//            try
//            {
//                // Fetch from multiple news sources
//                var tasks = new List<Task<IEnumerable<NewsArticle>>>
//                {
//                    FetchFromNewsAPIAsync(cancellationToken),
//                    FetchFromRSSFeedsAsync(cancellationToken)
//                };

//                var results = await Task.WhenAll(tasks);

//                foreach (var result in results)
//                {
//                    articles.AddRange(result);
//                }

//                // Remove duplicates and sort by publish date
//                var uniqueArticles = articles
//                    .GroupBy(a => a.Title.ToLowerInvariant())
//                    .Select(g => g.First())
//                    .OrderByDescending(a => a.PublishedAt)
//                    .ToList();

//                _logger.LogInformation("Successfully fetched {Count} unique articles from external sources", uniqueArticles.Count);

//                return uniqueArticles;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error fetching news from external sources");
//                return new List<NewsArticle>();
//            }
//        }

//        private async Task<IEnumerable<NewsArticle>> FetchFromNewsAPIAsync(CancellationToken cancellationToken)
//        {
//            try
//            {
//                var apiKey = _configuration["NewsAPI:ApiKey"];
//                if (string.IsNullOrEmpty(apiKey))
//                {
//                    _logger.LogWarning("NewsAPI key not configured, skipping NewsAPI fetch");
//                    return new List<NewsArticle>();
//                }

//                var url = $"https://newsapi.org/v2/top-headlines?country=us&apiKey={apiKey}";
//                var response = await _httpClient.GetStringAsync(url, cancellationToken);

//                var newsApiResponse = JsonSerializer.Deserialize<NewsApiResponse>(response, new JsonSerializerOptions
//                {
//                    PropertyNameCaseInsensitive = true
//                });

//                var articles = newsApiResponse?.Articles?.Select(article => new NewsArticle
//                {
//                    Title = article.Title ?? "No Title",
//                    Description = article.Description ?? "No Description",
//                    Url = article.Url ?? "",
//                    Source = article.Source?.Name ?? "NewsAPI",
//                    PublishedAt = DateTime.TryParse(article.PublishedAt, out var date) ? date : DateTime.UtcNow,
//                    ImageUrl = article.UrlToImage
//                }).ToList() ?? new List<NewsArticle>();

//                _logger.LogInformation("Fetched {Count} articles from NewsAPI", articles.Count);
//                return articles;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error fetching from NewsAPI");
//                return new List<NewsArticle>();
//            }
//        }

//        private async Task<IEnumerable<NewsArticle>> FetchFromRSSFeedsAsync(CancellationToken cancellationToken)
//        {
//            var articles = new List<NewsArticle>();

//            // Sample RSS feeds - you can add more
//            var rssFeeds = new[]
//            {
//                "https://rss.cnn.com/rss/edition.rss",
//                "https://feeds.bbci.co.uk/news/rss.xml",
//                "https://www.reuters.com/tools/rss"
//            };

//            try
//            {
//                foreach (var feedUrl in rssFeeds)
//                {
//                    try
//                    {
//                        var feedArticles = await ParseRSSFeedAsync(feedUrl, cancellationToken);
//                        articles.AddRange(feedArticles);
//                    }
//                    catch (Exception ex)
//                    {
//                        _logger.LogWarning(ex, "Failed to fetch RSS feed: {FeedUrl}", feedUrl);
//                    }
//                }

//                _logger.LogInformation("Fetched {Count} articles from RSS feeds", articles.Count);
//                return articles;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error fetching from RSS feeds");
//                return articles;
//            }
//        }

//        private async Task<IEnumerable<NewsArticle>> ParseRSSFeedAsync(string feedUrl, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var xmlContent = await _httpClient.GetStringAsync(feedUrl, cancellationToken);

//                // This is a simple RSS parser - you might want to use a more robust library like SyndicationFeed
//                var articles = new List<NewsArticle>();

//                // For demonstration, creating a simple RSS article
//                // In production, you'd want to use XDocument or SyndicationFeed to parse properly
//                articles.Add(new NewsArticle
//                {
//                    Title = $"Sample RSS Article from {new Uri(feedUrl).Host}",
//                    Description = "This is a placeholder for RSS content parsing",
//                    Url = feedUrl,
//                    Source = new Uri(feedUrl).Host,
//                    PublishedAt = DateTime.UtcNow
//                });

//                return articles;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error parsing RSS feed: {FeedUrl}", feedUrl);
//                return new List<NewsArticle>();
//            }
//        }
//    }

//    // Data models
//    public class NewsArticle
//    {
//        public string Title { get; set; } = string.Empty;
//        public string Description { get; set; } = string.Empty;
//        public string Url { get; set; } = string.Empty;
//        public string Source { get; set; } = string.Empty;
//        public DateTime PublishedAt { get; set; }
//        public string? ImageUrl { get; set; }
//        public List<string> Categories { get; set; } = new();
//    }

//    // NewsAPI response models
//    public class NewsApiResponse
//    {
//        public string Status { get; set; } = string.Empty;
//        public int TotalResults { get; set; }
//        public List<NewsApiArticle> Articles { get; set; } = new();
//    }

//    public class NewsApiArticle
//    {
//        public NewsApiSource? Source { get; set; }
//        public string? Author { get; set; }
//        public string? Title { get; set; }
//        public string? Description { get; set; }
//        public string? Url { get; set; }
//        public string? UrlToImage { get; set; }
//        public string? PublishedAt { get; set; }
//        public string? Content { get; set; }
//    }

//    public class NewsApiSource
//    {
//        public string? Id { get; set; }
//        public string? Name { get; set; }
//    }
//}