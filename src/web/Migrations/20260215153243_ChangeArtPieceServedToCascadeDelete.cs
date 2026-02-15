using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
    /// <inheritdoc />
    public partial class ChangeArtPieceServedToCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtPiecesServed_ArtPieces_ArtPieceId",
                table: "ArtPiecesServed");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtPiecesServed_ArtPieces_ArtPieceId",
                table: "ArtPiecesServed",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtPiecesServed_ArtPieces_ArtPieceId",
                table: "ArtPiecesServed");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtPiecesServed_ArtPieces_ArtPieceId",
                table: "ArtPiecesServed",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
