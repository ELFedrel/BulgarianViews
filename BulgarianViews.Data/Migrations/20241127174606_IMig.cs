using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulgarianViews.Data.Migrations
{
    /// <inheritdoc />
    public partial class IMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("04c672c9-bfaf-4f8d-9e8b-a2ff3b451d54"), "City" },
                    { new Guid("17856507-5a10-4ee6-905a-945ea5088587"), "Village" },
                    { new Guid("22d6fb61-21f9-472c-9706-5abfb146a5a6"), "Sea" },
                    { new Guid("e95abc3b-c68b-4fd8-a221-de2a62802d1e"), "Mountain" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("04c672c9-bfaf-4f8d-9e8b-a2ff3b451d54"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("17856507-5a10-4ee6-905a-945ea5088587"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("22d6fb61-21f9-472c-9706-5abfb146a5a6"));

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: new Guid("e95abc3b-c68b-4fd8-a221-de2a62802d1e"));

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
    }
}
