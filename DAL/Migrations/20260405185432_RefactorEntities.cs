using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class RefactorEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "WrappingPapers");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "WrappingPapers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Pattern",
                table: "WrappingPapers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "WrappingPapers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Shade",
                table: "Colors",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "BouquetFlowers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WrappingPapers_ColorId",
                table: "WrappingPapers",
                column: "ColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_WrappingPapers_Colors_ColorId",
                table: "WrappingPapers",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "ColorId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WrappingPapers_Colors_ColorId",
                table: "WrappingPapers");

            migrationBuilder.DropIndex(
                name: "IX_WrappingPapers_ColorId",
                table: "WrappingPapers");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "WrappingPapers");

            migrationBuilder.DropColumn(
                name: "Shade",
                table: "Colors");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "WrappingPapers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Pattern",
                table: "WrappingPapers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "WrappingPapers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "BouquetFlowers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
