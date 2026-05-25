using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loolpay.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToStore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_path",
                table: "stores",
                type: "TEXT",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_path",
                table: "stores");
        }
    }
}
