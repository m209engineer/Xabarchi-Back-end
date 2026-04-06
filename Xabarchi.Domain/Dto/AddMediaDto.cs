using Microsoft.AspNetCore.Http;
using Xabarchi.Domain.Model;

namespace Xabarchi.Domain.DTOs;

public class AddMediaDto
{
    public List<IFormFile> Files { get; set; } = new();
    
}