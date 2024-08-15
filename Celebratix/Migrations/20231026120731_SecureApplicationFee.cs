using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class SecureApplicationFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomVat",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "DefaultVat",
                table: "Businesses");

            migrationBuilder.AlterColumn<decimal>(
                name: "ServiceFee",
                table: "EventTicketTypes",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ApplicationFeeOverwrite",
                table: "EventTicketTypes",
                type: "decimal(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ApplicationFee",
                table: "Businesses",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationFeeOverwrite",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "ApplicationFee",
                table: "Businesses");

            migrationBuilder.AlterColumn<decimal>(
                name: "ServiceFee",
                table: "EventTicketTypes",
                type: "decimal(18,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AddColumn<decimal>(
                name: "CustomVat",
                table: "EventTicketTypes",
                type: "decimal(4,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultVat",
                table: "Businesses",
                type: "decimal(4,4)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
