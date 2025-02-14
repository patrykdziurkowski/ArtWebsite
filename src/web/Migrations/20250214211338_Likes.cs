using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
        /// <inheritdoc />
        public partial class Likes : Migration
        {
                /// <inheritdoc />
                protected override void Up(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.CreateTable(
                            name: "Like",
                            columns: table => new
                            {
                                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                                    ArtPieceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                            },
                            constraints: table =>
                            {
                                    table.PrimaryKey("PK_Like", x => x.Id);
                                    table.ForeignKey(
                            name: "FK_Like_ArtPieces_ArtPieceId",
                            column: x => x.ArtPieceId,
                            principalTable: "ArtPieces",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                                    table.ForeignKey(
                            name: "FK_Like_Reviewers_ReviewerId",
                            column: x => x.ReviewerId,
                            principalTable: "Reviewers",
                            principalColumn: "Id");
                            });

                        migrationBuilder.CreateIndex(
                            name: "IX_Like_ArtPieceId",
                            table: "Like",
                            column: "ArtPieceId");

                        migrationBuilder.CreateIndex(
                            name: "IX_Like_ReviewerId",
                            table: "Like",
                            column: "ReviewerId");
                }

                /// <inheritdoc />
                protected override void Down(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.DropTable(
                            name: "Like");
                }
        }
}
