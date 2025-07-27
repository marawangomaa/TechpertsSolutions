using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCartIdFromPCAssemblyItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PCAssemblyItems_Carts_CartId",
                table: "PCAssemblyItems");

            migrationBuilder.DropIndex(
                name: "IX_PCAssemblyItems_CartId",
                table: "PCAssemblyItems");

            migrationBuilder.DropColumn(
                name: "CartId",
                table: "PCAssemblyItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CartId",
                table: "PCAssemblyItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PCAssemblyItems_CartId",
                table: "PCAssemblyItems",
                column: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_PCAssemblyItems_Carts_CartId",
                table: "PCAssemblyItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
