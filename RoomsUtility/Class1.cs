namespace RoomsUtility;

public static class StaticData
{
    public enum ApiTypes
    {
        GET, POST, PUT, DELETE, PATCH
        
    }
    public enum ContentType
    {
        json,
        MultipartFormData
        
    }

    public static string AccessToken = "JWTToken";
    public static string RefreshToken = "RefToken";
}