using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xabarchi.Application.Abstractions;
using Xabarchi.Domain.DTOs;
using Xabarchi.Domain.ResponseDTOs;
using Xabarchi.Domain.SimpleDTOs;

namespace Xabarchi.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<UserResponseDto>> GetMyProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _userService.GetMyProfile(userId);
        if (result == null)
            return BadRequest(new { Message = "User not found" });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<UserResponseDto>> GetUserById([FromQuery] Guid userId)
    {
        var result = await _userService.GetUserById(userId);
        if (result == null)
            return BadRequest(new { Message = "User not found" });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<UserResponseDto>> GetUserByUsername([FromQuery] string username)
    {
        var result = await _userService.GetUserByUsername(username);
        if (result == null)
            return BadRequest(new { Message = "User not found" });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<UserSimpleDto>>> SearchUsers([FromQuery] string query)
    {
        var result = await _userService.SearchUsers(query);
        if (result == null)
            return BadRequest(new { Message = "No users found" });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<bool>> CheckUsernameAvailable([FromQuery] string username)
    {
        var result = await _userService.CheckUsernameAvailable(username);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<string>> UpdateProfile([FromForm] UserDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _userService.UpdateProfile(userId, dto);
        if (result == null)
            return BadRequest(new { Message = "Failed to update profile" });
        return Ok(new { Message = result });
    }

    [HttpPut]
    public async Task<ActionResult<bool>> ChangeUsername([FromQuery] string newUsername)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _userService.ChangeUsername(userId, newUsername);
        if (!result)
            return BadRequest(new { Message = "Username already taken" });
        return Ok(new { Message = "Username changed successfully" });
    }

    [HttpPut]
    public async Task<ActionResult<bool>> UpdateEmail([FromQuery] string email)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _userService.UpdateEmail(userId, email);
        if (!result)
            return BadRequest(new { Message = "Email already taken" });
        return Ok(new { Message = "Email updated successfully" });
    }

    [HttpPut]
    public async Task<ActionResult<bool>> UpdatePhone([FromQuery] string phone)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _userService.UpdatePhone(userId, phone);
        if (!result)
            return BadRequest(new { Message = "Phone already taken" });
        return Ok(new { Message = "Phone updated successfully" });
    }

    [HttpDelete]
    public async Task<ActionResult<string>> DeleteAvatar()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _userService.DeleteAvatar(userId);
        if (result == null)
            return BadRequest(new { Message = "Failed to delete avatar" });
        return Ok(new { Message = result });
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetUserAvatar()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var avatarUrl = await _userService.GetUserAvatar(userId);
    
        if (string.IsNullOrEmpty(avatarUrl))
            return NotFound(new { Message = "Avatar not found" });
    
        return Ok(new { AvatarUrl = avatarUrl });
    }
}