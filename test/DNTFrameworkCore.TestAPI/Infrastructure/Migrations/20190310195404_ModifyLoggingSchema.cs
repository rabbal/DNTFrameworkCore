using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class ModifyLoggingSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Logger",
                schema: "dbo",
                table: "Log",
                newName: "LoggerName");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                schema: "dbo",
                table: "Log",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CreatorIp",
                schema: "dbo",
                table: "Log",
                newName: "UserIP");

            migrationBuilder.RenameColumn(
                name: "CreatorBrowserName",
                schema: "dbo",
                table: "Log",
                newName: "UserBrowserName");

            migrationBuilder.RenameColumn(
                name: "CreationDateTime",
                schema: "dbo",
                table: "Log",
                newName: "Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_Log_Logger",
                schema: "dbo",
                table: "Log",
                newName: "IX_Log_LoggerName");

            migrationBuilder.AddColumn<string>(
                name: "UserDisplayName",
                schema: "dbo",
                table: "Log",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "dbo",
                table: "Log",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserDisplayName",
                schema: "dbo",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "dbo",
                table: "Log");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "dbo",
                table: "Log",
                newName: "CreatorUserId");

            migrationBuilder.RenameColumn(
                name: "UserIP",
                schema: "dbo",
                table: "Log",
                newName: "CreatorIp");

            migrationBuilder.RenameColumn(
                name: "UserBrowserName",
                schema: "dbo",
                table: "Log",
                newName: "CreatorBrowserName");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                schema: "dbo",
                table: "Log",
                newName: "CreationDateTime");

            migrationBuilder.RenameColumn(
                name: "LoggerName",
                schema: "dbo",
                table: "Log",
                newName: "Logger");

            migrationBuilder.RenameIndex(
                name: "IX_Log_LoggerName",
                schema: "dbo",
                table: "Log",
                newName: "IX_Log_Logger");
        }
    }
}
