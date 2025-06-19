using NewsAggregation.Server.Models.Entities;
namespace NewsAggregation.Server.Services.Interfaces
{
    public interface IExternalNewsService
    {
        Task<IEnumerable<NewsArticle>> FetchLatestNewsAsync(CancellationToken cancellationToken = default);
    }
}


//Task<IEnumerable<NewsArticle>> FetchFromNewsApiAsync();
//Task<IEnumerable<NewsArticle>> FetchFromTheNewsApiAsync();
//Task<IEnumerable<NewsArticle>> FetchFromFirebaseApiAsync();
//Task<int> ClassifyArticleCategoryAsync(string title, string description);
//Task<bool> ProcessAndStoreArticlesAsync(IEnumerable<NewsArticle> articles);