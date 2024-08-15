using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class DropExistingChannelEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelEvents");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                 name: "ChannelEvents",
                 columns: table => new
                 {
                     ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                     EventsId = table.Column<int>(type: "integer", nullable: false)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_ChannelEvents", x => new { x.ChannelId, x.EventsId });
                 });
        }
    }
}
