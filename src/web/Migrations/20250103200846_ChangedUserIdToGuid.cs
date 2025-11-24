using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations;

/// <inheritdoc />
public partial class ChangedUserIdToGuid : Migration
{
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        table: "AspNetUserTokens");

                migrationBuilder.DropForeignKey(
                        name: "FK_Artists_AspNetUsers_OwnerId",
                        table: "Artists");

                migrationBuilder.DropForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        table: "AspNetRoleClaims");

                migrationBuilder.DropForeignKey(
                    name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                    table: "AspNetUserRoles");

                migrationBuilder.DropForeignKey(
                    name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    table: "AspNetUserRoles");

                migrationBuilder.DropForeignKey(
                    name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                    table: "AspNetUserLogins");

                migrationBuilder.DropForeignKey(
                    name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                    table: "AspNetUserClaims");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_AspNetUserTokens",
                    table: "AspNetUserTokens");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_AspNetUsers",
                    table: "AspNetUsers");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_AspNetRoles",
                    table: "AspNetRoles");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_AspNetUserRoles",
                    table: "AspNetUserRoles");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_AspNetUserClaims",
                    table: "AspNetUserClaims");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_AspNetUserLogins",
                    table: "AspNetUserLogins");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_AspNetRoleClaims",
                    table: "AspNetRoleClaims");

                migrationBuilder.AlterColumn<Guid>(
                    name: "UserId",
                    table: "AspNetUserTokens",
                    type: "uniqueidentifier",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(450)");

                migrationBuilder.AlterColumn<Guid>(
                    name: "Id",
                    table: "AspNetUsers",
                    type: "uniqueidentifier",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(450)");

                migrationBuilder.AlterColumn<Guid>(
                    name: "RoleId",
                    table: "AspNetUserRoles",
                    type: "uniqueidentifier",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(450)");

                migrationBuilder.AlterColumn<Guid>(
                    name: "UserId",
                    table: "AspNetUserRoles",
                    type: "uniqueidentifier",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(450)");

                migrationBuilder.AlterColumn<Guid>(
                    name: "UserId",
                    table: "AspNetUserLogins",
                    type: "uniqueidentifier",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(450)");

                migrationBuilder.AlterColumn<Guid>(
                    name: "UserId",
                    table: "AspNetUserClaims",
                    type: "uniqueidentifier",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(450)");

                migrationBuilder.AlterColumn<Guid>(
                    name: "Id",
                    table: "AspNetRoles",
                    type: "uniqueidentifier",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(450)");

                migrationBuilder.AlterColumn<Guid>(
                    name: "RoleId",
                    table: "AspNetRoleClaims",
                    type: "uniqueidentifier",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(450)");

                migrationBuilder.AlterColumn<Guid>(
                    name: "OwnerId",
                    table: "Artists",
                    type: "uniqueidentifier",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(450)");

                migrationBuilder.AddPrimaryKey(
                    name: "PK_AspNetUserTokens",
                    table: "AspNetUserTokens",
                    column: "UserId");

                migrationBuilder.AddPrimaryKey(
                    name: "PK_AspNetUsers",
                    table: "AspNetUsers",
                    column: "Id");

                migrationBuilder.AddPrimaryKey(
                    name: "PK_AspNetRoles",
                    table: "AspNetRoles",
                    column: "Id");

                migrationBuilder.AddPrimaryKey(
                    name: "PK_AspNetUserRoles",
                    table: "AspNetUserRoles",
                    columns: new[] { "UserId", "RoleId" });

                migrationBuilder.AddPrimaryKey(
                    name: "PK_AspNetUserClaims",
                    table: "AspNetUserClaims",
                    column: "UserId");

                migrationBuilder.AddPrimaryKey(
                    name: "PK_AspNetUserLogins",
                    table: "AspNetUserLogins",
                    column: "UserId");

                migrationBuilder.AddPrimaryKey(
                    name: "PK_AspNetRoleClaims",
                    table: "AspNetRoleClaims",
                    column: "RoleId");

                migrationBuilder.AddForeignKey(
                    name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                    table: "AspNetUserTokens",
                    column: "UserId",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                migrationBuilder.AddForeignKey(
                    name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                    table: "AspNetUserRoles",
                    column: "UserId",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                migrationBuilder.AddForeignKey(
                    name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                    table: "AspNetUserRoles",
                    column: "RoleId",
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                migrationBuilder.AddForeignKey(
                    name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                    table: "AspNetUserLogins",
                    column: "UserId",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                migrationBuilder.AddForeignKey(
                    name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                    table: "AspNetUserClaims",
                    column: "UserId",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                migrationBuilder.AddForeignKey(
                    name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                    table: "AspNetRoleClaims",
                    column: "RoleId",
                    principalTable: "AspNetRoles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                migrationBuilder.AddForeignKey(
                        name: "FK_Artists_AspNetUsers_OwnerId",
                        table: "Artists",
                        column: "OwnerId",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.AlterColumn<string>(
                    name: "UserId",
                    table: "AspNetUserTokens",
                    type: "nvarchar(450)",
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "uniqueidentifier");

                migrationBuilder.AlterColumn<string>(
                    name: "Id",
                    table: "AspNetUsers",
                    type: "nvarchar(450)",
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "uniqueidentifier");

                migrationBuilder.AlterColumn<string>(
                    name: "RoleId",
                    table: "AspNetUserRoles",
                    type: "nvarchar(450)",
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "uniqueidentifier");

                migrationBuilder.AlterColumn<string>(
                    name: "UserId",
                    table: "AspNetUserRoles",
                    type: "nvarchar(450)",
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "uniqueidentifier");

                migrationBuilder.AlterColumn<string>(
                    name: "UserId",
                    table: "AspNetUserLogins",
                    type: "nvarchar(450)",
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "uniqueidentifier");

                migrationBuilder.AlterColumn<string>(
                    name: "UserId",
                    table: "AspNetUserClaims",
                    type: "nvarchar(450)",
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "uniqueidentifier");

                migrationBuilder.AlterColumn<string>(
                    name: "Id",
                    table: "AspNetRoles",
                    type: "nvarchar(450)",
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "uniqueidentifier");

                migrationBuilder.AlterColumn<string>(
                    name: "RoleId",
                    table: "AspNetRoleClaims",
                    type: "nvarchar(450)",
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "uniqueidentifier");

                migrationBuilder.AlterColumn<string>(
                    name: "OwnerId",
                    table: "Artists",
                    type: "nvarchar(450)",
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "uniqueidentifier");
        }
}
