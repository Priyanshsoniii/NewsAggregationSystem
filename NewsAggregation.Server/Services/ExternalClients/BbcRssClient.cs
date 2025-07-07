using System.ServiceModel.Syndication;
using System.Xml;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Services.Interfaces;
using NewsAggregation.Server.Exceptions;


namespace NewsAggregation.Server.Services.ExternalClients
{
    public class BbcRssClient : IBbcRssClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BbcRssClient> _logger;
        private readonly IConfiguration _configuration;
        private const string ApiName = "BBC RSS";
        private const string FeedUrl = "https://feeds.bbci.co.uk/news/rss.xml";

        public BbcRssClient(HttpClient httpClient, ILogger<BbcRssClient> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<NewsArticle>> FetchRssArticlesAsync(CancellationToken cancellationToken = default)
        {
            var articles = new List<NewsArticle>();
            try
            {
                ValidateConfiguration();
                using var stream = await _httpClient.GetStreamAsync(FeedUrl, cancellationToken);
                using var reader = XmlReader.Create(stream);
                var feed = SyndicationFeed.Load(reader);
                foreach (var item in feed.Items)
                {
                    articles.Add(new NewsArticle
                    {
                        Title = item.Title?.Text ?? "No Title",
                        Description = item.Summary?.Text ?? string.Empty,
                        Url = item.Links.FirstOrDefault()?.Uri.ToString() ?? string.Empty,
                        Source = ApiName,
                        PublishedAt = item.PublishDate.UtcDateTime != System.DateTime.MinValue ? item.PublishDate.UtcDateTime : System.DateTime.UtcNow,
                    });
                }
                return articles;
            }
            catch (BbcRssConfigurationException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                var message = $"HTTP request failed while fetching from {ApiName}";
                _logger.LogError(ex, message);
                throw new BbcRssException(message, ex);
            }
            catch (System.Exception ex)
            {
                var message = $"Unexpected error occurred while fetching from {ApiName}";
                _logger.LogError(ex, message);
                throw new BbcRssException(message, ex);
            }
        }

        public bool IsConfigurationValid()
        {
            return !string.IsNullOrWhiteSpace(FeedUrl);
        }

        private void ValidateConfiguration()
        {
            if (!IsConfigurationValid())
            {
                var message = $"{ApiName} configuration is invalid. Please check the RSS feed URL.";
                _logger.LogWarning(message);
                throw new BbcRssConfigurationException(message);
            }
        }
    }
} 