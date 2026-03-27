using System.Net.Mime;
using RoomsUtility;
namespace Hotel_Rooms_MVC;

public class ApiRequest
{
    public StaticData.ApiTypes ApiType { get; set; } = StaticData.ApiTypes.GET;
    public string Url { get; set; }
    public object data { get; set; }
    public string token { get; set; }

    public StaticData.ContentType ContentType { get; set; } = StaticData.ContentType.json;
}