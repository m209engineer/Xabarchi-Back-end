using Microsoft.AspNetCore.Http;
using Xabarchi.Domain.Model;

namespace Xabarchi.Domain.DTOs;

public class MessageDto
{
    public Guid ChatId { get; set; }
    public MessageType Type { get; set; }
    public string? Content { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    public List<IFormFile>? MediaFiles { get; set; }  
}