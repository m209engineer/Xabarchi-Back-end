using Xabarchi.Domain.Model;

using Xabarchi.Domain.SimpleDTOs;

namespace Xabarchi.Domain.ResponseDTOs;

public class MessageResponseDto
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public MessageType Type { get; set; }
    public string? Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsEdited { get; set; }      
    public DateTime? EditedAt { get; set; } 
    
    public UserSimpleDto Sender { get; set; } = null!;
    public MessageSimpleDto? ReplyToMessage { get; set; }  
    public List<MessagemediaResponseDto> MediaFiles { get; set; } = new();
    public List<ReactionSimpleDto> Reactions { get; set; } = new();
}