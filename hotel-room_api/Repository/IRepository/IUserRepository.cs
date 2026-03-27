using hotel_room_api.Models;
using hotel_room_api.Models.DTOs;
using hotel_room_api.Models.DTOs.InternalDTO;

namespace hotel_room_api.Repository.IRepository;

public interface IUserRepository
{
    Task<bool> IsUniqueUserName(string userName);
    Task<TokenDTO> Login(LoginRequestDTO loginRequestDto);
    Task<IdentityUserDto> Register(RegisterRequestDTO registerRequestDto);
    
    Task<TokenDTO> RefreshAccessToken(TokenDTO TokenDto);
    Task RevokeRefreshToken(TokenDTO TokenDto);
    Task<InternalTokenDTO> LoginHardWay(LoginRequestDTO loginRequestDto);
    Task<InternalUser> RegisterHardWay(RegisterRequestDTO registerRequestDto);


}