using System.ComponentModel.DataAnnotations;

namespace hotel_room_api.Models.DTOs.RoomNumberDTO;

public class BedNumberDTO
{
    public int bedNo { get; set; }
    [Required]
    public int RoomId { get; set; }
    public string? specialDetails { get; set; }
    
    public  RoomDTO Room { get; set; }
}