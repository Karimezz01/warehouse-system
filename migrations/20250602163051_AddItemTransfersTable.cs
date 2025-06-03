using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace warehousesystem.Migrations
{
    public partial class AddItemTransfersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransferItems",
                columns: table => new
                {
                    TransferID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferDate = table.Column<DateTime>(nullable: false),
                    ItemID = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    SourceWarehouseID = table.Column<int>(nullable: false),
                    DestinationWarehouseID = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferItems", x => x.TransferID);
                    table.ForeignKey(
                        name: "FK_TransferItems_Warehouses_DestinationWarehouseID",
                        column: x => x.DestinationWarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferItems_Items_ItemID",
                        column: x => x.ItemID,
                        principalTable: "Items",
                        principalColumn: "ItemID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransferItems_Warehouses_SourceWarehouseID",
                        column: x => x.SourceWarehouseID,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_DestinationWarehouseID",
                table: "TransferItems",
                column: "DestinationWarehouseID");

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_ItemID",
                table: "TransferItems",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_SourceWarehouseID",
                table: "TransferItems",
                column: "SourceWarehouseID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransferItems");
        }
    }
}
