using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddNameAndDescriptionToAffilateCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EventAffiliateCodes",
                type: "varchar",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "EventAffiliateCodes",
                type: "varchar",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "EventAffiliateCodes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "EventAffiliateCodes");
        }
    }
}
