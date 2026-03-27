using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using hotel_room_api;
using Hotel_Rooms_MVC;
using Hotel_Rooms_MVC.Models;
using Hotel_Rooms_MVC.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using RoomsUtility;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace Hotel_Rooms_MVC.Services.Services;

public class BaseService : IBaseService
{
    public APIResponse responseModel { get; set; }
    public IHttpClientFactory httpClient { get; set; }
    public ITokenProvider _TokenProvider { get; set; }
    private readonly string roomsApi;
    protected IHttpContextAccessor _httpContextAccessor;

    public BaseService(IHttpClientFactory httpClient, ITokenProvider TokenProvider, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _TokenProvider = TokenProvider;
        this.responseModel = new APIResponse();
        this.roomsApi = configuration.GetValue<string>("ServiceUrls:RoomApi");
        this.httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<T> SendAsync<T>(ApiRequest apiRequest, bool needBearer = true)
    {
        try
        {
            var client = httpClient.CreateClient("RoomApi");
            
            
            HttpResponseMessage httpResponseMessage = null;
            var messageFactory = () =>
            {
                HttpRequestMessage message = new HttpRequestMessage();

                if (apiRequest.ContentType == StaticData.ContentType.MultipartFormData)
                    message.Headers.Add("Accept", "*/*");
                else
                    message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url);
            
                if (apiRequest.ContentType == StaticData.ContentType.MultipartFormData)
                {
                    var content = new MultipartFormDataContent();

                    foreach (var prop in apiRequest.data.GetType().GetProperties())
                {
                    var value = prop.GetValue(apiRequest.data);
                    if (value is FormFile)
                    {
                        var file = (FormFile)value;
                        content.Add(new StreamContent(file?.OpenReadStream()), prop.Name, file.FileName);
                    }
                    else
                        content.Add(new StringContent(value == null ? "" : value?.ToString()), prop.Name);
                }

                    message.Content = content;

                }
                else
                {
                    if (apiRequest.data != null)
                    {
                        message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.data),
                            Encoding.UTF8, "application/json");
                    }
                }
    
                switch (apiRequest.ApiType)
                {
                    case StaticData.ApiTypes.GET:
                        message.Method = HttpMethod.Get;
                        break;
                    case StaticData.ApiTypes.POST:
                    message.Method = HttpMethod.Post;
                    break;
                    case StaticData.ApiTypes.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                    case StaticData.ApiTypes.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                }
                return message;
            };
            
            httpResponseMessage = await SendAsyncWithRefreshToken(client, messageFactory, needBearer);

