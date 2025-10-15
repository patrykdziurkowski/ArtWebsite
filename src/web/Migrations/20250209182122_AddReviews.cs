using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
        /// <inheritdoc />
        public partial class AddReviews : Migration
        {
                /// <inheritdoc />
                protected override void Up(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.CreateTable(
                            name: "Reviews",
                            columns: table => new
                            {
                                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                                    ArtPieceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                            },
                            constraints: table =>
                            {
                                    table.PrimaryKey("PK_Reviews", x => x.Id);
                                    table.ForeignKey(
                            name: "FK_Reviews_ArtPieces_ArtPieceId",
                            column: x => x.ArtPieceId,
                            principalTable: "ArtPieces",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                                    table.ForeignKey(
                            name: "FK_Reviews_AspNetUsers_ReviewerId",
                            column: x => x.ReviewerId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.NoAction);
                            });

                        migrationBuilder.CreateIndex(
                            name: "IX_Reviews_ArtPieceId",
                            table: "Reviews",
                            column: "ArtPieceId");

                        migrationBuilder.CreateIndex(
                            name: "IX_Reviews_ReviewerId",
                            table: "Reviews",
                            column: "ReviewerId");
                }

                /// <inheritdoc />
                protected override void Down(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.DropTable(
                            name: "Reviews");
                }
        }
}
