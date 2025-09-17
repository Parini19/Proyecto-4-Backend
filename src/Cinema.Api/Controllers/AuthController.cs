using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("me")]
public class AuthController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult Me()
    {
        // Claims típicos de Firebase: user_id/uid, email, etc.
        var uid = User.FindFirst("user_id")?.Value ?? User.FindFirst("uid")?.Value;
        var email = User.FindFirst("email")?.Value;
        return Ok(new { uid, email, claims = User.Claims.Select(c => new { c.Type, c.Value }) });
    }
}
