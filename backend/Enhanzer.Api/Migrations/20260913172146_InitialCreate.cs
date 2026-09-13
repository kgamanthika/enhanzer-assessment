using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Enhanzer.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Location_Details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location_Details", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseBills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BatchName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StandardCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StandardPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalSelling = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseBills", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Location_Details_LocationCode",
                table: "Location_Details",
                column: "LocationCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Location_Details");

            migrationBuilder.DropTable(
                name: "PurchaseBills");
        }
    }
}
