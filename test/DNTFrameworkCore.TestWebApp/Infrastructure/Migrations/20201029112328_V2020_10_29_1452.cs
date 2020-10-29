using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Migrations
{
    public partial class V2020_10_29_1452 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("SerialNumber", "User", "SecurityToken");

            migrationBuilder.RenameColumn(name: "NextNumber", table: "NumberedEntity", "NextValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}