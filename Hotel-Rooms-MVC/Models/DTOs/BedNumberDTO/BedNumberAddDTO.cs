using System.ComponentModel.DataAnnotations;

namespace Hotel_Rooms_MVC;

public class BedNumberAddDTO
{
    public int bedNo { get; set; }
    [Required]
    public int RoomId { get; set; }
    public string? specialDetails { get; set; }
}