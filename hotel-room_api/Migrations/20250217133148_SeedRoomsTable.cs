using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace hotel_room_api.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoomsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Amenity", "Details", "ImageUrl", "Name", "NumberOfBeds", "Rate", "SpaceByMiter", "createdDate", "updatedDate" },
                values: new object[,]
                {
                    { 1, "Free WiFi, Air Conditioning, Smart TV, Mini Bar", "A luxurious room with a king-sized bed and a stunning city view.", "https://example.com/images/deluxe-king.jpg", "Deluxe King Room", 1, 250, 35, new DateTime(2024, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Free WiFi, Kitchenette, Air Conditioning, Smart TV", "Spacious suite ideal for families, featuring two bedrooms and a living area.", "https://example.com/images/family-suite.jpg", "Family Suite", 3, 400, 60, new DateTime(2024, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Free WiFi, Work Desk, Air Conditioning, Smart TV", "A comfortable room with two twin beds, perfect for business travelers.", "https://example.com/images/standard-twin.jpg", "Standard Twin Room", 2, 180, 28, new DateTime(2024, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "Private Butler, Jacuzzi, Free WiFi, Smart TV, Mini Bar", "An exclusive suite with a private lounge, a king-sized bed, and personalized service.", "https://example.com/images/presidential-suite.jpg", "Presidential Suite", 1, 800, 100, new DateTime(2024, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "Free WiFi, Work Desk, Air Conditioning", "A budget-friendly room with essential amenities for a comfortable stay.", "https://example.com/images/economy-room.jpg", "Economy Room", 1, 120, 20, new DateTime(2024, 2, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
