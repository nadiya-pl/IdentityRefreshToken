using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtectedAPI.Models;

namespace ProtectedAPI.Controllers;

[ApiController]
[Route("api/protected")]
[Authorize]
public class SecureDataController : ControllerBase
{
    [HttpGet]
    public IActionResult GetProtectedData()
    {
        var response = new ProtectedResponse();
        response.Success = true;
        response.Data = $"Hello, {User.Identity?.Name}. This is secret string";
        return Ok(response);
    }
}
