namespace Hotel_Rooms_MVC.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
public class bedNumberModel
{
    public BedNumberDTO BedNumberDto { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> RoomListItems { get; set; }

}