using Xabarchi.Domain.DTOs;
using Xabarchi.Domain.ResponseDTOs;
using Xabarchi.Domain.SimpleDTOs;

namespace Xabarchi.Application.Abstractions;

public interface IReactionService
{
    Task<string> AddReaction(Guid userId, ReactionDto dto);           
    Task<string> RemoveReaction(Guid reactionId, Guid userId);       
    
    // Get
    Task<List<ReactionSimpleDto>> GetMessageReactions(Guid messageId);           
    Task<ReactionResponseDto?> GetUserReaction(Guid userId, Guid messageId);     
}