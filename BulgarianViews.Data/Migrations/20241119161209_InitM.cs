using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulgarianViews.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationPostTag");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("0c8501ec-1896-41e9-81dc-e317d5d40c2d"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("5b265e64-5065-47e1-8ef7-1d7819c8c348"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("a0a542dc-bb6a-4ed5-a52c-8a3fac4ae440"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("ffa935f5-6d89-4f70-97b2-4c10b4b3c491"));

            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "LocationPosts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("92e8d722-b13e-4792-b812-e03740c7742c"), "Mountain" },
                    { new Guid("9c3a0a7c-584d-45c0-8b4f-1c28a506817d"), "Sea" },
                    { new Guid("bc4fc902-5ea1-4614-9030-8cda6cedbfc4"), "City" },
                    { new Guid("ce8cf257-9010-48ea-9308-494d8f57ac16"), "Village" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationPosts_TagId",
                table: "LocationPosts",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationPosts_Tags_TagId",
                table: "LocationPosts",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationPosts_Tags_TagId",
                table: "LocationPosts");

            migrationBuilder.DropIndex(
                name: "IX_LocationPosts_TagId",
                table: "LocationPosts");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("92e8d722-b13e-4792-b812-e03740c7742c"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("9c3a0a7c-584d-45c0-8b4f-1c28a506817d"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("bc4fc902-5ea1-4614-9030-8cda6cedbfc4"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("ce8cf257-9010-48ea-9308-494d8f57ac16"));

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "LocationPosts");

            migrationBuilder.CreateTable(
                name: "LocationPostTag",
                columns: table => new
                {
                    LocationPostsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationPostTag", x => new { x.LocationPostsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_LocationPostTag_LocationPosts_LocationPostsId",
                        column: x => x.LocationPostsId,
                        principalTable: "LocationPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationPostTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("0c8501ec-1896-41e9-81dc-e317d5d40c2d"), "City" },
                    { new Guid("5b265e64-5065-47e1-8ef7-1d7819c8c348"), "Mountain" },
                    { new Guid("a0a542dc-bb6a-4ed5-a52c-8a3fac4ae440"), "Village" },
                    { new Guid("ffa935f5-6d89-4f70-97b2-4c10b4b3c491"), "Sea" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationPostTag_TagsId",
                table: "LocationPostTag",
                column: "TagsId");
        }
    }
}
