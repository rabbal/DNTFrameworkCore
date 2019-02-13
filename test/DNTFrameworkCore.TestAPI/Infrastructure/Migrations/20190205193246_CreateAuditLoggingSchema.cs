using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class CreateAuditLoggingSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<long>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    ServiceName = table.Column<string>(maxLength: 256, nullable: false),
                    MethodName = table.Column<string>(maxLength: 256, nullable: false),
                    Parameters = table.Column<string>(nullable: true),
                    ExecutionDateTime = table.Column<DateTimeOffset>(nullable: false),
                    ExecutionDuration = table.Column<int>(nullable: false),
                    UserIp = table.Column<string>(maxLength: 20, nullable: true),
                    UserBrowserName = table.Column<string>(maxLength: 1024, nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    ImpersonatorUserId = table.Column<long>(nullable: true),
                    ImpersonatorTenantId = table.Column<long>(nullable: true),
                    ExtensionJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog",
                schema: "dbo");
        }
    }
}
