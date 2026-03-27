using hotel_room_api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace hotel_room_api.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<BedNumber> BedNumbers { get; set; }
    public DbSet<InternalUser> InternalUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = 1,
                Name = "Deluxe King Room",
                Details = "A luxurious room with a king-sized bed and a stunning city view.",
                Rate = 250,
                SpaceByMiter = 35,
                NumberOfBeds = 1,
                ImageUrl = "",
                Amenity = "Free WiFi, Air Conditioning, Smart TV, Mini Bar",
                createdDate = new DateTime(2024, 2, 17)
            },
            new Room
            {
                Id = 2,
                Name = "Family Suite",
                Details = "Spacious suite ideal for families, featuring two bedrooms and a living area.",
                Rate = 400,
                SpaceByMiter = 60,
                NumberOfBeds = 3,
                ImageUrl = "",
                Amenity = "Free WiFi, Kitchenette, Air Conditioning, Smart TV",
                createdDate = new DateTime(2024, 2, 17)
            },
            new Room
            {
                Id = 3,
                Name = "Standard Twin Room",
                Details = "A comfortable room with two twin beds, perfect for business travelers.",
                Rate = 180,
                SpaceByMiter = 28,
                NumberOfBeds = 2,
                ImageUrl = "",
                Amenity = "Free WiFi, Work Desk, Air Conditioning, Smart TV",
                createdDate = new DateTime(2024, 2, 17)
            },
            new Room
            {
                Id = 4,
                Name = "Presidential Suite",
                Details = "An exclusive suite with a private lounge, a king-sized bed, and personalized service.",
                Rate = 800,
                SpaceByMiter = 100,
                NumberOfBeds = 1,
                ImageUrl = "",
                Amenity = "Private Butler, Jacuzzi, Free WiFi, Smart TV, Mini Bar",
                createdDate = new DateTime(2024, 2, 17)
            },
            new Room
            {
                Id = 5,
                Name = "Economy Room",
                Details = "A budget-friendly room with essential amenities for a comfortable stay.",
                Rate = 120,
                SpaceByMiter = 20,
                NumberOfBeds = 1,
                ImageUrl = "",
                Amenity = "Free WiFi, Work Desk, Air Conditioning",
                createdDate = new DateTime(2024, 2, 17)
            }
        );



    }
    
}