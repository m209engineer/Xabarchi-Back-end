
using Xabarchi.Domain.SimpleDTOs;


namespace Xabarchi.Domain.ResponseDTOs;

public class ReactionResponseDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    public UserSimpleDto User { get; set; } = null!;
    public MessageSimpleDto Message { get; set; } = null!;
}