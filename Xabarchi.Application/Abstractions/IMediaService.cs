using Microsoft.AspNetCore.Http;
using Xabarchi.Domain.ResponseDTOs;

namespace Xabarchi.Application.Abstractions;

public interface IMediaService
{
    Task<MessagemediaResponseDto> UploadImage(IFormFile file);
    Task<MessagemediaResponseDto> UploadVideo(IFormFile file);
    Task<MessagemediaResponseDto> UploadAudio(IFormFile file);
    Task<MessagemediaResponseDto> UploadVoiceMessage(IFormFile file);
    Task<MessagemediaResponseDto> UploadFile(IFormFile file);
    
    // Get
    Task<MessagemediaResponseDto> GetMediaById(Guid mediaId);
    Task<List<MessagemediaResponseDto>> GetMessageMedia(Guid messageId);
    
    // Delete
    Task<string> DeleteMedia(Guid mediaId);
}