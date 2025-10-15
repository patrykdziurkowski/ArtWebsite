using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
        /// <inheritdoc />
        public partial class ReplaceDateTimeWithDateTimeOffset : Migration
        {
                /// <inheritdoc />
                protected override void Up(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.AlterColumn<DateTimeOffset>(
                            name: "Date",
                            table: "Reviews",
                            type: "datetimeoffset",
                            nullable: false,
                            oldClrType: typeof(DateTime),
                            oldType: "datetime2");

                        migrationBuilder.AlterColumn<DateTimeOffset>(
                            name: "JoinDate",
                            table: "Reviewers",
                            type: "datetimeoffset",
                            nullable: false,
                            oldClrType: typeof(DateTime),
                            oldType: "datetime2");

                        migrationBuilder.AlterColumn<DateTimeOffset>(
                            name: "UploadDate",
                            table: "ArtPieces",
                            type: "datetimeoffset",
                            nullable: false,
                            oldClrType: typeof(DateTime),
                            oldType: "datetime2");
                }

                /// <inheritdoc />
                protected override void Down(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.AlterColumn<DateTime>(
                            name: "Date",
                            table: "Reviews",
                            type: "datetime2",
                            nullable: false,
                            oldClrType: typeof(DateTimeOffset),
                            oldType: "datetimeoffset");

                        migrationBuilder.AlterColumn<DateTime>(
                            name: "JoinDate",
                            table: "Reviewers",
                            type: "datetime2",
                            nullable: false,
                            oldClrType: typeof(DateTimeOffset),
                            oldType: "datetimeoffset");

                        migrationBuilder.AlterColumn<DateTime>(
                            name: "UploadDate",
                            table: "ArtPieces",
                            type: "datetime2",
                            nullable: false,
                            oldClrType: typeof(DateTimeOffset),
                            oldType: "datetimeoffset");
                }
        }
}
