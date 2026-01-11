using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceDurationWithExpiryDateOnSuspension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Suspensions");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiryDate",
                table: "Suspensions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "Suspensions");

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Suspensions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
