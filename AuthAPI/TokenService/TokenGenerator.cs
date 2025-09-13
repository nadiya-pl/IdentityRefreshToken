using AuthAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthAPI.TokenService;

public class TokenGenerator : ITokenGenerator
{
    private readonly string secretKey = "";
    private readonly int tokenLifetimeMinutes = 0;

    public TokenGenerator(IConfiguration configuration)
    {
        secretKey = configuration.GetValue<string>("JWT:SecretKey")!;
        tokenLifetimeMinutes = configuration.GetValue<int>("JWT:AccessTokenLifetimeMinutes", 20);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using (var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

    public string GenerateToken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(tokenLifetimeMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string? GetUserEmail(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            return principal.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value;
        }
        catch 
        {
            return null;
        }
    }
}
