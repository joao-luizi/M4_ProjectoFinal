using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLibrary.Migrations
{
    /// <inheritdoc />
    public partial class RefactorHorseModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Horses");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Horses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Horses",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Horses");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Horses");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Horses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
