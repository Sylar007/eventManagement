using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomSlugPropToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomSlug",
                table: "Events",
                type: "varchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_CustomSlug",
                table: "Events",
                column: "CustomSlug",
                unique: true,
                filter: "'CustomSlug' IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_CustomSlug",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CustomSlug",
                table: "Events");
        }
    }
}
