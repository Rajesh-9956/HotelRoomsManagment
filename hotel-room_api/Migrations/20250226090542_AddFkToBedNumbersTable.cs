using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotel_room_api.Migrations
{
    /// <inheritdoc />
    public partial class AddFkToBedNumbersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "BedNumbers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BedNumbers_RoomId",
                table: "BedNumbers",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_BedNumbers_Rooms_RoomId",
                table: "BedNumbers",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BedNumbers_Rooms_RoomId",
                table: "BedNumbers");

            migrationBuilder.DropIndex(
                name: "IX_BedNumbers_RoomId",
                table: "BedNumbers");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "BedNumbers");
        }
    }
}
