using Microsoft.AspNetCore.SignalR;
using Xabarchi.Api.Hubs;
using Xabarchi.Application.Abstractions;

namespace Xabarchi.Api.Services;

public class ChatHubService : IChatHubService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatHubService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessage(string chatId, object message)
        => await _hubContext.Clients.Group(chatId).SendAsync("ReceiveMessage", message);

    public async Task MessageEdited(string chatId, object message)
        => await _hubContext.Clients.Group(chatId).SendAsync("MessageEdited", message);

    public async Task MessageDeleted(string chatId, object data)
        => await _hubContext.Clients.Group(chatId).SendAsync("MessageDeleted", data);

    public async Task MessageRead(string chatId, object data)
        => await _hubContext.Clients.Group(chatId).SendAsync("MessageRead", data);

    public async Task ReactionsUpdated(string chatId, object data)
        => await _hubContext.Clients.Group(chatId).SendAsync("ReactionsUpdated", data);

    public async Task UserStatusChanged(object data)
        => await _hubContext.Clients.All.SendAsync("UserStatusChanged", data);

    public async Task NewChat(string userId, object chat)
        => await _hubContext.Clients.User(userId).SendAsync("NewChat", chat);

    public async Task ChatDeleted(string chatId, object data)
        => await _hubContext.Clients.Group(chatId).SendAsync("ChatDeleted", data);
}