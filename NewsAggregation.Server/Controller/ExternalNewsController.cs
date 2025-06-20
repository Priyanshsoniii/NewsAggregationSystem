using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Server.Services;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalNewsController : ControllerBase
    {
        private readonly IExternalNewsService _externalNewsService;
        private readonly INewsService _newsService;
        private readonly ILogger<ExternalNewsController> _logger;

        public ExternalNewsController(
            IExternalNewsService externalNewsService,
            INewsService newsService,
            ILogger<ExternalNewsController> logger)
        {
            _externalNewsService = externalNewsService;
            _newsService = newsService;
            _logger = logger;
        }

        // GET: api/ExternalNews/latest
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestNews(CancellationToken cancellationToken)
        {
            try
            {
                // Fetch articles from external sources
                var articles = await _externalNewsService.FetchLatestNewsAsync(cancellationToken);

                // Convert to List for ImportArticlesAsync method
                var articlesList = articles.ToList();

                if (articlesList.Any())
                {
                    // Save articles to database
                    await _newsService.ImportArticlesAsync(articlesList);
                    _logger.LogInformation("Successfully imported {Count} articles to database", articlesList.Count);
                }
                else
                {
                    _logger.LogWarning("No articles were fetched from external sources");
                }

                // Create DTOs to avoid circular reference
                var articleDtos = articlesList.Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.Url,
                    a.Source,
                    a.PublishedAt,
                    a.CreatedAt,
                    a.ImageUrl,
                    a.Author,
                    a.Likes,
                    a.Dislikes,
                    CategoryName = a.Category?.Name,
                    a.CategoryId
                }).ToList();

                return Ok(new
                {
                    Message = "Articles fetched and saved successfully",
                    Count = articlesList.Count,
                    Articles = articleDtos
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching and saving latest news");
                return StatusCode(500, new { Error = "An error occurred while processing the request" });
            }
        }

        // GET: api/ExternalNews/fetch-only (if you want to fetch without saving)
        [HttpGet("fetch-only")]
        public async Task<IActionResult> FetchLatestNewsOnly(CancellationToken cancellationToken)
        {
            try
            {
                var articles = await _externalNewsService.FetchLatestNewsAsync(cancellationToken);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching latest news");
                return StatusCode(500, new { Error = "An error occurred while processing the request" });
            }
        }
    }
}