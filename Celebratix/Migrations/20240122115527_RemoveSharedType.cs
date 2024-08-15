using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSharedType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SharedType",
                table: "EventTicketTypes");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "EventTicketTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
               name: "AvailableFrom",
               table: "EventTicketTypes",
               type: "timestamp with time zone",
               nullable: false);

            migrationBuilder.AlterColumn<DateTimeOffset>(
               name: "AvailableTo",
               table: "EventTicketTypes",
               type: "timestamp with time zone",
               nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "Layout",
                table: "Channels",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
               name: "Color",
               table: "Channels",
               type: "varchar",
               nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
               name: "SharedType",
               table: "EventTicketTypes",
               type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "EventTicketTypes");

            migrationBuilder.AlterColumn<DateTimeOffset>(
              name: "AvailableFrom",
              table: "EventTicketTypes",
              type: "timestamp with time zone",
              nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
               name: "AvailableTo",
               table: "EventTicketTypes",
               type: "timestamp with time zone",
               nullable: true);

            migrationBuilder.DropColumn(
                name: "Layout",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Channels");
        }
    }
}
