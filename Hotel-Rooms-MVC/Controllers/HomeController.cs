using System.Diagnostics;
using hotel_room_api;
using Microsoft.AspNetCore.Mvc;
using Hotel_Rooms_MVC.Models;
using Hotel_Rooms_MVC;
using Hotel_Rooms_MVC.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using RoomsUtility;

namespace Hotel_Rooms_MVC.Controllers;

public class HomeController : Controller
{
    public readonly IRoomService _RoomService;
    
    public HomeController(IRoomService roomService)
    {
        _RoomService = roomService;
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Index()
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

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}