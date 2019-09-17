using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class V2019_09_13_2316 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_UserToken_AccessTokenHash",
                table: "UserToken",
                newName: "IX_UserToken_TokenHash");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "dbo",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "TenantName",
                schema: "dbo",
                table: "Log");

            migrationBuilder.RenameIndex(
                name: "IX_UserToken_TokenHash",
                table: "UserToken",
                newName: "IX_UserToken_AccessTokenHash");

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
