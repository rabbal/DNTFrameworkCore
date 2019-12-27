using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class V2019_12_27_1612 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "CreatorBrowserName",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "CreatorIp",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "ModifierBrowserName",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "ModifierIp",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "ModifierUserId",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatorBrowserName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatorIp",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModifierBrowserName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModifierIp",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModifierUserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CreatorBrowserName",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CreatorIp",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ModifierBrowserName",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ModifierIp",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ModifierUserId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "ModifierBrowserName",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "ModifierIp",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "ModifierUserId",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatorBrowserName",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatorIp",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModifierBrowserName",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModifierIp",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModifierUserId",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "ModifierBrowserName",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "ModifierIp",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "ModifierUserId",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "CreatorBrowserName",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "CreatorIp",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "ModifierBrowserName",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "ModifierIp",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "ModifierUserId",
                table: "Blog");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByBrowserName",
                table: "UserToken",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIP",
                table: "UserToken",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "UserToken",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "UserToken",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "UserToken",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByBrowserName",
                table: "UserToken",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByIP",
                table: "UserToken",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedByUserId",
                table: "UserToken",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "UserToken",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByBrowserName",
                table: "UserRole",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIP",
                table: "UserRole",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "UserRole",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "UserRole",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "UserRole",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByBrowserName",
                table: "UserRole",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByIP",
                table: "UserRole",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedByUserId",
                table: "UserRole",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "UserRole",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByBrowserName",
                table: "UserClaim",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIP",
                table: "UserClaim",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "UserClaim",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "UserClaim",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "UserClaim",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByBrowserName",
                table: "UserClaim",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByIP",
                table: "UserClaim",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedByUserId",
                table: "UserClaim",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "UserClaim",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastLoggedInDateTime",
                table: "User",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByBrowserName",
                table: "User",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIP",
                table: "User",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "User",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "User",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByBrowserName",
                table: "User",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByIP",
                table: "User",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedByUserId",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByBrowserName",
                table: "Task",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIP",
                table: "Task",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Task",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Task",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByBrowserName",
                table: "Task",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByIP",
                table: "Task",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedByUserId",
                table: "Task",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "Task",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByBrowserName",
                table: "RoleClaim",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIP",
                table: "RoleClaim",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "RoleClaim",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "RoleClaim",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "RoleClaim",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByBrowserName",
                table: "RoleClaim",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByIP",
                table: "RoleClaim",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedByUserId",
                table: "RoleClaim",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "RoleClaim",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByBrowserName",
                table: "Role",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIP",
                table: "Role",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Role",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Role",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Role",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByBrowserName",
                table: "Role",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByIP",
                table: "Role",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedByUserId",
                table: "Role",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "Role",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByBrowserName",
                table: "Permission",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIP",
                table: "Permission",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Permission",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Permission",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Permission",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByBrowserName",
                table: "Permission",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByIP",
                table: "Permission",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedByUserId",
                table: "Permission",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "Permission",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByBrowserName",
                table: "Blog",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByIP",
                table: "Blog",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatedByUserId",
                table: "Blog",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Blog",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "Blog",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByBrowserName",
                table: "Blog",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByIP",
                table: "Blog",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedByUserId",
                table: "Blog",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "Blog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByBrowserName",
                table: "UserToken");

            migrationBuilder.DropColumn(
                name: "CreatedByIP",
                table: "UserToken");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "UserToken");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "UserToken");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "UserToken");

            migrationBuilder.DropColumn(
                name: "ModifiedByBrowserName",
                table: "UserToken");

            migrationBuilder.DropColumn(
                name: "ModifiedByIP",
                table: "UserToken");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "UserToken");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "UserToken");

            migrationBuilder.DropColumn(
                name: "CreatedByBrowserName",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "CreatedByIP",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "ModifiedByBrowserName",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "ModifiedByIP",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "CreatedByBrowserName",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "CreatedByIP",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "ModifiedByBrowserName",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "ModifiedByIP",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "CreatedByBrowserName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedByIP",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModifiedByBrowserName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModifiedByIP",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "CreatedByBrowserName",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CreatedByIP",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ModifiedByBrowserName",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ModifiedByIP",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Task");

            migrationBuilder.DropColumn(
                name: "CreatedByBrowserName",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "CreatedByIP",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "ModifiedByBrowserName",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "ModifiedByIP",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "CreatedByBrowserName",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatedByIP",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModifiedByBrowserName",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModifiedByIP",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatedByBrowserName",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "CreatedByIP",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "ModifiedByBrowserName",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "ModifiedByIP",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "CreatedByBrowserName",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "CreatedByIP",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "Hash",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "ModifiedByBrowserName",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "ModifiedByIP",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "ModifiedByUserId",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Blog");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "UserRole",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorBrowserName",
                table: "UserRole",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorIp",
                table: "UserRole",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "UserRole",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDateTime",
                table: "UserClaim",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierBrowserName",
                table: "UserClaim",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierIp",
                table: "UserClaim",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifierUserId",
                table: "UserClaim",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastLoggedInDateTime",
                table: "User",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorBrowserName",
                table: "User",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorIp",
                table: "User",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "User",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDateTime",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierBrowserName",
                table: "User",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierIp",
                table: "User",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifierUserId",
                table: "User",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "Task",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorBrowserName",
                table: "Task",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorIp",
                table: "Task",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "Task",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDateTime",
                table: "Task",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierBrowserName",
                table: "Task",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierIp",
                table: "Task",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifierUserId",
                table: "Task",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDateTime",
                table: "RoleClaim",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierBrowserName",
                table: "RoleClaim",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierIp",
                table: "RoleClaim",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifierUserId",
                table: "RoleClaim",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "Role",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorBrowserName",
                table: "Role",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorIp",
                table: "Role",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "Role",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDateTime",
                table: "Role",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierBrowserName",
                table: "Role",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierIp",
                table: "Role",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifierUserId",
                table: "Role",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDateTime",
                table: "Permission",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierBrowserName",
                table: "Permission",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierIp",
                table: "Permission",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifierUserId",
                table: "Permission",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "Blog",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatorBrowserName",
                table: "Blog",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorIp",
                table: "Blog",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "Blog",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDateTime",
                table: "Blog",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierBrowserName",
                table: "Blog",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifierIp",
                table: "Blog",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifierUserId",
                table: "Blog",
                type: "bigint",
                nullable: true);
        }
    }
}
