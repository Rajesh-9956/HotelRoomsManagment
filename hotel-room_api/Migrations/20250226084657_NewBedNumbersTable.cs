using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hotel_room_api.Migrations
{
    /// <inheritdoc />
    public partial class NewBedNumbersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomsNumbers");

            migrationBuilder.CreateTable(
                name: "BedNumbers",
                columns: table => new
                {
                    bedNo = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    specialDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BedNumbers", x => x.bedNo);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BedNumbers");

            migrationBuilder.CreateTable(
                name: "RoomsNumbers",
                columns: table => new
                {
                    RoomNo = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UpdatedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    specialDetails = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomsNumbers", x => x.RoomNo);
                });
        }
    }
}
