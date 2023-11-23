using Microsoft.EntityFrameworkCore.Migrations;
using MyShop_DataAccess;

#nullable disable

namespace MyShop_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddShortDescToProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortDes",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortDes",
                table: "Product");
        }
    }
}
