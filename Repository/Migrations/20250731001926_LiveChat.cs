using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class LiveChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Deliveries_DeliveryId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryStatus",
                table: "Deliveries");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "TechCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "TechCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommissionPlanId",
                table: "TechCompanies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "TechCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TechCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TechCompanies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "TechCompanies",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "TechCompanies",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "TechCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "TechCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "TechCompanies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AssemblyFee",
                table: "PCAssemblies",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "PCAssemblies",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "PCAssemblies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PCAssemblies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PCAssemblies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TechCompanyId",
                table: "PCAssemblies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Maintenances",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DeviceImages",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceType",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Issue",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceFee",
                table: "Maintenances",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderId",
                table: "Deliveries",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupAddress",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PickupDate",
                table: "Deliveries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Deliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CommissionPlan",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductSaleCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaintenanceCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PCAssemblyCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryCommission = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthlySubscriptionFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionPlan", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechCompanies_CommissionPlanId",
                table: "TechCompanies",
                column: "CommissionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PCAssemblies_TechCompanyId",
                table: "PCAssemblies",
                column: "TechCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Orders_OrderId",
                table: "Deliveries",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PCAssemblies_TechCompanies_TechCompanyId",
                table: "PCAssemblies",
                column: "TechCompanyId",
                principalTable: "TechCompanies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TechCompanies_CommissionPlan_CommissionPlanId",
                table: "TechCompanies",
                column: "CommissionPlanId",
                principalTable: "CommissionPlan",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Orders_OrderId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_PCAssemblies_TechCompanies_TechCompanyId",
                table: "PCAssemblies");

            migrationBuilder.DropForeignKey(
                name: "FK_TechCompanies_CommissionPlan_CommissionPlanId",
                table: "TechCompanies");

            migrationBuilder.DropTable(
                name: "CommissionPlan");

            migrationBuilder.DropIndex(
                name: "IX_TechCompanies_CommissionPlanId",
                table: "TechCompanies");

            migrationBuilder.DropIndex(
                name: "IX_PCAssemblies_TechCompanyId",
                table: "PCAssemblies");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_OrderId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "TechCompanies");

            migrationBuilder.DropColumn(
                name: "City",
                table: "TechCompanies");

            migrationBuilder.DropColumn(
                name: "CommissionPlanId",
                table: "TechCompanies");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "TechCompanies");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "TechCompanies");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TechCompanies");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "TechCompanies");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "TechCompanies");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "TechCompanies");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "TechCompanies");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "TechCompanies");

            migrationBuilder.DropColumn(
                name: "AssemblyFee",
                table: "PCAssemblies");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "PCAssemblies");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "PCAssemblies");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PCAssemblies");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PCAssemblies");

            migrationBuilder.DropColumn(
                name: "TechCompanyId",
                table: "PCAssemblies");

            migrationBuilder.DropColumn(
                name: "DeviceImages",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Issue",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "ServiceFee",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "PickupAddress",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "PickupDate",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Maintenances",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryStatus",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryId",
                table: "Orders",
                column: "DeliveryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Deliveries_DeliveryId",
                table: "Orders",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id");
        }
    }
}
