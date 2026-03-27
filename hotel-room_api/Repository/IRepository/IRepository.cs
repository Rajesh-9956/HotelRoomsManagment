using System.Linq.Expressions;

namespace hotel_room_api.Repository.IRepository;

public interface IRepository<T> where T : class 
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> Filter = null, Expression<Func<T, object>>[] Includes = null, 
        int pageSize = 4, int pageNumber = 1);
    Task<T> GetAsync(Expression<Func<T, bool>> Filter = null, Expression<Func<T, object>>[] Includes = null, bool tracked = true);
    Task AddAsync(T Entity);
    Task RemoveAsync(T Entity); 
    Task  SaveAsync();
}