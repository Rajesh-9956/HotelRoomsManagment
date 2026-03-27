using AutoMapper;
using hotel_room_api.Models;
using hotel_room_api.Models.DTOs;

namespace hotel_room_api;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<Room, RoomUpdateDTO>().ReverseMap();
        CreateMap<Room, RoomCreateDTO>().ReverseMap();

        CreateMap<Room, RoomDTO>();
        CreateMap<RoomDTO, Room>();

    }
}