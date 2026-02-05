using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace officeline.Migrations
{
    /// <inheritdoc />
    public partial class SyncUserIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_usersuserId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Products",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "usersuserId",
                table: "Products",
                newName: "userId1");

            migrationBuilder.RenameIndex(
                name: "IX_Products_usersuserId",
                table: "Products",
                newName: "IX_Products_userId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_userId1",
                table: "Products",
                column: "userId1",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_userId1",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Products",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "userId1",
                table: "Products",
                newName: "usersuserId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_userId1",
                table: "Products",
                newName: "IX_Products_usersuserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_usersuserId",
                table: "Products",
                column: "usersuserId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
