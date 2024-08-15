using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class MakeOriginalOrderVisibleOnTicketModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Orders_OrderId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Tickets",
                newName: "OriginalOrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_OrderId",
                table: "Tickets",
                newName: "IX_Tickets_OriginalOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Orders_OriginalOrderId",
                table: "Tickets",
                column: "OriginalOrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Orders_OriginalOrderId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "OriginalOrderId",
                table: "Tickets",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_OriginalOrderId",
                table: "Tickets",
                newName: "IX_Tickets_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Orders_OrderId",
                table: "Tickets",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
