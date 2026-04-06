namespace Xabarchi.Domain.DTOs;

public class ReactionDto
{
    public Guid MessageId { get; set; }
    public string Content { get; set; } = string.Empty;
}