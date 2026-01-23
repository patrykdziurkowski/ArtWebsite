using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations;

/// <inheritdoc />
public partial class TrackPointAwardHistoryForArtist : Migration
{
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropTable(
                    name: "PointAwards");

                migrationBuilder.CreateTable(
                    name: "ArtistPointAwards",
                    columns: table => new
                    {
                            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                            DateAwarded = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                            ArtistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                            PointValue = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                            table.PrimaryKey("PK_ArtistPointAwards", x => x.Id);
                            table.ForeignKey(
                    name: "FK_ArtistPointAwards_Artists_ArtistId",
                    column: x => x.ArtistId,
                    principalTable: "Artists",
                    principalColumn: "Id");
                    });

                migrationBuilder.CreateTable(
                    name: "ReviewerPointAwards",
                    columns: table => new
                    {
                            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                            DateAwarded = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                            ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                            PointValue = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                            table.PrimaryKey("PK_ReviewerPointAwards", x => x.Id);
                            table.ForeignKey(
                    name: "FK_ReviewerPointAwards_Reviewers_ReviewerId",
                    column: x => x.ReviewerId,
                    principalTable: "Reviewers",
                    principalColumn: "Id");
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_ArtistPointAwards_ArtistId",
                    table: "ArtistPointAwards",
                    column: "ArtistId");

                migrationBuilder.CreateIndex(
                    name: "IX_ReviewerPointAwards_ReviewerId",
                    table: "ReviewerPointAwards",
                    column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropTable(
                    name: "ArtistPointAwards");

                migrationBuilder.DropTable(
                    name: "ReviewerPointAwards");

                migrationBuilder.CreateTable(
                    name: "PointAwards",
                    columns: table => new
                    {
                            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                            DateAwarded = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                            PointValue = table.Column<int>(type: "int", nullable: false),
                            ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                    },
                    constraints: table =>
                    {
                            table.PrimaryKey("PK_PointAwards", x => x.Id);
                            table.ForeignKey(
                    name: "FK_PointAwards_Reviewers_ReviewerId",
                    column: x => x.ReviewerId,
                    principalTable: "Reviewers",
                    principalColumn: "Id");
                    });

                migrationBuilder.CreateIndex(
                    name: "IX_PointAwards_ReviewerId",
                    table: "PointAwards",
                    column: "ReviewerId");
        }
}
