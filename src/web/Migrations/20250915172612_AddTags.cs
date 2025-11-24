using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations;

/// <inheritdoc />
public partial class AddTags : Migration
{
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.CreateTable(
                    name: "Tags",
                    columns: table => new
                    {
                            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                            Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                    },
                    constraints: table =>
                    {
                            table.PrimaryKey("PK_Tags", x => x.Id);
                    });

                migrationBuilder.CreateTable(
                    name: "ArtPieceTags",
                    columns: table => new
                    {
                            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                            ArtPieceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                            TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                    },
                    constraints: table =>
                    {
                            table.PrimaryKey("PK_ArtPieceTags", x => x.Id);
                            table.ForeignKey(
                    name: "FK_ArtPieceTags_ArtPieces_ArtPieceId",
                    column: x => x.ArtPieceId,
                    principalTable: "ArtPieces",
                    principalColumn: "Id");
                            table.ForeignKey(
                    name: "FK_ArtPieceTags_Tags_TagId",
                    column: x => x.TagId,
                    principalTable: "Tags",
                    principalColumn: "Id");
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_ArtPieceTags_ArtPieceId",
                    table: "ArtPieceTags",
                    column: "ArtPieceId");

                migrationBuilder.CreateIndex(
                    name: "IX_ArtPieceTags_TagId",
                    table: "ArtPieceTags",
                    column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropTable(
                    name: "ArtPieceTags");

                migrationBuilder.DropTable(
                    name: "Tags");
        }
}
