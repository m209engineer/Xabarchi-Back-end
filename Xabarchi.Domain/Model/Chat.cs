namespace Xabarchi.Domain.Model;

public class Chat
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<Chatmember> Members { get; set; } = new();
    public List<Message> Messages { get; set; } = new();
}