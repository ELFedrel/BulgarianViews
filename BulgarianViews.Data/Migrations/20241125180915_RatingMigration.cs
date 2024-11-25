using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulgarianViews.Data.Migrations
{
    /// <inheritdoc />
    public partial class RatingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false),
                    LocationPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rating_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rating_LocationPosts_LocationPostId",
                        column: x => x.LocationPostId,
                        principalTable: "LocationPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("86541f03-62e6-4685-b5c4-02482a006414"), "City" },
                    { new Guid("a37299e4-b085-48eb-b1db-c4d2fb432d1f"), "Mountain" },
                    { new Guid("a9c03e00-230c-4d12-8698-0f0b932be421"), "Sea" },
                    { new Guid("fc762a1e-a98c-4473-866c-bf2264281276"), "Village" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rating_LocationPostId",
                table: "Rating",
                column: "LocationPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_UserId",
                table: "Rating",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("86541f03-62e6-4685-b5c4-02482a006414"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("a37299e4-b085-48eb-b1db-c4d2fb432d1f"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("a9c03e00-230c-4d12-8698-0f0b932be421"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("fc762a1e-a98c-4473-866c-bf2264281276"));

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
        }
    }
}
