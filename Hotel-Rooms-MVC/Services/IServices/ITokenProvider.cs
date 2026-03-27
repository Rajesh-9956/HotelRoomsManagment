using Hotel_Rooms_MVC.Models;

namespace Hotel_Rooms_MVC.Services.IServices;

public interface ITokenProvider
{
    void SetToken(TokenDTO tokenDto);
    TokenDTO? GetToken();
    void ClearToken();
}