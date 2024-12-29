using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
        /// <inheritdoc />
        public partial class AddedOwnerIdToArtist : Migration
        {
                /// <inheritdoc />
                protected override void Up(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.AddColumn<string>(
                            name: "OwnerId",
                            table: "Artists",
                            type: "nvarchar(450)",
                            nullable: false,
                            defaultValue: "");

                        migrationBuilder.CreateIndex(
                            name: "IX_Artists_OwnerId",
                            table: "Artists",
                            column: "OwnerId",
                            unique: true);

                        migrationBuilder.AddForeignKey(
                            name: "FK_Artists_AspNetUsers_OwnerId",
                            table: "Artists",
                            column: "OwnerId",
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                }

                /// <inheritdoc />
                protected override void Down(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.DropForeignKey(
                            name: "FK_Artists_AspNetUsers_OwnerId",
                            table: "Artists");

                        migrationBuilder.DropIndex(
                            name: "IX_Artists_OwnerId",
                            table: "Artists");

                        migrationBuilder.DropColumn(
                            name: "OwnerId",
                            table: "Artists");
                }
        }
}
