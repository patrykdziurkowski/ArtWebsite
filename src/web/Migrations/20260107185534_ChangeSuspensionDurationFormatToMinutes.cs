using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web.Migrations
{
        /// <inheritdoc />
        public partial class ChangeSuspensionDurationFormatToMinutes : Migration
        {
                /// <inheritdoc />
                protected override void Up(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.DropColumn(
                            name: "Duration",
                            table: "Suspensions");

                        migrationBuilder.AddColumn<double>(
                            name: "DurationMinutes",
                            table: "Suspensions",
                            type: "float",
                            nullable: false);
                }

                /// <inheritdoc />
                protected override void Down(MigrationBuilder migrationBuilder)
                {
                        migrationBuilder.RenameColumn(
                            name: "DurationMinutes",
                            table: "Suspensions",
                            newName: "Duration");

                        migrationBuilder.AlterColumn<TimeSpan>(
                            name: "Duration",
                            table: "Suspensions",
                            type: "time",
                            nullable: false,
                            oldClrType: typeof(double),
                            oldType: "float");
                }
        }
}
