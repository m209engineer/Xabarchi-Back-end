using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Xabarchi.Application.Abstractions;
using Xabarchi.Domain.Model;
using Xabarchi.Domain.ResponseDTOs;
using Xabarchi.Infrastructure.Persistance;

namespace Xabarchi.Application.Services;

public class MediaService : IMediaService
{
    private readonly ApplicationDbContext _context;
    private readonly ICloudinaryService _cloudinaryService;

    public MediaService(ApplicationDbContext context, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<MessagemediaResponseDto> UploadImage(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new Exception("File is empty");
        }

        if (file.Length > 10_000_000) // 10MB
        {
            throw new Exception("File too large (max 10MB)");
        }

        var uploadResult = await _cloudinaryService.UploadImage(file);

        var media = new Messagemedia
        {
            Id = Guid.NewGuid(),
            MessageId = Guid.Empty,
            Type = MediaType.Image,
            FileUrl = uploadResult.Url,
            PublicId = uploadResult.PublicId,  
            FileName = file.FileName,
            FileSize = file.Length,
            MimeType = file.ContentType,
            ThumbnailUrl = uploadResult.ThumbnailUrl,
            Width = uploadResult.Width,
            Height = uploadResult.Height,
            CreatedAt = DateTime.UtcNow
        };

        return MapToResponseDto(media);
    }

    public async Task<MessagemediaResponseDto> UploadVideo(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new Exception("File is empty");
        }

        if (file.Length > 50_000_000) // 50MB
        {
            throw new Exception("Video too large (max 50MB)");
        }

        var uploadResult = await _cloudinaryService.UploadVideo(file);

        var media = new Messagemedia
        {
            Id = Guid.NewGuid(),
            MessageId = Guid.Empty,
            Type = MediaType.Video,
            FileUrl = uploadResult.Url,
            PublicId = uploadResult.PublicId,  
            FileName = file.FileName,
            FileSize = file.Length,
            MimeType = file.ContentType,
            ThumbnailUrl = uploadResult.ThumbnailUrl,
            Width = uploadResult.Width,
            Height = uploadResult.Height,
            Duration = uploadResult.Duration,
            CreatedAt = DateTime.UtcNow
        };

        return MapToResponseDto(media);
    }

    public async Task<MessagemediaResponseDto> UploadAudio(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new Exception("File is empty");
        }

        if (file.Length > 20_000_000) // 20MB
        {
            throw new Exception("Audio too large (max 20MB)");
        }

        var uploadResult = await _cloudinaryService.UploadAudio(file);

        var media = new Messagemedia
        {
            Id = Guid.NewGuid(),
            MessageId = Guid.Empty,
            Type = MediaType.Audio,
            FileUrl = uploadResult.Url,
            PublicId = uploadResult.PublicId,  
            FileName = file.FileName,
            FileSize = file.Length,
            MimeType = file.ContentType,
            Duration = uploadResult.Duration,
            CreatedAt = DateTime.UtcNow
        };

        return MapToResponseDto(media);
    }

    public async Task<MessagemediaResponseDto> UploadVoiceMessage(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new Exception("File is empty");
        }

        if (file.Length > 5_000_000) // 5MB
        {
            throw new Exception("Voice message too large (max 5MB)");
        }

        var uploadResult = await _cloudinaryService.UploadVoiceMessage(file);

        var media = new Messagemedia
        {
            Id = Guid.NewGuid(),
            MessageId = Guid.Empty,
            Type = MediaType.VoiceMessage,
            FileUrl = uploadResult.Url,
            PublicId = uploadResult.PublicId,  
            FileName = file.FileName,
            FileSize = file.Length,
            MimeType = file.ContentType,
            Duration = uploadResult.Duration,
            CreatedAt = DateTime.UtcNow
        };

        return MapToResponseDto(media);
    }

    public async Task<MessagemediaResponseDto> UploadFile(IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new Exception("File is empty");
        }

        if (file.Length > 20_000_000) // 20MB
        {
            throw new Exception("File too large (max 20MB)");
        }

        var uploadResult = await _cloudinaryService.UploadFile(file);

        var media = new Messagemedia
        {
            Id = Guid.NewGuid(),
            MessageId = Guid.Empty,
            Type = MediaType.File,
            FileUrl = uploadResult.Url,
            PublicId = uploadResult.PublicId, 
            FileName = file.FileName,
            FileSize = file.Length,
            MimeType = file.ContentType,
            CreatedAt = DateTime.UtcNow
        };

        return MapToResponseDto(media);
    }

    public async Task<MessagemediaResponseDto> GetMediaById(Guid mediaId)
    {
        var media = await _context.Messagemedia.FindAsync(mediaId);

        if (media == null)
        {
            throw new Exception("Media not found");
        }

        return MapToResponseDto(media);
    }

    public async Task<List<MessagemediaResponseDto>> GetMessageMedia(Guid messageId)
    {
        var mediaList = await _context.Messagemedia
            .Where(m => m.MessageId == messageId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        return mediaList.Select(MapToResponseDto).ToList();
    }

    public async Task<string> DeleteMedia(Guid mediaId)
    {
        var media = await _context.Messagemedia.FindAsync(mediaId);

        if (media == null)
        {
            throw new Exception("Media not found");
        }

        // Cloudinary dan o'chirish
        var deleted = await _cloudinaryService.Delete(media.PublicId);

        if (!deleted)
        {
            throw new Exception("Failed to delete from Cloudinary");
        }

        // Database dan o'chirish
        _context.Messagemedia.Remove(media);
        await _context.SaveChangesAsync();

        return "Media deleted successfully";
    }

    // Helper
    private MessagemediaResponseDto MapToResponseDto(Messagemedia media)
    {
        return new MessagemediaResponseDto
        {
            Id = media.Id,
            Type = media.Type,
            FileUrl = media.FileUrl,
            PublicId = media.PublicId, 
            FileName = media.FileName,
            FileSize = media.FileSize,
            MimeType = media.MimeType,
            ThumbnailUrl = media.ThumbnailUrl,
            Width = media.Width,
            Height = media.Height,
            Duration = media.Duration,
            CreatedAt = media.CreatedAt
        };
    }
}