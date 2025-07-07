using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Services.Interfaces;
using System.Security.Claims;
using NewsAggregation.Server.Models.Dtos.News;

namespace NewsAggregation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly INotificationService _notificationService;

        public NewsController(INewsService newsService, INotificationService notificationService)
        {
            _newsService = newsService;
            _notificationService = notificationService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        // GET: api/News/headlines
        [HttpGet("headlines")]
        public async Task<IActionResult> GetHeadlines()
        {
            try
            {
                var headlines = await _newsService.GetHeadlinesAsync();
                var headlinesList = headlines.Select(h => new
                {
                    h.Id,
                    h.Title,
                    h.Description,
                    h.Url,
                    h.Source,
                    h.PublishedAt,
                    h.ImageUrl,
                    h.Author,
                    h.Likes,
                    h.Dislikes,
                    CategoryName = h.Category?.Name,
                    h.CategoryId
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Count = headlinesList.Count,
                    Headlines = headlinesList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving headlines" });
            }
        }

        // GET: api/News/headlines/category/{categoryId}
        [HttpGet("headlines/category/{categoryId:int}")]
        public async Task<IActionResult> GetHeadlinesByCategory(int categoryId)
        {
            var headlines = await _newsService.GetHeadlinesByCategoryAsync(categoryId);
            return Ok(headlines);
        }

        // GET: api/News/headlines/today
        [HttpGet("headlines/today")]
        public async Task<IActionResult> GetTodayHeadlines()
        {
            try
            {
                var headlines = await _newsService.GetTodayHeadlinesAsync();
                var headlinesList = headlines.Select(h => new
                {
                    h.Id,
                    h.Title,
                    h.Description,
                    h.Url,
                    h.Source,
                    h.PublishedAt,
                    h.ImageUrl,
                    h.Author,
                    h.Likes,
                    h.Dislikes,
                    CategoryName = h.Category?.Name,
                    h.CategoryId
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Count = headlinesList.Count,
                    Headlines = headlinesList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving today's headlines" });
            }
        }

        // GET: api/News/headlines/daterange
        [HttpGet("headlines/daterange")]
        public async Task<IActionResult> GetHeadlinesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var headlines = await _newsService.GetHeadlinesByDateRangeAsync(startDate, endDate);
            return Ok(headlines);
        }

        // GET: api/News/search
        [HttpGet("search")]
        public async Task<IActionResult> SearchNews([FromQuery] string query, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var results = await _newsService.SearchNewsAsync(query, startDate, endDate);
            return Ok(results);
        }

        // GET: api/News/article/{id}
        [HttpGet("article/{id:int}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            var article = await _newsService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound(new { Message = "Article not found" });

            return Ok(article);
        }

        // GET: api/News/saved
        [HttpGet("saved")]
        public async Task<IActionResult> GetSavedArticles()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var articles = await _newsService.GetSavedArticlesAsync(userId);
                var articlesList = articles.Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.Url,
                    a.Source,
                    a.PublishedAt,
                    a.ImageUrl,
                    a.Author,
                    a.Likes,
                    a.Dislikes,
                    CategoryName = a.Category?.Name,
                    a.CategoryId
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Count = articlesList.Count,
                    SavedArticles = articlesList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving saved articles" });
            }
        }

        // POST: api/News/saved
        [HttpPost("saved")]
        public async Task<IActionResult> SaveArticle([FromBody] SaveArticleDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var result = await _newsService.SaveArticleAsync(userId, dto.ArticleId);
                if (result)
                    return Ok(new { Success = true, Message = "Article saved successfully" });

                return BadRequest(new { Success = false, Message = "Failed to save article" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while saving article" });
            }
        }

        // DELETE: api/News/saved
        [HttpDelete("saved")]
        public async Task<IActionResult> RemoveSavedArticle([FromBody] SaveArticleDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var result = await _newsService.RemoveSavedArticleAsync(userId, dto.ArticleId);
                if (result)
                    return Ok(new { Success = true, Message = "Article removed from saved list" });

                return BadRequest(new { Success = false, Message = "Failed to remove saved article" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while removing saved article" });
            }
        }

        // POST: api/News/{id}/like
        [HttpPost("{id:int}/like")]
        public async Task<IActionResult> LikeArticle(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var result = await _newsService.LikeArticleAsync(userId, id);
                if (!result)
                    return BadRequest(new { Success = false, Message = "You have already liked this article or it does not exist." });

                return Ok(new { Success = true, Message = "Article liked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while liking article" });
            }
        }

        // POST: api/News/{id}/unlike
        [HttpPost("{id:int}/unlike")]
        public async Task<IActionResult> UnlikeArticle(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var result = await _newsService.UnlikeArticleAsync(userId, id);
                if (!result)
                    return BadRequest(new { Success = false, Message = "You have not liked this article or it does not exist." });

                return Ok(new { Success = true, Message = "Article unliked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while unliking article" });
            }
        }

        // POST: api/News/{id}/read
        [HttpPost("{id:int}/read")]
        public async Task<IActionResult> MarkArticleAsRead(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var result = await _newsService.MarkArticleAsReadAsync(userId, id);
                if (!result)
                    return BadRequest(new { Success = false, Message = "You have already marked this article as read or it does not exist." });

                return Ok(new { Success = true, Message = "Article marked as read" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while marking article as read" });
            }
        }

        // GET: api/News/recommendations
        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations([FromQuery] int count = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var articles = await _newsService.GetRecommendedArticlesAsync(userId, count);
                var likedArticles = await _newsService.GetLikedArticlesAsync(userId);
                var savedArticles = await _newsService.GetSavedArticlesAsync(userId);

                var likedIds = likedArticles.Select(a => a.Id).ToHashSet();
                var savedIds = savedArticles.Select(a => a.Id).ToHashSet();

                var articlesList = articles.Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.Url,
                    a.Source,
                    a.PublishedAt,
                    a.ImageUrl,
                    a.Author,
                    a.Likes,
                    a.Dislikes,
                    CategoryName = a.Category?.Name,
                    a.CategoryId,
                    IsLiked = likedIds.Contains(a.Id),
                    IsSaved = savedIds.Contains(a.Id)
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Count = articlesList.Count,
                    Recommendations = articlesList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving recommendations" });
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportArticles([FromBody] List<NewsArticle> articles)
        {
            await _newsService.ImportArticlesAsync(articles);
            return Ok(new { Message = "Articles imported successfully" });
        }

        // POST: api/News/report
        [HttpPost("report")]
        public async Task<IActionResult> ReportArticle([FromBody] ReportArticleDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var result = await _newsService.ReportArticleAsync(userId, dto.ArticleId, dto.Reason);
                if (!result)
                    return BadRequest(new { Success = false, Message = "You have already reported this article or it is hidden." });

                return Ok(new { Success = true, Message = "Article reported successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while reporting the article" });
            }
        }

        // GET: api/News/user-preferences
        [HttpGet("user-preferences")]
        public async Task<IActionResult> GetUserPreferences()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var likedArticles = await _newsService.GetLikedArticlesAsync(userId);

                var savedArticles = await _newsService.GetSavedArticlesAsync(userId);

                var readArticles = await _newsService.GetReadArticlesAsync(userId);

                var notificationSettings = await _notificationService.GetUserNotificationSettingsAsync(userId);

                var userKeywords = await _newsService.GetUserKeywordsAsync(userId);

                var preferences = new
                {
                    LikedArticlesCount = likedArticles.Count(),
                    SavedArticlesCount = savedArticles.Count(),
                    ReadArticlesCount = readArticles.Count(),
                    NotificationSettings = notificationSettings.Select(s => new
                    {
                        CategoryId = s.CategoryId,
                        CategoryName = s.Category?.Name ?? "General",
                        IsEnabled = s.IsEnabled,
                        EmailNotifications = s.EmailNotifications,
                        Keywords = s.Keywords
                    }),
                    UserKeywords = userKeywords,
                    PreferredCategories = likedArticles
                        .GroupBy(a => a.Category?.Name)
                        .OrderByDescending(g => g.Count())
                        .Take(5)
                        .Select(g => new { Category = g.Key, Count = g.Count() })
                };

                return Ok(new
                {
                    Success = true,
                    Preferences = preferences
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving user preferences" });
            }
        }

        // GET: api/News/category/{categoryId}/personalized
        [HttpGet("category/{categoryId:int}/personalized")]
        public async Task<IActionResult> GetPersonalizedArticlesByCategory(int categoryId, [FromQuery] int count = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0)
                    return Unauthorized(new { Message = "Invalid token" });

                var articles = await _newsService.GetPersonalizedArticlesByCategoryAsync(userId, categoryId, count);
                var articlesList = articles.Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.Url,
                    a.Source,
                    a.PublishedAt,
                    a.ImageUrl,
                    a.Author,
                    a.Likes,
                    a.Dislikes,
                    CategoryName = a.Category?.Name,
                    a.CategoryId,
                    IsLiked = false, 
                    IsSaved = false  
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Count = articlesList.Count,
                    CategoryId = categoryId,
                    CategoryName = articlesList.FirstOrDefault()?.CategoryName,
                    Articles = articlesList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving personalized articles" });
            }
        }
    }
}
