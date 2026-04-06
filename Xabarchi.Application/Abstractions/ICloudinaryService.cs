using Microsoft.AspNetCore.Http;
using Xabarchi.Application.DTOs;

namespace Xabarchi.Application.Abstractions;

public interface ICloudinaryService
{
    // Images
    Task<CloudinaryUploadResult> UploadImage(IFormFile file);
    Task<CloudinaryUploadResult> UploadAvatar(IFormFile file);
    
    // Videos
    Task<CloudinaryUploadResult> UploadVideo(IFormFile file);
    
    // Audio
    Task<CloudinaryUploadResult> UploadAudio(IFormFile file);
    Task<CloudinaryUploadResult> UploadVoiceMessage(IFormFile file);
    
    // Files
    Task<CloudinaryUploadResult> UploadFile(IFormFile file);
    
    // Delete
    Task<bool> Delete(string publicId);
}