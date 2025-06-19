using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NewsAggregation.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalServers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApiUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApiKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastAccessed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestsPerHour = table.Column<int>(type: "int", nullable: false),
                    CurrentHourRequests = table.Column<int>(type: "int", nullable: false),
                    LastHourReset = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalServers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsArticles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Likes = table.Column<int>(type: "int", nullable: false),
                    Dislikes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsArticles_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserNotificationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Keywords = table.Column<string>(type: "text", nullable: true),
                    EmailNotifications = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotificationSettings_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserNotificationSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelatedArticleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_NewsArticles_RelatedArticleId",
                        column: x => x.RelatedArticleId,
                        principalTable: "NewsArticles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedArticles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NewsArticleId = table.Column<int>(type: "int", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedArticles_NewsArticles_NewsArticleId",
                        column: x => x.NewsArticleId,
                        principalTable: "NewsArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedArticles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3731), "Business and finance news", true, "Business" },
                    { 2, new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3736), "Entertainment and celebrity news", true, "Entertainment" },
                    { 3, new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3737), "Sports news and updates", true, "Sports" },
                    { 4, new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3739), "Technology and innovation news", true, "Technology" },
                    { 5, new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3740), "General news and current events", true, "General" }
                });

            migrationBuilder.InsertData(
                table: "ExternalServers",
                columns: new[] { "Id", "ApiKey", "ApiUrl", "CreatedAt", "CurrentHourRequests", "IsActive", "LastAccessed", "LastHourReset", "Name", "RequestsPerHour", "ServerType" },
                values: new object[,]
                {
                    { 1, "YOUR_NEWS_API_KEY", "https://newsapi.org/v2/top-headlines", new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3886), 0, true, null, new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3886), "NewsAPI", 1000, "NewsAPI" },
                    { 2, "YOUR_THE_NEWS_API_KEY", "https://api.thenewsapi.com/v1/news/top", new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3890), 0, true, null, new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3890), "The News API", 1000, "TheNewsAPI" },
                    { 3, "af3ce09176fb4fd3be6fcfd1e000776c", "https://us-central1-symbolic-gift-98004.cloudfunctions.net/newsapi", new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3892), 0, true, null, new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3892), "Firebase API", 1000, "Firebase" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3920), "admin@newsaggregator.com", true, "$2a$11$mpgBmkGjrFcFY2nyNksR/.a.MwQqn017HF60IEZq0u7VHngQ8hT82", "Admin", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_CategoryId",
                table: "NewsArticles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_PublishedAt",
                table: "NewsArticles",
                column: "PublishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_NewsArticles_Url",
                table: "NewsArticles",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedArticleId",
                table: "Notifications",
                column: "RelatedArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedArticles_NewsArticleId",
                table: "SavedArticles",
                column: "NewsArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedArticles_UserId_NewsArticleId",
                table: "SavedArticles",
                columns: new[] { "UserId", "NewsArticleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationSettings_CategoryId",
                table: "UserNotificationSettings",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationSettings_UserId_CategoryId",
                table: "UserNotificationSettings",
                columns: new[] { "UserId", "CategoryId" },
                unique: true,
                filter: "[CategoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalServers");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "SavedArticles");

            migrationBuilder.DropTable(
                name: "UserNotificationSettings");

            migrationBuilder.DropTable(
                name: "NewsArticles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
