using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations;

/// <inheritdoc />
public partial class AddPointsToArtist : Migration
{
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.AddColumn<int>(
                    name: "Points",
                    table: "Artists",
                    type: "int",
                    nullable: false,
                    defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
                migrationBuilder.DropColumn(
                    name: "Points",
                    table: "Artists");
        }
}
