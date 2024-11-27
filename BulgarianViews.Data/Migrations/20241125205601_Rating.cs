using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulgarianViews.Data.Migrations
{
    /// <inheritdoc />
    public partial class Rating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("122a2ce2-c29c-40a1-a0ae-863afffb3764"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("90c37c76-b79e-4a57-97b0-23743e61cd68"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("94f88543-6fc1-4947-9e83-1cfd9dc097dc"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("d8db7b2c-2edb-4637-bde5-4b4c78ccb31b"));

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "LocationPosts",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0559d9bb-af50-4013-9ec4-1e4ef11adecf"), "Village" },
                    { new Guid("1e11d6c6-e915-473e-b771-2717106c2f12"), "Sea" },
                    { new Guid("8699da16-c06e-4895-9b82-bb85c7660464"), "City" },
                    { new Guid("d492b936-eb19-47eb-a8e5-d4b01247fe72"), "Mountain" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("0559d9bb-af50-4013-9ec4-1e4ef11adecf"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("1e11d6c6-e915-473e-b771-2717106c2f12"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("8699da16-c06e-4895-9b82-bb85c7660464"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("d492b936-eb19-47eb-a8e5-d4b01247fe72"));

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "LocationPosts");

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("122a2ce2-c29c-40a1-a0ae-863afffb3764"), "City" },
                    { new Guid("90c37c76-b79e-4a57-97b0-23743e61cd68"), "Mountain" },
                    { new Guid("94f88543-6fc1-4947-9e83-1cfd9dc097dc"), "Village" },
                    { new Guid("d8db7b2c-2edb-4637-bde5-4b4c78ccb31b"), "Sea" }
                });
        }
    }
}
