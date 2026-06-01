using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLibrary.Migrations
{
    /// <inheritdoc />
    public partial class filepathHorsFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Foto",
                table: "HorseFoto");

            migrationBuilder.AddColumn<string>(
                name: "FotoPath",
                table: "HorseFoto",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoPath",
                table: "HorseFoto");

            migrationBuilder.AddColumn<byte[]>(
                name: "Foto",
                table: "HorseFoto",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
