using Microsoft.EntityFrameworkCore;
using Xabarchi.Application.Abstractions;
using Xabarchi.Domain.DTOs;
using Xabarchi.Domain.Model;
using Xabarchi.Domain.ResponseDTOs;
using Xabarchi.Domain.SimpleDTOs;
using Xabarchi.Infrastructure.Persistance;

namespace Xabarchi.Application.Services;

public class ReactionService : IReactionService
{
    private readonly ApplicationDbContext _context;
    private readonly IChatHubService _hubService;  

    public ReactionService(ApplicationDbContext context, IChatHubService hubService)
    {
        _context = context;
        _hubService = hubService;
    }

    public async Task<string> AddReaction(Guid userId, ReactionDto dto)
    {
        var message = await _context.Messages.FindAsync(dto.MessageId);
        if (message == null)
            throw new Exception("Message not found");

        var existingReaction = await _context.Reactions
            .FirstOrDefaultAsync(r => r.UserId == userId 
                                      && r.MessageId == dto.MessageId 
                                      && r.Content == dto.Content);

        if (existingReaction != null)
            return existingReaction.Id.ToString(); 

        var reaction = new Reaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            MessageId = dto.MessageId,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reactions.Add(reaction);
        await _context.SaveChangesAsync();

        var reactions = await GetMessageReactions(dto.MessageId);
        await _hubService.ReactionsUpdated(message.ChatId.ToString(), 
            new { MessageId = dto.MessageId, Reactions = reactions });

        return reaction.Id.ToString();
    }

    public async Task<string> RemoveReaction(Guid reactionId, Guid userId)
    {
        var reaction = await _context.Reactions
            .FirstOrDefaultAsync(r => r.Id == reactionId);

        if (reaction == null)
            throw new Exception("Reaction not found");

        if (reaction.UserId != userId)
            throw new Exception("You can only remove your own reactions");

        var message = await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == reaction.MessageId);

        if (message == null)
            throw new Exception("Message not found");

        var chatId = message.ChatId;
        var messageId = reaction.MessageId;

        _context.Reactions.Remove(reaction);
        await _context.SaveChangesAsync();

        var reactions = await GetMessageReactions(messageId);
        await _hubService.ReactionsUpdated(chatId.ToString(), new { MessageId = messageId, Reactions = reactions });

        return "Reaction removed successfully";
    }

    public async Task<List<ReactionSimpleDto>> GetMessageReactions(Guid messageId)
    {
        var reactions = await _context.Reactions
            .Where(r => r.MessageId == messageId)
            .Include(r => r.User)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

        return reactions.Select(r => new ReactionSimpleDto
        {
            Id = r.Id,
            Content = r.Content,
            CreatedAt = r.CreatedAt,
            User = new UserSimpleDto
            {
                Id = r.User.Id,
                Firstname = r.User.Firstname,
                Lastname =  r.User.Lastname,
                Username = r.User.Username,
                AvatarUrl = r.User.AvatarUrl,
                IsOnline = r.User.IsOnline,
                LastSeen = r.User.LastSeen
            }
        }).ToList();
    }

    public async Task<ReactionResponseDto?> GetUserReaction(Guid userId, Guid messageId)
    {
        var reaction = await _context.Reactions
            .Include(r => r.User)
            .Include(r => r.Message)
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.MessageId == messageId);

        if (reaction == null)
            return null;

        return new ReactionResponseDto
        {
            Id = reaction.Id,
            Content = reaction.Content,
            CreatedAt = reaction.CreatedAt,
            User = new UserSimpleDto
            {
                Id = reaction.User.Id,
                Firstname = reaction.User.Firstname,
                Lastname = reaction.User.Lastname,
                Username = reaction.User.Username,
                AvatarUrl = reaction.User.AvatarUrl,
                IsOnline = reaction.User.IsOnline,
                LastSeen = reaction.User.LastSeen
            },
            Message = new MessageSimpleDto
            {
                Id = reaction.Message.Id,
                Type = reaction.Message.Type,
                Content = reaction.Message.Content,
                SentAt = reaction.Message.SentAt,
                Sender = new UserSimpleDto
                {
                    Id = reaction.Message.Sender.Id,
                    Firstname = reaction.Message.Sender.Firstname,
                    Lastname = reaction.Message.Sender.Lastname,
                    Username = reaction.Message.Sender.Username,
                    AvatarUrl = reaction.Message.Sender.AvatarUrl,
                    IsOnline = reaction.Message.Sender.IsOnline
                }
            }
        };
    }
}