using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Store.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "ModifiedAt", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), false, null, "Electronics" },
                    { 2, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), false, null, "Books" },
                    { 3, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), false, null, "Clothing" },
                    { 4, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), false, null, "Sports" },
                    { 5, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), false, null, "Toys" },
                    { 6, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), false, null, "Home & Kitchen" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsDeleted", "ModifiedAt", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Ergonomic wireless mouse.", false, null, "Wireless Mouse", 29.99m },
                    { 2, 1, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "RGB mechanical keyboard.", false, null, "Mechanical Keyboard", 89.50m },
                    { 3, 1, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "QHD IPS monitor.", false, null, "27-inch Monitor", 249.00m },
                    { 4, 1, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "7-in-1 USB-C hub.", false, null, "USB-C Hub", 45.00m },
                    { 5, 1, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Portable speaker, 12h battery.", false, null, "Bluetooth Speaker", 59.99m },
                    { 6, 2, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Classic novel.", false, null, "The Great Gatsby", 12.99m },
                    { 7, 2, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Handbook of agile software craftsmanship.", false, null, "Clean Code", 39.99m },
                    { 8, 2, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Science fiction epic.", false, null, "Dune", 15.50m },
                    { 9, 2, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "The big ideas behind reliable systems.", false, null, "Designing Data-Intensive Applications", 49.99m },
                    { 10, 2, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Your journey to mastery.", false, null, "The Pragmatic Programmer", 42.00m },
                    { 11, 3, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "100% cotton crew neck.", false, null, "Cotton T-Shirt", 18.99m },
                    { 12, 3, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Straight-fit jeans.", false, null, "Denim Jeans", 59.00m },
                    { 13, 3, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Waterproof insulated jacket.", false, null, "Winter Jacket", 99.00m },
                    { 14, 3, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Lightweight running shoes.", false, null, "Running Shoes", 74.50m },
                    { 15, 3, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), null, false, null, "Baseball Cap", 14.00m },
                    { 16, 4, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Official size 5 ball.", false, null, "Soccer Ball", 24.99m },
                    { 17, 4, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Non-slip 6mm mat.", false, null, "Yoga Mat", 27.50m },
                    { 18, 4, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Adjustable 20kg set.", false, null, "Dumbbell Set", 120.00m },
                    { 19, 4, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Graphite frame racket.", false, null, "Tennis Racket", 89.00m },
                    { 20, 4, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "Speed rope with bearings.", false, null, "Jump Rope", 9.99m },
                    { 21, 5, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "120-piece brick set.", false, null, "Building Blocks Set", 34.99m },
                    { 22, 5, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "4WD off-road RC car.", false, null, "Remote Control Car", 49.99m },
                    { 23, 5, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "40cm soft teddy bear.", false, null, "Plush Teddy Bear", 16.50m },
                    { 24, 6, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "12-cup drip coffee maker.", false, null, "Coffee Maker", 79.99m },
                    { 25, 6, new DateTime(2024, 1, 15, 8, 0, 0, 0, DateTimeKind.Utc), "28cm induction pan.", false, null, "Non-stick Frying Pan", 28.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
