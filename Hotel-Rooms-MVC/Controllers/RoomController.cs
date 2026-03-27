using hotel_room_api;
using Hotel_Rooms_MVC;
using Hotel_Rooms_MVC.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RoomsUtility;

namespace Hotel_Rooms_MVC.Controllers;

public class RoomController : Controller
{
    public readonly IRoomService _RoomService;
    
    public RoomController(IRoomService roomService)
    {
        _RoomService = roomService;
    }
    
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> IndexRoom()
    {
        List<RoomDTO> roomsList = new();

        var response = await _RoomService.GetAllAsync<APIResponse>();
        if (response != null && response.IsSuccess)
        {
            roomsList = JsonConvert.DeserializeObject<List<RoomDTO>>
                (Convert.ToString(response.Result));
        }
        
        return View(roomsList);
    }

    public async Task<IActionResult> CreateRoom()
    {
        return View();
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRoom( RoomCreateDTO newRoomDto)
    {
        if (ModelState.IsValid)
        {
            var response = await _RoomService.AddAsync<APIResponse>(newRoomDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Room Added Successfully";
                return RedirectToAction(nameof(IndexRoom));
            }
        }
        TempData["error"] = "Error When Create";
        return View(newRoomDto);
    }
    
    public async Task<IActionResult> UpdateRoom(int id)
    {
        var response = await _RoomService.GetAsync<APIResponse>(id);
        if (response != null && response.IsSuccess)
        {
            RoomDTO room = JsonConvert.DeserializeObject<RoomDTO>(Convert.ToString(response.Result));
            RoomUpdateDTO updateRoom = new RoomUpdateDTO()
            {
                Id = room.Id,
                Name = room.Name,
                Details = room.Details,
                NumberOfBeds = room.NumberOfBeds,
                Rate = room.Rate,
                Amenity = room.Amenity,
                ImageUrl = room.ImageUrl,
                SpaceByMiter = room.SpaceByMiter
            };
            return View(updateRoom);
        }

        return NotFound();
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRoom( RoomUpdateDTO updateRoom)
    {
        if (ModelState.IsValid)
        {
            var response = await _RoomService.UpdateAsync<APIResponse>(updateRoom);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Room Updated Successfully";
                return RedirectToAction(nameof(IndexRoom));
            }
        }
        TempData["error"] = "Error When Update";
        return View(updateRoom);
    }
    
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var response = await _RoomService.GetAsync<APIResponse>(id);
        if (response != null && response.IsSuccess)
        {
            RoomDTO room = JsonConvert.DeserializeObject<RoomDTO>(Convert.ToString(response.Result));
            
            return View(room);
        }
        return NotFound();
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRoom( RoomDTO room)
    {
        var response = await _RoomService.RemoveAsync<APIResponse>(room.Id);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Room Deleted Successfully";
            return RedirectToAction(nameof(IndexRoom));
        }
            
        TempData["error"] = "Error When Delete";
        return View(room);
    }
    

}