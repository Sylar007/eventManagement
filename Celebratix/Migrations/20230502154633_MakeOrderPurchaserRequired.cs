using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class MakeOrderPurchaserRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_PurchaserId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "PurchaserId",
                table: "Orders",
                type: "varchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "varchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_PurchaserId",
                table: "Orders",
                column: "PurchaserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_PurchaserId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "PurchaserId",
                table: "Orders",
                type: "varchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_PurchaserId",
                table: "Orders",
                column: "PurchaserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
