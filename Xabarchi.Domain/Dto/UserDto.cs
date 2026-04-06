using Microsoft.AspNetCore.Http;

namespace Xabarchi.Domain.DTOs;

public class UserDto
{
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Bio { get; set; }
    public IFormFile? AvatarFile { get; set; }
    public DateTime? Birthday { get; set; }
}