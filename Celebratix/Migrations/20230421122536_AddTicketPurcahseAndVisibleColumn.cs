using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketPurcahseAndVisibleColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketPurchaseId",
                table: "Tickets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Events",
                type: "varchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TicketPurchase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseStatus = table.Column<int>(type: "int", nullable: false),
                    PurchaserId = table.Column<string>(type: "varchar(450)", nullable: true),
                    TicketTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AffiliateCodeId = table.Column<Guid>(type: "uuid", nullable: true),
                    TicketQuantity = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    StripeId = table.Column<string>(type: "varchar", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketPurchase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketPurchase_AspNetUsers_PurchaserId",
                        column: x => x.PurchaserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TicketPurchase_EventAffiliateCodes_AffiliateCodeId",
                        column: x => x.AffiliateCodeId,
                        principalTable: "EventAffiliateCodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TicketPurchase_EventTicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "EventTicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketPurchaseId",
                table: "Tickets",
                column: "TicketPurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_CreatorId",
                table: "Events",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Visible",
                table: "Events",
                column: "Visible");

            migrationBuilder.CreateIndex(
                name: "IX_TicketPurchase_AffiliateCodeId",
                table: "TicketPurchase",
                column: "AffiliateCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketPurchase_PurchaserId",
                table: "TicketPurchase",
                column: "PurchaserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketPurchase_TicketTypeId",
                table: "TicketPurchase",
                column: "TicketTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_CreatorId",
                table: "Events",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_TicketPurchase_TicketPurchaseId",
                table: "Tickets",
                column: "TicketPurchaseId",
                principalTable: "TicketPurchase",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_CreatorId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TicketPurchase_TicketPurchaseId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "TicketPurchase");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketPurchaseId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Events_CreatorId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_Visible",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TicketPurchaseId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Visible",
                table: "Events");
        }
    }
}
