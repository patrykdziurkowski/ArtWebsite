using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
        /// <inheritdoc />
        public partial class AddArtistIdToBoost : Migration
        {
                /// <inheritdoc />
                protected override void Up(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.DropForeignKey(
                            name: "FK_Artists_Boost_ActiveBoostId",
                            table: "Artists");

                        migrationBuilder.DropIndex(
                            name: "IX_Artists_ActiveBoostId",
                            table: "Artists");

                        migrationBuilder.DropColumn(
                            name: "ActiveBoostId",
                            table: "Artists");

                        migrationBuilder.AddColumn<Guid>(
                            name: "ArtistId",
                            table: "Boost",
                            type: "uniqueidentifier",
                            nullable: false,
                            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

                        migrationBuilder.AddColumn<Guid>(
                            name: "ArtistId1",
                            table: "Boost",
                            type: "uniqueidentifier",
                            nullable: true);

                        migrationBuilder.CreateIndex(
                            name: "IX_Boost_ArtistId",
                            table: "Boost",
                            column: "ArtistId");

                        migrationBuilder.CreateIndex(
                            name: "IX_Boost_ArtistId1",
                            table: "Boost",
                            column: "ArtistId1",
                            unique: true,
                            filter: "[ArtistId1] IS NOT NULL");

                        migrationBuilder.AddForeignKey(
                            name: "FK_Boost_Artists_ArtistId",
                            table: "Boost",
                            column: "ArtistId",
                            principalTable: "Artists",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.NoAction);

                        migrationBuilder.AddForeignKey(
                            name: "FK_Boost_Artists_ArtistId1",
                            table: "Boost",
                            column: "ArtistId1",
                            principalTable: "Artists",
                            principalColumn: "Id");
                }

                /// <inheritdoc />
                protected override void Down(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.DropForeignKey(
                            name: "FK_Boost_Artists_ArtistId",
                            table: "Boost");

                        migrationBuilder.DropForeignKey(
                            name: "FK_Boost_Artists_ArtistId1",
                            table: "Boost");

                        migrationBuilder.DropIndex(
                            name: "IX_Boost_ArtistId",
                            table: "Boost");

                        migrationBuilder.DropIndex(
                            name: "IX_Boost_ArtistId1",
                            table: "Boost");

                        migrationBuilder.DropColumn(
                            name: "ArtistId",
                            table: "Boost");

                        migrationBuilder.DropColumn(
                            name: "ArtistId1",
                            table: "Boost");

                        migrationBuilder.AddColumn<Guid>(
                            name: "ActiveBoostId",
                            table: "Artists",
                            type: "uniqueidentifier",
                            nullable: true);

                        migrationBuilder.CreateIndex(
                            name: "IX_Artists_ActiveBoostId",
                            table: "Artists",
                            column: "ActiveBoostId");

                        migrationBuilder.AddForeignKey(
                            name: "FK_Artists_Boost_ActiveBoostId",
                            table: "Artists",
                            column: "ActiveBoostId",
                            principalTable: "Boost",
                            principalColumn: "Id");
                }
        }
}
