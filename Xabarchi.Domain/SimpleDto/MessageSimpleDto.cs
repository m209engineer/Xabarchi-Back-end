using Xabarchi.Domain.Model;


namespace Xabarchi.Domain.SimpleDTOs;

public class MessageSimpleDto
{
    public Guid Id { get; set; }
    public MessageType Type { get; set; }
    public string? Content { get; set; }
    public DateTime SentAt { get; set; }
    public UserSimpleDto Sender { get; set; } = null!;
}