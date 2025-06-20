using Microsoft.EntityFrameworkCore;
using NewsAggregation.Server.Models.Entities;

namespace NewsAggregation.Server.Data
{
    public class NewsAggregationContext : DbContext
    {
        public NewsAggregationContext(DbContextOptions<NewsAggregationContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<NewsArticle> NewsArticles { get; set; }
        public DbSet<SavedArticle> SavedArticles { get; set; }
        public DbSet<ExternalServer> ExternalServers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotificationSetting> UserNotificationSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<NewsArticle>(entity =>
            {
                entity.HasIndex(e => e.Url).IsUnique();
                entity.HasIndex(e => e.PublishedAt);
                entity.HasIndex(e => e.CategoryId);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.NewsArticles)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SavedArticle>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.NewsArticleId }).IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SavedArticles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.NewsArticle)
                    .WithMany(p => p.SavedByUsers)
                    .HasForeignKey(d => d.NewsArticleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CreatedAt);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserNotificationSetting>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.CategoryId }).IsUnique();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.NotificationSettings)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.UserNotificationSettings)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Business", Description = "Business and finance news" },
                new Category { Id = 2, Name = "Entertainment", Description = "Entertainment and celebrity news" },
                new Category { Id = 3, Name = "Sports", Description = "Sports news and updates" },
                new Category { Id = 4, Name = "Technology", Description = "Technology and innovation news" },
                new Category { Id = 5, Name = "General", Description = "General news and current events" }
            );

            modelBuilder.Entity<ExternalServer>().HasData(
                new ExternalServer
                {
                    Id = 1,
                    Name = "NewsAPI",
                    ApiUrl = "https://newsapi.org/v2/top-headlines",
                    ApiKey = "YOUR_NEWS_API_KEY",
                    ServerType = "NewsAPI",
                    IsActive = true
                },
                new ExternalServer
                {
                    Id = 2,
                    Name = "The News API",
                    ApiUrl = "https://api.thenewsapi.com/v1/news/top",
                    ApiKey = "2iEVsACWuYsI8wRG8VrwA972129RYibJJRBw0bzG",
                    ServerType = "TheNewsAPI",
                    IsActive = true
                },
                new ExternalServer
                {
                    Id = 3,
                    Name = "Firebase API",
                    ApiUrl = "https://us-central1-symbolic-gift-98004.cloudfunctions.net/newsapi",
                    ApiKey = "af3ce09176fb4fd3be6fcfd1e000776c",
                    ServerType = "Firebase",
                    IsActive = true
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@newsaggregator.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin"
                }
            );
        }
    }
}
