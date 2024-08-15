using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddChangesForPaymentFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TicketPurchase_TicketPurchaseId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "TicketPurchase");

            migrationBuilder.RenameColumn(
                name: "TicketPurchaseId",
                table: "Tickets",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_TicketPurchaseId",
                table: "Tickets",
                newName: "IX_Tickets_OrderId");

            migrationBuilder.RenameColumn(
                name: "AvailableTickets",
                table: "EventTicketTypes",
                newName: "MaxTicketsAvailable");

            migrationBuilder.AddColumn<Guid>(
                name: "FulfilledByOrderId",
                table: "MarketplaceListings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReservedAt",
                table: "MarketplaceListings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxTicketsPerPurchase",
                table: "EventTicketTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReservedTickets",
                table: "EventTicketTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicketsSold",
                table: "EventTicketTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TicketsSold",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DecimalPlaces",
                table: "Currencies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    PurchaserId = table.Column<string>(type: "varchar(450)", nullable: true),
                    TicketTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AffiliateCodeId = table.Column<Guid>(type: "uuid", nullable: true),
                    MarketplaceListingId = table.Column<Guid>(type: "uuid", nullable: true),
                    SecondaryMarketOrder = table.Column<bool>(type: "boolean", nullable: false),
                    TicketQuantity = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Vat = table.Column<decimal>(type: "decimal(4,4)", nullable: false),
                    StripePaymentIntentId = table.Column<string>(type: "varchar", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_PurchaserId",
                        column: x => x.PurchaserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_EventAffiliateCodes_AffiliateCodeId",
                        column: x => x.AffiliateCodeId,
                        principalTable: "EventAffiliateCodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_EventTicketTypes_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "EventTicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_MarketplaceListings_MarketplaceListingId",
                        column: x => x.MarketplaceListingId,
                        principalTable: "MarketplaceListings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceListings_FulfilledByOrderId",
                table: "MarketplaceListings",
                column: "FulfilledByOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AffiliateCodeId",
                table: "Orders",
                column: "AffiliateCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MarketplaceListingId",
                table: "Orders",
                column: "MarketplaceListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PurchaserId",
                table: "Orders",
                column: "PurchaserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TicketTypeId",
                table: "Orders",
                column: "TicketTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketplaceListings_Orders_FulfilledByOrderId",
                table: "MarketplaceListings",
                column: "FulfilledByOrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Orders_OrderId",
                table: "Tickets",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MarketplaceListings_Orders_FulfilledByOrderId",
                table: "MarketplaceListings");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Orders_OrderId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_MarketplaceListings_FulfilledByOrderId",
                table: "MarketplaceListings");

            migrationBuilder.DropColumn(
                name: "FulfilledByOrderId",
                table: "MarketplaceListings");

            migrationBuilder.DropColumn(
                name: "ReservedAt",
                table: "MarketplaceListings");

            migrationBuilder.DropColumn(
                name: "MaxTicketsPerPurchase",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "ReservedTickets",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "TicketsSold",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "TicketsSold",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "DecimalPlaces",
                table: "Currencies");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Tickets",
                newName: "TicketPurchaseId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_OrderId",
                table: "Tickets",
                newName: "IX_Tickets_TicketPurchaseId");

            migrationBuilder.RenameColumn(
                name: "MaxTicketsAvailable",
                table: "EventTicketTypes",
                newName: "AvailableTickets");

            migrationBuilder.CreateTable(
                name: "TicketPurchase",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AffiliateCodeId = table.Column<Guid>(type: "uuid", nullable: true),
                    PurchaserId = table.Column<string>(type: "varchar(450)", nullable: true),
                    TicketTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PurchaseStatus = table.Column<int>(type: "int", nullable: false),
                    StripeId = table.Column<string>(type: "varchar", nullable: true),
                    TicketQuantity = table.Column<int>(type: "int", nullable: false),
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
                name: "FK_Tickets_TicketPurchase_TicketPurchaseId",
                table: "Tickets",
                column: "TicketPurchaseId",
                principalTable: "TicketPurchase",
                principalColumn: "Id");
        }
    }
}
