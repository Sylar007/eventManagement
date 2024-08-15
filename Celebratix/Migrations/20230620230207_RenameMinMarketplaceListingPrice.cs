using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class RenameMinMarketplaceListingPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinimumPayoutValue",
                table: "Currencies",
                newName: "MinMarketplaceListingPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinMarketplaceListingPrice",
                table: "Currencies",
                newName: "MinimumPayoutValue");
        }
    }
}
