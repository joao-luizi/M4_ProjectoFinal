using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLibrary.Migrations
{
    /// <inheritdoc />
    public partial class SplitHorsePhotoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Horses");

            migrationBuilder.CreateTable(
                name: "HorseFoto",
                columns: table => new
                {
                    HorseId = table.Column<int>(type: "int", nullable: false),
                    Foto = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorseFoto", x => x.HorseId);
                    table.ForeignKey(
                        name: "FK_HorseFoto_Horses_HorseId",
                        column: x => x.HorseId,
                        principalTable: "Horses",
                        principalColumn: "HorseId");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorseFoto");

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Horses",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
