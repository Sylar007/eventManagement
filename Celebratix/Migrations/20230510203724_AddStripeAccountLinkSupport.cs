using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddStripeAccountLinkSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StripeConnectAccountId",
                table: "AspNetUsers",
                type: "varchar",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StripePayoutRequirementsFulfilled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StripePayoutRequirementsSubmitted",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeConnectAccountId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StripePayoutRequirementsFulfilled",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StripePayoutRequirementsSubmitted",
                table: "AspNetUsers");
        }
    }
}
