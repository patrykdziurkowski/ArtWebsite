using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations;

/// <inheritdoc />
public partial class AddArtPieces : Migration
{
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.CreateTable(
                    name: "ArtPieces",
                    columns: table => new
                    {
                            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                            Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                            ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                    },
                    constraints: table =>
                    {
                            table.PrimaryKey("PK_ArtPieces", x => x.Id);
                            table.ForeignKey(
                    name: "FK_ArtPieces_Artists_ArtistId",
                    column: x => x.ArtistId,
                    principalTable: "Artists",
                    principalColumn: "ArtistId",
                    onDelete: ReferentialAction.Cascade);
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_ArtPieces_ArtistId",
                    table: "ArtPieces",
                    column: "ArtistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropTable(
                    name: "ArtPieces");
        }
}
