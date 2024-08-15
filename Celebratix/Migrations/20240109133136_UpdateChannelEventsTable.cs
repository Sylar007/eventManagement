using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChannelEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChannelEvents_Channels_ChannelsId",
                table: "ChannelEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelEvents",
                table: "ChannelEvents");

            migrationBuilder.RenameColumn(
                name: "ChannelsId",
                table: "ChannelEvents",
                newName: "ChannelId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChannelId",
                table: "ChannelEvents",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelEvents",
                table: "ChannelEvents",
                columns: new[] { "ChannelId", "EventsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelEvents_Channels_ChannelId",
                table: "ChannelEvents",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "ChannelEvents",
                newName: "ChannelsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelEvents",
                table: "ChannelEvents",
                columns: new[] { "ChannelsId", "EventsId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelEvents_Channels_ChannelsId",
                table: "ChannelEvents",
                column: "ChannelsId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

        }
    }
}
