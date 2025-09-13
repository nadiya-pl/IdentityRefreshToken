using AuthAPI.Models;
using AuthAPI.Models.Dto;

namespace AuthAPI.Services;

public interface IAuthService
{
    Task<ApiResponse> LoginAsync(LoginDto dto);
    Task<ApiResponse> RegisterAsync(RegisterDto dto);
    Task<ApiResponse> RefreshTokensAsync(TokensDto dto);
}
