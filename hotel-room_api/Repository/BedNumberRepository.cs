using hotel_room_api.Data;
using hotel_room_api.Models;
using hotel_room_api.Repository.IRepository;

namespace hotel_room_api.Repository;

public class BedNumberRepository : Repository<BedNumber>, IBedNumber
{
    public AppDbContext _db;
    public BedNumberRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task update(BedNumber Entity)
    {
        Entity.UpdatedDate = DateOnly.FromDateTime(DateTime.UtcNow);
        _db.BedNumbers.Update(Entity);
        await _db.SaveChangesAsync();
    }
}