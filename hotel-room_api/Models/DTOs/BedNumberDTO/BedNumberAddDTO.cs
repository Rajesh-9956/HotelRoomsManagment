using System.ComponentModel.DataAnnotations;

namespace hotel_room_api.Models.DTOs.RoomNumberDTO;

public class BedNumberAddDTO
{
    public int bedNo { get; set; }
    [Required]
    public int RoomId { get; set; }
    public string? specialDetails { get; set; }
}