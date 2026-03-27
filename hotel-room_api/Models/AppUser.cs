using Microsoft.AspNetCore.Identity;

namespace hotel_room_api.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; }
}