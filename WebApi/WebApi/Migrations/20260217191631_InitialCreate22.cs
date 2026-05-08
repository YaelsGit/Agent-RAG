using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "Gifts");

            migrationBuilder.AddColumn<bool>(
                name: "IsRandom",
                table: "Gifts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRandom",
                table: "Gifts");

            migrationBuilder.AddColumn<int>(
                name: "PictureId",
                table: "Gifts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
