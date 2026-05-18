using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RepositoryLibrary.Migrations
{
    /// <inheritdoc />
    public partial class FixUserPaymentPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HorseFoto_Horses_HorseId",
                table: "HorseFoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPayments",
                table: "UserPayments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserPayments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserPayments",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPayments",
                table: "UserPayments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HorseFoto_Horses_HorseId",
                table: "HorseFoto",
                column: "HorseId",
                principalTable: "Horses",
                principalColumn: "HorseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HorseFoto_Horses_HorseId",
                table: "HorseFoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPayments",
                table: "UserPayments");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserPayments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UserPayments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPayments",
                table: "UserPayments",
                columns: new[] { "UserId", "BuyDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_HorseFoto_Horses_HorseId",
                table: "HorseFoto",
                column: "HorseId",
                principalTable: "Horses",
                principalColumn: "HorseId");
        }
    }
}
