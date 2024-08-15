using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChannelId",
                table: "EventAffiliateCodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Channels_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelEvents",
                columns: table => new
                {
                    ChannelsId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelEvents", x => new { x.ChannelsId, x.EventsId });
                    table.ForeignKey(
                        name: "FK_ChannelEvents_Channels_ChannelsId",
                        column: x => x.ChannelsId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelEvents_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventAffiliateCodes_ChannelId",
                table: "EventAffiliateCodes",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelEvents_EventsId",
                table: "ChannelEvents",
                column: "EventsId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_BusinessId",
                table: "Channels",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_CreatedAt",
                table: "Channels",
                column: "CreatedAt");

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

            migrationBuilder.DropTable(
                name: "ChannelEvents");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_EventAffiliateCodes_ChannelId",
                table: "EventAffiliateCodes");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "EventAffiliateCodes");
        }
    }
}
