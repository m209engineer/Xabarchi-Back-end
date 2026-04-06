namespace Xabarchi.Domain.Model;

public class Message
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }
    public MessageType Type { get; set; }
    public string? Content { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    public DateTime SentAt { get; set; }

   
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    
    public bool IsEdited { get; set; }              
    public DateTime? EditedAt { get; set; }  

    public Chat Chat { get; set; } = null!;
    public User Sender { get; set; } = null!;
    public Message? ReplyToMessage { get; set; }
    public List<Message> Replies { get; set; } = new();
    public List<Messagemedia> MediaFiles { get; set; } = new();
    public List<Reaction> Reactions { get; set; } = new();
}

public enum MessageType
{
    Text = 1,
    Image = 2,
    Video = 3,
    Audio = 4,
    VoiceMessage = 5,
    File = 6
}