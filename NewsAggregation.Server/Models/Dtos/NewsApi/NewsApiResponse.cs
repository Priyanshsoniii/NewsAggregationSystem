using static NewsAggregation.Server.Services.ExternalNewsService;

namespace NewsAggregation.Server.Models.Dtos.NewsApi
{
    public class NewsApiResponse
    {
        public string Status { get; set; } = string.Empty;
        public int TotalResults { get; set; }
        public List<NewsApiArticle> Articles { get; set; } = new();
    }
}
