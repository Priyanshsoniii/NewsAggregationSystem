//using NewsAggregation.Server.Data;
//using NewsAggregation.Server.Models.Entities;
//using NewsAggregation.Server.Repository.Interfaces;

//namespace NewsAggregation.Server.Repository
//{
//    public class NewsArticleRepository : INewsArticleRepository
//    {
//        private readonly NewsAggregationContext _context;

//        public NewsArticleRepository(NewsAggregationContext context)
//        {
//            _context = context;
//        }

//        public async Task SaveArticlesAsync(IEnumerable<NewsArticle> articles, CancellationToken cancellationToken)
//        {
//            await _context.NewsArticles.AddRangeAsync(articles, cancellationToken);
//            await _context.SaveChangesAsync(cancellationToken);
//        }
//    }
//}
