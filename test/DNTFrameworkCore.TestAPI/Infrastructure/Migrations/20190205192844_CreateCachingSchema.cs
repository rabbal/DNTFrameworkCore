using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class CreateCachingSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cache",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 449, nullable: false),
                    Value = table.Column<byte[]>(nullable: false),
                    ExpiresAtTime = table.Column<DateTimeOffset>(nullable: false),
                    SlidingExpirationInSeconds = table.Column<long>(nullable: true),
                    AbsoluteExpiration = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cache", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cache_ExpiresAtTime",
                schema: "dbo",
                table: "Cache",
                column: "ExpiresAtTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cache",
                schema: "dbo");
        }
    }
}
