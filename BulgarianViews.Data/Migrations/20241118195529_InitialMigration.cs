using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulgarianViews.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_AspNetUserTokens_AspNetUsers_UserId",
               table: "AspNetUserTokens");
            migrationBuilder.DropForeignKey(name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");
            migrationBuilder.DropPrimaryKey(name: "PK_AspNetUsers", table: "AspNetUsers");
            migrationBuilder.DropPrimaryKey(name: "PK_AspNetUserTokens", table: "AspNetUserTokens");
            migrationBuilder.DropPrimaryKey(name: "PK_AspNetUserRoles", table: "AspNetUserRoles");
            migrationBuilder.DropPrimaryKey(name: "PK_AspNetRoles", table: "AspNetRoles");


            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AspNetUserTokens",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureURL",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "AspNetUserRoles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AspNetUserRoles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AspNetUserLogins",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "AspNetUserClaims",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetRoles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");




            


            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PhotoURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationPosts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationPosts_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_LocationPosts_LocationPostId",
                        column: x => x.LocationPostId,
                        principalTable: "LocationPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteViews",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteViews", x => new { x.UserId, x.LocationId });
                    table.ForeignKey(
                        name: "FK_FavoriteViews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteViews_LocationPosts_LocationId",
                        column: x => x.LocationId,
                        principalTable: "LocationPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    { new Guid("4b06585f-58a3-4a3d-a7f3-de777bd91847"), "Mountain" },
                    { new Guid("93788e4e-dbe0-4584-b0cd-e7afa7525f2f"), "Village" },
                    { new Guid("aab8e790-fe74-4794-9a12-eea1e2b1c094"), "Sea" },
                    { new Guid("d973f78b-46e5-4a78-a1a3-0930f717b686"), "City" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_LocationPostId",
                table: "Comments",
                column: "LocationPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteViews_LocationId",
                table: "FavoriteViews",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationPosts_LocationId",
                table: "LocationPosts",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationPosts_UserId",
                table: "LocationPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationPostTag_TagsId",
                table: "LocationPostTag",
                column: "TagsId");



            migrationBuilder.AddPrimaryKey(name: "PK_AspNetRoles", table: "AspNetRoles", column: "Id");
            migrationBuilder.AddPrimaryKey(name: "PK_AspNetUserRoles", table: "AspNetUserRoles", columns: new[] { "UserId", "RoleId" });
            migrationBuilder.AddPrimaryKey(name: "PK_AspNetUserTokens", table: "AspNetUserTokens", columns: new[] { "UserId", "LoginProvider", "Name" });
            migrationBuilder.AddPrimaryKey(name: "PK_AspNetUsers", table: "AspNetUsers", column: "Id");
            migrationBuilder.AddForeignKey(name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims", column: "RoleId", principalTable: "AspNetRoles", principalColumn: "Id");
            migrationBuilder.AddForeignKey(name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles", column: "RoleId", principalTable: "AspNetRoles", principalColumn: "Id");
            migrationBuilder.AddForeignKey(name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims", column: "UserId", principalTable: "AspNetUsers", principalColumn: "Id");
            migrationBuilder.AddForeignKey(name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins", column: "UserId", principalTable: "AspNetUsers", principalColumn: "Id");
            migrationBuilder.AddForeignKey(name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles", column: "UserId", principalTable: "AspNetUsers", principalColumn: "Id");
            migrationBuilder.AddForeignKey(name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens", column: "UserId", principalTable: "AspNetUsers", principalColumn: "Id");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "FavoriteViews");

            migrationBuilder.DropTable(
                name: "LocationPostTag");

            migrationBuilder.DropTable(
                name: "LocationPosts");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePictureURL",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetUserRoles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserRoles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserClaims",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetRoles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
