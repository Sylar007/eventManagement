using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class CreateTemporaryNewChannelEventsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelEvents2",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone",
                        nullable: false,
                        defaultValue: DateTimeOffset.UtcNow),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone",
                        nullable: false,
                        defaultValue: DateTimeOffset.UtcNow)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelEvents2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelEvents2_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelEvents2_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelEvents2_ChannelId",
                table: "ChannelEvents2",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelEvents2_EventId",
                table: "ChannelEvents2",
                column: "EventId");

            migrationBuilder.Sql(
                "INSERT into public.\"ChannelEvents2\" (\"Id\", \"ChannelId\", \"EventId\") " +
                "SELECT uuid_in(md5(random()::text || random()::text)::cstring), \"ChannelId\", \"EventsId\" FROM public.\"ChannelEvents\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelEvents2");
        }
    }
}
