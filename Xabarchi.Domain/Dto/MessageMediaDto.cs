using Microsoft.AspNetCore.Http;
using Xabarchi.Domain.Model;

namespace Xabarchi.Domain.DTOs;

public class MessagemediaDto
{
    public Guid MessageId { get; set; }
    public MediaType Type { get; set; }
    public IFormFile? File { get; set; }
}