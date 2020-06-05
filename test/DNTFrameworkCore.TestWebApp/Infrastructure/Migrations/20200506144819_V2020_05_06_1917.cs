using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Migrations
{
    public partial class V2020_05_06_1917 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "LogoImage",
                table: "Blog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoImage",
                table: "Blog");
        }
    }
}
