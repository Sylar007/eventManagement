using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEventTicketsTableAndCreateChannelTicketTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelTickets");

            migrationBuilder.CreateTable(
                name: "ChannelEventTicketTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChannelEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTicketTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelEventTicketTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelEventTicketTypes_ChannelEvents_ChannelEventId",
                        column: x => x.ChannelEventId,
                        principalTable: "ChannelEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelEventTicketTypes_EventTicketTypes_EventTicketTypeId",
                        column: x => x.EventTicketTypeId,
                        principalTable: "EventTicketTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelEventTicketTypes_ChannelEventId",
                table: "ChannelEventTicketTypes",
                column: "ChannelEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelEventTicketTypes_EventTicketTypeId",
                table: "ChannelEventTicketTypes",
                column: "EventTicketTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelEventTicketTypes");

            migrationBuilder.CreateTable(
                name: "ChannelTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AddDescription = table.Column<bool>(type: "boolean", nullable: false),
                    ChannelEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    EventTicketIds = table.Column<string[]>(type: "text[]", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: true),
                    OpenByDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelTickets", x => x.Id);
                });
        }
    }
}
