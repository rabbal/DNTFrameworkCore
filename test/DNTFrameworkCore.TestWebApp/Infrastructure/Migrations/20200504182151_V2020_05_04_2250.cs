using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Migrations
{
    public partial class V2020_05_04_2250 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Task");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    NormalizedTitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<byte>(type: "tinyint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UIX_Task_NormalizedTitle",
                table: "Task",
                column: "NormalizedTitle",
                unique: true);
        }
    }
}
