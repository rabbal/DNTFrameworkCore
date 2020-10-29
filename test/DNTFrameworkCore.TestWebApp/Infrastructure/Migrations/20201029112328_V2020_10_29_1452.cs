using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Migrations
{
    public partial class V2020_10_29_1452 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "User");

            migrationBuilder.DropColumn(
                name: "NextNumber",
                table: "NumberedEntity");

            migrationBuilder.AddColumn<string>(
                name: "SecurityToken",
                table: "User",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "NextValue",
                table: "NumberedEntity",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityToken",
                table: "User");

            migrationBuilder.DropColumn(
                name: "NextValue",
                table: "NumberedEntity");

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "User",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "NextNumber",
                table: "NumberedEntity",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
