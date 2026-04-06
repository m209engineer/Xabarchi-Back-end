

namespace Xabarchi.Domain.SimpleDTOs;

public class ChatmemberSimpleDto
{
    public Guid Id { get; set; }
    public DateTime JoinedAt { get; set; }
    public UserSimpleDto User { get; set; } = null!;
}