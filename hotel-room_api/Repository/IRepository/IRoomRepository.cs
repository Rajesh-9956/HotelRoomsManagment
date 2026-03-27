using System.Linq.Expressions;
using hotel_room_api.Models;

namespace hotel_room_api.Repository.IRepository;

public interface IRoomRepository : IRepository<Room>
{
    Task UpdateAsync(Room Entity);
}