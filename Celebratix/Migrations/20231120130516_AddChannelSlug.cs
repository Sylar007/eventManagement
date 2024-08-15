using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Celebratix.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAffiliateCodes_Channels_ChannelId",
                table: "EventAffiliateCodes");

            migrationBuilder.DropIndex(
                name: "IX_EventAffiliateCodes_Code",
                table: "EventAffiliateCodes");

            migrationBuilder.DropIndex(
                name: "IX_Channels_CreatedAt",
                table: "Channels");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChannelId",
                table: "EventAffiliateCodes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Channels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                UPDATE ""Channels""
                SET ""Slug"" = md5(random()::text)
                WHERE ""Slug"" = ''
            ");

            migrationBuilder.CreateIndex(
                name: "IX_EventAffiliateCodes_Code",
                table: "EventAffiliateCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Slug",
                table: "Channels",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAffiliateCodes_Channels_ChannelId",
                table: "EventAffiliateCodes",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAffiliateCodes_Channels_ChannelId",
                table: "EventAffiliateCodes");

            migrationBuilder.DropIndex(
                name: "IX_EventAffiliateCodes_Code",
                table: "EventAffiliateCodes");

            migrationBuilder.DropIndex(
                name: "IX_Channels_Slug",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Channels");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChannelId",
                table: "EventAffiliateCodes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_EventAffiliateCodes_Code",
                table: "EventAffiliateCodes",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_CreatedAt",
                table: "Channels",
                column: "CreatedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAffiliateCodes_Channels_ChannelId",
                table: "EventAffiliateCodes",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id");
        }
    }
}
