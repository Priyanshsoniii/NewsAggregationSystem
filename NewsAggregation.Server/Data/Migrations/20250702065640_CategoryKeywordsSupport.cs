using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggregation.Server.Migrations
{
    /// <inheritdoc />
    public partial class CategoryKeywordsSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Keywords",
                table: "Categories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Keywords" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9060), null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Keywords" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9065), null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Keywords" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9067), null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Keywords" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9068), null });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Keywords" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9070), null });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9203), new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9203) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9207), new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9207) });

            migrationBuilder.UpdateData(
                table: "ExternalServers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9209), new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9210) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 7, 2, 6, 56, 40, 239, DateTimeKind.Utc).AddTicks(9232), "$2a$11$v5QBe/Tk6YmM/6CFowR3KeMAOimdJwX3aBC2y.jied7ZxBdkiEs42" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Keywords",
                table: "Categories");

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
        }
    }
}
