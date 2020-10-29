using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class V2020_09_04_1459 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImpersonatorTenantId",
                schema: "dbo",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "dbo",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "TenantName",
                schema: "dbo",
                table: "Log");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImpersonatorTenantId",
                schema: "dbo",
                table: "Log",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "dbo",
                table: "Log",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantName",
                schema: "dbo",
                table: "Log",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
