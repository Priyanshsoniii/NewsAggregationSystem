using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Models.Dtos.Admin;
using NewsAggregation.Server.Models.Dtos.News;
using NewsAggregation.Server.Services.Interfaces;
using NewsAggregation.Server.Repository.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IExternalServerService _externalServerService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<AdminController> _logger;
        private readonly IReportRepository _reportRepository;
        private readonly IFilteredKeywordRepository _filteredKeywordRepository;
        private readonly INewsRepository _newsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IExternalNewsService _externalNewsService;
        private readonly INewsService _newsService;

        public AdminController(
            IExternalServerService externalServerService,
            ICategoryService categoryService,
            ILogger<AdminController> logger,
            IReportRepository reportRepository,
            IFilteredKeywordRepository filteredKeywordRepository,
            INewsRepository newsRepository,
            ICategoryRepository categoryRepository,
            IExternalNewsService externalNewsService,
            INewsService newsService)
        {
            _externalServerService = externalServerService;
            _categoryService = categoryService;
            _logger = logger;
            _reportRepository = reportRepository;
            _filteredKeywordRepository = filteredKeywordRepository;
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
            _externalNewsService = externalNewsService;
            _newsService = newsService;
        }

        // GET: api/Admin/servers
        [HttpGet("servers")]
        public async Task<IActionResult> GetAllServers()
        {
            try
            {
                var servers = await _externalServerService.GetAllServersAsync();

                var serverList = servers.Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.ServerType,
                    Status = s.IsActive ? "Active" : "Not Active",
                    LastAccessed = s.LastAccessed?.ToString("dd MMM yyyy") ?? "Never",
                    s.ApiUrl,
                    s.RequestsPerHour,
                    s.CurrentHourRequests,
                    s.CreatedAt
                }).ToList();

                return Ok(new
                {
                    Message = "External servers retrieved successfully",
                    Count = serverList.Count,
                    Servers = serverList
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving external servers");
                return StatusCode(500, new { Error = "An error occurred while retrieving servers" });
            }
        }

        // GET: api/Admin/servers/{id}
        [HttpGet("servers/{id:int}")]
        public async Task<IActionResult> GetServerById(int id)
        {
            try
            {
                var server = await _externalServerService.GetServerByIdAsync(id);
                if (server == null)
                    return NotFound(new { Message = "Server not found" });

                var serverDetails = new
                {
                    server.Id,
                    server.Name,
                    server.ServerType,
                    server.ApiUrl,
                    ApiKey = "***" + server.ApiKey.Substring(Math.Max(0, server.ApiKey.Length - 4)), // Mask API key
                    server.IsActive,
                    server.LastAccessed,
                    server.RequestsPerHour,
                    server.CurrentHourRequests,
                    server.LastHourReset,
                    server.CreatedAt
                };

                return Ok(new
                {
                    Message = "Server details retrieved successfully",
                    Server = serverDetails
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving server details for ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while retrieving server details" });
            }
        }

        // PUT: api/Admin/servers/{id}
        [HttpPut("servers/{id:int}")]
        public async Task<IActionResult> UpdateServer(int id, [FromBody] UpdateServerDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingServer = await _externalServerService.GetServerByIdAsync(id);
                if (existingServer == null)
                    return NotFound(new { Message = "Server not found" });

                // Update only the fields provided
                existingServer.Name = updateDto.Name ?? existingServer.Name;
                existingServer.ApiUrl = updateDto.ApiUrl ?? existingServer.ApiUrl;
                existingServer.ApiKey = updateDto.ApiKey ?? existingServer.ApiKey;
                existingServer.ServerType = updateDto.ServerType ?? existingServer.ServerType;
                existingServer.IsActive = updateDto.IsActive ?? existingServer.IsActive;
                existingServer.RequestsPerHour = updateDto.RequestsPerHour ?? existingServer.RequestsPerHour;

                var updatedServer = await _externalServerService.UpdateServerAsync(existingServer);

                return Ok(new
                {
                    Message = "Server updated successfully",
                    Server = new
                    {
                        updatedServer.Id,
                        updatedServer.Name,
                        updatedServer.ServerType,
                        updatedServer.ApiUrl,
                        updatedServer.IsActive,
                        updatedServer.RequestsPerHour
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating server with ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while updating server" });
            }
        }

        // POST: api/Admin/servers
        [HttpPost("servers")]
        public async Task<IActionResult> CreateServer([FromBody] CreateServerDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var server = new ExternalServer
                {
                    Name = createDto.Name,
                    ApiUrl = createDto.ApiUrl,
                    ApiKey = createDto.ApiKey,
                    ServerType = createDto.ServerType,
                    IsActive = createDto.IsActive,
                    RequestsPerHour = createDto.RequestsPerHour
                };

                var createdServer = await _externalServerService.CreateServerAsync(server);

                return CreatedAtAction(nameof(GetServerById), new { id = createdServer.Id }, new
                {
                    Message = "Server created successfully",
                    Server = new
                    {
                        createdServer.Id,
                        createdServer.Name,
                        createdServer.ServerType,
                        createdServer.ApiUrl,
                        createdServer.IsActive,
                        createdServer.RequestsPerHour
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new server");
                return StatusCode(500, new { Error = "An error occurred while creating server" });
            }
        }

        // DELETE: api/Admin/servers/{id}
        [HttpDelete("servers/{id:int}")]
        public async Task<IActionResult> DeleteServer(int id)
        {
            try
            {
                var result = await _externalServerService.DeleteServerAsync(id);
                if (!result)
                    return NotFound(new { Message = "Server not found" });

                return Ok(new { Message = "Server deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting server with ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while deleting server" });
            }
        }

        // PATCH: api/Admin/servers/{id}/toggle-status
        [HttpPatch("servers/{id:int}/toggle-status")]
        public async Task<IActionResult> ToggleServerStatus(int id)
        {
            try
            {
                var result = await _externalServerService.ToggleServerStatusAsync(id);
                if (!result)
                    return NotFound(new { Message = "Server not found" });

                var server = await _externalServerService.GetServerByIdAsync(id);
                return Ok(new
                {
                    Message = $"Server status toggled successfully. New status: {(server?.IsActive == true ? "Active" : "Inactive")}",
                    IsActive = server?.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling server status for ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while toggling server status" });
            }
        }

        // POST: api/Admin/categories
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var category = new Category
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    IsActive = true
                };

                var createdCategory = await _categoryService.CreateCategoryAsync(category);

                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, new
                {
                    Message = "Category created successfully",
                    Category = new
                    {
                        createdCategory.Id,
                        createdCategory.Name,
                        createdCategory.Description,
                        createdCategory.IsActive,
                        createdCategory.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new category");
                return StatusCode(500, new { Error = "An error occurred while creating category" });
            }
        }

        // GET: api/Admin/categories/{id}
        [HttpGet("categories/{id:int}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound(new { Message = "Category not found" });

                return Ok(new
                {
                    Message = "Category retrieved successfully",
                    Category = new
                    {
                        category.Id,
                        category.Name,
                        category.Description,
                        category.IsActive,
                        category.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while retrieving category" });
            }
        }

        // GET: api/Admin/reported-articles
        [HttpGet("reported-articles")]
        public async Task<IActionResult> GetReportedArticles()
        {
            var allArticles = await _newsRepository.GetAllAsync();
            var reported = allArticles.Where(a => a.ReportCount > 0).Select(a => new
            {
                a.Id,
                a.Title,
                a.ReportCount,
                a.IsHidden
            }).OrderByDescending(a => a.ReportCount).ToList();
            return Ok(new { Success = true, Count = reported.Count, ReportedArticles = reported });
        }

        // PATCH: api/Admin/articles/{id}/hide
        [HttpPatch("articles/{id:int}/hide")]
        public async Task<IActionResult> HideArticle(int id, [FromQuery] bool hide = true)
        {
            var article = await _newsRepository.GetByIdAsync(id);
            if (article == null)
                return NotFound(new { Message = "Article not found" });
            article.IsHidden = hide;
            await _newsRepository.UpdateAsync(article);
            return Ok(new { Success = true, Message = $"Article {(hide ? "hidden" : "unhidden")} successfully" });
        }

        // PATCH: api/Admin/categories/{id}/hide
        [HttpPatch("categories/{id:int}/hide")]
        public async Task<IActionResult> HideCategory(int id, [FromQuery] bool hide = true)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { Message = "Category not found" });
            category.IsHidden = hide;
            await _categoryRepository.UpdateAsync(category);
            return Ok(new { Success = true, Message = $"Category {(hide ? "hidden" : "unhidden")} successfully" });
        }

        // GET: api/Admin/filtered-keywords
        [HttpGet("filtered-keywords")]
        public async Task<IActionResult> GetFilteredKeywords()
        {
            var keywords = await _filteredKeywordRepository.GetAllAsync();
            return Ok(new { Success = true, Count = keywords.Count(), Keywords = keywords });
        }

        // POST: api/Admin/filtered-keywords
        [HttpPost("filtered-keywords")]
        public async Task<IActionResult> AddFilteredKeyword([FromBody] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { Message = "Keyword is required" });
            var exists = await _filteredKeywordRepository.GetByKeywordAsync(keyword);
            if (exists != null)
                return Conflict(new { Message = "Keyword already exists" });
            var created = await _filteredKeywordRepository.CreateAsync(new FilteredKeyword { Keyword = keyword });
            return Ok(new { Success = true, Keyword = created });
        }

        // DELETE: api/Admin/filtered-keywords/{id}
        [HttpDelete("filtered-keywords/{id:int}")]
        public async Task<IActionResult> DeleteFilteredKeyword(int id)
        {
            var result = await _filteredKeywordRepository.DeleteAsync(id);
            if (!result)
                return NotFound(new { Message = "Keyword not found" });
            return Ok(new { Success = true, Message = "Keyword deleted successfully" });
        }

        // POST: api/Admin/aggregate-news
        [HttpPost("aggregate-news")]
        public async Task<IActionResult> AggregateNews()
        {
            try
            {
                _logger.LogInformation("Manual news aggregation triggered at {Time}", DateTime.UtcNow);

                // Fetch news from external sources
                var newsArticles = await _externalNewsService.FetchLatestNewsAsync();

                _logger.LogInformation("Fetched {Count} news articles", newsArticles.Count());

                // Save articles to database
                if (newsArticles.Any())
                {
                    await _newsService.ImportArticlesAsync(newsArticles.ToList());
                    _logger.LogInformation("Successfully saved {Count} articles to database", newsArticles.Count());
                }

                return Ok(new
                {
                    Message = "News aggregation completed successfully",
                    ArticlesFetched = newsArticles.Count(),
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during manual news aggregation");
                return StatusCode(500, new { Error = "An error occurred during news aggregation" });
            }
        }
    }

    // DTOs for Admin operations
    public class UpdateServerDto
    {
        public string? Name { get; set; }
        public string? ApiUrl { get; set; }
        public string? ApiKey { get; set; }
        public string? ServerType { get; set; }
        public bool? IsActive { get; set; }
        public int? RequestsPerHour { get; set; }
    }

    public class CreateServerDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Url]
        public string ApiUrl { get; set; } = string.Empty;

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        [Required]
        public string ServerType { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public int RequestsPerHour { get; set; } = 1000;
    }

    public class CreateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }
    }
}