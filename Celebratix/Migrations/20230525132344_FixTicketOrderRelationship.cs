using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class FixTicketOrderRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_PurchaserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_EventTicketTypes_TicketTypeId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_OwnerId",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Tickets",
                type: "varchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "PurchaserId",
                table: "Orders",
                type: "varchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(450)");

            migrationBuilder.CreateTable(
                name: "OrderTicket",
                columns: table => new
                {
                    OrdersId = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTicket", x => new { x.OrdersId, x.TicketsId });
                    table.ForeignKey(
                        name: "FK_OrderTicket_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTicket_Tickets_TicketsId",
                        column: x => x.TicketsId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderTicket_TicketsId",
                table: "OrderTicket",
                column: "TicketsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_PurchaserId",
                table: "Orders",
                column: "PurchaserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_EventTicketTypes_TicketTypeId",
                table: "Orders",
                column: "TicketTypeId",
                principalTable: "EventTicketTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_OwnerId",
                table: "Tickets",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_PurchaserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_EventTicketTypes_TicketTypeId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_OwnerId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "OrderTicket");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Tickets",
                type: "varchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PurchaserId",
                table: "Orders",
                type: "varchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_PurchaserId",
                table: "Orders",
                column: "PurchaserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_EventTicketTypes_TicketTypeId",
                table: "Orders",
                column: "TicketTypeId",
                principalTable: "EventTicketTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_OwnerId",
                table: "Tickets",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
