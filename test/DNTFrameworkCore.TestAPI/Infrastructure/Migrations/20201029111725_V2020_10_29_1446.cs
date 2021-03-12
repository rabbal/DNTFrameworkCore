using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class V2020_10_29_1446 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("SecurityStamp", "User", "SecurityToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}