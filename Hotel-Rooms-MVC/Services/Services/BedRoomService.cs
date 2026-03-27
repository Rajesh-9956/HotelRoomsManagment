using Hotel_Rooms_MVC.Services.IServices;
using RoomsUtility;

namespace Hotel_Rooms_MVC.Services.Services;

public class BedRoomService: IBedNumberService
{
    
    private readonly IBaseService _baseService;
    private string _roomUrl;
    public BedRoomService(IConfiguration configuration, IBaseService baseService)
    {
        _roomUrl = configuration.GetValue<string>("ServiceUrls:RoomApi");
        _baseService = baseService;
    }

    public async Task<T> AddAsync<T>(BedNumberAddDTO Entity)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.POST,
            data = Entity,
            Url = _roomUrl + "/api/bedNumber"
        });
    }
    
    public async Task<T> GetAllAsync<T>()
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.GET,
            Url = _roomUrl + "/api/bedNumber"
        });
    }

    public async Task<T> GetAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.GET,
            Url = _roomUrl + "/api/bedNumber/" + id
            
        });
    }

    public  async Task<T> UpdateAsync<T>(BedNumberUpdateDTO Entity)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.PUT,
            data = Entity,
            Url = _roomUrl + "/api/bedNumber/" + Entity.bedNo
        });
    }

    public async Task<T> RemoveAsync<T>(int id)
    {
        return await _baseService.SendAsync<T>(new ApiRequest()
        {
            ApiType = StaticData.ApiTypes.DELETE,
            Url = _roomUrl + "/api/bedNumber/" + id
        });
    }

}