using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueReqFromTicketTypeLinkCodeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventTicketTypes_LinkCode",
                table: "EventTicketTypes");

            migrationBuilder.CreateIndex(
                name: "IX_EventTicketTypes_LinkCode",
                table: "EventTicketTypes",
                column: "LinkCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventTicketTypes_LinkCode",
                table: "EventTicketTypes");

            migrationBuilder.CreateIndex(
                name: "IX_EventTicketTypes_LinkCode",
                table: "EventTicketTypes",
                column: "LinkCode",
                unique: true,
                filter: "'LinkCode' IS NOT NULL");
        }
    }
}
