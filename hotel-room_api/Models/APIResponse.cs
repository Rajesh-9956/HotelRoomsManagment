using System.Net;

namespace hotel_room_api.Models;

public class APIResponse
{
    public HttpStatusCode StatusCode { get; set; }

    public bool IsSuccess { get; set; } = true;

    public List<String> ErrorMessages { get; set; }

    public object Result { get; set; }
}