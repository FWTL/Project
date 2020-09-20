using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FWTL.Auth.Database.Migrations
{
    public partial class Alter_TelegramAccount_Change_PK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TelegramAccount",
                table: "TelegramAccount");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TelegramAccount",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "TelegramAccount",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelegramAccount",
                table: "TelegramAccount",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TelegramAccount_UserId",
                table: "TelegramAccount",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TelegramAccount",
                table: "TelegramAccount");

            migrationBuilder.DropIndex(
                name: "IX_TelegramAccount_UserId",
                table: "TelegramAccount");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "TelegramAccount");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TelegramAccount",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TelegramAccount",
                table: "TelegramAccount",
                columns: new[] { "UserId", "Id" });
        }
    }
}
