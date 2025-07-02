using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly ICategoryRepository _categoryRepository;

        public NewsService(INewsRepository newsRepository, ICategoryRepository categoryRepository)
        {
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<NewsArticle>> GetHeadlinesAsync()
        {
            return await _newsRepository.GetAllAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetHeadlinesByCategoryAsync(int categoryId)
        {
            return await _newsRepository.GetByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<NewsArticle>> GetTodayHeadlinesAsync()
        {
            return await _newsRepository.GetTodayHeadlinesAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetHeadlinesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _newsRepository.GetByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<NewsArticle>> SearchNewsAsync(string query, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _newsRepository.SearchAsync(query, startDate, endDate);
        }

        public async Task<NewsArticle?> GetArticleByIdAsync(int id)
        {
            return await _newsRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<NewsArticle>> GetSavedArticlesAsync(int userId)
        {
            return await _newsRepository.GetSavedArticlesByUserAsync(userId);
        }

        public async Task<bool> SaveArticleAsync(int userId, int articleId)
        {
            return await _newsRepository.SaveArticleForUserAsync(userId, articleId);
        }

        public async Task<bool> RemoveSavedArticleAsync(int userId, int articleId)
        {
            return await _newsRepository.RemoveSavedArticleAsync(userId, articleId);
        }

        public async Task<bool> LikeArticleAsync(int articleId)
        {
            var article = await _newsRepository.GetByIdAsync(articleId);
            if (article == null) return false;

            article.Likes++;
            await _newsRepository.UpdateAsync(article);
            return true;
        }

        public async Task<bool> DislikeArticleAsync(int articleId)
        {
            var article = await _newsRepository.GetByIdAsync(articleId);
            if (article == null) return false;

            article.Dislikes++;
            await _newsRepository.UpdateAsync(article);
            return true;
        }


        public async Task ImportArticlesAsync(List<NewsArticle> articlesFromApi)
        {
            foreach (var article in articlesFromApi)
            {
                if (await _newsRepository.ArticleExistsAsync(article.Url))
                    continue;

                if (article.CategoryId == 0)
                {
                    article.CategoryId = await GetCategoryIdForArticleAsync(article);
                }

                if (article.CreatedAt == default)
                    article.CreatedAt = DateTime.UtcNow;

                article.Likes = article.Likes;
                article.Dislikes = article.Dislikes;

                await _newsRepository.CreateAsync(article);
            }
        }

        private async Task<int> GetCategoryIdForArticleAsync(NewsArticle article)
        {
            var categories = await _categoryRepository.GetAllAsync();
            if (article.Title.Contains("sport", StringComparison.OrdinalIgnoreCase))
                return categories.FirstOrDefault(c => c.Name == "Sports")?.Id ?? categories.First().Id;
            if (article.Title.Contains("business", StringComparison.OrdinalIgnoreCase))
                return categories.FirstOrDefault(c => c.Name == "Business")?.Id ?? categories.First().Id;
            // Add more rules as needed

            return categories.First().Id; // Default
        }


    }
}