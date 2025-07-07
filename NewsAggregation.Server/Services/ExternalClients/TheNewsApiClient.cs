using System.Text.Json;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Services.Interfaces;
using NewsAggregation.Server.Exceptions;

namespace NewsAggregation.Server.Services.ExternalClients
{
    /// <summary>
    /// Concrete implementation of The News API client
    /// boundary implementation that handles all external API communication
    /// </summary>
    public class TheNewsApiClient : ITheNewsApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TheNewsApiClient> _logger;
        private readonly IConfiguration _configuration;
        private readonly IExternalServerService _externalServerService;
        
        private const string ApiName = "The News API";
        private const int ServerId = 2; 
        private const string ConfigKey = "TheNewsAPI:ApiKey";

        public TheNewsApiClient(
            HttpClient httpClient, 
            ILogger<TheNewsApiClient> logger, 
            IConfiguration configuration,
            IExternalServerService externalServerService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _externalServerService = externalServerService ?? throw new ArgumentNullException(nameof(externalServerService));
        }

        public async Task<IEnumerable<NewsArticle>> FetchTopNewsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateConfiguration();
                
                var apiKey = GetApiKey();
                var url = BuildApiUrl(apiKey);
                
                _logger.LogDebug("Fetching top news from {ApiName} at {Url}", ApiName, url);
                
                var response = await _httpClient.GetStringAsync(url, cancellationToken);
                var articles = ParseResponse(response);
                
                await UpdateServerLastAccessedAsync();
                
                _logger.LogInformation("Successfully fetched {Count} articles from {ApiName}", articles.Count(), ApiName);
                
                return articles;
            }
            catch (TheNewsApiConfigurationException)
            {
                throw;
            }
            catch (HttpRequestException ex)
            {
                var message = $"HTTP request failed while fetching from {ApiName}";
                _logger.LogError(ex, message);
                throw new TheNewsApiException(message, ex);
            }
            catch (JsonException ex)
            {
                var message = $"Failed to parse JSON response from {ApiName}";
                _logger.LogError(ex, message);
                throw new TheNewsApiException(message, ex);
            }
            catch (TaskCanceledException ex)
            {
                var message = $"Request to {ApiName} was cancelled or timed out";
                _logger.LogWarning(ex, message);
                throw new TheNewsApiException(message, ex);
            }
            catch (Exception ex)
            {
                var message = $"Unexpected error occurred while fetching from {ApiName}";
                _logger.LogError(ex, message);
                throw new TheNewsApiException(message, ex);
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
                throw new TheNewsApiConfigurationException(message);
            }
        }

        private string GetApiKey()
        {
            var apiKey = _configuration[ConfigKey];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new TheNewsApiConfigurationException($"{ApiName} API key is not configured. Please add '{ConfigKey}' to your configuration.");
            }
            return apiKey;
        }

        private string BuildApiUrl(string apiKey)
        {
            return $"https://api.thenewsapi.com/v1/news/top?language=en&api_token={apiKey}";
        }

        private IEnumerable<NewsArticle> ParseResponse(string response)
        {
            using var doc = JsonDocument.Parse(response);
            var articles = new List<NewsArticle>();
            
            if (!doc.RootElement.TryGetProperty("data", out var dataArray))
            {
                _logger.LogWarning("{ApiName} response does not contain 'data' property", ApiName);
                return articles;
            }

            foreach (var item in dataArray.EnumerateArray())
            {
                try
                {
                    var article = MapToNewsArticle(item);
                    if (article != null)
                    {
                        articles.Add(article);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse article from {ApiName} response", ApiName);
                }
            }

            return articles;
        }

        private NewsArticle? MapToNewsArticle(JsonElement item)
        {
            try
            {
                return new NewsArticle
                {
                    Title = GetJsonPropertyValue(item, "title") ?? "No Title",
                    Description = GetJsonPropertyValue(item, "description") ?? "No Description",
                    Url = GetJsonPropertyValue(item, "url") ?? string.Empty,
                    Source = GetJsonPropertyValue(item, "source") ?? ApiName,
                    PublishedAt = ParsePublishedDate(item),
                    ImageUrl = GetJsonPropertyValue(item, "image_url")
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to map {ApiName} item to article", ApiName);
                return null;
            }
        }

        private string? GetJsonPropertyValue(JsonElement element, string propertyName)
        {
            return element.TryGetProperty(propertyName, out var property) ? property.GetString() : null;
        }

        private DateTime ParsePublishedDate(JsonElement item)
        {
            var publishedAtString = GetJsonPropertyValue(item, "published_at");
            if (string.IsNullOrEmpty(publishedAtString))
            {
                return DateTime.UtcNow;
            }

            return DateTime.TryParse(publishedAtString, out var date) ? date : DateTime.UtcNow;
        }

        private async Task UpdateServerLastAccessedAsync()
        {
            try
            {
                await _externalServerService.UpdateServerLastAccessedAsync(ServerId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update last accessed time for {ApiName} server", ApiName);
            }
        }
    }
} 