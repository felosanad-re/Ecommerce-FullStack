using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Talabat.Repositaries.Data.Migrations.ShopMigrations
{
    /// <inheritdoc />
    public partial class AlterProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAddedToCart",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAddedToCart",
                table: "Products");
        }
    }
}
