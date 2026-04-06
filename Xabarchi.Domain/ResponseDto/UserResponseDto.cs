using Xabarchi.Domain.SimpleDTOs;

namespace Xabarchi.Domain.ResponseDTOs;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
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

    public List<ChatmemberSimpleDto> Chatmembers { get; set; } = new();
}