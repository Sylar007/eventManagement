using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class TicketLatestOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LatestOrderId",
                table: "Tickets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(@"
                UPDATE ""Tickets""
                SET ""LatestOrderId"" = (SELECT ""Id"" FROM ""Orders"" WHERE ""Id"" IN (SELECT ""OrdersId"" FROM ""OrderTicket"" WHERE ""TicketsId"" = ""Tickets"".""Id"") ORDER BY ""CompletedAt"" DESC FETCH FIRST ROW ONLY);
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_LatestOrderId",
                table: "Tickets",
                column: "LatestOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Orders_LatestOrderId",
                table: "Tickets",
                column: "LatestOrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Orders_LatestOrderId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_LatestOrderId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "LatestOrderId",
                table: "Tickets");
        }
    }
}
