using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xabarchi.Application.Abstractions;
using Xabarchi.Domain.DTOs;
using Xabarchi.Domain.ResponseDTOs;

namespace Xabarchi.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<ActionResult<string>> CreateChat([FromBody] ChatDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _chatService.CreateChat(userId, dto);
        if (result == null)
            return BadRequest(new { Message = "Failed to create chat" });
        return Ok(new { Message = result });
    }

    [HttpGet]
    public async Task<ActionResult<ChatResponseDto>> GetChatById([FromQuery] Guid chatId)
    {
        var result = await _chatService.GetChatById(chatId);
        if (result == null)
            return BadRequest(new { Message = "Chat not found" });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ChatResponseDto>>> GetUserChats()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _chatService.GetUserChats(userId);
        if (result == null)
            return BadRequest(new { Message = "No chats found" });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<ChatResponseDto>> GetOrCreateChat([FromQuery] Guid targetUserId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _chatService.GetOrCreateChat(userId, targetUserId);
        if (result == null)
            return BadRequest(new { Message = "Failed to get or create chat" });
        return Ok(result);
    }

    [HttpDelete]
    public async Task<ActionResult<string>> DeleteChat([FromQuery] Guid chatId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _chatService.DeleteChat(chatId, userId);
        if (result == null)
            return BadRequest(new { Message = "Failed to delete chat" });
        return Ok(new { Message = result });
    }
}