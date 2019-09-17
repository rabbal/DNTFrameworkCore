using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class V2019_09_16_0105 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImpersonatorTenantId",
                schema: "dbo",
                table: "Log",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImpersonatorUserId",
                schema: "dbo",
                table: "Log",
                maxLength: 256,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImpersonatorTenantId",
                schema: "dbo",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "ImpersonatorUserId",
                schema: "dbo",
                table: "Log");
        }
    }
}
