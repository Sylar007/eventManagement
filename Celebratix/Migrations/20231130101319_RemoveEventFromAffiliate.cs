using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEventFromAffiliate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAffiliateCodes_Channels_ChannelId",
                table: "EventAffiliateCodes");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAffiliateCodes_Events_EventId",
                table: "EventAffiliateCodes");

            migrationBuilder.DropIndex(
                name: "IX_EventAffiliateCodes_EventId",
                table: "EventAffiliateCodes");

            migrationBuilder.DropColumn(
                name: "EventId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_EventAffiliateCodes_Channels_ChannelId",
                table: "EventAffiliateCodes",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "EventAffiliateCodes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EventAffiliateCodes_EventId",
                table: "EventAffiliateCodes",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAffiliateCodes_Channels_ChannelId",
                table: "EventAffiliateCodes",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAffiliateCodes_Events_EventId",
                table: "EventAffiliateCodes",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
