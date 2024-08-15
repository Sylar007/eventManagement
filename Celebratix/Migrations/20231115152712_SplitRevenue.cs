using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class SplitRevenue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "BW");

            migrationBuilder.DeleteData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "MV");

            migrationBuilder.AddColumn<decimal>(
                name: "BaseAmount",
                table: "Orders",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceAmount",
                table: "Orders",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ApplicationAmount",
                table: "Orders",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);

            // Temporarily add PresumedApplicationFee columns to EventTicketTypes and Orders
            migrationBuilder.AddColumn<decimal>(
                name: "PresumedApplicationFee",
                table: "EventTicketTypes",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);
            migrationBuilder.AddColumn<decimal>(
                name: "PresumedApplicationFee2",
                table: "EventTicketTypes",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);
            migrationBuilder.AddColumn<decimal>(
                name: "PresumedApplicationFee",
                table: "Orders",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);

            // Temporarily add IsFree column to EventTicketTypes
            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "EventTicketTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Load Business ApplicationFee for later use
            migrationBuilder.Sql(@"
                UPDATE ""EventTicketTypes""
                SET ""PresumedApplicationFee"" = COALESCE(""ApplicationFeeOverwrite"", (SELECT ""ApplicationFee"" FROM ""Businesses"" WHERE ""Id"" = (SELECT ""BusinessId"" FROM ""Events"" WHERE ""Id"" = ""EventId"")));
            ");
            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""PresumedApplicationFee"" = (SELECT ""PresumedApplicationFee"" FROM ""EventTicketTypes"" WHERE ""Id"" = ""TicketTypeId"");
            ");

            // Determine if ticket free for later use
            migrationBuilder.Sql(@"
                UPDATE ""EventTicketTypes""
                SET ""IsFree"" = TRUE
                WHERE ""Price"" = 0 AND ""ServiceFee"" = 0 AND ""PresumedApplicationFee"" = 0;
            ");

            // Fix ServiceFee of EventTicketTypes and ApplicationFee of Businesses
            migrationBuilder.Sql(@"
                UPDATE ""EventTicketTypes""
                SET ""ServiceFee"" = ""ServiceFee"" - 0.7
                WHERE ""ServiceFee"" != 0 AND ""PresumedApplicationFee"" = 0;
            ");
            migrationBuilder.Sql(@"
                UPDATE ""Businesses""
                SET ""ApplicationFee"" = 0.7
                WHERE ""ApplicationFee"" = 0;
            ");

            // Get PresumedApplicationFee2
            migrationBuilder.Sql(@"
                UPDATE ""EventTicketTypes""
                SET ""PresumedApplicationFee2"" = COALESCE(""ApplicationFeeOverwrite"", (SELECT ""ApplicationFee"" FROM ""Businesses"" WHERE ""Id"" = (SELECT ""BusinessId"" FROM ""Events"" WHERE ""Id"" = ""EventId"")))
                WHERE ""IsFree"" = FALSE;
            ");
            migrationBuilder.Sql(@"
                UPDATE ""EventTicketTypes""
                SET ""PresumedApplicationFee2"" = 0
                WHERE ""IsFree"" = TRUE;
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""ApplicationAmount"" = (SELECT ""PresumedApplicationFee2"" FROM ""EventTicketTypes"" WHERE ""Id"" = ""TicketTypeId"") * ""TicketQuantity""
                WHERE ""MarketplaceListingId"" IS NULL AND ""TicketTransferOfferId"" IS NULL;
            ");
            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""ServiceAmount"" = (SELECT ""ServiceFee"" FROM ""EventTicketTypes"" WHERE ""Id"" = ""TicketTypeId"") * ""TicketQuantity""
                WHERE ""MarketplaceListingId"" IS NULL AND ""TicketTransferOfferId"" IS NULL;
            ");
            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""BaseAmount"" = ""Amount"" - ""ServiceAmount"" - ""ApplicationAmount"";
            ");
            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""ServiceAmount"" = 0,
                ""ApplicationAmount"" = 0,
                ""BaseAmount"" = ""Amount""
                WHERE ""BaseAmount"" < 0;
            ");

            migrationBuilder.DropColumn(
                name: "PresumedApplicationFee",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "PresumedApplicationFee2",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "EventTicketTypes");

            migrationBuilder.DropColumn(
                name: "PresumedApplicationFee",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CN",
                column: "Name",
                value: "China");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "HK",
                column: "Name",
                value: "Hong Kong SAR China");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "TR",
                column: "Name",
                value: "Turkey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Orders",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(@"
                UPDATE ""Orders""
                SET ""Amount"" = ""BaseAmount"" + ""ServiceAmount"" + ""ApplicationAmount"";
            ");

            migrationBuilder.AddColumn<decimal>(
                name: "Revenue",
                table: "Orders",
                type: "numeric(18,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.DropColumn(
                name: "ApplicationAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BaseAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceAmount",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "CN",
                column: "Name",
                value: "China mainland");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "HK",
                column: "Name",
                value: "Hong Kong");

            migrationBuilder.UpdateData(
                table: "Countries",
                keyColumn: "Id",
                keyValue: "TR",
                column: "Name",
                value: "Türkiye");

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "CallingCode", "Enabled", "ISO3", "Name" },
                values: new object[,]
                {
                    { "BW", "+267", false, "BWA", "Botswana" },
                    { "MV", "+960", false, "MDV", "Maldives" }
                });
        }
    }
}
