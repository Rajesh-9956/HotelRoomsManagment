using System.ComponentModel.DataAnnotations;

namespace Hotel_Rooms_MVC;

public class BedNumberUpdateDTO
{
    public int bedNo { get; set; }
    [Required]
    public int RoomId { get; set; }
    public string? specialDetails { get; set; }
    
}