using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class RenameProtectionKeyColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "XmlData",
                schema: "dbo",
                table: "DataProtectionKey",
                newName: "XmlValue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "XmlValue",
                schema: "dbo",
                table: "DataProtectionKey",
                newName: "XmlData");
        }
    }
}
