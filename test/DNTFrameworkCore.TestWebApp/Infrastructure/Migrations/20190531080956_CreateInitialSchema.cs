using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Migrations
{
    public partial class CreateInitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

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

            migrationBuilder.CreateTable(
                name: "Log",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Message = table.Column<string>(nullable: false),
                    Level = table.Column<string>(maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    LoggerName = table.Column<string>(maxLength: 256, nullable: false),
                    UserBrowserName = table.Column<string>(maxLength: 1024, nullable: true),
                    UserIP = table.Column<string>(maxLength: 256, nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    UserDisplayName = table.Column<string>(maxLength: 50, nullable: true),
                    EventId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProtectionKey",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FriendlyName = table.Column<string>(nullable: false),
                    XmlValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtectionKey", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_Cache_ExpiresAtTime",
                schema: "dbo",
                table: "Cache",
                column: "ExpiresAtTime");

            migrationBuilder.CreateIndex(
                name: "IX_Log_Level",
                schema: "dbo",
                table: "Log",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Log_LoggerName",
                schema: "dbo",
                table: "Log",
                column: "LoggerName");

            migrationBuilder.CreateIndex(
                name: "IX_ProtectionKey_FriendlyName",
                schema: "dbo",
                table: "ProtectionKey",
                column: "FriendlyName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumberedEntity");

            migrationBuilder.DropTable(
                name: "Cache",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Log",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ProtectionKey",
                schema: "dbo");
        }
    }
}
