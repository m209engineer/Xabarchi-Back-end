namespace Xabarchi.Application.DTOs;

public class CloudinaryUploadResult
{
    public string Url { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? Duration { get; set; }
    public long FileSize { get; set; }
    public string Format { get; set; } = string.Empty;
}