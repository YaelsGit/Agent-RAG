using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gift_Donor_DonorId",
                table: "Gift");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Gift_GiftId",
                table: "Purchase");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchase_Users_UserId",
                table: "Purchase");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Addresss_AddressId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Addresss");

            migrationBuilder.DropIndex(
                name: "IX_Users_AddressId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchase",
                table: "Purchase");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gift",
                table: "Gift");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Donor",
                table: "Donor");

            migrationBuilder.RenameTable(
                name: "Purchase",
                newName: "Purchases");

            migrationBuilder.RenameTable(
                name: "Gift",
                newName: "Gifts");

            migrationBuilder.RenameTable(
                name: "Donor",
                newName: "Donors");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "Users",
                newName: "BuildingNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Purchase_UserId",
                table: "Purchases",
                newName: "IX_Purchases_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchase_GiftId",
                table: "Purchases",
                newName: "IX_Purchases_GiftId");

            migrationBuilder.RenameIndex(
                name: "IX_Gift_DonorId",
                table: "Gifts",
                newName: "IX_Gifts_DonorId");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Users",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gifts",
                table: "Gifts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Donors",
                table: "Donors",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Gifts_Donors_DonorId",
                table: "Gifts",
                column: "DonorId",
                principalTable: "Donors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Gifts_GiftId",
                table: "Purchases",
                column: "GiftId",
                principalTable: "Gifts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Users_UserId",
                table: "Purchases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gifts_Donors_DonorId",
                table: "Gifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Gifts_GiftId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Users_UserId",
                table: "Purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gifts",
                table: "Gifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Donors",
                table: "Donors");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Purchases",
                newName: "Purchase");

            migrationBuilder.RenameTable(
                name: "Gifts",
                newName: "Gift");

            migrationBuilder.RenameTable(
                name: "Donors",
                newName: "Donor");

            migrationBuilder.RenameColumn(
                name: "BuildingNumber",
                table: "Users",
                newName: "AddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_UserId",
                table: "Purchase",
                newName: "IX_Purchase_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_GiftId",
                table: "Purchase",
                newName: "IX_Purchase_GiftId");

            migrationBuilder.RenameIndex(
                name: "IX_Gifts_DonorId",
                table: "Gift",
                newName: "IX_Gift_DonorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchase",
                table: "Purchase",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gift",
                table: "Gift",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Donor",
                table: "Donor",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Addresss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApartmenNumber = table.Column<int>(type: "int", nullable: false),
                    BuildingNumber = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Neighborhood = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresss", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressId",
                table: "Users",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gift_Donor_DonorId",
                table: "Gift",
                column: "DonorId",
                principalTable: "Donor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Gift_GiftId",
                table: "Purchase",
                column: "GiftId",
                principalTable: "Gift",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchase_Users_UserId",
                table: "Purchase",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Addresss_AddressId",
                table: "Users",
                column: "AddressId",
                principalTable: "Addresss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
