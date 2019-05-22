using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestWebApp.Migrations
{
    public partial class UpgradeToNewVersion1_9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DataProtectionKey",
                schema: "dbo");

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
                name: "IX_ProtectionKey_FriendlyName",
                schema: "dbo",
                table: "ProtectionKey",
                column: "FriendlyName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProtectionKey",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "AuditLog",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Exception = table.Column<string>(nullable: true),
                    ExecutionDateTime = table.Column<DateTimeOffset>(nullable: false),
                    ExecutionDuration = table.Column<int>(nullable: false),
                    ExtensionJson = table.Column<string>(nullable: true),
                    ImpersonatorTenantId = table.Column<long>(nullable: true),
                    ImpersonatorUserId = table.Column<long>(nullable: true),
                    MethodName = table.Column<string>(maxLength: 256, nullable: false),
                    Parameters = table.Column<string>(nullable: true),
                    ReturnValue = table.Column<string>(nullable: true),
                    ServiceName = table.Column<string>(maxLength: 256, nullable: false),
                    TenantId = table.Column<long>(nullable: true),
                    UserBrowserName = table.Column<string>(maxLength: 1024, nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    UserIp = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataProtectionKey",
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
                    table.PrimaryKey("PK_DataProtectionKey", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataProtectionKey_FriendlyName",
                schema: "dbo",
                table: "DataProtectionKey",
                column: "FriendlyName",
                unique: true);
        }
    }
}
