using System.Text.Json;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Services.Interfaces;
using NewsAggregation.Server.Exceptions;

namespace NewsAggregation.Server.Services.ExternalClients
{
    public class NewsApiClient : INewsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NewsApiClient> _logger;
        private readonly IConfiguration _configuration;
        private const string ApiName = "NewsAPI";
        private const string ConfigKey = "NewsAPI:ApiKey";
        private const string ApiUrl = "https://newsapi.org/v2/top-headlines?country=us&apiKey={0}";

        public NewsApiClient(HttpClient httpClient, ILogger<NewsApiClient> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<NewsArticle>> FetchTopHeadlinesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateConfiguration();
                var apiKey = GetApiKey();
                var url = string.Format(ApiUrl, apiKey);
                _logger.LogDebug("Fetching top headlines from {ApiName} at {Url}", ApiName, url);
                var response = await _httpClient.GetStringAsync(url, cancellationToken);
                return ParseResponse(response);
            }
            catch (NewsApiConfigurationException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                var message = $"HTTP request failed while fetching from {ApiName}";
                _logger.LogError(ex, message);
                throw new NewsApiException(message, ex);
            }
            catch (JsonException ex)
            {
                var message = $"Failed to parse JSON response from {ApiName}";
                _logger.LogError(ex, message);
                throw new NewsApiException(message, ex);
            }
            catch (TaskCanceledException ex)
            {
                var message = $"Request to {ApiName} was cancelled or timed out";
                _logger.LogWarning(ex, message);
                throw new NewsApiException(message, ex);
            }
            catch (System.Exception ex)
            {
                var message = $"Unexpected error occurred while fetching from {ApiName}";
                _logger.LogError(ex, message);
                throw new NewsApiException(message, ex);
            }
        }

        public bool IsConfigurationValid()
        {
            try
            {
                var apiKey = GetApiKey();
                return !string.IsNullOrWhiteSpace(apiKey);
            }
            catch
            {
                return false;
            }
        }

        private void ValidateConfiguration()
        {
            if (!IsConfigurationValid())
            {
                var message = $"{ApiName} configuration is invalid. Please check the API key configuration.";
                _logger.LogWarning(message);
                throw new NewsApiConfigurationException(message);
            }
        }

        private string GetApiKey()
        {
            var apiKey = _configuration[ConfigKey];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new NewsApiConfigurationException($"{ApiName} API key is not configured. Please add '{ConfigKey}' to your configuration.");
            }
            return apiKey;
        }

        private IEnumerable<NewsArticle> ParseResponse(string response)
        {
            using var doc = JsonDocument.Parse(response);
            var articles = new List<NewsArticle>();
            if (!doc.RootElement.TryGetProperty("articles", out var dataArray))
            {
                _logger.LogWarning("{ApiName} response does not contain 'articles' property", ApiName);
                return articles;
            }
            foreach (var item in dataArray.EnumerateArray())
            {
                try
                {
                    articles.Add(new NewsArticle
                    {
                        Title = item.GetProperty("title").GetString() ?? "No Title",
                        Description = item.GetProperty("description").GetString() ?? "No Description",
                        Url = item.GetProperty("url").GetString() ?? string.Empty,
                        Source = item.GetProperty("source").GetProperty("name").GetString() ?? ApiName,
                        PublishedAt = System.DateTime.TryParse(item.GetProperty("publishedAt").GetString(), out var date) ? date : System.DateTime.UtcNow,
                        ImageUrl = item.TryGetProperty("urlToImage", out var img) ? img.GetString() : null
                    });
                }
                catch (System.Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse article from {ApiName} response", ApiName);
                }
            }
            return articles;
        }
    }
} 