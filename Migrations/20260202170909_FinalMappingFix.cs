using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace officeline.Migrations
{
    /// <inheritdoc />
    public partial class FinalMappingFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_userId1",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_userId1",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "userId1",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_userId",
                table: "Products",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_userId",
                table: "Products",
                column: "userId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_userId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_userId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "userId1",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_userId1",
                table: "Products",
                column: "userId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_userId1",
                table: "Products",
                column: "userId1",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
