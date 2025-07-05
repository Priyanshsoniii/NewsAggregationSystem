using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Services
{
    public class NewsAggregationService : BackgroundService
    {
        private readonly ILogger<NewsAggregationService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(30); // Run every 30 minutes

        public NewsAggregationService(ILogger<NewsAggregationService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("News Aggregation Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await AggregateNewsAsync(stoppingToken);
                    await Task.Delay(_period, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("News Aggregation Service is stopping");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while aggregating news");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait 5 minutes before retrying
                }
            }
        }

        private async Task AggregateNewsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var externalNewsService = scope.ServiceProvider.GetRequiredService<ExternalNewsService>();
            var newsService = scope.ServiceProvider.GetRequiredService<INewsService>();

            _logger.LogInformation("Starting news aggregation at {Time}", DateTime.UtcNow);

            try
            {
                // Fetch news from external sources
                var newsArticles = await externalNewsService.FetchLatestNewsAsync(cancellationToken);

                _logger.LogInformation("Fetched {Count} news articles", newsArticles.Count());

                // Save articles to database
                if (newsArticles.Any())
                {
                    await newsService.ImportArticlesAsync(newsArticles.ToList());
                    _logger.LogInformation("Successfully saved {Count} articles to database", newsArticles.Count());
                }

                // Log first 5 titles for debugging
                foreach (var article in newsArticles.Take(5))
                {
                    _logger.LogInformation("Article: {Title}", article.Title);
                }

                _logger.LogInformation("News aggregation completed successfully at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during news aggregation");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("News Aggregation Service is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}