using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class quickfix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechCompanies_CommissionPlan_CommissionPlanId",
                table: "TechCompanies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommissionPlan",
                table: "CommissionPlan");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CommissionPlan");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CommissionPlan");

            migrationBuilder.RenameTable(
                name: "CommissionPlan",
                newName: "CommissionPlans");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "CategorySubCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommissionPlans",
                table: "CommissionPlans",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ChatRooms",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaintenanceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PCAssemblyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeliveryId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatRooms_Maintenances_MaintenanceId",
                        column: x => x.MaintenanceId,
                        principalTable: "Maintenances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatRooms_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatRooms_PCAssemblies_PCAssemblyId",
                        column: x => x.PCAssemblyId,
                        principalTable: "PCAssemblies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CommissionTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaintenanceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PCAssemblyId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DeliveryId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    ServiceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VendorPayout = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PlatformFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TechCompanyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeliveryPersonId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PayoutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PayoutReference = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommissionTransactions_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommissionTransactions_DeliveryPersons_DeliveryPersonId",
                        column: x => x.DeliveryPersonId,
                        principalTable: "DeliveryPersons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommissionTransactions_Maintenances_MaintenanceId",
                        column: x => x.MaintenanceId,
                        principalTable: "Maintenances",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommissionTransactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommissionTransactions_PCAssemblies_PCAssemblyId",
                        column: x => x.PCAssemblyId,
                        principalTable: "PCAssemblies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommissionTransactions_TechCompanies_TechCompanyId",
                        column: x => x.TechCompanyId,
                        principalTable: "TechCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChatRoomId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileSize = table.Column<long>(type: "bigint", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplyToMessageId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatMessages_ReplyToMessageId",
                        column: x => x.ReplyToMessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatParticipants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChatRoomId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastSeen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsTyping = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatParticipants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatParticipants_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChatRoomId",
                table: "ChatMessages",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ReplyToMessageId",
                table: "ChatMessages",
                column: "ReplyToMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_ChatRoomId",
                table: "ChatParticipants",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_UserId",
                table: "ChatParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_DeliveryId",
                table: "ChatRooms",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_MaintenanceId",
                table: "ChatRooms",
                column: "MaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_OrderId",
                table: "ChatRooms",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_PCAssemblyId",
                table: "ChatRooms",
                column: "PCAssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionTransactions_DeliveryId",
                table: "CommissionTransactions",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionTransactions_DeliveryPersonId",
                table: "CommissionTransactions",
                column: "DeliveryPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionTransactions_MaintenanceId",
                table: "CommissionTransactions",
                column: "MaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionTransactions_OrderId",
                table: "CommissionTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionTransactions_PCAssemblyId",
                table: "CommissionTransactions",
                column: "PCAssemblyId");

            migrationBuilder.CreateIndex(
                name: "IX_CommissionTransactions_TechCompanyId",
                table: "CommissionTransactions",
                column: "TechCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_TechCompanies_CommissionPlans_CommissionPlanId",
                table: "TechCompanies",
                column: "CommissionPlanId",
                principalTable: "CommissionPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechCompanies_CommissionPlans_CommissionPlanId",
                table: "TechCompanies");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatParticipants");

            migrationBuilder.DropTable(
                name: "CommissionTransactions");

            migrationBuilder.DropTable(
                name: "ChatRooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommissionPlans",
                table: "CommissionPlans");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CategorySubCategories");

            migrationBuilder.RenameTable(
                name: "CommissionPlans",
                newName: "CommissionPlan");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CommissionPlan",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CommissionPlan",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommissionPlan",
                table: "CommissionPlan",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TechCompanies_CommissionPlan_CommissionPlanId",
                table: "TechCompanies",
                column: "CommissionPlanId",
                principalTable: "CommissionPlan",
                principalColumn: "Id");
        }
    }
}
