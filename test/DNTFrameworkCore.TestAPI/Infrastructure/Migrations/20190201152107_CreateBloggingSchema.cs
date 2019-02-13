using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class CreateBloggingSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blog",
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
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    NormalizedTitle = table.Column<string>(maxLength: 50, nullable: false),
                    Url = table.Column<string>(maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "Blog_NormalizedTitle",
                table: "Blog",
                column: "NormalizedTitle",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blog");
        }
    }
}
