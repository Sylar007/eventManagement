using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class ChangesForTicketTransferSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActiveMarketplaceListingId",
                table: "Tickets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ActiveTicketTransferOfferId",
                table: "Tickets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyId",
                table: "Orders",
                type: "varchar(5)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(5)");

            migrationBuilder.AddColumn<Guid>(
                name: "TicketTransferOfferId",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TicketTransferOffers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferorId = table.Column<string>(type: "varchar(450)", nullable: true),
                    ReceiverId = table.Column<string>(type: "varchar(450)", nullable: true),
                    TransferredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Code = table.Column<string>(type: "varchar", nullable: false),
                    Cancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketTransferOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketTransferOffers_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TicketTransferOffers_AspNetUsers_TransferorId",
                        column: x => x.TransferorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TicketTransferOffers_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ActiveMarketplaceListingId",
                table: "Tickets",
                column: "ActiveMarketplaceListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_ActiveTicketTransferOfferId",
                table: "Tickets",
                column: "ActiveTicketTransferOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TicketTransferOfferId",
                table: "Orders",
                column: "TicketTransferOfferId",
                unique: true,
                filter: "'TicketTransferOfferId' IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTransferOffers_ReceiverId",
                table: "TicketTransferOffers",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTransferOffers_TicketId",
                table: "TicketTransferOffers",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketTransferOffers_TransferorId",
                table: "TicketTransferOffers",
                column: "TransferorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_TicketTransferOffers_TicketTransferOfferId",
                table: "Orders",
                column: "TicketTransferOfferId",
                principalTable: "TicketTransferOffers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_MarketplaceListings_ActiveMarketplaceListingId",
                table: "Tickets",
                column: "ActiveMarketplaceListingId",
                principalTable: "MarketplaceListings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_TicketTransferOffers_ActiveTicketTransferOfferId",
                table: "Tickets",
                column: "ActiveTicketTransferOfferId",
                principalTable: "TicketTransferOffers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_TicketTransferOffers_TicketTransferOfferId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_MarketplaceListings_ActiveMarketplaceListingId",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_TicketTransferOffers_ActiveTicketTransferOfferId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "TicketTransferOffers");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ActiveMarketplaceListingId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_ActiveTicketTransferOfferId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TicketTransferOfferId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ActiveMarketplaceListingId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ActiveTicketTransferOfferId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketTransferOfferId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyId",
                table: "Orders",
                type: "varchar(5)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldNullable: true);
        }
    }
}
