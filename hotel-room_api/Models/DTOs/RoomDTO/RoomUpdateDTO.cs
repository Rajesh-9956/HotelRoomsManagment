using System.ComponentModel.DataAnnotations;

namespace hotel_room_api.Models.DTOs;

public class RoomUpdateDTO
{
    [Required]
    public int Id { get; set; }
    [Required]
    [MaxLength(30)]
    public string  Name { get; set; }
    public string  Details { get; set; }
    [Required]
    public int  Rate { get; set; }
    [Required]
    public int SpaceByMiter { get; set; }
    [Required]
    public int NumberOfBeds { get; set; } 
    public string?  ImageUrl { get; set; }
    public string?  ImageLocalPath { get; set; }
    public IFormFile? Image { get; set; }
    [Required]
    public string  Amenity { get; set; }

}