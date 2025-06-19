using NewsAggregation.Server.Data.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;

        public NewsService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
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
    }
}