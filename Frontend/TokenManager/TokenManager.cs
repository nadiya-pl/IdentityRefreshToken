namespace Frontend.TokenManager;

public class TokenManager : ITokenManager
{
    private readonly IHttpContextAccessor _contextAccessor;
    private const string AccessTokenKey = "AccessToken";
    private const string RefreshTokenKey = "RefreshToken";

    public TokenManager(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public void ClearTokens()
    {
        _contextAccessor.HttpContext?.Response.Cookies.Delete(AccessTokenKey);
        _contextAccessor.HttpContext?.Response.Cookies.Delete(RefreshTokenKey);
    }

    public string? GetAccessToken()
    {
        return _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(AccessTokenKey, out var token) == true ? token : null;
    }

    public string? GetRefreshToken()
    {
        return _contextAccessor.HttpContext?.Request.Cookies.TryGetValue(RefreshTokenKey, out var token) == true ? token : null;
    }

    public void SetAccessToken(string token)
    {
        _contextAccessor.HttpContext?.Response.Cookies.Append(AccessTokenKey, token);
    }

    public void SetRefreshToken(string token)
    {
        _contextAccessor.HttpContext?.Response.Cookies.Append(RefreshTokenKey, token);
    }
}
