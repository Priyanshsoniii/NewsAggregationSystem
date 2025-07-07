using static NewsAggregation.Server.Services.ExternalNewsService;

namespace NewsAggregation.Server.Models.Dtos.NewsApi
{
    public class NewsApiArticle
    {
        public NewsApiSource? Source { get; set; }
        public string? Author { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? UrlToImage { get; set; }
        public string? PublishedAt { get; set; }
        public string? Content { get; set; }
    }
}
