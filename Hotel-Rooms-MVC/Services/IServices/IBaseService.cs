using Hotel_Rooms_MVC.Models;

namespace Hotel_Rooms_MVC.Services.IServices;

public interface IBaseService
{
    Task<T> SendAsync<T>(ApiRequest apiRequest, bool needBearer = true);
}