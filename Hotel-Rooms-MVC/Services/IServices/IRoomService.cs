
namespace Hotel_Rooms_MVC.Services.IServices;

public interface IRoomService 
{
    Task<T> GetAllAsync<T>();
    Task<T> GetAsync<T>(int id);
    Task<T> AddAsync<T>(RoomCreateDTO Entity);
    Task<T> UpdateAsync<T>(RoomUpdateDTO Entity);
    Task<T> RemoveAsync<T>(int id); 
    
}