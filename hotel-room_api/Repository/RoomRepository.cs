using System.Linq.Expressions;
using hotel_room_api.Data;
using hotel_room_api.Models;
using hotel_room_api.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace hotel_room_api.Repository;

public class RoomRepository(AppDbContext db) : Repository<Room>(db), IRoomRepository
{

    private readonly AppDbContext _db = db;


    public async Task UpdateAsync(Room Entity)
    {
        Entity.updatedDate = DateTime.UtcNow;
        _db.Rooms.Update(Entity);
        await _db.SaveChangesAsync();
    }

   
}