using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Repositaries.Data.Migrations.ShopMigrations
{
    /// <inheritdoc />
    public partial class AlterStockTypeNotSavedInDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockType",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StockType",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
