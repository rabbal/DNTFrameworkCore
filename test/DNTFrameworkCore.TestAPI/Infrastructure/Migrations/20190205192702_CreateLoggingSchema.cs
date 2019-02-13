using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
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
                    CreationDateTime = table.Column<DateTimeOffset>(nullable: false),
                    CreatorBrowserName = table.Column<string>(maxLength: 1024, nullable: true),
                    CreatorIp = table.Column<string>(maxLength: 256, nullable: true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    Message = table.Column<string>(nullable: false),
                    Level = table.Column<string>(maxLength: 50, nullable: false),
                    Logger = table.Column<string>(maxLength: 256, nullable: false),
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
                name: "IX_Log_Logger",
                schema: "dbo",
                table: "Log",
                column: "Logger");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Log",
                schema: "dbo");
        }
    }
}
