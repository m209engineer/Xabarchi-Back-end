using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xabarchi.Application.Abstractions;
using Xabarchi.Domain.AuthDto;

namespace Xabarchi.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    public async Task<ActionResult<TokenResponse>> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.Register(dto);
        if (!result.Success)
            return BadRequest(new { Message = result.Message });
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.Login(dto);
        if (!result.Success)
            return BadRequest(new { Message = result.Message });
        return Ok(result);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<string>> ChangePassword([FromQuery] string oldPassword, [FromQuery] string newPassword)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _authService.ChangePassword(userId, oldPassword, newPassword);
        if (!result)
            return BadRequest(new { Message = "Failed to change password" });
        return Ok(new { Message = "Password changed successfully" });
    }
    
    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        await _authService.Logout(Guid.Parse(userId));
        return Ok();
    }
}