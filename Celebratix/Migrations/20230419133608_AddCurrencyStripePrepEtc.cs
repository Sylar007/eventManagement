using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyStripePrepEtc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessOwner",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyId",
                table: "MarketplaceListings",
                type: "varchar(5)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyId",
                table: "Events",
                type: "varchar(5)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OpenDate",
                table: "Events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "AspNetUsers",
                type: "varchar",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Code);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceListings_CurrencyId",
                table: "MarketplaceListings",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CurrencyId",
                table: "Events",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Currencies_CurrencyId",
                table: "Events",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceListings_Currencies_CurrencyId",
                table: "MarketplaceListings",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Currencies_CurrencyId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceListings_Currencies_CurrencyId",
                table: "MarketplaceListings");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceListings_CurrencyId",
                table: "MarketplaceListings");

            migrationBuilder.DropIndex(
                name: "IX_Events_CurrencyId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "MarketplaceListings");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "OpenDate",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "BusinessOwner",
                table: "AspNetUsers",
                type: "boolean",
                nullable: true);
        }
    }
}
