using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel_Rooms_MVC.Models.ViewModel;

public class bedNumberAddVM
{
    public BedNumberAddDTO BedNumberAddDto { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> RoomListItems { get; set; }
}