using Hotel_Rooms_MVC.Models;
using Hotel_Rooms_MVC.Services.IServices;
using RoomsUtility;

namespace Hotel_Rooms_MVC.Services.Services;

public class authService : IAuthService
{

    private readonly IBaseService _baseService;
    private string _userUrl;
    public authService( IConfiguration configuration, IBaseService baseService)

    {
        _baseService = baseService;

        _userUrl = configuration.GetValue<string>("ServiceUrls:RoomApi");
    }

    public async Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDto)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.POST,
            data = loginRequestDto,
            Url = _userUrl + "/api/UserAuth/Login"
        }, needBearer:false);
    }

    public async Task<T> RegisterAsync<T>(RegisterRequestDTO registerRequestDto)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.POST,
            data = registerRequestDto,
            Url = _userUrl + "/api/UserAuth/register"
        }, needBearer:false);
    }

    public async Task<T> LogoutAsync<T>(TokenDTO tokenDto)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.POST,
            data = tokenDto,
            Url = _userUrl + "/api/UserAuth/revoke"
        }, needBearer:false);
    }
}