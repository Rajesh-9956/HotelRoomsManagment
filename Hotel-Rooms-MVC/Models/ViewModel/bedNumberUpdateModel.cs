using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel_Rooms_MVC.Models.ViewModel;

public class bedNumberUpdateModel
{
    public BedNumberUpdateDTO BedNumberUpdateDto { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> RoomListItems { get; set; }

}