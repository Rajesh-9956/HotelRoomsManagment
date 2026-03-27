namespace hotel_room_api.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string JwtTokenId { get; set; }
    public string Refresh_Token { get; set; }
    public bool IsValid { get; set; }
    public DateTime ExpiredAt { get; set; }
}