using Xabarchi.Domain.DTOs;
using Xabarchi.Domain.ResponseDTOs;

namespace Xabarchi.Application.Abstractions;

public interface IChatService
{
    Task<string> CreateChat(Guid userId, ChatDto dto);
    Task<string> DeleteChat(Guid chatId, Guid userId);
    

    Task<ChatResponseDto> GetChatById(Guid chatId);
    Task<List<ChatResponseDto>> GetUserChats(Guid userId);
    
    Task<ChatResponseDto> GetOrCreateChat(Guid userId, Guid targetUserId);
    Task<bool> IsChatMember(Guid chatId, Guid userId);
}