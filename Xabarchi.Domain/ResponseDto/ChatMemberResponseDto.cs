
using Xabarchi.Domain.SimpleDTOs;

namespace Xabarchi.Domain.ResponseDTOs;

public class ChatmemberResponseDto
{
    public Guid Id { get; set; }
    public DateTime JoinedAt { get; set; }
    
    public ChatSimpleDto Chat { get; set; } = null!;
    public UserSimpleDto User { get; set; } = null!;
}