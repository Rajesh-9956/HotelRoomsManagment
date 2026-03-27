using System.Net;
using System.Reflection.Metadata;
using hotel_room_api.Models;
using hotel_room_api.Models.DTOs;
using hotel_room_api.Models.DTOs.InternalDTO;
using hotel_room_api.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace hotel_room_api.Controllers;

[Route("api/UserAuth")]
[ApiController]
public class UsersController : Controller
{
    private readonly IUserRepository _userRepository;
    protected APIResponse _apiResponse;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _apiResponse = new APIResponse();
    }

    [HttpPost("login")]
    public async Task<ActionResult<APIResponse>> Login([FromBody] LoginRequestDTO user)
    {
        var TokenDTO = await _userRepository.Login(user);
        if (TokenDTO== null || string.IsNullOrEmpty(TokenDTO.AccessToken) )
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.ErrorMessages = new List<string>() { "Username or Password is not Valid" };
            
            return BadRequest(_apiResponse);
        }
        
        _apiResponse.IsSuccess = true;
        _apiResponse.StatusCode = HttpStatusCode.OK;
        _apiResponse.Result = TokenDTO;
        return Ok(_apiResponse);
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<APIResponse>> Register([FromBody] RegisterRequestDTO user)
    {
        try
        {
            bool isUserNameUnique = await _userRepository.IsUniqueUserName(user.UserName);

            if (!isUserNameUnique)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = new List<string>() { "Username Already Exist" };

                return BadRequest(_apiResponse);

            }

            var registerdUser = await _userRepository.Register(user);
            if (registerdUser == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = new List<string>() { "Registration Failed" };

                return BadRequest(_apiResponse);
            }

            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = registerdUser;
            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
            _apiResponse.ErrorMessages = new List<string>() { e.Message };
            return BadRequest(_apiResponse);
        }
    }
    
    [HttpPost("refresh")]
    public async Task<ActionResult<APIResponse>> GetNewTokenByAccessToken([FromBody] TokenDTO tokenDto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var tokenDTOResponse = await _userRepository.RefreshAccessToken(tokenDto);
                
                if (string.IsNullOrEmpty(tokenDTOResponse?.AccessToken))
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.ErrorMessages = new List<string>() { "Token Invalid" };

                    return BadRequest(_apiResponse);
                }
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = tokenDTOResponse;
                return Ok(_apiResponse);
            }
            _apiResponse.IsSuccess = false;
            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.ErrorMessages = new List<string>() { "Invalid Input " };

            return BadRequest(_apiResponse);
            
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
            _apiResponse.ErrorMessages = new List<string>() { e.Message };
            return BadRequest(_apiResponse);
        }
    }

    [HttpPost("Revoke")]
    public async Task<ActionResult<APIResponse>> RevokeRefreshToken([FromBody] TokenDTO tokenDto)
    {
            if (ModelState.IsValid)
            {
                await _userRepository.RevokeRefreshToken(tokenDto);
                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);

            }
            _apiResponse.IsSuccess = false;
            _apiResponse.StatusCode = HttpStatusCode.BadRequest;
            _apiResponse.Result= "Invalid input";

            return BadRequest(_apiResponse);
    }

    
    [Route("ping")]
    [HttpGet]
    public IActionResult TestErrorHandler()
    {
        throw new FileNotFoundException();
    }
}









