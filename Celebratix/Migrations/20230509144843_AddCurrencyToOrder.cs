using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecondaryMarketOrder",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyId",
                table: "Orders",
                type: "varchar(5)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CurrencyId",
                table: "Orders",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Currencies_CurrencyId",
                table: "Orders",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Currencies_CurrencyId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CurrencyId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "SecondaryMarketOrder",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
