using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Migrations
{
    public partial class V2020_06_19_0339 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserClaim_User_UserId",
                table: "UserClaim");

            migrationBuilder.DropTable(
                name: "RoleClaim");

            migrationBuilder.DropIndex(
                name: "IX_Permission_Discriminator",
                table: "Permission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClaim",
                table: "UserClaim");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "NextNumber",
                table: "NumberedEntity");

            migrationBuilder.RenameTable(
                name: "UserClaim",
                newName: "Claim");

            migrationBuilder.RenameIndex(
                name: "IX_UserClaim_UserId",
                table: "Claim",
                newName: "IX_Claim_UserId");

            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                table: "Permission",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "NextValue",
                table: "NumberedEntity",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "Claim",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                table: "Claim",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "RoleId",
                table: "Claim",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Claim",
                table: "Claim",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_EntityName",
                table: "Permission",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_EntityName",
                table: "Claim",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_Claim_RoleId",
                table: "Claim",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claim_Role_RoleId",
                table: "Claim",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Claim_User_UserId",
                table: "Claim",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claim_Role_RoleId",
                table: "Claim");

            migrationBuilder.DropForeignKey(
                name: "FK_Claim_User_UserId",
                table: "Claim");

            migrationBuilder.DropIndex(
                name: "IX_Permission_EntityName",
                table: "Permission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Claim",
                table: "Claim");

            migrationBuilder.DropIndex(
                name: "IX_Claim_EntityName",
                table: "Claim");

            migrationBuilder.DropIndex(
                name: "IX_Claim_RoleId",
                table: "Claim");

            migrationBuilder.DropColumn(
                name: "EntityName",
                table: "Permission");

            migrationBuilder.DropColumn(
                name: "NextValue",
                table: "NumberedEntity");

            migrationBuilder.DropColumn(
                name: "EntityName",
                table: "Claim");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Claim");

            migrationBuilder.RenameTable(
                name: "Claim",
                newName: "UserClaim");

            migrationBuilder.RenameIndex(
                name: "IX_Claim_UserId",
                table: "UserClaim",
                newName: "IX_UserClaim_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Permission",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "NextNumber",
                table: "NumberedEntity",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "UserClaim",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClaim",
                table: "UserClaim",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByBrowserName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedByIP = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ModifiedByBrowserName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ModifiedByIP = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ModifiedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaim_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Discriminator",
                table: "Permission",
                column: "Discriminator");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaim_RoleId",
                table: "RoleClaim",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaim_User_UserId",
                table: "UserClaim",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
