using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETicket.Db.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Add_Seat_Version : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Seat",
                type: "bigint",
                rowVersion: true,
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Seat");
        }
    }
}
