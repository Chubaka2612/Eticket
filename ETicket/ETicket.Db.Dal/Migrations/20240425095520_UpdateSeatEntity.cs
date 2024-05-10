using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETicket.Db.Dal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeatEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "Seat");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EventVenue");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "EventVenue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "OrderItemId",
                table: "Seat",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EventVenue",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "EventVenue",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");
        }
    }
}
