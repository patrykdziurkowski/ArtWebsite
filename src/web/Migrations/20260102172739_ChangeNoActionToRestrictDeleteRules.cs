using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNoActionToRestrictDeleteRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistPointAwards_Artists_ArtistId",
                table: "ArtistPointAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtPieces_Artists_ArtistId",
                table: "ArtPieces");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtPiecesServed_ArtPieces_ArtPieceId",
                table: "ArtPiecesServed");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtPieceTags_ArtPieces_ArtPieceId",
                table: "ArtPieceTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtPieceTags_Tags_TagId",
                table: "ArtPieceTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Boosts_ArtPieces_ArtPieceId",
                table: "Boosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Boosts_Artists_ArtistId",
                table: "Boosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_ArtPieces_ArtPieceId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Reviewers_ReviewerId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_MissionProgresses_AspNetUsers_UserId",
                table: "MissionProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewerPointAwards_Reviewers_ReviewerId",
                table: "ReviewerPointAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_AspNetUsers_UserId",
                table: "Reviewers");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_ArtPieces_ArtPieceId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Reviewers_ReviewerId",
                table: "Reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistPointAwards_Artists_ArtistId",
                table: "ArtistPointAwards",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtPieces_Artists_ArtistId",
                table: "ArtPieces",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtPiecesServed_ArtPieces_ArtPieceId",
                table: "ArtPiecesServed",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtPieceTags_ArtPieces_ArtPieceId",
                table: "ArtPieceTags",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtPieceTags_Tags_TagId",
                table: "ArtPieceTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Boosts_ArtPieces_ArtPieceId",
                table: "Boosts",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Boosts_Artists_ArtistId",
                table: "Boosts",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_ArtPieces_ArtPieceId",
                table: "Likes",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Reviewers_ReviewerId",
                table: "Likes",
                column: "ReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MissionProgresses_AspNetUsers_UserId",
                table: "MissionProgresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewerPointAwards_Reviewers_ReviewerId",
                table: "ReviewerPointAwards",
                column: "ReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_AspNetUsers_UserId",
                table: "Reviewers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ArtPieces_ArtPieceId",
                table: "Reviews",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Reviewers_ReviewerId",
                table: "Reviews",
                column: "ReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistPointAwards_Artists_ArtistId",
                table: "ArtistPointAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtPieces_Artists_ArtistId",
                table: "ArtPieces");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtPiecesServed_ArtPieces_ArtPieceId",
                table: "ArtPiecesServed");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtPieceTags_ArtPieces_ArtPieceId",
                table: "ArtPieceTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtPieceTags_Tags_TagId",
                table: "ArtPieceTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Boosts_ArtPieces_ArtPieceId",
                table: "Boosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Boosts_Artists_ArtistId",
                table: "Boosts");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_ArtPieces_ArtPieceId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_Reviewers_ReviewerId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_MissionProgresses_AspNetUsers_UserId",
                table: "MissionProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewerPointAwards_Reviewers_ReviewerId",
                table: "ReviewerPointAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviewers_AspNetUsers_UserId",
                table: "Reviewers");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_ArtPieces_ArtPieceId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Reviewers_ReviewerId",
                table: "Reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistPointAwards_Artists_ArtistId",
                table: "ArtistPointAwards",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtPieces_Artists_ArtistId",
                table: "ArtPieces",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtPiecesServed_ArtPieces_ArtPieceId",
                table: "ArtPiecesServed",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtPieceTags_ArtPieces_ArtPieceId",
                table: "ArtPieceTags",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtPieceTags_Tags_TagId",
                table: "ArtPieceTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Boosts_ArtPieces_ArtPieceId",
                table: "Boosts",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Boosts_Artists_ArtistId",
                table: "Boosts",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_ArtPieces_ArtPieceId",
                table: "Likes",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_Reviewers_ReviewerId",
                table: "Likes",
                column: "ReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MissionProgresses_AspNetUsers_UserId",
                table: "MissionProgresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewerPointAwards_Reviewers_ReviewerId",
                table: "ReviewerPointAwards",
                column: "ReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviewers_AspNetUsers_UserId",
                table: "Reviewers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ArtPieces_ArtPieceId",
                table: "Reviews",
                column: "ArtPieceId",
                principalTable: "ArtPieces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Reviewers_ReviewerId",
                table: "Reviews",
                column: "ReviewerId",
                principalTable: "Reviewers",
                principalColumn: "Id");
        }
    }
}
