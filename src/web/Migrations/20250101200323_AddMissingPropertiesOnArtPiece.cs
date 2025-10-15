using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
        /// <inheritdoc />
        public partial class AddMissingPropertiesOnArtPiece : Migration
        {
                /// <inheritdoc />
                protected override void Up(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.AddColumn<string>(
                            name: "ImagePath",
                            table: "ArtPieces",
                            type: "nvarchar(max)",
                            nullable: false,
                            defaultValue: "");

                        migrationBuilder.AddColumn<DateTime>(
                            name: "UploadDate",
                            table: "ArtPieces",
                            type: "datetime2",
                            nullable: false,
                            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
                }

                /// <inheritdoc />
                protected override void Down(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.DropColumn(
                            name: "ImagePath",
                            table: "ArtPieces");

                        migrationBuilder.DropColumn(
                            name: "UploadDate",
                            table: "ArtPieces");
                }
        }
}
