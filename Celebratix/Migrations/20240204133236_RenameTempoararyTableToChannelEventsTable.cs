using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class RenameTempoararyTableToChannelEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ChannelEvents2",
                newName: "ChannelEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelEvents2",
                table: "ChannelEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ChannelEvents2_Channels_ChannelId",
                table: "ChannelEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_ChannelEvents2_Events_EventId",
                table: "ChannelEvents");

            migrationBuilder.RenameIndex(
                name: "IX_ChannelEvents2_EventId",
                newName: "IX_ChannelEvents_EventId",
                table: "ChannelEvents");

            migrationBuilder.RenameIndex(
                name: "IX_ChannelEvents2_ChannelId",
                newName: "IX_ChannelEventss_ChannelId",
                table: "ChannelEvents");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelEvents",
                table: "ChannelEvents",
                columns: new[] { "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelEvents_Channels_ChannelId",
                table: "ChannelEvents",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelEvents_Events_EventsId",
                table: "ChannelEvents",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ChannelEvents",
                newName: "ChannelEvents2");

        }
    }
}
