using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class buildV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_PCAssemblies_PcAssemblyId",
                table: "CartItems");

            migrationBuilder.AddColumn<decimal>(
                name: "AssemblyFee",
                table: "CartItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ProductTotal",
                table: "CartItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_PCAssemblies_PcAssemblyId",
                table: "CartItems",
                column: "PcAssemblyId",
                principalTable: "PCAssemblies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_PCAssemblies_PcAssemblyId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "AssemblyFee",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ProductTotal",
                table: "CartItems");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_PCAssemblies_PcAssemblyId",
                table: "CartItems",
                column: "PcAssemblyId",
                principalTable: "PCAssemblies",
                principalColumn: "Id");
        }
    }
}
