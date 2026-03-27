namespace hotel_room_api.Models.DTOs.InternalDTO;

public class RegisterRequestDTO
{
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}