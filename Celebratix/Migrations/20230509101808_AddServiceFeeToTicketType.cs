using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceFeeToTicketType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ServiceFee",
                table: "EventTicketTypes",
                type: "decimal(18,6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceFee",
                table: "EventTicketTypes");
        }
    }
}
