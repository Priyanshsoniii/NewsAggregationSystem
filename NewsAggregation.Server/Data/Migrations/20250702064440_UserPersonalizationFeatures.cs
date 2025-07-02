using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggregation.Server.Migrations
{
    /// <inheritdoc />
    public partial class UserPersonalizationFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserArticleLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NewsArticleId = table.Column<int>(type: "int", nullable: false),
                    LikedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserArticleLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserArticleLikes_NewsArticles_NewsArticleId",
                        column: x => x.NewsArticleId,
                        principalTable: "NewsArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserArticleLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserArticleReads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NewsArticleId = table.Column<int>(type: "int", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserArticleReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserArticleReads_NewsArticles_NewsArticleId",
                        column: x => x.NewsArticleId,
                        principalTable: "NewsArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserArticleReads_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(6800));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(6805));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(6807));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(6808));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(6810));

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(7003), new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(7004) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(7008), new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(7008) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(7010), new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(7010) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 44, 39, 785, DateTimeKind.Utc).AddTicks(7032), "$2a$11$F.Sk.utzFNLlaZnxXH/3JOWtSYt5uzlctdxWEv7rtHMMM230PefEu" });

            migrationBuilder.CreateIndex(
                name: "IX_UserArticleLikes_NewsArticleId",
                table: "UserArticleLikes",
                column: "NewsArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserArticleLikes_UserId_NewsArticleId",
                table: "UserArticleLikes",
                columns: new[] { "UserId", "NewsArticleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserArticleReads_NewsArticleId",
                table: "UserArticleReads",
                column: "NewsArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserArticleReads_UserId_NewsArticleId",
                table: "UserArticleReads",
                columns: new[] { "UserId", "NewsArticleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserArticleLikes");

            migrationBuilder.DropTable(
                name: "UserArticleReads");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8269));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8273));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8275));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8276));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8278));

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8427), new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8427) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8431), new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8431) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8475), new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8475) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8495), "$2a$11$LBqoiEObHfGV7TH/jWdCquiAFXp/WRmTTmxxV0PHp9Jj2aXPvRJ5G" });
        }
    }
}
