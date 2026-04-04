using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsAndNormalizeFlower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Flowers");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Flowers");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Flowers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "Flowers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StemKind",
                table: "Flowers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "StemThicknessMm",
                table: "Flowers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "BouquetFlowers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    ColorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.ColorId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flowers_CategoryId",
                table: "Flowers",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Flowers_ColorId",
                table: "Flowers",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryName",
                table: "Categories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colors_ColorName",
                table: "Colors",
                column: "ColorName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Flowers_Categories_CategoryId",
                table: "Flowers",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flowers_Colors_ColorId",
                table: "Flowers",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "ColorId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flowers_Categories_CategoryId",
                table: "Flowers");

            migrationBuilder.DropForeignKey(
                name: "FK_Flowers_Colors_ColorId",
                table: "Flowers");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropIndex(
                name: "IX_Flowers_CategoryId",
                table: "Flowers");

            migrationBuilder.DropIndex(
                name: "IX_Flowers_ColorId",
                table: "Flowers");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Flowers");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "Flowers");

            migrationBuilder.DropColumn(
                name: "StemKind",
                table: "Flowers");

            migrationBuilder.DropColumn(
                name: "StemThicknessMm",
                table: "Flowers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "BouquetFlowers");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Flowers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Flowers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
