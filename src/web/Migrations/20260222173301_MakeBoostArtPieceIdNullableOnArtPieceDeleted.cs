using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
    /// <inheritdoc />
    public partial class MakeBoostArtPieceIdNullableOnArtPieceDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boosts_ArtPieces_ArtPieceId",
                table: "Boosts");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArtPieceId",
                table: "Boosts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Boosts_ArtPieces_ArtPieceId",
                table: "Boosts",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boosts_ArtPieces_ArtPieceId",
                table: "Boosts");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArtPieceId",
                table: "Boosts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Boosts_ArtPieces_ArtPieceId",
                table: "Boosts",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
