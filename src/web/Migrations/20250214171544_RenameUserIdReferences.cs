using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserIdReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_AspNetUsers_OwnerId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_AspNetUsers_OwnerId",
                table: "Reviewers");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Reviewers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviewers_OwnerId",
                table: "Reviewers",
                newName: "IX_Reviewers_UserId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Artists",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Artists_OwnerId",
                table: "Artists",
                newName: "IX_Artists_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_AspNetUsers_UserId",
                table: "Artists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_AspNetUsers_UserId",
                table: "Reviewers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artists_AspNetUsers_UserId",
                table: "Artists");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_AspNetUsers_UserId",
                table: "Reviewers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reviewers",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviewers_UserId",
                table: "Reviewers",
                newName: "IX_Reviewers_OwnerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Artists",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Artists_UserId",
                table: "Artists",
                newName: "IX_Artists_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artists_AspNetUsers_OwnerId",
                table: "Artists",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_AspNetUsers_OwnerId",
                table: "Reviewers",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
