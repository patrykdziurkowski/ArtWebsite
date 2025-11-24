using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations;

/// <inheritdoc />
public partial class AddBoostsToApplicationDbContext : Migration
{
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropForeignKey(
                    name: "FK_Boost_ArtPieces_ArtPieceId",
                    table: "Boost");

                migrationBuilder.DropForeignKey(
                    name: "FK_Boost_Artists_ArtistId",
                    table: "Boost");

                migrationBuilder.DropForeignKey(
                    name: "FK_Boost_Artists_ArtistId1",
                    table: "Boost");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_Boost",
                    table: "Boost");

                migrationBuilder.RenameTable(
                    name: "Boost",
                    newName: "Boosts");

                migrationBuilder.RenameIndex(
                    name: "IX_Boost_ArtPieceId",
                    table: "Boosts",
                    newName: "IX_Boosts_ArtPieceId");

                migrationBuilder.RenameIndex(
                    name: "IX_Boost_ArtistId1",
                    table: "Boosts",
                    newName: "IX_Boosts_ArtistId1");

                migrationBuilder.RenameIndex(
                    name: "IX_Boost_ArtistId",
                    table: "Boosts",
                    newName: "IX_Boosts_ArtistId");

                migrationBuilder.AddPrimaryKey(
                    name: "PK_Boosts",
                    table: "Boosts",
                    column: "Id");

                migrationBuilder.AddForeignKey(
                    name: "FK_Boosts_ArtPieces_ArtPieceId",
                    table: "Boosts",
                    column: "ArtPieceId",
                    principalTable: "ArtPieces",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);

                migrationBuilder.AddForeignKey(
                    name: "FK_Boosts_Artists_ArtistId",
                    table: "Boosts",
                    column: "ArtistId",
                    principalTable: "Artists",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.NoAction);

                migrationBuilder.AddForeignKey(
                    name: "FK_Boosts_Artists_ArtistId1",
                    table: "Boosts",
                    column: "ArtistId1",
                    principalTable: "Artists",
                    principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropForeignKey(
                    name: "FK_Boosts_ArtPieces_ArtPieceId",
                    table: "Boosts");

                migrationBuilder.DropForeignKey(
                    name: "FK_Boosts_Artists_ArtistId",
                    table: "Boosts");

                migrationBuilder.DropForeignKey(
                    name: "FK_Boosts_Artists_ArtistId1",
                    table: "Boosts");

                migrationBuilder.DropPrimaryKey(
                    name: "PK_Boosts",
                    table: "Boosts");

                migrationBuilder.RenameTable(
                    name: "Boosts",
                    newName: "Boost");

                migrationBuilder.RenameIndex(
                    name: "IX_Boosts_ArtPieceId",
                    table: "Boost",
                    newName: "IX_Boost_ArtPieceId");

                migrationBuilder.RenameIndex(
                    name: "IX_Boosts_ArtistId1",
                    table: "Boost",
                    newName: "IX_Boost_ArtistId1");

                migrationBuilder.RenameIndex(
                    name: "IX_Boosts_ArtistId",
                    table: "Boost",
                    newName: "IX_Boost_ArtistId");

                migrationBuilder.AddPrimaryKey(
                    name: "PK_Boost",
                    table: "Boost",
                    column: "Id");

                migrationBuilder.AddForeignKey(
                    name: "FK_Boost_ArtPieces_ArtPieceId",
                    table: "Boost",
                    column: "ArtPieceId",
                    principalTable: "ArtPieces",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                migrationBuilder.AddForeignKey(
                    name: "FK_Boost_Artists_ArtistId",
                    table: "Boost",
                    column: "ArtistId",
                    principalTable: "Artists",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);

                migrationBuilder.AddForeignKey(
                    name: "FK_Boost_Artists_ArtistId1",
                    table: "Boost",
                    column: "ArtistId1",
                    principalTable: "Artists",
                    principalColumn: "Id");
        }
}
