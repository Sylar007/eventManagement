using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddPayoutAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayoutAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<string>(type: "varchar(450)", nullable: true),
                    CountryCode = table.Column<string>(type: "varchar(450)", nullable: true),
                    CurrencyCode = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true),
                    FirstName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    AccountName = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    AccountNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    AddressLine1 = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    City = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    PostalCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp", nullable: false),
                    IpAddress = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayoutAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayoutAccounts_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayoutAccounts_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayoutAccounts_Currencies_CurrencyCode",
                        column: x => x.CurrencyCode,
                        principalTable: "Currencies",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayoutAccounts_CountryCode",
                table: "PayoutAccounts",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_PayoutAccounts_CurrencyCode",
                table: "PayoutAccounts",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_PayoutAccounts_OwnerId",
                table: "PayoutAccounts",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayoutAccounts");
        }
    }
}
