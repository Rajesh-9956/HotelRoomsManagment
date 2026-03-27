namespace Hotel_Rooms_MVC.Services.IServices;

public interface IBedNumberService
{
    Task<T> GetAllAsync<T>();
    Task<T> GetAsync<T>(int id);
    Task<T> AddAsync<T>(BedNumberAddDTO Entity);
    Task<T> UpdateAsync<T>(BedNumberUpdateDTO Entity );
    Task<T> RemoveAsync<T>(int id); 

}