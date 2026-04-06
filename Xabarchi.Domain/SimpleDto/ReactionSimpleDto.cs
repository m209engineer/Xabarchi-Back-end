

namespace Xabarchi.Domain.SimpleDTOs;

public class ReactionSimpleDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public UserSimpleDto User { get; set; } = null!;
}