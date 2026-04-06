using Xabarchi.Domain.SimpleDTOs;

namespace Xabarchi.Domain.ResponseDTOs;

public class ChatResponseDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<ChatmemberSimpleDto> Members { get; set; } = new();
    public List<MessageSimpleDto> Messages { get; set; } = new();
}