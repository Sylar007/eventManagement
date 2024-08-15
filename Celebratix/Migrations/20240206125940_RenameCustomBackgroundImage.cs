using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class RenameCustomBackgroundImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomBackground",
                table: "Channels");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomBackgroundId",
                table: "Channels",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Images_CustomBackgroundId",
                table: "Channels",
                column: "CustomBackgroundId",
                principalTable: "Images",
                principalColumn: "StorageIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Images_CustomBackgroundId",
                table: "Channels");

            migrationBuilder.RenameColumn(
                name: "CustomBackgroundId",
                table: "Channels",
                newName: "CustomBackground");
        }
    }
}
