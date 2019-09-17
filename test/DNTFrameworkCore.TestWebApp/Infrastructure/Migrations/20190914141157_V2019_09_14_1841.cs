using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Migrations
{
    public partial class V2019_09_14_1841 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RowVersion",
                table: "User",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                table: "Task",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                table: "Role",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                table: "Product",
                newName: "Version");

            migrationBuilder.RenameColumn(
                name: "RowVersion",
                table: "Blog",
                newName: "Version");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "dbo",
                table: "Log",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                schema: "dbo",
                table: "Log",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantName",
                schema: "dbo",
                table: "Log",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    Version = table.Column<byte[]>(rowVersion: true, nullable: true),
                    CreationDateTime = table.Column<DateTimeOffset>(nullable: false),
                    CreatorBrowserName = table.Column<string>(maxLength: 1024, nullable: true),
                    CreatorIp = table.Column<string>(maxLength: 256, nullable: true),
                    CreatorUserId = table.Column<long>(nullable: true),
                    ModificationDateTime = table.Column<DateTimeOffset>(nullable: true),
                    ModifierBrowserName = table.Column<string>(maxLength: 1024, nullable: true),
                    ModifierIp = table.Column<string>(maxLength: 256, nullable: true),
                    ModifierUserId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Quantity = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    UnitDiscount = table.Column<decimal>(nullable: false),
                    ProductId = table.Column<long>(nullable: false),
                    InvoiceId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItem_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_InvoiceId",
                table: "InvoiceItem",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_ProductId",
                table: "InvoiceItem",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceItem");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "dbo",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "TenantName",
                schema: "dbo",
                table: "Log");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "User",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Task",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Role",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Product",
                newName: "RowVersion");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Blog",
                newName: "RowVersion");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                schema: "dbo",
                table: "Log",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
