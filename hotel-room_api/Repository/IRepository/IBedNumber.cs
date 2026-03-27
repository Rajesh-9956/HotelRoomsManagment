using hotel_room_api.Models;

namespace hotel_room_api.Repository.IRepository;

public interface IBedNumber : IRepository<BedNumber>
{
    Task update(BedNumber Entity);
}