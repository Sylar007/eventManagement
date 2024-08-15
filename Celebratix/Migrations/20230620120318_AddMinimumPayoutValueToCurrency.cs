using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddMinimumPayoutValueToCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinimumPayoutValue",
                table: "Currencies",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumPayoutValue",
                table: "Currencies");
        }
    }
}
