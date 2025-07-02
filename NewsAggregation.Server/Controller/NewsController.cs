using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Services.Interfaces;
using System.Security.Claims;

namespace NewsAggregation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
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
                var result = await _newsService.LikeArticleAsync(id);
                if (!result)
                    return NotFound(new { Message = "Article not found" });

                return Ok(new { Success = true, Message = "Article liked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while liking article" });
            }
        }

        // POST: api/News/{id}/dislike
        [HttpPost("{id:int}/dislike")]
        public async Task<IActionResult> DislikeArticle(int id)
        {
            try
            {
                var result = await _newsService.DislikeArticleAsync(id);
                if (!result)
                    return NotFound(new { Message = "Article not found" });

                return Ok(new { Success = true, Message = "Article disliked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while disliking article" });
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportArticles([FromBody] List<NewsArticle> articles)
        {
            await _newsService.ImportArticlesAsync(articles);
            return Ok(new { Message = "Articles imported successfully" });
        }
    }

    public class SaveArticleDto
    {
        public int ArticleId { get; set; }
    }
}
