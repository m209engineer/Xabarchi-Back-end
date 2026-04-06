using Xabarchi.Domain.DTOs;
using Xabarchi.Domain.ResponseDTOs;

namespace Xabarchi.Application.Abstractions;

public interface IMessageService
{
    Task<string> SendMessage(Guid userId, MessageDto dto);
    Task<string> EditMessage(Guid messageId, Guid userId, MessageDto model);
    Task<string> ReplyToMessage(Guid userId, Guid replyToMessageId, MessageDto dto);
    Task<string> DeleteMessage(Guid messageId, Guid userId);
    Task<string> MarkAsRead(Guid messageId, Guid userId);

    Task<string> AddMedia(Guid messageId, Guid userId, AddMediaDto dto);
    Task<string> DeleteMedia(Guid messageId, Guid userId, Guid mediaId);

    Task<MessageResponseDto> GetMessageById(Guid messageId);
    Task<List<MessageResponseDto>> GetChatMessages(Guid chatId, int page, int pageSize);
    Task<List<MessageResponseDto>> GetMessageReplies(Guid messageId);
    Task<List<MessageResponseDto>> GetUnreadMessages(Guid userId, Guid chatId);
    Task<int> GetUnreadCount(Guid userId, Guid chatId);
}