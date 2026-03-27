using hotel_room_api;
using Hotel_Rooms_MVC;
using Hotel_Rooms_MVC.Models.ViewModel;
using Hotel_Rooms_MVC.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RoomsUtility;

namespace Hotel_Rooms_MVC.Controllers;

public class BedNumber : Controller
{
    public readonly IBedNumberService _BedNumberService;
    public readonly IRoomService _RoomService;
    
    public BedNumber(IBedNumberService bedNumberService, IRoomService RoomService)
    {
        _BedNumberService = bedNumberService;
        _RoomService = RoomService;
    }
    
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> IndexBedNumber()
    {
        List<BedNumberDTO> BedNumbersList = new();

        var response = await _BedNumberService.GetAllAsync<APIResponse>();
        if (response != null && response.IsSuccess)
        {
            BedNumbersList = JsonConvert.DeserializeObject<List<BedNumberDTO>>
                (Convert.ToString(response.Result));
        }

        return View(BedNumbersList);
    }
    
    public async Task<IActionResult> AddNewBedNumber()
    {
        bedNumberAddVM bedNumberAddVm = new();
        var response = await _RoomService.GetAllAsync<APIResponse>();
        if (response != null && response.IsSuccess)
        {
            bedNumberAddVm.RoomListItems = JsonConvert.DeserializeObject<List<RoomDTO>>
                (Convert.ToString(response.Result)).Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }

        return View(bedNumberAddVm);
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddNewBedNumber( bedNumberAddVM newbedNumberAddVM)
    {
        if (ModelState.IsValid)
        {
            APIResponse response = await _BedNumberService.AddAsync<APIResponse>(newbedNumberAddVM.BedNumberAddDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Bed Number Added Successfully";
                return RedirectToAction(nameof(IndexBedNumber));
            }
            else
            {
                if (response.ErrorMessages.Count > 0)
                {
                    ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                }
            }
        }
        var res = await _RoomService.GetAllAsync<APIResponse>();
        if (res != null && res.IsSuccess)
        {
            newbedNumberAddVM.RoomListItems = JsonConvert.DeserializeObject<List<RoomDTO>>
                (Convert.ToString(res.Result)).Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }
        else
        {
            TempData["error"] = (res?.ErrorMessages.Count() > 0) ? 
                res.ErrorMessages[0] : "Error Encountered";
        }
        
        return View(newbedNumberAddVM);
    }
    
    
    
    public async Task<IActionResult> UpdateBedNumber(int id)
    {
        bedNumberUpdateModel bedNumberUpdateVm = new();
        var bedNumberResponse =await _BedNumberService.GetAsync<APIResponse>(id);
        var response = await _RoomService.GetAllAsync<APIResponse>();
        
        if (bedNumberResponse != null && response != null && response.IsSuccess)
        {
            bedNumberUpdateVm.RoomListItems = JsonConvert.DeserializeObject<List<RoomDTO>>
                (Convert.ToString(response.Result)).Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            BedNumberDTO model = JsonConvert.DeserializeObject<BedNumberDTO>
                (Convert.ToString(bedNumberResponse.Result));
            bedNumberUpdateVm.BedNumberUpdateDto = new BedNumberUpdateDTO()
            {
                bedNo = model.bedNo,
                RoomId = model.RoomId
            };
        }

        return View(bedNumberUpdateVm);
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBedNumber( bedNumberUpdateModel updatedBed)
    {
        if (ModelState.IsValid)
        {
            var bedNumberResponse = _BedNumberService.GetAsync<APIResponse>(updatedBed.BedNumberUpdateDto.bedNo);
            APIResponse response = await _BedNumberService.UpdateAsync<APIResponse>(updatedBed.BedNumberUpdateDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Bed Number Updated Successfully";
                return RedirectToAction(nameof(IndexBedNumber));
            }
            else
            {
                if (response.ErrorMessages.Count > 0)
                {
                    ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                    
                }
            }
        }
        
        var res = await _RoomService.GetAllAsync<APIResponse>();
        if (res != null && res.IsSuccess)
        {
            updatedBed.RoomListItems = JsonConvert.DeserializeObject<List<RoomDTO>>
                (Convert.ToString(res.Result)).Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }
        
        TempData["error"] = "Error When Update";
        return View(updatedBed);
    }
    
    
    public async Task<IActionResult> DeleteBedNumber(int id)
    {
        bedNumberModel bedNumberVm = new();
        var response = await _RoomService.GetAllAsync<APIResponse>();
        var bedNumberResponse =await _BedNumberService.GetAsync<APIResponse>(id);
        
        if (bedNumberResponse != null && response != null && response.IsSuccess)
        {
            bedNumberVm.RoomListItems = JsonConvert.DeserializeObject<List<RoomDTO>>
                (Convert.ToString(response.Result)).Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            BedNumberDTO model = JsonConvert.DeserializeObject<BedNumberDTO>
                (Convert.ToString(bedNumberResponse.Result));
            bedNumberVm.BedNumberDto = new BedNumberDTO()
            {
                bedNo = model.bedNo,
                RoomId = model.RoomId
            };
        }
        else
        {
            TempData["error"] = (response?.ErrorMessages.Count() > 0) ? 
                response.ErrorMessages[0] : "Error Encountered";
        }

        return View(bedNumberVm);
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBedNumber( bedNumberModel deletedBed)
    {
        var response = await _BedNumberService.RemoveAsync<APIResponse>(deletedBed.BedNumberDto.bedNo);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Bed Number Deleted Successfully";
            return RedirectToAction(nameof(IndexBedNumber));
        }
        else
        {
            TempData["error"] = (response?.ErrorMessages.Count() > 0) ? 
                response.ErrorMessages[0] : "Error Encountered";
        }
        
        return View(deletedBed);
        
    }

}