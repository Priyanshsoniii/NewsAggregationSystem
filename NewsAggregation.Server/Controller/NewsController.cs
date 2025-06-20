using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        // GET: api/News/headlines
        [HttpGet("headlines")]
        public async Task<IActionResult> GetHeadlines()
        {
            var headlines = await _newsService.GetHeadlinesAsync();
            return Ok(headlines);
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
            var headlines = await _newsService.GetTodayHeadlinesAsync();
            return Ok(headlines);
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

        // GET: api/News/saved/{userId}
        [HttpGet("saved/{userId:int}")]
        public async Task<IActionResult> GetSavedArticles(int userId)
        {
            var articles = await _newsService.GetSavedArticlesAsync(userId);
            return Ok(articles);
        }

        // POST: api/News/saved
        [HttpPost("saved")]
        public async Task<IActionResult> SaveArticle([FromBody] SaveArticleDto dto)
        {
            var result = await _newsService.SaveArticleAsync(dto.UserId, dto.ArticleId);
            if (result)
                return Ok(new { Message = "Article saved successfully" });

            return BadRequest(new { Message = "Failed to save article" });
        }

        // DELETE: api/News/saved
        [HttpDelete("saved")]
        public async Task<IActionResult> RemoveSavedArticle([FromBody] SaveArticleDto dto)
        {
            var result = await _newsService.RemoveSavedArticleAsync(dto.UserId, dto.ArticleId);
            if (result)
                return Ok(new { Message = "Article removed from saved list" });

            return BadRequest(new { Message = "Failed to remove saved article" });
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
        public int UserId { get; set; }
        public int ArticleId { get; set; }
    }
}
