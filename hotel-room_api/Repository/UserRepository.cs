using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Identity;
using hotel_room_api.Data;
using hotel_room_api.Models;
using hotel_room_api.Models.DTOs;
using hotel_room_api.Models.DTOs.InternalDTO;
using hotel_room_api.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace hotel_room_api.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    private  string secretKey;

    public UserRepository(AppDbContext db, IConfiguration configuration,
        RoleManager<IdentityRole> roleManager,UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        secretKey = configuration.GetValue<string>("ApiSettings:Secret");
    }


    public async Task<bool> IsUniqueUserName(string userName)
    {
        //var user = await _db.InternalUsers.FirstOrDefaultAsync(u => u.UserName == userName);
        var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.UserName == userName);
        
        return (user == null);
    }

    public async Task<IdentityUserDto> Register(RegisterRequestDTO registerRequestDto)
    {
        AppUser user = new AppUser()
        {
            UserName = registerRequestDto.UserName,
            Email=registerRequestDto.UserName,
            NormalizedEmail=registerRequestDto.UserName.ToUpper(),
            Name = registerRequestDto.Name,
        };

        try
        {
            var result = await _userManager.CreateAsync(user, registerRequestDto.Password);
            if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole("admin"));
                await _roleManager.CreateAsync(new IdentityRole("test"));
            }
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "admin");
                var userToReturn =
                    await _db.AppUsers.FirstOrDefaultAsync(u => u.UserName == registerRequestDto.UserName);
                return new IdentityUserDto()
                {
                    ID = userToReturn.Id,
                    Name = userToReturn.Name,
                    userName = userToReturn.UserName
                };
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return  null;
    }
    
    public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDto)
    {
        
        // User Identity 
        var userIdentity = _db.AppUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

        bool isValidPwd = await _userManager.CheckPasswordAsync(userIdentity, loginRequestDto.Password);
        
        if (userIdentity == null || isValidPwd == false)
        {
            return new TokenDTO()
            {
                AccessToken =""
            };
        }

        var jwtTokenId = $"JWT_ID_{Guid.NewGuid()}";
        string tokenString = await GenerateAccessToken(userIdentity, jwtTokenId);
        string refreshToken = await CreateNewRefreshToken(userIdentity.Id, jwtTokenId);
        
        TokenDTO TokenDto = new TokenDTO()
        {
            AccessToken = tokenString,
            RefreshAccessToken = refreshToken
        };

        return TokenDto;
    }

    private async Task<string> GenerateAccessToken(AppUser userIdentity, string jwtID)
    {
        var roles = await _userManager.GetRolesAsync(userIdentity);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var TokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, userIdentity.UserName),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                new Claim(JwtRegisteredClaimNames.Jti , jwtID),
                new Claim(JwtRegisteredClaimNames.Sub , userIdentity.Id),
            }),
            Expires = DateTime.UtcNow.AddMinutes(100),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(TokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }

    public async Task<TokenDTO> RefreshAccessToken(TokenDTO TokenDto)
    {
        //Find an existing refresh Token
        var existingRefreshToken =
            await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Refresh_Token == TokenDto.RefreshAccessToken);
        if (existingRefreshToken is null)
            return new TokenDTO();

        // compare data from existing refresh and access token provided "Mismatch = fraud"
        var accessTokenData = GetAccessTokenData(TokenDto.AccessToken);
        if (!accessTokenData.isSuccess || accessTokenData.userId != existingRefreshToken.UserId
            || accessTokenData.tokenId != existingRefreshToken.JwtTokenId)
        {
            await markTokenAsInvalid(existingRefreshToken);
            return new TokenDTO();
        }

        // when someone try to use unvalid refreshToken
        if (!existingRefreshToken.IsValid)
        {
            await markAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            return new TokenDTO();
        }
        
        // if refresh is expired return empty, so must relogin
        if (existingRefreshToken.ExpiredAt < DateTime.UtcNow)
        {
            await markTokenAsInvalid(existingRefreshToken);
            return new TokenDTO();
        }
        
        // replace old refresh with a new one with update expiredDate
        var NewRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);

        // revoke - cancel - existing refresh token
        existingRefreshToken.IsValid = false;
        _db.SaveChanges();
        
        // generate new access token
        var appUser = _db.AppUsers.FirstOrDefault(u => u.Id == existingRefreshToken.UserId);
        if (appUser is null)
            return new TokenDTO();

        var newAccessToken = await GenerateAccessToken(appUser, existingRefreshToken.JwtTokenId);

        return new TokenDTO()
        {
            AccessToken = newAccessToken,
            RefreshAccessToken = NewRefreshToken
        };
    }

    private async Task<string> CreateNewRefreshToken(string userId, string tokenId)
    {
        await DevalidateRefreshTokens(userId);
        RefreshToken refreshToken = new RefreshToken()
        {
            UserId = userId,
            IsValid = true,
            JwtTokenId = tokenId,
            ExpiredAt = DateTime.UtcNow.AddMinutes(100),
            Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid()

        };

        await _db.RefreshTokens.AddAsync(refreshToken);
        _db.SaveChanges();

        return refreshToken.Refresh_Token;

    }
    private (bool isSuccess, string userId, string tokenId) GetAccessTokenData(string accessToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(accessToken);
            var jwtID = jwt.Claims.FirstOrDefault(j => j.Type == JwtRegisteredClaimNames.Jti).Value;
            var userId = jwt.Claims.FirstOrDefault(j => j.Type == JwtRegisteredClaimNames.Sub).Value;
            return (true, userId, jwtID);
        }
        catch
        {
            return (false, null, null);
        }
    }

    private async Task DevalidateRefreshTokens(string username)
    {
        
        var records = _db.RefreshTokens.Where(r => r.UserId == username);
        foreach (var record in records)
            record.IsValid = false;
        
        _db.UpdateRange(records);
        await _db.SaveChangesAsync();
        
    }
    
    public async Task RevokeRefreshToken(TokenDTO TokenDto)
    {
        var record = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Refresh_Token == TokenDto.RefreshAccessToken);
        if (record is null) return ;

        var isTokenValid = GetAccessTokenData(TokenDto.AccessToken);

        if (!isTokenValid.isSuccess) return;

        await markAllTokenInChainAsInvalid(record.UserId, record.JwtTokenId);


    }
    
    public async Task<InternalUser> RegisterHardWay(RegisterRequestDTO registerRequestDto)
    {
        InternalUser user = new InternalUser()
        {
            UserName = registerRequestDto.UserName,
            Password = registerRequestDto.Password,
            Name = registerRequestDto.Name,
            Role = registerRequestDto.Role
        };
        _db.InternalUsers.Add(user);
        await _db.SaveChangesAsync();

        user.Password = "";
        return user;
    }
    public async Task<InternalTokenDTO> LoginHardWay(LoginRequestDTO loginRequestDto)
    {
        // Hard way
        var user =await _db.InternalUsers.FirstOrDefaultAsync(
            u => u.UserName.ToLower() == loginRequestDto.UserName
                 && u.Password == loginRequestDto.Password);
        
        if (user == null)
        {
            return new InternalTokenDTO()
            {
                AccessToken =""
            };
        }
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var TokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(TokenDescriptor);
        InternalTokenDTO internalTokenDto = new InternalTokenDTO()
        {
            AccessToken = tokenHandler.WriteToken(token)
        };

        return internalTokenDto;
    }

    private async Task markAllTokenInChainAsInvalid(string userId, string tokenId)
    {
         await _db.RefreshTokens.Where(u => u.UserId == userId 
                                                              && u.JwtTokenId == tokenId)
            .ExecuteUpdateAsync(u=>u.SetProperty(Refreshtoken => Refreshtoken.IsValid, false));

        // foreach (var record in chainRecords)
        //     record.IsValid = false;
        // _db.UpdateRange(chainRecords);
        // _db.SaveChanges();
    }

    private Task markTokenAsInvalid(RefreshToken refreshToken)
    {
        refreshToken.IsValid = false;
       return _db.SaveChangesAsync();
    }
}