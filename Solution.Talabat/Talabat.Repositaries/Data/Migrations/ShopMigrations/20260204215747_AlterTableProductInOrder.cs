using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Repositaries.Data.Migrations.ShopMigrations
{
    /// <inheritdoc />
    public partial class AlterTableProductInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Product_Price",
                table: "OrderItems");

            migrationBuilder.AddColumn<int>(
                name: "Product_ProductId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Product_ProductId",
                table: "OrderItems");

            migrationBuilder.AddColumn<decimal>(
                name: "Product_Price",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
