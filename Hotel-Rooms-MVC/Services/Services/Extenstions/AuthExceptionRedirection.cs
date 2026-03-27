using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hotel_Rooms_MVC.Services.Services.Extenstions;

public class AuthExceptionRedirection : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is AuthException)
        {
            context.Result = new RedirectToActionResult("login", "auth", null);
        }
    }
}