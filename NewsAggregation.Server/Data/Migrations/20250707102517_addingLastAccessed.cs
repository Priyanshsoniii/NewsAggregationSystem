using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggregation.Server.Migrations
{
    /// <inheritdoc />
    public partial class addingLastAccessed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7244));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7248));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7250));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7251));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7252));

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastAccessed", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7354), new DateTime(2025, 7, 7, 15, 55, 16, 685, DateTimeKind.Local).AddTicks(7361), new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7355) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "LastAccessed", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7374), new DateTime(2025, 7, 7, 15, 55, 16, 685, DateTimeKind.Local).AddTicks(7376), new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7374) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "LastAccessed", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7376), new DateTime(2025, 7, 7, 15, 55, 16, 685, DateTimeKind.Local).AddTicks(7377), new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7377) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Email", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 7, 10, 25, 16, 685, DateTimeKind.Utc).AddTicks(7398), "admin@gmail.com", "$2a$11$Ux0cfZnStc1OTWhWtsGk1u6U8dbFq1cqWCMFctBKG4jFIVqFaCb06" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9060));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9065));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9067));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9068));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9070));

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastAccessed", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9203), null, new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9203) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "LastAccessed", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9207), null, new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9207) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "LastAccessed", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9209), null, new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9210) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Email", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9232), "admin@newsaggregator.com", "$2a$11$v5QBe/Tk6YmM/6CFowR3KeMAOimdJwX3aBC2y.jied7ZxBdkiEs42" });
        }
    }
}
