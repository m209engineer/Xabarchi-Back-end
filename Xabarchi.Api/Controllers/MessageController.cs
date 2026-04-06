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
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpPost]
    public async Task<ActionResult<string>> SendMessage([FromForm] MessageDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _messageService.SendMessage(userId, dto);
        if (result == null)
            return BadRequest(new { Message = "Failed to send message" });
        return Ok(new { Message = result });
    }

    [HttpPut]
    public async Task<ActionResult<string>> EditMessage([FromQuery] Guid messageId, [FromBody] MessageDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _messageService.EditMessage(messageId, userId, dto);
        if (result == null)
            return BadRequest(new { Message = "Failed to edit message" });
        return Ok(new { Message = result });
    }

    [HttpPost]
    public async Task<ActionResult<string>> ReplyToMessage([FromQuery] Guid replyToMessageId, [FromForm] MessageDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _messageService.ReplyToMessage(userId, replyToMessageId, dto);
        if (result == null)
            return BadRequest(new { Message = "Failed to reply to message" });
        return Ok(new { Message = result });
    }

    [HttpPut]
    public async Task<ActionResult<string>> MarkAsRead([FromQuery] Guid messageId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _messageService.MarkAsRead(messageId, userId);
        if (result == null)
            return BadRequest(new { Message = "Failed to mark as read" });
        return Ok(new { Message = result });
    }

    [HttpDelete]
    public async Task<ActionResult<string>> DeleteMessage([FromQuery] Guid messageId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _messageService.DeleteMessage(messageId, userId);
        if (result == null)
            return BadRequest(new { Message = "Failed to delete message" });
        return Ok(new { Message = result });
    }

    [HttpPost]
    public async Task<ActionResult<string>> AddMedia([FromQuery] Guid messageId, [FromForm] AddMediaDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _messageService.AddMedia(messageId, userId, dto);
        if (result == null)
            return BadRequest(new { Message = "Failed to add media" });
        return Ok(new { Message = result });
    }

    [HttpDelete]
    public async Task<ActionResult<string>> DeleteMedia([FromQuery] Guid messageId, [FromQuery] Guid mediaId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _messageService.DeleteMedia(messageId, userId, mediaId);
        if (result == null)
            return BadRequest(new { Message = "Failed to delete media" });
        return Ok(new { Message = result });
    }

    [HttpGet]
    public async Task<ActionResult<MessageResponseDto>> GetMessageById([FromQuery] Guid messageId)
    {
        var result = await _messageService.GetMessageById(messageId);
        if (result == null)
            return BadRequest(new { Message = "Message not found" });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<MessageResponseDto>>> GetChatMessages([FromQuery] Guid chatId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _messageService.GetChatMessages(chatId, page, pageSize);
        if (result == null)
            return BadRequest(new { Message = "No messages found" });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<MessageResponseDto>>> GetMessageReplies([FromQuery] Guid messageId)
    {
        var result = await _messageService.GetMessageReplies(messageId);
        if (result == null)
            return BadRequest(new { Message = "No replies found" });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<MessageResponseDto>>> GetUnreadMessages([FromQuery] Guid chatId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _messageService.GetUnreadMessages(userId, chatId);
        if (result == null)
            return BadRequest(new { Message = "No unread messages found" });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<int>> GetUnreadCount([FromQuery] Guid chatId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _messageService.GetUnreadCount(userId, chatId);
        return Ok(result);
    }
}