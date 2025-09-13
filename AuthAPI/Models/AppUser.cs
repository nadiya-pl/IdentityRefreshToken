using Microsoft.AspNetCore.Identity;

namespace AuthAPI.Models;

public class AppUser: IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
