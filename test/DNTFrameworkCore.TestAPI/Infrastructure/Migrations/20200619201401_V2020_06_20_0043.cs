using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class V2020_06_20_0043 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "User",
                maxLength: 128,
                nullable: false,
                defaultValue: "9618BDE3-A0C3-4E88-9E89-36E356170538");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "User",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}
