using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggregation.Server.Migrations
{
    /// <inheritdoc />
    public partial class ReportingAndFilteringFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "NewsArticles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReportCount",
                table: "NewsArticles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FilteredKeywords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Keyword = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilteredKeywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NewsArticleId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_NewsArticles_NewsArticleId",
                        column: x => x.NewsArticleId,
                        principalTable: "NewsArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IsHidden" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8269), false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "IsHidden" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8273), false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "IsHidden" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8275), false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "IsHidden" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8276), false });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "IsHidden" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 31, 56, 98, DateTimeKind.Utc).AddTicks(8278), false });

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

            migrationBuilder.CreateIndex(
                name: "IX_FilteredKeywords_Keyword",
                table: "FilteredKeywords",
                column: "Keyword",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_NewsArticleId",
                table: "Reports",
                column: "NewsArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UserId_NewsArticleId",
                table: "Reports",
                columns: new[] { "UserId", "NewsArticleId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilteredKeywords");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "ReportCount",
                table: "NewsArticles");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Categories");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7536));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7541));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7542));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7543));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7544));

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7685), new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7686) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7689), new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7689) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7691), new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7691) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 2, 3, 17, 5, 668, DateTimeKind.Utc).AddTicks(7708), "$2a$11$LmAW.fHekcwfS2JE/5RCT.k4J1rY7L6TePH3btC.zg3MrpN21rj3a" });
        }
    }
}
