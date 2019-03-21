using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestWebApp.Migrations
{
    public partial class CreateNumberingSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NumberedEntity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityName = table.Column<string>(unicode: false, maxLength: 256, nullable: false),
                    NextNumber = table.Column<long>(nullable: false),
                    TenantId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberedEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UIX_NumberedEntity_EntityName",
                table: "NumberedEntity",
                column: "EntityName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NumberedEntity_TenantId",
                table: "NumberedEntity",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumberedEntity");
        }
    }
}
