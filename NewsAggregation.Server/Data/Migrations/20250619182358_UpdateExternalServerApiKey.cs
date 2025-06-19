using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggregation.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExternalServerApiKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5153));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5158));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5159));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5160));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5161));

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5343), new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5343) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApiKey", "CreatedAt", "LastHourReset" },
                values: new object[] { "2iEVsACWuYsI8wRG8VrwA972129RYibJJRBw0bzG", new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5347), new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5347) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5349), new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5349) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5373), "$2a$11$Y.RS2UClJKMlQE9mJ9bvYelRgi9DJQNfLGnh//QFpJ8dvihTJxoSa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3731));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3736));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3737));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3739));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3740));

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3886), new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3886) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ApiKey", "CreatedAt", "LastHourReset" },
                values: new object[] { "YOUR_THE_NEWS_API_KEY", new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3890), new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3890) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3892), new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3892) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 6, 19, 8, 1, 12, 492, DateTimeKind.Utc).AddTicks(3920), "$2a$11$mpgBmkGjrFcFY2nyNksR/.a.MwQqn017HF60IEZq0u7VHngQ8hT82" });
        }
    }
}
