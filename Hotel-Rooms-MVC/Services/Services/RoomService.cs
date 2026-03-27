using Hotel_Rooms_MVC.Services.IServices;
using RoomsUtility;

namespace Hotel_Rooms_MVC.Services.Services;

public class RoomService : IRoomService
{

    private readonly IBaseService _baseService;
    private string _roomUrl;
    public RoomService( IConfiguration configuration, IBaseService baseService)
    {
        _baseService = baseService;
        _roomUrl = configuration.GetValue<string>("ServiceUrls:RoomApi");
    }

    public async Task<T> AddAsync<T>(RoomCreateDTO Entity)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.POST,
            data = Entity,
            Url = _roomUrl + "/api/RoomApi",
            ContentType = StaticData.ContentType.MultipartFormData
        });
    }
    
    public async Task<T> GetAllAsync<T>()
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.GET,
            Url = _roomUrl + "/api/RoomApi"
        });
    }

    public async Task<T> GetAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.GET,
            Url = _roomUrl + "/api/RoomApi/" + id
        });
    }

    public async Task<T> UpdateAsync<T>(RoomUpdateDTO Entity)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.PUT,
            data = Entity,
            Url = _roomUrl + "/api/RoomApi/" + Entity.Id,
            ContentType = StaticData.ContentType.MultipartFormData
        });
    }
    
    public async Task<T> RemoveAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.DELETE,
            Url = _roomUrl + "/api/RoomApi/" + id
        });
    }
}