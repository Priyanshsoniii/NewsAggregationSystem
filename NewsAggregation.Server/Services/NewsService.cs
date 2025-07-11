﻿using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Repository.Interfaces;
using NewsAggregation.Server.Services.Interfaces;

namespace NewsAggregation.Server.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReportRepository _reportRepository;
        private readonly INotificationService _notificationService;
        private readonly IFilteredKeywordRepository _filteredKeywordRepository;
        private readonly IUserArticleLikeRepository _userArticleLikeRepository;
        private readonly IUserArticleReadRepository _userArticleReadRepository;
        private readonly IUserService _userService;

        public NewsService(
            INewsRepository newsRepository,
            ICategoryRepository categoryRepository,
            IReportRepository reportRepository,
            INotificationService notificationService,
            IFilteredKeywordRepository filteredKeywordRepository,
            IUserArticleLikeRepository userArticleLikeRepository,
            IUserArticleReadRepository userArticleReadRepository,
            IUserService userService)
        {
            _newsRepository = newsRepository;
            _categoryRepository = categoryRepository;
            _reportRepository = reportRepository;
            _notificationService = notificationService;
            _filteredKeywordRepository = filteredKeywordRepository;
            _userArticleLikeRepository = userArticleLikeRepository;
            _userArticleReadRepository = userArticleReadRepository;
            _userService = userService;
        }

        private async Task<IEnumerable<NewsArticle>> FilterArticlesAsync(IEnumerable<NewsArticle> articles)
        {
            var filteredKeywords = (await _filteredKeywordRepository.GetAllAsync()).Select(k => k.Keyword.ToLower()).ToList();
            return articles.Where(a =>
                !a.IsHidden &&
                (a.Category == null || !a.Category.IsHidden) &&
                !filteredKeywords.Any(k => a.Title.ToLower().Contains(k) || (a.Description != null && a.Description.ToLower().Contains(k)))
            );
        }

        public async Task<IEnumerable<NewsArticle>> GetHeadlinesAsync()
        {
            var articles = await _newsRepository.GetAllAsync();
            return await FilterArticlesAsync(articles);
        }

        public async Task<IEnumerable<NewsArticle>> GetHeadlinesByCategoryAsync(int categoryId)
        {
            var articles = await _newsRepository.GetByCategoryAsync(categoryId);
            return await FilterArticlesAsync(articles);
        }

        public async Task<IEnumerable<NewsArticle>> GetTodayHeadlinesAsync()
        {
            var articles = await _newsRepository.GetTodayHeadlinesAsync();
            return await FilterArticlesAsync(articles);
        }

        public async Task<IEnumerable<NewsArticle>> GetHeadlinesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var articles = await _newsRepository.GetByDateRangeAsync(startDate, endDate);
            return await FilterArticlesAsync(articles);
        }

        public async Task<IEnumerable<NewsArticle>> SearchNewsAsync(string query, DateTime? startDate = null, DateTime? endDate = null)
        {
            var articles = await _newsRepository.SearchAsync(query, startDate, endDate);
            return await FilterArticlesAsync(articles);
        }

        public async Task<NewsArticle?> GetArticleByIdAsync(int id)
        {
            var article = await _newsRepository.GetByIdAsync(id);
            if (article == null || article.IsHidden || (article.Category != null && article.Category.IsHidden))
                return null;
            var filteredKeywords = (await _filteredKeywordRepository.GetAllAsync()).Select(k => k.Keyword.ToLower()).ToList();
            if (filteredKeywords.Any(k => article.Title.ToLower().Contains(k) || (article.Description != null && article.Description.ToLower().Contains(k))))
                return null;
            return article;
        }

        public async Task<IEnumerable<NewsArticle>> GetSavedArticlesAsync(int userId)
        {
            var articles = await _newsRepository.GetSavedArticlesByUserAsync(userId);
            return await FilterArticlesAsync(articles);
        }

        public async Task<bool> SaveArticleAsync(int userId, int articleId)
        {
            return await _newsRepository.SaveArticleForUserAsync(userId, articleId);
        }

        public async Task<bool> RemoveSavedArticleAsync(int userId, int articleId)
        {
            return await _newsRepository.RemoveSavedArticleAsync(userId, articleId);
        }

        public async Task<bool> LikeArticleAsync(int userId, int articleId)
        {
            var article = await _newsRepository.GetByIdAsync(articleId);
            if (article == null) return false;
            var existingLike = await _userArticleLikeRepository.GetByUserAndArticleAsync(userId, articleId);
            if (existingLike != null) return false;
            await _userArticleLikeRepository.CreateAsync(new UserArticleLike { UserId = userId, NewsArticleId = articleId, LikedAt = DateTime.UtcNow });
            article.Likes++;
            await _newsRepository.UpdateAsync(article);
            return true;
        }

        public async Task<bool> UnlikeArticleAsync(int userId, int articleId)
        {
            var article = await _newsRepository.GetByIdAsync(articleId);
            if (article == null) return false;
            var deleted = await _userArticleLikeRepository.DeleteAsync(userId, articleId);
            if (!deleted) return false;
            if (article.Likes > 0) article.Likes--;
            await _newsRepository.UpdateAsync(article);
            return true;
        }

        public async Task<bool> MarkArticleAsReadAsync(int userId, int articleId)
        {
            var article = await _newsRepository.GetByIdAsync(articleId);
            if (article == null) return false;
            var existingRead = await _userArticleReadRepository.GetByUserAndArticleAsync(userId, articleId);
            if (existingRead != null) return false;
            await _userArticleReadRepository.CreateAsync(new UserArticleRead { UserId = userId, NewsArticleId = articleId, ReadAt = DateTime.UtcNow });
            return true;
        }

        public async Task<IEnumerable<NewsArticle>> GetRecommendedArticlesAsync(int userId, int count = 10)
        {
            var liked = (await _userArticleLikeRepository.GetByUserAsync(userId)).Select(l => l.NewsArticleId).ToHashSet();
            var read = (await _userArticleReadRepository.GetByUserAsync(userId)).Select(r => r.NewsArticleId).ToHashSet();
            var saved = (await _newsRepository.GetSavedArticlesByUserAsync(userId)).Select(a => a.Id).ToHashSet();

            var userKeywords = await GetUserNotificationKeywordsAsync(userId);

            var allArticles = await _newsRepository.GetAllAsync();
            var filtered = await FilterArticlesAsync(allArticles);

            var scoredArticles = new List<(NewsArticle Article, double Score)>();

            foreach (var article in filtered)
            {
                double score = 0;

                var hoursSincePublished = (DateTime.UtcNow - article.PublishedAt).TotalHours;
                if (hoursSincePublished <= 24)
                    score += 30; 
                else if (hoursSincePublished <= 72)
                    score += 20; 
                else
                    score += Math.Max(0, 15 - (hoursSincePublished / 24));
   
                if (liked.Contains(article.Id))
                    score += 100;

                if (saved.Contains(article.Id))
                    score += 80;

                if (userKeywords.Any() && article.Description != null)
                {
                    var titleLower = article.Title.ToLower();
                    var descriptionLower = article.Description.ToLower();

                    var matchingKeywords = userKeywords
                        .Where(keyword => titleLower.Contains(keyword.ToLower()) || descriptionLower.Contains(keyword.ToLower()))
                        .ToList();

                    if (matchingKeywords.Any())
                    {
                        score += matchingKeywords.Count * 25;
                    }
                }

                var userNotificationSettings = await _notificationService.GetUserNotificationSettingsAsync(userId);
                var enabledCategories = userNotificationSettings
                    .Where(s => s.IsEnabled && s.CategoryId.HasValue)
                    .Select(s => s.CategoryId.Value)
                    .ToHashSet();

                if (enabledCategories.Contains(article.CategoryId))
                    score += 30;

                if (read.Contains(article.Id))
                    score -= 25;

                var popularityScore = article.Likes - article.Dislikes;
                score += Math.Min(Math.Max(popularityScore, 0), 25); 

                if (!string.IsNullOrWhiteSpace(article.Description) && article.Description.Length > 50)
                    score += 10;

                scoredArticles.Add((article, score));
            }

            var recommended = scoredArticles
                .OrderByDescending(x => x.Score)
                .Take(count)
                .Select(x => x.Article)
                .ToList();

            return recommended;
        }

        private async Task<List<string>> GetUserNotificationKeywordsAsync(int userId)
        {
            var keywords = new List<string>();
            var settings = await _notificationService.GetUserNotificationSettingsAsync(userId);

            foreach (var setting in settings.Where(s => s.IsEnabled && !string.IsNullOrEmpty(s.Keywords)))
            {
                try
                {
                    var settingKeywords = System.Text.Json.JsonSerializer.Deserialize<List<string>>(setting.Keywords);
                    if (settingKeywords != null)
                    {
                        keywords.AddRange(settingKeywords);
                    }
                }
                catch
                {
                    var commaKeywords = setting.Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    keywords.AddRange(commaKeywords);
                }
            }

            return keywords.Distinct().ToList();
        }

        public async Task<bool> ReportArticleAsync(int userId, int articleId, string? reason, int reportThreshold = 3)
        {
            var article = await _newsRepository.GetByIdAsync(articleId);
            if (article == null || article.IsHidden)
                return false;

            var existingReport = await _reportRepository.GetByUserAndArticleAsync(userId, articleId);
            if (existingReport != null)
                return false;

            var report = new Report
            {
                UserId = userId,
                NewsArticleId = articleId,
                Reason = reason,
                CreatedAt = DateTime.UtcNow
            };
            await _reportRepository.CreateAsync(report);

            article.ReportCount = await _reportRepository.GetReportCountForArticleAsync(articleId);
            if (article.ReportCount >= reportThreshold)
            {
                article.IsHidden = true;
                await _notificationService.SendEmailNotificationAsync(1, "Article Hidden Due to Reports", $"Article '{article.Title}' was automatically hidden after {article.ReportCount} reports.", article.Id);
            }
            await _newsRepository.UpdateAsync(article);

            await _notificationService.SendEmailNotificationAsync(1, "Article Reported", $"Article '{article.Title}' was reported by a user. Reason: {reason}", article.Id);

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

                await SendKeywordBasedNotificationsForArticleAsync(article);
            }
        }

        private async Task SendKeywordBasedNotificationsForArticleAsync(NewsArticle article)
        {
            try
            {
                var allUsers = await _userService.GetAllUsersAsync();

                foreach (var user in allUsers)
                {
                    var userKeywords = await GetUserNotificationKeywordsAsync(user.Id);

                    if (userKeywords.Any() && article.Description != null)
                    {
                        var titleLower = article.Title.ToLower();
                        var descriptionLower = article.Description.ToLower();

                        var matchingKeywords = userKeywords
                            .Where(keyword => titleLower.Contains(keyword.ToLower()) || descriptionLower.Contains(keyword.ToLower()))
                            .ToList();

                        if (matchingKeywords.Any())
                        {
                            await _notificationService.SendKeywordBasedNotificationsAsync(user.Id, matchingKeywords, article);
                        }
                    }

                    var userSettings = await _notificationService.GetUserNotificationSettingsAsync(user.Id);
                    var categorySetting = userSettings.FirstOrDefault(s => s.CategoryId == article.CategoryId);

                    if (categorySetting != null && categorySetting.IsEnabled && categorySetting.EmailNotifications)
                    {
                        var title = $"New {article.Category?.Name ?? "Article"} Available: {article.Title}";
                        var message = $"A new article in the {article.Category?.Name ?? "selected"} category has been published: {article.Title}. Read more at: {article.Url}";

                        await _notificationService.SendEmailNotificationAsync(user.Id, title, message, article.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notifications: {ex.Message}");
            }
        }

        private async Task<int> GetCategoryIdForArticleAsync(NewsArticle article)
        {
            var categories = await _categoryRepository.GetAllAsync();
            if (categories == null || !categories.Any())
                throw new InvalidOperationException("No categories available for categorization.");

            var title = article.Title?.ToLower() ?? string.Empty;
            var description = article.Description?.ToLower() ?? string.Empty;
            var combinedText = $"{title} {description}";

            var categoryScores = new Dictionary<int, int>();

            foreach (var category in categories)
            {
                if (!string.IsNullOrWhiteSpace(category.Keywords))
                {
                    var keywords = category.Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    int score = 0;

                    foreach (var keyword in keywords)
                    {
                        var keywordLower = keyword.ToLower().Trim();

                        if (title.Contains(keywordLower))
                            score += 3;

                        if (description.Contains(keywordLower))
                            score += 2;

                        if (combinedText.Contains(keywordLower))
                            score += 1;
                    }

                    if (score > 0)
                        categoryScores[category.Id] = score;
                }
            }

            if (categoryScores.Any())
            {
                return categoryScores.OrderByDescending(x => x.Value).First().Key;
            }

            if (title.Contains("israel") || title.Contains("gaza") || title.Contains("hamas") ||
                title.Contains("netanyahu") || title.Contains("ceasefire") || title.Contains("war") ||
                title.Contains("politics") || title.Contains("government") || title.Contains("election"))
            {
                var politicsCategory = categories.FirstOrDefault(c => c.Name?.ToLower().Contains("politics") == true);
                if (politicsCategory != null) return politicsCategory.Id;
            }

            if (title.Contains("flood") || title.Contains("disaster") || title.Contains("accident") ||
                title.Contains("killed") || title.Contains("death") || title.Contains("missing"))
            {
                var generalCategory = categories.FirstOrDefault(c => c.Name?.ToLower().Contains("general") == true);
                if (generalCategory != null) return generalCategory.Id;
            }

            if (title.Contains("sport") || title.Contains("football") || title.Contains("tennis") ||
                title.Contains("baseball") || title.Contains("wimbledon") || title.Contains("championship"))
            {
                var sportsCategory = categories.FirstOrDefault(c => c.Name?.ToLower().Contains("sport") == true);
                if (sportsCategory != null) return sportsCategory.Id;
            }

            return categories.First().Id;
        }

        public async Task<int> ReCategorizeAllArticlesAsync()
        {
            var articles = await _newsRepository.GetAllAsync();
            int updatedCount = 0;
            foreach (var article in articles)
            {
                int newCategoryId = await GetCategoryIdForArticleAsync(article);
                if (article.CategoryId != newCategoryId)
                {
                    article.CategoryId = newCategoryId;
                    await _newsRepository.UpdateAsync(article);
                    updatedCount++;
                }
            }
            return updatedCount;
        }

        public async Task<int> FixInvalidCategoriesAsync()
        {
            var articles = await _newsRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var validCategoryIds = categories.Select(c => c.Id).ToHashSet();

            int fixedCount = 0;
            foreach (var article in articles)
            {
                if (!validCategoryIds.Contains(article.CategoryId))
                {
                    int newCategoryId = await GetCategoryIdForArticleAsync(article);
                    article.CategoryId = newCategoryId;
                    await _newsRepository.UpdateAsync(article);
                    fixedCount++;
                }
            }
            return fixedCount;
        }

        public async Task<IEnumerable<NewsArticle>> GetLikedArticlesAsync(int userId)
        {
            var likedArticleIds = (await _userArticleLikeRepository.GetByUserAsync(userId)).Select(l => l.NewsArticleId);
            var articles = new List<NewsArticle>();

            foreach (var articleId in likedArticleIds)
            {
                var article = await _newsRepository.GetByIdAsync(articleId);
                if (article != null)
                {
                    articles.Add(article);
                }
            }

            return await FilterArticlesAsync(articles);
        }

        public async Task<IEnumerable<NewsArticle>> GetReadArticlesAsync(int userId)
        {
            var readArticleIds = (await _userArticleReadRepository.GetByUserAsync(userId)).Select(r => r.NewsArticleId);
            var articles = new List<NewsArticle>();

            foreach (var articleId in readArticleIds)
            {
                var article = await _newsRepository.GetByIdAsync(articleId);
                if (article != null)
                {
                    articles.Add(article);
                }
            }

            return await FilterArticlesAsync(articles);
        }

        public async Task<List<string>> GetUserKeywordsAsync(int userId)
        {
            return await GetUserNotificationKeywordsAsync(userId);
        }

        public async Task<IEnumerable<NewsArticle>> GetPersonalizedArticlesByCategoryAsync(int userId, int categoryId, int count = 10)
        {
            var liked = (await _userArticleLikeRepository.GetByUserAsync(userId)).Select(l => l.NewsArticleId).ToHashSet();
            var read = (await _userArticleReadRepository.GetByUserAsync(userId)).Select(r => r.NewsArticleId).ToHashSet();
            var saved = (await _newsRepository.GetSavedArticlesByUserAsync(userId)).Select(a => a.Id).ToHashSet();

            var userKeywords = await GetUserNotificationKeywordsAsync(userId);

            var categoryArticles = await _newsRepository.GetByCategoryAsync(categoryId);
            var filtered = await FilterArticlesAsync(categoryArticles);

            var scoredArticles = new List<(NewsArticle Article, double Score)>();

            foreach (var article in filtered)
            {
                double score = 0;

                var daysSincePublished = (DateTime.UtcNow - article.PublishedAt).TotalDays;
                score += Math.Max(0, 10 - daysSincePublished);


                if (liked.Contains(article.Id))
                    score += 50;

               
                if (saved.Contains(article.Id))
                    score += 40;

                
                if (userKeywords.Any() && article.Description != null)
                {
                    var titleLower = article.Title.ToLower();
                    var descriptionLower = article.Description.ToLower();

                    foreach (var keyword in userKeywords)
                    {
                        if (titleLower.Contains(keyword.ToLower()) || descriptionLower.Contains(keyword.ToLower()))
                        {
                            score += 30;
                            break;
                        }
                    }
                }

             
                if (read.Contains(article.Id))
                    score -= 10;

                score += Math.Min(article.Likes, 20);

                scoredArticles.Add((article, score));
            }

          
            return scoredArticles
                .OrderByDescending(x => x.Score)
                .Take(count)
                .Select(x => x.Article);
        }
    }
}