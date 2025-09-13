using AuthAPI.Models;

namespace AuthAPI.TokenService;

public interface ITokenGenerator
{
    string GenerateToken(AppUser user);
    string GenerateRefreshToken();
    string GetUserEmail(string token);
}
