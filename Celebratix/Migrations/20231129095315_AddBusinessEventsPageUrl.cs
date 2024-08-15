using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessEventsPageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventsPageUrl",
                table: "Businesses",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventsPageUrl",
                table: "Businesses");
        }
    }
}
