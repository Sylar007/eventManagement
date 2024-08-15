using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class MakeTicketImagePropertyNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketImageId",
                table: "EventTicketTypes");

            migrationBuilder.AddColumn<Guid>(
                name: "TicketImageId",
                table: "EventTicketTypes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTicketTypes_Images_TicketImageId",
                table: "EventTicketTypes",
                column: "TicketImageId",
                principalTable: "Images",
                principalColumn: "StorageIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketImageId",
                table: "EventTicketTypes");

            migrationBuilder.AddColumn<Guid>(
                name: "TicketImageId",
                table: "EventTicketTypes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventTicketTypes_Images_TicketImageId",
                table: "EventTicketTypes",
                column: "TicketImageId",
                principalTable: "Images",
                principalColumn: "StorageIdentifier");
        }
    }
}
