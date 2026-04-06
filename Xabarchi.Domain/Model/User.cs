namespace Xabarchi.Domain.Model;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? Birthday { get; set; }

    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<Chatmember> Chatmembers { get; set; } = new();
    public List<Message> SentMessages { get; set; } = new();
}