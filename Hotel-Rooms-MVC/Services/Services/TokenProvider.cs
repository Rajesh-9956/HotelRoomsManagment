using System.IdentityModel.Tokens.Jwt;
using Hotel_Rooms_MVC.Models;
using Hotel_Rooms_MVC.Services.IServices;
using RoomsUtility;

namespace Hotel_Rooms_MVC.Services.Services;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor _contextAccessor;

    public TokenProvider(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    
    
    public void SetToken(TokenDTO tokenDto)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(tokenDto.AccessToken);
        
        var cookieOption = new CookieOptions { Expires = jwt.ValidTo };
        var cookieOptionRefresh = new CookieOptions { Expires = jwt.ValidTo.AddDays(1) };
        _contextAccessor.HttpContext?.Response.Cookies.Append(StaticData.AccessToken, tokenDto.AccessToken, cookieOption);
        _contextAccessor.HttpContext?.Response.Cookies.Append(StaticData.RefreshToken, tokenDto.RefreshAccessToken, cookieOptionRefresh);
    }

    public TokenDTO? GetToken()
    {
        try
        {
            bool hasAccessToken =  _contextAccessor.HttpContext.Request.Cookies.TryGetValue(StaticData.AccessToken, out string accessToken);
            bool hasRefreshToken =  _contextAccessor.HttpContext.Request.Cookies.TryGetValue(StaticData.RefreshToken, out string refreshToken);
            TokenDTO tokenDto = new TokenDTO()
            {
                AccessToken = accessToken,
                RefreshAccessToken = refreshToken
            };

            return hasAccessToken || hasRefreshToken ? tokenDto : null;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public void ClearToken()
    {
        _contextAccessor.HttpContext?.Response.Cookies.Delete(StaticData.AccessToken);
        _contextAccessor.HttpContext?.Response.Cookies.Delete(StaticData.RefreshToken);
    }
}