using Microsoft.EntityFrameworkCore.Migrations;

namespace FWTL.Auth.Database.Migrations
{
    public partial class Alter_Table_Job_Add_Columns_MessagesToProcess_ProcessedMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MessagesToProcess",
                table: "Job",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ProcessedMessages",
                table: "Job",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessagesToProcess",
                table: "Job");

            migrationBuilder.DropColumn(
                name: "ProcessedMessages",
                table: "Job");
        }
    }
}
