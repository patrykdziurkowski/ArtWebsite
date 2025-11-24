using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations;

/// <inheritdoc />
public partial class SwapReviewOwnerFromUserToReviewer : Migration
{
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropForeignKey(
                    name: "FK_Reviews_AspNetUsers_ReviewerId",
                    table: "Reviews");

                migrationBuilder.AddForeignKey(
                    name: "FK_Reviews_Reviewers_ReviewerId",
                    table: "Reviews",
                    column: "ReviewerId",
                    principalTable: "Reviewers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropForeignKey(
                    name: "FK_Reviews_Reviewers_ReviewerId",
                    table: "Reviews");

                migrationBuilder.AddForeignKey(
                    name: "FK_Reviews_AspNetUsers_ReviewerId",
                    table: "Reviews",
                    column: "ReviewerId",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
        }
}