            APIResponse finalApiResponse = new()
            {
                IsSuccess = false
            };
            
            
            try
            {
                switch (httpResponseMessage.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        finalApiResponse.ErrorMessages = new List<string>() { "404 Not Found"};
                    break;
                    case HttpStatusCode.Unauthorized:
                        finalApiResponse.ErrorMessages = new List<string>() { "Unauthorized"};
                        break;
                    case HttpStatusCode.Forbidden:
                        finalApiResponse.ErrorMessages = new List<string>() { "Forbidden"};
                        break;
                    case HttpStatusCode.InternalServerError:
                        finalApiResponse.ErrorMessages = new List<string>() { "Internal Server Error"};
                        break;
                    default:
                        var apiContent = await httpResponseMessage.Content.ReadAsStringAsync();
                        finalApiResponse.IsSuccess = true;
                        finalApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                        break;
                }
                
            }
            catch (AuthException)
            {
                throw new AuthException();
            }
            catch (Exception e)
            {
                finalApiResponse.ErrorMessages = new List<string>() { "Internal Server Error", e.Message};
            }
            
            
            var res = JsonConvert.SerializeObject(finalApiResponse);
            var returnObj = JsonConvert.DeserializeObject<T>(res);
            return returnObj;
        }
        catch (AuthException)
        {
            throw new AuthException();
        }
        catch (Exception e)
        {
            var dto = new APIResponse()
            {
                ErrorMessages = new List<string>() { Convert.ToString(e.Message) },
                IsSuccess = false
            };

            var res = JsonConvert.SerializeObject(dto);
            var apiResponse = JsonConvert.DeserializeObject<T>(res);
            return apiResponse;
        }
        
    }

    private async Task<HttpResponseMessage> SendAsyncWithRefreshToken(HttpClient httpClient, 
    Func<HttpRequestMessage> httpRequestMessageFactory, bool needBearer = true)
{
    if (!needBearer)
    {
        return await httpClient.SendAsync(httpRequestMessageFactory());
    }
    else
    {
        TokenDTO tokenDto = _TokenProvider.GetToken();
        
        // Check if token exists
        if (string.IsNullOrEmpty(tokenDto?.AccessToken))
        {
            // No token, user must login
            await _httpContextAccessor.HttpContext.SignOutAsync();
            throw new AuthException();
            throw new UnauthorizedAccessException("No authentication token found. Please login.");
        }

        // Check if token is expired
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(tokenDto.AccessToken);
        bool isTokenExpired = jwt.ValidTo < DateTime.UtcNow;

        // If token is expired, try refresh token before making the call
        if (isTokenExpired)
        {
            if (string.IsNullOrEmpty(tokenDto.RefreshAccessToken))
            {
                // No refresh token, user must login
                await _httpContextAccessor.HttpContext.SignOutAsync();
                _TokenProvider.ClearToken();
                throw new AuthException();
                throw new UnauthorizedAccessException("Authentication expired. Please login again.");
            }

            // Try refresh token
            bool refreshSuccessful = await invokeRefreshTokenEndPoint(httpClient, tokenDto.AccessToken, tokenDto.RefreshAccessToken);
            if (!refreshSuccessful)
            {
                // Refresh failed, user must login
                await _httpContextAccessor.HttpContext.SignOutAsync();
                _TokenProvider.ClearToken();
                throw new AuthException();
                throw new UnauthorizedAccessException("Session expired. Please login again.");
            }
            
            // Get new token after refresh
            tokenDto = _TokenProvider.GetToken();
        }

        // Set authorization header with current token
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);

        try
        {
            // Make the API call
            var response = await httpClient.SendAsync(httpRequestMessageFactory());
            
            // If still unauthorized after using a valid token, try refresh once
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool refreshSuccessful = await invokeRefreshTokenEndPoint(httpClient, tokenDto.AccessToken, tokenDto.RefreshAccessToken);
                if (!refreshSuccessful)
                {
                    // Refresh failed, user must login
                    return response; // Return the 401 response
                }
                
                // Retry with new token
                tokenDto = _TokenProvider.GetToken();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);
                return await httpClient.SendAsync(httpRequestMessageFactory());
            }
            
            return response;
        }
        catch (AuthException)
        {
            throw new AuthException();
        }
        catch (HttpRequestException httpRequestException)
        {
           
            throw;
        }
    }
}


private async Task<bool> invokeRefreshTokenEndPoint(HttpClient httpClient, string existingAccessToken,
    string existingRefreshToken)
{
    try
    {
        HttpRequestMessage message = new();
        message.Headers.Add("Accept", "application/json");
        message.RequestUri = new Uri($"{roomsApi}/api/UserAuth/refresh");
        message.Method = HttpMethod.Post;
        message.Content = new StringContent(JsonConvert.SerializeObject(new TokenDTO()
        {
            AccessToken = existingAccessToken,
            RefreshAccessToken = existingRefreshToken
        }), Encoding.UTF8, "application/json");

        var response = await httpClient.SendAsync(message);
        var content = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<APIResponse>(content);

        if (apiResponse?.IsSuccess is not true)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync();
            _TokenProvider.ClearToken();
            throw new AuthException();
            return false;
        }
        else
        {
            var tokenData = JsonConvert.SerializeObject(apiResponse.Result);
            var tokenDto = JsonConvert.DeserializeObject<TokenDTO>(tokenData);

            if (tokenDto?.AccessToken is not null)
            {
                await SigninWithNewToken(tokenDto);
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);
                return true;
            }

            return false;
        }
    }
    catch (AuthException)
    {
        throw new AuthException();
    }
    catch
    {
        return false;
    }
}

    private async Task SigninWithNewToken(TokenDTO tokenDto)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(tokenDto.AccessToken);
            
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            
        identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type =="unique_name").Value));
        identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type =="role" ).Value));

        var principal = new ClaimsPrincipal(identity);
        await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            
        
        _TokenProvider.SetToken(tokenDto);
    }
    
    
    // create token with same EndDate into server
    // check if token is still valid .. pass it
    // if not valid .. pass both(token & refresh Token)
    // if refresh token is not valid .. user must re Login
    
}
