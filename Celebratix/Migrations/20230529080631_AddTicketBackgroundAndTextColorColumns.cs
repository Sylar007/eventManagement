using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketBackgroundAndTextColorColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TicketBackgroundColor",
                table: "Events",
                type: "varchar",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketTextColor",
                table: "Events",
                type: "varchar",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketBackgroundColor",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TicketTextColor",
                table: "Events");
        }
    }
}
