using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Rooms_MVC;

public class Room
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string  Name { get; set; }
    public string  Details { get; set; }
    [Required]
    public int  Rate { get; set; }
    [Required]
    public int  SpaceByMiter { get; set; }
    [Required]
    public int  NumberOfBeds { get; set; }
    public string?  ImageUrl { get; set; }
    public string  Amenity { get; set; }
    public DateTime createdDate { get; set; }
    public DateTime updatedDate { get; set; }
    

}