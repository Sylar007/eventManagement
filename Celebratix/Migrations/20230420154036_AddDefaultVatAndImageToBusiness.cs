using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultVatAndImageToBusiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomVatPercentage",
                table: "EventTicketTypes");

            migrationBuilder.AddColumn<decimal>(
                name: "CustomVat",
                table: "EventTicketTypes",
                type: "decimal(4,4)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultVat",
                table: "Businesses",
                type: "decimal(4,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Events_ImageId",
                table: "Events",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Images_ImageId",
                table: "Events",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "StorageIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Images_ImageId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_ImageId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CustomVat",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "DefaultVat",
                table: "Businesses");

            migrationBuilder.AddColumn<decimal>(
                name: "CustomVatPercentage",
                table: "EventTicketTypes",
                type: "decimal(18,6)",
                nullable: true);
        }
    }
}
