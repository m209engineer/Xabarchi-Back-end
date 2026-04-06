using Microsoft.EntityFrameworkCore;
using Xabarchi.Application.Abstractions;
using Xabarchi.Domain.DTOs;
using Xabarchi.Domain.Model;
using Xabarchi.Domain.ResponseDTOs;
using Xabarchi.Domain.SimpleDTOs;
using Xabarchi.Infrastructure.Persistance;

namespace Xabarchi.Application.Services;

public class MessageService : IMessageService
{
    private readonly ApplicationDbContext _context;
    private readonly IMediaService _mediaService;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IChatHubService _hubService;  

    public MessageService(ApplicationDbContext context, IMediaService mediaService, IChatHubService hubService, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _mediaService = mediaService;
        _hubService = hubService;  
        _cloudinaryService = cloudinaryService;
    }

    public async Task<string> SendMessage(Guid userId, MessageDto dto)
    {
        var isMember = await _context.Chatmembers
            .AnyAsync(cm => cm.ChatId == dto.ChatId && cm.UserId == userId);

        if (!isMember)
            throw new Exception("You are not a member of this chat");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = dto.ChatId,
            SenderId = userId,
            Type = dto.Type,
            Content = dto.Content,
            ReplyToMessageId = dto.ReplyToMessageId,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        if (dto.MediaFiles != null && dto.MediaFiles.Any())
        {
            foreach (var file in dto.MediaFiles)
            {
                MessagemediaResponseDto mediaResult = dto.Type switch
                {
                    MessageType.Image => await _mediaService.UploadImage(file),
                    MessageType.Video => await _mediaService.UploadVideo(file),
                    MessageType.Audio => await _mediaService.UploadAudio(file),
                    MessageType.VoiceMessage => await _mediaService.UploadVoiceMessage(file),
                    MessageType.File => await _mediaService.UploadFile(file),
                    _ => throw new Exception("Invalid message type")
                };

                var media = new Messagemedia
                {
                    Id = mediaResult.Id,
                    MessageId = message.Id,
                    Type = (MediaType)(int)dto.Type,
                    FileUrl = mediaResult.FileUrl,
                    PublicId = mediaResult.PublicId,
                    FileName = mediaResult.FileName,
                    FileSize = mediaResult.FileSize,
                    MimeType = mediaResult.MimeType,
                    ThumbnailUrl = mediaResult.ThumbnailUrl,
                    Width = mediaResult.Width,
                    Height = mediaResult.Height,
                    Duration = mediaResult.Duration,
                    CreatedAt = mediaResult.CreatedAt
                };

                _context.Messagemedia.Add(media);
            }

            await _context.SaveChangesAsync();
        }

        var messageDto = await GetMessageById(message.Id);
        await _hubService.SendMessage(dto.ChatId.ToString(), messageDto); 

        return "Message sent successfully";
    }

    public async Task<string> EditMessage(Guid messageId, Guid userId, MessageDto model)
    {
        var message = await _context.Messages.FindAsync(messageId);

        if (message == null)
            throw new Exception("Message not found");

        if (message.SenderId != userId)
            throw new Exception("You can only edit your own messages");

        message.Content = model.Content;
        message.IsEdited = true;
        message.EditedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var messageDto = await GetMessageById(messageId);
        await _hubService.MessageEdited(message.ChatId.ToString(), messageDto);  
        return "Message edited successfully";
    }

    public async Task<string> ReplyToMessage(Guid userId, Guid replyToMessageId, MessageDto dto)
    {
        var replyToMessage = await _context.Messages.FindAsync(replyToMessageId);
        if (replyToMessage == null)
            throw new Exception("Reply message not found");

        dto.ReplyToMessageId = replyToMessageId;

        return await SendMessage(userId, dto);
    }

    public async Task<string> MarkAsRead(Guid messageId, Guid userId)
    {
        var message = await _context.Messages.FindAsync(messageId);

        if (message == null)
            throw new Exception("Message not found");

        if (message.SenderId == userId)
            return "Cannot mark your own message as read";

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _hubService.MessageRead(message.ChatId.ToString(), new { MessageId = messageId, ReadAt = message.ReadAt });

        return "Message marked as read";
    }

    public async Task<string> DeleteMessage(Guid messageId, Guid userId)
    {
        var message = await _context.Messages
            .Include(m => m.MediaFiles)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null)
            throw new Exception("Message not found");

