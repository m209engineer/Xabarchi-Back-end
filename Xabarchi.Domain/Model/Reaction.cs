namespace Xabarchi.Domain.Model;

public class Reaction
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid MessageId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Message Message { get; set; } = null!;
}