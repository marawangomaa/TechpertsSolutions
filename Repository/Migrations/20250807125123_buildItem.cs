using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class buildItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustomBuild",
                table: "CartItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PcAssemblyId",
                table: "CartItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "CartItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_PcAssemblyId",
                table: "CartItems",
                column: "PcAssemblyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_PCAssemblies_PcAssemblyId",
                table: "CartItems",
                column: "PcAssemblyId",
                principalTable: "PCAssemblies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_PCAssemblies_PcAssemblyId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_PcAssemblyId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "IsCustomBuild",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "PcAssemblyId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "CartItems");
        }
    }
}
