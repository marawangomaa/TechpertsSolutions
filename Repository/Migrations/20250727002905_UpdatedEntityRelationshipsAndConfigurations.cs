using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEntityRelationshipsAndConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Deliveries_DeliveryId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_ServiceUsages_ServiceUsageId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Warranties_WarrantyId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Deliveries_DeliveryId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderHistories_OrderHistoryId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_SalesManagers_SalesManagerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ServiceUsages_ServiceUsageId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_StockControlManagers_StockControlManagerId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_TechManagers_TechManagerId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_WishListItems_Carts_CartId",
                table: "WishListItems");

            migrationBuilder.DropTable(
                name: "SalesManagers");

            migrationBuilder.DropTable(
                name: "StockControlManagers");

            migrationBuilder.DropTable(
                name: "TechManagers");

            migrationBuilder.DropIndex(
                name: "IX_WishListItems_CartId",
                table: "WishListItems");

            migrationBuilder.DropIndex(
                name: "IX_Products_StockControlManagerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TechManagerId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SalesManagerId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_ServiceUsageId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_WarrantyId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_Customers_DeliveryId",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderHistories",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "CartId",
                table: "WishListItems");

            migrationBuilder.DropColumn(
                name: "StockControlManagerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TechManagerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalesManagerId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ServiceUsageId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "Customers");

            migrationBuilder.RenameTable(
                name: "OrderHistories",
                newName: "OrderHistory");

            migrationBuilder.AddColumn<string>(
                name: "MaintenanceId",
                table: "ServiceUsages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServiceUsageId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "WarrantyId",
                table: "Maintenances",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualDeliveryDate",
                table: "Deliveries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Deliveries",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhone",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryFee",
                table: "Deliveries",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryPersonId",
                table: "Deliveries",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryStatus",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedDeliveryDate",
                table: "Deliveries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderHistory",
                table: "OrderHistory",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DeliveryPersons",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VehicleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryPersons_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryPersons_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceUsages_MaintenanceId",
                table: "ServiceUsages",
                column: "MaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_WarrantyId",
                table: "Maintenances",
                column: "WarrantyId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_CustomerId",
                table: "Deliveries",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_DeliveryPersonId",
                table: "Deliveries",
                column: "DeliveryPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPersons_RoleId",
                table: "DeliveryPersons",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryPersons_UserId",
                table: "DeliveryPersons",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Customers_CustomerId",
                table: "Deliveries",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_DeliveryPersons_DeliveryPersonId",
                table: "Deliveries",
                column: "DeliveryPersonId",
                principalTable: "DeliveryPersons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Warranties_WarrantyId",
                table: "Maintenances",
                column: "WarrantyId",
                principalTable: "Warranties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Deliveries_DeliveryId",
                table: "Orders",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderHistory_OrderHistoryId",
                table: "Orders",
                column: "OrderHistoryId",
                principalTable: "OrderHistory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ServiceUsages_ServiceUsageId",
                table: "Orders",
                column: "ServiceUsageId",
                principalTable: "ServiceUsages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceUsages_Maintenances_MaintenanceId",
                table: "ServiceUsages",
                column: "MaintenanceId",
                principalTable: "Maintenances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Customers_CustomerId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_DeliveryPersons_DeliveryPersonId",
                table: "Deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Warranties_WarrantyId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Deliveries_DeliveryId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderHistory_OrderHistoryId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ServiceUsages_ServiceUsageId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceUsages_Maintenances_MaintenanceId",
                table: "ServiceUsages");

            migrationBuilder.DropTable(
                name: "DeliveryPersons");

            migrationBuilder.DropIndex(
                name: "IX_ServiceUsages_MaintenanceId",
                table: "ServiceUsages");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_WarrantyId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_CustomerId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_DeliveryPersonId",
                table: "Deliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderHistory",
                table: "OrderHistory");

            migrationBuilder.DropColumn(
                name: "MaintenanceId",
                table: "ServiceUsages");

            migrationBuilder.DropColumn(
                name: "ActualDeliveryDate",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "CustomerPhone",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryFee",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryPersonId",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "DeliveryStatus",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "EstimatedDeliveryDate",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "Deliveries");

            migrationBuilder.RenameTable(
                name: "OrderHistory",
                newName: "OrderHistories");

            migrationBuilder.AddColumn<string>(
                name: "CartId",
                table: "WishListItems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StockControlManagerId",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TechManagerId",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceUsageId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesManagerId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "WarrantyId",
                table: "Maintenances",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceUsageId",
                table: "Maintenances",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryId",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderHistories",
                table: "OrderHistories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SalesManagers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesManagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesManagers_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SalesManagers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockControlManagers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockControlManagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockControlManagers_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockControlManagers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechManagers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechManagers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechManagers_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechManagers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WishListItems_CartId",
                table: "WishListItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_StockControlManagerId",
                table: "Products",
                column: "StockControlManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TechManagerId",
                table: "Products",
                column: "TechManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SalesManagerId",
                table: "Orders",
                column: "SalesManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_ServiceUsageId",
                table: "Maintenances",
                column: "ServiceUsageId");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_WarrantyId",
                table: "Maintenances",
                column: "WarrantyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DeliveryId",
                table: "Customers",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesManagers_RoleId",
                table: "SalesManagers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesManagers_UserId",
                table: "SalesManagers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockControlManagers_RoleId",
                table: "StockControlManagers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_StockControlManagers_UserId",
                table: "StockControlManagers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechManagers_RoleId",
                table: "TechManagers",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TechManagers_UserId",
                table: "TechManagers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Deliveries_DeliveryId",
                table: "Customers",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_ServiceUsages_ServiceUsageId",
                table: "Maintenances",
                column: "ServiceUsageId",
                principalTable: "ServiceUsages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Warranties_WarrantyId",
                table: "Maintenances",
                column: "WarrantyId",
                principalTable: "Warranties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Deliveries_DeliveryId",
                table: "Orders",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderHistories_OrderHistoryId",
                table: "Orders",
                column: "OrderHistoryId",
                principalTable: "OrderHistories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_SalesManagers_SalesManagerId",
                table: "Orders",
                column: "SalesManagerId",
                principalTable: "SalesManagers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ServiceUsages_ServiceUsageId",
                table: "Orders",
                column: "ServiceUsageId",
                principalTable: "ServiceUsages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_StockControlManagers_StockControlManagerId",
                table: "Products",
                column: "StockControlManagerId",
                principalTable: "StockControlManagers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_TechManagers_TechManagerId",
                table: "Products",
                column: "TechManagerId",
                principalTable: "TechManagers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishListItems_Carts_CartId",
                table: "WishListItems",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
