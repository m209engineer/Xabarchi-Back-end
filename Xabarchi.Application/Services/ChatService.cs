using Microsoft.EntityFrameworkCore;
using Xabarchi.Application.Abstractions;
using Xabarchi.Domain.DTOs;
using Xabarchi.Domain.Model;
using Xabarchi.Domain.ResponseDTOs;
using Xabarchi.Domain.SimpleDTOs;
using Xabarchi.Infrastructure.Persistance;

namespace Xabarchi.Application.Services;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;
    private readonly IChatHubService _hubService;

    public ChatService(ApplicationDbContext context, IChatHubService hubService)
    {
        _context = context;
        _hubService = hubService;
    }

    public async Task<string> CreateChat(Guid userId, ChatDto dto)
    {
        var targetUser = await _context.Users.FindAsync(dto.TargetUserId);
        if (targetUser == null)
            throw new Exception("Target user not found");

        if (userId == dto.TargetUserId)
            throw new Exception("Cannot create chat with yourself");

        var existingChat = await _context.Chats
            .Include(c => c.Members)
            .Where(c => c.Members.Any(m => m.UserId == userId) &&
                        c.Members.Any(m => m.UserId == dto.TargetUserId))
            .FirstOrDefaultAsync();

        if (existingChat != null)
            throw new Exception("Chat already exists");

        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        var chatMembers = new List<Chatmember>
        {
            new Chatmember { Id = Guid.NewGuid(), ChatId = chat.Id, UserId = userId, JoinedAt = DateTime.UtcNow },
            new Chatmember { Id = Guid.NewGuid(), ChatId = chat.Id, UserId = dto.TargetUserId, JoinedAt = DateTime.UtcNow }
        };

        _context.Chatmembers.AddRange(chatMembers);
        await _context.SaveChangesAsync();

        var chatDto = await GetChatById(chat.Id);
        await _hubService.NewChat(dto.TargetUserId.ToString(), chatDto);

        return "Chat created successfully";
    }

    public async Task<string> DeleteChat(Guid chatId, Guid userId)
    {
        var chat = await _context.Chats
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            throw new Exception("Chat not found");

        var isMember = chat.Members.Any(m => m.UserId == userId);
        if (!isMember)
            throw new Exception("You are not a member of this chat");

        _context.Chats.Remove(chat);
        await _context.SaveChangesAsync();

        await _hubService.ChatDeleted(chatId.ToString(), new { ChatId = chatId });

        return "Chat deleted successfully";
    }

    public async Task<ChatResponseDto> GetChatById(Guid chatId)
    {
        var chat = await _context.Chats
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(50))
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat == null)
            throw new Exception("Chat not found");

        return MapToResponseDto(chat);
    }

    public async Task<List<ChatResponseDto>> GetUserChats(Guid userId)
    {
        var chatIds = await _context.Chatmembers
            .Where(cm => cm.UserId == userId)
            .Select(cm => cm.ChatId)
            .ToListAsync();

        var chats = await _context.Chats
            .Where(c => chatIds.Contains(c.Id))
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .ThenInclude(m => m.Sender)
            .OrderByDescending(c => c.Messages.Max(m => (DateTime?)m.SentAt) ?? c.CreatedAt)
            .ToListAsync();

        return chats.Select(MapToResponseDto).ToList();
    }

    public async Task<ChatResponseDto> GetOrCreateChat(Guid userId, Guid targetUserId)
    {
        var existingChat = await _context.Chats
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(50))
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c =>
                c.Members.Any(m => m.UserId == userId) &&
                c.Members.Any(m => m.UserId == targetUserId));

        if (existingChat != null)
            return MapToResponseDto(existingChat);

        await CreateChat(userId, new ChatDto { TargetUserId = targetUserId });

        var newChat = await _context.Chats
            .Include(c => c.Members)
                .ThenInclude(m => m.User)
            .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c =>
                c.Members.Any(m => m.UserId == userId) &&
                c.Members.Any(m => m.UserId == targetUserId));

        return MapToResponseDto(newChat!);
    }

    public async Task<bool> IsChatMember(Guid chatId, Guid userId)
    {
        return await _context.Chatmembers
            .AnyAsync(cm => cm.ChatId == chatId && cm.UserId == userId);
    }

    private ChatResponseDto MapToResponseDto(Chat chat)
    {
        return new ChatResponseDto
        {
            Id = chat.Id,
            CreatedAt = chat.CreatedAt,
            Members = chat.Members.Select(m => new ChatmemberSimpleDto
            {
                Id = m.Id,
                JoinedAt = m.JoinedAt,
                User = new UserSimpleDto
                {
                    Id = m.User.Id,
                    Firstname = m.User.Firstname,
                    Lastname = m.User.Lastname,
                    Username = m.User.Username,
                    AvatarUrl = m.User.AvatarUrl,
                    IsOnline = m.User.IsOnline,
                    LastSeen = m.User.LastSeen
                }
            }).ToList(),
            Messages = chat.Messages.Select(m => new MessageSimpleDto
            {
                Id = m.Id,
                Type = m.Type,
                Content = m.Content,
                SentAt = m.SentAt,
                Sender = new UserSimpleDto
                {
                    Id = m.Sender.Id,
                    Firstname = m.Sender.Firstname,
                    Lastname = m.Sender.Lastname,
                    Username = m.Sender.Username,
                    AvatarUrl = m.Sender.AvatarUrl,
                    IsOnline = m.Sender.IsOnline
                }
            }).OrderByDescending(m => m.SentAt).ToList()
        };
    }
}