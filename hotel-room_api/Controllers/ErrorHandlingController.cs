using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace hotel_room_api.Controllers;

[Route("ErrorHandling")]
[ApiController]
[AllowAnonymous]
public class ErrorHandlingController : ControllerBase
{
    [Route("ProcessError")]
    [HttpGet]
    public IActionResult ProcessError()
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request.",
            Detail = "Please try again later."
        };
        
        return StatusCode(500, problemDetails);
    }
}