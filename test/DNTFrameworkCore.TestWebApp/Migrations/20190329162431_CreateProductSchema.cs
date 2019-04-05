using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestWebApp.Migrations
{
    public partial class CreateProductSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationDateTime = table.Column<DateTimeOffset>(nullable: false),
                    LastModificationDateTime = table.Column<DateTimeOffset>(nullable: true),
                    CreatorIp = table.Column<string>(maxLength: 256, nullable: true),
                    LastModifierIp = table.Column<string>(maxLength: 256, nullable: true),
                    CreatorBrowserName = table.Column<string>(maxLength: 1024, nullable: true),
                    LastModifierBrowserName = table.Column<string>(maxLength: 1024, nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Number = table.Column<string>(maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UIX_Product_Number",
                table: "Product",
                column: "Number",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
