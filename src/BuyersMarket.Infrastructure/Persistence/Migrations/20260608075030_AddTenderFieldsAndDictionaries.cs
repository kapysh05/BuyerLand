using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BuyersMarket.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenderFieldsAndDictionaries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Tenders");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tenders",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "BrandId",
                table: "Tenders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Tenders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "Tenders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "DesiredByDate",
                table: "Tenders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredCountry",
                table: "Tenders",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Tenders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceUrl",
                table: "Tenders",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    LogoUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenderAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenderAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenderAttributes_Tenders_TenderId",
                        column: x => x.TenderId,
                        principalTable: "Tenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "LogoUrl", "Name" },
                values: new object[,]
                {
                    { new Guid("22222222-0000-0000-0000-000000000001"), null, "Lacoste" },
                    { new Guid("22222222-0000-0000-0000-000000000002"), null, "Calvin Klein" },
                    { new Guid("22222222-0000-0000-0000-000000000003"), null, "Stussy" },
                    { new Guid("22222222-0000-0000-0000-000000000004"), null, "Nike" },
                    { new Guid("22222222-0000-0000-0000-000000000005"), null, "Adidas" },
                    { new Guid("22222222-0000-0000-0000-000000000006"), null, "Apple" },
                    { new Guid("22222222-0000-0000-0000-000000000007"), null, "Samsung" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "ParentId" },
                values: new object[,]
                {
                    { new Guid("33333333-0000-0000-0000-000000000001"), "Одежда", null },
                    { new Guid("33333333-0000-0000-0000-000000000010"), "Электроника", null },
                    { new Guid("33333333-0000-0000-0000-000000000020"), "Обувь", null }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Code", "Name", "Symbol" },
                values: new object[,]
                {
                    { new Guid("11111111-0000-0000-0000-000000000001"), "KZT", "Казахстанский тенге", "₸" },
                    { new Guid("11111111-0000-0000-0000-000000000002"), "USD", "US Dollar", "$" },
                    { new Guid("11111111-0000-0000-0000-000000000003"), "EUR", "Euro", "€" },
                    { new Guid("11111111-0000-0000-0000-000000000004"), "RUB", "Российский рубль", "₽" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "ParentId" },
                values: new object[,]
                {
                    { new Guid("33333333-0000-0000-0000-000000000002"), "Верх", new Guid("33333333-0000-0000-0000-000000000001") },
                    { new Guid("33333333-0000-0000-0000-000000000011"), "Смартфоны", new Guid("33333333-0000-0000-0000-000000000010") },
                    { new Guid("33333333-0000-0000-0000-000000000012"), "Ноутбуки", new Guid("33333333-0000-0000-0000-000000000010") },
                    { new Guid("33333333-0000-0000-0000-000000000021"), "Кроссовки", new Guid("33333333-0000-0000-0000-000000000020") },
                    { new Guid("33333333-0000-0000-0000-000000000003"), "Футболки", new Guid("33333333-0000-0000-0000-000000000002") },
                    { new Guid("33333333-0000-0000-0000-000000000004"), "Худи", new Guid("33333333-0000-0000-0000-000000000002") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenders_BrandId",
                table: "Tenders",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenders_CategoryId",
                table: "Tenders",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenders_CurrencyId",
                table: "Tenders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Name",
                table: "Brands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenderAttributes_TenderId",
                table: "TenderAttributes",
                column: "TenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenders_Brands_BrandId",
                table: "Tenders",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenders_Categories_CategoryId",
                table: "Tenders",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenders_Currencies_CurrencyId",
                table: "Tenders",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenders_Users_CustomerId",
                table: "Tenders",
                column: "CustomerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenders_Brands_BrandId",
                table: "Tenders");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenders_Categories_CategoryId",
                table: "Tenders");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenders_Currencies_CurrencyId",
                table: "Tenders");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenders_Users_CustomerId",
                table: "Tenders");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "TenderAttributes");

            migrationBuilder.DropIndex(
                name: "IX_Tenders_BrandId",
                table: "Tenders");

            migrationBuilder.DropIndex(
                name: "IX_Tenders_CategoryId",
                table: "Tenders");

            migrationBuilder.DropIndex(
                name: "IX_Tenders_CurrencyId",
                table: "Tenders");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Tenders");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Tenders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Tenders");

            migrationBuilder.DropColumn(
                name: "DesiredByDate",
                table: "Tenders");

            migrationBuilder.DropColumn(
                name: "PreferredCountry",
                table: "Tenders");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Tenders");

            migrationBuilder.DropColumn(
                name: "ReferenceUrl",
                table: "Tenders");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Tenders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Tenders",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");
        }
    }
}
