using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETicket.Db.Dal.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePriceEntityName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Prices_PriceId",
                table: "OrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prices",
                table: "Prices");

            migrationBuilder.RenameTable(
                name: "Prices",
                newName: "Price");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Price",
                table: "Price",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Price_PriceId",
                table: "OrderItem",
                column: "PriceId",
                principalTable: "Price",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Price_PriceId",
                table: "OrderItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Price",
                table: "Price");

            migrationBuilder.RenameTable(
                name: "Price",
                newName: "Prices");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prices",
                table: "Prices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Prices_PriceId",
                table: "OrderItem",
                column: "PriceId",
                principalTable: "Prices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
