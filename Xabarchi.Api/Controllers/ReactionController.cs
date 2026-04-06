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
public class ReactionController : ControllerBase
{
    private readonly IReactionService _reactionService;

    public ReactionController(IReactionService reactionService)
    {
        _reactionService = reactionService;
    }

    [HttpPost]
    public async Task<ActionResult<string>> AddReaction([FromBody] ReactionDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _reactionService.AddReaction(userId, dto);
        if (result == null)
            return BadRequest(new { Message = "Failed to add reaction" });
        return Ok(new { ReactionId = result }); // ← Message → ReactionId
    }

    [HttpDelete]
    public async Task<ActionResult<string>> RemoveReaction([FromQuery] Guid reactionId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _reactionService.RemoveReaction(reactionId, userId);
        if (result == null)
            return BadRequest(new { Message = "Failed to remove reaction" });
        return Ok(new { Message = result });
    }

    [HttpGet]
    public async Task<ActionResult<List<ReactionSimpleDto>>> GetMessageReactions([FromQuery] Guid messageId)
    {
        var result = await _reactionService.GetMessageReactions(messageId);
        if (result == null)
            return BadRequest(new { Message = "No reactions found" });
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<ReactionResponseDto>> GetUserReaction([FromQuery] Guid messageId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _reactionService.GetUserReaction(userId, messageId);
        if (result == null)
            return BadRequest(new { Message = "No reaction found" });
        return Ok(result);
    }
}