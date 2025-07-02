using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAggregation.Server.Migrations
{
    /// <inheritdoc />
    public partial class AdminFeaturesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                columns: new[] { "CreatedAt", "LastHourReset" },
                values: new object[] { new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5347), new DateTime(2025, 6, 19, 18, 23, 57, 939, DateTimeKind.Utc).AddTicks(5347) });

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
    }
}
