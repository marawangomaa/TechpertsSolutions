using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatusEnumWithDataConversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, add a temporary column
            migrationBuilder.AddColumn<int>(
                name: "StatusTemp",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Convert existing string values to enum values
            migrationBuilder.Sql(@"
                UPDATE Orders 
                SET StatusTemp = CASE 
                    WHEN Status = 'Pending' THEN 0
                    WHEN Status = 'InProgress' THEN 1
                    WHEN Status = 'Delivered' THEN 2
                    ELSE 0
                END");

            // Drop the old column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            // Rename the temporary column to Status
            migrationBuilder.RenameColumn(
                name: "StatusTemp",
                table: "Orders",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add a temporary string column
            migrationBuilder.AddColumn<string>(
                name: "StatusString",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending");

            // Convert enum values back to strings
            migrationBuilder.Sql(@"
                UPDATE Orders 
                SET StatusString = CASE 
                    WHEN Status = 0 THEN 'Pending'
                    WHEN Status = 1 THEN 'InProgress'
                    WHEN Status = 2 THEN 'Delivered'
                    ELSE 'Pending'
                END");

            // Drop the enum column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Orders");

            // Rename the string column to Status
            migrationBuilder.RenameColumn(
                name: "StatusString",
                table: "Orders",
                newName: "Status");
        }
    }
}
