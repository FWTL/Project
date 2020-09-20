using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FWTL.Auth.Database.Migrations
{
    public partial class Add_Table_Job : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Job",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DialogId = table.Column<string>(maxLength: 50, nullable: false),
                    MaxHistoryId = table.Column<long>(nullable: false),
                    JobStatus = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Job", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelegramAccountJob",
                columns: table => new
                {
                    JobId = table.Column<Guid>(nullable: false),
                    TelegramAccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelegramAccountJob", x => new { x.JobId, x.TelegramAccountId });
                    table.ForeignKey(
                        name: "FK_TelegramAccountJob_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TelegramAccountJob_TelegramAccount_TelegramAccountId",
                        column: x => x.TelegramAccountId,
                        principalTable: "TelegramAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TelegramAccountJob_TelegramAccountId",
                table: "TelegramAccountJob",
                column: "TelegramAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TelegramAccountJob");

            migrationBuilder.DropTable(
                name: "Job");
        }
    }
}
