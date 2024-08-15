using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChannelFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorMode",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Layout",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Theme",
                table: "Channels");

            migrationBuilder.AddColumn<int>(
                name: "TemplateType",
                table: "Channels",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateType",
                table: "Channels");

            migrationBuilder.AddColumn<int>(
                name: "ColorMode",
                table: "Channels",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Layout",
                table: "Channels",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Theme",
                table: "Channels",
                type: "integer",
                nullable: true);
        }
    }
}
