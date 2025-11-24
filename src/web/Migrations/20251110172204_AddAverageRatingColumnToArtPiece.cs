using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations;

/// <inheritdoc />
public partial class AddAverageRatingColumnToArtPiece : Migration
{
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.AddColumn<int>(
                    name: "AverageRating",
                    table: "ArtPieces",
                    type: "int",
                    nullable: false,
                    defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropColumn(
                    name: "AverageRating",
                    table: "ArtPieces");
        }
}
