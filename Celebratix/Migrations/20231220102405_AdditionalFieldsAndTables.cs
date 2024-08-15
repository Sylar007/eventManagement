using System;
using EllipticCurve.Utils;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalFieldsAndTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
               name: "AddressLine1",
               table: "Events",
               nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Postcode",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
               name: "Publish",
               table: "Events",
               type: "boolean",
               nullable: false,
               defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TicketImageId",
                table: "EventTicketTypes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "EventTicketTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
               name: "SharedType",
               table: "EventTicketTypes",
               type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HideSoldOut",
                table: "EventTicketTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ColorMode",
                table: "Channels",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Theme",
                table: "Channels",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CustomBackground",
                table: "Channels",
                type: "uuid",
                nullable: true,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.RenameTable(
                    name: "EventAffiliateCodes",
                    newName: "Affiliates");

            migrationBuilder.CreateTable(
                name: "ChannelTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ChannelEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventTicketIds = table.Column<string[]>(type: "text[]", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: true),
                    Group = table.Column<string>(type: "varchar", nullable: true),
                    AddDescription = table.Column<bool>(type: "boolean", nullable: false),
                    OpenByDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
               name: "ChannelTicketsResale",
               columns: table => new
               {
                   Id = table.Column<int>(type: "int", nullable: false)
                       .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                   ChannelTicketsId = table.Column<int>(type: "integer", nullable: false),
                   DescriptionFlag = table.Column<int>(type: "integer", nullable: false),
                   Description = table.Column<string>(type: "varchar", nullable: true),
                   RedirectTo = table.Column<string>(type: "varchar", nullable: true)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_ChannelTicketsResale", x => x.Id);
               });

            migrationBuilder.CreateTable(
                 name: "Trackings",
                 columns: table => new
                 {
                     Id = table.Column<Guid>(type: "uuid", nullable: false),
                     ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                     AffiliateId = table.Column<Guid>(type: "uuid", nullable: false),
                     BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                     Name = table.Column<string>(type: "varchar", nullable: true),
                     CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                     UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                 },
                 constraints: table =>
                 {
                     table.PrimaryKey("PK_Trackings", x => x.Id);
                     table.ForeignKey(
                        name: "FK_Trackings_Channel_Id",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id");
                     table.ForeignKey(
                         name: "FK_Trackings_Affiliate_Id",
                         column: x => x.AffiliateId,
                         principalTable: "Affiliates",
                         principalColumn: "Id");
                 });


            migrationBuilder.CreateTable(
                   name: "Transactions",
                   columns: table => new
                   {
                       Id = table.Column<Guid>(type: "uuid", nullable: false),
                       EventId = table.Column<int>(type: "integer", nullable: true),
                       ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                       BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                       TrackingId = table.Column<Guid>(type: "uuid", nullable: false),
                       FullName = table.Column<string>(type: "varchar", nullable: true),
                       TransactionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                       Email = table.Column<string>(type: "varchar", nullable: true),
                       CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                       UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                   },
                   constraints: table =>
                   {
                       table.PrimaryKey("PK_Transactions", x => x.Id);
                       table.ForeignKey(
                          name: "FK_Transactions_Event_Id",
                          column: x => x.EventId,
                          principalTable: "Events",
                          principalColumn: "Id");
                       table.ForeignKey(
                           name: "FK_Transactions_Channel_Id",
                           column: x => x.ChannelId,
                           principalTable: "Channels",
                           principalColumn: "Id");
                       table.ForeignKey(
                           name: "FK_Transactions_Business_Id",
                           column: x => x.BusinessId,
                           principalTable: "Businesses",
                           principalColumn: "Id");
                       table.ForeignKey(
                           name: "FK_Transactions_Tracking_Id",
                           column: x => x.TrackingId,
                           principalTable: "Trackings",
                           principalColumn: "Id");
                   });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Postcode",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Publish",
                table: "Events");

            migrationBuilder.DropColumn(
               name: "TicketImageId",
               table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "SharedType",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "HideSoldOut",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "ColorMode",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Theme",
                table: "Channels");

            migrationBuilder.DropColumn(
               name: "CustomBackground",
               table: "Channels");

            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Event_Id",
                table: "ChannelEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Ticket_Id",
                table: "EventTicketTypes");

            migrationBuilder.RenameTable(
                   name: "Affiliates",
                   newName: "EventAffiliateCodes");

            migrationBuilder.DropTable(
                name: "ChannelTickets");

            migrationBuilder.DropTable(
                 name: "ChannelTicketsResale");

            migrationBuilder.DropTable(
                   name: "Trackings");

            migrationBuilder.DropTable(
                  name: "Transactions");
        }
    }
}
