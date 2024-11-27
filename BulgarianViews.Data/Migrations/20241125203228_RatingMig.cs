using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulgarianViews.Data.Migrations
{
    /// <inheritdoc />
    public partial class RatingMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_AspNetUsers_UserId",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_LocationPosts_LocationPostId",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rating",
                table: "Rating");

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

            migrationBuilder.RenameTable(
                name: "Rating",
                newName: "Ratings");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_UserId",
                table: "Ratings",
                newName: "IX_Ratings_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_LocationPostId",
                table: "Ratings",
                newName: "IX_Ratings_LocationPostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_LocationPosts_LocationPostId",
                table: "Ratings",
                column: "LocationPostId",
                principalTable: "LocationPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_LocationPosts_LocationPostId",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

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

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "Rating");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_UserId",
                table: "Rating",
                newName: "IX_Rating_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_LocationPostId",
                table: "Rating",
                newName: "IX_Rating_LocationPostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rating",
                table: "Rating",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_AspNetUsers_UserId",
                table: "Rating",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_LocationPosts_LocationPostId",
                table: "Rating",
                column: "LocationPostId",
                principalTable: "LocationPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
