using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Xabarchi.Application.Abstractions;
using Xabarchi.Application.DTOs;

namespace Xabarchi.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly HttpClient _httpClient;
    private readonly string _bucketName;
    private readonly string _baseUrl;

    public CloudinaryService(IConfiguration config)
    {
        var url = config["Supabase:Url"]!;
        var serviceKey = config["Supabase:ServiceKey"]!;
        _bucketName = config["Supabase:BucketName"]!;
        _baseUrl = $"{url}/storage/v1/object";

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {serviceKey}");
        _httpClient.DefaultRequestHeaders.Add("apikey", serviceKey);
    }

    public async Task<CloudinaryUploadResult> UploadImage(IFormFile file)
        => await UploadToSupabase(file, "images");

    public async Task<CloudinaryUploadResult> UploadAvatar(IFormFile file)
        => await UploadToSupabase(file, "avatars");

    public async Task<CloudinaryUploadResult> UploadVideo(IFormFile file)
        => await UploadToSupabase(file, "videos");

    public async Task<CloudinaryUploadResult> UploadAudio(IFormFile file)
        => await UploadToSupabase(file, "audio");

    public async Task<CloudinaryUploadResult> UploadVoiceMessage(IFormFile file)
        => await UploadToSupabase(file, "voice");

    public async Task<CloudinaryUploadResult> UploadFile(IFormFile file)
        => await UploadToSupabase(file, "files");

    public async Task<bool> Delete(string publicId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(
                $"{_baseUrl}/{_bucketName}/{publicId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task<CloudinaryUploadResult> UploadToSupabase(IFormFile file, string folder)
    {
        var fileId = Guid.NewGuid().ToString();
        var extension = Path.GetExtension(file.FileName);
        var objectPath = $"{folder}/{fileId}{extension}";

        using var stream = file.OpenReadStream();
        using var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        var response = await _httpClient.PostAsync(
            $"{_baseUrl}/{_bucketName}/{objectPath}",
            content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Supabase upload failed: {error}");
        }

        // Public URL
        var url = $"{_baseUrl.Replace("/object", "/object/public")}/{_bucketName}/{objectPath}";

        return new CloudinaryUploadResult
        {
            Url = url,
            PublicId = objectPath,
            FileSize = file.Length,
            Format = extension.TrimStart('.')
        };
    }
}