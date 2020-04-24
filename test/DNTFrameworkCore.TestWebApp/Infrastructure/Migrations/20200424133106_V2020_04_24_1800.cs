using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Migrations
{
    public partial class V2020_04_24_1800 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                schema: "dbo",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "ModificationDateTime",
                table: "Blog");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                table: "UserRole",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatorIp",
                table: "UserRole",
                newName: "CreatedByIP");

            migrationBuilder.RenameColumn(
                name: "CreatorBrowserName",
                table: "UserRole",
                newName: "CreatedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifierUserId",
                table: "UserClaim",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifierIp",
                table: "UserClaim",
                newName: "ModifiedByIP");

            migrationBuilder.RenameColumn(
                name: "ModifierBrowserName",
                table: "UserClaim",
                newName: "ModifiedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifierUserId",
                table: "User",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifierIp",
                table: "User",
                newName: "ModifiedByIP");

            migrationBuilder.RenameColumn(
                name: "ModifierBrowserName",
                table: "User",
                newName: "ModifiedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                table: "User",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatorIp",
                table: "User",
                newName: "CreatedByIP");

            migrationBuilder.RenameColumn(
                name: "CreatorBrowserName",
                table: "User",
                newName: "CreatedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifierUserId",
                table: "RoleClaim",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifierIp",
                table: "RoleClaim",
                newName: "ModifiedByIP");

            migrationBuilder.RenameColumn(
                name: "ModifierBrowserName",
                table: "RoleClaim",
                newName: "ModifiedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifierUserId",
                table: "Role",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifierIp",
                table: "Role",
                newName: "ModifiedByIP");

            migrationBuilder.RenameColumn(
                name: "ModifierBrowserName",
                table: "Role",
                newName: "ModifiedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                table: "Role",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatorIp",
                table: "Role",
                newName: "CreatedByIP");

            migrationBuilder.RenameColumn(
                name: "CreatorBrowserName",
                table: "Role",
                newName: "CreatedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifierUserId",
                table: "Product",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifierIp",
                table: "Product",
                newName: "ModifiedByIP");

            migrationBuilder.RenameColumn(
                name: "ModifierBrowserName",
                table: "Product",
                newName: "ModifiedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                table: "Product",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatorIp",
                table: "Product",
                newName: "CreatedByIP");

            migrationBuilder.RenameColumn(
                name: "CreatorBrowserName",
                table: "Product",
                newName: "CreatedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifierUserId",
                table: "Permission",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifierIp",
                table: "Permission",
                newName: "ModifiedByIP");

            migrationBuilder.RenameColumn(
                name: "ModifierBrowserName",
                table: "Permission",
                newName: "ModifiedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifierUserId",
                table: "Invoice",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifierIp",
                table: "Invoice",
                newName: "ModifiedByIP");

            migrationBuilder.RenameColumn(
                name: "ModifierBrowserName",
                table: "Invoice",
                newName: "ModifiedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                table: "Invoice",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatorIp",
                table: "Invoice",
                newName: "CreatedByIP");

            migrationBuilder.RenameColumn(
                name: "CreatorBrowserName",
                table: "Invoice",
                newName: "CreatedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifierUserId",
                table: "Blog",
                newName: "ModifiedByUserId");

            migrationBuilder.RenameColumn(
                name: "ModifierIp",
                table: "Blog",
                newName: "ModifiedByIP");

            migrationBuilder.RenameColumn(
                name: "ModifierBrowserName",
                table: "Blog",
                newName: "ModifiedByBrowserName");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                table: "Blog",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatorIp",
                table: "Blog",
                newName: "CreatedByIP");

            migrationBuilder.RenameColumn(
                name: "CreatorBrowserName",
                table: "Blog",
                newName: "CreatedByBrowserName");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                schema: "dbo",
                table: "Log",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "UserRole",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "UserClaim",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "User",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "RoleClaim",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Role",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "Role",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Product",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "Permission",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Invoice",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "Invoice",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDateTime",
                table: "Blog",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDateTime",
                table: "Blog",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                schema: "dbo",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "RoleClaim");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "CreatedDateTime",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "ModifiedDateTime",
                table: "Blog");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "UserRole",
                newName: "CreatorUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedByIP",
                table: "UserRole",
                newName: "CreatorIp");

            migrationBuilder.RenameColumn(
                name: "CreatedByBrowserName",
                table: "UserRole",
                newName: "CreatorBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "UserClaim",
                newName: "ModifierUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByIP",
                table: "UserClaim",
                newName: "ModifierIp");

            migrationBuilder.RenameColumn(
                name: "ModifiedByBrowserName",
                table: "UserClaim",
                newName: "ModifierBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "User",
                newName: "ModifierUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByIP",
                table: "User",
                newName: "ModifierIp");

            migrationBuilder.RenameColumn(
                name: "ModifiedByBrowserName",
                table: "User",
                newName: "ModifierBrowserName");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "User",
                newName: "CreatorUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedByIP",
                table: "User",
                newName: "CreatorIp");

            migrationBuilder.RenameColumn(
                name: "CreatedByBrowserName",
                table: "User",
                newName: "CreatorBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "RoleClaim",
                newName: "ModifierUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByIP",
                table: "RoleClaim",
                newName: "ModifierIp");

            migrationBuilder.RenameColumn(
                name: "ModifiedByBrowserName",
                table: "RoleClaim",
                newName: "ModifierBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Role",
                newName: "ModifierUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByIP",
                table: "Role",
                newName: "ModifierIp");

            migrationBuilder.RenameColumn(
                name: "ModifiedByBrowserName",
                table: "Role",
                newName: "ModifierBrowserName");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Role",
                newName: "CreatorUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedByIP",
                table: "Role",
                newName: "CreatorIp");

            migrationBuilder.RenameColumn(
                name: "CreatedByBrowserName",
                table: "Role",
                newName: "CreatorBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Product",
                newName: "ModifierUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByIP",
                table: "Product",
                newName: "ModifierIp");

            migrationBuilder.RenameColumn(
                name: "ModifiedByBrowserName",
                table: "Product",
                newName: "ModifierBrowserName");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Product",
                newName: "CreatorUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedByIP",
                table: "Product",
                newName: "CreatorIp");

            migrationBuilder.RenameColumn(
                name: "CreatedByBrowserName",
                table: "Product",
                newName: "CreatorBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Permission",
                newName: "ModifierUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByIP",
                table: "Permission",
                newName: "ModifierIp");

            migrationBuilder.RenameColumn(
                name: "ModifiedByBrowserName",
                table: "Permission",
                newName: "ModifierBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Invoice",
                newName: "ModifierUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByIP",
                table: "Invoice",
                newName: "ModifierIp");

            migrationBuilder.RenameColumn(
                name: "ModifiedByBrowserName",
                table: "Invoice",
                newName: "ModifierBrowserName");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Invoice",
                newName: "CreatorUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedByIP",
                table: "Invoice",
                newName: "CreatorIp");

            migrationBuilder.RenameColumn(
                name: "CreatedByBrowserName",
                table: "Invoice",
                newName: "CreatorBrowserName");

            migrationBuilder.RenameColumn(
                name: "ModifiedByUserId",
                table: "Blog",
                newName: "ModifierUserId");

            migrationBuilder.RenameColumn(
                name: "ModifiedByIP",
                table: "Blog",
                newName: "ModifierIp");

            migrationBuilder.RenameColumn(
                name: "ModifiedByBrowserName",
                table: "Blog",
                newName: "ModifierBrowserName");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Blog",
                newName: "CreatorUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedByIP",
                table: "Blog",
                newName: "CreatorIp");

            migrationBuilder.RenameColumn(
                name: "CreatedByBrowserName",
                table: "Blog",
                newName: "CreatorBrowserName");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Timestamp",
                schema: "dbo",
                table: "Log",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationDateTime",
                table: "UserRole",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModificationDateTime",
                table: "UserClaim",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationDateTime",
                table: "User",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModificationDateTime",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModificationDateTime",
                table: "RoleClaim",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationDateTime",
                table: "Role",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModificationDateTime",
                table: "Role",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationDateTime",
                table: "Product",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModificationDateTime",
                table: "Product",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModificationDateTime",
                table: "Permission",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationDateTime",
                table: "Invoice",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModificationDateTime",
                table: "Invoice",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreationDateTime",
                table: "Blog",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ModificationDateTime",
                table: "Blog",
                nullable: true);
        }
    }
}
