using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class RenameCheckedInTicketsForConsistency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketsSold",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "CheckedInTickets",
                table: "EventTicketTypes",
                newName: "TicketsCheckedIn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TicketsCheckedIn",
                table: "EventTicketTypes",
                newName: "CheckedInTickets");

            migrationBuilder.AddColumn<int>(
                name: "TicketsSold",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
