using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestWebApp.Migrations
{
    public partial class CreateLoggingSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Log",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Message = table.Column<string>(nullable: false),
                    Level = table.Column<string>(maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    LoggerName = table.Column<string>(maxLength: 256, nullable: false),
                    UserBrowserName = table.Column<string>(maxLength: 1024, nullable: true),
                    UserIP = table.Column<string>(maxLength: 256, nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    UserDisplayName = table.Column<string>(maxLength: 50, nullable: true),
                    EventId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Log_Level",
                schema: "dbo",
                table: "Log",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Log_LoggerName",
                schema: "dbo",
                table: "Log",
                column: "LoggerName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log",
                schema: "dbo");
        }
    }
}
