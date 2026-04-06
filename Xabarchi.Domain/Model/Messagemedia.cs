namespace Xabarchi.Domain.Model;



public class Messagemedia
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public MediaType Type { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;  
    public string? FileName { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; }
    public DateTime CreatedAt { get; set; }

    public Message Message { get; set; } = null!;
}

public enum MediaType
{
    Image = 1,
    Video = 2,
    Audio = 3,
    VoiceMessage = 4,
    File = 5
}