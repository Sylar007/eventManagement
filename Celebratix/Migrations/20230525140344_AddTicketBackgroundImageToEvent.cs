using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketBackgroundImageToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketBackgroundImageId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_TicketBackgroundImageId",
                table: "Events",
                column: "TicketBackgroundImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Images_TicketBackgroundImageId",
                table: "Events",
                column: "TicketBackgroundImageId",
                principalTable: "Images",
                principalColumn: "StorageIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Images_TicketBackgroundImageId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_TicketBackgroundImageId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TicketBackgroundImageId",
                table: "Events");
        }
    }
}
