using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddResaleOptionsToChannelEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResaleDisabledDescription",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ResaleEnabled",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "ResaleRedirectUrl",
                table: "Channels");

            migrationBuilder.AddColumn<string>(
                name: "ResaleDisabledDescription",
                table: "ChannelEvents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ResaleEnabled",
                table: "ChannelEvents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResaleRedirectUrl",
                table: "ChannelEvents",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResaleDisabledDescription",
                table: "ChannelEvents");

            migrationBuilder.DropColumn(
                name: "ResaleEnabled",
                table: "ChannelEvents");

            migrationBuilder.DropColumn(
                name: "ResaleRedirectUrl",
                table: "ChannelEvents");

            migrationBuilder.AddColumn<string>(
                name: "ResaleDisabledDescription",
                table: "Channels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ResaleEnabled",
                table: "Channels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ResaleRedirectUrl",
                table: "Channels",
                type: "text",
                nullable: true);
        }
    }
}
