using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCascadeDeletionFromCurrencyAndMarketplaceListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceListings_Currencies_CurrencyId",
                table: "MarketplaceListings");

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
                name: "FK_MarketplaceListings_Currencies_CurrencyId",
                table: "MarketplaceListings");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceListings_Currencies_CurrencyId",
                table: "MarketplaceListings",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
