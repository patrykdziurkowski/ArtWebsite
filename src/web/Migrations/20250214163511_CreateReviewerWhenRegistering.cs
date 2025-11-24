using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations;

/// <inheritdoc />
public partial class CreateReviewerWhenRegistering : Migration
{
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropForeignKey(
                    name: "FK_Reviewer_AspNetUsers_OwnerId",
                    table: "Reviewer");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_Reviewer",
                    table: "Reviewer");

                migrationBuilder.RenameTable(
                    name: "Reviewer",
                    newName: "Reviewers");

                migrationBuilder.RenameIndex(
                    name: "IX_Reviewer_OwnerId",
                    table: "Reviewers",
                    newName: "IX_Reviewers_OwnerId");

                migrationBuilder.AddPrimaryKey(
                    name: "PK_Reviewers",
                    table: "Reviewers",
                    column: "Id");

                migrationBuilder.AddForeignKey(
                    name: "FK_Reviewers_AspNetUsers_OwnerId",
                    table: "Reviewers",
                    column: "OwnerId",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropForeignKey(
                    name: "FK_Reviewers_AspNetUsers_OwnerId",
                    table: "Reviewers");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_Reviewers",
                    table: "Reviewers");

                migrationBuilder.RenameTable(
                    name: "Reviewers",
                    newName: "Reviewer");

                migrationBuilder.RenameIndex(
                    name: "IX_Reviewers_OwnerId",
                    table: "Reviewer",
                    newName: "IX_Reviewer_OwnerId");

                migrationBuilder.AddPrimaryKey(
                    name: "PK_Reviewer",
                    table: "Reviewer",
                    column: "Id");

                migrationBuilder.AddForeignKey(
                    name: "FK_Reviewer_AspNetUsers_OwnerId",
                    table: "Reviewer",
                    column: "OwnerId",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
        }
}
