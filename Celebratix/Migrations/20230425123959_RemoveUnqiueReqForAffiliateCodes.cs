using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnqiueReqForAffiliateCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventAffiliateCodes_Code",
                table: "EventAffiliateCodes");

            migrationBuilder.CreateIndex(
                name: "IX_EventAffiliateCodes_Code",
                table: "EventAffiliateCodes",
                column: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventAffiliateCodes_Code",
                table: "EventAffiliateCodes");

            migrationBuilder.CreateIndex(
                name: "IX_EventAffiliateCodes_Code",
                table: "EventAffiliateCodes",
                column: "Code",
                unique: true);
        }
    }
}
