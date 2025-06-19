using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalNewsController : ControllerBase
    {
        private readonly IExternalNewsService _externalNewsService;

        public ExternalNewsController(IExternalNewsService externalNewsService)
        {
            _externalNewsService = externalNewsService;
        }

        // GET: api/ExternalNews/latest
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestNews(CancellationToken cancellationToken)
        {
            var articles = await _externalNewsService.FetchLatestNewsAsync(cancellationToken);
            return Ok(articles);
        }
    }
}
