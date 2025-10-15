using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
        /// <inheritdoc />
        public partial class AddBoostEntity : Migration
        {
                /// <inheritdoc />
                protected override void Up(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.AddColumn<Guid>(
                            name: "ActiveBoostId",
                            table: "Artists",
                            type: "uniqueidentifier",
                            nullable: true);

                        migrationBuilder.CreateTable(
                            name: "Boost",
                            columns: table => new
                            {
                                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                                    ExpirationDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                                    ArtPieceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                            },
                            constraints: table =>
                            {
                                    table.PrimaryKey("PK_Boost", x => x.Id);
                                    table.ForeignKey(
                            name: "FK_Boost_ArtPieces_ArtPieceId",
                            column: x => x.ArtPieceId,
                            principalTable: "ArtPieces",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                            });

                        migrationBuilder.CreateIndex(
                            name: "IX_Artists_ActiveBoostId",
                            table: "Artists",
                            column: "ActiveBoostId");

                        migrationBuilder.CreateIndex(
                            name: "IX_Boost_ArtPieceId",
                            table: "Boost",
                            column: "ArtPieceId");

                        migrationBuilder.AddForeignKey(
                            name: "FK_Artists_Boost_ActiveBoostId",
                            table: "Artists",
                            column: "ActiveBoostId",
                            principalTable: "Boost",
                            principalColumn: "Id");
                }

                /// <inheritdoc />
                protected override void Down(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.DropForeignKey(
                            name: "FK_Artists_Boost_ActiveBoostId",
                            table: "Artists");

                        migrationBuilder.DropTable(
                            name: "Boost");

                        migrationBuilder.DropIndex(
                            name: "IX_Artists_ActiveBoostId",
                            table: "Artists");

                        migrationBuilder.DropColumn(
                            name: "ActiveBoostId",
                            table: "Artists");
                }
        }
}
