using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class OrderEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Revenue",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "Revenue",
                table: "EventAffiliateCodes");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(@"
                MERGE INTO ""Orders""
                USING ""EventTicketTypes""
                ON ""Orders"".""TicketTypeId"" = ""EventTicketTypes"".""Id""
                WHEN MATCHED THEN
                    UPDATE SET ""EventId"" = ""EventTicketTypes"".""EventId"";
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_EventId",
                table: "Orders",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Events_EventId",
                table: "Orders",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Events_EventId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_EventId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Orders");

            migrationBuilder.AddColumn<decimal>(
                name: "Revenue",
                table: "EventTicketTypes",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Revenue",
                table: "EventAffiliateCodes",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
