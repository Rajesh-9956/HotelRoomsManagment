using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hotel_room_api.Models;

public class Room
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string  Name { get; set; }
    public string  Details { get; set; }
    public int  Rate { get; set; }
    public int  SpaceByMiter { get; set; }
    public int  NumberOfBeds { get; set; }
    public string?  ImageUrl { get; set; }
    public string?  ImageLocalPath { get; set; }
    public string  Amenity { get; set; }
    public DateTime createdDate { get; set; }
    public DateTime updatedDate { get; set; }
    
}