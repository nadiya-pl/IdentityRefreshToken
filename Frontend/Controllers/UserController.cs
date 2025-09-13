using Frontend.HttpClients;
using Frontend.Models.Dto;
using Frontend.TokenManager;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Frontend.Controllers;

public class UserController : Controller
{
    private readonly AuthHttpClient _client;
    private readonly ITokenManager _tokenProvider;
    private readonly ILogger<UserController> _logger;

    public UserController(AuthHttpClient client, ITokenManager tokenProvider, ILogger<UserController> logger)
    {
        _client = client;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    public IActionResult Login()
    {
        return View(new LoginDto());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Invalid form data.");
            return View(dto);
        }

        _logger.LogInformation("[UserController] Login attempt for {Email}", dto.Email);

        var response = await _client.Login(dto);


        // validation errors from API
        if (!response.Success && response.Errors != null)
        {
            _logger.LogWarning("[UserController] Login validation failed for {Email}. Errors: {Count}",
               dto.Email, response.Errors.Count);

            foreach (var error in response.Errors)
                ModelState.AddModelError(error.Key, error.Value);

            return View(dto);
        }
        // non-validation failure from API
        else if (response.Success == false && response.Errors == null)
        {
            _logger.LogWarning("[UserController] Login failed for {Email}. Message: {Message}",
                dto.Email, response.Message);
            TempData["ErrorMessage"] = response.Message ?? "Login failed.";
            return RedirectToAction("Error");
        }
        // нет токенов
        else if (string.IsNullOrEmpty(response.Token) || string.IsNullOrEmpty(response.RefreshToken))
        {
            _logger.LogError("[UserController] Login succeeded but tokens are missing for {Email}", dto.Email);
            ModelState.AddModelError(string.Empty, "Unexpected server response. Please try again.");
            return View(dto);
        }


        if(response.Token.Length > 5 && response.RefreshToken.Length > 5 && response.Success == true)
        {
            await SignInAsync(response.Token);
            _tokenProvider.SetAccessToken(response.Token);
            _tokenProvider.SetRefreshToken(response.RefreshToken);

            _logger.LogInformation("[UserController] Login successful for {Email}", dto.Email);

            return RedirectToAction("Index", "Home");
        }

        return View(dto);
    }


    public IActionResult Register()
    {
        return View(new RegisterDto());
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Invalid form data.");
            return View(dto);
        }

        _logger.LogInformation("[UserController] Registration attempt for {Email}", dto.Email);

        var response = await _client.Register(dto);


        // validation errors from API
        if (!response.Success && response.Errors != null)
        {
            foreach (var error in response.Errors)
                ModelState.AddModelError(error.Key, error.Value);

            _logger.LogWarning("[UserController] Registration validation failed for {Email}. Errors: {Count}",
            dto.Email, response.Errors.Count);
            return View(dto);
        }        
        // non-validation failure from API
        else if (response.Success == false && response.Errors == null)
        {
            _logger.LogWarning("[UserController] Registration failed for {Email}. Message: {Message}",
            dto.Email, response.Message);
            TempData["ErrorMessage"] = response.Message ?? "Registration failed.";
            return RedirectToAction("Error");
        }
        // missing tokens
        else if (string.IsNullOrEmpty(response.Token) || string.IsNullOrEmpty(response.RefreshToken))
        {
            _logger.LogError("[UserController] Registration succeeded but tokens are missing for {Email}", dto.Email);
            ModelState.AddModelError(string.Empty, "Unexpected server response. Please try again.");
            return View(dto);
        }


        if (response.Token.Length > 5 && response.RefreshToken.Length > 5 && response.Success == true)
        {
            await SignInAsync(response.Token);
            _tokenProvider.SetAccessToken(response.Token);
            _tokenProvider.SetRefreshToken(response.RefreshToken);

            _logger.LogInformation("[UserController] Registration successful for {Email}", dto.Email);

            return RedirectToAction("Index", "Home");
        }

        return View(dto);
    }

 
    public async Task<IActionResult> RefreshTokens()
    {
        var tokensDto = new TokensDto();
        tokensDto.Token = _tokenProvider.GetAccessToken();
        tokensDto.RefreshToken = _tokenProvider.GetRefreshToken();

        if (string.IsNullOrEmpty(tokensDto.Token) || string.IsNullOrEmpty(tokensDto.RefreshToken))
        {
            _logger.LogWarning("[UserController] RefreshTokens called but tokens are missing");
            return RedirectToAction("Login");
        }

        var response = await _client.RefreshTokens(tokensDto);


        if (response.Success == true)
        {
            await SignInAsync(response.Token);
            _tokenProvider.SetAccessToken(response.Token);
            _tokenProvider.SetRefreshToken(response.RefreshToken);
            return RedirectToAction("Protected", "Home");
        }
        else
        {
            _logger.LogWarning("[UserController] Refresh failed. Message: {Message}", response.Message);
        }

        return RedirectToAction("Login");
    }  

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        _tokenProvider.ClearTokens();
        return RedirectToAction("Index", "Home");
    }
    
    public IActionResult AccessDenied()
    {
        return View();
    }

    public IActionResult Error()
    {
        var error = TempData["ErrorMessage"] as string ?? "Unknown error occurred.";
        return View("Error");
    }


    public async Task SignInAsync(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

        GetClaims(jwt, identity);

        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    void GetClaims(JwtSecurityToken jwt, ClaimsIdentity identity)
    {
        try
        {
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

            var userName = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            if (!string.IsNullOrEmpty(userId))
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));

            if (!string.IsNullOrEmpty(userName))
                identity.AddClaim(new Claim(ClaimTypes.Name, userName));

            if (!string.IsNullOrEmpty(email))
                identity.AddClaim(new Claim(ClaimTypes.Email, email));
        }
        catch { }
    }


}
