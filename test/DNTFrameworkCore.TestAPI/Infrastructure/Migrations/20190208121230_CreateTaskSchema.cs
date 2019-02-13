using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class CreateTaskSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReturnValue",
                schema: "dbo",
                table: "AuditLog",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationDateTime = table.Column<DateTimeOffset>(nullable: false),
                    LastModificationDateTime = table.Column<DateTimeOffset>(nullable: true),
                    CreatorIp = table.Column<string>(maxLength: 256, nullable: true),
                    LastModifierIp = table.Column<string>(maxLength: 256, nullable: true),
                    CreatorBrowserName = table.Column<string>(maxLength: 1024, nullable: true),
                    LastModifierBrowserName = table.Column<string>(maxLength: 1024, nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    Title = table.Column<string>(maxLength: 256, nullable: false),
                    NormalizedTitle = table.Column<string>(maxLength: 256, nullable: false),
                    Number = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    State = table.Column<byte>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "UIX_Task_Number",
                table: "Task",
                column: "Number",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropColumn(
                name: "ReturnValue",
                schema: "dbo",
                table: "AuditLog");
        }
    }
}
