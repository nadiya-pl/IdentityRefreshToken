using AuthAPI.Models;
using AuthAPI.Models.Dto;
using AuthAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AuthAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IAuthService _authService;

    public UserController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        ApiResponse result = new ApiResponse();

        if(!ModelState.IsValid)
        {
            result.Success = false;
            result.Message = "Ошибка валидации.";
            result.Errors = GetValidationsErrors(ModelState);
            return BadRequest(result);
        }

        result = await _authService.RegisterAsync(dto);

        if (!result.Success)
            return BadRequest(result);
        else
            return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        ApiResponse result = new ApiResponse();

        if (!ModelState.IsValid)
        {
            result.Success = false;
            result.Message = "Ошибка валидации.";
            result.Errors = GetValidationsErrors(ModelState);
            return BadRequest(result);
        }

        result = await _authService.LoginAsync(dto);

        if(!result.Success)
            return BadRequest(result);
        else 
            return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokensDto dto)
    {
        ApiResponse result = new ApiResponse();

        if(!ModelState.IsValid)
        {
            result.Success = false;
            result.Message = "Ошибка валидации токенов.";
            result.Errors = GetValidationsErrors(ModelState);
            return BadRequest(result);
        }

        result = await _authService.RefreshTokensAsync(dto);

        if (!result.Success)
            return BadRequest(result);
        else
            return Ok(result);
    }

    private Dictionary<string, string> GetValidationsErrors(ModelStateDictionary modelState)
    {
        return modelState
        .Where(kvp => kvp.Value.Errors.Any())
        .ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Errors.First().ErrorMessage
        );
    }
}
