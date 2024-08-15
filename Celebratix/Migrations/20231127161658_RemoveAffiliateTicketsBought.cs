using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAffiliateTicketsBought : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAffiliateCodes_Channels_ChannelId",
                table: "EventAffiliateCodes");

            migrationBuilder.DropColumn(
                name: "TicketsBought",
                table: "EventAffiliateCodes");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChannelId",
                table: "EventAffiliateCodes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAffiliateCodes_Channels_ChannelId",
                table: "EventAffiliateCodes",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAffiliateCodes_Channels_ChannelId",
                table: "EventAffiliateCodes");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChannelId",
                table: "EventAffiliateCodes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketsBought",
                table: "EventAffiliateCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAffiliateCodes_Channels_ChannelId",
                table: "EventAffiliateCodes",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