        if (message.SenderId != userId)
            throw new Exception("You can only delete your own messages");

        var chatId = message.ChatId;

        foreach (var media in message.MediaFiles)
        {
            await _cloudinaryService.Delete(media.PublicId);
        }

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();

        await _hubService.MessageDeleted(chatId.ToString(), new { MessageId = messageId });

        return "Message deleted successfully";
    }

    public async Task<string> AddMedia(Guid messageId, Guid userId, AddMediaDto dto)
    {
        var message = await _context.Messages
            .Include(m => m.MediaFiles)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null)
            throw new Exception("Message not found");

        if (message.SenderId != userId)
            throw new Exception("You can only add media to your own messages");

        if (message.Type == MessageType.Text)
            throw new Exception("Cannot add media to text message");

        if (message.Type == MessageType.VoiceMessage)
            throw new Exception("Voice message can only have one media file");

        var expectedMediaType = (MediaType)(int)message.Type;
        foreach (var file in dto.Files)
        {
            var mimeType = file.ContentType.ToLower();

            var isValid = message.Type switch
            {
                MessageType.Image => mimeType.StartsWith("image/"),
                MessageType.Video => mimeType.StartsWith("video/"),
                MessageType.Audio => mimeType.StartsWith("audio/"),
                MessageType.File  => true,
                _ => false
            };

            if (!isValid)
                throw new Exception($"File type does not match message type '{message.Type}'");
        }

        foreach (var file in dto.Files)
        {
            MessagemediaResponseDto mediaResult = message.Type switch
            {
                MessageType.Image => await _mediaService.UploadImage(file),
                MessageType.Video => await _mediaService.UploadVideo(file),
                MessageType.Audio => await _mediaService.UploadAudio(file),
                MessageType.File  => await _mediaService.UploadFile(file),
                _ => throw new Exception("Invalid message type")
            };

            var media = new Messagemedia
            {
                Id = mediaResult.Id,
                MessageId = message.Id,
                Type = expectedMediaType,
                FileUrl = mediaResult.FileUrl,
                PublicId = mediaResult.PublicId,
                FileName = mediaResult.FileName,
                FileSize = mediaResult.FileSize,
                MimeType = mediaResult.MimeType,
                ThumbnailUrl = mediaResult.ThumbnailUrl,
                Width = mediaResult.Width,
                Height = mediaResult.Height,
                Duration = mediaResult.Duration,
                CreatedAt = mediaResult.CreatedAt
            };

            _context.Messagemedia.Add(media);
        }



        await _context.SaveChangesAsync();

        var messageDto = await GetMessageById(messageId);
        await _hubService.MessageEdited(message.ChatId.ToString(), messageDto);

        return "Media added successfully";
    }

    public async Task<string> DeleteMedia(Guid messageId, Guid userId, Guid mediaId)
    {
        var message = await _context.Messages
            .Include(m => m.MediaFiles)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null)
            throw new Exception("Message not found");

        if (message.SenderId != userId)
            throw new Exception("You can only delete media from your own messages");

        var media = message.MediaFiles.FirstOrDefault(m => m.Id == mediaId);
        if (media == null)
            throw new Exception("Media not found in this message");

        var chatId = message.ChatId;

        await _cloudinaryService.Delete(media.PublicId);

        if (message.MediaFiles.Count == 1)
        {
            if (!string.IsNullOrEmpty(message.Content))
            {
                _context.Messagemedia.Remove(media);
                message.Type = MessageType.Text;
                message.IsEdited = true;
                message.EditedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var messageDto = await GetMessageById(messageId);
                await _hubService.MessageEdited(chatId.ToString(), messageDto);

                return "Media deleted, message converted to text";
            }
            else
            {
                _context.Messagemedia.Remove(media);
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();

                await _hubService.MessageDeleted(chatId.ToString(), new { MessageId = messageId });

                return "Last media deleted, message removed";
            }
        }

        _context.Messagemedia.Remove(media);
        message.IsEdited = true;
        message.EditedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var updatedDto = await GetMessageById(messageId);
        await _hubService.MessageEdited(chatId.ToString(), updatedDto);

        return "Media deleted successfully";
    }

    public async Task<MessageResponseDto> GetMessageById(Guid messageId)
    {
        var message = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.ReplyToMessage)
                .ThenInclude(r => r.Sender)
            .Include(m => m.MediaFiles)
            .Include(m => m.Reactions)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(m => m.Id == messageId);

        if (message == null)
            throw new Exception("Message not found");

        return MapToResponseDto(message);
    }
    

    

    public async Task<List<MessageResponseDto>> GetChatMessages(Guid chatId, int page, int pageSize)
    {
        var messages = await _context.Messages
            .Where(m => m.ChatId == chatId)
            .Include(m => m.Sender)
            .Include(m => m.ReplyToMessage)
                .ThenInclude(r => r.Sender)
            .Include(m => m.MediaFiles)
            .Include(m => m.Reactions)
                .ThenInclude(r => r.User)
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return messages.Select(MapToResponseDto).ToList();
    }

    public async Task<List<MessageResponseDto>> GetMessageReplies(Guid messageId)
    {
        var replies = await _context.Messages
            .Where(m => m.ReplyToMessageId == messageId)
            .Include(m => m.Sender)
            .Include(m => m.MediaFiles)
            .Include(m => m.Reactions)
                .ThenInclude(r => r.User)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return replies.Select(MapToResponseDto).ToList();
    }

    public async Task<List<MessageResponseDto>> GetUnreadMessages(Guid userId, Guid chatId)
    {
        var messages = await _context.Messages
            .Where(m => m.ChatId == chatId && !m.IsRead && m.SenderId != userId)
            .Include(m => m.Sender)
            .Include(m => m.MediaFiles)
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return messages.Select(MapToResponseDto).ToList();
    }

    public async Task<int> GetUnreadCount(Guid userId, Guid chatId)
    {
        return await _context.Messages
            .Where(m => m.ChatId == chatId && !m.IsRead && m.SenderId != userId)
            .CountAsync();
    }

    private MessageResponseDto MapToResponseDto(Message message)
    {
        return new MessageResponseDto
        {
            Id = message.Id,
            ChatId = message.ChatId,
            Type = message.Type,
            Content = message.Content,
            IsRead = message.IsRead,
            ReadAt = message.ReadAt,
            SentAt = message.SentAt,
            IsEdited = message.IsEdited,
            EditedAt = message.EditedAt,
            Sender = new UserSimpleDto
            {
                Id = message.Sender.Id,
                Firstname = message.Sender.Firstname,
                Lastname = message.Sender.Lastname,
                Username = message.Sender.Username,
                AvatarUrl = message.Sender.AvatarUrl,
                IsOnline = message.Sender.IsOnline,
                LastSeen = message.Sender.LastSeen
            },
            ReplyToMessage = message.ReplyToMessage == null ? null : new MessageSimpleDto
            {
                Id = message.ReplyToMessage.Id,
                Type = message.ReplyToMessage.Type,
                Content = message.ReplyToMessage.Content,
                SentAt = message.ReplyToMessage.SentAt,
                Sender = new UserSimpleDto
                {
                    Id = message.ReplyToMessage.Sender.Id,
                    Firstname = message.ReplyToMessage.Sender.Firstname,
                    Lastname = message.ReplyToMessage.Sender.Lastname,
                    Username = message.ReplyToMessage.Sender.Username,
                    AvatarUrl = message.ReplyToMessage.Sender.AvatarUrl,
                    IsOnline = message.ReplyToMessage.Sender.IsOnline
                }
            },
            MediaFiles = message.MediaFiles.Select(m => new MessagemediaResponseDto
            {
                Id = m.Id,
                Type = m.Type,
                FileUrl = m.FileUrl,
                FileName = m.FileName,
                FileSize = m.FileSize,
                MimeType = m.MimeType,
                ThumbnailUrl = m.ThumbnailUrl,
                Width = m.Width,
                Height = m.Height,
                Duration = m.Duration,
                CreatedAt = m.CreatedAt
            }).ToList(),
            Reactions = message.Reactions.Select(r => new ReactionSimpleDto
            {
                Id = r.Id,
                Content = r.Content,
                CreatedAt = r.CreatedAt,
                User = new UserSimpleDto
                {
                    Id = r.User.Id,
                    Firstname = r.User.Firstname,
                    Lastname = r.User.Lastname,
                    Username = r.User.Username,
                    AvatarUrl = r.User.AvatarUrl,
                    IsOnline = r.User.IsOnline
                }
            }).ToList()
        };
    }
}