
using Xabarchi.Domain.SimpleDTOs;

namespace Xabarchi.Domain.SimpleDTOs;

public class ChatSimpleDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserSimpleDto OtherUser { get; set; } = null!;  
    public MessageSimpleDto? LastMessage { get; set; }    
}
